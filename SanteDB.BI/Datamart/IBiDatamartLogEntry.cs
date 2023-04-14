using SanteDB.Core.Model.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace SanteDB.BI.Datamart
{

    /// <summary>
    /// The log entry type
    /// </summary>
    public enum BiExecutionPurposeType
    {
        /// <summary>
        /// The execution is being started to refresh the data in the data mart
        /// </summary>
        Refresh = 0x1,
        /// <summary>
        /// The execution is for migrating schema
        /// </summary>
        SchemaManagement =0x2,
        /// <summary>
        /// Execution purpose is for database management
        /// </summary>
        DatabaseManagement =0x4,
        /// <summary>
        /// Execution is for read only operations
        /// </summary>
        Discovery = 0x8
    }

    /// <summary>
    /// The outcome of the entry
    /// </summary>
    public enum BiExecutionOutcomeType
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
    public interface IBiDatamartExecutionEntry : IIdentifiedResource
    {

        /// <summary>
        /// Gets the type of the log entry
        /// </summary>
        BiExecutionPurposeType Purpose { get; }

        /// <summary>
        /// Gets the outcome of the log entry
        /// </summary>
        BiExecutionOutcomeType Outcome { get; }

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
        IEnumerable<IBiDatamartLogEntry> LogEntries { get; }
    }

    /// <summary>
    /// Represents a single log entry for the datamart
    /// </summary>
    public interface IBiDatamartLogEntry : IIdentifiedResource
    {

        /// <summary>
        /// Gets the priority of the log entry
        /// </summary>
        EventLevel Priority { get; }

        /// <summary>
        /// Gets the started date of the log entry
        /// </summary>
        DateTimeOffset Timestamp { get; }

        /// <summary>
        /// Gets the text of the log entry
        /// </summary>
        string Text { get; }

    }
}