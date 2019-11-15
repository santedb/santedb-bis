using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a collection of BI definitions
    /// </summary>
    [XmlType(nameof(BiDefinitionCollection), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(BiDefinitionCollection), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [XmlInclude(typeof(BiViewDefinition))]
    [XmlInclude(typeof(BiQueryDefinition))]
    [XmlInclude(typeof(BiParameterDefinition))]
    [XmlInclude(typeof(BiReportDefinition))]
    [XmlInclude(typeof(BiRenderFormatDefinition))]
    [XmlInclude(typeof(BiDataSourceDefinition))]
    [XmlInclude(typeof(BiPackage))]
    public class BiDefinitionCollection : List<BiDefinition>
    {

        /// <summary>
        /// Collection ctor
        /// </summary>
        public BiDefinitionCollection()
        {

        }

        /// <summary>
        /// Collection ctor with objects 
        /// </summary>
        public BiDefinitionCollection(IEnumerable<BiDefinition> items) : base(items)
        {

        }
    }
}
