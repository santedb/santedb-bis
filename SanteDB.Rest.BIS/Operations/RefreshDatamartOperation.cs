﻿/*
 * Copyright (C) 2021 - 2024, SanteSuite Inc. and the SanteSuite Contributors (See NOTICE.md for full copyright notices)
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
 */
using SanteDB.BI.Datamart;
using SanteDB.BI.Jobs;
using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.Core.i18n;
using SanteDB.Core.Interop;
using SanteDB.Core.Jobs;
using SanteDB.Core.Model.Parameters;
using SanteDB.Rest.Common;
using System;
using System.Collections.Generic;

namespace SanteDB.Rest.BIS.Operations
{
    /// <summary>
    /// Refreshes a registered 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class RefreshDatamartOperation : IApiChildOperation
    {
        private readonly IBiMetadataRepository m_metadataRepository;
        private readonly IBiDatamartRepository m_datamartRepository;
        private readonly IJobManagerService m_jobManager;

        /// <summary>
        /// Datamart repository
        /// </summary>
        public RefreshDatamartOperation(IBiMetadataRepository biMetadataRepository, IBiDatamartRepository datamartRepository, IJobManagerService jobManagerService)
        {
            this.m_metadataRepository = biMetadataRepository;
            this.m_datamartRepository = datamartRepository;
            this.m_jobManager = jobManagerService;
        }

        /// <inheritdoc/>
        public string Name => "refresh";

        /// <inheritdoc/>
        public ChildObjectScopeBinding ScopeBinding => ChildObjectScopeBinding.Instance;

        /// <inheritdoc/>
        public Type[] ParentTypes => new Type[] { typeof(BiDatamartDefinition) };

        /// <inheritdoc/>
        public object Invoke(Type scopingType, object scopingKey, ParameterCollection parameters)
        {
            if (scopingType == typeof(BiDatamartDefinition) && scopingKey is String martId)
            {
                var job = this.m_jobManager.GetJobInstance(BiDatamartJob.JOBID);
                if (job != null)
                {
                    this.m_jobManager.StartJob(job, new object[]
                    {
                        false, martId
                    });
                }
                return null;
            }
            else
            {
                throw new ArgumentOutOfRangeException(String.Format(ErrorMessages.ARGUMENT_INCOMPATIBLE_TYPE, typeof(BiDatamartDefinition), scopingType));
            }
        }
    }
}
