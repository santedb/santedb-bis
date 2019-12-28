/*
 * Copyright 2015-2019 Mohawk College of Applied Arts and Technology
 * Copyright 2019-2019 SanteSuite Contributors (See NOTICE)
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
 * User: Justin Fyfe
 * Date: 2019-12-9
 */
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using SanteDB.BI.Rendering;
using System.Reflection;
using SanteDB.Core.Model;

namespace SanteDB.BI.Components.Chart
{
    /// <summary>
    /// Chart component
    /// </summary>
    public class BiChartComponent : IBiViewComponent
    {
        /// <summary>
        /// Get the component name
        /// </summary>
        public XName ComponentName => (XNamespace)BiConstants.ComponentNamespace + "chart";

        /// <summary>
        /// Render the chart
        /// </summary>
        public void Render(XElement element, XmlWriter writer, IRenderContext context)
        {
            var dataSource = (context.Root as RootRenderContext).GetOrExecuteQuery(element.Attribute("source").Value);
            if (dataSource.Dataset.Count() == 0)
                writer.WriteElementString("strong", BiConstants.HtmlNamespace, $"{dataSource.QueryDefinition.Name} - 0 REC");
            else
            {

                // Render the object 
                writer.WriteStartElement("chart", BiConstants.HtmlNamespace);
                writer.WriteAttributeString("type", $"'{element.Attribute("type").Value}'");
                writer.WriteAttributeString("legend", $"{element.Attribute("legend")?.Value ?? "false" }");

                var title = element.Element((XNamespace)BiConstants.ComponentNamespace + "title");
                if (title != null)
                    writer.WriteAttributeString("title", $"'{title.Value}'");

                var chartContext = new RenderContext(context, dataSource.Dataset);
                chartContext.Tags.Add("expressions", new Dictionary<String, Delegate>());
                // Now sort the result set by the key
                var labels = element.Element((XNamespace)BiConstants.ComponentNamespace + "labels");
                var axis = element.Element((XNamespace)BiConstants.ComponentNamespace + "axis");
                var axisDataExpression = (labels ?? axis).Attribute("data").Value;
                var axisFormat = (labels ?? axis).Attribute("format")?.Value;
                var axisSelector = ReportViewUtil.CompileExpression(new RenderContext(chartContext, dataSource.Dataset.First()), axisDataExpression);
                var chartData = dataSource.Dataset.OrderBy(o => axisSelector.DynamicInvoke(o));

                // If the axis is formatted, then group

                var axisElements = chartData.Select(o => axisSelector.DynamicInvoke(o)).Select(o => $"'{String.Format($"{{0:{axisFormat}}}", o)}'").Distinct();

                // Is this a labeled data set?
                if (labels != null)
                {
                    writer.WriteAttributeString("labels", $"[{String.Join(",", axisElements)}]");
                }
                else // it is an X/Y dataset
                {
                    writer.WriteStartAttribute("axis");
                    writer.WriteString("{ type: '");

                    // First determine type
                    if (typeof(DateTime).GetTypeInfo().IsAssignableFrom(axisSelector.DynamicInvoke(chartData.First()).GetType()))
                        writer.WriteString($"time', time: {{ distribution: 'linear', unit: '{(axis.Attribute("time-unit")?.Value ?? "day")}' }}");
                    else
                        writer.WriteString("linear'");

                    if (axis.Attribute("label") != null)
                        writer.WriteString($", scaleLabel: '{ReportViewUtil.GetString(axis.Attribute("label").Value)}'");
                    writer.WriteString("}");
                    writer.WriteEndAttribute(); // axis
                }

                // Now process datasets
                List<ExpandoObject> dataSetOptions = new List<ExpandoObject>();
                var dataGroup = chartData.GroupBy(o => $"'{String.Format($"{{0:{axisFormat}}}", axisSelector.DynamicInvoke(o))}'");

                foreach (var ds in element.Elements((XNamespace)BiConstants.ComponentNamespace + "dataset"))
                {
                    var dataSelector = ReportViewUtil.CompileExpression(new RenderContext(chartContext, dataGroup.First().First()), ds.Value);

                    IDictionary<String, Object> data = new ExpandoObject();
                    data.Add("label", ReportViewUtil.GetString(ds.Attribute("label")?.Value ?? "unknown"));

                    if (ds.Attribute("backgroundColor") != null)
                        data.Add("backgroundColor", ds.Attribute("backgroundColor").Value);
                    if (ds.Attribute("borderColor") != null)
                        data.Add("borderColor", ds.Attribute("borderColor").Value);
                    if (ds.Attribute("type") != null)
                        data.Add("type", ds.Attribute("type").Value);
                    if (ds.Attribute("fill") != null)
                        data.Add("fill", Boolean.Parse(ds.Attribute("fill").Value));

                    if (axis != null) // this is an X/Y plot
                    {
                        switch (ds.Attribute("aggregate")?.Value ?? "sum")
                        {
                            case "count":
                                data.Add("data", dataGroup.Select(o => new { x = o.Key, y = o.Count(e => this.SafeInvoke(dataSelector, null, e) != null) }).ToArray());
                                break;
                            case "sum":
                                data.Add("data", dataGroup.Select(o => new { x = o.Key, y = o.Sum(e => (decimal?)this.SafeInvoke(dataSelector, 0, e)) }).ToArray());
                                break;
                            case "avg":
                                data.Add("data", dataGroup.Select(o => new { x = o.Key, y = o.Average(e => (decimal?)this.SafeInvoke(dataSelector, 0, e)) }).ToArray());
                                break;
                            case "min":
                                data.Add("data", dataGroup.Select(o => new { x = o.Key, y = o.Min(e => (decimal?)this.SafeInvoke(dataSelector, 0, e)) }).ToArray());
                                break;
                            case "max":
                                data.Add("data", dataGroup.Select(o => new { x = o.Key, y = o.Max(e => (decimal?)this.SafeInvoke(dataSelector, 0, e)) }).ToArray());
                                break;
                            case "first":
                                data.Add("data", dataGroup.Select(o => new { x = o.Key, y = this.SafeInvoke(dataSelector, null, o.First()) }).ToArray());
                                break;
                            case "last":
                                data.Add("data", dataGroup.Select(o => new { x = o.Key, y = this.SafeInvoke(dataSelector, null, o.Last()) }).ToArray());
                                break;
                        }
                    }
                    else
                    {
                        switch (ds.Attribute("aggregate")?.Value ?? "sum")
                        {
                            case "count":
                                data.Add("data", dataGroup.Select(o => o.Count(e => this.SafeInvoke(dataSelector, null, e) != null)).ToArray());
                                break;
                            case "sum":
                                data.Add("data", dataGroup.Select(o => o.Sum(e => (decimal?)this.SafeInvoke(dataSelector, 0, e))).ToArray());
                                break;
                            case "avg":
                                data.Add("data", dataGroup.Select(o => o.Average(e => (decimal?)this.SafeInvoke(dataSelector, 0, e))).ToArray());
                                break;
                            case "min":
                                data.Add("data", dataGroup.Select(o => o.Min(e => (decimal?)this.SafeInvoke(dataSelector, 0, e))).ToArray());
                                break;
                            case "max":
                                data.Add("data", dataGroup.Select(o => o.Max(e => (decimal?)this.SafeInvoke(dataSelector, 0, e))).ToArray());
                                break;
                            case "first":
                                data.Add("data", dataGroup.Select(o => this.SafeInvoke(dataSelector, null, o.First())).ToArray());
                                break;
                            case "last":
                                data.Add("data", dataGroup.Select(o => this.SafeInvoke(dataSelector, null, o.Last())).ToArray());
                                break;
                        }
                    }

                    dataSetOptions.Add(data as ExpandoObject);
                }

                // Serialize data set options
                var optionJson = JsonConvert.SerializeObject(dataSetOptions);
                writer.WriteAttributeString("data", optionJson);
                writer.WriteEndElement(); // chart
            }

        }

        /// <summary>
        /// Safe invoke
        /// </summary>
        private object SafeInvoke(Delegate invokee, object defaultValue, params object[] args)
        {
            try { return invokee.DynamicInvoke(args); }
            catch { return defaultValue; }
        }

        /// <summary>
        /// Validate that the chart settings are valid
        /// </summary>
        public bool Validate(XElement element, IRenderContext context)
        {
            var valid = !String.IsNullOrEmpty(element.Attribute("type")?.Value) && !String.IsNullOrEmpty(element.Attribute("source")?.Value);
            // TODO : Validate based on type of chart
            return valid;
        }
    }
}
