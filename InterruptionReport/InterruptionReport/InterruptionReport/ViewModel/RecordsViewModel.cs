using InterruptionReport.Model.DBModel;
using InterruptionReport.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace InterruptionReport.ViewModel
{
    public class RecordsViewModel : BaseViewModel
    {
        public RecordsViewModel(ContentPage view) : base(view)
        {
            BaseContent.Appearing += BaseContent_Appearing;
        }

        private void BaseContent_Appearing(object sender, EventArgs e)
        {
            if (FilterPage == null)
            {
                FilterCommand.Execute(null);
            }
            else
            {
                Task.Run(async () =>
                {
                    var data = await (FilterPage?.BindingContext as PrepareReportViewModel)?.GetFilteredItems();
                    if (data != null)
                        Records = new ObservableCollection<InterruptionDbModel>(data);
                    else
                        Records = null;
                });
            }
        }

        public ICommand EditInterruptionCommand { get { return new Command<long>(async (long id) => await EditInterruptionCommandEvent(id)); } }
        private async Task EditInterruptionCommandEvent(long id)
        {
            await BaseContent.Navigation.PushAsync(new AddInterruptionPage("Edit Interruption", id));
        }
        public ICommand FilterCommand { get { return new Command(async () => await FilterCommandEvent()); } }
        private async Task FilterCommandEvent()
        {
            if (FilterPage == null)
            {
                FilterPage = new PrepareReportPage("Filter");
            }
            await BaseContent.Navigation.PushAsync(FilterPage);
        }

        PrepareReportPage FilterPage = null;

        private ObservableCollection<InterruptionDbModel> _records;
        public ObservableCollection<InterruptionDbModel> Records
        {
            get { return _records; }
            set { SetProperty(ref _records, value); }
        }

    }
}
