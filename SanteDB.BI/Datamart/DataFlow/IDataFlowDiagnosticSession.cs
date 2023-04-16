using SanteDB.BI.Model;
using System;

namespace SanteDB.BI.Datamart.DataFlow
{

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
        /// Log a sample
        /// </summary>
        /// <param name="sampleTag">The object tag to set</param>
        /// <param name="sample">The sample to emit</param>
        void LogSample<T>(DataFlowDiagnosticSampleType sampleTag, T sample);

    }

    /// <summary>
    /// Represetns a diagnostic session
    /// </summary>
    public interface IDataFlowDiagnosticSession
    {

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