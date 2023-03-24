using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreRazor_MSGraph.Graph;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace DotNetCoreRazor_MSGraph.Pages
{
    [AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
    public class CalendarModel : PageModel
    {
        private readonly ILogger<CalendarModel> _logger;
        private readonly GraphCalendarClient _graphCalendarClient;
        private readonly GraphProfileClient _graphProfileClient;
        public String deleteResult { get; set; }

        public String editResult { get; set; }
        private MailboxSettings MailboxSettings { get; set; }

        public IEnumerable<Event> Events  { get; private set; }

       //public Event Event { get; set; }

        public CalendarModel(ILogger<CalendarModel> logger, GraphCalendarClient graphCalendarClient, GraphProfileClient graphProfileClient)
        {
            _logger = logger;
            _graphCalendarClient = graphCalendarClient;
            _graphProfileClient = graphProfileClient;
        }

        public async Task OnGetAsync()
        {
            MailboxSettings = await _graphCalendarClient.GetUserMailboxSettings();
            var userTimeZone = (String.IsNullOrEmpty(MailboxSettings.TimeZone))
                ? "Pacific Standard Time"
                : MailboxSettings.TimeZone;
            Events = await _graphCalendarClient.GetEvents(userTimeZone);
        }

        public string FormatDateTimeTimeZone(DateTimeTimeZone value)
        {
            // Parse the date/time string from Graph into a DateTime
            var graphDatetime = value.DateTime;
            if (DateTime.TryParse(graphDatetime, out DateTime dateTime)) 
            {
                var dateTimeFormat = $"{MailboxSettings.DateFormat} {MailboxSettings.TimeFormat}".Trim();
                if (!String.IsNullOrEmpty(dateTimeFormat)) {
                    return dateTime.ToString(dateTimeFormat);
                }
                else 
                {
                    return $"{dateTime.ToShortDateString()} {dateTime.ToShortTimeString()}";
                }
            }
            else
            {
                return graphDatetime;
            }
        }

        public IActionResult OnGetDelete(String evtid)
        {
            this.deleteResult = _graphCalendarClient.deleteEvent(evtid);
            return RedirectToPage("Calendar");
            
        }



    }
}
