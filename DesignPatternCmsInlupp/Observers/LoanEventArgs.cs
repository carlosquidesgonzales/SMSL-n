using DesignPatternCmsInlupp.Models;
using DesignPatternCmsInlupp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.Observers
{
    public class LoanEventArgs
    {
        public string PersonNummer;
        public Loan Loan;
        internal Logger.Actions NewAction;
    }
}