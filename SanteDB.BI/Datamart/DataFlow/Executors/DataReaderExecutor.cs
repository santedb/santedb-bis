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
using SanteDB.BI.Exceptions;
using SanteDB.BI.Model;
using SanteDB.Core.i18n;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SanteDB.BI.Datamart.DataFlow.Executors
{
    /// <summary>
    /// Data reader executor
    /// </summary>
    internal class DataReaderExecutor : IDataFlowStepExecutor
    {
        /// <inheritdoc/>
        public Type Handles => typeof(BiDataFlowDataReaderStep);

        /// <inheritdoc/>
        public IEnumerable<dynamic> Execute(BiDataFlowStep flowStep, DataFlowScope scope)
        {
            if (!(flowStep is BiDataFlowDataReaderStep brs))
            {
                throw new ArgumentOutOfRangeException(nameof(flowStep), String.Format(ErrorMessages.ARGUMENT_INCOMPATIBLE_TYPE, this.Handles.FullName, flowStep.GetType().FullName));
            }

            var myAction = scope.Context.DiagnosticSession?.LogStartAction(flowStep);
            try
            {

                var dataSource = brs.InputConnection.ResolveReferenceTo<BiDataFlowStep>(scope).Execute(scope).First() as IDataIntegrator;
                IEnumerable<dynamic> recordSource = null;
                try
                {
                    recordSource = dataSource.Query(brs.Definition, brs.Schema);
                }
                catch (Exception e)
                {
                    throw new DataFlowException(flowStep, e);
                }

                var sw = new Stopwatch();
                sw.Start();
                int nRecs = 0;
                foreach (var itm in recordSource)
                {
                    myAction?.LogSample(DataFlowDiagnosticSampleType.TotalRecordProcessed, ++nRecs);
                    myAction?.LogSample(DataFlowDiagnosticSampleType.RecordThroughput, (nRecs / (float)sw.ElapsedMilliseconds) * 100.0f);
                    myAction?.LogSample(DataFlowDiagnosticSampleType.CurrentRecord, itm);
                    yield return itm;
                }
                sw.Stop();

            }
            finally
            {
                scope.Context.DiagnosticSession?.LogEndAction(myAction);
            }
        }
    }
}
