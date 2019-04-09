using System;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a BIS datasource definition
    /// </summary>
    [XmlType(nameof(BisDataSourceDefinition), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BisDataSourceDefinition), Namespace = BiConstants.XmlNamespace)]
    public class BisDataSourceDefinition : BisDefinition
    {

        /// <summary>
        /// Gets or sets the instance of the provider
        /// </summary>
        [XmlAttribute("provider")]
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
        [XmlIgnore]
        public Type ProviderType { get; set; }

        /// <summary>
        /// Gets or sets the connection string
        /// </summary>
        [XmlAttribute("connectionString")]
        public String ConnectionString { get; set; }

    }
}