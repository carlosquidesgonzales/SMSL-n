using DesignPatternCmsInlupp.FinansInspektionsRapportering;
using DesignPatternCmsInlupp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.Observers
{
    public class NewLoan
    {
        public event EventHandler<LoanEventArgs> SendReportNewLoanToFinansInspektionen;
        public void CatchNewLoan(string personnummer, Loan loan)
        {
            SendReportNewLoanToFinansInspektionen.Invoke(this, new LoanEventArgs { PersonNummer = personnummer, Loan = loan });
        }
        public void ReportNewLoan(object sender, LoanEventArgs eventargs)
        {
            var report = new Report(
                Report.ReportType.Loan,
                eventargs.PersonNummer,
                eventargs.Loan.LoanNo,
                0,
                eventargs.Loan.Belopp,
                0);
            report.Send();
        }
    }
}