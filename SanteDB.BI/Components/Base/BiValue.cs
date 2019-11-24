using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using ExpressionEvaluator;
using SanteDB.BI.Rendering;

namespace SanteDB.BI.Components.Base
{
    /// <summary>
    /// Binding to a value
    /// </summary>
    public class BiValue : IBiViewComponent
    {

        // Expression is being represented
        private Regex m_exprRegex = new Regex(@"^[\w_\s]+$");

        /// <summary>
        /// Gets the component name
        /// </summary>
        public XName ComponentName => (XNamespace)BiConstants.ComponentNamespace + "value";

        /// <summary>
        /// Render the value
        /// </summary>
        public void Render(XElement element, XmlWriter writer, IRenderContext context)
        {
            // First, find the current data context
            var field = element.Value;
            object value = null;
            IDictionary<String, object> watches = context.Parent.Tags["watches"] as IDictionary<String, Object>;
            IDictionary<String, Delegate> exprs = context.Parent.Tags["expressions"] as IDictionary<String, Delegate>;

            // Is there an expression?
            if (this.m_exprRegex.IsMatch(field))
            {
                var scopedExpando = (context.ScopedObject as IDictionary<String, Object>);
                var currentContext = context;
                while ((scopedExpando == null || !scopedExpando.TryGetValue(field, out value)) && currentContext != null)
                {
                    currentContext = currentContext.Parent;
                    scopedExpando = currentContext.ScopedObject as IDictionary<String, Object>;
                }
            }
            else // complex expression
            {
                Delegate evaluator = null;

                if (!exprs.TryGetValue(field, out evaluator))
                {

                    var expression = new CompiledExpression(field);
                    expression.TypeRegistry = new TypeRegistry();
                    expression.TypeRegistry.RegisterDefaultTypes();
                    expression.TypeRegistry.RegisterType<Guid>();
                    expression.TypeRegistry.RegisterType<DateTimeOffset>();
                    expression.TypeRegistry.RegisterType<TimeSpan>();

                    evaluator = expression.ScopeCompile<ExpandoObject>();

                    exprs.Add(field, evaluator);

                }

                value = evaluator.DynamicInvoke(context.ScopedObject);
            }

            // Is the required value a change?
            if (element.Attribute("when").Value == "changed")
            {
                if (!watches.TryGetValue(field, out var exisitngValue))
                    watches.Add(field, value);
                else if (exisitngValue?.Equals(value) == true)
                    return;
            }

            // Is there a format?
            if (!String.IsNullOrEmpty(element.Attribute("format").Value))
                writer.WriteString(String.Format($"{{0:{element.Attribute("format").Value}}}", value));
            else
                writer.WriteString(value.ToString());
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
