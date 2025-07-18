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
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Schema view definition
    /// </summary>
    [XmlRoot(nameof(BiSchemaViewDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlType(nameof(BiSchemaViewDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] // Serialization class
    public class BiSchemaViewDefinition : BiSchemaObjectDefinition
    {

        /// <summary>
        /// True if the view is materialized
        /// </summary>
        [XmlAttribute("materialized"), JsonProperty("materialized")]
        public bool IsMaterialized { get; set; }

        /// <summary>
        /// Gets or sets the query for this view
        /// </summary>
        [XmlArray("sql"), XmlArrayItem("add"), JsonProperty("query")]
        public List<BiSqlDefinition> Query { get; set; }
    }

}
