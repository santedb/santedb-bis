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
 * Date: 2025-1-11
 */
using NUnit.Framework;
using SanteDB.BI.Model;
using SanteDB.BI.Services;
using SanteDB.BI.Util;
using SanteDB.Core;
using SanteDB.Core.TestFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.BI.Test
{
    [TestFixture]
    public class DocumentationTests
    {
        [OneTimeSetUp]
        public void Setup()
        {
            TestApplicationContext.TestAssembly = typeof(TestDatamart).Assembly;
            TestApplicationContext.Initialize(TestContext.CurrentContext.TestDirectory);

        }

        [Test]
        public void TestCreateWeeklyPeriod()
        {

            var repositoryService = ApplicationServiceContext.Current.GetService<IBiMetadataRepository>();
            var coreMart = repositoryService.Query<BiDatamartDefinition>(o => o.Id == "org.santedb.bi.datamart.core").FirstOrDefault();
            Assert.IsNotNull(coreMart);

            using (var ms = new MemoryStream())
            {
                coreMart.DocumentObject().CopyTo(ms);
                var html = Encoding.UTF8.GetString(ms.ToArray());
            }

        }
    }
}
