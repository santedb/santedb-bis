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
using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a tag on Bi metadata
    /// </summary>
    [XmlType(nameof(BiMetadataTag), Namespace = BiConstants.XmlNamespace), JsonObject]
    public class BiMetadataTag
    {
        /// <summary>
        /// Serialization constructor
        /// </summary>
        public BiMetadataTag()
        {

        }

        /// <summary>
        /// Creates a BI metadata tag with the specified name and value
        /// </summary>
        /// <param name="name">The name of the tag</param>
        /// <param name="value">The value of the tag</param>
        public BiMetadataTag(String name, String value)
        {
            this.Name = name;
            this.Value = value;
        }
        /// <summary>
        /// Gets the name of the tag
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets the value of the tag
        /// </summary>
        [XmlAttribute("value"), JsonProperty("value")]
        public String Value { get; set; }

    }
}
