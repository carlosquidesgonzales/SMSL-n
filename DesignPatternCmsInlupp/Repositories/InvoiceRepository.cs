using DesignPatternCmsInlupp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        Lazy<List<string[]>> _invoices = new Lazy<List<string[]>>(() =>
        {
            string invoicesDatabas = HttpContext.Current.Server.MapPath("~/invoices.txt");
            var invoices = new List<string[]>();
            foreach (var line in System.IO.File.ReadAllLines(invoicesDatabas))
            {
                string[] parts = line.Split(';');
                if (parts.Length < 2) continue;
                invoices.Add(parts);
            }
            return invoices;
        });
        public IEnumerable<Invoice> GetForLoan(string loanNo)
        {
            List<string[]> allInvoices = _invoices.Value; //Lazy Loading
            List<Invoice> invoices = new List<Invoice>();

            foreach (var item in allInvoices)
            {
                if (loanNo == item[0])
                {
                    var invoice = new Invoice
                    {
                        InvoiceNo = Convert.ToInt32(item[1]),
                        Belopp = Convert.ToInt32(item[2]),
                        InvoiceDate = DateTime.ParseExact(item[3], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                        DueDate = DateTime.ParseExact(item[3], "yyyy-MM-dd", CultureInfo.InvariantCulture),
                    };
                    invoices.Add(invoice);

                }
            }
            return invoices;
        }
    }
}