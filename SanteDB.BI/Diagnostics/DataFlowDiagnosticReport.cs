using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SanteDB.BI.Diagnostics
{
    /// <summary>
    /// Data flow diagnostic report
    /// </summary>
    [XmlType(nameof(DataFlowDiagnosticReport), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(DataFlowDiagnosticReport), Namespace = BiConstants.XmlNamespace)]
    public class DataFlowDiagnosticReport
    {

        /// <summary>
        /// Data flow diagnostics
        /// </summary>
        public DataFlowDiagnosticReport()
        {
            
        }

        /// <summary>
        /// Create report from actions
        /// </summary>
        internal DataFlowDiagnosticReport(DateTime startTime, IEnumerable<DataFlowDiagnosticActionInfo> rootActions)
        {
            this.StartTime = startTime;
            this.GeneratedTime = DateTime.Now;
            this.Actions = rootActions.Select(o => new DataFlowDiagnosticActionReport(o)).ToList();
        }

        /// <summary>
        /// Gets the time that the session started
        /// </summary>
        [XmlElement("start"), JsonProperty("start")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets the time that this report was generated
        /// </summary>
        [XmlElement("generated"), JsonProperty("generated")]
        public DateTime GeneratedTime { get; set; }

        /// <summary>
        /// Gets the list of actions in this report
        /// </summary>
        [XmlArray("actions"), XmlArrayItem("action"), JsonProperty("actions")]
        public List<DataFlowDiagnosticActionReport> Actions { get; set; }

    }
    
    /// <summary>
    /// Action reports
    /// </summary>
    [XmlType(nameof(DataFlowDiagnosticActionReport), Namespace = BiConstants.XmlNamespace)]
    public class DataFlowDiagnosticActionReport
    {

        /// <summary>
        /// Serialization ctor
        /// </summary>
        public DataFlowDiagnosticActionReport()
        {
            
        }

        /// <summary>
        /// Copy values from <paramref name="sourceData"/>
        /// </summary>
        internal DataFlowDiagnosticActionReport(DataFlowDiagnosticActionInfo sourceData)
        {
            this.Uuid = sourceData.Uuid;
            this.Name = sourceData.Name;
            this.StartOfAction = sourceData.StartOfAction.DateTime;
            this.EndOfAction = sourceData.EndOfAction?.DateTime;
            this.StepType = sourceData.StepType;
            this.Children = sourceData.Children.Select(o => new DataFlowDiagnosticActionReport(o)).ToList();
        }

        /// <summary>
        /// Get the UUID of the action
        /// </summary>
        [XmlAttribute("uuid"), JsonProperty("uuid")]
        public Guid Uuid { get; set; }

        /// <summary>
        /// The flow step type
        /// </summary>
        [XmlAttribute("type"), JsonProperty("type")]
        public String StepType { get; set; }

        /// <summary>
        /// The flow step name
        /// </summary>
        [XmlAttribute("name"), JsonProperty("name")]
        public String Name { get; set; }

        /// <summary>
        /// Gets the start of the action
        /// </summary>
        [XmlElement("started"), JsonProperty("started")]
        public DateTime StartOfAction { get; set; }

        /// <summary>
        /// Gets the end of the action
        /// </summary>
        [XmlElement("ended"), JsonProperty("ended")]
        public DateTime? EndOfAction { get; set; }

        /// <summary>
        /// Children 
        /// </summary>
        [XmlArray("children"), XmlArrayItem("child"), JsonProperty("children")]
        public List<DataFlowDiagnosticActionReport> Children { get; set; }

    }
}
