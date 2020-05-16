using Acr.UserDialogs;
using InterruptionReport.Interface;
using InterruptionReport.Model;
using InterruptionReport.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace InterruptionReport.ViewModel
{
    public class ReportsViewModel : BaseViewModel
    {
        public ReportsViewModel(ContentPage view) : base(view)
        {
        }

        public ICommand RefreshCommand { get { return new Command(async () => await RefreshCommandEvent()); } }
        private async Task RefreshCommandEvent()
        {
            try
            {
                List<SavedReport> reports = new List<SavedReport>();
                bool permissionGranted = await AskForPermission(new Permissions.StorageRead());
                if (permissionGranted)
                {
                    string folderpath = DependencyService.Get<IFileHelper>().GetLocalFilePath("");
                    if (Directory.Exists(folderpath))
                    {
                        string[] files = Directory.GetFiles(folderpath, "*.csv");
                        foreach (var file in files)
                        {
                            SavedReport savedReport = new SavedReport()
                            {
                                FilePath = file,
                                FileName = Path.GetFileName(file),
                                FileCreatedOn = File.GetCreationTime(file),
                            };
                            reports.Add(savedReport);
                        }
                        Reports = reports.OrderByDescending(r => r.FileCreatedOn).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                await BaseContent.DisplayAlert("Error", ex.Message, "Ok");
            }
        }
        public ICommand PrepareNewReportCommand { get { return new Command(async () => await PrepareNewReportCommandEvent()); } }
        private async Task PrepareNewReportCommandEvent()
        {
            await BaseContent.Navigation.PushAsync(new PrepareReportPage("Generate Report"));
        }
        public ICommand OpenReportCommand { get { return new Command<SavedReport>(async (SavedReport savedReport) => await OpenReportCommandEvent(savedReport)); } }
        private async Task OpenReportCommandEvent(SavedReport savedReport)
        {
            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(savedReport.FilePath)
            });
        }
        public ICommand ShareReportCommand { get { return new Command<SavedReport>(async (SavedReport savedReport) => await ShareReportCommandEvent(savedReport)); } }
        private async Task ShareReportCommandEvent(SavedReport savedReport)
        {

            await Share.RequestAsync(new ShareFileRequest
            {
                Title = savedReport.FileName,
                File = new ShareFile(savedReport.FilePath)
            });
        }
        public ICommand DeleteCommand { get { return new Command<SavedReport>(async (SavedReport savedReport) => await DeleteCommandEvent(savedReport)); } }
        private async Task DeleteCommandEvent(SavedReport savedReport)
        {

            bool shouldDelete = await BaseContent.DisplayAlert("Delete Report", "Are you sure you want to delete " + savedReport.FileName, "Yes", "No");
            if (shouldDelete)
            {
                if (File.Exists(savedReport.FilePath))
                {
                    File.Delete(savedReport.FilePath);
                }
                RefreshCommand.Execute(null);
            }
        }

        private List<SavedReport> _report;
        public List<SavedReport> Reports
        {
            get { return _report; }
            set { SetProperty(ref _report, value); }
        }

    }


}
