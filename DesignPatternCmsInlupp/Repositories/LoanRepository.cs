using DesignPatternCmsInlupp.FinansInspektionsRapportering;
using DesignPatternCmsInlupp.Models;
using DesignPatternCmsInlupp.Observers;
using DesignPatternCmsInlupp.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.Repositories
{
    public class LoanRepository: ILoanRepository
    {
        private readonly string _loansDatabas = HttpContext.Current.Server.MapPath("~/loans.txt");
        public void Save(Customer c, Loan l)
        {
            var reportNewLoan = new NewLoan();
            var mail = new NewMail();
            var logFile = new NewLogFile();
            var allLines = System.IO.File.ReadAllLines(_loansDatabas).ToList();
            foreach (var line in allLines)
            {
                string[] parts = line.Split(';');
                if (parts.Length < 1) continue;
                if (parts[0] == c.PersonNummer && parts[1] == l.LoanNo)
                    return;
            }
            allLines.Add($"{c.PersonNummer};{l.LoanNo};{l.Belopp};{l.FromWhen.ToString("yyyy-MM-dd")};{l.InterestRate}");
            System.IO.File.WriteAllLines(_loansDatabas, allLines);
            mail.OnSendMail += mail.SendMail;
            mail.CatchNewEmail("harry@hederligeharry.se", "New loan!", c.PersonNummer + " " + l.LoanNo); // Observer
            reportNewLoan.SendReportNewLoanToFinansInspektionen += reportNewLoan.ReportNewLoan;
            reportNewLoan.CatchNewLoan(c.PersonNummer, l);//Observer 
            logFile.OnLogFile += logFile.LogAFile;
            logFile.CatchNewLgFile(Logger.Actions.CreatingLoan, $"{c.PersonNummer} {l.LoanNo}  {l.Belopp}"); // Observer
        }
        public void SetLoansForCustomer(Customer customer)
        {
            foreach (var line in System.IO.File.ReadAllLines(_loansDatabas))
            {
                string[] parts = line.Split(';');
                if (parts.Length < 2) continue;
                if (parts[0] == customer.PersonNummer)
                {
                    var loan = new Loan
                    {
                        LoanNo = parts[1],
                        Belopp = Convert.ToInt32(parts[2]),
                        FromWhen = DateTime.ParseExact(parts[3], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        InterestRate = Convert.ToDecimal(parts[4])
                    };
                    customer.Loans.Add(loan);
                }
            }
        }
    }
}