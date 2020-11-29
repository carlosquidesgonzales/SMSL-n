using DesignPatternCmsInlupp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.Services
{
    public class CustomerTransaction
    {
        private readonly string _invoicesDatabas = HttpContext.Current.Server.MapPath("~/invoices.txt");
        private readonly string _paymentsDatabas = HttpContext.Current.Server.MapPath("~/payments.txt");
        private readonly string _loansDatabas = HttpContext.Current.Server.MapPath("~/loans.txt");
        public enum CustomerTransactions
        {          
            SetInvoice,
            SetPayment,
            SetLoan,
        };     
        public CustomerTransaction(){}
        private static Lazy<CustomerTransaction> instance = new Lazy<CustomerTransaction>(() => new CustomerTransaction());
        public static CustomerTransaction Instance => instance.Value;
        public void SetCustomerTransactions(Customer customer, CustomerTransactions transaction)
        {
            switch (transaction)
            {
                case CustomerTransactions.SetInvoice:
                    SetInvoicesForCustomer(customer);
                    break;
                case CustomerTransactions.SetPayment:
                    SetPaymentsForCustomer(customer);
                    break;
                case CustomerTransactions.SetLoan:
                    SetLoansForCustomer(customer);
                    break;
            }
        }
        private void SetInvoicesForCustomer(Customer customer)
        {
            foreach (var line in System.IO.File.ReadAllLines(_invoicesDatabas))
            {
                string[] parts = line.Split(';');
                if (parts.Length < 2) continue;
                var loan = customer.Loans.FirstOrDefault(r => r.LoanNo == parts[0]);
                if (loan == null) continue;
                if (loan.Invoices == null)
                {
                    var invoice = new Invoice
                    {
                        InvoiceNo = Convert.ToInt32(parts[1]),
                        Belopp = Convert.ToInt32(parts[2]),
                        InvoiceDate = DateTime.ParseExact(parts[3], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        DueDate = DateTime.ParseExact(parts[3], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    };
                    loan.Invoices.Add(invoice);
                }
            }
        }
        private void SetPaymentsForCustomer(Customer customer)
        {
            foreach (var line in System.IO.File.ReadAllLines(_paymentsDatabas))
            {
                string[] parts = line.Split(';');
                if (parts.Length < 2) continue;
                var invoice = customer.Loans.SelectMany(r => r.Invoices).FirstOrDefault(i => i.InvoiceNo == Convert.ToInt32(parts[0]));
                if (invoice == null) continue;
                var payment = new Payment
                {
                    Belopp = Convert.ToInt32(parts[1]),
                    PaymentDate = DateTime.ParseExact(parts[2], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    BankPaymentReference = parts[3],
                };
                invoice.Payments.Add(payment);
            }
        }
        private void SetLoansForCustomer(Customer customer)
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