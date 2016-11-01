using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeigthWatcher.Services;

namespace WeigthWatcher.Controllers
{
    public class LoginController : BaseController
    {
        private readonly ITicketValidator ticketValidator;

        public LoginController(ITicketValidator ticketValidator) : base(ticketValidator)
        {
            this.ticketValidator = ticketValidator;
        }

        // GET: Login
        public ActionResult Index()
        {
            //if (UserLoggedIn())
            //    return RedirectToAction("Index", "Home");

            return View();
        }
    }
}