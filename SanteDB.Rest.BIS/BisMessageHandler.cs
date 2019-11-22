using RestSrvr;
using SanteDB.Core;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Interop;
using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Rest.BIS
{
    /// <summary>
    /// Represents a message handler for the BIS
    /// </summary>
    public class BisMessageHandler : IDaemonService, IApiEndpointProvider
    {
        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(BisMessageHandler));

        // The REST host
        private RestService m_webHost;

        /// <summary>
        /// True if the service is running
        /// </summary>
        public bool IsRunning => this.m_webHost?.IsRunning == true;

        /// <summary>
        /// Gets the service name
        /// </summary>
        public string ServiceName => "BIS REST Daemon";

        /// <summary>
        /// Gets the API type
        /// </summary>
        public ServiceEndpointType ApiType => ServiceEndpointType.BusinessIntelligenceService;

        /// <summary>
        /// Gets the urls that this operates on
        /// </summary>
        public string[] Url => this.m_webHost?.Endpoints.Select(o => o.Description.ListenUri.ToString()).ToArray();

        /// <summary>
        /// Gets the capabilities 
        /// </summary>
        public ServiceEndpointCapabilities Capabilities => (ServiceEndpointCapabilities)ApplicationServiceContext.Current.GetService<IRestServiceFactory>().GetServiceCapabilities(this.m_webHost);

        /// <summary>
        /// Gets the type of behavior
        /// </summary>
        public Type BehaviorType => typeof(BisServiceBehavior);

        /// <summary>
        /// Fired when the service is starting
        /// </summary>
        public event EventHandler Starting;
        /// <summary>
        /// Fired when the service has started
        /// </summary>
        public event EventHandler Started;
        /// <summary>
        /// Fired when the service is stopping
        /// </summary>
        public event EventHandler Stopping;
        /// <summary>
        /// Fired when the service has stopped
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Start the service
        /// </summary>
        public bool Start()
        {
            // Don't startup unless in SanteDB
            if (Assembly.GetEntryAssembly().GetName().Name != "SanteDB")
                return true;

            try
            {
                this.Starting?.Invoke(this, EventArgs.Empty);
                this.m_webHost = ApplicationServiceContext.Current.GetService<IRestServiceFactory>().CreateService(typeof(BisServiceBehavior));

                // Add service behaviors
                foreach (ServiceEndpoint endpoint in this.m_webHost.Endpoints)
                {
                    this.m_tracer.TraceInfo("Starting BIS on {0}...", endpoint.Description.ListenUri);
                }
                
                // Start the webhost
                this.m_webHost.Start();

                this.Started?.Invoke(this, EventArgs.Empty);
                return true;
            }
            catch (Exception e)
            {
                this.m_tracer.TraceError(e.ToString());
                return false;
            }
        }

        /// <summary>
        /// Stop the handler
        /// </summary>
        public bool Stop()
        {
            this.Stopping?.Invoke(this, EventArgs.Empty);

            if (this.m_webHost != null)
            {
                this.m_webHost.Stop();
                this.m_webHost = null;
            }

            this.Stopped?.Invoke(this, EventArgs.Empty);

            return true;
        }
    }
}
