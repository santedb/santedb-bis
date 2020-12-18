using Newtonsoft.Json;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a reference to another schema object
    /// </summary>
    [XmlType(nameof(BiSchemaObjectReference), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    public class BiSchemaObjectReference : BiDefinition
    {
    }
}