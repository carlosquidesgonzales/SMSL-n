using DesignPatternCmsInlupp.Models;
using DesignPatternCmsInlupp.Observers;
using DesignPatternCmsInlupp.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DesignPatternCmsInlupp.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
       
        private IPaymentRepository _paymentRepository;
        private ILoanRepository _loanRepository;
        public CustomerRepository()
        {
            _paymentRepository = new PaymentRepository();
            _loanRepository = new LoanRepository();
        }
        private readonly string _customersDatabas = HttpContext.Current.Server.MapPath("~/customers.txt");
        public IEnumerable<Customer> GetCustomers()
        {
            var customers = new List<Customer>();
            foreach (var line in System.IO.File.ReadAllLines(_customersDatabas))
            {
                string[] parts = line.Split(';');
                if (parts.Length < 1) continue;
                var customer = new Customer { PersonNummer = parts[0] };
                Parallel.Invoke(
                    () => _loanRepository.SetLoansForCustomer(customer),
                    () => _paymentRepository.SetPaymentsForCustomer(customer)
                    );
                customers.Add(customer);
            }
            return customers.AsEnumerable();
        }
        public Customer FindCustomer(string personnummer)
        {
            Customer customer = GetCustomers().FirstOrDefault(c => c.PersonNummer == personnummer);
            return customer;
        }
        public void SaveToFile(Customer c)
        {
            var mail = new NewMail();
            var logFile = new NewLogFile();         
            var allLines = System.IO.File.ReadAllLines(_customersDatabas).ToList();
            foreach (var line in allLines)
            {
                string[] parts = line.Split(';');
                if (parts.Length < 1) continue;
                if (parts[0] == c.PersonNummer)
                    return;
            }
            allLines.Add(c.PersonNummer);
            System.IO.File.WriteAllLines(_customersDatabas, allLines);
            mail.OnSendMail += mail.SendMail;
            mail.CatchNewEmail("harry@hederligeharry.se", "New customer!", c.PersonNummer); // Observer
            logFile.OnLogFile += logFile.LogAFile;
            logFile.CatchNewLgFile(Logger.Actions.CreatingCustomer, c.PersonNummer); // Observer
        }
    }
}