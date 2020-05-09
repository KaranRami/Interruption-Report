using InterruptionReport.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InterruptionReport.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportsPage : ContentPage
    {
        public ReportsPage()
        {
            InitializeComponent();
            BindingContext = new ReportsViewModel(this);
        }

        protected override void OnAppearing()
        {
            (BindingContext as ReportsViewModel).RefreshCommand.Execute(null);
        }
    }
}