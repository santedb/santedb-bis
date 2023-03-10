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
 * Date: 2023-3-10
 */
using SanteDB.BI.Model;
using SanteDB.Core.Model.Query;
using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SanteDB.BI.Services
{
    /// <summary>
    /// Represents a metadata repository for the BIS services
    /// </summary>
    public interface IBiMetadataRepository : IServiceImplementation
    {

        /// <summary>
        /// True if the source of metadata is local
        /// </summary>
        bool IsLocal { get; }

        /// <summary>
        /// Query metadata repository for 
        /// </summary>
        /// <typeparam name="TBisDefinition">The type of definition to query for</typeparam>
        /// <param name="filter">The filter for query</param>
        /// <returns>Matching bis definitions </returns>
        [Obsolete("Use Query(Expression<Func<TBidDefinition, bool>>)", true)]
        IEnumerable<TBisDefinition> Query<TBisDefinition>(Expression<Func<TBisDefinition, bool>> filter, int offset, int? count) where TBisDefinition : BiDefinition;

        /// <summary>
        /// Query the underlying BI definition repository for the specified <typeparamref name="TBisDefinition"/>
        /// </summary>
        /// <typeparam name="TBisDefinition">The type of BI definition being queried</typeparam>
        /// <param name="filter">The filter to apply</param>
        /// <returns>A <see cref="IQueryResultSet"/> representing the results of the query</returns>
        IQueryResultSet<TBisDefinition> Query<TBisDefinition>(Expression<Func<TBisDefinition, bool>> filter)
            where TBisDefinition : BiDefinition;

        /// <summary>
        /// Get the specified BI definition by identifier
        /// </summary>
        /// <typeparam name="TBisDefinition">The type of definition to fetch</typeparam>
        /// <param name="id">The identifier of the object to fetch</param>
        /// <returns>The fetched definition</returns>
        TBisDefinition Get<TBisDefinition>(String id) where TBisDefinition : BiDefinition;

        /// <summary>
        /// Removes the specified BI definition from the repository
        /// </summary>
        /// <typeparam name="TBisDefinition">The type of metadata to remove</typeparam>
        /// <param name="id">The id of the defintion to remove</param>
        void Remove<TBisDefinition>(String id) where TBisDefinition : BiDefinition;

        /// <summary>
        /// Inserts the specified BI definition into the repository
        /// </summary>
        /// <typeparam name="TBisDefinition">The type of BIS definition to be added</typeparam>
        /// <param name="metadata">The metadata to be added</param>
        /// <returns>The inserted data (if the repository made any changes)</returns>
        TBisDefinition Insert<TBisDefinition>(TBisDefinition metadata) where TBisDefinition : BiDefinition;

    }
}
