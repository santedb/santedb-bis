using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using SanteDB.BI.Exceptions;
using SanteDB.BI.Rendering;
using SanteDB.BI.Services;
using SanteDB.Core;

namespace SanteDB.BI.Components.Base
{
    /// <summary>
    /// Repeating control for data, changes the current data context
    /// </summary>
    public class BiRepeatComponent : IBiViewComponent
    {
        /// <summary>
        /// Gets the component name
        /// </summary>
        public XName ComponentName => (XNamespace)BiConstants.ComponentNamespace + "repeat";

        /// <summary>
        /// Render the specified element
        /// </summary>
        public void Render(XElement element, XmlWriter writer, RenderContext context)
        {
            // Run dataset and start context
            var dataSource = (context.Root as RootRenderContext).GetOrExecuteQuery(element.Attribute("source").Value);
            var thisContext = new RenderContext(context, dataSource.Dataset);

            writer.WriteComment($"start repeat : {dataSource.QueryDefinition.Id}");

            foreach(var itm in dataSource.Dataset)
                foreach(var el in element.Elements())
                {
                    // TODO get helper here
                    IBiViewComponent component = null; // ComponentUtil.GetRenderer()

                    if(component.Validate(el, thisContext))
                        component.Render(el, writer, thisContext);
                    else
                    {
#if DEBUG
                        throw new ViewValidationException(el, $"Component {component.ComponentName} failed validation");
#else
                        writer.WriteStartElement("em", BiConstants.HtmlNamespace);
                        writer.WriteAttributeString("style", "color: #f00");
                        StringBuilder path = new StringBuilder($"/{el.Name}");
                        var p = el.Parent;
                        while (p != el.Document.Root && p != null) {
                            path.Insert(0, $"/{p.Name}");
                            p = p.Parent;
                        }
                        writer.WriteString($"Component {component.ComponentName} failed validation at {path}");
                        writer.WriteEndElement(); // em
#endif
                    }

                }

            writer.WriteComment($"end repeat : {dataSource.QueryDefinition.Id}");
        }

        /// <summary>
        /// Validate that this component is correctly represented prior to calling render
        /// </summary>
        public bool Validate(XElement element, RenderContext context)
        {
            return element.HasAttributes && element.Attribute("source") != null &&
                (context.Root as RootRenderContext).HasDataSource(element.Attribute("source").Value);
        }
    }
}
