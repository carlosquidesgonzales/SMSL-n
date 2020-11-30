using DesignPatternCmsInlupp.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace DesignPatternCmsInlupp.FinansInspektionsRapportering
{
    public class Report
    {
        public enum ReportType
        {
            Loan,
            LatePayment,
            AverageRta
        }
        public ReportType Type { get; set; }
        public decimal LoanBelopp { get; }
        public string PersonNummer { get; set; }
        public string LoanNumber { get; set; }
        public decimal AvgRta { get; set; }
        public decimal LatePaymentBelopp { get; set; }
        public override string ToString()
        {
            return base.ToString();
        }

        public Report(ReportType reportType, string personNummer, string loanNumber, decimal avgRta, decimal loanBelopp, decimal latePaymentBelopp)
        {
            Type = reportType;
            LoanBelopp = loanBelopp;
            if (Type == ReportType.LatePayment || Type == ReportType.Loan)
            {
                PersonNummer = personNummer;
                LoanNumber = loanNumber;
            }
            if(Type == ReportType.LatePayment)
            {
                LatePaymentBelopp = latePaymentBelopp;
            }
            if (Type == ReportType.Loan)
            {
                LoanBelopp = loanBelopp;
            }
            if (Type == ReportType.AverageRta)
                AvgRta = avgRta;
        }
        public void Send()
        {
            var sb = new StringBuilder();
            var rapportBuilder = new RapportBuilder();
            sb.Clear();
            rapportBuilder.Clear();
            string transaction = "";
            if(Type == ReportType.AverageRta)
            {
                rapportBuilder.Addtext("AR").WithAVG(AvgRta); //Fluent Builder        
                //transaction = $"AR;{AvgRta}";
            }
            if (Type == ReportType.LatePayment)
            {
                rapportBuilder.Addtext("AR").WithPnumber(PersonNummer).WithLoanNumber(LoanNumber).WithLatePaymentBelopp(LatePaymentBelopp);//Fluent Builder             
                //transaction = $"AR;{PersonNummer};{LoanNumber};{LatePaymentBelopp}";
            }
            if (Type == ReportType.Loan)
            {
                rapportBuilder.Addtext("AR").WithPnumber(PersonNummer).WithLoanNumber(LoanNumber).WithLatePaymentBelopp(LoanBelopp);//Fluent Builder   
                //transaction = $"AR;{PersonNummer};{LoanNumber};{LoanBelopp}";
            }
            transaction = rapportBuilder.ToString();
            //Send transaction
            //Dummy --- nothing happens here...
        }
    }
}