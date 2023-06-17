﻿using SanteDB.BI.Exceptions;
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
