using Newtonsoft.Json;
using SanteDB.Core.BusinessRules;
using SanteDB.Core.i18n;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Schema column reference
    /// </summary>
    [XmlType(nameof(BiSchemaObjectReference), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiSchemaObjectReference
    {

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public string Name { get; set; }

        /// <inheritdoc/>
        internal virtual IEnumerable<DetectedIssue> Validate()
        {
            if (String.IsNullOrEmpty(this.Name))
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, $"bi.mart.table.column.ref.name.missing", String.Format(ErrorMessages.MISSING_VALUE, nameof(Name)), DetectedIssueKeys.InvalidDataIssue);
            }
        }
    }
}
