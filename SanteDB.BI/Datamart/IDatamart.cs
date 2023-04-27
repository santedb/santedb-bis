﻿using SanteDB.BI.Datamart.DataFlow;
using SanteDB.BI.Model;
using SanteDB.Core.Configuration.Data;
using SanteDB.Core.Model.Attributes;
using SanteDB.Core.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanteDB.BI.Datamart
{
    /// <summary>
    /// An abstract representation of a datamart which has been registered in the manager
    /// </summary>
    public interface IDatamart : IIdentifiedResource, INonVersionedData
    {
        /// <summary>
        /// Get the unique identifier for this datamart
        /// </summary>
        [QueryParameter("id")]
        String Id { get; }

        /// <summary>
        /// Get the name of the datamart
        /// </summary>
        [QueryParameter("name")]
        String Name { get; }

        /// <summary>
        /// Get the description for the datamart
        /// </summary>
        [QueryParameter("description")]
        String Description { get; }

        /// <summary>
        /// Get the version of the deployed datamart
        /// </summary>
        [QueryParameter("version")]
        String Version { get; }

        /// <summary>
        /// Gets the log entries related to this datamart entry
        /// </summary>
        IEnumerable<IDataFlowExecutionEntry> FlowExecutions { get; }

    }
}
