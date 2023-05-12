using SanteDB.BI.Model;
using System;
using System.Collections.Generic;

namespace SanteDB.BI.Datamart.DataFlow
{
    /// <summary>
    /// Represents an execution class for the various <see cref="BI.Model.BiDataFlowStreamStep"/>
    /// </summary>
    internal interface IDataFlowStepExecutor
    {

        /// <summary>
        /// Gets the type of streaming step this executor handles
        /// </summary>
        Type Handles { get; }

        /// <summary>
        /// Execute the <paramref name="flowStep"/> using <paramref name="input"/> as a source of data
        /// </summary>
        /// <param name="flowStep">The flow step to be executed</param>
        /// <param name="executionContext">The current execution context</param>
        /// <param name="scope">The current scope of the context</param>
        /// <returns>The transformed or input data objects</returns>
        IEnumerable<dynamic> Execute(BiDataFlowStep flowStep, DataFlowScope scope);

    }
}
