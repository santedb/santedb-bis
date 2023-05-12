using SanteDB.BI.Exceptions;
using SanteDB.BI.Model;
using SanteDB.Core.i18n;
using System;
using System.Collections.Generic;

namespace SanteDB.BI.Datamart.DataFlow.Executors
{
    /// <summary>
    /// Represents a connection to the database
    /// </summary>
    internal class ConnectionExecutor : IDataFlowStepExecutor
    {
        /// <inheritdoc/>
        public Type Handles => typeof(BiDataFlowConnectionStep);

        /// <inheritdoc/>
        IEnumerable<dynamic> IDataFlowStepExecutor.Execute(BiDataFlowStep flowStep, DataFlowScope scope)
        {
            if (!(flowStep is BiDataFlowConnectionStep bcs))
            {
                throw new ArgumentOutOfRangeException(nameof(flowStep), String.Format(ErrorMessages.ARGUMENT_INCOMPATIBLE_TYPE, this.Handles.FullName, flowStep.GetType().FullName));
            }

            var myAction = scope.Context.DiagnosticSession?.LogStartAction(flowStep);
            try
            {

                if (!scope.TryGetSysVar<IDataIntegrator>(bcs.Name, out IDataIntegrator dataIntegrator))
                {
                    try
                    {
                        // We want to define a connection for a data integrator
                        dataIntegrator = scope.Context.GetIntegrator(bcs.DataSource);
                        if (bcs.Mode == BiDataFlowConnectionMode.ReadWrite)
                        {
                            dataIntegrator.OpenWrite();
                        }
                        else
                        {
                            dataIntegrator.OpenRead();
                        }
                        scope.SetSysVar(bcs.Name, dataIntegrator);
                    }
                    catch (Exception e)
                    {
                        throw new DataFlowException(flowStep, e);
                    }
                }
                yield return dataIntegrator;

            }
            finally
            {
                scope.Context.DiagnosticSession?.LogEndAction(myAction);
            }
        }
    }
}
