using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeigthWatcher.Extensions
{
    public static class RequestExtensions
    {
        public static string GetIpAddress(this HttpRequest request)
        {
            string result = request.UserHostAddress;

            if (request.Headers["X-Forwarded-For"] != null)
            {
                result = request.Headers["X-Forwarded-For"].Split(new[] { ',' }).FirstOrDefault();
            }

            return result;
        }

        public static string GetCookie(this HttpRequestBase request, string cookieName)
        {
            if (request.Cookies[cookieName] != null && request.Cookies[cookieName].Value != null)
            {
                return Uri.UnescapeDataString(request.Cookies[cookieName].Value);
            }

            return null;
        }

        public static string GetCookie(this HttpRequest request, string cookieName)
        {
            if (request.Cookies[cookieName] != null && request.Cookies[cookieName].Value != null)
            {
                return Uri.UnescapeDataString(request.Cookies[cookieName].Value);
            }

            return null;
        }

        public static string GetCookie(this HttpContextBase httpContext, string cookieName)
        {
            if (httpContext.Request.Cookies[cookieName] != null && httpContext.Request.Cookies[cookieName].Value != null)
            {
                return Uri.UnescapeDataString(httpContext.Request.Cookies[cookieName].Value);
            }

            return null;
        }

        public static void DeleteCookie(this HttpContextBase httpContext, string cookieName)
        {
            httpContext.Response.Cookies.Remove(cookieName);
        }

        public static HttpCookie SetCookie(this HttpContextBase httpContext, string cookieName, string cookieValue)
        {
            HttpCookie cookie = null;

            if (httpContext.Request.Cookies.AllKeys.Contains(cookieName))
            {
                cookie = httpContext.Request.Cookies[cookieName];
                cookie.Expires = DateTime.Now.AddDays(-1);
            }

            cookie = new HttpCookie(cookieName, cookieValue);
            cookie.Expires = DateTime.Now.AddMinutes(20); // appSettings ? 

            httpContext.Response.Cookies.Add(cookie);

            return cookie;
        }

        public static string GetBaseUrl(this HttpRequestBase request)
        {
            return request.Url.GetLeftPart(UriPartial.Authority);// +"/" + GetLanguage(request);
        }
    }
}