using SanteDB.BI.Datamart.DataFlow;
using SanteDB.BI.Model;
using SanteDB.Core.i18n;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

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
        }

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
            if (sampleType.HasFlag(DataFlowDiagnosticSampleType.PointInTime)) // replace
            {
                var originalSample = sampleType ^ DataFlowDiagnosticSampleType.PointInTime;
                if (this.m_samples.TryGetValue(originalSample, out var sample))
                {
                    sample.Value = value;
                }
                else
                {
                    this.m_samples.Add(originalSample, new DataFlowDiagnosticSample(sampleType, value));
                }
            }
            else if(this.m_samples.TryGetValue(sampleType, out var sample))
            {
                sample.Value = value;
            }
            else
            {
                this.m_samples.Add(sampleType, new DataFlowDiagnosticSample(sampleType, value));
            }
        }
    }

    /// <summary>
    /// Represents a dataflow diagnostic session
    /// </summary>
    public class DataFlowDiagnosticSession : IDataFlowDiagnosticSession
    {


        private DataFlowDiagnosticActionInfo m_currentAction;

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
        public object GetSessionData()
        {
            var sd = this.m_currentAction;
            while(sd.Parent != null)
            {
                sd = sd.Parent;
            }
            return sd;
        }

        /// <inheritdoc/>
        public void LogEndAction(IDataFlowDiagnosticAction expectedAction)
        {
            if(this.m_currentAction == null)
            {
                throw new InvalidOperationException(String.Format(ErrorMessages.WOULD_RESULT_INVALID_STATE, nameof(LogEndAction)));
            }

            this.m_currentAction.End();
            this.m_currentAction = this.m_currentAction.Parent;
            if(this.m_currentAction != expectedAction && this.m_currentAction != null)
            {
                this.LogEndAction(expectedAction);
            }
        }


        /// <inheritdoc/>
        public IDataFlowDiagnosticAction LogStartAction(BiDataFlowStep flowStep)
        {
            var childAction = new DataFlowDiagnosticActionInfo(flowStep, this.m_currentAction);
            this.m_currentAction?.AddChild(childAction);
            this.m_currentAction = childAction;
            return this.m_currentAction;
        }
    }
}
