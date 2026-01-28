/*
 * Copyright (C) 2021 - 2026, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
using SanteDB.Core.BusinessRules;
using SanteDB.Core.i18n;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{

    /// <summary>
    /// Data flow connection mode
    /// </summary>
    [XmlType(nameof(BiDataFlowConnectionMode), Namespace = BiConstants.XmlNamespace)]
    public enum BiDataFlowConnectionMode
    {
        /// <summary>
        /// Read only
        /// </summary>
        [XmlEnum("read-only")]
        ReadOnly,
        /// <summary>
        /// Read or write
        /// </summary>
        [XmlEnum("read-write")]
        ReadWrite
    }

    /// <summary>
    /// Represents a connection
    /// </summary>
    [XmlType(nameof(BiDataFlowConnectionStep), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [ExcludeFromCodeCoverage] // Serialization class
    public class BiDataFlowConnectionStep : BiDataFlowStep
    {

        /// <summary>
        /// Gets or sets the data source
        /// </summary>
        [XmlElement("dataSource"), JsonProperty("dataSource")]
        public BiDataSourceDefinition DataSource { get; set; }

        /// <summary>
        /// Gets or sets the mode
        /// </summary>
        [XmlAttribute("mode"), JsonProperty("mode")]
        public BiDataFlowConnectionMode Mode { get; set; }

        /// <inheritdoc/>
        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            foreach (var itm in base.Validate(isRoot))
            {
                yield return itm;
            }

            if (this.DataSource == null && String.IsNullOrEmpty(this.Ref))
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, $"bi.mart.flow.step[{this.Name}].dataSource.missing", string.Format(ErrorMessages.MISSING_VALUE, nameof(DataSource)), Guid.Empty);
            }
            else if (this.DataSource != null)
            {
                foreach (var itm in this.DataSource.Validate(false))
                {
                    yield return itm;
                }
            }
        }


        /// <inheritdoc/>
        public override string ToString() => $"[{this.GetType().Name} {this.Name} = {this.Mode}({this.DataSource})]";
    }
}