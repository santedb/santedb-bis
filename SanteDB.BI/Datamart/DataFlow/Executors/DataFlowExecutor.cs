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
using SanteDB.Core.Model.Audit;
using SanteDB.Core.Security.Audit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SanteDB.BI.Datamart.DataFlow.Executors
{
    /// <summary>
    /// Represents an executor which executes a data-flow
    /// </summary>
    internal class DataFlowExecutor : IDataFlowStepExecutor
    {
        /// <inheritdoc/>
        public Type Handles => typeof(BiDataFlowDefinition);

        /// <inheritdoc/>
        public IEnumerable<dynamic> Execute(BiDataFlowStep flowStep, DataFlowScope scope)
        {
            if (!(flowStep is BiDataFlowDefinition bfd))
            {
                throw new ArgumentOutOfRangeException(nameof(flowStep), String.Format(ErrorMessages.ARGUMENT_INCOMPATIBLE_TYPE, this.Handles.FullName, flowStep.GetType().FullName));
            }

            var myAction = scope.Context.DiagnosticSession?.LogStartAction(flowStep);
            try
            {

                // Audit this is being executed
                var audit = scope.GetVariable<IAuditBuilder>(BiConstants.AuditDataFlowParameterName);
                audit.WithAuditableObjects(new AuditableObject()
                {
                    IDTypeCode = AuditableObjectIdType.Custom,
                    CustomIdTypeCode = new AuditCode(flowStep.GetType().Name, "http://santedb.org/bi"),
                    LifecycleType = AuditableObjectLifecycle.Access,
                    NameData = flowStep.Name,
                    ObjectId = $"{scope.Context.Datamart.Id}/flow/{flowStep.Name}",
                    Role = AuditableObjectRole.Job,
                    Type = AuditableObjectType.SystemObject
                });

                // Define this scope
                scope = new DataFlowScope(bfd.Name, scope);

                // Find all objects which have no input stream dependencies - these represent our "start" objects for the data
                // flow definition. Data flows don't start with the first object and work backwards, rather they rely on a streaming 
                // paradigm. 

                // If there is an explicit returning step - then that is our start object
                if (bfd.ReturnObject?.Resolved is BiDataFlowStep retStep)
                {
                    scope.Context.Log(System.Diagnostics.Tracing.EventLevel.Verbose, $"Execution Plan: {retStep.Name}");

                    var sw = new Stopwatch();
                    sw.Start();
                    var nRecs = 0;
                    foreach (var itm in retStep.Execute(scope))
                    {
                        myAction?.LogSample(DataFlowDiagnosticSampleType.TotalRecordProcessed, ++nRecs);
                        myAction?.LogSample(DataFlowDiagnosticSampleType.RecordThroughput, (nRecs / (float)sw.ElapsedMilliseconds) * 100.0f);
                        myAction?.LogSample(DataFlowDiagnosticSampleType.CurrentRecord, itm);
                        yield return itm;
                    }
                    sw.Stop();
                }
                else
                {
                    try
                    {
                        var executeRoot = bfd.GetExecutionTreeRoot();
                        scope.Context.Log(System.Diagnostics.Tracing.EventLevel.Verbose, bfd.FormatExecutionPlan());
                        // Now we process the terminal objects and execute them
                        var processedRecords = executeRoot.SelectMany(o => o.Execute(scope)).Count();
                        myAction?.LogSample(DataFlowDiagnosticSampleType.TotalRecordProcessed, processedRecords);
                    }
                    catch (Exception e)
                    {
                        throw new DataFlowException(flowStep, e);
                    }

                    yield break;
                }
            }
            finally
            {
                scope.Context.DiagnosticSession?.LogEndAction(myAction);
            }
        }
    }
}
