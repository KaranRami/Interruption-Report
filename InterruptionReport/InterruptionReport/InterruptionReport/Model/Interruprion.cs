using InterruptionReport.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterruptionReport.Model
{
    public class Interruption: BaseModel
    {
        public long ID { get; set; }

        private Subdivision subDivision;
        public Subdivision SubDivision
        {
            get { return subDivision; }
            set { SetProperty(ref subDivision , value); }
        }

        private Substation subStation;
        public Substation SubStation
        {
            get { return subStation; }
            set { SetProperty(ref subStation, value); }
        }

        private Feeder fider;
        public Feeder Fider
        {
            get { return fider; }
            set { SetProperty(ref fider, value); }
        }

        private DateTime reportedDate;
        public DateTime ReportedDate
        {
            get { return reportedDate; }
            set { SetProperty(ref reportedDate, value); }
        }

        private DateTime reportTimeFrom;
        public DateTime ReportTimeFrom
        {
            get { return reportTimeFrom; }
            set { SetProperty(ref reportTimeFrom, value); }
        }

        private DateTime reportTimeTo;
        public DateTime ReportTimeTo
        {
            get { return reportTimeTo; }
            set { SetProperty(ref reportTimeTo, value); }
        }

        private InterruprionType interruprionType;
        public InterruprionType InterruprionType
        {
            get { return interruprionType; }
            set { SetProperty(ref interruprionType, value); }
        }
    }
}
