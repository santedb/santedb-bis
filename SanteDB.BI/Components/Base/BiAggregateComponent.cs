/*
 * Copyright (C) 2021 - 2023, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-3-10
 */
using SanteDB.BI.Rendering;
using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

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
            using (var dataSource = (context.Root as RootRenderContext).GetOrExecuteQuery(dataSourceName))
            {

                // Now we want to select the values for this object
                if (String.IsNullOrEmpty(fieldOrExpression))
                {
                    fieldOrExpression = "!null";
                }

                var expression = context.CompileExpression(fieldOrExpression);
                object value = null;
                switch (function)
                {
                    case "sum":
                        value = dataSource.Records.Sum(o => (decimal)expression.Invoke(ReportViewUtil.ToParameterArray(o)));
                        break;

                    case "count":
                        if (expression.ReturnType == typeof(bool))
                        {
                            value = dataSource.Records.Count(o => expression.Invoke(ReportViewUtil.ToParameterArray(o)));
                        }
                        else
                        {
                            value = dataSource.Records.Count(o => expression.Invoke(ReportViewUtil.ToParameterArray(o)) != null);
                        }
                        break;

                    case "count-distinct":
                        value = dataSource.Records.Select(o => expression.Invoke(ReportViewUtil.ToParameterArray(o))).Distinct().Count();
                        break;

                    case "min":
                        value = dataSource.Records.Min(o => (decimal)expression.Invoke(ReportViewUtil.ToParameterArray(o)));
                        break;

                    case "max":
                        value = dataSource.Records.Max(o => (decimal)expression.Invoke(ReportViewUtil.ToParameterArray(o)));
                        break;

                    case "avg":
                        value = dataSource.Records.Average(o => (decimal)expression.Invoke(ReportViewUtil.ToParameterArray(o)));
                        break;

                    default:
                        throw new ArgumentOutOfRangeException($"Aggregate function {function} is not known");
                }

                // Is there a format?
                if (!String.IsNullOrEmpty(element.Attribute("format")?.Value))
                {
                    writer.WriteString(String.Format($"{{0:{element.Attribute("format").Value}}}", value));
                }
                else
                {
                    writer.WriteString(value.ToString());
                }
            }

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