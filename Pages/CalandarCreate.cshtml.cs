using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Graph;
using DotNetCoreRazor_MSGraph.Graph;
using DotNetCoreRazor_MSGraph.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetCoreRazor_MSGraph.Pages
{
    public class CalandarCreateModel : PageModel
    {
        [BindProperty]
        public Event Event { get; set; }
        [BindProperty]
        public Attendee Attendee { get; set; }
        private MailboxSettings MailboxSettings { get; set; }
        public String Message { get; set; }
        public GraphCreateCalendarClient _graphCreateCalendarClient;
        public GraphCalendarClient _graphCalendarClient;
        public CalandarCreateModel(GraphCreateCalendarClient graphCreateCalendarClient,
            GraphCalendarClient graphCalendarClient)
        {
            this._graphCreateCalendarClient = graphCreateCalendarClient;
            this._graphCalendarClient = graphCalendarClient;
        }
        public async Task<IActionResult> OnPostSubmit(EventDetails eventDetails)
        {
            this.Message = null;
            MailboxSettings = await _graphCalendarClient.GetUserMailboxSettings();
            var userTimeZone = (String.IsNullOrEmpty(MailboxSettings.TimeZone))
                ? "Pacific Standard Time"
                : MailboxSettings.TimeZone;
            List<Attendee> attendees = new List<Attendee>();
            eventDetails.Attendee = Attendee;
            attendees.Add(eventDetails.Attendee);
            eventDetails.Event.Attendees = attendees;
            eventDetails.Event.Start.TimeZone = userTimeZone;
            eventDetails.Event.End.TimeZone = userTimeZone;
            this.Event = _graphCreateCalendarClient.createEvent(eventDetails.Event,userTimeZone);
            if(this.Event != null)
            {
                this.Message = "Meeting Scheduled at specified timings!";
            }
            return RedirectToPage("Calendar");
        }
    }
}
