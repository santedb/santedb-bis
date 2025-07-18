﻿/*
 * Copyright (C) 2021 - 2025, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{


    /// <summary>
    /// BI metadata
    /// </summary>
    [XmlType(nameof(BiMetadata), Namespace = BiConstants.XmlNamespace), JsonObject]
    [ExcludeFromCodeCoverage] // Serialization class
    public class BiMetadata
    {

        /// <summary>
        /// BI Metadata ctor
        /// </summary>
        public BiMetadata()
        {
            this.Demands = new List<string>();
            this.Tags = new List<BiMetadataTag>();
        }

        /// <summary>
        /// Gets the list of tags
        /// </summary>
        [XmlElement("tag"), JsonProperty("tag")]
        public List<BiMetadataTag> Tags { get; set; }

        /// <summary>
        /// Gets or sets the version of this object
        /// </summary>
        [XmlAttribute("version"), JsonProperty("version")]
        public String Version { get; set; }

        /// <summary>
        /// Gets or sets the authors
        /// </summary>
        [XmlArray("authors"), XmlArrayItem("add"), JsonProperty("authors")]
        public List<string> Authors { get; set; }

        /// <summary>
        /// Gets or sets the documentation for this object
        /// </summary>
        [XmlElement("annotation"), JsonProperty("doc")]
        public BiAnnotation Annotation { get; set; }

        /// <summary>
        /// Gets or sets the list of demand policies
        /// </summary>
        [XmlArray("policies"), XmlArrayItem("demand"), JsonProperty("policies")]
        public List<string> Demands { get; set; }

        /// <summary>
        /// True if the report is public
        /// </summary>
        [XmlElement("public"), JsonProperty("public")]
        public bool IsPublic { get; set; }

        /// <summary>
        /// Last modified by
        /// </summary>
        [XmlElement("lastModified"), JsonProperty("lastModified")]
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Last modified by
        /// </summary>
        [XmlElement("lastModifiedProvenance"), JsonProperty("lastModifiedProvenance")]
        public Guid LastModifiedBy { get; set; }

    }
}