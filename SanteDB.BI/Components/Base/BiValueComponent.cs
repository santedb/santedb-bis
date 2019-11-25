using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using ExpressionEvaluator;
using SanteDB.BI.Rendering;

namespace SanteDB.BI.Components.Base
{
    /// <summary>
    /// Binding to a value
    /// </summary>
    public class BiValueComponent : IBiViewComponent
    {

        /// <summary>
        /// Gets the component name
        /// </summary>
        public XName ComponentName => (XNamespace)BiConstants.ComponentNamespace + "value";

        /// <summary>
        /// Render the value
        /// </summary>
        public void Render(XElement element, XmlWriter writer, IRenderContext context)
        {

            var fieldOrExpression = element.Value;
            var value = ReportViewUtil.GetValue(context, fieldOrExpression);

            // Is the required value a change?
            if (element.Attribute("when")?.Value == "changed")
            {
                IDictionary<String, object> watches = context.Parent.Tags["watches"] as IDictionary<String, Object>;

                if (!watches.TryGetValue(fieldOrExpression, out var exisitngValue))
                    watches.Add(fieldOrExpression, value);
                else if (exisitngValue?.Equals(value) == true)
                    return;
            }

            // Is there a format?
            if (!String.IsNullOrEmpty(element.Attribute("format")?.Value))
                writer.WriteString(String.Format($"{{0:{element.Attribute("format").Value}}}", value));
            else
                writer.WriteString(value.ToString());
        }

        /// <summary>
        /// Validate the value element
        /// </summary>
        public bool Validate(XElement element, IRenderContext context)
        {
            return !String.IsNullOrEmpty(element.Value);
        }
    }
}
