using System.Web.Mvc;
using System.Web.Routing;

namespace DapperRepository.Web.Infrastructure
{
    public static class RouteProvider
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Customer", action = "List", id = UrlParameter.Optional }
            );
        }
    }
}