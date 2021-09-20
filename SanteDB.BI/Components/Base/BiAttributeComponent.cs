using SanteDB.BI.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SanteDB.BI.Components.Base
{
    /// <summary>
    /// Attribute component appends an attribute to the previous element
    /// </summary>
    public class BiAttributeComponent : IBiViewComponent
    {
        /// <summary>
        /// Name of the component
        /// </summary>
        public XName ComponentName => (XNamespace)BiConstants.ComponentNamespace + "attribute";

        /// <summary>
        /// Render the element
        /// </summary>
        public void Render(XElement element, XmlWriter writer, IRenderContext context)
        {
            var fieldOrExpression = element.Value;
            var value = ReportViewUtil.GetValue(context, fieldOrExpression);
            if (value == null && !String.IsNullOrEmpty(element.Attribute("default")?.Value))
                value = ReportViewUtil.GetValue(context, element.Attribute("default")?.Value);

            var attributeName = element.Attribute("name")?.Value;

            // Is there a format?
            var format = element.Attribute("format")?.Value;
            if (value != null)
            {
                if (!String.IsNullOrEmpty(format))
                {
                    if (format.Contains("{0}"))
                        writer.WriteAttributeString(attributeName, String.Format(format, value));
                    else
                        writer.WriteAttributeString(attributeName, String.Format($"{{0:{format}}}", value));
                }
                else
                    writer.WriteAttributeString(attributeName, value.ToString());
            }

        }

        /// <summary>
        /// Validate
        /// </summary>
        public bool Validate(XElement element, IRenderContext context) =>
            element.HasAttributes && element.Attribute("name") != null && !String.IsNullOrEmpty(element.Value);
    }
}
