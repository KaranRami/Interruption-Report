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
        public AddInterruptionPage(string title,long interruptionId=0)
        {
            InitializeComponent();
            Title = title;
            if(interruptionId!=0)
            {
                ToolbarItems.Remove(tbiReport);
                ToolbarItems.Remove(tbiRecords);
            }
            else
            {
                ToolbarItems.Remove(tbiDelete);
            }
            BindingContext = new AddInterruptionViewModel(this, interruptionId);
        }
    }
}