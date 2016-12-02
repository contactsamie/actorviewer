using System.Collections.Generic;

namespace ActorViewer.ActorViewerMessages
{
    public class QueryDebugUpdatesCompletedMessage
    {
        public QueryDebugUpdatesCompletedMessage(List<ActorDebugUpdateMessage> actorDebugUpdateMessages)
        {
            ActorDebugUpdateMessages = actorDebugUpdateMessages;
        }

        public List<ActorDebugUpdateMessage> ActorDebugUpdateMessages {get; }
    }
}