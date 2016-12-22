using ActorViewer.ActorExports;

using ActorViewer.ActorViewerService;
using Microsoft.AspNet.SignalR;

namespace ActorViewer.UIHost
{
    public class SignalRNotificationService: ISignalRNotificationService
    {

        public void SendDebugUpdates(QueryDebugUpdatesCompletedMessage debugUpdates)
        {
            GlobalHost.ConnectionManager.GetHubContext<ActorViewerHub>().Clients.All.updateLog(debugUpdates);
        }

        public void SendLastUpdate(ActorDebugUpdateMessage lattestUpdate)
        {
            GlobalHost.ConnectionManager.GetHubContext<ActorViewerHub>().Clients.All.updateLog(lattestUpdate);
        }
    }
}