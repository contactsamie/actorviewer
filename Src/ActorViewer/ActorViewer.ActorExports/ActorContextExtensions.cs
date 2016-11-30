using Akka.Actor;
using Akka.DI.Core;
using System;

namespace ActorViewer.ActorExports
{
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
}