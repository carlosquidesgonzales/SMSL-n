using DesignPatternCmsInlupp.FinansInspektionsRapportering;
using DesignPatternCmsInlupp.Models;
using DesignPatternCmsInlupp.Observers;
using DesignPatternCmsInlupp.Repositories;
using DesignPatternCmsInlupp.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DesignPatternCmsInlupp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository _repository;
        private IGetRiksBankensBaseRate _riksBankensBaseRate;
        public HomeController(IRepository repository)
        {
            _repository = repository; //Repository med dependency injection
                                      // _riksBankensBaseRate = riksBankensBaseRate;        
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Parametrar()
        {
            var loggFile = new LoggFile();
            _riksBankensBaseRate = new CachedRiksBankensBaseRate(new InterestService()); // Cache Decorator
            loggFile.SendLoggFile += SendLoggFile;
            loggFile.CatchNewLoan(Logger.Actions.ParametrarPage, "");

            var model = new Parametrar();
            model.CurrentRiksbankenStibor = _riksBankensBaseRate.GetRiksbankensBaseRate();
            return View(model);
        }
        [HttpGet]
        public ActionResult ListCustomers()
        {
            var model = _repository.GetCustomers().ToList();
            return View(model);
        }
        [HttpGet]
        public ActionResult Customer(string PersonNummer)
        {
            var loggFile = new LoggFile();
            loggFile.SendLoggFile += SendLoggFile;
            loggFile.CatchNewLoan(Logger.Actions.ViewCustomerPage, PersonNummer);
            var customer = _repository.FindCustomer(PersonNummer);
            return View(customer);
        }
        [HttpGet]
        public ActionResult Ringinstruktioner()
        {
            var loggFile = new LoggFile();
            loggFile.SendLoggFile += SendLoggFile;
            loggFile.CatchNewLoan(Logger.Actions.CallReceived, " some more useless info...");
            var model = new CallInstructions();
            return View(model);
        }
        [HttpPost]
        public ActionResult NewLoan(CallInstructions model)
        {
            var mail = new Mail();
            var reportNewLoan = new NewLoan();
            var loggFile = new LoggFile();
            var c = _repository.FindCustomer(model.Personnummer);
            if (c == null)
            {
                c = new Customer { PersonNummer = model.Personnummer };
                _repository.SaveToFile(c);

                mail.SendMail += SendEmailToBoss;
                mail.CatchEmail("harry@hederligeharry.se", "New customer!", model.Personnummer);//Observer
                loggFile.SendLoggFile += SendLoggFile;
                loggFile.CatchNewLoan(Logger.Actions.CreatingCustomer, model.Personnummer);//Observer
            }
            var loan = new Loan
            {
                LoanNo = DateTime.Now.Ticks.ToString(),
                Belopp = model.HowMuchDoYouNeed,
                FromWhen = DateTime.Now,
                InterestRate = model.RateWeCanOffer
            };
            c.Loans.Add(loan);
            _repository.SaveLoanToFile(c, loan);

            mail.SendMail += SendEmailToBoss;
            mail.CatchEmail("harry@hederligeharry.se", "New loan!", model.Personnummer + " " + loan.LoanNo);//Observer
            reportNewLoan.SendReportNewLoanToFinansInspektionen += ReportNewLoanToFinansInspektionen;
            reportNewLoan.CatchNewLoan(model.Personnummer, loan);//Observer 
            loggFile.SendLoggFile += SendLoggFile;
            loggFile.CatchNewLoan(Logger.Actions.CreatingLoan, $"{model.Personnummer} {loan.LoanNo}  {loan.Belopp}");//Observer
            return View(loan);
        }
        private static void SendLoggFile(object sender, LoggFileEventargs eventargs)
        {
            var logger = Logger.Instance; //Singleton
            logger.LogAction(Logger.Actions.CreatingLoan, eventargs.Message);
        }
        private static void SendEmailToBoss(object sender, MailEventargs eventargs)
        {
            var mailer = new Mailer();
            mailer.SendMail(eventargs.To, eventargs.Subject, eventargs.Message);
        }
        void ReportNewLoanToFinansInspektionen(object sender, NewLoanEventArgs eventargs)
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
        [HttpPost]
        public ActionResult Ringinstruktioner(CallInstructions model)
        {
            _riksBankensBaseRate = new CachedRiksBankensBaseRate(new InterestService());
            var c = _repository.FindCustomer(model.Personnummer);
            model.Result = true;
            if (c == null)
                model.Customer = c;
            int age = GetAge(model.Personnummer);
            decimal baseRate = _riksBankensBaseRate.GetRiksbankensBaseRate();
            if (c == null)
            {
                if (age < 18)
                    model.RateWeCanOffer = 30.22m + baseRate;
                else if (age < 35)
                    model.RateWeCanOffer = 32.18m + baseRate;
                else if (age < 65)
                    model.RateWeCanOffer = 22.30m + baseRate;
                else
                    model.RateWeCanOffer = 45.30m + baseRate;
            }
            else
            {
                if (age < 18)
                    model.RateWeCanOffer = 29.32m + baseRate;
                else if (age < 35)
                    model.RateWeCanOffer = 31.38m + baseRate;
                else if (age < 65)
                    model.RateWeCanOffer = 21.20m + baseRate;
                else
                    model.RateWeCanOffer = 41.12m + baseRate;
                if (c.HasEverBeenLatePaying)
                {
                    model.RateWeCanOffer += 10.0m;
                }
            }
            return View(model);
        }
        int GetAge(string personnummer)
        {
            if (personnummer.Length == 10) //8101011234
                return DateTime.Now.Year - 1900 - Convert.ToInt32(personnummer.Substring(0, 2));
            if (personnummer.Length == 12 && !personnummer.Contains("-")) //198101011234
                return DateTime.Now.Year - Convert.ToInt32(personnummer.Substring(0, 4));
            if (personnummer.Length == 11) //810101-1234
                return DateTime.Now.Year - 1900 - Convert.ToInt32(personnummer.Substring(0, 2));
            if (personnummer.Length == 13) //19810101-1234
                return DateTime.Now.Year - Convert.ToInt32(personnummer.Substring(0, 4));
            //Fake if not correct
            return 50;
        }
        public ActionResult GenerateFakeData(int antal)
        {
            var rnd = new Random();
            for (int i = 0; i < antal; i++)
            {
                var persnr = rnd.Next(1934, 1999).ToString() +
                    rnd.Next(1, 12).ToString("00") +
                    rnd.Next(1, 28).ToString("00") +
                    rnd.Next(1000, 9999);
                var c = _repository.FindCustomer(persnr);
                if (c != null) continue;
                c = new Customer { PersonNummer = persnr };
                _repository.SaveToFile(c);
                for (int l = 0; l <= rnd.Next(1, 7); l++)
                {
                    var loan = new Loan
                    {
                        LoanNo = DateTime.Now.AddDays(-rnd.Next(10, 2000)).Ticks.ToString(),
                        Belopp = rnd.Next(3, 200) * 100,
                        FromWhen = DateTime.Now.AddDays(-rnd.Next(10, 2000)),
                        InterestRate = Convert.ToDecimal(rnd.NextDouble() * (45 - 20) + 20)
                    };
                    _repository.SaveLoanToFile(c, loan);
                }
            }
            return Content("Done");
        }
    }
}