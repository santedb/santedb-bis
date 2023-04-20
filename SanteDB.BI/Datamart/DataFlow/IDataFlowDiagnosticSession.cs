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
        /// The sample is a point in time sample - so don't persist
        /// </summary>
        PointInTime = 0x1,
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
        CurrentRecord = 0x8 | PointInTime
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
        object GetSessionData();
    }

}