using System.Collections.Generic;
using System.Threading.Tasks;
using ActorViewer.ActorViewerMessages;
using Akka.Event;
using Topshelf;

namespace ActorViewer.UIHostDeployment
{
    class Program
    {
        private static void Main(string[] args)
        {
            HostFactory.Run(x =>                                 //1
            {
                x.Service<WebUIDeploymentHost>(s =>                        //2
                {
                    s.ConstructUsing(name => new WebUIDeploymentHost());     //3
                    s.WhenStarted(tc => tc.Start());              //4
                    s.WhenStopped(tc => tc.Stop());               //5
                });
                x.RunAsLocalSystem();                            //6
                x.UseNLog();
                x.SetDescription("ActorViewer MicroService UI");        //7
                x.SetDisplayName("ActorViewer Service UI");                       //8
                x.SetServiceName("ActorViewerService UI");                   //9
            });                                                  //10
        }
    }
}
