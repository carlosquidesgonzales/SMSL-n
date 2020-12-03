using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.Models
{
    public class Mail : EventArgs
    {
        public string Email { get; set; }
        public string Message { get; set; }
        public string TansactionType { get; set; }
    }
}