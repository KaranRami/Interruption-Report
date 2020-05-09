using System;
using System.Collections.Generic;
using System.Text;

namespace InterruptionReport.Model
{
    public class Interruption: BaseModel
    {

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

        private Feeder feeder;
        public Feeder Feeder
        {
            get { return feeder; }
            set { SetProperty(ref feeder, value); }
        }

        private DateTime reportedDate;
        public DateTime ReportedDate
        {
            get { return reportedDate; }
            set { SetProperty(ref reportedDate, value); }
        }

        private TimeSpan reportTimeFrom;
        public TimeSpan ReportTimeFrom
        {
            get { return reportTimeFrom; }
            set { SetProperty(ref reportTimeFrom, value); }
        }

        private TimeSpan reportTimeTo;
        public TimeSpan ReportTimeTo
        {
            get { return reportTimeTo; }
            set { SetProperty(ref reportTimeTo, value); }
        }

        private string interruprionType;
        public string InterruprionType
        {
            get { return interruprionType; }
            set { SetProperty(ref interruprionType, value); }
        }
        private string comment;
        public string Comment
        {
            get { return comment; }
            set { SetProperty(ref comment, value); }
        }
    }
}
