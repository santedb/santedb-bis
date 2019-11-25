﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using SanteDB.BI.Rendering;

namespace SanteDB.BI.Components.Base
{
    /// <summary>
    /// Represents a view component which can render an HTML element
    /// </summary>
    public class HtmlElementComponent : IBiViewComponent
    {
        /// <summary>
        /// Handles all HTML
        /// </summary>
        public XName ComponentName => (XNamespace)BiConstants.HtmlNamespace + "any";

        /// <summary>
        /// Render hte element
        /// </summary>
        public void Render(XElement element, XmlWriter writer, IRenderContext context)
        {

            writer.WriteStartElement(element.Name.LocalName, element.Name.NamespaceName);

            // Render attributes
            foreach (var itm in element.Attributes())
                writer.WriteAttributeString(itm.Name.LocalName, itm.Value);

            // Render children
            foreach (var node in element.Nodes())
            {
                if (node is XElement)
                    ReportViewUtil.Write(writer, node as XElement, context);
                else if (node is XText)
                {
                    var text = (node as XText).Value;
                    if(!String.IsNullOrWhiteSpace(text))
                        writer.WriteString(text);
                }
            }
            writer.WriteEndElement();

        }

        /// <summary>
        /// Validate the rendering can be done
        /// </summary>
        public bool Validate(XElement element, IRenderContext context)
        {
            return true;
        }
    }
}
