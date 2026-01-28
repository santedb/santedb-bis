/*
 * Copyright (C) 2021 - 2026, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
using SanteDB.BI.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace SanteDB.BI.Components.Base
{
    /// <summary>
    /// Repeating control for data, changes the current data context
    /// </summary>
    public class BiRepeatComponent : IBiViewComponent
    {
        /// <summary>
        /// Gets the component name
        /// </summary>
        public XName ComponentName => (XNamespace)BiConstants.ComponentNamespace + "repeat";

        /// <summary>
        /// Render the specified element
        /// </summary>
        public void Render(XElement element, XmlWriter writer, IRenderContext context)
        {
            // Run dataset and start context
            if (element.Attribute("source") != null)
            {
                using (var dataSource = (context.Root as RootRenderContext).GetOrExecuteQuery(element.Attribute("source").Value))
                {
                    var thisContext = new RenderContext(context, dataSource.Records);

                    // Add watches and expressions
                    thisContext.Tags.Add("watches", new Dictionary<String, Object>());
                    thisContext.Tags.Add("expressions", new Dictionary<String, Lambda>());

                    writer.WriteComment($"start repeat : {(dataSource.QueryDefinition?.Id ?? "adhoc")}");

                    foreach (var itm in dataSource.Records)
                    {
                        foreach (var el in element.Nodes())
                        {
                            ReportViewUtil.Write(writer, el, new RenderContext(thisContext, itm));
                        }
                    }

                    writer.WriteComment($"end repeat : {(dataSource.QueryDefinition?.Id ?? "adhoc")} ");
                }
            }
            else // Execute expression
            {
                var value = ReportViewUtil.GetValue(context, element.Attribute("expression").Value);
                if (value is IEnumerable enumerable)
                {
                    foreach (var itm in enumerable)
                    {
                        var subCtx = new RenderContext(context, itm);
                        foreach (var el in element.Nodes())
                        {
                            ReportViewUtil.Write(writer, el, subCtx);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validate that this component is correctly represented prior to calling render
        /// </summary>
        public bool Validate(XElement element, IRenderContext context)
        {
            return element.HasAttributes && (element.Attribute("source") != null &&
                (context.Root as RootRenderContext).HasDataSource(element.Attribute("source").Value) ||
                element.Attribute("expression") != null);
        }
    }
}