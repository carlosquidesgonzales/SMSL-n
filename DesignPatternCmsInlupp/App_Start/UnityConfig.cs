using DesignPatternCmsInlupp.Repositories;
using DesignPatternCmsInlupp.Services;
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
            container.RegisterType<IRepository, Repository>();
            container.RegisterType<IGetRiksBankensBaseRate, CachedRiksBankensBaseRate>();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}