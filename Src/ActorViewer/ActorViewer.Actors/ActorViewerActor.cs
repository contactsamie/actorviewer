using ActorViewer.ActorViewerMessages;
using ActorViewer.ActorViewerService;
using Akka.Actor;
using Akka.Event;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ActorViewer.Actors
{
    public class ActorViewerActor : ReceiveActor
    {
        public readonly ILoggingAdapter Logger = Context.GetLogger();
        private ISignalRNotificationService SignalRNotificationService { set; get; }
        public List<ActorDebugUpdateMessage> ActorDebugUpdateMessages { set; get; }
        public ActorViewerActor(ISignalRNotificationService signalRNotificationService)
        {
            ActorDebugUpdateMessages = new List<ActorDebugUpdateMessage>();
            SignalRNotificationService = signalRNotificationService;
            Receive<ActorDebugUpdateMessage>(message =>
            {
                ActorDebugUpdateMessages.Add(message);
            });
            Receive<UpdateClients>(_ =>
            {
                var last = ActorDebugUpdateMessages.Count - 1;
                if (last < 0) return;

                var  lattestUpdate= ActorDebugUpdateMessages[last];
                SignalRNotificationService.SendLastUpdate(lattestUpdate);
                Console.WriteLine(ActorDebugUpdateMessages.Count + " Messages so far - " + lattestUpdate?.ToString());
            });
            Receive<QueryDebugUpdatesMessage>(message =>
            {
                var resultSet =
                    ActorDebugUpdateMessages/*.Where(x => x.ReceivedOn >= message.From && x.ReceivedOn <= message.To)*/
                        .Skip(message.Skip)
                        .Take(message.Take)
                        .ToList();
                Sender.Tell(new QueryDebugUpdatesCompletedMessage(resultSet));
                SignalRNotificationService.SendDebugUpdates(new QueryDebugUpdatesCompletedMessage(resultSet));
            });
            ReceiveAny(Console.WriteLine);
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3), Self, new UpdateClients(), Self);
        }
    }
}