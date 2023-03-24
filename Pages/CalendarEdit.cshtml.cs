using DotNetCoreRazor_MSGraph.Graph;
using DotNetCoreRazor_MSGraph.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;
using System;
using System.Threading.Tasks;

namespace DotNetCoreRazor_MSGraph.Pages
{ 
    public class CalendarEditModel : PageModel
    {
       [BindProperty]
        public Event Event { get; set; }
       
        //public Attendee Attendee { get; set; }
        public String updateResult { get; set; }
        private MailboxSettings MailboxSettings { get; set; }
        private GraphUpdateCalendarClient _graphUpdateCalendarClient;
        private GraphCalendarClient _graphCalendarClient;

        public CalendarEditModel(GraphUpdateCalendarClient graphUpdateCalendarClient,
            GraphCalendarClient graphCalendarClient)
        {
            this._graphUpdateCalendarClient = graphUpdateCalendarClient;
            this._graphCalendarClient = graphCalendarClient;
        }

        public void OnGetEdit(String evtid)
        {
            this.Event = _graphUpdateCalendarClient.editEvent(evtid);
            
        }
        public async Task<IActionResult> OnPostUpdate()
        {
            MailboxSettings = await _graphCalendarClient.GetUserMailboxSettings();
            var userTimeZone = (String.IsNullOrEmpty(MailboxSettings.TimeZone))
                ? "Pacific Standard Time"
                : MailboxSettings.TimeZone;
            this.Event.Start.TimeZone = userTimeZone;
            this.Event.End.TimeZone = userTimeZone;
            this.Event = _graphUpdateCalendarClient.updateEvent(this.Event);
            return RedirectToPage("Calendar");
        }

        public string FormatDateTimeTimeZone(DateTimeTimeZone value)
        {
            // Parse the date/time string from Graph into a DateTime
            var graphDatetime = value.DateTime;
            if (DateTime.TryParse(graphDatetime, out DateTime dateTime))
            {
                var dateTimeFormat = $"{MailboxSettings.DateFormat} {MailboxSettings.TimeFormat}".Trim();
                if (!String.IsNullOrEmpty(dateTimeFormat))
                {
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
    }
}
