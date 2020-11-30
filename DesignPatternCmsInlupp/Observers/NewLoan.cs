using DesignPatternCmsInlupp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.Observers
{
    public class NewLoan
    {
        public event EventHandler<NewLoanEventArgs> SendReportNewLoanToFinansInspektionen;
        public void CatchNewLoan(string personnummer, Loan loan)
        {
            SendReportNewLoanToFinansInspektionen.Invoke(this, new NewLoanEventArgs { PersonNummer = personnummer, Loan = loan });
        }
    }
}