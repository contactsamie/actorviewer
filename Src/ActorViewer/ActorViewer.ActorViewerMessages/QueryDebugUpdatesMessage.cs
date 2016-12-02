using System;

namespace ActorViewer.ActorViewerMessages
{
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
}