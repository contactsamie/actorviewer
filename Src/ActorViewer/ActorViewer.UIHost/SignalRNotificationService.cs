using ActorViewer.ActorViewerService;
using Microsoft.AspNet.SignalR;

namespace ActorViewer.UIHost
{
    public class SignalRNotificationService: ISignalRNotificationService
    {

        public void SendSomething(double speed)
        {
            GlobalHost.ConnectionManager.GetHubContext<ActorViewerHub>().Clients.All.messageSpeed(speed);
        }

    }
}