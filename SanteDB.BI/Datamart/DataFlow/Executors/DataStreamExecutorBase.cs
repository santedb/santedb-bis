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
using SanteDB.BI.Exceptions;
using SanteDB.BI.Model;
using SanteDB.Core.i18n;
using System;
using System.Collections.Generic;

namespace SanteDB.BI.Datamart.DataFlow.Executors
{
    /// <summary>
    /// Generic data stream executor
    /// </summary>
    internal abstract class DataStreamExecutorBase<TStreamStep> : IDataFlowStepExecutor
        where TStreamStep : BiDataFlowStreamStep
    {

        /// <inheritdoc/>
        public Type Handles => typeof(TStreamStep);

        /// <inheritdoc/>
        public IEnumerable<dynamic> Execute(BiDataFlowStep flowStep, DataFlowScope scope)
        {
            if (!(flowStep is TStreamStep bss) || !this.Handles.IsAssignableFrom(flowStep.GetType()))
            {
                throw new ArgumentOutOfRangeException(nameof(flowStep), String.Format(ErrorMessages.ARGUMENT_INCOMPATIBLE_TYPE, this.Handles.FullName, flowStep.GetType().FullName));
            }

            try
            {
                scope.Context.Log(System.Diagnostics.Tracing.EventLevel.Verbose, flowStep.FormatExecutionPlan());
                var inputStream = bss.InputObject.ResolveReferenceTo<BiDataFlowStep>(scope).Execute(scope);
                return this.ProcessStream(bss, scope, inputStream);
            }
            catch (Exception e)
            {
                throw new DataFlowException(flowStep, e);
            }
        }

        /// <summary>
        /// Create a stream tuple
        /// </summary>
        protected DataFlowStreamTuple CreateStreamTuple(object tuple)
        {
            switch (tuple)
            {
                case DataFlowStreamTuple tup:
                    return tup;
                case IDictionary<String, Object> dict:
                    return new DataFlowStreamTuple(dict);
                default:
                    throw new ArgumentOutOfRangeException(nameof(tuple));
            }

        }

        /// <summary>
        /// Perform the <paramref name="flowStep"/> for the <paramref name="inputStream"/>
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<dynamic> ProcessStream(TStreamStep flowStep, DataFlowScope scope, IEnumerable<dynamic> inputStream);
    }
}