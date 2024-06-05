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
 * User: fyfej
 * Date: 2023-6-21
 */
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

        /// <summary>
        /// Data flow audit parameter name
        /// </summary>
        public const string AuditDataFlowParameterName = "$audit";


        /// <summary>
        /// Data flow principal name parameter name
        /// </summary>
        public const string PrincipalDataFlowParameterName = "$principal";

        /// <summary>
        /// Data flow principal name parameter name
        /// </summary>
        public const string StartTimeDataFlowParameterName = "$startTime";

        /// <summary>
        /// The target mart
        /// </summary>
        public const string DataMartDataFlowParameterName = "$targetMart";
    }
}
