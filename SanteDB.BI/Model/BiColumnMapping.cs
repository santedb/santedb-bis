/*
 * Copyright (C) 2021 - 2023, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-3-10
 */
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{

    /// <summary>
    /// A column mapping between a souce and destination column
    /// </summary>
    [XmlType(nameof(BiColumnMapping), Namespace = BiConstants.XmlNamespace)]
    [ExcludeFromCodeCoverage] // Serialization class
    public class BiColumnMapping
    {
        /// <summary>
        /// Gets or sets the source of the mapping
        /// </summary>
        [XmlElement("source"), JsonProperty("source")]
        public BiColumnMappingSource Source { get; set; }

        /// <summary>
        /// Gets or sets the target of the mapping
        /// </summary>
        [XmlElement("target"), JsonProperty("target")]
        public String Target { get; set; }
    }

    /// <summary>
    /// Schema definition for an input column with an optional transform
    /// </summary>
    [XmlType(nameof(BiColumnMappingSource), Namespace = BiConstants.XmlNamespace)]
    [ExcludeFromCodeCoverage] // Serialization class
    public class BiColumnMappingSource : BiSchemaColumnDefinition
    {

        /// <summary>
        /// The column transformation expression
        /// </summary>
        [XmlElement("transform", typeof(String))]
        [XmlElement("lookup", typeof(BiColumnMappingTransformJoin))]
        public Object TransformExpression { get; set; }

    }

    /// <summary>
    /// Indicates the source transform from another column
    /// </summary>
    [XmlType(nameof(BiColumnMappingTransformJoin), Namespace = BiConstants.XmlNamespace)]
    [ExcludeFromCodeCoverage] // Serialization class
    public class BiColumnMappingTransformJoin
    {

        /// <summary>
        /// Gets or sets the input stream step reference where the join should be sourced
        /// </summary>
        [XmlElement("input"), JsonProperty("input")]
        public BiDataFlowStreamStep Input { get; set; }

        /// <summary>
        /// Gets or sets the join expression for the join operation
        /// </summary>
        [XmlElement("join"), JsonProperty("join")]
        public String Join { get; set; }
    }
}