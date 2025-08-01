﻿/*
 * Copyright (C) 2021 - 2025, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 * Date: 2023-6-21
 */
using SanteDB.BI.Exceptions;
using SanteDB.BI.Model;
using SanteDB.Core.i18n;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SanteDB.BI.Datamart.DataFlow.Executors
{
    /// <summary>
    /// A transaction executor
    /// </summary>
    internal class TransactionExecutor : IDataFlowStepExecutor
    {
        /// <inheritdoc/>
        public Type Handles => typeof(BiDataFlowTransactionStep);

        /// <inheritdoc/>
        IEnumerable<dynamic> IDataFlowStepExecutor.Execute(BiDataFlowStep flowStep, DataFlowScope scope)
        {
            if (!(flowStep is BiDataFlowTransactionStep bts))
            {
                throw new ArgumentOutOfRangeException(nameof(flowStep), String.Format(ErrorMessages.ARGUMENT_INCOMPATIBLE_TYPE, this.Handles.FullName, flowStep.GetType().FullName));
            }

            var myAction = scope.Context.DiagnosticSession?.LogStartAction(flowStep);
            try
            {

                var dataSource = bts.InputConnection.ResolveReferenceTo<BiDataFlowStep>(scope).Execute(scope).First() as IDataIntegrator;
                try
                {
                    using (var newScope = new DataFlowScope(bts.Name, scope))
                    {
                        using (var tx = dataSource.BeginTransaction())
                        {


                            var executeRoot = bts.GetExecutionTreeRoot();
                            newScope.Context.Log(System.Diagnostics.Tracing.EventLevel.Verbose, bts.FormatExecutionPlan());
                            // Now we process the terminal objects and execute them
                            var processedRecords = executeRoot.SelectMany(o => o.Execute(newScope)).Count();
                            myAction?.LogSample(DataFlowDiagnosticSampleType.TotalRecordProcessed, processedRecords);
                            dataSource.CommitTransaction();
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new DataFlowException(flowStep, e);
                }
                yield break;


            }
            finally
            {
                scope.Context.DiagnosticSession?.LogEndAction(myAction);
            }

        }
    }
}
