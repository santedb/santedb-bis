/*
 * Copyright (C) 2019 - 2020, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
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
 * Date: 2019-11-27
 */
using ExpressionEvaluator;
using SanteDB.BI.Exceptions;
using SanteDB.BI.Rendering;
using SanteDB.Core;
using SanteDB.Core.Applets.Services;
using SanteDB.Core.Interfaces;
using SanteDB.Core.Security;
using SanteDB.Core.Security.Claims;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SanteDB.BI.Components
{
    /// <summary>
    /// Report view utility
    /// </summary>
    internal static class ReportViewUtil
    {

        // Expression regex
        private static Regex m_exprRegex = new Regex(@"^[\w_\s]+$");

        // Component cache
        private static Dictionary<XName, IBiViewComponent> m_componentCache;

        /// <summary>
        /// Static CTOR
        /// </summary>
        static ReportViewUtil()
        {
            m_componentCache = ApplicationServiceContext.Current.GetService<IServiceManager>()
                .GetAllTypes()
                .Where(o => typeof(IBiViewComponent).GetTypeInfo().IsAssignableFrom(o.GetTypeInfo()) && !o.GetTypeInfo().IsInterface && !o.GetTypeInfo().IsAbstract)
                .Select(o => Activator.CreateInstance(o) as IBiViewComponent)
                .ToDictionary(o => o.ComponentName, o => o);
        }

        /// <summary>
        /// Get localized string
        /// </summary>
        public static string GetString(string key)
        {
            // TODO: Replace with ILocalizationService call
            var slnMgr = ApplicationServiceContext.Current.GetService<IAppletSolutionManagerService>();
            if(slnMgr?.Solutions?.Any() == true)
            {
                String retVal = null;
                foreach(var solution in slnMgr.Solutions)
                {
                    
                    retVal = slnMgr.GetApplets(solution.Meta.Id).SelectMany(o => o.Strings)
                        .Where(o => o.Language == (AuthenticationContext.Current.Principal.GetClaimValue(SanteDBClaimTypes.Language) ?? CultureInfo.CurrentUICulture.TwoLetterISOLanguageName))
                        .SelectMany(o => o.String)
                        .Where(o => o.Key == key)
                        .OrderByDescending(o => o.Priority)
                        .FirstOrDefault()?.Value;
                    if (retVal != null) break;
                }
                return retVal ?? key;
            }
            else 
                return ApplicationServiceContext.Current.GetService<IAppletManagerService>().Applets
                    .SelectMany(o => o.Strings)
                    .Where(o => o.Language == (AuthenticationContext.Current.Principal.GetClaimValue(SanteDBClaimTypes.Language) ?? CultureInfo.CurrentUICulture.TwoLetterISOLanguageName))
                    .SelectMany(o => o.String)
                    .Where(o => o.Key == key)
                    .OrderByDescending(o=>o.Priority)
                    .FirstOrDefault()?.Value ?? key;
        }

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
            if (m_exprRegex.IsMatch(field))
            {
                var scopedExpando = (context.ScopedObject as IDictionary<String, Object>);
                var currentContext = context;
                while ((scopedExpando == null || !scopedExpando.TryGetValue(field, out value)) && currentContext != null)
                {
                    currentContext = currentContext.Parent;
                    scopedExpando = currentContext.ScopedObject as IDictionary<String, Object>;
                }
                return value;
            }
            else // complex expression
            {
                Delegate evaluator = context.CompileExpression(field);
                return evaluator.DynamicInvoke(context.ScopedObject);
            }
        }

        /// <summary>
        /// Compile the specified expression
        /// </summary>
        /// <param name="me"></param>
        /// <param name="fieldOrExpression"></param>
        /// <returns></returns>
        public static Delegate CompileExpression(this IRenderContext me, String fieldOrExpression)
        {
            IDictionary<String, Delegate> exprs = me.Parent?.Tags["expressions"] as IDictionary<String, Delegate>;

            Delegate evaluator = null;
            if (exprs?.TryGetValue(fieldOrExpression, out evaluator) != true)
            {

                var expression = new CompiledExpression(fieldOrExpression);
                expression.TypeRegistry = new TypeRegistry();
                expression.TypeRegistry.RegisterDefaultTypes();
                expression.TypeRegistry.RegisterType<Guid>();
                expression.TypeRegistry.RegisterType<DateTimeOffset>();
                expression.TypeRegistry.RegisterType<TimeSpan>();

                evaluator = expression.ScopeCompile<ExpandoObject>();

                exprs?.Add(fieldOrExpression, evaluator);

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
                return null;
            return retVal;

        }

        /// <summary>
        /// Write the specified object to the screen
        /// </summary>
        internal static void Write(XmlWriter writer, XElement el, IRenderContext context)
        {
            // TODO get helper here
            IBiViewComponent component = ReportViewUtil.GetViewComponent(el.Name);

            if (component == null)
            {
                writer.WriteComment($"WARNING: No component for {el.Name} is registered");
            }
            else if (component.Validate(el, context))
                component.Render(el, writer, context);
            else
            {
#if DEBUG
                throw new ViewValidationException(el, $"Component {component?.ComponentName} failed validation");
#else
                        writer.WriteStartElement("em", BiConstants.HtmlNamespace);
                        writer.WriteAttributeString("style", "color: #f00");
                        StringBuilder path = new StringBuilder($"/{el.Name}");
                        var p = el.Parent;
                        while (p != el.Document.Root && p != null) {
                            path.Insert(0, $"/{p.Name}");
                            p = p.Parent;
                        }
                        writer.WriteString($"Component {component.ComponentName} failed validation at {path}");
                        writer.WriteEndElement(); // em
#endif

            }
        }
    }
}
