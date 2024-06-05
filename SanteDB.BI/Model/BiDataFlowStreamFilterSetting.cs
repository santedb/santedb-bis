/*
 * Copyright (C) 2021 - 2024, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a simple filter setting
    /// </summary>
    [XmlType(nameof(BiDataFlowStreamFilterSetting), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiDataFlowStreamFilterSetting
    {

        /// <summary>
        /// Gets or sets the field to be checked
        /// </summary>
        [XmlAttribute("field"), JsonProperty("field")]
        public String Field { get; set; }

        /// <summary>
        /// Gets or sets the comparison operator
        /// </summary>
        [XmlAttribute("op"), JsonProperty("op")]
        public BiComparisonOperatorType Operator { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        [XmlElement("string", typeof(String)), XmlElement("int", typeof(Int64?)), XmlElement("bool", typeof(bool?)), JsonProperty("value")]
        public object Value { get; set; }

        /// <summary>
        /// Validate this object
        /// </summary>
        internal virtual IEnumerable<DetectedIssue> Validate()
        {

            if (String.IsNullOrEmpty(this.Field))
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, $"bi.mart.flow.filter.all[{this.Field}].field.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(Field)), Guid.Empty);

            }
            if (this.Value == null)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, $"bi.mart.flow.filter.all[{this.Field}].value.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(Value)), Guid.Empty);

            }
        }
    }
}