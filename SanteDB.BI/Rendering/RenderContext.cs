using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SanteDB.BI.Model;

namespace SanteDB.BI.Rendering
{
    /// <summary>
    /// Represents a report rendering context implementation linked to a report
    /// </summary>
    public class RenderContext  : IRenderContext
    {

       
        /// <summary>
        /// Creates a new child rendering context
        /// </summary>
        public RenderContext(IRenderContext parent, Object scopedObject)
        {
            this.Parent = parent;
            this.ScopedObject = scopedObject;
            this.Tags = new Dictionary<String, Object>();
        }

        /// <summary>
        /// Gets the root
        /// </summary>
        public IRenderContext Root => this.Parent?.Root ?? this;

        /// <summary>
        /// Gets the parent context
        /// </summary>
        public IRenderContext Parent { get; }

        /// <summary>
        /// Gets the scoped object
        /// </summary>
        public dynamic ScopedObject { get; }

        /// <summary>
        /// Report watches for this instance of the report
        /// </summary>
        public IDictionary<String, Object> Tags { get; }
    }
}
