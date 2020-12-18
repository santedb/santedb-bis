using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents an abstract flow step
    /// </summary>
    [XmlType(nameof(BiDataFlowStep), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public abstract class BiDataFlowStep : BiDefinition
    {
    }
}