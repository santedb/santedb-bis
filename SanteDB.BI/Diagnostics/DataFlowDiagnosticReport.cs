﻿using Newtonsoft.Json;
using SanteDB.BI.Datamart.DataFlow;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
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

        private static XmlSerializer s_xsz = new XmlSerializer(typeof(DataFlowDiagnosticReport));

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
        [XmlElement("action"), JsonProperty("actions")]
        public List<DataFlowDiagnosticActionReport> Actions { get; set; }

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="str"></param>
        public void Save(Stream str)
        {
            s_xsz.Serialize(str, this);
        }
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
            this.DebugInfo = sourceData.DebugInfo;
            this.StartOfAction = sourceData.StartOfAction.DateTime;
            this.EndOfAction = sourceData.EndOfAction?.DateTime;
            this.StepType = sourceData.StepType;
            this.Actions = sourceData.Children.Select(o => new DataFlowDiagnosticActionReport(o)).ToList();
            this.Samples = sourceData.Samples.Where(o=>o.Type != DataFlowDiagnosticSampleType.CurrentRecord).Select(o => new DataFlowDiagnosticSampleReport(o)).ToList();
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
        /// Gets the debug information
        /// </summary>
        [XmlElement("debug"), JsonProperty("debug")]
        public String DebugInfo { get; set; }

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
        [XmlElement("action"), JsonProperty("actions")]
        public List<DataFlowDiagnosticActionReport> Actions { get; set; }

        /// <summary>
        /// Gets or sets the samples collected
        /// </summary>
        [XmlElement("sample"), JsonProperty("samples")]
        public List<DataFlowDiagnosticSampleReport> Samples { get; set; }
    }

    /// <summary>
    /// Data flow diagnostic sample report
    /// </summary>
    [XmlType(nameof(DataFlowDiagnosticSampleReport), Namespace = BiConstants.XmlNamespace)]
    public class DataFlowDiagnosticSampleReport
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        public DataFlowDiagnosticSampleReport()
        {
            
        }
        /// <summary>
        /// New sample ctor
        /// </summary>
        internal DataFlowDiagnosticSampleReport(DataFlowDiagnosticSample sample)
        {
            this.Type = sample.Type.HasFlag(DataFlowDiagnosticSampleType.PointInTime) ? sample.Type ^ DataFlowDiagnosticSampleType.PointInTime : sample.Type;
            this.Timestamp = sample.Timestamp.DateTime;
            this.Value = sample.Value.ToString();
        }

        /// <summary>
        /// Gets the type of the sample
        /// </summary>
        [XmlAttribute("type"), JsonProperty("type")]
        public DataFlowDiagnosticSampleType Type { get; set; }


        /// <summary>
        /// Gets the timestamp of the sample
        /// </summary>
        [XmlAttribute("timestamp"), JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        [XmlText, JsonProperty("value")]
        public String Value { get; set; }
    }
}
