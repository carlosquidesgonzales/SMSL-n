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
       
        private readonly ICustomerRepository _customerRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IInterestService _interestService;
        public HomeController(ICustomerRepository repository)
        {
            _customerRepository = repository; //dependency injection
            _loanRepository = new LoanRepository();
            _interestService = new CacheInterestService(new InterestService()); // Cache Decorator

        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Parametrar()
        {
            var loggFile = new NewLogFile();

            loggFile.OnLogFile += loggFile.LogAFile;
            loggFile.CatchNewLgFile(Logger.Actions.ParametrarPage, "");

            var model = new Parametrar();
            model.CurrentRiksbankenStibor = _interestService.GetRiksbankensBaseRate();
            return View(model);
        }
        [HttpGet]
        public ActionResult ListCustomers()
        {
            var model = _customerRepository.GetCustomers().ToList();
            return View(model);
        }
        [HttpGet]
        public ActionResult Customer(string PersonNummer)
        {
            var loggFile = new NewLogFile();
            loggFile.OnLogFile += loggFile.LogAFile;
            loggFile.CatchNewLgFile(Logger.Actions.ViewCustomerPage, PersonNummer);
            var customer = _customerRepository.FindCustomer(PersonNummer);
            return View(customer);
        }
        [HttpGet]
        public ActionResult Ringinstruktioner()
        {
            var loggFile = new NewLogFile();
            loggFile.OnLogFile += loggFile.LogAFile;
            loggFile.CatchNewLgFile(Logger.Actions.CallReceived, " some more useless info...");
            var model = new CallInstructions();
            return View(model);
        }
        [HttpPost]
        public ActionResult NewLoan(CallInstructions model)
        {
            var c = _customerRepository.FindCustomer(model.Personnummer);
            if (c == null)
            {
                c = new Customer { PersonNummer = model.Personnummer };
                _customerRepository.SaveToFile(c);
            }
            var loan = new Loan
            {
                LoanNo = DateTime.Now.Ticks.ToString(),
                Belopp = model.HowMuchDoYouNeed,
                FromWhen = DateTime.Now,
                InterestRate = model.RateWeCanOffer
            };
            c.Loans.Add(loan);
            _loanRepository.Save(c, loan);
            return View(loan);
        }
        [HttpPost]
        public ActionResult Ringinstruktioner(CallInstructions model)
        {
            var customer = new Customer();
            var c = _customerRepository.FindCustomer(model.Personnummer);
            model.Result = true;
            if (c == null)
                model.Customer = c;
            int age = customer.GetAge(model.Personnummer);
            decimal baseRate = _interestService.GetRiksbankensBaseRate();
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
        public ActionResult GenerateFakeData(int antal)
        {
            var rnd = new Random();
            for (int i = 0; i < antal; i++)
            {
                var persnr = rnd.Next(1934, 1999).ToString() +
                    rnd.Next(1, 12).ToString("00") +
                    rnd.Next(1, 28).ToString("00") +
                    rnd.Next(1000, 9999);
                var c = _customerRepository.FindCustomer(persnr);
                if (c != null) continue;
                c = new Customer { PersonNummer = persnr };
                _customerRepository.SaveToFile(c);
                for (int l = 0; l <= rnd.Next(1, 7); l++)
                {
                    var loan = new Loan
                    {
                        LoanNo = DateTime.Now.AddDays(-rnd.Next(10, 2000)).Ticks.ToString(),
                        Belopp = rnd.Next(3, 200) * 100,
                        FromWhen = DateTime.Now.AddDays(-rnd.Next(10, 2000)),
                        InterestRate = Convert.ToDecimal(rnd.NextDouble() * (45 - 20) + 20)
                    };
                    _loanRepository.Save(c, loan);
                }
            }
            return Content("Done");
        }
    }
}