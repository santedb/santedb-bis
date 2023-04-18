using DocumentFormat.OpenXml.Wordprocessing;
using SanteDB.BI.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SanteDB.BI.Datamart.DataFlow.Executors
{
    /// <summary>
    /// Execute a filter step
    /// </summary>
    internal class FilterExecutor : DataStreamExecutorBase<BiDataFlowFilterStep>
    {

        /// <inheritdoc/>
        protected override IEnumerable<dynamic> ProcessStream(BiDataFlowFilterStep flowStep, DataFlowScope scope, IEnumerable<dynamic> inputStream, IDataFlowDiagnosticAction diagnosticLog)
        {
            var filterFnVarName = $"filterFn.{flowStep.Name}";
            if(!scope.TryGetSysVar(filterFnVarName, out Func<DataFlowStreamTuple, bool> filterFn))
            {
                filterFn = this.CreateFilterFunction(flowStep);
                scope.SetSysVar(filterFnVarName, filterFn);

            }
            var sw = new Stopwatch();
            sw.Start();
            int nRecs = 0;
            foreach (var itm in inputStream.Select(o=>CreateStreamTuple(o)))
            {
                if (filterFn(itm))
                {
                    diagnosticLog?.LogSample(DataFlowDiagnosticSampleType.TotalRecordProcessed | DataFlowDiagnosticSampleType.PointInTime, ++nRecs);
                    diagnosticLog?.LogSample(DataFlowDiagnosticSampleType.RecordThroughput | DataFlowDiagnosticSampleType.PointInTime, (nRecs / (float)sw.ElapsedMilliseconds) * 100.0f);
                    diagnosticLog?.LogSample(DataFlowDiagnosticSampleType.CurrentRecord, itm);
                    yield return itm;
                }

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
                if(bodyExpression != null)
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
