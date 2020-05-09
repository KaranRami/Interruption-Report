using System;
using System.Collections.Generic;
using System.Text;

namespace InterruptionReport.Model
{
    public class SavedReport
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public DateTime FileCreatedOn { get; set; }
    }
}
