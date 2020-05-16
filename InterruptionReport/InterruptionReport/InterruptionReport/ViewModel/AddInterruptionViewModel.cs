using Acr.UserDialogs;
using InterruptionReport.Interface;
using InterruptionReport.Model;
using InterruptionReport.Model.DBModel;
using InterruptionReport.View;
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
    public class AddInterruptionViewModel : BaseViewModel
    {
        public AddInterruptionViewModel(ContentPage view, long interruptionId) : base(view)
        {
            this.interruptionID = interruptionId;
            Task.Run(async () =>
            {
                await ReadInputData();
                if (interruptionId != 0)
                {
                    var interruptionDB = await App.Database.GetItemAsync(interruptionId);
                    CurrentInterruption.Comment = interruptionDB.Comment;
                    CurrentInterruption.SubStation = CurrentInterruption.SubDivision.SubStations.FirstOrDefault(s => s.Name == interruptionDB.SubStation);
                    CurrentInterruption.Feeder = CurrentInterruption.SubStation.Feeders.FirstOrDefault(f => f.Name == interruptionDB.Feeder);
                    CurrentInterruption.InterruprionType = interruptionDB.InterruprionType;
                    CurrentInterruption.ReportedDate = DateTime.Parse(interruptionDB.ReportedDate);
                    CurrentInterruption.ReportTimeFrom = interruptionDB.ReportTimeFrom;
                    CurrentInterruption.ReportTimeTo = interruptionDB.ReportTimeTo;
                }
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
                            CurrentInterruption = new Interruption()
                            {
                                SubDivision = subdivision,
                                ReportedDate = DateTime.Now,
                            };
                        }
                    }
                }
            }
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
            else if (string.IsNullOrEmpty(CurrentInterruption.InterruprionType))
            {
                UserDialogs.Instance.Toast("Please Select Interruprion Type");
                return false;
            }
            else if (!string.IsNullOrEmpty(CurrentInterruption.Comment) && CurrentInterruption.Comment.Contains(","))
            {
                UserDialogs.Instance.Toast("Comment should not contains ',' Please Remove before saving Interruption");
                return false;
            }
            else
            {
                return true;
            }


        }
        private void ResetInterruption()
        {
            CurrentInterruption.InterruprionType = string.Empty;
            CurrentInterruption.Comment = string.Empty;
            CurrentInterruption.ReportTimeFrom = new TimeSpan(0, 0, 0);
            CurrentInterruption.ReportTimeTo = new TimeSpan(0, 0, 0);
        }

        public ICommand SaveNewInterruptionCommand { get { return new Command(async () => await SaveNewInterruptionCommandEvent()); } }
        private async Task SaveNewInterruptionCommandEvent()
        {
            try
            {
                if (!Validate())
                    return;
                InterruptionDbModel interruptionDbModel = new InterruptionDbModel()
                {
                    Comment = CurrentInterruption.Comment,
                    Feeder = CurrentInterruption.Feeder.Name,
                    InterruprionType = CurrentInterruption.InterruprionType,
                    ReportedDate = CurrentInterruption.ReportedDate.ToString("yyyy-MM-dd"),
                    ReportTimeFrom = CurrentInterruption.ReportTimeFrom,
                    ReportTimeTo = CurrentInterruption.ReportTimeTo,
                    SubDivision = CurrentInterruption.SubDivision.Name,
                    SubStation = CurrentInterruption.SubStation.Name,
                };
                if (BaseContent.Title.ToLower() == "edit interruption")
                {
                    interruptionDbModel.ID = interruptionID;
                }
                await App.Database.SaveItemAsync(interruptionDbModel);
                if (BaseContent.Title.ToLower() == "edit interruption")
                {
                    await BaseContent.Navigation.PopAsync();
                }
                else
                {
                    ResetInterruption();
                    await BaseContent.DisplayAlert("Save", "Interruption Added successfully", "Ok");
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower() == "constraint")
                {
                    UserDialogs.Instance.Toast("Interruption already exist");
                }
                else
                {
                    await BaseContent.DisplayAlert("Save", ex.Message, "Ok");
                }
            }

        }
        public ICommand DeleteCommand { get { return new Command(async () => await DeleteCommandEvent()); } }
        private async Task DeleteCommandEvent()
        {
            await App.Database.DeleteItemAsync(interruptionID);
            await BaseContent.Navigation.PopAsync();
        }
        public ICommand ShowRecordsCommand { get { return new Command(async () => await ShowRecordsCommandEvent()); } }
        private async Task ShowRecordsCommandEvent()
        {
            await BaseContent.Navigation.PushAsync(new RecordsPage());
        }
        public ICommand PrepareReportCommand { get { return new Command(async () => await PrepareReportCommandEvent()); } }
        private async Task PrepareReportCommandEvent()
        {
            bool permissionGranted = await AskForPermission(new Permissions.StorageRead());
            if (permissionGranted)
            {
                if (permissionGranted)
                {
                    permissionGranted = await AskForPermission(new Permissions.StorageWrite());
                    if (permissionGranted)
                        await BaseContent.Navigation.PushAsync(new ReportsPage());
                }
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

        long interruptionID = 0;
    }
}
