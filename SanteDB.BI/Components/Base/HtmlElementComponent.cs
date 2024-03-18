/*
 * Copyright (C) 2021 - 2024, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-6-21
 */
using SanteDB.BI.Rendering;
using System;
using System.Xml;
using System.Xml.Linq;

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
            {
                writer.WriteAttributeString(itm.Name.LocalName, itm.Value);
            }

            // Render children
            foreach (var node in element.Nodes())
            {
                if (node is XElement ele)
                {
                    ReportViewUtil.Write(writer, ele, context);
                }
                else if (node is XText xtext)
                {
                    var text = xtext.Value;
                    if (!String.IsNullOrWhiteSpace(text))
                    {
                        writer.WriteString(text);
                    }
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
