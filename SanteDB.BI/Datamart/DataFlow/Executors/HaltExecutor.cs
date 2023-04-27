using SanteDB.BI.Exceptions;
using SanteDB.BI.Model;
using SanteDB.Core;
using SanteDB.Core.i18n;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SanteDB.BI.Datamart.DataFlow.Executors
{
    /// <summary>
    /// Flow step execution for halt
    /// </summary>
    internal class HaltExecutor : DataStreamExecutorBase<BiDataFlowHaltStep>
    {

        /// <summary>
        /// Process the stream
        /// </summary>
        protected override IEnumerable<dynamic> ProcessStream(BiDataFlowHaltStep flowStep, DataFlowScope scope, IEnumerable<dynamic> inputStream)
        {
            var diagnosticLog = scope.Context.DiagnosticSession?.LogStartAction(flowStep);
            try
            {
                foreach (object itm in inputStream)
                {
                    throw new DataFlowException(flowStep, flowStep.Message.FormatString(itm));
                }
                yield break;
            }
            finally
            {
                scope.Context.DiagnosticSession?.LogEndAction(diagnosticLog);
            }
        }
    }
}
