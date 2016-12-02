using ActorViewer.Actors;
using ActorViewer.ActorSystemFactoryLib;
using ActorViewer.UIHost;
using Akka.Actor;
using Autofac;
using Autofac.Integration.SignalR;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.StaticFiles;
using NLog;
using Owin;
using System;
using System.Configuration;
using System.IO;

namespace ActorViewer.UIHostDeployment
{
    public class WebUIDeploymentHost
    {
        public WebUIDeploymentHost()
        {
            ActorSystemFactory = new ActorSystemFactory();
        }

        private ActorSystemFactory ActorSystemFactory { set; get; }
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public void Start(string serverEndPoint = null, string serverActorSystemName = null, ActorSystem serverActorSystem = null, string serverActorSystemConfig = null)
        {
            try
            {
                ActorSystemFactory.CreateOrSetUpActorSystem(serverActorSystemName: serverActorSystemName, actorSystem: serverActorSystem, actorSystemConfig: serverActorSystemConfig);

                var actorViewerActorRef = ActorSystemFactory.ActorViewerActorSystem.ActorOf(Props.Create(() => new ActorViewerActor(new SignalRNotificationService())), typeof(ActorViewerActor).Name);

                Log.Debug("Starting ActorViewer service ...");
                serverEndPoint = serverEndPoint ?? ConfigurationManager.AppSettings["ServerEndPoint"];

                if (!string.IsNullOrEmpty(serverEndPoint))
                {
                    OwinRef = WebApp.Start(serverEndPoint, (appBuilder) =>
                    {
                        if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "/site")) return;
                        var builder = new ContainerBuilder();
                        builder.Register(c => actorViewerActorRef).ExternallyOwned();
                        // Register your SignalR hubs.
                        builder.RegisterType<ActorViewerHub>().ExternallyOwned();

                        var container = builder.Build();
                        GlobalHost.DependencyResolver = new AutofacDependencyResolver(container);
                        appBuilder.UseAutofacMiddleware(container);

                        appBuilder.MapSignalR();

                        var fileSystem = new PhysicalFileSystem(AppDomain.CurrentDomain.BaseDirectory + "/site");
                        var options = new FileServerOptions
                        {
                            EnableDirectoryBrowsing = true,
                            FileSystem = fileSystem,
                            EnableDefaultFiles = true
                        };

                        appBuilder.UseFileServer(options);
                    });
                }

                Log.Debug("WebUIDeployment initialized successfully");
            }
            catch (Exception e)
            {
                Log.Error(e, "Unable to start ActorViewer service UI");
                throw;
            }
        }

        protected IDisposable OwinRef { get; set; }

        public void Stop()
        {
            OwinRef?.Dispose();
        }
    }
}