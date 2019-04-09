using SanteDB.BI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.BI.Services
{
    /// <summary>
    /// Represents a metadata repository for the BIS services
    /// </summary>
    public interface IBisMetadataRepository
    {

        /// <summary>
        /// Query metadata repository for 
        /// </summary>
        /// <typeparam name="TBisDefinition">The type of definition to query for</typeparam>
        /// <param name="filter">The filter for query</param>
        /// <returns>Matching bis definitions </returns>
        IEnumerable<TBisDefinition> Query<TBisDefinition>(Expression<Func<TBisDefinition, bool>> filter) where TBisDefinition : BisDefinition;

        /// <summary>
        /// Get the specified BI definition by identifier
        /// </summary>
        /// <typeparam name="TBisDefinition">The type of definition to fetch</typeparam>
        /// <param name="id">The identifier of the object to fetch</param>
        /// <returns>The fetched definition</returns>
        TBisDefinition Get<TBisDefinition>(String id) where TBisDefinition : BisDefinition;

        /// <summary>
        /// Removes the specified BI definition from the repository
        /// </summary>
        /// <typeparam name="TBisDefinition">The type of metadata to remove</typeparam>
        /// <param name="id">The id of the defintion to remove</param>
        void Remove<TBisDefinition>(String id) where TBisDefinition : BisDefinition;

        /// <summary>
        /// Inserts the specified BI definition into the repository
        /// </summary>
        /// <typeparam name="TBisDefinition">The type of BIS definition to be added</typeparam>
        /// <param name="metadata">The metadata to be added</param>
        /// <returns>The inserted data (if the repository made any changes)</returns>
        TBisDefinition Insert<TBisDefinition>(TBisDefinition metadata) where TBisDefinition : BisDefinition;

    }
}
