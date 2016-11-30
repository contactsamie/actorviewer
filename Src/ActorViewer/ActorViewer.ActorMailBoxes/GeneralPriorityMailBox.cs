using Akka.Configuration;
using Akka.Dispatch;

namespace ActorViewer.ActorMailBoxes
{
    public class GeneralPriorityMailBox : UnboundedPriorityMailbox
    {
        protected override int PriorityGenerator(object message)
        {
            return 0;
        }

        public GeneralPriorityMailBox(Akka.Actor.Settings settings, Config config) : base(settings, config)
        {
        }
    }
}