/*
 * Copyright (C) 2021 - 2023, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors
 * Portions Copyright (C) 2015-2018 Mohawk College of Applied Arts and Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2023-5-19
 */
using Newtonsoft.Json;
using SanteDB.BI.Datamart.DataFlow;
using SanteDB.BI.Model;
using SanteDB.Core.BusinessRules;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Xml.Serialization;

namespace SanteDB.BI.Datamart
{
    /// <summary>
    /// Represents metadata in XML for <see cref="IDatamart"/>
    /// </summary>
    [XmlType(nameof(DatamartInfo), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(DatamartInfo), Namespace = BiConstants.XmlNamespace)]
    public class DatamartInfo : BiDefinition
    {

        /// <summary>
        /// Serialziation ctor
        /// </summary>
        public DatamartInfo()
        {
        }

        /// <summary>
        /// Copy from <paramref name="datamart"/>
        /// </summary>
        public DatamartInfo(IDatamart datamart)
        {
            this.Id = datamart.Id;
            this.CreatedByKey = datamart.CreatedByKey.GetValueOrDefault();
            this.CreationTime = datamart.CreationTime.DateTime;
            this.Name = datamart.Name;
            this.ObsoletedByKey = datamart.ObsoletedByKey;
            this.ObsoletionTime = datamart.ObsoletionTime?.DateTime;
            this.UpdatedByKey = datamart.UpdatedByKey;
            this.UpdatedTime = datamart.UpdatedTime?.DateTime;
            this.Executions = datamart.FlowExecutions.Take(25).Select(o => new DataMartExecutionInfo(o)).ToList();
            this.MetaData = new BiMetadata()
            {
                Annotation = new BiAnnotation()
                {
                    JsonBody = datamart.Description
                },
                Version = datamart.Version
            };
        }


        /// <summary>
        /// Gets or sets the creation time
        /// </summary>
        [XmlElement("createdBy"), JsonProperty("createdBy")]
        public Guid CreatedByKey { get; set; }

        /// <summary>
        /// Gets or set the creation time
        /// </summary>
        [XmlElement("creationTime"), JsonProperty("creationTime")]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the obsoletion time
        /// </summary>
        [XmlElement("obsoletedBy"), JsonProperty("obsoletedBy")]
        public Guid? ObsoletedByKey { get; set; }

        /// <summary>
        /// Gets or sets the obsolete time
        /// </summary>
        [XmlElement("obsoletionTime"), JsonProperty("obsoletionTime")]
        public DateTime? ObsoletionTime { get; set; }

        /// <summary>
        /// Gets or sets the user that updated
        /// </summary>
        [XmlElement("updatedBy"), JsonProperty("updatedBy")]
        public Guid? UpdatedByKey { get; set; }

        /// <summary>
        /// Gets or sets the updated time
        /// </summary>
        [XmlElement("updatedTime"), JsonProperty("updatedTime")]
        public DateTime? UpdatedTime { get; set; }

        /// <summary>
        /// Flow executions
        /// </summary>
        [XmlElement("exec"), JsonProperty("exec")]
        public List<DataMartExecutionInfo> Executions { get; set; }
    }

    /// <summary>
    /// Get the execution informations
    /// </summary>
    [XmlType(nameof(DataMartExecutionInfo), Namespace = BiConstants.XmlNamespace)]
    [XmlRoot(nameof(DataMartExecutionInfo), Namespace = BiConstants.XmlNamespace)]
    public class DataMartExecutionInfo
    {

        /// <summary>
        /// Serialization ctor
        /// </summary>
        public DataMartExecutionInfo()
        {

        }

        /// <summary>
        /// Get the execution entry
        /// </summary>
        public DataMartExecutionInfo(IDataFlowExecutionEntry dataFlowExecutionEntry)
        {
            this.Key = dataFlowExecutionEntry.Key.GetValueOrDefault();
            this.StartedByKey = dataFlowExecutionEntry.CreatedByKey.GetValueOrDefault();
            this.Finished = dataFlowExecutionEntry.Finished?.DateTime;
            this.Started = dataFlowExecutionEntry.Started.DateTime;
            this.DiagnosticLink = dataFlowExecutionEntry.DiagnosticSessionKey;
            this.Outcome = dataFlowExecutionEntry.Outcome;
            this.Purpose = dataFlowExecutionEntry.Purpose;
        }

        /// <summary>
        /// Gets or sets the key
        /// </summary>
        [XmlElement("id"), JsonProperty("id")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the started user
        /// </summary>
        [XmlElement("startedBy"), JsonProperty("startedBy")]
        public Guid StartedByKey { get; set; }

        /// <summary>
        /// Gets or sets the started time
        /// </summary>
        [XmlElement("started"), JsonProperty("started")]
        public DateTime Started { get; set; }

        /// <summary>
        /// Gets or sets the finished time
        /// </summary>
        [XmlElement("finished"), JsonProperty("finished")]
        public DateTime? Finished { get; set; }

        /// <summary>
        /// Gets or sets the diagnostic stream link
        /// </summary>
        [XmlElement("diagnostic"), JsonProperty("diagnostic")]
        public Guid? DiagnosticLink { get; set; }

        /// <summary>
        /// Outcome
        /// </summary>
        [XmlElement("outcome"), JsonProperty("outcome")]
        public DataFlowExecutionOutcomeType Outcome { get; set; }

        /// <summary>
        /// Purpose
        /// </summary>
        [XmlElement("purpose"), JsonProperty("purpose")]
        public DataFlowExecutionPurposeType Purpose { get; set; }

        /// <summary>
        /// Gets or sets the log entries
        /// </summary>
        [XmlArray("log"), XmlArrayItem("entry"), JsonProperty("log")]
        public List<DatamartLogEntry> LogEntry { get; set; }
    }

    /// <summary>
    /// Datamart log entry
    /// </summary>
    [XmlType(nameof(DatamartLogEntry), Namespace = BiConstants.XmlNamespace)]
    public class DatamartLogEntry
    {

        /// <summary>
        /// Serialization ctor
        /// </summary>
        public DatamartLogEntry()
        {
        }

        /// <summary>
        /// Create a log entry from a BI log entry
        /// </summary>
        /// <param name="logEntry">The log entry from which to populate this object</param>
        public DatamartLogEntry(IDataFlowLogEntry logEntry)
        {
            this.Priority = logEntry.Priority;
            this.Text = logEntry.Text;
            this.Timestamp = logEntry.Timestamp.DateTime;
        }

        /// <summary>
        /// Gets or sets the priority level
        /// </summary>
        [XmlAttribute("priority"), JsonProperty("priority")]
        public EventLevel Priority { get; set; }

        /// <summary>
        /// Gets or sets the text
        /// </summary>
        [XmlText, JsonProperty("text")]
        public string Text { get;  set; }

        /// <summary>
        /// Gets or sets the timestamp
        /// </summary>
        [XmlAttribute("timestamp"), JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
