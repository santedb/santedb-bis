/*
 * Copyright (C) 2021 - 2026, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
using SanteDB.Core.BusinessRules;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Model.Audit;
using SanteDB.Core.Security.Audit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SanteDB.BI.Datamart.DataFlow.Executors
{
    /// <summary>
    /// Data flow step executor for a writer
    /// </summary>
    internal class DataWriterExecutor : DataStreamExecutorBase<BiDataFlowDataWriterStep>
    {

        private Tracer m_tracer = Tracer.GetTracer(typeof(DataWriterExecutor));

        /// <inheritdoc/>
        protected override IEnumerable<dynamic> ProcessStream(BiDataFlowDataWriterStep flowStep, DataFlowScope scope, IEnumerable<dynamic> inputStream)
        {

            // Get the output connection
            var outputConnection = flowStep.OutputConnection.ResolveReferenceTo<BiDataFlowStep>(scope).Execute(scope).First() as IDataIntegrator;

            // Ensure the table is migrated
            if (outputConnection.NeedsMigration(flowStep.Target))
            {
                var audit = scope.GetVariable<IAuditBuilder>(BiConstants.AuditDataFlowParameterName);
                audit.WithAuditableObjects(new AuditableObject()
                {
                    CustomIdTypeCode = new AuditCode(flowStep.Target.GetType().GetSerializationName(), BiConstants.XmlNamespace),
                    IDTypeCode = AuditableObjectIdType.Custom,
                    LifecycleType = AuditableObjectLifecycle.Creation,
                    NameData = flowStep.Target.Name,
                    ObjectId = $"{scope.Context.Datamart.Id}/schema/{flowStep.Target.Name}",
                    Role = AuditableObjectRole.Table,
                    Type = AuditableObjectType.SystemObject
                });
                outputConnection.RecreateObject(flowStep.Target);
            }

            if (flowStep.TruncateTable)
            {
                outputConnection.TruncateObject(flowStep.Target);
            }

            var diagnosticLog = scope.Context.DiagnosticSession?.LogStartAction(flowStep);
            try
            {
                var sw = new Stopwatch();
                sw.Start();
                var nRecs = 0;
                foreach (IDictionary<String, Object> itm in inputStream)
                {
                    var record = itm;

                    if (!this.ValidateRecord(record, flowStep.Target, out DetectedIssue issue))
                    {
                        this.m_tracer.TraceWarning("Data Flow Data {0} failed pre-validation", record);
                        record.Add("$reject", true);
                        record.Add("$reject.reason", issue.Text);
                        yield return record;
                        continue;
                    }

                    try
                    {
                        switch (flowStep.Mode)
                        {
                            case DataWriterModeType.Insert:
                                record = outputConnection.Insert(flowStep.Target, record);
                                break;
                            case DataWriterModeType.InsertOrUpdate:
                                record = outputConnection.InsertOrUpdate(flowStep.Target, record);
                                break;
                            case DataWriterModeType.Update:
                                record = outputConnection.Update(flowStep.Target, record);
                                break;
                            case DataWriterModeType.Delete:
                                record = outputConnection.Delete(flowStep.Target, record);
                                break;
                        }

                    }
                    catch (Exception e)
                    {
                        record.Add("$reject", true);
                        record.Add("$reject.reason", e.ToHumanReadableString());
                    }

                    diagnosticLog?.LogSample(DataFlowDiagnosticSampleType.TotalRecordProcessed, ++nRecs);
                    diagnosticLog?.LogSample(DataFlowDiagnosticSampleType.RecordThroughput, (nRecs / (float)sw.ElapsedMilliseconds) * 100.0f);
                    diagnosticLog?.LogSample(DataFlowDiagnosticSampleType.CurrentRecord, record);

                    yield return record;
                }
                sw.Stop();
            }
            finally
            {
                scope.Context.DiagnosticSession?.LogEndAction(diagnosticLog);
            }
        }

        /// <summary>
        /// Validate the record
        /// </summary>
        private bool ValidateRecord(IDictionary<String, Object> record, BiSchemaTableDefinition target, out DetectedIssue issue)
        {
            foreach (var col in target.Columns)
            {
                if ((!record.TryGetValue(col.Name.ToLowerInvariant(), out var value) || value == null) && (col.NotNull || col.IsKey))
                {
                    issue = new DetectedIssue(DetectedIssuePriorityType.Error, "required", $"{col.Name} failed NOT NULL constraint", Guid.Empty);
                    return false;
                }
                else if (!col.ValidateValue(value))
                {
                    issue = new DetectedIssue(DetectedIssuePriorityType.Error, "wrongtype", $"{col.Name} failed TYPE constraint", Guid.Empty);
                    return false;
                }
            }
            issue = null;
            return true;
        }
    }
}
