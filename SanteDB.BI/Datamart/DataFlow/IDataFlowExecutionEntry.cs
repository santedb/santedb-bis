using SanteDB.Core.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanteDB.BI.Datamart.DataFlow
{

    /// <summary>
    /// The log entry type
    /// </summary>
    public enum DataFlowExecutionPurposeType
    {
        /// <summary>
        /// The purpose of this session is to run diagnostics
        /// </summary>
        Diagnostics = 0x1,
        /// <summary>
        /// The execution is being started to refresh the data in the data mart
        /// </summary>
        Refresh = 0x2,
        /// <summary>
        /// The execution is for migrating schema
        /// </summary>
        SchemaManagement = 0x4,
        /// <summary>
        /// Execution purpose is for database management
        /// </summary>
        DatabaseManagement = 0x8,
        /// <summary>
        /// Execution is for read only operations
        /// </summary>
        Discovery = 0x10
    }

    /// <summary>
    /// The outcome of the entry
    /// </summary>
    public enum DataFlowExecutionOutcomeType
    {
        /// <summary>
        /// Status is unknown
        /// </summary>
        Unknown,
        /// <summary>
        /// The log entry indicates success
        /// </summary>
        Success,
        /// <summary>
        /// The log entry indicates partial success
        /// </summary>
        PartialSuccess,
        /// <summary>
        /// The log entry indicates a failure
        /// </summary>
        Fail
    }

    /// <summary>
    /// Datamart execution entry
    /// </summary>
    public interface IDataFlowExecutionEntry : IIdentifiedResource
    {

        /// <summary>
        /// Gets the type of the log entry
        /// </summary>
        DataFlowExecutionPurposeType Purpose { get; }

        /// <summary>
        /// Gets the outcome of the log entry
        /// </summary>
        DataFlowExecutionOutcomeType Outcome { get; }

        /// <summary>
        /// Gets the started date of the log entry
        /// </summary>
        DateTimeOffset Started { get; }

        /// <summary>
        /// Gets the finished date of the log entry
        /// </summary>
        DateTimeOffset? Finished { get; }

        /// <summary>
        /// Gets the user that created this log entry
        /// </summary>
        Guid? CreatedByKey { get; }

        /// <summary>
        /// Get the log entries
        /// </summary>
        IEnumerable<IDataFlowLogEntry> LogEntries { get; }
    }
}
