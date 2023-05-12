using SanteDB.BI.Model;
using SanteDB.Core.Diagnostics;
using SanteDB.Core.i18n;
using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;

namespace SanteDB.BI.Datamart.DataFlow.Executors
{
    /// <summary>
    /// Data flow executor for logs
    /// </summary>
    internal class LogExecutor : IDataFlowStepExecutor
    {

        public LogExecutor(IThreadPoolService thp)
        {
            this.m_threadPool = thp;
        }
        // TRacer
        private readonly Tracer m_tracer = Tracer.GetTracer(typeof(LogExecutor));
        private readonly IThreadPoolService m_threadPool;

        /// <inheritdoc/>
        public Type Handles => typeof(BiDataFlowLogStep);

        /// <inheritdoc/>
        public IEnumerable<dynamic> Execute(BiDataFlowStep flowStep, DataFlowScope scope)
        {

            if (!(flowStep is BiDataFlowLogStep bls))
            {
                throw new ArgumentOutOfRangeException(nameof(flowStep), String.Format(ErrorMessages.ARGUMENT_INCOMPATIBLE_TYPE, typeof(BiDataFlowLogStep), flowStep.GetType().FullName));
            }

            var myAction = scope.Context.DiagnosticSession?.LogStartAction(flowStep);
            try
            {

                // Is this a streaming entry?
                if (bls.InputObject != null)
                {
                    var sw = new Stopwatch();
                    sw.Start();

                    var inputStream = bls.InputObject.ResolveReferenceTo<BiDataFlowStep>(scope).Execute(scope);


                    var nRecs = 0;
                    foreach (var itm in inputStream)
                    {
                        this.Emit(bls.Destination, scope, bls.Priority, bls.Format((object)itm), myAction);
                        myAction?.LogSample(DataFlowDiagnosticSampleType.CurrentRecord, itm);
                        myAction?.LogSample(DataFlowDiagnosticSampleType.TotalRecordProcessed, ++nRecs);
                        myAction?.LogSample(DataFlowDiagnosticSampleType.RecordThroughput, (nRecs / (float)sw.ElapsedMilliseconds) * 100.0f);
                        yield return itm;
                    }
                    sw.Stop();

                }
                else
                {
                    this.Emit(bls.Destination, scope, bls.Priority, bls.Format(scope), myAction);
                }
            }
            finally
            {
                scope.Context.DiagnosticSession?.LogEndAction(myAction);
            }
        }

        /// <summary>
        /// Emit to the intended source
        /// </summary>
        private void Emit(BiLogDestinationType logDestination, DataFlowScope scope, EventLevel priority, string message, IDataFlowDiagnosticAction diagnosticAction)
        {
            if (logDestination.HasFlag(BiLogDestinationType.ExecutionLog))
            {
                scope.Context.Log(priority, message);
            }
            if (logDestination.HasFlag(BiLogDestinationType.Console))
            {
                Console.WriteLine("DataFlow: {0} -> {1} {2}", scope.Context.Datamart.Id, priority, message);
            }
            if (logDestination.HasFlag(BiLogDestinationType.Trace))
            {
                this.m_tracer.TraceEvent(priority, message);
            }
            if (logDestination == BiLogDestinationType.Any)
            {
                Debug.WriteLine("DataFlow: {0} -> {1} {2}", scope.Context.Datamart.Id, priority, message);
                this.m_tracer.TraceEvent(priority, message);
            }

            diagnosticAction?.LogSample(DataFlowDiagnosticSampleType.LoggedData, message);
        }
    }
}
