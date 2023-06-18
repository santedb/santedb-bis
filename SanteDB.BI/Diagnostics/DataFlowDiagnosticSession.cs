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
using SanteDB.BI.Datamart.DataFlow;
using SanteDB.BI.Model;
using SanteDB.Core.i18n;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SanteDB.BI.Diagnostics
{

    /// <summary>
    /// Diagnostic sample
    /// </summary>
    internal class DataFlowDiagnosticSample
    {

        /// <summary>
        /// Create new diagnostic information
        /// </summary>
        public DataFlowDiagnosticSample(DataFlowDiagnosticSampleType type, Object value)
        {
            this.Type = type;
            this.Value = value;
            this.Timestamp = DateTimeOffset.Now;
        }

        /// <summary>
        /// Gets the time the sample was taken
        /// </summary>
        public DateTimeOffset Timestamp { get; }

        /// <summary>
        /// Gets the type of sample
        /// </summary>
        public DataFlowDiagnosticSampleType Type { get; set; }

        /// <summary>
        /// Gets the value of the sample
        /// </summary>
        public Object Value { get; set; }
    }

    /// <summary>
    /// Diagnostic information about a start or stop
    /// </summary>
    internal class DataFlowDiagnosticActionInfo : IDataFlowDiagnosticAction
    {

        // samples
        private readonly IDictionary<DataFlowDiagnosticSampleType, DataFlowDiagnosticSample> m_samples = new ConcurrentDictionary<DataFlowDiagnosticSampleType, DataFlowDiagnosticSample>();
        private readonly List<DataFlowDiagnosticActionInfo> m_children;

        /// <inheritdoc/>
        public event EventHandler<DiagnosticSampleEventArgs> SampleCollected;

        /// <summary>
        /// Data flow diagnostic action
        /// </summary>
        public DataFlowDiagnosticActionInfo(BiDataFlowStep flowStep, DataFlowDiagnosticActionInfo parentAction)
        {
            this.StepType = flowStep.GetType().GetSerializationName();
            this.Name = flowStep.Name;
            this.StartOfAction = DateTimeOffset.Now;
            this.m_children = new List<DataFlowDiagnosticActionInfo>();
            this.Parent = parentAction;
            this.Uuid = Guid.NewGuid();
            this.DebugInfo = flowStep.ToString();
        }

        /// <summary>
        /// Get the UUID of the action
        /// </summary>
        internal Guid Uuid { get; }
        /// <summary>
        /// Gets the debug information
        /// </summary>
        public string DebugInfo { get; }

        /// <summary>
        /// The flow step type
        /// </summary>
        public String StepType { get; }

        /// <summary>
        /// The flow step name
        /// </summary>
        public String Name { get; }

        /// <summary>
        /// Gets the start of the action
        /// </summary>
        public DateTimeOffset StartOfAction { get; }

        /// <summary>
        /// Gets the end of the action
        /// </summary>
        public DateTimeOffset? EndOfAction { get; private set; }

        /// <summary>
        /// Gets the diagnostic samples
        /// </summary>
        public IEnumerable<DataFlowDiagnosticSample> Samples => this.m_samples.Values;

        /// <summary>
        /// Gets the child sample
        /// </summary>
        public IEnumerable<DataFlowDiagnosticActionInfo> Children => this.m_children;

        /// <summary>
        /// Gets the parent of this step
        /// </summary>
        public DataFlowDiagnosticActionInfo Parent { get; }

        /// <summary>
        /// Indicate the step has ended
        /// </summary>
        internal void End() => this.EndOfAction = DateTimeOffset.Now;

        /// <summary>
        /// Add a child action
        /// </summary>
        internal void AddChild(DataFlowDiagnosticActionInfo action) => this.m_children.Add(action);

        /// <summary>
        /// Add a sample
        /// </summary>
        public void LogSample<T>(DataFlowDiagnosticSampleType sampleType, T value)
        {
            if (sampleType == DataFlowDiagnosticSampleType.LoggedData)
            {
                if (!this.m_samples.TryGetValue(DataFlowDiagnosticSampleType.LoggedData, out var logSample))
                {
                    this.m_samples.Add(DataFlowDiagnosticSampleType.LoggedData, new DataFlowDiagnosticSample(DataFlowDiagnosticSampleType.LoggedData, new List<T>() { value }));
                }
                else if (logSample.Value is List<T> list)
                {
                    list.Add(value);
                }
                else
                {
                    throw new InvalidOperationException("Cannot log sample of different types");
                }
            }
            else if (this.m_samples.TryGetValue(sampleType, out var sample))
            {
                sample.Value = value;
            }
            else
            {
                this.m_samples.Add(sampleType, new DataFlowDiagnosticSample(sampleType, value));
            }

            this.SampleCollected?.Invoke(this, new DiagnosticSampleEventArgs(sampleType, value));

        }
    }

    /// <summary>
    /// Represents a dataflow diagnostic session
    /// </summary>
    public class DataFlowDiagnosticSession : IDataFlowDiagnosticSession
    {


        private DataFlowDiagnosticActionInfo m_currentAction;
        private LinkedList<DataFlowDiagnosticActionInfo> m_completedActions = new LinkedList<DataFlowDiagnosticActionInfo>();
        private readonly DateTimeOffset m_createdTime = DateTimeOffset.Now;

        /// <summary>
        /// Creates a new data flow diagnostic session
        /// </summary>
        public DataFlowDiagnosticSession(IDataFlowExecutionContext context)
        {
            this.Context = context;
        }

        /// <inheritdoc/>
        public IDataFlowExecutionContext Context { get; }

        /// <inheritdoc/>
        public event EventHandler<DiagnosticActionEventArgs> ActionStarted;
        /// <inheritdoc/>
        public event EventHandler<DiagnosticActionEventArgs> ActionEnded;

        /// <inheritdoc/>
        public DataFlowDiagnosticReport GetSessionData()
        {
            return new DataFlowDiagnosticReport(this.m_createdTime.DateTime, this.m_completedActions);
        }

        /// <inheritdoc/>
        public void LogEndAction(IDataFlowDiagnosticAction expectedAction)
        {
            if (!(this.m_currentAction is DataFlowDiagnosticActionInfo actionInfo) ||
                actionInfo.Uuid != this.m_currentAction.Uuid)
            {
                throw new InvalidOperationException(String.Format(ErrorMessages.WOULD_RESULT_INVALID_STATE, nameof(LogEndAction)));
            }

            this.m_currentAction.End();

            // Ended?
            if (this.m_currentAction.Parent == null)
            {
                this.m_completedActions.AddLast(this.m_currentAction);
            }

            this.m_currentAction = this.m_currentAction.Parent;
            this.ActionEnded?.Invoke(this, new DiagnosticActionEventArgs(expectedAction));
        }


        /// <inheritdoc/>
        public IDataFlowDiagnosticAction LogStartAction(BiDataFlowStep flowStep)
        {
            var childAction = new DataFlowDiagnosticActionInfo(flowStep, this.m_currentAction);
            this.m_currentAction?.AddChild(childAction);
            this.m_currentAction = childAction;
            this.ActionStarted?.Invoke(this, new DiagnosticActionEventArgs(childAction));

            return this.m_currentAction;
        }

    }
}
