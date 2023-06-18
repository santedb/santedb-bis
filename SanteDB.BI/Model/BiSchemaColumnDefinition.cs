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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{



    /// <summary>
    /// Represents a definition for a column
    /// </summary>
    [XmlType(nameof(BiSchemaColumnDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] // Serialization class
    public class BiSchemaColumnDefinition : BiDefinition
    {

        /// <summary>
        /// Gets or sets the type of column
        /// </summary>
        [XmlAttribute("type"), JsonProperty("type")]
        public BiDataType Type { get; set; }

        /// <summary>
        /// True if this is not null
        /// </summary>
        [XmlAttribute("notNull"), JsonProperty("notNull")]
        public bool NotNull { get; set; }

        /// <summary>
        /// True if this column is indexed
        /// </summary>
        [XmlAttribute("index"), JsonProperty("index")]
        public bool IsIndex { get; set; }

        /// <summary>
        /// True if this column is unique
        /// </summary>
        [XmlAttribute("unique"), JsonProperty("unique")]
        public bool IsUnique { get; set; }

        /// <summary>
        /// True if this object is a key
        /// </summary>
        [XmlAttribute("key"), JsonProperty("key")]
        public bool IsKey { get; set; }

        /// <summary>
        /// Gets or sets the table that this column referneces (as a foreign key)
        /// </summary>
        [XmlElement("otherTable"), JsonProperty("otherTable")]
        public BiObjectReference References { get; set; }

        /// <inheritdoc/>
        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            if (String.IsNullOrEmpty(this.Name))
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, $"bi.mart.schema.table.column.name.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(Name)), DetectedIssueKeys.InvalidDataIssue);
            }
        }

        /// <summary>
        /// Validate the <paramref name="value"/> can be inserted in this column
        /// </summary>
        internal bool ValidateValue(object value)
        {
            var expectedType = this.Type;
            if (expectedType == BiDataType.Ref && this.References.Resolved is BiSchemaTableDefinition otherTable)
            {
                BiSchemaColumnDefinition keyCol = null;
                do
                {
                    keyCol = otherTable.Columns.FirstOrDefault(o => o.IsKey);
                    otherTable = otherTable.Parent?.Resolved as BiSchemaTableDefinition;
                } while (keyCol == null && otherTable != null);
                expectedType = keyCol?.Type ?? throw new MissingPrimaryKeyException();
            }
            switch (value)
            {

                case DateTime a:
                case DateTimeOffset b:
                    return expectedType == BiDataType.DateTime || expectedType == BiDataType.Date;
                case String c:
                    return expectedType == BiDataType.String;
                case double dd:
                case float ff:
                    return expectedType == BiDataType.Float || expectedType == BiDataType.Decimal;
                case int d:
                case uint e:
                case long f:
                case short g:
                case byte h:
                case ulong i:
                    return expectedType == BiDataType.Integer ||
                        expectedType == BiDataType.DateTime || expectedType == BiDataType.Date ||
                        (expectedType == BiDataType.Boolean && (long)value <= 1 && (long)value >= 0); // HACK: SQLITE dates are represented as integers as well and the BI layer doesnt differentiate between them sometimes
                case Guid j:
                    return expectedType == BiDataType.Uuid;
                case bool k:
                    return expectedType == BiDataType.Boolean;
                default:
                    if (value is Decimal) return expectedType == BiDataType.Decimal;
                    else return true;
            }
        }
    }
}