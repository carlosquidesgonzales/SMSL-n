using DesignPatternCmsInlupp.FinansInspektionsRapportering;
using DesignPatternCmsInlupp.Models;
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
        private readonly IGetRiksBankensBaseRate _riksBankensBaseRate;
        public HomeController()
        {
            _repository = new Repository();
            _riksBankensBaseRate = new CachedRiksBankensBaseRate(new InterestService());
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Parametrar()
        {
            var logger = Logger.Instance;
            logger.LogAction(Logger.Actions.ParametrarPage, "");

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
            var logger = Logger.Instance;
            logger.LogAction(Logger.Actions.ViewCustomerPage, PersonNummer);

            var customer = _repository.FindCustomer(PersonNummer);
            return View(customer);
        }
        [HttpGet]
        public ActionResult Ringinstruktioner()
        {
            var logger = Logger.Instance;
            logger.LogAction(Logger.Actions.CallReceived, " some more useless info...");
            var model = new CallInstructions();
            return View(model);
        }
        [HttpPost]
        public ActionResult NewLoan(CallInstructions model)
        {
            var logger = Logger.Instance;
            var c = _repository.FindCustomer(model.Personnummer);
            if (c == null)
            {
                c = new Customer { PersonNummer = model.Personnummer };
                _repository.SaveToFile(c);
                logger.LogAction(Logger.Actions.CreatingCustomer, model.Personnummer);
                SendEmailToBoss("New customer!", model.Personnummer);
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
            SendEmailToBoss("New loan!", model.Personnummer + " " + loan.LoanNo);
            ReportNewLoanToFinansInspektionen(model.Personnummer, loan);
            logger.LogAction(Logger.Actions.CreatingLoan, $"{model.Personnummer} {loan.LoanNo}  {loan.Belopp}");
            return View(loan);
        }
        void SendEmailToBoss(string subject, string message)
        {
            var mailer = new Mailer();
            mailer.SendMail("harry@hederligeharry.se", subject, message);
        }
        void ReportNewLoanToFinansInspektionen(string personNummer, Loan loan)
        {
            //var report = new FinansInspektionsRapportering.Report(
            //    FinansInspektionsRapportering.Report.ReportType.Loan,
            //    personNummer, loan.LoanNo, 0, loan.Belopp, 0
            //    );
            var reports = new FinansInspektionsRapporteringBuilder()
                .WithType(FinansInspektionsRapportering.Report.ReportType.Loan)
                .WithPersonNummer(personNummer)
                .WithLoanNumber(loan.LoanNo)
                .WithAvgRta(0)
                .WithLoanBelopp(loan.Belopp)
                .WithLatePaymenyBelopp(0)
                .Build();
                
            reports.Send();
        }
        [HttpPost]
        public ActionResult Ringinstruktioner(CallInstructions model)
        {
            var c = _repository.FindCustomer(model.Personnummer);
            model.Result = true;
            if (c == null)
                model.Customer = c;
            int age = GetAge(model.Personnummer);
            
            //decimal baseRate = InterestService.GetRiksbankensBaseRate();
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