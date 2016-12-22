using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActorViewer.ActorExports;


namespace ActorViewer.ActorViewerService
{
    public interface ISignalRNotificationService
    {
        void SendDebugUpdates(QueryDebugUpdatesCompletedMessage debugUpdates);

        void SendLastUpdate(ActorDebugUpdateMessage lattestUpdate);
    }
}
