using Akka.Actor;
using Microsoft.AspNet.SignalR;
using NLog;

namespace ActorViewer.UIHost
{
    public class ActorViewerHub : Hub
    {
        private IActorRef SignalRNotificationsActorRef { set; get; }

        public ActorViewerHub(IActorRef signalRNotificationsActorRef)
        {
            SignalRNotificationsActorRef = signalRNotificationsActorRef;
        }

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public void PerformOperation(string operation)
        {
            SignalRNotificationsActorRef.Tell(operation);
        }
    }
}