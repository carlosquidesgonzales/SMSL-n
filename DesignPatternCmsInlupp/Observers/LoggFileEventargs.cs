using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DesignPatternCmsInlupp.Services.Logger;

namespace DesignPatternCmsInlupp.Observers
{
    public class LoggFileEventargs
    {
        public Actions TheNewAction;
        public string Message;
    }
}