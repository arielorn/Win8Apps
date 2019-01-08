using System.Web.Routing;

namespace Win8Apps.NavigationRoutes
{
    public interface INavigationRouteFilter
    {
        bool  ShouldRemove(Route navigationRoutes);
    }
}
