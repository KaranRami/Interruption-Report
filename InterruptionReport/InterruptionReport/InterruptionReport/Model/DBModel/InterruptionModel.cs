using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace InterruptionReport.Model.DBModel
{
    public class InterruptionDbModel
    {
        [PrimaryKey,AutoIncrement]
        public long ID { get; set; }
        public string SubDivision { get; set; }
        public string SubStation { get; set; }
        public string Feeder { get; set; }
        public string ReportedDate { get; set; }
        public TimeSpan ReportTimeFrom { get; set; }
        public TimeSpan ReportTimeTo { get; set; }
        public string InterruprionType { get; set; }
        public string Comment { get; set; }

    }
    public class InterruptionReportModel
    {
        public string SubDivision { get; set; }
        public string SubStation { get; set; }
        public string Feeder { get; set; }
        public string ReportedDate { get; set; }
        public string ReportTimeFrom { get; set; }
        public string ReportTimeTo { get; set; }
        public string InterruprionType { get; set; }
        public string Comment { get; set; }
        public string Hours { get; set; }

    }
}

