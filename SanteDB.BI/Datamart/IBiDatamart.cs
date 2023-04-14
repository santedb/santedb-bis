using SanteDB.BI.Model;
using SanteDB.Core.Configuration.Data;
using SanteDB.Core.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanteDB.BI.Datamart
{
    /// <summary>
    /// An abstract representation of a datamart which has been registered in the manager
    /// </summary>
    public interface IBiDatamart : IIdentifiedResource, INonVersionedData
    {
        /// <summary>
        /// Get the unique identifier for this datamart
        /// </summary>
        String Id { get; }

        /// <summary>
        /// Get the name of the datamart
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Get the description for the datamart
        /// </summary>
        String Description { get; }

        /// <summary>
        /// Get the version of the deployed datamart
        /// </summary>
        String Version { get; }

        /// <summary>
        /// Gets the log entries related to this datamart entry
        /// </summary>
        IEnumerable<IBiDatamartExecutionEntry> Executions { get; }

    }
}
