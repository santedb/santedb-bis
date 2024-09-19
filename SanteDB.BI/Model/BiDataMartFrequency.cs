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
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Datamart refresh frequency
    /// </summary>
    [XmlType(nameof(BiDataMartFrequency), Namespace = BiConstants.XmlNamespace)]
    public enum BiDataMartFrequency
    {
        /// <summary>
        /// At most, should be refreshed daily
        /// </summary>
        [XmlEnum("daily")]
        Daily,
        /// <summary>
        /// At most, should be refreshed weekly
        /// </summary>
        [XmlEnum("weekly")]
        Weekly,
        /// <summary>
        /// At most, should be refreshed monthly
        /// </summary>
        [XmlEnum("monthly")]
        Monthly

    }
}