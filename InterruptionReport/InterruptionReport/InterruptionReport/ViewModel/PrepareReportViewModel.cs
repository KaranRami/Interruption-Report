using Acr.UserDialogs;
using InterruptionReport.Interface;
using InterruptionReport.Model;
using InterruptionReport.Model.DBModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace InterruptionReport.ViewModel
{
    public class PrepareReportViewModel : BaseViewModel
    {
        public PrepareReportViewModel(ContentPage view) : base(view)
        {
            Task.Run(async () =>
            {
                await ReadInputData();
            });
        }

        private async Task ReadInputData()
        {
            using (var stream = await FileSystem.OpenAppPackageFileAsync("Inputs.json"))
            {
                using (var reader = new StreamReader(stream))
                {
                    var jsonInputs = await reader.ReadToEndAsync();
                    if (jsonInputs == null)
                        throw new Exception("Unable to read files");
                    else
                    {
                        Subdivision subdivision = JsonConvert.DeserializeObject<Subdivision>(jsonInputs);
                        if (subdivision == null)
                            throw new Exception("Unable to read files");
                        else
                        {
                            var allFeederStation = new Substation()
                            {
                                ID = -1,
                                Name = "All Substations",
                                Feeders = new List<Feeder>(),
                            };
                            allFeederStation.Feeders.Insert(0, new Feeder()
                            {
                                ID = -1,
                                Name = "All Feeders",
                            });
                            foreach (var substation in subdivision.SubStations)
                            {
                                foreach (var feeder in substation.Feeders)
                                {
                                    allFeederStation.Feeders.Add(feeder);
                                }
                                substation.Feeders.Insert(0, new Feeder()
                                {
                                    ID = -1,
                                    Name = "All Feeders",
                                });
                            }
                            subdivision.SubStations.Insert(0, allFeederStation);
                            CurrentInterruption = new Interruption()
                            {
                                SubDivision = subdivision,
                                SubStation = subdivision.SubStations[0],
                                Feeder = subdivision.SubStations[0].Feeders[0],
                                ReportedDate = DateTime.Now,
                            };
                        }
                    }
                }
            }
        }
        private void SaveDataToDownloadFolder(string downloadPath, string csv)
        {
            if (File.Exists(downloadPath))
                File.Delete(downloadPath);
            if (!File.Exists(downloadPath))
            {
                var ds = File.Create(downloadPath);
                ds.Dispose();
                File.WriteAllText(downloadPath, csv);
            }
        }
        public static string ToCSV(DataTable table, string delimator = ",")
        {
            var result = new StringBuilder();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                result.Append(table.Columns[i].ColumnName);
                result.Append(i == table.Columns.Count - 1 ? "\n" : delimator);
            }
            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    result.Append(row[i].ToString());
                    result.Append(i == table.Columns.Count - 1 ? "\n" : delimator);
                }
            }
            return result.ToString().TrimEnd(new char[] { '\r', '\n' });
        }
        private bool Validate()
        {
            if (CurrentInterruption.SubDivision == null)
            {
                UserDialogs.Instance.Toast("Please Select Subdivision");
                return false;
            }
            else if (CurrentInterruption.SubStation == null)
            {
                UserDialogs.Instance.Toast("Please Select SubStation");
                return false;
            }
            else if (CurrentInterruption.Feeder == null)
            {
                UserDialogs.Instance.Toast("Please Select Feeder");
                return false;
            }
            else if (FromDate > ToDate)
            {
                UserDialogs.Instance.Toast("To Date can not be before from Date for Interruptions");
                return false;
            }
            else
            {
                return true;
            }
        }
        private async Task<List<InterruptionDbModel>> GetFilteredItems()
        {
            var data = await App.Database.GetItemsAsync(FromDate, ToDate, CurrentInterruption.SubStation.Name, CurrentInterruption.Feeder.Name, CurrentInterruption.InterruprionType);
            return data;

        }
        int tapCount = 0;
        public ICommand CustomQueryCommand { get { return new Command(async () => await CustomQueryCommandEvent()); } }
        private async Task CustomQueryCommandEvent()
        {
            tapCount++;
            if (tapCount <= 6)
                return;
            tapCount = 0;
            string query = await BaseContent.DisplayPromptAsync("Query Data", "Query Database for custom operation", "Apply", "Cancel", "Query");
            if (!string.IsNullOrEmpty(query))
            {
                try
                {
                    bool resilt = await App.Database.CustomQueryAsync(query);
                    UserDialogs.Instance.Toast("Query Successful");
                }
                catch (Exception ex)
                {
                    await BaseContent.DisplayAlert("Query Error", ex.Message, "Ok");
                }
            }
        }
        public ICommand ExportDatabaseCommand { get { return new Command(async () => await ExportDatabaseCommandEvent()); } }
        private async Task ExportDatabaseCommandEvent()
        {
            try
            {
                if (!Validate())
                    return;
                string filename = await BaseContent.DisplayPromptAsync("Report Name", "Enter Report name to save with", "Save", "Cancel", "Report Name");
                if (string.IsNullOrEmpty(filename))
                {
                    if (filename != null)
                        UserDialogs.Instance.Toast("Please enter valid report name");
                    return;
                }
                bool permissionGranted = await AskForPermission(new Permissions.StorageWrite());
                if (permissionGranted)
                {

                    string downloadPath = DependencyService.Get<IFileHelper>().GetLocalFilePath(filename.Trim() + ".csv");
                    if (!string.IsNullOrEmpty(downloadPath))
                    {
                        List<InterruptionDbModel> data = await GetFilteredItems();
                        double totalMinutes = 0;
                        List<InterruptionReportModel> items = new List<InterruptionReportModel>();
                        foreach (var item in data)
                        {
                            double timeDifference = item.ReportTimeTo.TotalMinutes - item.ReportTimeFrom.TotalMinutes;
                            if (timeDifference < 0)
                                timeDifference = (60 * 24) + timeDifference;
                            TimeSpan workMin = TimeSpan.FromMinutes(timeDifference);
                            string workHours = workMin.ToString(@"hh\:mm");
                            InterruptionReportModel interruptionReport = new InterruptionReportModel()
                            {
                                Comment = item.Comment,
                                Feeder = item.Feeder,
                                InterruprionType = item.InterruprionType,
                                ReportedDate = DateTime.Parse(item.ReportedDate).ToString("dd/MM/yyyy"),
                                ReportTimeFrom = item.ReportTimeFrom.ToString(),
                                ReportTimeTo = item.ReportTimeTo.ToString(),
                                SubDivision = item.SubDivision,
                                SubStation = item.SubStation,
                                Hours = workHours,
                            };
                            totalMinutes += timeDifference;
                            items.Add(interruptionReport);
                        }
                        TimeSpan totalWorkMin = TimeSpan.FromMinutes(totalMinutes);
                        string totalWorkHours = totalWorkMin.ToString(@"hh\:mm");
                        InterruptionReportModel totalHoursInterruption = new InterruptionReportModel()
                        {
                            Hours = totalWorkHours,
                        };
                        items.Add(totalHoursInterruption);
                        string jsonData = JsonConvert.SerializeObject(items);
                        if (!string.IsNullOrEmpty(jsonData))
                        {
                            XmlNode xml = JsonConvert.DeserializeXmlNode("{records:{record:" + jsonData + "}}");
                            XmlDocument xmldoc = new XmlDocument();
                            //Create XmlDoc Object
                            xmldoc.LoadXml(xml.InnerXml);
                            //Create XML Steam 
                            var xmlReader = new XmlNodeReader(xmldoc);
                            DataSet dataSet = new DataSet();
                            //Load Dataset with Xml
                            dataSet.ReadXml(xmlReader);
                            //return single table inside of dataset
                            string csv = ToCSV(dataSet.Tables[0]);
                            if (!string.IsNullOrEmpty(csv))
                            {
                                SaveDataToDownloadFolder(downloadPath, csv);
                                UserDialogs.Instance.Toast("Database saved to your download folder.");
                            }
                            else
                            {
                                await BaseContent.DisplayAlert("Export Database", "Unable to save data to storage.", "Ok");
                            }
                        }
                        else
                        {
                            await BaseContent.DisplayAlert("Export Database", "Unable to prepare report.", "Ok");
                        }
                    }
                    else
                    {
                        await BaseContent.DisplayAlert("Export Database", "Download path is invalid.", "Ok");
                    }
                }
                else
                {
                    await BaseContent.DisplayAlert("Export Database", "Allow permission to store data, you can chance permissions from settings anytime", "Ok");
                }
            }
            catch (Exception ex)
            {
                await BaseContent.DisplayAlert("Export Database", ex.Message, "Ok");
            }
        }
        public ICommand InterruprionTypeChangedCommand { get { return new Command<string>(InterruprionTypeChangedCommandEvent); } }
        private void InterruprionTypeChangedCommandEvent(string interruprionType)
        {
            if (CurrentInterruption.InterruprionType != interruprionType)
                CurrentInterruption.InterruprionType = interruprionType;
            else
                CurrentInterruption.InterruprionType = string.Empty;
        }

        private Interruption currentInterruption;
        public Interruption CurrentInterruption
        {
            get { return currentInterruption; }
            set { SetProperty(ref currentInterruption, value); }
        }

        private DateTime fromDate = System.DateTime.Now;
        public DateTime FromDate
        {
            get { return fromDate; }
            set { SetProperty(ref fromDate, value); }
        }

        private DateTime toDate = System.DateTime.Now;
        public DateTime ToDate
        {
            get { return toDate; }
            set { SetProperty(ref toDate, value); }
        }
    }
}
