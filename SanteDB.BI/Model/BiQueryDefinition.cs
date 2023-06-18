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
 * Date: 2023-5-19
 */
using Newtonsoft.Json;
using SanteDB.Core.BusinessRules;
using SanteDB.Core.i18n;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a query definition
    /// </summary>
    [XmlType(nameof(BiQueryDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiQueryDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] // Serialization class
    public class BiQueryDefinition : BiDefinition
    {
        /// <summary>
        /// Default ctor
        /// </summary>
        public BiQueryDefinition()
        {
            this.DataSources = new List<BiDataSourceDefinition>();
            this.Parameters = new List<BiParameterDefinition>();
            this.QueryDefinitions = new List<BiSqlDefinition>();
        }

        /// <summary>
        /// Gets or sets the data sources
        /// </summary>
        [XmlArray("dataSources"), XmlArrayItem("add"), JsonProperty("dataSources")]
        public List<BiDataSourceDefinition> DataSources { get; set; }

        /// <summary>
        /// Gets or sets the parameter for this query
        /// </summary>
        [XmlArray("parameters"), XmlArrayItem("add"), JsonProperty("parameters")]
        public List<BiParameterDefinition> Parameters { get; set; }

        /// <summary>
        /// Gets or sets the SQL definition
        /// </summary>
        [XmlArray("definitions"), XmlArrayItem("add"), JsonProperty("queryDefinitions")]
        public List<BiSqlDefinition> QueryDefinitions { get; set; }

        /// <inheritdoc/>
        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            foreach (var itm in base.Validate(isRoot))
            {
                yield return itm;
            }

            if (this.DataSources == null || this.DataSources.Count == 0)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bi.source.missing", string.Format(ErrorMessages.MISSING_VALUE, nameof(DataSources)), DetectedIssueKeys.InvalidDataIssue);
            }
            if (this.QueryDefinitions == null || this.QueryDefinitions.Count == 0)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bi.sql.missing", string.Format(ErrorMessages.MISSING_VALUE, nameof(QueryDefinitions)), DetectedIssueKeys.InvalidDataIssue);
            }

            foreach (var itm in this.Parameters.OfType<BiDefinition>().Union(this.QueryDefinitions).SelectMany(o => o.Validate(false)))
            {
                yield return itm;
            }
        }

    }
}