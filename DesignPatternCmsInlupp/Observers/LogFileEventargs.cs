using DesignPatternCmsInlupp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DesignPatternCmsInlupp.Services.Logger;

namespace DesignPatternCmsInlupp.Observers
{
    public class LogFileEventargs
    {
        public Actions Actions { get; set; }
        public string Message { get; set; }

    }
}