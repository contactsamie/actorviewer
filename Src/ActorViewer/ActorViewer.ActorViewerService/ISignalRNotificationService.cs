using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActorViewer.ActorViewerService
{
    public interface ISignalRNotificationService
    {
        void SendSomething(double speed);
    }
}
