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
using SanteDB.Core.BusinessRules;
using SanteDB.Core.i18n;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a basic SQL definition
    /// </summary>
    [XmlType(nameof(BiSqlDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiSqlDefinition), Namespace = BiConstants.XmlNamespace)]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage] // Model classes - ignored
    public class BiSqlDefinition : BiDefinition
    {

        /// <summary>
        /// Gets or sets the invariant of SQL dialect
        /// </summary>
        [XmlArray("providers"), XmlArrayItem("invariant")]
        public List<string> Invariants { get; set; }

        /// <summary>
        /// When specified, the materialized view information
        /// </summary>
        [XmlElement("materialize")]
        public BiMaterializeDefinition Materialize { get; set; }

        /// <summary>
        /// Gets or sets the SQL
        /// </summary>
        [XmlText]
        public string Sql { get; set; }

        /// <inheritdoc />
        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            foreach (var itm in base.Validate(isRoot))
            {
                yield return itm;
            }

            if (string.IsNullOrEmpty(this.Sql))
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bi.sql.missing", string.Format(ErrorMessages.MISSING_VALUE, nameof(Sql)), DetectedIssueKeys.InvalidDataIssue);
            }
            if (this.Materialize != null)
            {
                if (string.IsNullOrEmpty(this.Materialize.Name))
                {
                    yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bi.qry.materialize.name.missing", string.Format(ErrorMessages.MISSING_VALUE, nameof(BiMaterializeDefinition.Name)), Guid.Empty);
                }
                else if (String.IsNullOrEmpty(this.Materialize.Sql))
                {
                    yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bi.qry.materialize.sql.missing", string.Format(ErrorMessages.MISSING_VALUE, nameof(BiMaterializeDefinition.Sql)), Guid.Empty);
                }
            }

        }


    }
}