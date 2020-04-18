using System.Collections.Generic;

namespace InterruptionReport.Model
{
    public class Substation: BaseFields
    {
        private List<Feeder> feeders;
        public List<Feeder> Feeders
        {
            get { return feeders; }
            set { SetProperty(ref feeders, value); }
        }
    }
}