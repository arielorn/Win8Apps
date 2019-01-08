using System.Web.Http;
using Raven.Client;
using Win8Apps.Extensions;

namespace Win8Apps.Controllers.Api
{
    public abstract class RavenDbApiController : ApiController
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
        public override System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> ExecuteAsync(System.Web.Http.Controllers.HttpControllerContext controllerContext, System.Threading.CancellationToken cancellationToken)
        {
            var result = base.ExecuteAsync(controllerContext, cancellationToken);

            using (_documentSession)
            {
                //if (_documentSession != null && filterContext.Exception == null)
                //{
                //    _documentSession.SaveChanges();
                //}
            }

            return result;
        }
    }
}