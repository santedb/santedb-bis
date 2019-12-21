/*
 * Copyright 2015-2019 Mohawk College of Applied Arts and Technology
 * Copyright 2019-2019 SanteSuite Contributors (See NOTICE)
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
 * User: Justin Fyfe
 * Date: 2019-12-6
 */
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
            writer.WriteString(ReportViewUtil.GetString(element.Value));
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
