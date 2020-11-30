using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.Builders
{
    public class RapportBuilder
    {     
        public RapportBuilder Addtext(string childName)
        {
            var e = new RapportElement("",childName);
            root.Elements.Add(e);
            return this;
        }
        public RapportBuilder WithPnumber(string pNumber)
        {
            var e = new RapportElement(";",pNumber.ToString());
            root.Elements.Add(e);
            return this;
        }
        public RapportBuilder WithLoanNumber(string lNumber)
        {
            var e = new RapportElement(";",lNumber.ToString());
            root.Elements.Add(e);
            return this;
        }
        public RapportBuilder WithLatePaymentBelopp(decimal lPayment)
        {
            var e = new RapportElement(";",lPayment.ToString());
            root.Elements.Add(e);
            return this;
        }
        public RapportBuilder WithLoanBelopp(decimal lBelopp)
        {
            var e = new RapportElement(";",lBelopp.ToString());
            root.Elements.Add(e);
            return this;
        }
        public RapportBuilder WithAVG(decimal avg)
        {
            var e = new RapportElement(";",avg.ToString());
            root.Elements.Add(e);
            return this;
        }
        public override string ToString()
        {
            return root.ToString();
        }
        public void Clear()
        {
            root = new RapportElement { };
        }
        RapportElement root = new RapportElement();
    }
}