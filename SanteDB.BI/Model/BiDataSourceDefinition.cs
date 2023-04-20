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
using SanteDB.Core.BusinessRules;
using SanteDB.Core.i18n;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a BIS datasource definition
    /// </summary>
    [XmlType(nameof(BiDataSourceDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiDataSourceDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [ExcludeFromCodeCoverage] // Serialization class
    public class BiDataSourceDefinition : BiDefinition
    {

        /// <summary>
        /// Gets or sets the instance of the provider
        /// </summary>
        [XmlAttribute("provider"), JsonProperty("provider")]
        public String ProviderTypeXml
        {
            get => this.ProviderType?.AssemblyQualifiedName;
            set
            {
                if (value != null)
                {
                    this.ProviderType = Type.GetType(value);
                }
                else
                {
                    this.ProviderType = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the C# type of the provider
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public Type ProviderType { get; set; }

        /// <summary>
        /// Gets or sets the connection string
        /// </summary>
        [XmlAttribute("connectionString"), JsonProperty("connectionString")]
        public String ConnectionString { get; set; }

        /// <inheritdoc/>
        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            foreach(var itm in base.Validate(isRoot))
            {
                yield return itm;
            }

            if(isRoot)
            {
                if(String.IsNullOrEmpty(this.ConnectionString))
                {
                    yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bre.connectionString.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(ConnectionString)), Guid.Empty);
                }
                if(String.IsNullOrEmpty(this.ProviderTypeXml))
                {
                    yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bre.provider.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(ProviderType)), Guid.Empty);
                }
                else if(this.ProviderType == null)
                {
                    yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bre.provider.type", String.Format(ErrorMessages.TYPE_NOT_FOUND, this.ProviderTypeXml), Guid.Empty);
                }
            }
        }

        /// <inheritdoc/>
        public override string ToString() => $"[{this.GetType().Name} {this.Name}({this.ConnectionString ?? "auto"})]";


        /// <summary>
        /// Return as a summarized data
        /// </summary>
        /// <returns></returns>
        public override BiDefinition AsSummarized() => this;
    }
}