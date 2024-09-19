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
 */
using SanteDB.BI.Datamart.DataFlow;
using SanteDB.Core.Model.Attributes;
using SanteDB.Core.Model.Interfaces;
using System;
using System.Collections.Generic;

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

        /// <summary>
        /// Gets the hash of the definition
        /// </summary>
        byte[] DefinitionHash { get; }

    }
}
