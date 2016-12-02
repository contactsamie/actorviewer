using System;
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
        public void GetList()
        {
            ActorViewerActorRef.Tell(new QueryDebugUpdatesMessage(DateTime.UtcNow.AddYears(-365), DateTime.UtcNow, 10000,0 ));
        }
        public void PerformOperation(QueryDebugUpdatesMessage operation)
        {
           // ActorViewerActorRef.Tell(operation);
        }
    }
}