using SanteDB.BI.Exceptions;
using SanteDB.BI.Model;
using SanteDB.Core.i18n;
using System;
using System.Collections.Generic;

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

            try
            {
                scope.Context.Log(System.Diagnostics.Tracing.EventLevel.Verbose, flowStep.FormatExecutionPlan());
                var inputStream = bss.InputObject.ResolveReferenceTo<BiDataFlowStep>(scope).Execute(scope);
                return this.ProcessStream(bss, scope, inputStream);
            }
            catch (Exception e)
            {
                throw new DataFlowException(flowStep, e);
            }
        }

        /// <summary>
        /// Create a stream tuple
        /// </summary>
        protected DataFlowStreamTuple CreateStreamTuple(object tuple)
        {
            switch (tuple)
            {
                case DataFlowStreamTuple tup:
                    return tup;
                case IDictionary<String, Object> dict:
                    return new DataFlowStreamTuple(dict);
                default:
                    throw new ArgumentOutOfRangeException(nameof(tuple));
            }

        }

        /// <summary>
        /// Perform the <paramref name="flowStep"/> for the <paramref name="inputStream"/>
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<dynamic> ProcessStream(TStreamStep flowStep, DataFlowScope scope, IEnumerable<dynamic> inputStream);
    }
}