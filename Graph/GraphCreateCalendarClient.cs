using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using System.Threading.Tasks;
using DotNetCoreRazor_MSGraph.Pages;
using System.Collections;
using System.Collections.Generic;
using System;

namespace DotNetCoreRazor_MSGraph.Graph
{
    public class GraphCreateCalendarClient
    {
        private readonly ILogger<GraphCreateCalendarClient> _logger = null;
        private readonly GraphServiceClient _graphServiceClient = null;
        public GraphCreateCalendarClient(
            ILogger<GraphCreateCalendarClient> logger,
            GraphServiceClient graphServiceClient)
        {
            this._logger = logger;
            this._graphServiceClient = graphServiceClient;
        }

        public Event createEvent(Event model, string userTimeZone)
        {
            Random random = new Random();
            int randomNumber = random.Next(0, 100);
            model.IsOnlineMeeting = true;
            model.ReminderMinutesBeforeStart = 5;
            model.IsReminderOn = true;
            model.TransactionId = model.TransactionId+ randomNumber;
            //model.Recurrence.Range.StartDate = new Date(2023, 03, 21);
            var Event = model;
            var result = _graphServiceClient.
                Me.Events.
                Request().Header("Prefer", $"outlook.timezone=\"{userTimeZone}\"").AddAsync(Event).Result;
            return result;
        }

        

    }//class
}//namespace
