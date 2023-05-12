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
    /// Registers a datamart operation
    /// </summary>
    public class RegisterDatamartOperation : IApiChildOperation
    {
        private readonly IBiMetadataRepository m_metadataRepository;
        private readonly IBiDatamartRepository m_datamartRepository;
        private readonly IJobManagerService m_jobManager;

        /// <summary>
        /// Datamart repository
        /// </summary>
        public RegisterDatamartOperation(IBiMetadataRepository biMetadataRepository, IBiDatamartRepository datamartRepository, IBiDatamartManager datamartManager, IJobManagerService jobManagerService)
        {
            this.m_metadataRepository = biMetadataRepository;
            this.m_datamartRepository = datamartRepository;
            this.m_jobManager = jobManagerService;
        }

        /// <inheritdoc/>
        public string Name => "register";

        /// <inheritdoc/>
        public ChildObjectScopeBinding ScopeBinding => ChildObjectScopeBinding.Instance;

        /// <inheritdoc/>
        public Type[] ParentTypes => new Type[] { typeof(BiDatamartDefinition) };

        /// <inheritdoc/>
        public object Invoke(Type scopingType, object scopingKey, ParameterCollection parameters)
        {
            if (scopingType == typeof(BiDatamartDefinition) && scopingKey is String martId)
            {
                var definition = this.m_metadataRepository.Get<BiDatamartDefinition>(martId);
                if (definition == null)
                {
                    throw new KeyNotFoundException(martId);
                }
                var registry = this.m_datamartRepository.Register(definition);

                var job = this.m_jobManager.GetJobInstance(BiDatamartJob.JOBID);
                if (job != null)
                {
                    this.m_jobManager.StartJob(job, new object[]
                    {
                        false, martId
                    });
                }
                return new DatamartInfo(registry);
            }
            else
            {
                throw new ArgumentOutOfRangeException(String.Format(ErrorMessages.ARGUMENT_INCOMPATIBLE_TYPE, typeof(BiDatamartDefinition), scopingType));
            }
        }
    }
}
