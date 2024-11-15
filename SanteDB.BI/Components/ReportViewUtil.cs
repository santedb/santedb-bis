/*
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
using DynamicExpresso;
using SanteDB.BI.Exceptions;
using SanteDB.BI.Rendering;
using SanteDB.Core;
using SanteDB.Core.i18n;
using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace SanteDB.BI.Components
{
    /// <summary>
    /// Report view utility
    /// </summary>
    internal static partial class ReportViewUtil
    {
        // Helpers
        private static BiExpressionHelpers m_helpers = new BiExpressionHelpers();

        // Expression regex
        private static Regex m_exprRegex = new Regex(@"^[\w_\s]+$");

        // Component cache
        private static Dictionary<XName, IBiViewComponent> m_componentCache;

        // Localization service ref
        private static ILocalizationService m_localeService = ApplicationServiceContext.Current.GetService<ILocalizationService>();

        /// <summary>
        /// Convert to parameter array
        /// </summary>
        public static object[] ToParameterArray(dynamic context, DynamicExpresso.Lambda expression)
        {
            if(context is IDictionary<String, object> dictValues)
            {
                return expression.DeclaredParameters.Select(o => dictValues.TryGetValue(o.Name, out var rv) ? rv : null).ToArray();
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(context), String.Format(ErrorMessages.ARGUMENT_INCOMPATIBLE_TYPE, typeof(IDictionary<String, Object>), context.GetType()));
            }
        }

        /// <summary>
        /// Static CTOR
        /// </summary>
        static ReportViewUtil()
        {
            m_componentCache = ApplicationServiceContext.Current.GetService<IServiceManager>()
                .GetAllTypes()
                .Where(o => typeof(IBiViewComponent).IsAssignableFrom(o) && !o.IsInterface && !o.IsAbstract)
                .Select(o => Activator.CreateInstance(o) as IBiViewComponent)
                .ToDictionary(o => o.ComponentName, o => o);
        }

        /// <summary>
        /// Get localized string
        /// </summary>
        public static string GetString(string key) => m_localeService.GetString(key);

        /// <summary>
        /// Gets the value of the specified context
        /// </summary>
        /// <param name="context">The context from which the value/expression should be extracted</param>
        /// <param name="expressionText">The expression to evaluate</param>
        /// <returns>The extracted value</returns>
        public static object GetValue(IRenderContext context, string expressionText)
        {
            // First, find the current data context
            var field = expressionText;
            object value = null;

            // Is there an expression?
            if (field.Equals("."))
            {
                return context.ScopedObject.ToString();
            }
            else if (m_exprRegex.IsMatch(field))
            {
                var scopedExpando = context.ScopedObject as IDictionary<String, Object>;
                var currentContext = context;
                while ((scopedExpando == null || !scopedExpando.TryGetValue(field, out value)) && currentContext != null)
                {
                    currentContext = currentContext.Parent;
                    scopedExpando = currentContext?.ScopedObject as IDictionary<String, Object>;
                }
                return value;
            }
            else // complex expression
            {
                var fld = context.CompileExpression(field);
                return fld.Invoke(ToParameterArray(context.ScopedObject, fld));
            }
        }

        /// <summary>
        /// Compile the specified expression
        /// </summary>
        /// <param name="me"></param>
        /// <param name="fieldOrExpression"></param>
        /// <returns></returns>
        public static Lambda CompileExpression(this IRenderContext me, String fieldOrExpression)
        {
            IDictionary<String, Lambda> exprs = me.Parent?.Tags["expressions"] as IDictionary<String, Lambda>;

            Lambda evaluator = null;
            if (exprs?.TryGetValue(fieldOrExpression, out evaluator) != true)
            {
                var interpretor = new Interpreter(InterpreterOptions.Default)
                    .Reference(typeof(Guid))
                    .Reference(typeof(DateTimeOffset))
                    .Reference(typeof(TimeSpan))
                    .SetVariable("BiUtil", m_helpers)
                    .SetFunction("now", (Func<DateTime>)(() => DateTime.Now));

                evaluator = interpretor.Parse(fieldOrExpression,
                    (me.ScopedObject as IDictionary<String, Object>).Select(o => new Parameter(o.Key, o.Value?.GetType() ?? typeof(Object))).ToArray());
            }

            return evaluator;
        }

        /// <summary>
        /// Get the view component for the specified object
        /// </summary>
        internal static IBiViewComponent GetViewComponent(XName elementName)
        {
            IBiViewComponent retVal = null;
            if (!m_componentCache.TryGetValue(elementName, out retVal) && !m_componentCache.TryGetValue(elementName.Namespace + "any", out retVal))
            {
                return null;
            }

            return retVal;
        }

        /// <summary>
        /// Write the output data to the stream
        /// </summary>
        internal static void Write(Stream writerStream, XNode formatNode, IRenderContext context)
        {
            using (var xw = XmlWriter.Create(writerStream, new XmlWriterSettings()
            {
                CloseOutput = false,
                Indent = true,
                NewLineOnAttributes = true
            }))
            {
                Write(xw, formatNode, context);
            }
        }

        /// <summary>
        /// Write the specified object to the screen
        /// </summary>
        internal static void Write(XmlWriter writer, XNode nd, IRenderContext context)
        {
            if (nd is XElement el)
            {
                // TODO get helper here
                IBiViewComponent component = ReportViewUtil.GetViewComponent(el.Name);

                if (component == null)
                {
                    writer.WriteComment($"WARNING: No component for {el.Name} is registered");
                }
                else if (component.Validate(el, context))
                {
                    component.Render(el, writer, context);
                }
                else
                {
#if DEBUG
                    throw new ViewValidationException(el, $"Component {component?.ComponentName} failed validation");
#else
                writer.WriteStartElement("em", BiConstants.HtmlNamespace);
                writer.WriteAttributeString("style", "color: #f00");
                StringBuilder path = new StringBuilder($"/{el.Name}");
                var p = el.Parent;
                while (p != el.Document.Root && p != null)
                {
                    path.Insert(0, $"/{p.Name}");
                    p = p.Parent;
                }
                writer.WriteString($"Component {component.ComponentName} failed validation at {path}");
                writer.WriteEndElement(); // em
#endif
                }
            }
            else if (nd is XText xt)
            {
                writer.WriteString(xt.Value.Trim());
            }
        }
    }
}