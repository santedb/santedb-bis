﻿using SanteDB.BI.Rendering;
using SanteDB.Core;
using SanteDB.Core.Model.Query;
using SanteDB.Core.Model.Serialization;
using SanteDB.Core.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
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
