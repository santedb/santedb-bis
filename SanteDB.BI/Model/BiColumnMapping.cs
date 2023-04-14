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
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Drawing.Charts;
using Newtonsoft.Json;
using SanteDB.Core.BusinessRules;
using SanteDB.Core.i18n;
using SixLabors.Fonts.Unicode;
using System;
using System.Collections;
using System.Collections.Generic;
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
        public BiSchemaObjectReference Target { get; set; }

        /// <summary>
        /// Validate the mapping 
        /// </summary>
        internal IEnumerable<DetectedIssue> Validate()
        {
            if (this.Target == null)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, $"bi.mart.flow.step[$].map.target.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(Target)), Guid.Empty);
            }
            else
            {
                foreach(var itm in this.Target.Validate())
                {
                    yield return itm;
                }
            }
            if (this.Source == null)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, $"bi.mart.flow.step[$].map.source.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(Source)), Guid.Empty);
            }
            else
            {
                foreach(var itm in this.Source.Validate())
                {
                    yield return itm;
                }
            }
        }
    }

    /// <summary>
    /// Schema definition for an input column with an optional transform
    /// </summary>
    [XmlType(nameof(BiColumnMappingSource), Namespace = BiConstants.XmlNamespace)]
    [ExcludeFromCodeCoverage] // Serialization class
    public class BiColumnMappingSource : BiSchemaObjectReference
    {

        /// <summary>
        /// The column transformation expression
        /// </summary>
        [XmlElement("transform", typeof(String))]
        [XmlElement("lookup", typeof(BiColumnMappingTransformJoin))]
        public Object TransformExpression { get; set; }

        internal override IEnumerable<DetectedIssue> Validate()
        {
            foreach (var itm in base.Validate())
            {
                yield return itm;
            }

            if (this.TransformExpression == null && String.IsNullOrEmpty(this.Name))
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, $"bi.mart.flow.step[$].map.source.name.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(Name)), Guid.Empty);
            }
            else if (this.TransformExpression is BiColumnMappingTransformJoin bcmtj)
            {
                foreach(var itm in bcmtj.Validate(false))
                {
                    yield return itm;
                }
            }
        }
    }

    /// <summary>
    /// Indicates the source transform from another column
    /// </summary>
    [XmlType(nameof(BiColumnMappingTransformJoin), Namespace = BiConstants.XmlNamespace)]
    [ExcludeFromCodeCoverage] // Serialization class
    public class BiColumnMappingTransformJoin : BiDataFlowStreamStep
    {
        /// <summary>
        /// Look up the join column
        /// </summary>
        [XmlAttribute("joinColumn"), JsonProperty("joinColumn")]
        public String Column { get; set; }

        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            foreach(var itm in base.Validate(isRoot))
            {
                yield return itm;
            }
            if(String.IsNullOrEmpty(this.Column))
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, $"bi.mart.flow.step[$].map.target.lookup.join.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(Column)), Guid.Empty);
            }
        }
    }

}