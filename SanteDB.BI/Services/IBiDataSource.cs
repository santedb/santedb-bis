/*
 * Based on OpenIZ, Copyright (C) 2015 - 2019 Mohawk College of Applied Arts and Technology
 * Copyright (C) 2019 - 2020, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
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
 * Date: 2019-11-27
 */
using SanteDB.BI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.BI.Services
{
    /// <summary>
    /// Represents a data source which can execute specified <see cref="SanteDB.BI.Model.BiQueryDefinition"/> and return results
    /// </summary>
    public interface IBiDataSource
    {

        /// <summary>
        /// Executes the specified <paramref name="queryDefinition"/> using the specified <paramref name="parameters"/>
        /// </summary>
        /// <param name="queryDefinition">The query definition to be executed</param>
        /// <param name="parameters">The parameter values to supply</param>
        /// <returns>A query result indicating the results of the query</returns>
        BisResultContext ExecuteQuery(BiQueryDefinition queryDefinition, IDictionary<String, Object> parameters, BiAggregationDefinition[] aggregation, int offset, int? count);

        /// <summary>
        /// Executes the specified <paramref name="queryDefinition"/> using the specified <paramref name="parameters"/>
        /// </summary>
        /// <param name="queryId">The ID of the query definition to execute</param>
        /// <param name="parameters">The parameter values to supply</param>
        /// <returns>A query result indicating the results of the query</returns>
        BisResultContext ExecuteQuery(String queryId, IDictionary<String, Object> parameters, BiAggregationDefinition[] aggregation, int offset, int? count);

        /// <summary>
        /// Executes the specified view
        /// </summary>
        /// <param name="viewDef"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        BisResultContext ExecuteView(BiViewDefinition viewDef, IDictionary<string, object> parameters, int offset, int? count);
    }
}
