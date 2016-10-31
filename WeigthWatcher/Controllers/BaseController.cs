using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeigthWatcher.Services;
using WeigthWatcher.Extensions;

namespace WeigthWatcher.Controllers
{
    public class BaseController : Controller
    {
        private readonly ITicketValidator ticketValidator;

        public BaseController(ITicketValidator ticketValidator)
        {
            this.ticketValidator = ticketValidator;
        }


        protected string GetUserIdFromTicket()
        {
            string userId = string.Empty;

            string ticket = Request.GetCookie("ticket");

            try
            {
                userId = ticketValidator.GetUserIdFromTicket(ticket);
            }
            catch (Exception ex)
            {
                //logger.Info(string.Join(" ", "Error while retrieving userId from ticket", Regex.Replace(ex.Message, "\n|\r\n", " ")));
            }

            return userId;
        }


        protected bool UserLoggedIn()
        {
            string userId = GetUserIdFromTicket();
            return !string.IsNullOrEmpty(userId);
        }


    }
}