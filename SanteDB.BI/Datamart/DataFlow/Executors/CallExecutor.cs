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
using SanteDB.BI.Exceptions;
using SanteDB.BI.Model;
using SanteDB.Core.i18n;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SanteDB.BI.Datamart.DataFlow.Executors
{
    /// <summary>
    /// Executor for call step
    /// </summary>
    internal class CallExecutor : IDataFlowStepExecutor
    {
        /// <inheritdoc/>
        public Type Handles => typeof(BiDataFlowCallStep);

        /// <inheritdoc/>
        public IEnumerable<dynamic> Execute(BiDataFlowStep flowStep, DataFlowScope scope)
        {
            if (!(flowStep is BiDataFlowCallStep dcs))
            {
                throw new ArgumentOutOfRangeException(nameof(flowStep), String.Format(ErrorMessages.ARGUMENT_INCOMPATIBLE_TYPE, this.Handles.FullName, flowStep.GetType().FullName));
            }

            var myAction = scope.Context.DiagnosticSession?.LogStartAction(flowStep);
            try
            {

                // Validate target
                BiDataFlowDefinition targetFlow = dcs.TargetMethod.Resolved as BiDataFlowDefinition;
                try
                {
                    if (targetFlow == null)
                    {
                        throw new InvalidOperationException(String.Format(ErrorMessages.ARGUMENT_INCOMPATIBLE_TYPE, typeof(BiDataFlowDefinition), dcs.TargetMethod.Resolved.GetType()));
                    }
                    var missingArguments = targetFlow.Parameters.Where(p => !dcs.Arguments.Any(a => a.Name == p.Name));
                    if (missingArguments.Any())
                    {
                        throw new ArgumentException(String.Format(ErrorMessages.MISSING_VALUE, String.Join(",", missingArguments.Select(a => a.Name))));
                    }
                }
                catch (Exception e)
                {
                    throw new DataFlowException(flowStep, e);
                }

                // Create a sub-scope for the call
                var newScope = new DataFlowScope($"{scope.Name}_call", scope.Root);

                dcs.Arguments.ForEach(a =>
                {
                    var argValue = a.SimpleValue;
                    if (a.SimpleValue is BiObjectReference bir)
                    {
                        // If there's already an open or actioned object in scope use it!
                        if (scope.TryGetSysVar(bir.Ref, out object val))
                        {
                            newScope.SetSysVar(bir.Ref, val);
                        }
                        argValue = bir.Resolved;
                    }

                    newScope.DeclareConstant(a.Name, argValue);
                });

                // Invoke the method - as streaming?
                var sw = new Stopwatch();
                sw.Start();
                var nRecs = 0;
                foreach (var itm in targetFlow.Execute(newScope))
                {
                    myAction?.LogSample(DataFlowDiagnosticSampleType.TotalRecordProcessed, ++nRecs);
                    myAction?.LogSample(DataFlowDiagnosticSampleType.RecordThroughput, (nRecs / (float)sw.ElapsedMilliseconds) * 100.0f);
                    myAction?.LogSample(DataFlowDiagnosticSampleType.CurrentRecord, itm);
                    yield return itm;
                }

            }
            finally
            {
                scope.Context.DiagnosticSession?.LogEndAction(myAction);
            }
        }
    }
}
