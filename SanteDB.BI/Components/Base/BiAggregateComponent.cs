/*
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
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
 * Date: 2021-2-9
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using SanteDB.BI.Rendering;
using SanteDB.Core.Model.Map;

namespace SanteDB.BI.Components.Base
{
    /// <summary>
    /// Represents a bi aggregation component
    /// </summary>
    public class BiAggregateComponent : IBiViewComponent
    {
        /// <summary>
        /// Get the component name
        /// </summary>
        public XName ComponentName => (XNamespace)BiConstants.ComponentNamespace + "aggregate";

        /// <summary>
        /// Render the element
        /// </summary>
        public void Render(XElement element, XmlWriter writer, IRenderContext context)
        {

            // Get the data source
            var dataSourceName = element.Attribute("source").Value;
            var function = element.Attribute("fn").Value;
            var fieldOrExpression = element.Value;

            // Run dataset and start context
            var dataSource = (context.Root as RootRenderContext).GetOrExecuteQuery(dataSourceName);

            // Now we want to select the values for this object
            if (String.IsNullOrEmpty(fieldOrExpression))
                fieldOrExpression = "!null";

            var expression = context.CompileExpression(fieldOrExpression);
            object value = null;
            switch (function)
            {
                case "sum":
                    value = dataSource.Dataset.Sum(o => (decimal)expression.DynamicInvoke(o));
                    break;
                case "count":
                    if(expression.Method.ReturnType == typeof(bool))
                        value = dataSource.Dataset.Count(o => expression.DynamicInvoke(o));
                    else
                    {
                        value = dataSource.Dataset.Count(o => expression.DynamicInvoke(o) != null);
                    }
                    break;
                case "count-distinct":
                    value = dataSource.Dataset.Select(o => expression.DynamicInvoke(o)).Distinct().Count();
                    break;
                case "min":
                    value = dataSource.Dataset.Min(o => (decimal)expression.DynamicInvoke(o));
                    break;
                case "max":
                    value = dataSource.Dataset.Max(o => (decimal)expression.DynamicInvoke(o));
                    break;
                case "avg":
                    value = dataSource.Dataset.Average(o => (decimal)expression.DynamicInvoke(o));
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Aggregate function {function} is not known");
            }

            // Is there a format?
            if (!String.IsNullOrEmpty(element.Attribute("format")?.Value))
                writer.WriteString(String.Format($"{{0:{element.Attribute("format").Value}}}", value));
            else
                writer.WriteString(value.ToString());
        }

        /// <summary>
        /// Validate the object
        /// </summary>
        public bool Validate(XElement element, IRenderContext context)
        {
            return !String.IsNullOrEmpty(element.Attribute("fn")?.Value) && !String.IsNullOrEmpty(element.Attribute("source")?.Value);
        }
    }
}
