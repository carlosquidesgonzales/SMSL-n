using DesignPatternCmsInlupp.Controllers;
using DesignPatternCmsInlupp.Repositories;
using DesignPatternCmsInlupp.Services;
using System;
using System.Web.Mvc;
using Unity;
using Unity.Mvc5;

namespace DesignPatternCmsInlupp
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
           
            var container = new UnityContainer();
            container.RegisterType<ICustomerRepository, CustomerRepository>();
            container.RegisterType<ILoanRepository, LoanRepository>();
            container.Resolve<HomeController>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

       
    }
}