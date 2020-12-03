﻿using DesignPatternCmsInlupp.Models;
using DesignPatternCmsInlupp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.Observers
{
    public class MailEventargs
    {
        public string Email { get; set; }
        public string Message { get; set; }
        public string TansactionType { get; set; }
    }
}