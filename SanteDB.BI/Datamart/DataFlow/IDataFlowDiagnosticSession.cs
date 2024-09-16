/*
 * Copyright (C) 2021 - 2024, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 */
using SanteDB.BI.Diagnostics;
using SanteDB.BI.Model;
using System;

namespace SanteDB.BI.Datamart.DataFlow
{
    /// <summary>
    /// Diagnostic action event arguments
    /// </summary>
    public class DiagnosticActionEventArgs : EventArgs
    {

        /// <summary>
        /// Gets the action that started or ended
        /// </summary>
        public IDataFlowDiagnosticAction Action { get; }

        /// <summary>
        /// Create a new diagnostic sample event args
        /// </summary>
        public DiagnosticActionEventArgs(IDataFlowDiagnosticAction action)
        {
            this.Action = action;
        }
    }


    /// <summary>
    /// Diagnostic sample event arguments
    /// </summary>
    public class DiagnosticSampleEventArgs : EventArgs
    {

        /// <summary>
        /// Gets the type of sample
        /// </summary>
        public DataFlowDiagnosticSampleType SampleType { get; }

        /// <summary>
        /// Gets the value of the sample
        /// </summary>
        public object SampleValue { get; }

        /// <summary>
        /// Create a new diagnostic sample event args
        /// </summary>
        public DiagnosticSampleEventArgs(DataFlowDiagnosticSampleType sampleType, object value)
        {
            this.SampleType = sampleType;
            this.SampleValue = value;
        }
    }

    /// <summary>
    /// Idnetiifes the type of diagnostic sample collected
    /// </summary>
    public enum DataFlowDiagnosticSampleType
    {
        /// <summary>
        /// The sample is a log of raw data
        /// </summary>
        LoggedData = 0x1,
        /// <summary>
        /// The sample represents throughput
        /// </summary>
        RecordThroughput = 0x2,
        /// <summary>
        /// The sample represents a total of records processed
        /// </summary>
        TotalRecordProcessed = 0x4,
        /// <summary>
        /// The sample represents a serialization of the current record
        /// </summary>
        CurrentRecord = 0x8
    }

    /// <summary>
    /// Data flow diagnostic action
    /// </summary>
    public interface IDataFlowDiagnosticAction
    {

        /// <summary>
        /// Get the name of the action
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get the start of the action
        /// </summary>
        DateTimeOffset StartOfAction { get; }

        /// <summary>
        /// Get the end of the action
        /// </summary>
        DateTimeOffset? EndOfAction { get; }

        /// <summary>
        /// Log a sample
        /// </summary>
        /// <param name="sampleTag">The object tag to set</param>
        /// <param name="sample">The sample to emit</param>
        void LogSample<T>(DataFlowDiagnosticSampleType sampleTag, T sample);

        /// <summary>
        /// Diagnostic sample has been received
        /// </summary>
        event EventHandler<DiagnosticSampleEventArgs> SampleCollected;

    }

    /// <summary>
    /// Represetns a diagnostic session
    /// </summary>
    public interface IDataFlowDiagnosticSession
    {

        /// <summary>
        /// Diagnostic sample has been received
        /// </summary>
        event EventHandler<DiagnosticActionEventArgs> ActionStarted;

        /// <summary>
        /// Diagnostic sample has been received
        /// </summary>
        event EventHandler<DiagnosticActionEventArgs> ActionEnded;

        /// <summary>
        /// Gets the context to which this diagnostic session applies
        /// </summary>
        IDataFlowExecutionContext Context { get; }

        /// <summary>
        /// Log the starting of a stage
        /// </summary>
        /// <param name="flowStep">The stage identifier</param>
        IDataFlowDiagnosticAction LogStartAction(BiDataFlowStep flowStep);

        /// <summary>
        /// Log the end of the action
        /// </summary>
        void LogEndAction(IDataFlowDiagnosticAction action);

        /// <summary>
        /// Get the session data
        /// </summary>
        DataFlowDiagnosticReport GetSessionData();
    }

}