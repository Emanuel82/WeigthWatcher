using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Globalization;

namespace Obalon.Extensions
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString Nl2Br(this HtmlHelper htmlHelper, string text)
        {
            if (string.IsNullOrEmpty(text))
                return MvcHtmlString.Create(text);
            else
            {
                StringBuilder builder = new StringBuilder();
                string[] lines = text.Split('\n');
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i > 0)
                        builder.Append("<br/>\n");
                    builder.Append(HttpUtility.HtmlEncode(lines[i]));
                }
                return MvcHtmlString.Create(builder.ToString());
            }
        }

        public static string FormatForPortal(this HtmlHelper htmlHelper, DateTime? dt, string lang = "en-US")
        {
            if (!dt.HasValue)
                return string.Empty;

            CultureInfo ci = CultureInfo.CreateSpecificCulture(lang);
            return dt.Value.ToString("MM.dd.yy", ci) + "<span> |  " + dt.Value.ToString("hh:mm tt", ci) + "</span>";
        }

        public static string FormatForPortal(this HtmlHelper htmlHelper, DateTime? dt, Obalon.Models.DateTimeFormatInfo datetimeFormatInfo)
        {
            if (!dt.HasValue)
                return string.Empty;

            string dateFormat = "MM.dd.yy";
            string timeFormat = "h:mm tt UTCz";

            string dateOutput = dt.Value.ToString(dateFormat);
            string timeOutput = dt.Value.ToString(timeFormat);

            if (datetimeFormatInfo != null)
            {
                DateTimeOffset dtOffset = new DateTimeOffset(dt.Value);
                dtOffset = dtOffset.ToOffset(TimeSpan.Zero);
                dtOffset = dtOffset.ToOffset(new TimeSpan(datetimeFormatInfo.UtcHours, datetimeFormatInfo.UtcMinutes, 0));
                dateOutput = dtOffset.ToString(dateFormat, CultureInfo.CurrentCulture);
                timeOutput = dtOffset.ToString(timeFormat, CultureInfo.CurrentCulture);
            }

            return dateOutput + "<span> |  " + timeOutput + "</span>";
        }

    }
}