using DesignPatternCmsInlupp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatternCmsInlupp.Repositories
{
    interface IInvoiceRepository
    {
        IEnumerable<Invoice> GetForLoan(string loanNo);
    }
}
