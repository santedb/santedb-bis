using SanteDB.Core.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
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

    }
}
