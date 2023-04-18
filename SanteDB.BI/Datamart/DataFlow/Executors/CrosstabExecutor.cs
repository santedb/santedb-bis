using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.BI.Services.Impl;
using SanteDB.Core.i18n;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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
        protected override IEnumerable<dynamic> ProcessStream(BiDataFlowCrosstabStep flowStep, DataFlowScope scope, IEnumerable<dynamic> inputStream, IDataFlowDiagnosticAction diagnosticLog)
        {

            if(flowStep.Pivot == null)
            {
                throw new InvalidOperationException(String.Format(ErrorMessages.MISSING_VALUE, nameof(BiDataFlowCrosstabStep.Pivot)));
            }

            var sw = new Stopwatch();
            sw.Start();
            var nRecs = 0;
            foreach(var itm in this.m_pivotProvider.Pivot(inputStream, flowStep.Pivot))
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
