using System;
using System.Collections.Generic;
using System.Text;

namespace InterruptionReport.Interface
{
    public interface IFileHelper
    {
        string GetLocalFilePath(string filename);
    }
}
