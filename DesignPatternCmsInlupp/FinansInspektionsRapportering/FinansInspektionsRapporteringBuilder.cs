using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static DesignPatternCmsInlupp.FinansInspektionsRapportering.Report;

namespace DesignPatternCmsInlupp.FinansInspektionsRapportering
{
    public class FinansInspektionsRapporteringBuilder
    {
        private ReportType _type;
        private decimal _loanBelopp;
        private string _personNummer;
        private string _loanNumber;
        private decimal _avgRta;
        private decimal _latePaymentBelopp;
        public FinansInspektionsRapporteringBuilder WithType(ReportType _type)
        {
            this._type = _type;
            return this;
        }
        public FinansInspektionsRapporteringBuilder WithLoanBelopp(decimal _loanBelopp)
        {
            this._loanBelopp = _loanBelopp;
            return this;
        }
        public FinansInspektionsRapporteringBuilder WithPersonNummer(string _personNummer)
        {
            this._personNummer = _personNummer;
            return this;
        }
        public FinansInspektionsRapporteringBuilder WithLoanNumber(string _loanNumber)
        {
            this._loanNumber = _loanNumber;
            return this;
        }
        public FinansInspektionsRapporteringBuilder WithAvgRta(decimal _avgRta)
        {
            this._avgRta = _avgRta;
            return this;
        }
        public FinansInspektionsRapporteringBuilder WithLatePaymenyBelopp(decimal _latePaymentBelopp)
        {
            this._latePaymentBelopp = _latePaymentBelopp;
            return this;
        }
        public Report Build()
        {
            var report = new Report(
                _type,
                _personNummer,
                _loanNumber,
                _avgRta,
                _loanBelopp,
                _latePaymentBelopp);
            return report;
        }
    }
}