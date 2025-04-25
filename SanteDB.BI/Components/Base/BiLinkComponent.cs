/*
 * Copyright (C) 2021 - 2025, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors
 * Portions Copyright (C) 2015-2018 Mohawk College of Applied Arts and Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2025-1-10
 */
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
