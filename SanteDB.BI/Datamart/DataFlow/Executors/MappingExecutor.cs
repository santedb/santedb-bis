/*
 * Copyright (C) 2021 - 2023, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-5-19
 */
using SanteDB.BI.Model;
using SanteDB.Core.i18n;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

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
                new[] { resultVar },
                new Expression[] { initializeResult }.Union(
                    flowStep.Mapping.Select(column =>
                    {

                        // TODO: Implement lookup and simple expressions
                        if (column.Source.TransformExpression is string str)
                        {
                            return Expression.Call(resultVar, setDataMethod, Expression.Constant(column.Target.Name), Expression.Constant(str));
                        }
                        else if (column.Source.TransformExpression is BiColumnMappingTransformJoin tj)
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
