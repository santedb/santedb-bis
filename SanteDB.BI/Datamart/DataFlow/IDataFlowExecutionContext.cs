using SanteDB.BI.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text;

namespace SanteDB.BI.Datamart.DataFlow
{
    /// <summary>
    /// Represents an execution flow context
    /// </summary>
    public interface IDataFlowExecutionContext : IDisposable
    {

        /// <summary>
        /// The data flow execution context
        /// </summary>
        Guid Key { get; }

        /// <summary>
        /// Get the purpose of this execution context
        /// </summary>
        DataFlowExecutionPurposeType Purpose { get; }

        /// <summary>
        /// Log the end of the BI execution
        /// </summary>
        /// <param name="outcome">The outcome of the execution</param>
        void SetOutcome(DataFlowExecutionOutcomeType outcome);

        /// <summary>
        /// Log to the execution context
        /// </summary>
        /// <param name="priority">The priority of the log</param>
        /// <param name="logText">The log text</param>
        /// <returns>The created log entry</returns>
        IDataFlowLogEntry Log(EventLevel priority, String logText);

        /// <summary>
        /// The datamart that this execution context applies to
        /// </summary>
        IDatamart Datamart { get; }

        /// <summary>
        /// Get an integration implementation for a particular data source
        /// </summary>
        /// <param name="dataSource">The data source for which the data integrator should be obtained</param>
        /// <returns>The integrator</returns>
        IDataIntegrator GetIntegrator(BiDataSourceDefinition dataSource);

        /// <summary>
        /// Gets the current diagnostic session on the object
        /// </summary>
        IDataFlowDiagnosticSession DiagnosticSession { get; }

    }
}
