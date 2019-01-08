using System;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using Win8Apps.Models;
using MailMessage = System.Net.Mail.MailMessage;

namespace Win8Apps.Controllers
{
    public class HomeController : BootstrapBaseController
    {
        public ActionResult Index()
        {
            return View();
        }


		public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult Advertise()
        {
            return View();
        }

        public ActionResult ContactUs()
        {
            return View();
        }

        public ActionResult FacebookTags()
        {
            return PartialView();
        }


        [HttpPost]
        public ActionResult ContactUs(ContactUsViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }
            using (var client = new SmtpClient())
            {
                var message = new MailMessage();
                message.To.Add(new MailAddress("itaysa@gmail.com", "Itay Sagui"));
                message.To.Add(new MailAddress("arielorn@gmail.com", "Ariel Ornstein"));

                message.Body = string.Format("Name : {0} <{1}>\r\n\t\n{2}", viewModel.Name, viewModel.Email, viewModel.Message);
                message.Subject = "WinApps: " + viewModel.Subject;

                try
                {
                    client.Send(message);
                }
                catch (Exception)
                {
                }
            }

            return View();
        }

    }
}
