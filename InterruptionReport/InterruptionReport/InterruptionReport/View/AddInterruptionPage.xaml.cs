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
    public partial class AddInterruptionPage : ContentPage
    {
        public AddInterruptionPage()
        {
            InitializeComponent();
            BindingContext = new AddInterruptionViewModel(this);
        }
    }
}