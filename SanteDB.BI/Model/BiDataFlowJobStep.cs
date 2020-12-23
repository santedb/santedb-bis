using SanteDB.Core.Jobs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{
    /// <summary>
    /// Represents a data flow step which can invoke a job
    /// </summary>
    [XmlType(nameof(BiDataFlowJobStep), Namespace = BiConstants.XmlNamespace)]
    public class BiDataFlowJobStep : BiDataFlowCallStep
    {



    }
}
