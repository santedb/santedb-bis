using SanteDB.BI.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace SanteDB.BI.Components.Base
{
    /// <summary>
    /// Represents a link to an object
    /// </summary>
    public class BiLinkComponent : IBiViewComponent
    {

        private static readonly Regex m_extractBindingRegex = new Regex(@"\$\{([^\}\:]+)(?::([^\}]+))?\}", RegexOptions.Compiled);

        /// <inheritdoc/>
        public XName ComponentName => (XNamespace)BiConstants.ComponentNamespace + "link";

        /// <inheritdoc/>
        public void Render(XElement element, XmlWriter writer, IRenderContext context)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            else if (element.Attribute("href") == null)
            {
                throw new InvalidOperationException("Cannot find destination HREF attribute");
            }
            else if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            else if (context == null)
            {
                throw new ArgumentException(nameof(context));
            }

            var targetUrl = m_extractBindingRegex.Replace(element.Attribute("href").Value, (o) =>
            {
                // Attempt to get the binding parameter
                var format = String.Empty;
                if (!String.IsNullOrEmpty(o.Groups[2].Value))
                {
                    format = $":{o.Groups[2].Value}";
                }
                return String.Format($"{{0}}{format}", ReportViewUtil.GetValue(context, o.Groups[1].Value));
            });

            writer.WriteStartElement("a", BiConstants.HtmlNamespace);

            writer.WriteAttributeString("_target", "blank");
            writer.WriteAttributeString("href", targetUrl);

            foreach (var itm in element.Nodes())
            {
                ReportViewUtil.Write(writer, itm, context);
            }

            writer.WriteEndElement(); // a

        }

        /// <inheritdoc/>
        public bool Validate(XElement element, IRenderContext context) => !String.IsNullOrEmpty(element.Attribute("href")?.Value);
    }
}
