/*
 * Copyright (C) 2021 - 2025, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
using SanteDB.BI.Datamart.DataFlow;
using SanteDB.BI.Model;
using SanteDB.Core.Services;
using System;

namespace SanteDB.BI.Services
{
    /// <summary>
    /// Represents a flow executor 
    /// </summary>
    public interface IBiDatamartManager : IServiceImplementation
    {

        /// <summary>
        /// Fired after a diagnostic sample was received
        /// </summary>
        event EventHandler DiagnosticEventReceived;

        /// <summary>
        /// Migrate the datamart schema specified by <paramref name="datamartDefinition"/>
        /// </summary>
        /// <param name="datamartDefinition">The datamart definition to migrate</param>
        void Migrate(BiDatamartDefinition datamartDefinition);

        /// <summary>
        /// Refresh the datamart specified by <paramref name="datamartDefinition"/>
        /// </summary>
        /// <param name="datamartDefinition">The datamart definition</param>
        /// <param name="includeDiagnostics">True if diagnostics should be included</param>
        /// <returns>The diagnostic session inforamtion for the refresh operation</returns>
        IDataFlowDiagnosticSession Refresh(BiDatamartDefinition datamartDefinition, bool includeDiagnostics);

        /// <summary>
        /// Destroy the datamart data
        /// </summary>
        /// <param name="datamartDefinition"></param>
        void Destroy(BiDatamartDefinition datamartDefinition);

    }
}
