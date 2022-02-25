/*
 * Copyright (C) 2021 - 2022, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2022-1-6
 */
using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.BI.Util;
using SanteDB.Core;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Jobs;
using SanteDB.Core.Security;
using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SanteDB.BI.Jobs
{
    /// <summary>
    /// Materialized view job
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] // Model classes - ignored
    public class BiMaterializeJob  : IJob
    {

        // Tracer 
        private Tracer m_tracer = Tracer.GetTracer(typeof(BiMaterializeJob));

        // Cancel request
        private volatile bool m_cancel = false;

        // State manager
        private readonly IJobStateManagerService m_stateManager;

        /// <summary>
        /// DI constructor for state manager
        /// </summary>
        public BiMaterializeJob(IJobStateManagerService stateManager)
        {
            this.m_stateManager = stateManager;
        }

        /// <summary>
        /// Gets the ID of the specified job
        /// </summary>
        public Guid Id => Guid.Parse("B5D6A459-C0FC-4D2F-A653-733C849BEAB9");

        /// <summary>
        /// Gets the name of the job
        /// </summary>
        public string Name => "Refresh BI Materialized Views";

        /// <inheritdoc/>
        public string Description => "Refreshes the materialized views in data sources for BI reports (run if you suspect your reports are out of date)";

        /// <summary>
        /// Can cancel
        /// </summary>
        public bool CanCancel => true;

        /// <summary>
        /// Gets the parameters for this job
        /// </summary>
        public IDictionary<string, Type> Parameters => new Dictionary<String, Type>()
        {
            { "viewName", typeof(String) }
        };

        /// <summary>
        /// Cancel this job
        /// </summary>
        public void Cancel()
        {
            this.m_stateManager.SetProgress(this, "Cancel Requested", 0.0f);
            this.m_cancel = true;
        }

        /// <summary>
        /// Run the specified job
        /// </summary>
        public void Run(object sender, EventArgs e, object[] parameters)
        {
            var biProvider = ApplicationServiceContext.Current.GetService<IBiDataSource>(); // Global default
            var biRepository = ApplicationServiceContext.Current.GetService<IBiMetadataRepository>(); // Global default
            var serviceManager = ApplicationServiceContext.Current.GetService<IServiceManager>(); // 

            try
            {

                this.m_tracer.TraceInfo("Starting refresh of defined BI materialized views");

                this.m_cancel = false;
                this.m_stateManager.SetState(this, JobStateType.Running);
                // TODO: Refactor on new enhanced persistence layer definition
                using (AuthenticationContext.EnterSystemContext())
                {
                    var definitions = biRepository.Query<BiQueryDefinition>(o => o.MetaData.Status != BiDefinitionStatus.Deprecated && o.MetaData.Status != BiDefinitionStatus.Obsolete);
                    int i = 0, count = definitions.Count();
                    foreach (var itm in definitions)
                    {

                        if (parameters.Length > 0 && !String.IsNullOrEmpty(parameters[0]?.ToString()) && parameters[0]?.Equals(itm.Id) != true)
                        {
                            continue;
                        }


                        this.m_stateManager.SetProgress(this, $"Refreshing {itm.Name ?? itm.Id}", ((float)i++ / (float)definitions.Length));

                        var dataSource = biProvider;
                        var queryDefinition = BiUtils.ResolveRefs(itm) as BiQueryDefinition;
                        var providerType = queryDefinition.DataSources.FirstOrDefault()?.ProviderType;
                        if (providerType != null)
                        {
                            dataSource = serviceManager.CreateInjected(providerType) as IBiDataSource;
                        }
                        dataSource.RefreshMaterializedView(itm);

                        if (this.m_cancel)
                        {
                            this.m_stateManager.SetState(this, JobStateType.Cancelled);
                            return;
                        }
                    }
                }

                this.m_stateManager.SetState(this, JobStateType.Completed);
            }
            catch (Exception ex)
            {
                this.m_stateManager.SetState(this, JobStateType.Aborted);
                this.m_stateManager.SetProgress(this, ex.Message, 0.0f);
                this.m_cancel = false;
                this.m_tracer.TraceError("Error processing BI materialized views: {0}", ex.Message);
                throw new Exception("Error running BI refresh job", ex);
            }
            finally
            {
                this.m_cancel = false;
            }
        }
    }
}
