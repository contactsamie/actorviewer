using ActorViewer.ActorExports;
using Akka.Actor;
using System;

namespace ActorViewer.TestActorSystem
{
    internal class JohnActor : ReceiveActor
    {
        public JohnActor()
        {
            ReceiveAny(message =>
            {
                Console.WriteLine("REAL " + GetType().Name + " received a message : " + message);
            });

            ActorRefs.JohnChildActor = Context.CreateActorInDebugMode<JohnChildActor>();
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(3), ActorRefs.MaryActor, DateTime.Now.ToLongDateString(), Self);
        }
    }

    internal class JohnChildActor : ReceiveActor
    {
        public JohnChildActor()
        {
            ReceiveAny(message =>
            {
                Console.WriteLine("REAL " + GetType().Name + " received a message : " + message);
            });

            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(12), TimeSpan.FromSeconds(3), ActorRefs.MaryChildActor, DateTime.Now.ToLongDateString(), Self);
        }
    }

    public class MaryActor : ReceiveActor
    {
        public MaryActor()
        {
            ReceiveAny(message =>
            {
                Console.WriteLine("REAL " + GetType().Name + " received a message : " + message);
            });

            ActorRefs.MaryChildActor = Context.CreateActorInDebugMode<MaryChildActor>();
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(3), ActorRefs.JohnActor, DateTime.Now.ToLongDateString(), Self);
        }
    }

    public class MaryChildActor : ReceiveActor
    {
        public MaryChildActor()
        {
            ReceiveAny(message =>
            {
                Console.WriteLine("REAL " + GetType().Name + " received a message : " + message);
            });

            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(12), TimeSpan.FromSeconds(3), ActorRefs.JohnChildActor, DateTime.Now.ToLongDateString(), Self);
        }
    }
}