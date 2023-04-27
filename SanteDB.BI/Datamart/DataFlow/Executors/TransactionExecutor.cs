using SanteDB.BI.Exceptions;
using SanteDB.BI.Model;
using SanteDB.Core.i18n;
using SharpCompress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SanteDB.BI.Datamart.DataFlow.Executors
{
    /// <summary>
    /// A transaction executor
    /// </summary>
    internal class TransactionExecutor : IDataFlowStepExecutor
    {
        /// <inheritdoc/>
        public Type Handles => typeof(BiDataFlowTransactionStep);

        /// <inheritdoc/>
        IEnumerable<dynamic> IDataFlowStepExecutor.Execute(BiDataFlowStep flowStep, DataFlowScope scope)
        {
            if (!(flowStep is BiDataFlowTransactionStep bts))
            {
                throw new ArgumentOutOfRangeException(nameof(flowStep), String.Format(ErrorMessages.ARGUMENT_INCOMPATIBLE_TYPE, this.Handles.FullName, flowStep.GetType().FullName));
            }

            var myAction = scope.Context.DiagnosticSession?.LogStartAction(flowStep);
            try
            {

                var dataSource = bts.InputConnection.ResolveReferenceTo<BiDataFlowStep>(scope).Execute(scope).First() as IDataIntegrator;
                try
                {
                    scope = new DataFlowScope(bts.Name, scope);
                    using (var tx = dataSource.BeginTransaction())
                    {


                        var executeRoot = bts.GetExecutionTreeRoot();
                        scope.Context.Log(System.Diagnostics.Tracing.EventLevel.Verbose, bts.FormatExecutionPlan());
                        // Now we process the terminal objects and execute them
                        var processedRecords = executeRoot.SelectMany(o => o.Execute(scope)).Count();
                        myAction?.LogSample(DataFlowDiagnosticSampleType.TotalRecordProcessed , processedRecords);
                        dataSource.CommitTransaction();
                    }
                }
                catch (Exception e)
                {
                    throw new DataFlowException(flowStep, e);
                }
                yield break;


            }
            finally
            {
                scope.Context.DiagnosticSession?.LogEndAction(myAction);
            }

        }
    }
}
