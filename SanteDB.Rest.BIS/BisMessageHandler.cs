/*
 * Copyright (C) 2021 - 2023, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors
 * Portions Copyright (C) 2015-2018 Mohawk College of Applied Arts and Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2023-5-19
 */
using RestSrvr;
using SanteDB.Core;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Interop;
using SanteDB.Core.Services;
using SanteDB.Rest.BIS.Configuration;
using SanteDB.Rest.Common.Behavior;
using System;
using System.ComponentModel;
using System.Linq;

namespace SanteDB.Rest.BIS
{
    /// <summary>
    /// Represents a message handler for the BIS
    /// </summary>
    [Description("Exposes the SanteDB Business Intelligence functions (reports, queries, etc.) over HTTP REST")]
    [ApiServiceProvider("Business Intelligence Service", typeof(BisServiceBehavior), ServiceEndpointType.BusinessIntelligenceService, Configuration = typeof(BisServiceConfigurationSection))]
    public class BisMessageHandler : IDaemonService, IApiEndpointProvider
    {
        // Tracer
        private readonly Tracer m_tracer = Tracer.GetTracer(typeof(BisMessageHandler));

        // The REST host
        private RestService m_webHost;

        /// <summary>
        /// True if the service is running
        /// </summary>
        public bool IsRunning => this.m_webHost?.IsRunning == true;

        /// <summary>
        /// Name of the service in the configuration file
        /// </summary>
        public const string ConfigurationName = "BIS";

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
            if (ApplicationServiceContext.Current.HostType == SanteDBHostType.Test)
            {
                return true;
            }

            try
            {
                this.Starting?.Invoke(this, EventArgs.Empty);
                this.m_webHost = ApplicationServiceContext.Current.GetService<IRestServiceFactory>().CreateService(ConfigurationName);

                // Add service behaviors
                foreach (ServiceEndpoint endpoint in this.m_webHost.Endpoints)
                {
                    this.m_tracer.TraceInfo("Starting BIS on {0}...", endpoint.Description.ListenUri);
                }

                this.m_webHost.AddServiceBehavior(new ErrorServiceBehavior());

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