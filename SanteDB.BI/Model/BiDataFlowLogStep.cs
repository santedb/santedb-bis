﻿/*
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
 * Date: 2023-3-10
 */
using DynamicExpresso;
using Newtonsoft.Json;
using SanteDB.BI.Datamart.DataFlow;
using SanteDB.Core.BusinessRules;
using SanteDB.Core.i18n;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Tracing;
using System.Dynamic;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace SanteDB.BI.Model
{

    /// <summary>
    /// Log destinations
    /// </summary>
    [XmlType(nameof(BiLogDestinationType), Namespace = BiConstants.XmlNamespace)]
    [Flags]
    public enum BiLogDestinationType
    {
        /// <summary>
        ///  Log to any
        /// </summary>
        [XmlEnum("any")]
        Any = 0, 
        /// <summary>
        /// log to the execution log
        /// </summary>
        [XmlEnum("execution")]
        ExecutionLog = 0x1,
        /// <summary>
        /// Log to the console
        /// </summary>
        [XmlEnum("console")]
        Console = 0x2,
        /// <summary>
        /// log to tracer
        /// </summary>
        [XmlEnum("trace")]
        Trace = 0x4
    }

    /// <summary>
    /// Writes to the log file the rows/message to the log
    /// </summary>
    [XmlType(nameof(BiDataFlowLogStep), Namespace = BiConstants.XmlNamespace)]
    [JsonObject]
    [ExcludeFromCodeCoverage] // Serialization class
    public class BiDataFlowLogStep : BiDataFlowStreamStep
    {

        // Lambda from the message property
        private static readonly Regex m_formatRegex = new Regex(@"\{\{(.+?)\}\}", RegexOptions.Compiled);

        /// <summary>
        /// The priority of the object to log
        /// </summary>
        [XmlAttribute("priority"), JsonProperty("priority")]
        public EventLevel Priority { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        [XmlText(), JsonProperty("message")]
        public string Message { get; set; }

        /// <summary>
        /// Log destination
        /// </summary>
        [XmlAttribute("logTo"), JsonProperty("logTo")]
        public BiLogDestinationType Destination { get; set; }

        /// <summary>
        /// Get a string formatted to the message with the specified input object
        /// </summary>
        /// <param name="scopedObject">The object to emit the log for</param>
        internal String Format(object scopedObject)
        {
            if(scopedObject is IDictionary<String, Object> dict)
            {
                if (String.IsNullOrEmpty(this.Message?.Trim()))
                {
                    return String.Join(",", dict.Values);
                }
                else
                {
                    return m_formatRegex.Replace(this.Message.Trim(), o => dict.TryGetValue(o.Groups[1].Value.Trim(), out var v) ? v.ToString() : String.Empty);
                }
            }
            else if(scopedObject is DataFlowScope dfs)
            {
                return m_formatRegex.Replace(this.Message.Trim(), o => dfs[o.Groups[1].Value.Trim()].ToString());
            }
            else
            {
                throw new ArgumentOutOfRangeException(String.Format(ErrorMessages.ARGUMENT_INCOMPATIBLE_TYPE, typeof(IDictionary), scopedObject.GetType()));
            }
        }

        internal override IEnumerable<DetectedIssue> Validate(bool isRoot)
        {
            if (this.InputObject != null) // Input is not null - so we need to validate like regular stream step!
            {
                foreach(var itm in base.Validate(isRoot))
                {
                    yield return itm;
                }
            }
            else if(string.IsNullOrEmpty(this.Message))
            {
                yield return new DetectedIssue(DetectedIssuePriorityType.Error, $"bi.mart.flow.step[${this.Name}].message.missing", string.Format(ErrorMessages.MISSING_VALUE, nameof(Message)), Guid.Empty);
            }
        }
    }
}