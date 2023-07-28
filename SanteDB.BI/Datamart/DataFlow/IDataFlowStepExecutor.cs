/*
 * Copyright (C) 2021 - 2023, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-5-19
 */
using SanteDB.BI.Model;
using System;
using System.Collections.Generic;

namespace SanteDB.BI.Datamart.DataFlow
{
    /// <summary>
    /// Represents an execution class for the various <see cref="BI.Model.BiDataFlowStreamStep"/>
    /// </summary>
    internal interface IDataFlowStepExecutor
    {

        /// <summary>
        /// Gets the type of streaming step this executor handles
        /// </summary>
        Type Handles { get; }

        /// <summary>
        /// Execute the <paramref name="flowStep"/>.
        /// </summary>
        /// <param name="flowStep">The flow step to be executed</param>
        /// <param name="scope">The current scope of the context</param>
        /// <returns>The transformed or input data objects</returns>
        IEnumerable<dynamic> Execute(BiDataFlowStep flowStep, DataFlowScope scope);

    }
}
