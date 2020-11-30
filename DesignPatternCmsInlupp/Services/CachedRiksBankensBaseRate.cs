using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.Services
{
    public class CachedRiksBankensBaseRate : IGetRiksBankensBaseRate
    {
        private readonly IGetRiksBankensBaseRate _riksBankensBaseRate;
        private static DateTime lastDateTime = DateTime.MinValue;
        private decimal caschedRiksBankensBaseRate;
        public CachedRiksBankensBaseRate(IGetRiksBankensBaseRate riksBankensBaseRate)
        {
            _riksBankensBaseRate = riksBankensBaseRate;
        }
        public decimal GetRiksbankensBaseRate()
        {
            if (lastDateTime == DateTime.MinValue || (DateTime.Now - lastDateTime).TotalMinutes > 60 * 24)
            {
                caschedRiksBankensBaseRate = _riksBankensBaseRate.GetRiksbankensBaseRate();
                lastDateTime = DateTime.Now;
            }
            return caschedRiksBankensBaseRate;
        }
    }
}