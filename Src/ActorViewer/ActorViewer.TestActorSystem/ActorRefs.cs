using Akka.Actor;

namespace ActorViewer.TestActorSystem
{
    public static class ActorRefs
    {
        public static IActorRef JohnActor { set; get; }
        public static IActorRef MaryActor { set; get; }
        public static IActorRef JohnChildActor { set; get; }
        public static IActorRef MaryChildActor { set; get; }
    }
}