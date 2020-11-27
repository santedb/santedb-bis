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
using SanteDB.BI.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SanteDB.BI.Components
{
    /// <summary>
    /// Represents a BI View component which can render content 
    /// </summary>
    public interface IBiViewComponent
    {
        /// <summary>
        /// Gets the name of the component which is to be rendered
        /// </summary>
        XName ComponentName { get; }

        /// <summary>
        /// Returns true if this component renderer can render the specified element
        /// </summary>
        /// <param name="source">The source renderer</param>
        /// <param name="element">The element being tested for render</param>
        /// <returns>The render object</returns>
        bool Validate(XElement element, IRenderContext context);

        /// <summary>
        /// Renders the specified view component
        /// </summary>
        /// <param name="element">The element to be rendered</param>
        /// <param name="writer">The writer to which output HTML should be appended</param>
        /// <param name="context">The current context of the report in this render</param>
        void Render(XElement element, XmlWriter writer, IRenderContext context);

    }
}
