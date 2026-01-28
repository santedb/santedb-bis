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
using SanteDB.Core.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace SanteDB.BI.Configuration
{
    /// <summary>
    /// Settings related to the BI infrastructure
    /// </summary>
    [XmlType(nameof(BiConfigurationSection), Namespace = "http://santedb.org/configuration")]
    public class BiConfigurationSection : IConfigurationSection
    {

        /// <summary>
        /// Maximum BI result set size
        /// </summary>
        [XmlElement("maxBiResultSize")]
        [Category("Performance")]
        [DisplayName("Maximum BI Result Set Size")]
        [Description("Sets the maximum number of results that can be returned in a BI result set. This will prevent BI reports from consuming large amounts of data (default is 10,000)")]
        public int? MaxBiResultSetSize { get; set; }

        /// <summary>
        /// Gets or sets the identifiers of datamarts which should be automatically registered
        /// </summary>
        [XmlArray("dataMarts"), XmlArrayItem("register")]
        [Category("Datamarts")]
        [DisplayName("Automatically Register")]
        [Description("Sets the data mart identifiers which are to be automatically registered if they aren't already registered")]
        public List<String> AutoRegisterDatamarts { get; set; }

    }
}
