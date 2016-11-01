using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeigthWatcher.Services;

namespace WeigthWatcher.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ITicketValidator ticketValidator;

        public HomeController(ITicketValidator ticketValidator) : base(ticketValidator)
        {
            this.ticketValidator = ticketValidator;
        }

        public ActionResult Index()
        {
            if (!UserLoggedIn())
            {
                return Redirect("~/Login/Index");
            }

            this.ViewBag.Title = "Titlu";

            return View();
        }

        public ActionResult About()
        {
            if (!UserLoggedIn())
            {
                return Redirect("~/Login/Index");
            }

            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            if (!UserLoggedIn())
            {
                return Redirect("~/Login/Index");
            }

            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}