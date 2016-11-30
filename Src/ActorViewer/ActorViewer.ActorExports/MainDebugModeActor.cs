using Akka.Actor;
using System;
using System.Configuration;

namespace ActorViewer.ActorExports
{
    public class MainDebugModeActor : UntypedActor
    {
        private IActorRef WatchedActor { set; get; }
        private Action<object> MessageLogger { set; get; }
        private ActorSelection RemoteActorSelection { set; get; }

        /// <summary>
        /// Requires RemoteActorViewerActorAddress to be in app config
        /// </summary>
        public MainDebugModeActor(Props actorProps, string detDebugModeActorName, Action<object> messageLogger = null)
        {
            var remoteActorAddress = ConfigurationManager.AppSettings["RemoteActorViewerActorAddress"];
            RemoteActorSelection = Context.System.ActorSelection(remoteActorAddress);
            MessageLogger = messageLogger;
            WatchedActor = Context.ActorOf(actorProps, detDebugModeActorName);
            Context.Watch(WatchedActor);

            SendMessageUpdate(GetType().Name + " has now been created ");
        }

        private void SendMessageUpdate(string message)
        {
            string m=null;
            try
            {
                 m = "From " + Sender.Path.ToStringWithUid() + " To " + Self.Path.ToStringWithUid() + " : " + message;
            }
            catch (Exception)
            {
                
            }
            MessageLogger?.Invoke(m?? message);
            RemoteActorSelection?.Tell(m?? message);
        }

        protected override void OnReceive(object message)
        {
            SendMessageUpdate(GetType().Name + " received " + message.GetType().Name + " from " + Sender.Path.ToStringWithUid());
            if (message is Terminated)
            {
                SendMessageUpdate(GetType().Name + " is now going to die ");
                Self.GracefulStop(TimeSpan.FromSeconds(1));
            }
            else
            {
                WatchedActor.Forward(message);
            }
        }
    }
}