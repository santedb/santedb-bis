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
 * Date: 2025-1-10
 */
using Newtonsoft.Json;
using SanteDB.Core.BusinessRules;
using SanteDB.Core.i18n;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents an indicator definition. An "indicator" represents a key-performance value that is measured at regular intervals over time for 
    /// particular populations and groups.
    /// </summary>
    [XmlRoot(nameof(BiIndicatorDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlType(nameof(BiIndicatorDefinition), Namespace = BiConstants.XmlNamespace)]
    public class BiIndicatorDefinition : BiDefinition
    {

        /// <summary>
        /// Defines the data sources
        /// </summary>
        [XmlElement("query"), JsonProperty("query")]
        public BiQueryDefinition Query { get; set; }

        /// <summary>
        /// Gets or sets the period of the measure (how often the measure is to be taken)
        /// </summary>
        [XmlElement("period"), JsonProperty("period")]
        public BiIndicatorPeriodDefinition Period { get; set; }

        /// <summary>
        /// Gets or ses the reference ranges
        /// </summary>
        [XmlElement("referenceRange"), JsonProperty("referenceRange")]
        public List<BiSqlDefinition> ReferenceRanges { get; set; }

        /// <summary>
        /// Gets or sets the subject reference
        /// </summary>
        [XmlElement("subject"), JsonProperty("subject")]
        public BiIndicatorSubjectFieldRef Subject { get; set; }

        /// <summary>
        /// Gets or sets the measures in this indicator
        /// </summary>
        [XmlArray("measures"), XmlArrayItem("add"), JsonProperty("measures")]
        public List<BiIndicatorMeasureDefinition> Measures { get; set; }

        /// <inheritdoc/>
        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            if (this.Period == null)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bi.indicator.period.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(Period)), DetectedIssueKeys.InvalidDataIssue);
            }
            if (this.Measures?.Any() != true)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bi.indicator.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(Measures)), DetectedIssueKeys.InvalidDataIssue);
            }
            if (this.Subject == null)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bi.indicator.subject.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(Subject)), DetectedIssueKeys.InvalidDataIssue);
            }
            foreach (var itm in base.Validate(isRoot))
            {
                yield return itm;
            }
        }
    }
}
