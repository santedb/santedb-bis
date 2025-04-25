/*
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

namespace SanteDB.BI.Datamart.DataFlow.Executors
{
    /// <summary>
    /// Represents a connection to the database
    /// </summary>
    internal class ConnectionExecutor : IDataFlowStepExecutor
    {
        /// <inheritdoc/>
        public Type Handles => typeof(BiDataFlowConnectionStep);

        /// <inheritdoc/>
        IEnumerable<dynamic> IDataFlowStepExecutor.Execute(BiDataFlowStep flowStep, DataFlowScope scope)
        {
            if (!(flowStep is BiDataFlowConnectionStep bcs))
            {
                throw new ArgumentOutOfRangeException(nameof(flowStep), String.Format(ErrorMessages.ARGUMENT_INCOMPATIBLE_TYPE, this.Handles.FullName, flowStep.GetType().FullName));
            }

            var myAction = scope.Context.DiagnosticSession?.LogStartAction(flowStep);
            try
            {

                if (!scope.TryGetSysVar<IDataIntegrator>(bcs.Name, out IDataIntegrator dataIntegrator))
                {
                    try
                    {
                        // We want to define a connection for a data integrator
                        dataIntegrator = scope.Context.GetIntegrator(bcs.DataSource);
                        if (bcs.Mode == BiDataFlowConnectionMode.ReadWrite)
                        {
                            dataIntegrator.OpenWrite();
                        }
                        else
                        {
                            dataIntegrator.OpenRead();
                        }
                        scope.SetSysVar(bcs.Name, dataIntegrator);
                    }
                    catch (Exception e)
                    {
                        throw new DataFlowException(flowStep, e);
                    }
                }
                yield return dataIntegrator;

            }
            finally
            {
                scope.Context.DiagnosticSession?.LogEndAction(myAction);
            }
        }
    }
}
