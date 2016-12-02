using ActorViewer.ActorViewerMessages;
using Akka.Actor;
using Microsoft.AspNet.SignalR;
using NLog;

namespace ActorViewer.UIHost
{
    public class ActorViewerHub : Hub
    {
        private IActorRef ActorViewerActorRef { set; get; }
     

        public ActorViewerHub(IActorRef actorViewerActorRef)
        {
            ActorViewerActorRef = actorViewerActorRef;
        }

        public void QueryDebugUpdates(QueryDebugUpdatesMessage operation)
        {
            ActorViewerActorRef.Tell(operation);
        }
    }
}