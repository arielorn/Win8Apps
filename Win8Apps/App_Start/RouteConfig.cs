using System.Web.Mvc;
using System.Web.Routing;
using Win8Apps.Controllers;
using Win8Apps.Model;
using Win8Apps.NavigationRoutes;

namespace Win8Apps.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapNavigationRoute<HomeController>("Home", c => c.Index());

            routes.MapNavigationRoute<AppsController>("Browse", c => c.Search("", 1, null, null, null, null, null, null, null, SortOrder.Desc));

            routes.MapNavigationRoute<StatsController>("Statistics", c => c.Index())
                .AddChildRoute<StatsController>("Apps", c => c.Apps())
                .AddChildRoute<StatsController>("Developers", c => c.Developers())
                ;


            routes.MapRoute(
                "Http404", 
                "{*url}",
                defaults: new { controller = "Errors", action = "Http404", id = UrlParameter.Optional });

        }
    }
}