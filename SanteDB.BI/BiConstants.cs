using SanteDB.BI.Services;
using SanteDB.BI.Services.Impl;
using SanteDB.Core;
using SanteDB.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.BI
{
    /// <summary>
    /// Represents business intel constants
    /// </summary>
    public static class BiConstants
    {
        /// <summary>
        /// Represetns the XML namespace for the BI project
        /// </summary>
        public const string XmlNamespace = "http://santedb.org/bi";

        /// <summary>
        /// Represents the HtmlNAmespace for the BI project
        /// </summary>
        public const string HtmlNamespace = "http://www.w3.org/1999/xhtml";

        /// <summary>
        /// XML View component namespace
        /// </summary>
        public const string ComponentNamespace = "http://santedb.org/bi/view";
    }
}
