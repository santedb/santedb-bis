﻿/*
 * Copyright (C) 2021 - 2025, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-6-21
 */
using SanteDB.BI.Configuration;
using SanteDB.BI.Services.Impl;
using SanteDB.Core.Configuration;
using SanteDB.Core.Exceptions;
using SanteDB.Docker.Core;
using SanteDB.Rest.Common.Behavior;
using SanteDB.Rest.Common.Configuration;
using SanteDB.Rest.Common.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SanteDB.Rest.BIS
{
    /// <summary>
    /// A docker feature for the AMI
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class BisDockerFeature : IDockerFeature
    {

        /// <summary>
        /// Setting ID for listen address
        /// </summary>
        public const string ListenUriSetting = "LISTEN";
        /// <summary>
        /// Setting ID for CORS enable
        /// </summary>
        public const string CorsSetting = "CORS";
        /// <summary>
        /// Set ID for authentication
        /// </summary>
        public const string AuthenticationSetting = "AUTH";

        /// <summary>
        /// Max results setting
        /// </summary>
        public const string MaxResultsSetting = "MAX_RESULTS";

        /// <summary>
        /// Map the settings to the authentication behavior
        /// </summary>
        private readonly IDictionary<String, Type> authSettings = new Dictionary<String, Type>()
        {
            { "TOKEN", typeof(TokenAuthorizationAccessBehavior) },
            { "BASIC", typeof(BasicAuthorizationAccessBehavior) },
            { "APP_BASIC", typeof(BasicApplicationAuthorizationAccessBehavior) },
            { "NONE", null }
        };

        /// <summary>
        /// Get the id of the docker feature
        /// </summary>
        public string Id => "BIS";

        /// <summary>
        /// Get the settings
        /// </summary>
        public IEnumerable<string> Settings => new String[] { AuthenticationSetting, ListenUriSetting, CorsSetting, MaxResultsSetting };

        /// <summary>
        /// Create an endpoint config
        /// </summary>
        private RestEndpointConfiguration CreateEndpoint(String endpointUrl) => new Rest.Common.Configuration.RestEndpointConfiguration()
        {
            Address = endpointUrl,
            Contract = typeof(IBisServiceContract),
            Behaviors = new List<Rest.Common.Configuration.RestEndpointBehaviorConfiguration>()
                            {
                                new Rest.Common.Configuration.RestEndpointBehaviorConfiguration(typeof(MessageLoggingEndpointBehavior)),
                                new Rest.Common.Configuration.RestEndpointBehaviorConfiguration(typeof(MessageCompressionEndpointBehavior)),
                                new Rest.Common.Configuration.RestEndpointBehaviorConfiguration(typeof(MessageDispatchFormatterBehavior)),
                                new RestEndpointBehaviorConfiguration(typeof(AcceptLanguageEndpointBehavior))
                            }
        };

        /// <summary>
        /// Configure the feature
        /// </summary>
        public void Configure(SanteDBConfiguration configuration, IDictionary<string, string> settings)
        {
            var restConfiguration = configuration.GetSection<SanteDB.Rest.Common.Configuration.RestConfigurationSection>();
            if (restConfiguration == null)
            {
                throw new ConfigurationException("Error retrieving REST configuration", configuration);
            }

            var hdsiRestConfiguration = restConfiguration.Services.FirstOrDefault(o => o.ServiceType == typeof(IBisServiceContract));
            if (hdsiRestConfiguration == null) // add fhir rest config
            {
                hdsiRestConfiguration = new Rest.Common.Configuration.RestServiceConfiguration()
                {
                    Behaviors = new List<Rest.Common.Configuration.RestServiceBehaviorConfiguration>()
                    {
                        new Rest.Common.Configuration.RestServiceBehaviorConfiguration(typeof(TokenAuthorizationAccessBehavior))
                    },
                    ServiceType = typeof(BisServiceBehavior),
                    ConfigurationName = BisMessageHandler.ConfigurationName,
                    Endpoints = new List<Rest.Common.Configuration.RestEndpointConfiguration>()
                    {
                        this.CreateEndpoint("http://0.0.0.0:8080/bis")
                    }
                };
                restConfiguration.Services.Add(hdsiRestConfiguration);
            }


            // Listen address
            if (settings.TryGetValue(ListenUriSetting, out string listen))
            {
                if (!Uri.TryCreate(listen, UriKind.Absolute, out Uri listenUri))
                {
                    throw new ArgumentOutOfRangeException($"{listen} is not a valid URL");
                }

                // Setup the endpoint
                hdsiRestConfiguration.Endpoints.Clear();
                hdsiRestConfiguration.Endpoints.Add(this.CreateEndpoint(listen));
            }

            // Authentication
            if (settings.TryGetValue(AuthenticationSetting, out string auth))
            {
                if (!this.authSettings.TryGetValue(auth.ToUpperInvariant(), out Type authType))
                {
                    throw new ArgumentOutOfRangeException($"Don't understand auth option {auth} allowed values {String.Join(",", this.authSettings.Keys)}");
                }

                // Add behavior
                if (authType != null)
                {
                    hdsiRestConfiguration.Behaviors.Add(new RestServiceBehaviorConfiguration() { Type = authType });
                }
                else
                {
                    hdsiRestConfiguration.Behaviors.RemoveAll(o => this.authSettings.Values.Any(v => v == o.Type));
                }
            }

            // Has the user set CORS?
            if (settings.TryGetValue(CorsSetting, out string cors))
            {
                if (!Boolean.TryParse(cors, out bool enabled))
                {
                    throw new ArgumentOutOfRangeException($"{cors} is not a valid boolean value");
                }

                // Cors is disabled?
                if (!enabled)
                {
                    hdsiRestConfiguration.Endpoints.ForEach(ep => ep.Behaviors.RemoveAll(o => o.Type == typeof(CorsEndpointBehavior)));
                }
                else
                {
                    hdsiRestConfiguration.Endpoints.ForEach(ep => ep.Behaviors.RemoveAll(o => o.Type == typeof(CorsEndpointBehavior)));
                    hdsiRestConfiguration.Endpoints.ForEach(ep => ep.Behaviors.Add(new RestEndpointBehaviorConfiguration()
                    {
                        Type = typeof(CorsEndpointBehavior)
                    }));
                }
            }

            if (settings.TryGetValue(MaxResultsSetting, out var maxResults))
            {
                var biConfig = configuration.GetSection<BiConfigurationSection>();
                if (biConfig == null)
                {
                    biConfig = new BiConfigurationSection();
                    configuration.AddSection(biConfig);
                }
                biConfig.MaxBiResultSetSize = Int32.Parse(maxResults);
            }

            var serviceTypes = new Type[]
            {
                typeof(BisMessageHandler),
                typeof(InMemoryPivotProvider),
                typeof(LocalBiRenderService),
                typeof(DefaultDatamartManager)
            };
            var serviceConfiguration = configuration.GetSection<ApplicationServiceContextConfigurationSection>().ServiceProviders;
            serviceConfiguration.AddRange(serviceTypes.Where(t => !serviceConfiguration.Any(c => c.Type == t)).Select(t => new TypeReferenceConfiguration(t)));

        }
    }
}
