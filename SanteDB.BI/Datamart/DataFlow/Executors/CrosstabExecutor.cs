﻿/*
 * Copyright (C) 2021 - 2024, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.BI.Services.Impl;
using SanteDB.Core.i18n;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SanteDB.BI.Datamart.DataFlow.Executors
{
    /// <summary>
    /// Cross tab executor
    /// </summary>
    internal class CrosstabExecutor : DataStreamExecutorBase<BiDataFlowCrosstabStep>
    {
        private readonly IBiPivotProvider m_pivotProvider;

        /// <summary>
        /// DI constructor
        /// </summary>
        public CrosstabExecutor(IBiPivotProvider pivotProvider = null)
        {
            this.m_pivotProvider = pivotProvider ?? new InMemoryPivotProvider();
        }

        /// <inheritdoc/>
        protected override IEnumerable<dynamic> ProcessStream(BiDataFlowCrosstabStep flowStep, DataFlowScope scope, IEnumerable<dynamic> inputStream)
        {

            if (flowStep.Pivot == null)
            {
                throw new InvalidOperationException(String.Format(ErrorMessages.MISSING_VALUE, nameof(BiDataFlowCrosstabStep.Pivot)));
            }

            var sw = new Stopwatch();
            sw.Start();
            var nRecs = 0;
            var diagnosticLog = scope.Context.DiagnosticSession?.LogStartAction(flowStep);
            try
            {

                foreach (var itm in this.m_pivotProvider.Pivot(inputStream, flowStep.Pivot))
                {
                    diagnosticLog?.LogSample(DataFlowDiagnosticSampleType.TotalRecordProcessed, ++nRecs);
                    diagnosticLog?.LogSample(DataFlowDiagnosticSampleType.RecordThroughput, (nRecs / (float)sw.ElapsedMilliseconds) * 100.0f);
                    diagnosticLog?.LogSample(DataFlowDiagnosticSampleType.CurrentRecord, itm);
                    yield return itm;
                }
            }
            finally
            {
                scope.Context.DiagnosticSession?.LogEndAction(diagnosticLog);
            }
            sw.Stop();
        }
    }
}
