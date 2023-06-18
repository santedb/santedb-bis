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
using SanteDB.BI.Datamart;
using SanteDB.BI.Datamart.DataFlow;
using SanteDB.BI.Model;
using SanteDB.Core.Model.Query;
using SanteDB.Core.Services;
using System;
using System.Linq.Expressions;

namespace SanteDB.BI.Services
{
    /// <summary>
    /// Represents a BI datamart registry which contains metadata about the various
    /// BI datamarts that exist in the SanteDB deployment.
    /// </summary>
    public interface IBiDatamartRepository : IServiceImplementation
    {

        /// <summary>
        /// Gets the datamarts which are configured with this manager
        /// </summary>
        IQueryResultSet<IDatamart> Find(Expression<Func<IDatamart, bool>> query);

        /// <summary>
        /// Create a new BI datamart based on <paramref name="dataMartDefinition"/>
        /// </summary>
        /// <param name="dataMartDefinition">The datamart definition</param>
        /// <returns>The registered IBiDatamart</returns>
        IDatamart Register(BiDatamartDefinition dataMartDefinition);

        /// <summary>
        /// Removes the <paramref name="datamart"/> from the manager
        /// </summary>
        /// <param name="datamart">The datamart to un-register</param>
        void Unregister(IDatamart datamart);

        /// <summary>
        /// Begins the refresh process for the datamart object
        /// </summary>
        /// <param name="datamart">The datamart to be refreshed</param>
        /// <param name="purpose">The purpose the execution context is being requested</param>
        /// <returns>The execution context which can be used by the executors to </returns>
        IDataFlowExecutionContext GetExecutionContext(IDatamart datamart, DataFlowExecutionPurposeType purpose);

    }
}
