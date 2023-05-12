using SanteDB.BI.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace SanteDB.BI.Datamart.DataFlow.Executors
{
    /// <summary>
    /// Execute a filter step
    /// </summary>
    internal class FilterExecutor : DataStreamExecutorBase<BiDataFlowFilterStep>
    {

        /// <inheritdoc/>
        protected override IEnumerable<dynamic> ProcessStream(BiDataFlowFilterStep flowStep, DataFlowScope scope, IEnumerable<dynamic> inputStream)
        {
            var filterFnVarName = $"filterFn.{flowStep.Name}";
            if (!scope.TryGetSysVar(filterFnVarName, out Func<DataFlowStreamTuple, bool> filterFn))
            {
                filterFn = this.CreateFilterFunction(flowStep);
                scope.SetSysVar(filterFnVarName, filterFn);

            }
            var sw = new Stopwatch();
            sw.Start();
            int nRecs = 0;
            var diagnosticLog = scope.Context.DiagnosticSession?.LogStartAction(flowStep);
            try
            {
                foreach (var itm in inputStream.Select(o => CreateStreamTuple(o)))
                {
                    if (filterFn(itm))
                    {
                        diagnosticLog?.LogSample(DataFlowDiagnosticSampleType.TotalRecordProcessed, ++nRecs);
                        diagnosticLog?.LogSample(DataFlowDiagnosticSampleType.RecordThroughput, (nRecs / (float)sw.ElapsedMilliseconds) * 100.0f);
                        diagnosticLog?.LogSample(DataFlowDiagnosticSampleType.CurrentRecord, itm);
                        yield return itm;
                    }

                }
            }
            finally
            {
                scope.Context.DiagnosticSession?.LogEndAction(diagnosticLog);
            }
            sw.Stop();
        }

        /// <summary>
        /// Create a filter function for this object
        /// </summary>
        private Func<DataFlowStreamTuple, bool> CreateFilterFunction(BiDataFlowFilterStep flowStep)
        {

            var getDataMethod = typeof(DataFlowStreamTuple).GetMethod(nameof(DataFlowStreamTuple.GetData));
            var setDataMethod = typeof(DataFlowStreamTuple).GetMethod(nameof(DataFlowStreamTuple.SetData));

            var inputParm = Expression.Parameter(typeof(DataFlowStreamTuple));
            Expression bodyExpression = null;

            foreach (var itm in flowStep.When)
            {
                var sourceValue = Expression.Call(inputParm, getDataMethod, Expression.Constant(itm.Field));
                if (!Enum.TryParse<ExpressionType>(itm.Operator.ToString(), out var comparisonType))
                {
                    throw new ArgumentOutOfRangeException(nameof(BiDataFlowStreamFilterSetting.Operator));
                }

                var dType = itm.Value != null ? typeof(Nullable<>).MakeGenericType(itm.Value.GetType()) : typeof(Object);
                var whenExpr = Expression.MakeBinary(comparisonType, Expression.Convert(sourceValue, dType), Expression.Convert(Expression.Constant(itm.Value), dType));
                if (bodyExpression != null)
                {
                    bodyExpression = Expression.MakeBinary(ExpressionType.AndAlso, bodyExpression, whenExpr);
                }
                else
                {
                    bodyExpression = whenExpr;
                }
            }

            return Expression.Lambda<Func<DataFlowStreamTuple, bool>>(
                bodyExpression,
                inputParm

            ).Compile();
        }
    }
}
