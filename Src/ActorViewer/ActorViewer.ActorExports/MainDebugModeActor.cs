using Akka.Actor;
using Akka.Routing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

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
                , JsonConvert.SerializeObject(message)
                , message.GetType().FullName
                , extraInfo
                , messageNature);
            return load;
        }
    }

    public class ActorSetUpOptions
    {
        public RouterConfig RouterConfig { get; set; }
        public SupervisorStrategy SupervisoryStrategy { get; set; }
        public string Dispatcher { get; set; }
        public string MailBox { get; set; }
    }

    public static class ActorContextExtensions
    {
        /// <summary>
        /// Requires RemoteActorViewerActorAddress to be in app config in order to call CreateActorInDebugMode //todo : pass address from here
        /// </summary>
        public static IActorRef CreateActorInDebugMode<TActorType>(this ActorSystem system, ActorSetUpOptions options = null, Action<object> messageLogger = null, string debugerPrefix = "DEBUG_") where TActorType : ActorBase
        {
            var actorName = typeof(TActorType).Name;
            return system.ActorOf(Props.Create(() => new MainDebugModeActor(GetActorProps<TActorType>(system, options), debugerPrefix + actorName, messageLogger)), actorName);
        }

        public static IActorRef CreateActorInDebugMode(this ActorSystem system,Props actorProps,string actorName,  Action<object> messageLogger = null, string debugerPrefix = "DEBUG_")
        {

            return system.ActorOf(Props.Create(() => new MainDebugModeActor(actorProps, debugerPrefix + actorName, messageLogger)), actorName);
        }
        public static IActorRef CreateActorInDebugMode(this IActorContext context, Props actorProps, string actorName, Action<object> messageLogger = null, string debugerPrefix = "DEBUG_") 
        {
            return context.ActorOf(Props.Create(() => new MainDebugModeActor(actorProps, debugerPrefix + actorName, messageLogger)), actorName);
        }
        /// <summary>
        /// Requires RemoteActorViewerActorAddress to be in app config in order to call CreateActorInDebugMode //todo : pass address from here
        /// </summary>
        public static IActorRef CreateActorInDebugMode<TActorType>(this IUntypedActorContext context, ActorSetUpOptions options = null, Action<object> messageLogger = null, string debugerPrefix = "DEBUG_") where TActorType : ActorBase
        {
            var actorName = typeof(TActorType).Name;
            return context.ActorOf(Props.Create(() => new MainDebugModeActor(GetActorProps<TActorType>(context.System, options), debugerPrefix + actorName, messageLogger)), actorName);
        }

        public static Props GetActorProps<TActor>(ActorSystem system, ActorSetUpOptions options = null)
        {
            var props = Props.Create(typeof(TActor));

            props = PrepareProps(options, props);

            return props;
        }

        public static Props PrepareProps(ActorSetUpOptions options, Props props)
        {
            if (options == null) return props;
            if (options.RouterConfig != null)
            {
                props = props.WithRouter(options.RouterConfig);
            }
            if (options.SupervisoryStrategy != null)
            {
                props = props.WithSupervisorStrategy(options.SupervisoryStrategy);
            }
            if (options.Dispatcher != null)
            {
                props = props.WithDispatcher(options.Dispatcher);
            }
            if (options.MailBox != null)
            {
                props = props.WithMailbox(options.MailBox);
            }
            return props;
        }
    }

    public class ActorDebugUpdateMessage
    {
        public ActorDebugUpdateMessage(string actorSystemInformation, string sourceActor, string sourceDescription, string destinationActor, string destinationDescription, string messageType, object message, string messageFullType, object extraInformation, MessageNature messageNature)
        {
            SourceActor = sourceActor;
            SourceDescription = sourceDescription;
            DestinationActor = destinationActor;
            DestinationDescription = destinationDescription;
            MessageType = messageType;
            Message = message;
            MessageFullType = messageFullType;
            ExtraInformation = extraInformation;
            MessageNature = Enum.GetName(typeof(MessageNature), messageNature);
            ActorSystemInformation = actorSystemInformation;
            ReceivedOn = DateTime.UtcNow;
        }

        public string ActorSystemInformation { get; }
        public string SourceActor { get; }
        public string SourceDescription { get; }
        public string DestinationActor { get; }
        public string DestinationDescription { get; }
        public string MessageType { get; }
        public object Message { get; }
        public string MessageFullType { get; }
        public DateTime ReceivedOn { get; }
        public object ExtraInformation { get; }
        public string MessageNature { get; }

        public List<string> GetDestinationPath()
        {
            return GetPath(DestinationActor);
        }

        public List<string> GetSourcePath()
        {
            return GetPath(SourceActor);
        }

        private List<string> GetPath(string path)
        {
            return path.Split('/').ToList();
        }

        public override string ToString()
        {
            var propertyInfos = GetType().GetProperties();
            var sb = new StringBuilder();
            foreach (var info in propertyInfos)
            {
                var value = info.GetValue(this, null) ?? "(null)";
                sb.AppendLine(info.Name + ": " + value);
            }
            return sb.ToString();
        }
    }

    public class QueryDebugUpdatesCompletedMessage
    {
        public QueryDebugUpdatesCompletedMessage(List<ActorDebugUpdateMessage> actorDebugUpdateMessages)
        {
            ActorDebugUpdateMessages = actorDebugUpdateMessages;
        }

        public List<ActorDebugUpdateMessage> ActorDebugUpdateMessages { get; }
    }

    public class QueryDebugUpdatesMessage
    {
        public QueryDebugUpdatesMessage(DateTime @from, DateTime to, int take, int skip)
        {
            From = @from;
            To = to;
            Take = take;
            Skip = skip;
        }

        public DateTime From { get; }
        public DateTime To { get; }
        public int Take { get; }
        public int Skip { get; }
    }

    public enum MessageNature
    {
        Unknown = 0,
        Received,
        Initialization
    }
}