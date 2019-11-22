using SanteDB.BI.Exceptions;
using SanteDB.BI.Rendering;
using SanteDB.Core;
using SanteDB.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SanteDB.BI.Components
{
    /// <summary>
    /// Report view utility
    /// </summary>
    internal static class ReportViewUtil
    {

        // Component cache
        private static Dictionary<XName, IBiViewComponent> m_componentCache;

        /// <summary>
        /// Static CTOR
        /// </summary>
        static ReportViewUtil()
        {
            m_componentCache = ApplicationServiceContext.Current.GetService<IServiceManager>()
                .GetAllTypes()
                .Where(o => typeof(IBiViewComponent).GetTypeInfo().IsAssignableFrom(o.GetTypeInfo()) && !o.GetTypeInfo().IsInterface && !o.GetTypeInfo().IsAbstract)
                .Select(o => Activator.CreateInstance(o) as IBiViewComponent)
                .ToDictionary(o => o.ComponentName, o => o);
        }

        /// <summary>
        /// Get the view component for the specified object
        /// </summary>
        internal static IBiViewComponent GetViewComponent(XName elementName)
        {
            IBiViewComponent retVal = null;
            if (!m_componentCache.TryGetValue(elementName, out retVal) && !m_componentCache.TryGetValue(elementName.Namespace + "any", out retVal))
                return null;
            return retVal;

        }

        /// <summary>
        /// Write the specified object to the screen
        /// </summary>
        internal static void Write(XmlWriter writer, XElement el, IRenderContext context)
        {
            {
                // TODO get helper here
                IBiViewComponent component = ReportViewUtil.GetViewComponent(el.Name);
                
                if(component == null)
                {
                    writer.WriteComment($"WARNING: No component for {el.Name} is registered");
                }
                else if (component.Validate(el, context))
                    component.Render(el, writer, context);
                else
                {
#if DEBUG
                    throw new ViewValidationException(el, $"Component {component?.ComponentName} failed validation");
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
        }
    }
}
