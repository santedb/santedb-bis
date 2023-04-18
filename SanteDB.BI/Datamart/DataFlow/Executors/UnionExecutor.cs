using SanteDB.BI.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SanteDB.BI.Datamart.DataFlow.Executors
{
    /// <summary>
    /// Union with executor
    /// </summary>
    internal class UnionExecutor : DataStreamExecutorBase<BiDataFlowUnionStreamStep>
    {
        /// <inheritdoc/>
        protected override IEnumerable<dynamic> ProcessStream(BiDataFlowUnionStreamStep flowStep, DataFlowScope scope, IEnumerable<dynamic> inputStream, IDataFlowDiagnosticAction diagnosticLog)
        {
            var masterList = flowStep.UnionWith?.SelectMany(o => o.ResolveReferenceTo<BiDataFlowStep>(scope).Execute(scope)) ?? new dynamic[0];
            var sw = new Stopwatch();
            sw.Start();
            int nRecs = 0;
            foreach (var itm in inputStream.Union(masterList))
            {
                diagnosticLog?.LogSample(DataFlowDiagnosticSampleType.TotalRecordProcessed | DataFlowDiagnosticSampleType.PointInTime, ++nRecs);
                diagnosticLog?.LogSample(DataFlowDiagnosticSampleType.RecordThroughput | DataFlowDiagnosticSampleType.PointInTime, (nRecs / (float)sw.ElapsedMilliseconds) * 100.0f);
                diagnosticLog?.LogSample(DataFlowDiagnosticSampleType.CurrentRecord, itm);
                yield return itm;
            }
            sw.Stop();
        }
    }
}
