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