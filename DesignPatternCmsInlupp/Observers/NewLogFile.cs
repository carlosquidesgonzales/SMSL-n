using DesignPatternCmsInlupp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DesignPatternCmsInlupp.Services.Logger;

namespace DesignPatternCmsInlupp.Observers
{
    public class NewLogFile
    {
        public Actions Actions { get; internal set; }

        public event EventHandler<LogFileEventargs> OnLogFile;
        public void CatchNewLgFile(Actions action, string message)
        {
            OnLogFile.Invoke(this, new LogFileEventargs { Actions = action, Message = message });
        }
        public void LogAFile(object sender, LogFileEventargs e)
        {
            var logger = Logger.Instance; //Singleton
            logger.LogAction(e.Actions, e.Message);
        }
    }
}