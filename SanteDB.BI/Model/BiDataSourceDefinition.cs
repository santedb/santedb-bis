using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a BIS datasource definition
    /// </summary>
    [XmlType(nameof(BiDataSourceDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiDataSourceDefinition), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiDataSourceDefinition : BiDefinition
    {

        /// <summary>
        /// Gets or sets the instance of the provider
        /// </summary>
        [XmlAttribute("provider"), JsonProperty("provider")]
        public String ProviderTypeXml {
            get => this.ProviderType?.AssemblyQualifiedName;
            set
            {
                if (value != null)
                    this.ProviderType = Type.GetType(value);
                else
                    this.ProviderType = null;
            }
        }

        /// <summary>
        /// Gets or sets the C# type of the provider
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public Type ProviderType { get; set; }

        /// <summary>
        /// Gets or sets the connection string
        /// </summary>
        [XmlAttribute("connectionString"), JsonProperty("connectionString")]
        public String ConnectionString { get; set; }

    }
}