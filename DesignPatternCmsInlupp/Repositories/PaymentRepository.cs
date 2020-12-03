using DesignPatternCmsInlupp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.Repositories
{
    public class PaymentRepository: IPaymentRepository
    {
        private readonly string _paymentsDatabas = HttpContext.Current.Server.MapPath("~/payments.txt");
        public void SetPaymentsForCustomer(Customer customer)
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
    }
}