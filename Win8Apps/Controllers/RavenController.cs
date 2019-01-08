using System.Web.Mvc;
using Raven.Client;
using Win8Apps.Extensions;

namespace Win8Apps.Controllers
{
    public abstract class RavenDbController : BootstrapBaseController
    {
        private IDocumentSession _documentSession;
        internal IDocumentSession DocumentSession
        {
            get
            {
                return _documentSession ??
                       (_documentSession = DocumentStoreHolder.OpenSession());
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            using (_documentSession)
            {
                //if (_documentSession != null && filterContext.Exception == null)
                //{
                //    _documentSession.SaveChanges();
                //}
            }

            base.OnActionExecuted(filterContext);
        }
    }
}