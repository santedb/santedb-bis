using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.BI.Rendering
{
    /// <summary>
    /// Represents a rendering context
    /// </summary>
    public interface IRenderContext
    {

        /// <summary>
        /// Gets the parent of this element
        /// </summary>
        IRenderContext Parent { get; }

        /// <summary>
        /// Gets the root of this element
        /// </summary>
        IRenderContext Root { get; }

        /// <summary>
        /// Gets the scope object
        /// </summary>
        dynamic ScopedObject { get; }

    }
}
