using DesignPatternCmsInlupp.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.Models
{
    public class Loan
    {
        private List<Invoice> _invoices = null;
        private readonly IInvoiceRepository _invoiceRepository;
        public Loan()
        {
            _invoiceRepository = new InvoiceRepository();
        }
        public string LoanNo { get; set; }
        public int Belopp { get; set; }
        public DateTime FromWhen { get; set; }
        public decimal InterestRate { get; set; }
        public List<Invoice> Invoices
        {
            get
            {
                if (_invoices == null)
                    _invoices = _invoiceRepository.GetForLoan(LoanNo).ToList();
                return _invoices;
            }
        }
    }
}