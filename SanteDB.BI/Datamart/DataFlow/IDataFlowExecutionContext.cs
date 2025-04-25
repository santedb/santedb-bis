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
using SanteDB.BI.Model;
using System;
using System.Diagnostics.Tracing;

namespace SanteDB.BI.Datamart.DataFlow
{
    /// <summary>
    /// Represents an execution flow context
    /// </summary>
    public interface IDataFlowExecutionContext : IDisposable
    {

        /// <summary>
        /// The data flow execution context
        /// </summary>
        Guid Key { get; }

        /// <summary>
        /// Get the purpose of this execution context
        /// </summary>
        DataFlowExecutionPurposeType Purpose { get; }

        /// <summary>
        /// Log the end of the BI execution
        /// </summary>
        /// <param name="outcome">The outcome of the execution</param>
        void SetOutcome(DataFlowExecutionOutcomeType outcome);

        /// <summary>
        /// Log to the execution context
        /// </summary>
        /// <param name="priority">The priority of the log</param>
        /// <param name="logText">The log text</param>
        /// <returns>The created log entry</returns>
        IDataFlowLogEntry Log(EventLevel priority, String logText);

        /// <summary>
        /// The datamart that this execution context applies to
        /// </summary>
        IDatamart Datamart { get; }

        /// <summary>
        /// Get an integration implementation for a particular data source
        /// </summary>
        /// <param name="dataSource">The data source for which the data integrator should be obtained</param>
        /// <returns>The integrator</returns>
        IDataIntegrator GetIntegrator(BiDataSourceDefinition dataSource);

        /// <summary>
        /// Gets the current diagnostic session on the object
        /// </summary>
        IDataFlowDiagnosticSession DiagnosticSession { get; }

    }
}
