﻿/*
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
using DynamicExpresso;
using Newtonsoft.Json;
using SanteDB.BI.Rendering;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

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
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }
            else if (element.Attribute("source") == null)
            {
                throw new InvalidOperationException("Cannot find root source");
            }
            else if (writer == null)
            {
                throw new ArgumentNullException(nameof(writer));
            }
            else if (context == null)
            {
                throw new ArgumentException(nameof(context));
            }
            else if (!(context.Root is RootRenderContext rootContext))
            {
                throw new InvalidOperationException("Invalid root context");
            }
            using (var dataSource = (context.Root as RootRenderContext).GetOrExecuteQuery(element.Attribute("source").Value))
            {
                if (!dataSource.Records.Any())
                {
                    writer.WriteElementString("strong", BiConstants.HtmlNamespace, $"{dataSource.QueryDefinition?.Name} - 0 REC");
                }
                else
                {
                    // Render the object
                    writer.WriteStartElement("chart", BiConstants.HtmlNamespace);
                    writer.WriteAttributeString("type", $"'{element.Attribute("type").Value}'");
                    writer.WriteAttributeString("legend", $"{element.Attribute("legend")?.Value ?? "false"}");

                    var title = element.Element((XNamespace)BiConstants.ComponentNamespace + "title");
                    if (title != null)
                    {
                        writer.WriteAttributeString("title", $"'{title.Value}'");
                    }

                    var chartContext = new RenderContext(context, dataSource.Records);
                    chartContext.Tags.Add("expressions", new Dictionary<String, Lambda>());
                    // Now sort the result set by the key
                    var labels = element.Element((XNamespace)BiConstants.ComponentNamespace + "labels");
                    var axis = element.Element((XNamespace)BiConstants.ComponentNamespace + "axis");
                    var axisDataExpression = (labels ?? axis).Value;
                    var axisFormat = (labels ?? axis).Attribute("format")?.Value;
                    var axisSelector = ReportViewUtil.CompileExpression(new RenderContext(chartContext, dataSource.Records.First()), axisDataExpression);
                    var chartData = dataSource.Records.OrderBy(o => axisSelector.Invoke(ReportViewUtil.ToParameterArray(o))).ToList();

                    // If the axis is formatted, then group
                    var axisElements = chartData.Select(o => axisSelector.Invoke(ReportViewUtil.ToParameterArray(o))).Select(o => $"'{String.Format($"{{0:{axisFormat}}}", o)}'").Distinct();

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
                        if (typeof(DateTime).IsAssignableFrom(axisSelector.Invoke(ReportViewUtil.ToParameterArray(chartData.First())).GetType()))
                        {
                            writer.WriteString($"time', time: {{ distribution: 'linear', unit: '{(axis.Attribute("time-unit")?.Value ?? "day")}' }}");
                        }
                        else
                        {
                            writer.WriteString("linear'");
                        }

                        if (axis.Attribute("label") != null)
                        {
                            writer.WriteString($", scaleLabel: '{ReportViewUtil.GetString(axis.Attribute("label").Value)}'");
                        }

                        writer.WriteString("}");
                        writer.WriteEndAttribute(); // axis
                    }

                    // Now process datasets
                    List<ExpandoObject> dataSetOptions = new List<ExpandoObject>();
                    var dataGroup = chartData.GroupBy(o => $"'{String.Format($"{{0:{axisFormat}}}", axisSelector.Invoke(ReportViewUtil.ToParameterArray(o)))}'");

                    foreach (var ds in element.Elements((XNamespace)BiConstants.ComponentNamespace + "dataset"))
                    {
                        try
                        {
                            var dataSelector = ReportViewUtil.CompileExpression(new RenderContext(chartContext, dataGroup.First().First()), ds.Value);

                            IDictionary<String, Object> data = new ExpandoObject();
                            data.Add("label", ReportViewUtil.GetString(ds.Attribute("label")?.Value ?? "unknown"));

                            if (ds.Attribute("backgroundColor") != null)
                            {
                                data.Add("backgroundColor", ds.Attribute("backgroundColor").Value);
                            }

                            if (ds.Attribute("borderColor") != null)
                            {
                                data.Add("borderColor", ds.Attribute("borderColor").Value);
                            }

                            if (ds.Attribute("type") != null)
                            {
                                data.Add("type", ds.Attribute("type").Value);
                            }

                            if (ds.Attribute("fill") != null)
                            {
                                data.Add("fill", Boolean.Parse(ds.Attribute("fill").Value));
                            }

                            if (axis != null) // this is an X/Y plot
                            {
                                switch (ds.Attribute("aggregate")?.Value ?? "sum")
                                {
                                    case "count":
                                        data.Add("data", dataGroup.Select(o => new { x = o.Key, y = o.Count(e => this.SafeInvoke(dataSelector, null, ReportViewUtil.ToParameterArray(e)) != null) }).ToArray());
                                        break;

                                    case "sum":
                                        data.Add("data", dataGroup.Select(o => new { x = o.Key, y = o.Sum(e => (decimal?)this.SafeInvoke(dataSelector, 0, ReportViewUtil.ToParameterArray(e))) }).ToArray());
                                        break;

                                    case "avg":
                                        data.Add("data", dataGroup.Select(o => new { x = o.Key, y = o.Average(e => (decimal?)this.SafeInvoke(dataSelector, 0, ReportViewUtil.ToParameterArray(e))) }).ToArray());
                                        break;

                                    case "min":
                                        data.Add("data", dataGroup.Select(o => new { x = o.Key, y = o.Min(e => (decimal?)this.SafeInvoke(dataSelector, 0, ReportViewUtil.ToParameterArray(e))) }).ToArray());
                                        break;

                                    case "max":
                                        data.Add("data", dataGroup.Select(o => new { x = o.Key, y = o.Max(e => (decimal?)this.SafeInvoke(dataSelector, 0, ReportViewUtil.ToParameterArray(e))) }).ToArray());
                                        break;

                                    case "first":
                                        data.Add("data", dataGroup.Select(o => new { x = o.Key, y = this.SafeInvoke(dataSelector, null, ReportViewUtil.ToParameterArray(o.First())) }).ToArray());
                                        break;

                                    case "last":
                                        data.Add("data", dataGroup.Select(o => new { x = o.Key, y = this.SafeInvoke(dataSelector, null, ReportViewUtil.ToParameterArray(o.Last())) }).ToArray());
                                        break;
                                }
                            }
                            else
                            {
                                switch (ds.Attribute("aggregate")?.Value ?? "sum")
                                {
                                    case "count":
                                        data.Add("data", dataGroup.Select(o => o.Count(e => this.SafeInvoke(dataSelector, null, ReportViewUtil.ToParameterArray(e)) != null)).ToArray());
                                        break;

                                    case "sum":
                                        data.Add("data", dataGroup.Select(o => o.Sum(e => (decimal?)this.SafeInvoke(dataSelector, 0, ReportViewUtil.ToParameterArray(e)))).ToArray());
                                        break;

                                    case "avg":
                                        data.Add("data", dataGroup.Select(o => o.Average(e => (decimal?)this.SafeInvoke(dataSelector, 0, ReportViewUtil.ToParameterArray(e)))).ToArray());
                                        break;

                                    case "min":
                                        data.Add("data", dataGroup.Select(o => o.Min(e => (decimal?)this.SafeInvoke(dataSelector, 0, ReportViewUtil.ToParameterArray(e)))).ToArray());
                                        break;

                                    case "max":
                                        data.Add("data", dataGroup.Select(o => o.Max(e => (decimal?)this.SafeInvoke(dataSelector, 0, ReportViewUtil.ToParameterArray(e)))).ToArray());
                                        break;

                                    case "first":
                                        data.Add("data", dataGroup.Select(o => this.SafeInvoke(dataSelector, null, ReportViewUtil.ToParameterArray(o.First()))).ToArray());
                                        break;

                                    case "last":
                                        data.Add("data", dataGroup.Select(o => this.SafeInvoke(dataSelector, null, ReportViewUtil.ToParameterArray(o.Last()))).ToArray());
                                        break;
                                }
                            }

                            dataSetOptions.Add(data as ExpandoObject);
                        }
                        catch
                        {
                        }
                    }
                    // Serialize data set options
                    var optionJson = JsonConvert.SerializeObject(dataSetOptions);
                    writer.WriteAttributeString("data", optionJson);
                    writer.WriteEndElement(); // chart
                }
            }
        }

        /// <summary>
        /// Safe invoke
        /// </summary>
        private object SafeInvoke(Lambda invokee, object defaultValue, params object[] args)
        {
            try { return invokee.Invoke(args); }
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