using Microsoft.Graph;
using System;

namespace DotNetCoreRazor_MSGraph.Graph
{
    public class GraphUpdateCalendarClient
{
        private GraphServiceClient _graphServiceClient;
        public GraphUpdateCalendarClient(GraphServiceClient graphServiceClient)
        {
            this._graphServiceClient = graphServiceClient;  
        }

        public Event editEvent(String eventId)
        {
            var evnt = _graphServiceClient.Me.Events[eventId].Request().GetAsync().Result;
            return evnt;
        }

        
        public  Event updateEvent(Event evt)
        {
            Event evnt = _graphServiceClient.Me.Events[evt.Id].Request().GetAsync().Result;
            if(evnt != null)
            {
               evnt = _graphServiceClient.Me.Events[evnt.Id].Request().UpdateAsync(evt).GetAwaiter().GetResult();
            }
            return evnt;
        }
}
}
