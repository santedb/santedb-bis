/*
 * Copyright (C) 2021 - 2021, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2021-8-5
 */
using SanteDB.BI.Rendering;
using SanteDB.Core;
using SanteDB.Core.Model.Query;
using SanteDB.Core.Model.Serialization;
using SanteDB.Core.Services;
using System;
using System.Collections.Concurrent;
using System.Xml;
using System.Xml.Linq;

namespace SanteDB.BI.Components.Data
{
    /// <summary>
    /// Fetches a resource from the database/cache and passes it to the value for rendering
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class BiFetch : IBiViewComponent
    {

        /// <summary>
        /// Selectors
        /// </summary>
        private ConcurrentDictionary<String, Delegate> m_selectors = new ConcurrentDictionary<string, Delegate>();

        /// <summary>
        /// Gets the component name
        /// </summary>
        public XName ComponentName => (XNamespace)BiConstants.ComponentNamespace + "refer";

        /// <summary>
        /// Render the object
        /// </summary>
        public void Render(XElement element, XmlWriter writer, IRenderContext context)
        {
            var resourceType = new ModelSerializationBinder().BindToType(String.Empty, element.Attribute("resource")?.Value);
            if (resourceType == null)
            {
                writer.WriteStartElement("span");
                writer.WriteAttributeString("style", "color:red");
                writer.WriteString($"{element.Attribute("resource")?.Value} not valid");
                writer.WriteEndElement();
            }
            else
            {
                var render = element.Attribute("render")?.Value;
                var identifier = ReportViewUtil.GetValue(context, element.Value);
                if (identifier != null)
                {
                    var dpType = typeof(IDataPersistenceService<>).MakeGenericType(resourceType);
                    var dpService = ApplicationServiceContext.Current.GetService(dpType) as IDataPersistenceService;
                    var key = identifier is Guid || identifier is Guid? ? (Guid)identifier : Guid.Parse(identifier.ToString());
                    var instance = dpService.Get(key);

                    if (instance == null)
                        writer.WriteComment($"{resourceType.Name}/{key} not found");
                    else if (!String.IsNullOrEmpty(render))
                    {
                        var selKey = $"{resourceType.Name}.{render}";
                        if (!this.m_selectors.TryGetValue(selKey, out Delegate selDelegate))
                        {
                            selDelegate = QueryExpressionParser.BuildPropertySelector(resourceType, render).Compile();
                            this.m_selectors.TryAdd(selKey, selDelegate);
                        }
                        writer.WriteString(selDelegate.DynamicInvoke(instance)?.ToString());
                    }
                    else
                        writer.WriteString(instance?.ToString());
                }
            }
        }

        /// <summary>
        /// Validate the object
        /// </summary>
        public bool Validate(XElement element, IRenderContext context)
        {
            return !String.IsNullOrEmpty(element.Attribute("resource")?.Value) &&
                !String.IsNullOrEmpty(element.Value)
                && !String.IsNullOrEmpty(element.Attribute("render")?.Value);
        }
    }
}
