using Microsoft.Graph;

namespace DotNetCoreRazor_MSGraph.Model
{
    public class EventDetails
{
        public Event Event { get; set; }

        public Attendee Attendee { get; set; }
}
}
