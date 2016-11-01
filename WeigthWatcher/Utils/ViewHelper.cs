using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using WeigthWatcher.Services;
using WeigthWatcher.Extensions;


namespace WeigthWatcher.Utils
{
    public class ViewHelper
    {
        private static ViewHelper instance;
        private ITicketValidator ticketValidator;

        public static ViewHelper Instance
        {
            get
            {
                if (instance == null)
                    instance = new ViewHelper();

                return instance;
            }
        }


        private ViewHelper()
        {
            ticketValidator = new TicketValidator();
        }


        public PortalController GetCurrentController(string controllerRouteValue)
        {
            PortalController result = PortalController.None;

            if (Regex.IsMatch(controllerRouteValue, "patient", RegexOptions.IgnoreCase))
            {
                result = PortalController.Patient;
            }
            else if (Regex.IsMatch(controllerRouteValue, "home", RegexOptions.IgnoreCase))
            {
                result = PortalController.Home;
            }
            else if (Regex.IsMatch(controllerRouteValue, "events", RegexOptions.IgnoreCase))
            {
                result = PortalController.Events;
            }

            return result;
        }

        public string GetUserIdFromTicket(HttpRequestBase request)
        {
            string ticket = request.GetCookie("ticket");

            return ticketValidator.GetUserIdFromTicket(ticket);
        }

        public bool UserLoggedIn(HttpRequestBase request)
        {
            string userId = GetUserIdFromTicket(request);
            return !string.IsNullOrEmpty(userId);
        }
    }
}