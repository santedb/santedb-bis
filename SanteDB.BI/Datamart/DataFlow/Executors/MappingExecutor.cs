using Newtonsoft.Json.Serialization;
using SanteDB.BI.Model;
using SanteDB.Core.i18n;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SanteDB.BI.Datamart.DataFlow.Executors
{
    /// <summary>
    /// Executor for mapping
    /// </summary>
    internal class MappingExecutor : DataStreamExecutorBase<BiDataFlowMappingStep>
    {

        /// <inheritdoc />
        protected override IEnumerable<dynamic> ProcessStream(BiDataFlowMappingStep flowStep, DataFlowScope scope, IEnumerable<dynamic> inputStream)
        {

            // Get or open all reference columns for lookup
            var mapFnVarName = $"mapfn.{flowStep.Name}";
            if (!scope.TryGetSysVar(mapFnVarName, out Func<DataFlowStreamTuple, DataFlowStreamTuple> mappingFunc))
            {
                mappingFunc = this.BuildMappingFunc(flowStep);
                scope.SetSysVar(mapFnVarName, mappingFunc);
            }

            // Process results
            var diagnosticLog = scope.Context.DiagnosticSession?.LogStartAction(flowStep);
            try
            {
                var sw = new Stopwatch();
                sw.Start();
                int nRecs = 0;
                foreach (var itm in inputStream)
                {
                    var record = mappingFunc(CreateStreamTuple(itm));
                    diagnosticLog?.LogSample(DataFlowDiagnosticSampleType.TotalRecordProcessed | DataFlowDiagnosticSampleType.PointInTime, ++nRecs);
                    diagnosticLog?.LogSample(DataFlowDiagnosticSampleType.RecordThroughput | DataFlowDiagnosticSampleType.PointInTime, (nRecs / (float)sw.ElapsedMilliseconds) * 100.0f);
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
        /// Builds a mapping function 
        /// </summary>
        private Func<DataFlowStreamTuple, DataFlowStreamTuple> BuildMappingFunc(BiDataFlowMappingStep flowStep)
        {
            var getDataMethod = typeof(DataFlowStreamTuple).GetMethod(nameof(DataFlowStreamTuple.GetData));
            var setDataMethod = typeof(DataFlowStreamTuple).GetMethod(nameof(DataFlowStreamTuple.SetData));
            

            var inputParm = Expression.Parameter(typeof(DataFlowStreamTuple), "input");
            var resultVar = Expression.Variable(typeof(DataFlowStreamTuple), "result");
            var initializeResult = Expression.Assign(resultVar, Expression.New(typeof(DataFlowStreamTuple)));
            var labelTarget = Expression.Label(typeof(DataFlowStreamTuple));
            var returnResult = Expression.Return(labelTarget, resultVar);

            // Loop and create the mapping code
            var body = Expression.Block(
                new [] { resultVar },
                new Expression[] { initializeResult }.Union(
                    flowStep.Mapping.Select(column =>
                    {
                
                        // TODO: Implement lookup and simple expressions
                        if (column.Source.TransformExpression is string str)
                        {
                            return Expression.Call(resultVar, setDataMethod, Expression.Constant(column.Target.Name), Expression.Constant(str));
                        }
                        else if(column.Source.TransformExpression is BiColumnMappingTransformJoin tj)
                        {
                            throw new NotSupportedException(ErrorMessages.NOT_SUPPORTED); // not supported yet
                        }
                        else
                        {
                            var readSourceDataExpression = Expression.Call(inputParm, getDataMethod, Expression.Constant(column.Source.Name));
                            return Expression.Call(resultVar, setDataMethod, Expression.Constant(column.Target.Name), readSourceDataExpression);
                        }
                    })).Union(
                        new Expression[] { returnResult, Expression.Label(labelTarget, resultVar) }
                        ));


            return Expression.Lambda<Func<DataFlowStreamTuple, DataFlowStreamTuple>>(body, inputParm).Compile();
        }
    }
}
