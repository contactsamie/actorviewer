using System;
using ActorViewer.ActorExports;

namespace ActorViewer.TestActorSystem
{
    public class Program
    {
        private static void Main(string[] args)
        {
             var actorSystem=Akka.Actor.ActorSystem.Create("TestActorSystem");
            ActorRefs.MaryActor= actorSystem.CreateActorInDebugMode<MaryActor>();
            ActorRefs.JohnActor= actorSystem.CreateActorInDebugMode<JohnActor>();
            Console.ReadKey();
        }
    }
}