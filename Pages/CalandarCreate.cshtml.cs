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
        public Boolean IsRecurringMeeting { get; set; }
        [BindProperty]
        public Event Event { get; set; }
        [BindProperty]
        public Attendee Attendee { get; set; }
        [BindProperty]
        public Attendee OptionalAttendee { get; set; }
        private MailboxSettings MailboxSettings { get; set; }

        [BindProperty]
        public int startDateDay { get; set; }
        [BindProperty]
        public int startDateMonth { get; set; }
        [BindProperty]
        public int startDateYear { get; set; }
        [BindProperty]
        public int endDateDay { get; set; }
        [BindProperty]
        public int endDateMonth { get; set; }
        [BindProperty]
        public int endDateYear { get; set; }
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
            List<Attendee> optionalAttendees = new List<Attendee>();
            //add required attendess
            eventDetails.Attendee.EmailAddress.Address = Attendee.EmailAddress.Address;
            //add optional attendees
            eventDetails.OptionalAttendee.EmailAddress.Address = OptionalAttendee.EmailAddress.Address;
            Attendee.Type = AttendeeType.Required;
            eventDetails.Attendee.Type = Attendee.Type;
            OptionalAttendee.Type = AttendeeType.Optional;
            eventDetails.OptionalAttendee.Type = OptionalAttendee.Type;
            //add required and optional attendees to list of attendees
            attendees.Add(eventDetails.Attendee);
            attendees.Add(eventDetails.OptionalAttendee);
            eventDetails.Event.Attendees = attendees;
            eventDetails.Event.Start.TimeZone = userTimeZone;
            eventDetails.Event.End.TimeZone = userTimeZone;
            
            if(IsRecurringMeeting == false)
            {
                eventDetails.Event.Recurrence = null;
                this.Event.Recurrence = null;
            }
            else if(IsRecurringMeeting == true && 
                Event.Recurrence.Pattern.Type == RecurrencePatternType.Daily 
                && Event.Recurrence.Pattern.Interval<1)
            {
                
                eventDetails.Event.Recurrence.Pattern.DaysOfWeek = new Microsoft.Graph.DayOfWeek[] { 
                    Microsoft.Graph.DayOfWeek.Monday,
                    Microsoft.Graph.DayOfWeek.Tuesday,
                    Microsoft.Graph.DayOfWeek.Wednesday,
                    Microsoft.Graph.DayOfWeek.Thursday,
                    Microsoft.Graph.DayOfWeek.Friday};

                eventDetails.Event.Recurrence.Pattern.Interval = 1;
            }
            else if(IsRecurringMeeting == true &&
                Event.Recurrence.Pattern.Type != RecurrencePatternType.Weekly)
            {
                Event.Recurrence.Pattern.DayOfMonth = null;
            }
            else if(IsRecurringMeeting == true && Event.Recurrence.Pattern.Type == RecurrencePatternType.AbsoluteMonthly)
            {
                startDateDay = (int)Event.Recurrence.Pattern.DayOfMonth;
            }
            //validate the start & end dates
            if (startDateDay > 0 && startDateMonth > 0 && startDateYear >= System.DateTime.Now.Year)
            {
                eventDetails.Event.Recurrence.Range.StartDate = new Date(startDateYear, startDateMonth, startDateDay);
            }
            if (endDateDay > 0 && endDateMonth > 0 && endDateYear >= System.DateTime.Now.Year)
            {
                eventDetails.Event.Recurrence.Range.EndDate = new Date(endDateYear, endDateMonth, endDateDay);
            }
            this.Event = _graphCreateCalendarClient.createEvent(eventDetails.Event,userTimeZone);
            if(this.Event != null)
            {
                this.Message = "Hi "+User.Identity.Name+" Meeting Scheduled at specified timings!";
            }
            return RedirectToPage("Calendar");
        }
    }
}
