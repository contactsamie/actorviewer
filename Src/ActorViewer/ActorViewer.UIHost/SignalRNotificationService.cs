using ActorViewer.ActorViewerMessages;
using ActorViewer.ActorViewerService;
using Microsoft.AspNet.SignalR;

namespace ActorViewer.UIHost
{
    public class SignalRNotificationService: ISignalRNotificationService
    {

        public void SendDebugUpdates(QueryDebugUpdatesCompletedMessage debugUpdates)
        {
            GlobalHost.ConnectionManager.GetHubContext<ActorViewerHub>().Clients.All.messageSpeed(debugUpdates);
        }

        public void SendLastUpdate(ActorDebugUpdateMessage lattestUpdate)
        {
            GlobalHost.ConnectionManager.GetHubContext<ActorViewerHub>().Clients.All.messageSpeed(lattestUpdate);
        }
    }
}