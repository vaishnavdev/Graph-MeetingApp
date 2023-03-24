using Microsoft.Graph;
using System.Collections.Generic;

namespace DotNetCoreRazor_MSGraph.Model
{
    public class EventDetails
{
        public Event Event { get; set; }

        public Attendee Attendee { get; set; }

        public Attendee OptionalAttendee { get; set; }

        public RecurrenceRange RecurrenceRange { get; set; }

}
}
