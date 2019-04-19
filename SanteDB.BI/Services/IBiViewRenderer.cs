using SanteDB.BI.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.BI.Services
{
    /// <summary>
    /// Represents a view renderer which can render a particular view given a particular context
    /// </summary>
    public interface IBiViewRenderer
    {

        /// <summary>
        /// Render the specified view
        /// </summary>
        /// <param name="resultContext">The results</param>
        /// <param name="viewDefinition">The view definition</param>
        /// <returns>The rendered output stream</returns>
        Stream Render(BisResultContext resultContext, BiViewDefinition viewDefinition);

    }
}
