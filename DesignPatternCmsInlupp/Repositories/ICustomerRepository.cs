using DesignPatternCmsInlupp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPatternCmsInlupp.Repositories
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetCustomers();
        Customer FindCustomer(string personnummer);
        void SaveToFile(Customer c);
    }
}
