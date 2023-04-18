using DocumentFormat.OpenXml.Presentation;
using SanteDB.BI.Exceptions;
using SanteDB.BI.Model;
using SanteDB.Core.i18n;
using SanteDB.Core.Model.Audit;
using SanteDB.Core.Security.Audit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

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
                    foreach(var itm in retStep.Execute(scope))
                    {
                        myAction?.LogSample(DataFlowDiagnosticSampleType.TotalRecordProcessed | DataFlowDiagnosticSampleType.PointInTime, ++nRecs);
                        myAction?.LogSample(DataFlowDiagnosticSampleType.RecordThroughput | DataFlowDiagnosticSampleType.PointInTime, (nRecs / (float)sw.ElapsedMilliseconds) * 100.0f);
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
                        myAction?.LogSample(DataFlowDiagnosticSampleType.TotalRecordProcessed | DataFlowDiagnosticSampleType.PointInTime, processedRecords);
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
