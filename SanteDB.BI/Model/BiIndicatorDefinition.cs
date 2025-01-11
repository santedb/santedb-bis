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
        /// Gets or sets the measures in this indicator
        /// </summary>
        [XmlArray("measures"), XmlArrayItem("add"), JsonProperty("measures")]
        public List<BiIndicatorMeasureDefinition> Measures { get; set; }

        /// <inheritdoc/>
        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            if(this.Measures?.Any() != true)
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, "bi.measure.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(Measures)), DetectedIssueKeys.InvalidDataIssue);
            }
            foreach(var itm in base.Validate(isRoot))
            {
                yield return itm;
            }
        }
    }
}
