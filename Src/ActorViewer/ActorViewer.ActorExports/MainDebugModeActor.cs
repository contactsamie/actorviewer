using ActorViewer.ActorViewerMessages;
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
            SendMessageUpdate(GetLoadToSend(GetType().Name, MessageNature.Initialization, GetType().Name + " actor has been constructed"));
        }

        private void SendMessageUpdate(ActorDebugUpdateMessage message)
        {
            MessageLogger?.Invoke(message);
            RemoteActorSelection?.Tell(message);
        }

        protected override void OnReceive(object message)
        {
            var load = GetLoadToSend(message, MessageNature.Received, " received " + message.GetType().Name + " from " + Sender.Path.ToStringWithUid());

            SendMessageUpdate(load);

            if (message is Terminated)
            {
                Self.GracefulStop(TimeSpan.FromSeconds(1));
            }
            else
            {
                WatchedActor.Forward(message);
            }
        }

        private ActorDebugUpdateMessage GetLoadToSend(object message, MessageNature messageNature, string extraInfo = "")
        {
            var load = new ActorDebugUpdateMessage(
                Context.System.Name
                , Sender?.Path?.ToStringWithAddress()
                , Sender?.Path?.ToStringWithUid()
                , Self.Path.ToStringWithAddress()
                , Self.Path.ToStringWithUid()
                , message.GetType().Name
                , message
                , message.GetType().FullName
                , extraInfo
                , messageNature);
            return load;
        }
    }
}