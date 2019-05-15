using SanteDB.BI.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.BI.Services
{
    /// <summary>
    /// Represents a PIVOT provider which can take a dataset and pivot it
    /// </summary>
    public interface IBiPivotProvider
    {

        /// <summary>
        /// Pivots <paramref name="context"/> in place returning it for chaining
        /// </summary>
        /// <param name="context">The result context to be pivoted</param>
        /// <param name="pivot">The pivot definition to apply</param>
        /// <returns>The pivoted context</returns>
        BisResultContext Pivot(BisResultContext context, BiViewPivotDefinition pivot);

    }
}
