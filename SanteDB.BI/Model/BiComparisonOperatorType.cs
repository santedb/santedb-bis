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
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Comparison operator
    /// </summary>
    [XmlType(nameof(BiComparisonOperatorType), Namespace = BiConstants.XmlNamespace)]
    public enum BiComparisonOperatorType
    {
        /// <summary>
        /// Equal
        /// </summary>
        [XmlEnum("eq")]
        Equal,
        /// <summary>
        /// Not equal
        /// </summary>
        [XmlEnum("ne")]
        NotEqual,
        /// <summary>
        /// Less than
        /// </summary>
        [XmlEnum("lt")]
        LessThan,
        /// <summary>
        /// Less than or equal
        /// </summary>
        [XmlEnum("lte")]
        LessThanOrEqual,
        /// <summary>
        /// Greater than
        /// </summary>
        [XmlEnum("gt")]
        GreaterThan,
        /// <summary>
        /// Greater than or equal
        /// </summary>
        [XmlEnum("gte")]
        GreaterThanOrEqual

    }
}