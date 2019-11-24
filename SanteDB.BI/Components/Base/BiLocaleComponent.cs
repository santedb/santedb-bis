using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using SanteDB.BI.Rendering;
using SanteDB.Core;
using SanteDB.Core.Applets.Services;

namespace SanteDB.BI.Components.Base
{
    /// <summary>
    /// bi:locale component
    /// </summary>
    public class BiLocaleComponent : IBiViewComponent
    {
        /// <summary>
        /// Gets the component name
        /// </summary>
        public XName ComponentName => (XNamespace)BiConstants.ComponentNamespace + "locale";

        /// <summary>
        /// Render the object
        /// </summary>
        public void Render(XElement element, XmlWriter writer, IRenderContext context)
        {
            // Get the application manager
            writer.WriteString(ApplicationServiceContext.Current.GetService<IAppletManagerService>().Applets.SelectMany(o => o.Strings).Where(o => o.Language == CultureInfo.CurrentUICulture.TwoLetterISOLanguageName).SelectMany(o => o.String).FirstOrDefault(o => o.Key == element.Value)?.Value ?? element.Value);
        }

        /// <summary>
        /// Validate that this component can run
        /// </summary>
        public bool Validate(XElement element, IRenderContext context)
        {
            return !String.IsNullOrEmpty(element.Value);
        }
    }
}
