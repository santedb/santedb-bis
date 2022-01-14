/*
 * Copyright (C) 2021 - 2021, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2021-8-5
 */
using SanteDB.BI.Jobs;
using SanteDB.BI.Model;
using SanteDB.BI.Util;
using SanteDB.Core;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Jobs;
using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SanteDB.BI.Services.Impl
{
    /// <summary>
    /// Rendering service which renders reports locally
    /// </summary>
    public class LocalBiRenderService : IBiRenderService
    {

        // Service manager
        private readonly IServiceManager m_serviceManager;

        private readonly Tracer m_tracer = Tracer.GetTracer(typeof(LocalBiRenderService));

        /// <summary>
        /// DI constructor
        /// </summary>
        public LocalBiRenderService(IServiceManager serviceManager, IJobManagerService jobManager, IBiMetadataRepository metadataRepository, IBiDataSource defaultDataSource = null)
        {
            this.m_serviceManager = serviceManager;
            var job = new BiMaterializeJob();
            jobManager.AddJob(job, JobStartType.TimerOnly);  // Set default job
            if (jobManager.GetJobSchedules(job)?.Any() != true)
            {
                jobManager.SetJobSchedule(job, new DayOfWeek[]
                {
                DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday
                }, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day + 1, 0, 0, 0)); // First run for tomorrow
            }

            // Scan and initialize all BI materialized views
            ApplicationServiceContext.Current.Started += (o, e) =>
            {
                foreach (var itm in metadataRepository.Query<BiQueryDefinition>(x => x.MetaData.Status == BiDefinitionStatus.Active, 0, 100))
                {
                    try
                    {
                        IBiDataSource dataSource = null;
                        var queryDefinition = BiUtils.ResolveRefs(itm) as BiQueryDefinition;
                        var providerType = queryDefinition.DataSources.FirstOrDefault()?.ProviderType;
                        if (providerType != null)
                        {
                            dataSource = this.m_serviceManager.CreateInjected(providerType) as IBiDataSource;
                        }
                        else
                        {
                            dataSource = defaultDataSource;
                        }

                        this.m_tracer.TraceInfo("Materializing views for {0}", queryDefinition.Id);
                        dataSource.CreateMaterializedView(queryDefinition);
                    }
                    catch(Exception ex)
                    {
                        this.m_tracer.TraceWarning("Could not initialize materialized views for {0} - {1}", itm.Id, ex);
                    }
                }
            };
        }

        /// <summary>
        /// Service name
        /// </summary>
        public string ServiceName => "Local BI Rendering Service";

        /// <summary>
        /// Render the specified report
        /// </summary>
        public Stream Render(string reportId, string viewName, string formatName, IDictionary<string, object> parameters, out string mimeType)
        {
            try
            {

                // Get the report format 
                var formatDefinition = ApplicationServiceContext.Current.GetService<IBiMetadataRepository>().Query<BiRenderFormatDefinition>(o => o.FormatExtension == formatName, 0, 1).FirstOrDefault();
                if (formatDefinition == null)
                    throw new KeyNotFoundException($"Report format {formatName} is not registered");

                var reportDefinition = ApplicationServiceContext.Current.GetService<IBiMetadataRepository>().Get<BiReportDefinition>(reportId);
                if (reportDefinition == null)
                    throw new KeyNotFoundException($"Report {reportId} is not registered");

                // Render the report
                var renderer = this.m_serviceManager.CreateInjected(formatDefinition.Type) as IBiReportFormatProvider;

                mimeType = formatDefinition.ContentType;
                return renderer.Render(reportDefinition, viewName, parameters);
            }
            catch (Exception e)
            {
                throw new Exception($"Error rendering BIS report: {reportId}", e);
            }
        }
    }
}
