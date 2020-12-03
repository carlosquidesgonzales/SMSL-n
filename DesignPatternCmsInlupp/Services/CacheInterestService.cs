using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DesignPatternCmsInlupp.Services
{
    public class CacheInterestService : IInterestService
    {
        private readonly IInterestService _interestService;
        private static DateTime lastDateTime = DateTime.MinValue;
        private decimal caschedRiksBankensBaseRate;
        public CacheInterestService(IInterestService interestService)
        {
            _interestService = interestService;
        }
        public decimal GetRiksbankensBaseRate()
        {
            if (lastDateTime == DateTime.MinValue || (DateTime.Now - lastDateTime).TotalMinutes > 60 * 24)
            {
                caschedRiksBankensBaseRate = _interestService.GetRiksbankensBaseRate();
                lastDateTime = DateTime.Now;
            }
            return caschedRiksBankensBaseRate;
        }
    }
}