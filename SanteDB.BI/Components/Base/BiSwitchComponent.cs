﻿using System;
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
    /// bi:switch report component allows switch/case statements in reports
    /// </summary>
    public class BiSwitchComponent : IBiViewComponent
    {
        /// <summary>
        /// Get the component name
        /// </summary>
        public XName ComponentName => (XNamespace)BiConstants.ComponentNamespace + "switch";

        /// <summary>
        /// Render the component
        /// </summary>
        public void Render(XElement element, XmlWriter writer, IRenderContext context)
        {
            var fieldOrExpression = element.Attribute("value").Value;
            var fieldValue = ReportViewUtil.GetValue(context, fieldOrExpression);

            foreach(var whenClauseElement in element.Elements((XNamespace)BiConstants.ComponentNamespace + "when"))
            {
                var op = whenClauseElement.Attribute("op").Value;
                var isNot = whenClauseElement.Attribute("not")?.Value?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;
                var xmlValue = whenClauseElement.Attribute("value").Value;
                object value = null;
                
                // Convert for comparison
                if (xmlValue == "null") // Null 
                    value = null;
                else if (!MapUtil.TryConvert(xmlValue, fieldValue?.GetType(), out value))
                    throw new InvalidOperationException($"Switch statement from {fieldValue} to {xmlValue}");

                // Operator
                bool opSuccess = false;

                try
                {
                    switch (op)
                    {
                        case "lt":
                            opSuccess = ((IComparable)fieldValue)?.CompareTo(value) < 0;
                            break;
                        case "lte":
                            opSuccess = ((IComparable)fieldValue)?.CompareTo(value) <= 0;
                            break;
                        case "gt":
                            opSuccess = ((IComparable)fieldValue)?.CompareTo(value) > 0;
                            break;
                        case "gte":
                            opSuccess = ((IComparable)fieldValue)?.CompareTo(value) >= 0;
                            break;
                        case "eq":
                            opSuccess = fieldValue?.Equals(value) == true || fieldValue == value;
                            break;
                    }

                    if (isNot) opSuccess = !opSuccess;
                }
                catch(InvalidCastException)
                {
                    throw new InvalidOperationException($"Field value type {fieldValue.GetType().Name} is not IComparible and cannot be used in a switch");
                }

                if (opSuccess)
                {
                    foreach (var itm in whenClauseElement.Elements())
                        ReportViewUtil.Write(writer, itm, context);
                    return;
                }
            }

            // Default condition?
            var defaultOption = element.Element((XNamespace)BiConstants.ComponentNamespace + "default");
            if(defaultOption != null)
                foreach (var itm in defaultOption.Elements())
                    ReportViewUtil.Write(writer, itm, context);
            
        }

        /// <summary>
        /// Validate the component
        /// </summary>
        public bool Validate(XElement element, IRenderContext context)
        {
            // Must have at least one when case or value and must have inner text
            return !String.IsNullOrEmpty(element.Attribute("value")?.Value) &&
                element.Element((XNamespace)BiConstants.ComponentNamespace + "when") != null;
        }
    }
}