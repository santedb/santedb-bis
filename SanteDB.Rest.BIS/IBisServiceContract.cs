﻿/*
 * Copyright (C) 2019 - 2021, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE.md)
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
 * Date: 2021-2-9
 */
using RestSrvr.Attributes;
using SanteDB.BI.Model;
using SanteDB.Core.Interop;
using SanteDB.Rest.Common.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SanteDB.Rest.BIS
{
    /// <summary>
    /// Represents a service contract
    /// </summary>
    [ServiceContract(Name = "BIS")]
    [ServiceKnownResource(typeof(BiPackage))]
    [ServiceKnownResource(typeof(BiDataSourceDefinition))]
    [ServiceKnownResource(typeof(BiParameterDefinition))]
    [ServiceKnownResource(typeof(BiQueryDefinition))]
    [ServiceKnownResource(typeof(BiReportDefinition))]
    [ServiceKnownResource(typeof(BiViewDefinition))]
    [ServiceKnownResource(typeof(BiRenderFormatDefinition))]
    [ServiceProduces("application/json")]
    [ServiceProduces("application/xml")]
    public interface IBisServiceContract
    {


        /// <summary>
        /// Renders the specified BIS view in the specified format
        /// </summary>
        /// <param name="id"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        [Get("/Report/{format}/{id}")]
        Stream RenderReport(String id, String format);

        /// <summary>
        /// Executes the specified query 
        /// </summary>
        [Get("/Query/{id}")]
        IEnumerable<dynamic> RenderQuery(String id);

        /// <summary>
        /// Options for this instance of the BIS service
        /// </summary>
        [RestInvoke("OPTIONS", "/")]
        ServiceOptions Options();

        /// <summary>
        /// Pings the service to ensure that it is available on the current endpoint
        /// </summary>
        [RestInvoke("PING", "/")]
        void Ping();
        
        /// <summary>
        /// Searches the specified resource at endpoint
        /// </summary>
        [Get("/{resourceType}")]
        BiDefinitionCollection Search(String resourceType);

        /// <summary>
        /// Gets the specified resource at the endpoint
        /// </summary>
        /// <returns></returns>
        [Get("/{resourceType}/{id}")]
        BiDefinition Get(String resourceType, String id);

        /// <summary>
        /// Creates a new instance of a BIS resource
        /// </summary>
        [Post("/{resourceType}")]
        BiDefinition Create(String resourceType, BiDefinition body);

        /// <summary>
        /// Deletes the specified BIS resource
        /// </summary>
        [Delete("/{resourceType}/{id}")]
        BiDefinition Delete(String resourceType, String id);

        /// <summary>
        /// Updates the specified BIS resource
        /// </summary>
        [Put("/{resourceType}/{id}")]
        BiDefinition Update(String resourceType, String id, BiDefinition body);

    }
}
