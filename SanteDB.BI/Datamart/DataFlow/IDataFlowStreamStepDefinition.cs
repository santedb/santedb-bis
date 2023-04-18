using SanteDB.BI.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanteDB.BI.Datamart.DataFlow
{
    /// <summary>
    /// Represents a data flow step definition which is stremaing in nature
    /// </summary>
    internal interface IDataFlowStreamStepDefinition
    {

        /// <summary>
        /// Gets the input step
        /// </summary>
        BiDataFlowStep InputStep { get; }

    }

    /// <summary>
    /// Multiple stream input
    /// </summary>
    internal interface IDataFlowMultiStreamStepDefinition : IDataFlowStreamStepDefinition
    {
        /// <summary>
        /// Gets the input step
        /// </summary>
        IEnumerable<BiDataFlowStep> InputSteps { get; }
    }
}
