using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActorViewer.ActorViewerService;
using Akka.Actor;
using Akka.Event;

namespace ActorViewer.Actors
{
    public class ActorViewerActor : ReceiveActor
    {
        public readonly ILoggingAdapter Logger = Context.GetLogger();
        private ISignalRNotificationService SignalRNotificationService { set; get; }

        public ActorViewerActor(ISignalRNotificationService signalRNotificationService)
        {
            //signalRNotificationService
            ReceiveAny(message =>
            {
                Console.WriteLine("Loging from - "+ Sender.Path.ToStringWithUid()+"  "+message);
            });
        }
    }
}
