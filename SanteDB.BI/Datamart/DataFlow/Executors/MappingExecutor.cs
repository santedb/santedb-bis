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

        /// <summary>
        /// Represents a simple mapping tuple
        /// </summary>
        private class MappingDataTuple 
        {

            // Backing dictionary
            private readonly IDictionary<String, Object> m_dictionary;

            /// <summary>
            /// Default ctor (creates a new output tuple)
            /// </summary>
            public MappingDataTuple()
            {
                this.m_dictionary = new ExpandoObject();
            }

            /// <summary>
            /// Create an input tuple based on the provided data
            /// </summary>
            public MappingDataTuple(IDictionary<String, Object> inputData)
            {
                this.m_dictionary = inputData.ToDictionary(o=>o.Key.ToLowerInvariant(), o=>o.Value);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public Object this[String name]
            {
                get => this.GetData(name);
                set => this.SetData(name, value);
            }

            /// <summary>
            /// Set data in the tuple
            /// </summary>
            public void SetData(string name, object value)
            {
                if (this.m_dictionary.ContainsKey(name.ToLowerInvariant()))
                {
                    this.m_dictionary[name.ToLowerInvariant()] = value;
                }
                else
                {
                    this.m_dictionary.Add(name.ToLowerInvariant(), value);
                }
            }

            /// <summary>
            /// Get data from the tuple
            /// </summary>
            public object GetData(string name) => this.m_dictionary.TryGetValue(name.ToLowerInvariant(), out var result) ? result : null;

            /// <summary>
            /// Represent as a string
            /// </summary>
            public override string ToString() => String.Join(",", this.m_dictionary.Values);

            /// <summary>
            /// Convert to a dictionary
            /// </summary>
            public IDictionary<String, Object> ToDictionary() => this.m_dictionary.ToDictionary(o=>o.Key, o=>o.Value);
        }


        /// <inheritdoc />
        protected override IEnumerable<dynamic> ProcessStream(BiDataFlowMappingStep flowStep, DataFlowScope scope, IEnumerable<dynamic> inputStream, IDataFlowDiagnosticAction diagnosticLog)
        {

            // Get or open all reference columns for lookup
            var mapFnVarName = $"mapfn.{flowStep.Name}";
            if (!scope.TryGetSysVar(mapFnVarName, out Func<MappingDataTuple, MappingDataTuple> mappingFunc))
            {
                mappingFunc = this.BuildMappingFunc(flowStep);
                scope.SetSysVar(mapFnVarName, mappingFunc);
            }

            // Process results
            var sw = new Stopwatch();
            sw.Start();
            int nRecs = 0;
            foreach(var itm in inputStream.OfType<IDictionary<string, Object>>())
            {
                var record = mappingFunc(new MappingDataTuple(itm));
                diagnosticLog?.LogSample(DataFlowDiagnosticSampleType.TotalRecordProcessed | DataFlowDiagnosticSampleType.PointInTime, ++nRecs);
                diagnosticLog?.LogSample(DataFlowDiagnosticSampleType.RecordThroughput | DataFlowDiagnosticSampleType.PointInTime, (nRecs / (float)sw.ElapsedMilliseconds) * 100.0f);
                diagnosticLog?.LogSample(DataFlowDiagnosticSampleType.CurrentRecord, record);
                yield return record.ToDictionary();

            }
            sw.Stop();
        }

        /// <summary>
        /// Builds a mapping function 
        /// </summary>
        private Func<MappingDataTuple, MappingDataTuple> BuildMappingFunc(BiDataFlowMappingStep flowStep)
        {
            var getDataMethod = typeof(MappingDataTuple).GetMethod(nameof(MappingDataTuple.GetData));
            var setDataMethod = typeof(MappingDataTuple).GetMethod(nameof(MappingDataTuple.SetData));
            var nullExpression = Expression.Constant(null);

            var inputParm = Expression.Parameter(typeof(MappingDataTuple), "input");
            var resultVar = Expression.Variable(typeof(MappingDataTuple), "result");
            var initializeResult = Expression.Assign(resultVar, Expression.New(typeof(MappingDataTuple)));
            var labelTarget = Expression.Label(typeof(MappingDataTuple));
            var returnResult = Expression.Return(labelTarget, resultVar);

            // Loop and create the mapping code
            var body = Expression.Block(
                new [] { resultVar },
                new Expression[] { initializeResult }.Union(
                    flowStep.Mapping.Select(column =>
                    {
                        var readSourceDataExpression = Expression.Call(inputParm, getDataMethod, Expression.Constant(column.Source.Name));
                
                        // TODO: Implement lookup and simple expressions
                        if (column.Source.TransformExpression != null)
                        {
                            throw new NotSupportedException(ErrorMessages.NOT_SUPPORTED); // not supported yet
                        }
                        else
                        {
                            return Expression.Call(resultVar, setDataMethod, Expression.Constant(column.Target.Name), readSourceDataExpression);
                        }
                    })).Union(
                        new Expression[] { returnResult, Expression.Label(labelTarget, resultVar) }
                        ));


            return Expression.Lambda<Func<MappingDataTuple, MappingDataTuple>>(body, inputParm).Compile();
        }
    }
}
