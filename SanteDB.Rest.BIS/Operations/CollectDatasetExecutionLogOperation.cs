using SanteDB.BI.Datamart;
using SanteDB.BI.Datamart.DataFlow;
using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.Core.i18n;
using SanteDB.Core.Interop;
using SanteDB.Core.Model.Parameters;
using SanteDB.Rest.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SanteDB.Rest.BIS.Operations
{
    /// <summary>
    /// Collect dataset execution log
    /// </summary>
    public class CollectDatasetExecutionLogOperation : IApiChildOperation
    {
        private readonly IBiDatamartRepository m_datamartRepository;

        /// <summary>
        /// DI ctor
        /// </summary>
        public CollectDatasetExecutionLogOperation(IBiDatamartRepository biDatamartRepository)
        {
            this.m_datamartRepository = biDatamartRepository;
        }

        /// <inheritdoc/>
        public string Name => "gather";

        /// <inheritdoc/>
        public ChildObjectScopeBinding ScopeBinding => ChildObjectScopeBinding.Instance;

        /// <inheritdoc/>
        public Type[] ParentTypes => new Type[] { typeof(DatamartInfo) };

        /// <inheritdoc/>
        public object Invoke(Type scopingType, object scopingKey, ParameterCollection parameters)
        {
            if(scopingType == typeof(DatamartInfo) && scopingKey is String martId)
            {
                var dataMart = this.m_datamartRepository.Find(o => o.Id == martId).First();

                // Specific execution or last?
                IDataFlowExecutionEntry entryForCollection = null;
                if(parameters.TryGet("execution", out Guid executionId))
                {
                    entryForCollection = dataMart.FlowExecutions.Where(o => o.Key == executionId).First();
                }
                else
                {
                    entryForCollection = dataMart.FlowExecutions.First();
                }

                var retVal = new DataMartExecutionInfo(entryForCollection);
                retVal.LogEntry = entryForCollection.LogEntries.Select(o => new DatamartLogEntry(o)).ToList();
                return retVal;
            }
            else
            {
                throw new ArgumentOutOfRangeException(String.Format(ErrorMessages.ARGUMENT_INCOMPATIBLE_TYPE, typeof(BiDatamartDefinition), scopingType));
            }
        }
    }
}
