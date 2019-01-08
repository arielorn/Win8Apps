using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Win8Apps.App_Start;
using Win8Apps.Extensions;
using Win8Apps.Model;

namespace Win8Apps
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BootstrapBundleConfig.RegisterBundles(System.Web.Optimization.BundleTable.Bundles);

            //InitializeRavenProfiler();

            //TryCreatingIndexesOrRedirectToErrorPage();
            //RavenDbUtils.TryCreatingFacets(DocumentStoreHolder.DocumentStore);
        }

        [Conditional("DEBUG")]
        private void InitializeRavenProfiler()
        {
            //Raven.Client.MvcIntegration.RavenProfiler.InitializeFor(DocumentStoreHolder.DocumentStore);
        }

        private static void TryCreatingIndexesOrRedirectToErrorPage()
        {
            try
            {
                RavenDbUtils.TryCreatingIndexes(DocumentStoreHolder.DocumentStore);
            }
            catch (WebException e)
            {
                var socketException = e.InnerException as SocketException;
                if (socketException == null)
                    throw;

                switch (socketException.SocketErrorCode)
                {
                    case SocketError.AddressNotAvailable:
                    case SocketError.NetworkDown:
                    case SocketError.NetworkUnreachable:
                    case SocketError.ConnectionAborted:
                    case SocketError.ConnectionReset:
                    case SocketError.TimedOut:
                    case SocketError.ConnectionRefused:
                    case SocketError.HostDown:
                    case SocketError.HostUnreachable:
                    case SocketError.HostNotFound:
                        HttpContext.Current.Response.Redirect("~/RavenNotReachable.htm");
                        break;
                    default:
                        throw;
                }
            }
        }
    }
}