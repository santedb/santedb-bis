/*
 * Copyright (C) 2021 - 2021, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2021-8-5
 */
using Newtonsoft.Json;
using SanteDB.Core.Model.Attributes;
using System;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Documents the render formatters
    /// </summary>
    [XmlType(nameof(BiRenderFormatDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiRenderFormatDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] // Serialization class
    public class BiRenderFormatDefinition : BiDefinition
    {

        /// <summary>
        /// Gets the render format
        /// </summary>
        [XmlAttribute("extension"), JsonProperty("extension"), QueryParameter("extension")]
        public String FormatExtension { get; set; }

        /// <summary>
        /// Gets or sets the mime encoding for the formatting
        /// </summary>
        [XmlAttribute("contentType"), JsonProperty("contentType"), QueryParameter("contentType")]
        public String ContentType { get; set; }

        /// <summary>
        /// Gets or sets the rendere xml format
        /// </summary>
        [XmlAttribute("renderer"), JsonProperty("renderer")]
        public String TypeXml
        {
            get => this.Type?.AssemblyQualifiedName;
            set
            {
                if (value != null)
                    this.Type = Type.GetType(value);
                else
                    this.Type = null;
            }
        }

        /// <summary>
        /// Gets or sets the actual renderer type
        /// </summary>
        [XmlIgnore]
        public Type Type { get; set; }

    }
}
