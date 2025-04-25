/*
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
using SanteDB.BI.Datamart;
using SanteDB.BI.Datamart.DataFlow;
using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Jobs;
using SanteDB.Core.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SanteDB.BI.Jobs
{
    /// <summary>
    /// A JOB which refreshes BI datamarts
    /// </summary>
    public class BiDatamartJob : IJob
    {


        private readonly Tracer m_tracer = Tracer.GetTracer(typeof(BiDatamartJob));
        /// <summary>
        /// Unique identifier used to identify the job type for a <see cref="BiDatamartJob"/>.
        /// </summary>
        public static readonly Guid JOBID = Guid.Parse("751B0333-952B-4250-B117-D2E6A70C4ECD");
        private readonly IBiMetadataRepository m_biMetaRepository;
        private readonly IBiDatamartRepository m_biRepository;
        private readonly IBiDatamartManager m_biManager;
        private readonly IJobStateManagerService m_stateManager;

        /// <summary>
        /// DI ctor
        /// </summary>
        public BiDatamartJob(IBiMetadataRepository biMetaRepository, IBiDatamartManager biManager, IBiDatamartRepository biRepository, IJobStateManagerService stateManagerService)
        {
            this.m_biMetaRepository = biMetaRepository;
            this.m_biRepository = biRepository;
            this.m_biManager = biManager;
            this.m_stateManager = stateManagerService;
            this.m_biManager.DiagnosticEventReceived += (o, e) =>
            {

                switch (e)
                {
                    case DiagnosticSampleEventArgs sample:

                        if (sample.SampleType == DataFlowDiagnosticSampleType.RecordThroughput && o is IDataFlowDiagnosticAction act)
                        {
                            var currentState = this.m_stateManager.GetJobState(this);
                            this.m_stateManager.SetProgress(this, $"{act.Name} ({sample.SampleValue} r/s)", currentState.Progress);
                        }
                        break;
                    case DiagnosticActionEventArgs action:
                        var state = this.m_stateManager.GetJobState(this);
                        this.m_stateManager.SetProgress(this, $"{action.Action.Name}", state.Progress);
                        break;
                }
            };
        }

        /// <inheritdoc/>
        public Guid Id => JOBID;

        /// <inheritdoc/>
        public string Name => "Refresh Datamarts";

        /// <inheritdoc/>
        public string Description => "Refreshes the BI data marts in SanteDB";

        /// <inheritdoc/>
        public bool CanCancel => false;

        /// <inheritdoc/>
        public IDictionary<string, Type> Parameters => new Dictionary<String, Type>
        {
            { "diagnostic", typeof(bool) },
            { "mart", typeof(string) }
        };

        /// <inheritdoc/>
        public void Cancel()
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc/>
        public void Run(object sender, EventArgs e, object[] parameters)
        {
            try
            {
                this.m_stateManager.SetState(this, JobStateType.Running);

                if (parameters.Length == 0)
                {
                    parameters = new object[] { false, null };
                }

                if (!(parameters[0] is bool diagnostic))
                {
                    diagnostic = false;
                }

                using (AuthenticationContext.EnterSystemContext())
                {
                    // All active and registered repository metadata stuff
                    IDatamart[] registeredMarts = null;
                    if (parameters.Length == 2 && parameters[1] is String martName)
                    {
                        registeredMarts = this.m_biRepository.Find(o => o.Id == martName).ToArray();
                    }
                    else
                    {
                        registeredMarts = this.m_biRepository.Find(o => true).ToArray();
                    }

                    var martCount = 0;
                    foreach (var itm in registeredMarts)
                    {
                        var dataMartDefinition = this.m_biMetaRepository.Get<BiDatamartDefinition>(itm.Id);
                        this.m_stateManager.SetProgress(this, $"Refresh {itm.Name}", (float)martCount / registeredMarts.Length);
                        this.m_biManager.Refresh(dataMartDefinition, diagnostic);
                    }

                    this.m_stateManager.SetState(this, JobStateType.Completed);
                }
            }
            catch (Exception ex)
            {
                this.m_tracer.TraceError("Error refreshing datamarts: {0}", ex);
                this.m_stateManager.SetProgress(this, ex.ToHumanReadableString(), 0.0f);
                this.m_stateManager.SetState(this, JobStateType.Aborted, ex.ToHumanReadableString());
            }
        }
    }
}
