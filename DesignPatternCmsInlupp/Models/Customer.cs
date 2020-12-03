using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.Models
{
    public class Customer
    {
        public Customer()
        {
            Loans = new List<Loan>();
        }
        public string PersonNummer { get; set; }
        public List<Loan> Loans { get; set; }
        public int Total()
        {
            return Loans.Sum(l => l.Belopp);
        }
        public bool HasEverBeenLatePaying
        {
            get
            {
                foreach (var loan in Loans)
                    foreach (var i in loan.Invoices)
                        if (i.LatePayment() > 0) return true;
                return false;
            }
        }
        public int GetAge(string personnummer)
        {
            if (personnummer.Length == 10) //8101011234
                return DateTime.Now.Year - 1900 - Convert.ToInt32(personnummer.Substring(0, 2));
            if (personnummer.Length == 12 && !personnummer.Contains("-")) //198101011234
                return DateTime.Now.Year - Convert.ToInt32(personnummer.Substring(0, 4));
            if (personnummer.Length == 11) //810101-1234
                return DateTime.Now.Year - 1900 - Convert.ToInt32(personnummer.Substring(0, 2));
            if (personnummer.Length == 13) //19810101-1234
                return DateTime.Now.Year - Convert.ToInt32(personnummer.Substring(0, 4));
            //Fake if not correct
            return 50;
        }
    }
}