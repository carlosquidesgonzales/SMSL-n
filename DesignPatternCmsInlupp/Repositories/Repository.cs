using DesignPatternCmsInlupp.Models;
using DesignPatternCmsInlupp.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DesignPatternCmsInlupp.Repositories
{
    public class Repository : IRepository
    {
        private readonly string _customersDatabas = HttpContext.Current.Server.MapPath("~/customers.txt");
        private readonly string _loansDatabas = HttpContext.Current.Server.MapPath("~/loans.txt");
        private readonly string _invoicesDatabas = HttpContext.Current.Server.MapPath("~/invoices.txt");
        private readonly string _paymentsDatabas = HttpContext.Current.Server.MapPath("~/payments.txt");
        public IEnumerable<Customer> GetCustomers()
        {
            var customers = new List<Customer>();
            foreach (var line in System.IO.File.ReadAllLines(_customersDatabas))
            {
                string[] parts = line.Split(';');
                if (parts.Length < 1) continue;
                var customer = new Customer { PersonNummer = parts[0] };
                Parallel.Invoke(
                    () => SetLoansForCustomer(customer),
                    () => SetInvoicesForCustomer(customer),
                    () => SetPaymentsForCustomer(customer)
                    );
                //SetLoansForCustomer(customer);
                //SetInvoicesForCustomer(customer);
                //SetPaymentsForCustomer(customer);
                customers.Add(customer);
            }
            return customers.AsEnumerable();
        }
        public Customer FindCustomer(string personnummer)
        {
            Customer customer = GetCustomers().FirstOrDefault(c => c.PersonNummer == personnummer);
            return customer;
        }
        public Task SetInvoicesForCustomer(Customer customer)
        {
            var invoiceForCustomer = CustomerTransaction.Instance;
            invoiceForCustomer.SetCustomerTransactions(customer, CustomerTransaction.CustomerTransactions.SetInvoice);
            //foreach (var line in System.IO.File.ReadAllLines(_invoicesDatabas))
            //{
            //    string[] parts = line.Split(';');
            //    if (parts.Length < 2) continue;
            //    var loan = customer.Loans.FirstOrDefault(r => r.LoanNo == parts[0]);
            //    if (loan == null) continue;
            //    var invoice = new Invoice
            //    {
            //        InvoiceNo = Convert.ToInt32(parts[1]),
            //        Belopp = Convert.ToInt32(parts[2]),
            //        InvoiceDate = DateTime.ParseExact(parts[3], "yyyy-MM-dd", CultureInfo.InvariantCulture),
            //        DueDate = DateTime.ParseExact(parts[3], "yyyy-MM-dd", CultureInfo.InvariantCulture),
            //    };
            //    loan.Invoices.Add(invoice);
            //}
            return Task.CompletedTask;
        }
        public Task SetLoansForCustomer(Customer customer)
        {
            var loanForCustomer = CustomerTransaction.Instance;
            loanForCustomer.SetCustomerTransactions(customer, CustomerTransaction.CustomerTransactions.SetLoan);
            //foreach (var line in System.IO.File.ReadAllLines(_loansDatabas))
            //{
            //    string[] parts = line.Split(';');
            //    if (parts.Length < 2) continue;
            //    if (parts[0] == c.PersonNummer)
            //    {
            //        var loan = new Loan
            //        {
            //            LoanNo = parts[1],
            //            Belopp = Convert.ToInt32(parts[2]),
            //            FromWhen = DateTime.ParseExact(parts[3], "yyyy-MM-dd", CultureInfo.InvariantCulture),
            //            InterestRate = Convert.ToDecimal(parts[4])
            //        };
            //        c.Loans.Add(loan);
            //    }
            //}
            return Task.CompletedTask;
        }
        public Task SetPaymentsForCustomer(Customer customer)
        {
            var paymentForCustomer = CustomerTransaction.Instance;
            paymentForCustomer.SetCustomerTransactions(customer, CustomerTransaction.CustomerTransactions.SetPayment);
            //foreach (var line in System.IO.File.ReadAllLines(_paymentsDatabas))
            //{
            //    string[] parts = line.Split(';');
            //    if (parts.Length < 2) continue;
            //    var invoice = customer.Loans.SelectMany(r => r.Invoices).FirstOrDefault(i => i.InvoiceNo == Convert.ToInt32(parts[0]));
            //    if (invoice == null) continue;
            //    var payment = new Payment
            //    {
            //        Belopp = Convert.ToInt32(parts[1]),
            //        PaymentDate = DateTime.ParseExact(parts[2], "yyyy-MM-dd", CultureInfo.InvariantCulture),
            //        BankPaymentReference = parts[3],
            //    };
            //    invoice.Payments.Add(payment);
            //}
            return Task.CompletedTask;
        }
        public void SaveToFile(Customer c)
        {
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
        }
        public void SaveLoanToFile(Customer c, Loan l)
        {
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
        }
    }
}