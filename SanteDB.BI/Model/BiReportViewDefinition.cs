/*
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
using SanteDB.Core.BusinessRules;
using SanteDB.Core.i18n;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{

    /// <summary>
    /// The classification of view type
    /// </summary>
    [XmlType(nameof(BiReportViewType), Namespace = BiConstants.XmlNamespace)]
    public enum BiReportViewType
    {
        /// <summary>
        /// The report is a tabular report
        /// </summary>
        [XmlEnum("tabular")]
        Tabular,
        /// <summary>
        /// The report is a chart
        /// </summary>
        [XmlEnum("chart")]
        Chart
    }
    /// <summary>
    /// Represents a view
    /// </summary>
    [XmlType(nameof(BiReportViewDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiReportViewDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] // Serialization class
    public class BiReportViewDefinition : BiDefinition
    {

        /// <summary>
        /// Gets or sets the type of report view
        /// </summary>
        [XmlAttribute("type"), JsonProperty("type")]
        public BiReportViewType Type { get; set; }

        /// <summary>
        /// Don't allow the export of this view
        /// </summary>
        [XmlAttribute("no-export"), JsonProperty("noExport")]
        public bool PreventExport { get; set; }

        /// <summary>
        /// Don't allow printing
        /// </summary>
        [XmlAttribute("no-print"), JsonProperty("noPrint")]
        public bool PreventPrinting { get; set; }

        /// <summary>
        /// Gets or sets the body of the element
        /// </summary>
        [XmlElement("div", Namespace = BiConstants.HtmlNamespace), JsonIgnore]
        public XElement Body { get; set; }

        /// <inheritdoc/>
        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            foreach (var itm in base.Validate(isRoot))
            {
                yield return itm;
            }
            if (this.Body == null)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bre.div.missing", string.Format(ErrorMessages.MISSING_VALUE, nameof(Body)), Guid.Empty);
            }
            if (String.IsNullOrEmpty(this.Name))
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bre.name.missing", string.Format(ErrorMessages.MISSING_VALUE, nameof(Name)), Guid.Empty);
            }
        }
    }
}