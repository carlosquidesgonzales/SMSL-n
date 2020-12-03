using DesignPatternCmsInlupp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatternCmsInlupp.Repositories
{
    interface ILoanRepository
    {
        void Save(Customer c, Loan l);
        void SetLoansForCustomer(Customer customer);
    }
}
