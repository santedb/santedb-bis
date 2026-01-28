/*
 * Copyright (C) 2021 - 2026, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2024-6-21
 */
using SanteDB.BI.Datamart;
using SanteDB.BI.Datamart.DataFlow;
using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.Core.i18n;
using SanteDB.Core.Interop;
using SanteDB.Core.Model.Parameters;
using SanteDB.Rest.Common;
using System;
using System.Linq;

namespace SanteDB.Rest.BIS.Operations
{
    /// <summary>
    /// Collect dataset execution log
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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
            if (scopingType == typeof(DatamartInfo) && scopingKey is String martId)
            {
                var dataMart = this.m_datamartRepository.Find(o => o.Id == martId).First();

                // Specific execution or last?
                IDataFlowExecutionEntry entryForCollection = null;
                if (parameters.TryGet("execution", out Guid executionId))
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
