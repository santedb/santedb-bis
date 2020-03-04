/*
 * Based on OpenIZ, Copyright (C) 2015 - 2019 Mohawk College of Applied Arts and Technology
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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a collection of BI definitions
    /// </summary>
    [XmlType(nameof(BiDefinitionCollection), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiDefinitionCollection), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [XmlInclude(typeof(BiViewDefinition))]
    [XmlInclude(typeof(BiQueryDefinition))]
    [XmlInclude(typeof(BiParameterDefinition))]
    [XmlInclude(typeof(BiReportDefinition))]
    [XmlInclude(typeof(BiRenderFormatDefinition))]
    [XmlInclude(typeof(BiDataSourceDefinition))]
    [XmlInclude(typeof(BiPackage))]
    public class BiDefinitionCollection 
    {

        /// <summary>
        /// Collection ctor
        /// </summary>
        public BiDefinitionCollection()
        {

        }

        /// <summary>
        /// Collection ctor with objects 
        /// </summary>
        public BiDefinitionCollection(IEnumerable<BiDefinition> items) 
        {
            this.Items = new List<BiDefinition>(items);
        }

        /// <summary>
        /// Gets or sets the items part of this collection
        /// </summary>
        [XmlElement("item"), JsonProperty("item")]
        public List<BiDefinition> Items { get; set; }
    }
}
