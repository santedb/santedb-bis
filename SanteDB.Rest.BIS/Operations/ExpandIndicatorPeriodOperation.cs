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
 * Date: 2025-1-11
 */
using Newtonsoft.Json;
using SanteDB.BI;
using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.Core.Interop;
using SanteDB.Core.Model.Parameters;
using SanteDB.Rest.Common;
using SharpCompress;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using ZstdSharp.Unsafe;

namespace SanteDB.Rest.BIS.Operations
{
    /// <summary>
    /// Expands a <see cref="BiIndicatorPeriodDefinition"/> 
    /// </summary>
    public class ExpandIndicatorPeriodOperation : IApiChildOperation
    {

        /// <summary>
        /// Period information
        /// </summary>
        [XmlRoot(nameof(BiPeriodInfo), Namespace = BiConstants.XmlNamespace)]
        [XmlType(nameof(BiPeriodInfo), Namespace = BiConstants.XmlNamespace)]
        public class BiPeriodInfo
        {
            public BiPeriodInfo()
            {
            }

            public BiPeriodInfo(BiIndicatorPeriod period)
            {
                this.Start = period.Start;
                this.End = period.End;
                this.Index = period.Index;
            }

            [XmlAttribute("start"), JsonProperty("start")]
            public DateTime Start { get; set; }

            [XmlAttribute("end"), JsonProperty("end")]
            public DateTime End { get; set; }
            
            [XmlAttribute("index"), JsonProperty("index")]
            public long Index { get; set; }
        }

        public const string FROM_PARAMETER_NAME = "from";
        public const string TO_PARAMETER_NAME = "to";
        public const string COUNT_PARAMETER_NAME = "count";
        private readonly IBiMetadataRepository m_metaDataRepository;

        /// <summary>
        /// DI ctor
        /// </summary>
        public ExpandIndicatorPeriodOperation(IBiMetadataRepository biMetadataRepository)
        {
            this.m_metaDataRepository = biMetadataRepository;
        }

        /// <inheritdoc/>
        public string Name => "expand";

        /// <inheritdoc/>
        public ChildObjectScopeBinding ScopeBinding => ChildObjectScopeBinding.Instance;

        /// <inheritdoc/>
        public Type[] ParentTypes => new Type[] { typeof(BiIndicatorPeriodDefinition) };

        /// <inheritdoc/>
        public object Invoke(Type scopingType, object scopingKey, ParameterCollection parameters)
        {
            var keyString = scopingKey.ToString();
            var definition = this.m_metaDataRepository.Get<BiIndicatorPeriodDefinition>(keyString);
            if(definition == null)
            {
                throw new KeyNotFoundException(keyString);
            }

            bool fromSpecified = parameters.TryGet(FROM_PARAMETER_NAME, out DateTime from),
                toSpecified = parameters.TryGet(TO_PARAMETER_NAME, out DateTime to),
                countSpecified = parameters.TryGet(COUNT_PARAMETER_NAME, out int count);

            if(fromSpecified && countSpecified)
            {
                return definition.GetPeriods(from, count).Select(o=> new BiPeriodInfo(o)).ToList();
            }
            else if(fromSpecified && toSpecified)
            {
                return definition.GetPeriods(from, to).Select(o => new BiPeriodInfo(o)).ToList();
            }
            else
            {
                throw new ArgumentException(TO_PARAMETER_NAME);
            }
        }
    }
}
