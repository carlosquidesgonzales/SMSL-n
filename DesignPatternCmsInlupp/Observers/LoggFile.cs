using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DesignPatternCmsInlupp.Services.Logger;

namespace DesignPatternCmsInlupp.Observers
{
    public class LoggFile
    {
        public event EventHandler<LoggFileEventargs> SendLoggFile;
        public void CatchNewLoan(Actions tAction, string message) =>
            SendLoggFile.Invoke(this, new LoggFileEventargs { TheNewAction = tAction, Message = message });
    }
}