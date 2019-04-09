﻿using SanteDB.BI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.BI.Services
{
    /// <summary>
    /// Represents a data source which can execute specified <see cref="SanteDB.BI.Model.BisQueryDefinition"/> and return results
    /// </summary>
    public interface IBisDataSource
    {

        /// <summary>
        /// Executes the specified <paramref name="queryDefinition"/> using the specified <paramref name="parameters"/>
        /// </summary>
        /// <param name="queryDefinition">The query definition to be executed</param>
        /// <param name="parameters">The parameter values to supply</param>
        /// <returns>A query result indicating the results of the query</returns>
        BisResultContext ExecuteQuery(BisQueryDefinition queryDefinition, Dictionary<String, Object> parameters);

        /// <summary>
        /// Executes the specified <paramref name="queryDefinition"/> using the specified <paramref name="parameters"/>
        /// </summary>
        /// <param name="queryId">The ID of the query definition to execute</param>
        /// <param name="parameters">The parameter values to supply</param>
        /// <returns>A query result indicating the results of the query</returns>
        BisResultContext ExecuteQuery(String queryId, Dictionary<String, Object> parameters);

    }
}
