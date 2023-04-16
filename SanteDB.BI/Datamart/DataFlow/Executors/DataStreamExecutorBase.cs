using SanteDB.BI.Exceptions;
using SanteDB.BI.Model;
using SanteDB.Core.i18n;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SanteDB.BI.Datamart.DataFlow.Executors
{
    /// <summary>
    /// Generic data stream executor
    /// </summary>
    internal abstract class DataStreamExecutorBase<TStreamStep> : IDataFlowStepExecutor
        where TStreamStep : BiDataFlowStreamStep
    {

        /// <inheritdoc/>
        public Type Handles => typeof(TStreamStep);

        /// <inheritdoc/>
        public IEnumerable<dynamic> Execute(BiDataFlowStep flowStep, DataFlowScope scope)
        {
            if (!(flowStep is TStreamStep bss) || !this.Handles.IsAssignableFrom(flowStep.GetType()))
            {
                throw new ArgumentOutOfRangeException(nameof(flowStep), String.Format(ErrorMessages.ARGUMENT_INCOMPATIBLE_TYPE, this.Handles.FullName, flowStep.GetType().FullName));
            }

            var myAction = scope.Context.DiagnosticSession?.LogStartAction(flowStep);
            try
            {
                scope.Context.Log(System.Diagnostics.Tracing.EventLevel.Verbose, flowStep.FormatExecutionPlan());
                var inputStream = bss.InputObject.ResolveReferenceTo<BiDataFlowStep>(scope).Execute(scope);
                return this.ProcessStream(bss, scope, inputStream, myAction);
            }
            catch(Exception e)
            {
                throw new DataFlowException(flowStep, e);
            }
            finally
            {
                scope.Context.DiagnosticSession?.LogEndAction(myAction);
            }
        }

        /// <summary>
        /// Perform the <paramref name="flowStep"/> for the <paramref name="inputStream"/>
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<dynamic> ProcessStream(TStreamStep flowStep, DataFlowScope scope, IEnumerable<dynamic> inputStream, IDataFlowDiagnosticAction diagnosticLog);
    }
}