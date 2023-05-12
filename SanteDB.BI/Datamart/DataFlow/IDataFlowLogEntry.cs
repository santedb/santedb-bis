using SanteDB.Core.Model.Interfaces;
using System;
using System.Diagnostics.Tracing;

namespace SanteDB.BI.Datamart.DataFlow
{

    /// <summary>
    /// Represents a single log entry for the datamart
    /// </summary>
    public interface IDataFlowLogEntry : IIdentifiedResource
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