﻿using DynamicExpresso;
using SanteDB.BI.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SanteDB.BI.Components.Data
{
    /// <summary>
    /// Data table component which renders the entire dataset as a table
    /// </summary>
    public class BiDataTableComponent : IBiViewComponent
    {
        /// <summary>
        /// Gets the component name
        /// </summary>
        public XName ComponentName => (XNamespace)BiConstants.ComponentNamespace + "dataTable";

        /// <summary>
        /// Writer header row
        /// </summary>
        private void WriteHeaderRow(XmlWriter writer, dynamic itm, IRenderContext context, params XElement[] fieldTemplates)
        {
            writer.WriteStartElement("thead", BiConstants.HtmlNamespace);
            if (fieldTemplates.Any()) // user has custom fields specified
            {
                var subFieldStack = new Queue<List<XElement>>();
                subFieldStack.Enqueue(new List<XElement>(fieldTemplates));

                // Emit the fields
                while (subFieldStack.Any())
                {
                    writer.WriteStartElement("tr", BiConstants.HtmlNamespace);
                    var subFieldData = subFieldStack.Dequeue();
                    foreach (var fld in subFieldData)
                    {
                        var header = fld.Element((XNamespace)BiConstants.ComponentNamespace + "header");
                        if (header == null)
                        {
                            throw new InvalidOperationException("Field is missing a <bi:header> element");
                        }
                        writer.WriteStartElement("th", BiConstants.HtmlNamespace);

                        var subFields = fld.Elements((XNamespace)BiConstants.ComponentNamespace + "column");
                        if (subFields.Any())
                        {
                            writer.WriteAttributeString("colspan", subFields.Count().ToString());
                            if (subFieldStack.Any())
                                subFieldStack.Peek().AddRange(subFields);
                            else
                                subFieldStack.Enqueue(subFields.ToList());
                        }
                        else
                        {
                            writer.WriteAttributeString("rowspan", $"{subFieldStack.Count + 1}");
                        }

                        // Write out header elements
                        foreach (var el in header.Nodes())
                            ReportViewUtil.Write(writer, el, new RenderContext(context, itm));
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }
            }
            else if (itm is IDictionary<String, Object> dict) // emit all fields
            {
                writer.WriteStartElement("tr", BiConstants.HtmlNamespace);

                foreach (var col in dict)
                {
                    writer.WriteElementString("th", BiConstants.HtmlNamespace, col.Key);
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Write row data
        /// </summary>
        private void WriteDataRow(XmlWriter writer, dynamic itm, IRenderContext context, params XElement[] fieldTemplates)
        {
            writer.WriteStartElement("tr", BiConstants.HtmlNamespace);
            if (fieldTemplates.Any()) // user has custom fields specified
            {
                // Flatten field templates
                foreach (var fld in fieldTemplates)
                {
                    this.WriteDataCell(writer, fld, itm, context);
                }
            }
            else if (itm is IDictionary<String, Object> dict) // emit all fields
            {
                foreach (var col in dict)
                {
                    writer.WriteElementString("td", BiConstants.HtmlNamespace, col.Value?.ToString());
                }
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Write data cells
        /// </summary>
        private void WriteDataCell(XmlWriter writer, XElement fieldElement, dynamic itm, IRenderContext context)
        {
            // Sub-items?
            var subFields = fieldElement.Elements((XNamespace)BiConstants.ComponentNamespace + "column");
            if (subFields.Any())
            {
                foreach (var sf in subFields)
                {
                    WriteDataCell(writer, sf, itm, context);
                }
            }
            else
            {
                var cell = fieldElement.Element((XNamespace)BiConstants.ComponentNamespace + "cell");
                if (cell == null)
                {
                    throw new InvalidOperationException("Field is missing a <bi:cell> element");
                }
                writer.WriteStartElement("td", BiConstants.HtmlNamespace);
                foreach (var el in cell.Elements())
                    ReportViewUtil.Write(writer, el, new RenderContext(context, itm));
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Render the data-table element
        /// </summary>
        public void Render(XElement element, XmlWriter writer, IRenderContext context)
        {
            var columnList = element.Elements((XNamespace)BiConstants.ComponentNamespace + "column").ToArray();

            writer.WriteStartElement("table", BiConstants.HtmlNamespace);
            if (element.Attribute("source") != null)
            {
                // Render from source
                var dataSource = (context.Root as RootRenderContext).GetOrExecuteQuery(element.Attribute("source").Value);
                var thisContext = new RenderContext(context, dataSource.Dataset);

                // Add watches and expressions
                thisContext.Tags.Add("watches", new Dictionary<String, Object>());
                thisContext.Tags.Add("expressions", new Dictionary<String, Lambda>());

                writer.WriteComment($"start dataTable : {(dataSource.QueryDefinition?.Id ?? "adhoc")}");

                var sn = 0;
                foreach (var itm in dataSource.Dataset)
                {
                    if (sn++ == 0)
                    {
                        this.WriteHeaderRow(writer, itm, thisContext, columnList);
                        writer.WriteStartElement("tbody", BiConstants.HtmlNamespace);
                    }
                    this.WriteDataRow(writer, itm, thisContext, columnList);
                }
                writer.WriteEndElement();

                writer.WriteComment($"end dataTable : {(dataSource.QueryDefinition?.Id ?? "adhoc")} ");
            }
            else
            {
                var value = ReportViewUtil.GetValue(context, element.Attribute("expression").Value);

                if (value is IEnumerable enumerable)
                {
                    var thisContext = new RenderContext(context, enumerable);
                    var sn = 0;
                    foreach (var itm in enumerable)
                    {
                        if (sn++ == 0)
                        {
                            this.WriteHeaderRow(writer, itm, thisContext, columnList);
                            writer.WriteStartElement("tbody", BiConstants.HtmlNamespace);
                        }
                        this.WriteDataRow(writer, itm, thisContext, columnList);
                    }
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement(); // table
        }

        /// <summary>
        /// Validate the element
        /// </summary>
        public bool Validate(XElement element, IRenderContext context)
        {
            return element.HasAttributes && (element.Attribute("source") != null &&
                (context.Root as RootRenderContext).HasDataSource(element.Attribute("source").Value) ||
                element.Attribute("expression") != null);
        }
    }
}