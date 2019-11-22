using SanteDB.BI.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SanteDB.BI.Components
{
    /// <summary>
    /// Represents a BI View component which can render content 
    /// </summary>
    public interface IBiViewComponent
    {
        /// <summary>
        /// Gets the name of the component which is to be rendered
        /// </summary>
        XName ComponentName { get; }

        /// <summary>
        /// Returns true if this component renderer can render the specified element
        /// </summary>
        /// <param name="source">The source renderer</param>
        /// <param name="element">The element being tested for render</param>
        /// <returns>The render object</returns>
        bool Validate(XElement element, IRenderContext context);

        /// <summary>
        /// Renders the specified view component
        /// </summary>
        /// <param name="element">The element to be rendered</param>
        /// <param name="writer">The writer to which output HTML should be appended</param>
        /// <param name="context">The current context of the report in this render</param>
        void Render(XElement element, XmlWriter writer, IRenderContext context);

    }
}
