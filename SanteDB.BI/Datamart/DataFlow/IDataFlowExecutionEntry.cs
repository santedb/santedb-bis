/*
 * Copyright (C) 2021 - 2024, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors
 * Portions Copyright (C) 2015-2018 Mohawk College of Applied Arts and Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2023-6-21
 */
using SanteDB.Core.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Datamart.DataFlow
{

    /// <summary>
    /// The log entry type
    /// </summary>
    [XmlType(nameof(DataFlowExecutionPurposeType), Namespace = BiConstants.XmlNamespace)]
    [Flags]
    public enum DataFlowExecutionPurposeType
    {
        /// <summary>
        /// The purpose of this session is to run diagnostics
        /// </summary>
        [XmlEnum("dx")]
        Diagnostics = 0x1,
        /// <summary>
        /// The execution is being started to refresh the data in the data mart
        /// </summary>
        [XmlEnum("refresh")]
        Refresh = 0x2,
        /// <summary>
        /// The execution is for migrating schema
        /// </summary>
        [XmlEnum("service-manage")]
        SchemaManagement = 0x4,
        /// <summary>
        /// Execution purpose is for database management
        /// </summary>
        [XmlEnum("data-manage")]
        DatabaseManagement = 0x8,
        /// <summary>
        /// Execution is for read only operations
        /// </summary>
        [XmlEnum("disc")]
        Discovery = 0x10
    }

    /// <summary>
    /// The outcome of the entry
    /// </summary>
    [XmlType(nameof(DataFlowExecutionOutcomeType), Namespace = BiConstants.XmlNamespace)]
    public enum DataFlowExecutionOutcomeType
    {
        /// <summary>
        /// Status is unknown
        /// </summary>
        [XmlEnum("unknown")]
        Unknown,
        /// <summary>
        /// The log entry indicates success
        /// </summary>
        [XmlEnum("success")]
        Success,
        /// <summary>
        /// The log entry indicates partial success
        /// </summary>
        [XmlEnum("partial")]
        PartialSuccess,
        /// <summary>
        /// The log entry indicates a failure
        /// </summary>
        [XmlEnum("fail")]
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

        /// <summary>
        /// Gets the diagnostic session if the execution was run in diagnostic mode
        /// </summary>
        Guid? DiagnosticSessionKey { get; }
    }
}
