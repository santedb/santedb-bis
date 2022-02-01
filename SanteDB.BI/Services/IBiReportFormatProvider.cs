/*
 * Copyright (C) 2021 - 2022, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2021-8-27
 */
using SanteDB.BI.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace SanteDB.BI.Services
{
    /// <summary>
    /// Represents a view renderer which can render a particular view given a particular context
    /// </summary>
    public interface IBiReportFormatProvider
    {
        /// <summary>
        /// Render the specified report according to the format
        /// </summary>
        /// <param name="parameters">The parameters used to populate the report</param>
        /// <param name="reportDefinition">The report that should be rendered</param>
        /// <param name="viewName">The name of the view to berendered</param>
        /// <returns>The rendered output stream</returns>
        Stream Render(BiReportDefinition reportDefinition, String viewName, IDictionary<String, Object> parameters);
    }
}