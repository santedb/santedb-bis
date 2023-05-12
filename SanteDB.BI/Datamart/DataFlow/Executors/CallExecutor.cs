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
