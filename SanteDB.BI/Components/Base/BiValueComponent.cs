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
 */
using SanteDB.BI.Rendering;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace SanteDB.BI.Components.Base
{
    /// <summary>
    /// Binding to a value
    /// </summary>
    public class BiValueComponent : IBiViewComponent
    {

        /// <summary>
        /// Gets the component name
        /// </summary>
        public XName ComponentName => (XNamespace)BiConstants.ComponentNamespace + "value";

        /// <summary>
        /// Render the value
        /// </summary>
        public void Render(XElement element, XmlWriter writer, IRenderContext context)
        {

            var fieldOrExpression = element.Value;
            var value = ReportViewUtil.GetValue(context, fieldOrExpression);
            if (value == null && !String.IsNullOrEmpty(element.Attribute("default")?.Value))
            {
                value = ReportViewUtil.GetValue(context, element.Attribute("default")?.Value);
            }

            // Is the required value a change?
            if (element.Attribute("when")?.Value == "changed")
            {
                IDictionary<String, object> watches = context.Parent.Tags["watches"] as IDictionary<String, Object>;

                if (!watches.TryGetValue(fieldOrExpression, out var exisitngValue))
                {
                    watches.Add(fieldOrExpression, value);
                }
                else if (exisitngValue?.Equals(value) == true)
                {
                    return;
                }
            }

            // Is there a format?
            var format = element.Attribute("format")?.Value;
            if (value != null)
            {
                if (!String.IsNullOrEmpty(format))
                {
                    if (format.Contains("{0}"))
                    {
                        writer.WriteString(String.Format(format, value));
                    }
                    else
                    {
                        writer.WriteString(String.Format($"{{0:{format}}}", value));
                    }
                }
                else
                {
                    writer.WriteString(value.ToString());
                }
            }

        }

        /// <summary>
        /// Validate the value element
        /// </summary>
        public bool Validate(XElement element, IRenderContext context)
        {
            return !String.IsNullOrEmpty(element.Value);
        }
    }
}
