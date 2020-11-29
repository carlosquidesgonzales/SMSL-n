using DesignPatternCmsInlupp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatternCmsInlupp.Repositories
{
    public interface IRepository
    {
        IEnumerable<Customer> GetCustomers();
        Customer FindCustomer(string personnummer);
        Task SetLoansForCustomer(Customer c);
        Task SetInvoicesForCustomer(Customer customer);
        Task SetPaymentsForCustomer(Customer customer);
        void SaveToFile(Customer c);
        void SaveLoanToFile(Customer c, Loan l);
    }
}
