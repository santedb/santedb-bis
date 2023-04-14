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
    public interface IBiDataFlowExecutionContext : IDisposable
    {

        /// <summary>
        /// The data flow execution context
        /// </summary>
        Guid Key { get; }

        /// <summary>
        /// Get the purpose of this execution context
        /// </summary>
        BiExecutionPurposeType Purpose { get; }

        /// <summary>
        /// Log the end of the BI execution
        /// </summary>
        /// <param name="outcome">The outcome of the execution</param>
        void SetOutcome(BiExecutionOutcomeType outcome);

        /// <summary>
        /// Log to the execution context
        /// </summary>
        /// <param name="priority">The priority of the log</param>
        /// <param name="logText">The log text</param>
        /// <returns>The created log entry</returns>
        IBiDatamartLogEntry Log(EventLevel priority, String logText);

        /// <summary>
        /// The datamart that this execution context applies to
        /// </summary>
        IBiDatamart Datamart { get; }

        /// <summary>
        /// Get an integration implementation for a particular data source
        /// </summary>
        /// <param name="dataSource">The data source for which the data integrator should be obtained</param>
        /// <returns>The integrator</returns>
        IBiDataIntegrator GetIntegrator(BiDataSourceDefinition dataSource);

    }
}
