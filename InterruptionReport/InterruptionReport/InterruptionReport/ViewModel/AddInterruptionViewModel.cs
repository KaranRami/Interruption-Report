using InterruptionReport.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace InterruptionReport.ViewModel
{
    public class AddInterruptionViewModel : BaseViewModel
    {
        public AddInterruptionViewModel(ContentPage view) : base(view)
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
                        if(subdivision == null)
                            throw new Exception("Unable to read files");
                        else
                        {
                            CurrentInterruption = new Interruption()
                            {
                                SubDivision = subdivision,
                                ReportedDate=DateTime.Now,
                            };
                        }
                    }
                }
            }
        }

        private Interruption currentInterruption;
        public Interruption CurrentInterruption
        {
            get { return currentInterruption; }
            set { SetProperty(ref currentInterruption, value); }
        }

    }
}
