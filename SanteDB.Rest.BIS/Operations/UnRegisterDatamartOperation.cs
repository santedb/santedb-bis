using DocumentFormat.OpenXml.Wordprocessing;
using SanteDB.BI.Datamart;
using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.Core.i18n;
using SanteDB.Core.Interop;
using SanteDB.Core.Model.Parameters;
using SanteDB.Rest.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SanteDB.Rest.BIS.Operations
{
    /// <summary>
    /// Un-Registers a datamart operation
    /// </summary>
    public class UnRegisterDatamartOperation : IApiChildOperation
    {
        private readonly IBiMetadataRepository m_metadataRepository;
        private readonly IBiDatamartRepository m_datamartRepository;
        private readonly IBiDatamartManager m_datamartMaanger;

        /// <summary>
        /// Datamart repository
        /// </summary>
        public UnRegisterDatamartOperation(IBiMetadataRepository biMetadataRepository, IBiDatamartRepository datamartRepository, IBiDatamartManager datamartManager)
        {
            this.m_metadataRepository = biMetadataRepository;
            this.m_datamartRepository = datamartRepository;
            this.m_datamartMaanger = datamartManager;
        }

        /// <inheritdoc/>
        public string Name => "unregister";

        /// <inheritdoc/>
        public ChildObjectScopeBinding ScopeBinding => ChildObjectScopeBinding.Instance;

        /// <inheritdoc/>
        public Type[] ParentTypes => new Type[] { typeof(BiDatamartDefinition) };

        /// <inheritdoc/>
        public object Invoke(Type scopingType, object scopingKey, ParameterCollection parameters)
        {
            if(scopingType == typeof(BiDatamartDefinition) && scopingKey is String martId)
            {
                var definition = this.m_metadataRepository.Get<BiDatamartDefinition>(martId);
                if(definition == null)
                {
                    throw new KeyNotFoundException(martId);
                }
                var registry = this.m_datamartRepository.Find(o => o.Id == martId).FirstOrDefault();
                this.m_datamartMaanger.Destroy(definition);
                this.m_datamartRepository.Unregister(registry);
                return new DatamartInfo(registry);
            }
            else
            {
                throw new ArgumentOutOfRangeException(String.Format(ErrorMessages.ARGUMENT_INCOMPATIBLE_TYPE, typeof(BiDatamartDefinition), scopingType));
            }
        }
    }
}
