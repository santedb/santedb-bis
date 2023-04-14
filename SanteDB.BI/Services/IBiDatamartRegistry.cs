using SanteDB.BI.Datamart;
using SanteDB.BI.Datamart.DataFlow;
using SanteDB.BI.Model;
using SanteDB.Core.Model.Query;
using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Text;

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
        IQueryResultSet<IBiDatamart> Find(Expression<Func<IBiDatamart, bool>> query);

        /// <summary>
        /// Create a new BI datamart based on <paramref name="dataMartDefinition"/>
        /// </summary>
        /// <param name="dataMartDefinition">The datamart definition</param>
        /// <returns>The registered IBiDatamart</returns>
        IBiDatamart Register(BiDatamartDefinition dataMartDefinition);

        /// <summary>
        /// Removes the <paramref name="datamart"/> from the manager
        /// </summary>
        /// <param name="datamart">The datamart to un-register</param>
        void Unregister(IBiDatamart datamart);

        /// <summary>
        /// Begins the refresh process for the datamart object
        /// </summary>
        /// <param name="datamart">The datamart to be refreshed</param>
        /// <param name="purpose">The purpose the execution context is being requested</param>
        /// <returns>The execution context which can be used by the executors to </returns>
        IBiDataFlowExecutionContext GetExecutionContext(IBiDatamart datamart, BiExecutionPurposeType purpose);

    }
}
