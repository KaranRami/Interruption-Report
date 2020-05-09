using System.Collections.Generic;

namespace InterruptionReport.Model
{
    public class Subdivision: BaseFields
    {
        private List<Substation> subStations;
        public List<Substation> SubStations
        {
            get { return subStations; }
            set { SetProperty(ref subStations, value); }
        }
    }
}