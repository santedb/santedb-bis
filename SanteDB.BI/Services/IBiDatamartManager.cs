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
        /// Migrate the datamart schema specified by <paramref name="datamartDefinition"/>
        /// </summary>
        /// <param name="datamartDefinition">The datamart definition to migrate</param>
        void Migrate(BiDatamartDefinition datamartDefinition);

        /// <summary>
        /// Refresh the datamart specified by <paramref name="datamartDefinition"/>
        /// </summary>
        /// <param name="datamartDefinition">The datamart definition</param>
        void Refresh(BiDatamartDefinition datamartDefinition);

        /// <summary>
        /// Destroy the datamart data
        /// </summary>
        /// <param name="datamartDefinition"></param>
        void Destroy(BiDatamartDefinition datamartDefinition);

    }
}
