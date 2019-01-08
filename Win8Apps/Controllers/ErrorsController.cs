using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Win8Apps.Controllers
{
    public class ErrorsController : Controller
    {
        public ActionResult Index()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;
            return View("Http404");
        }

        public ActionResult Http404()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;
            return View();
        }


        public ActionResult Http500()
        {
            Response.StatusCode = 500;
            Response.TrySkipIisCustomErrors = true;
            return View();
        }

    }
}
