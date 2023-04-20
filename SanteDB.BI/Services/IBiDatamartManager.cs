using SanteDB.BI.Datamart.DataFlow;
using SanteDB.BI.Model;
using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

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
