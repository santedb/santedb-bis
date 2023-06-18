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
using SanteDB.BI.Model;
using System;
using System.Collections.Generic;

namespace SanteDB.BI.Datamart
{
    /// <summary>
    /// Implementers are responsible for binding the BI layer to a persistence layer (such as the ORM)
    /// </summary>
    public interface IDataIntegrator : IDisposable
    {

        /// <summary>
        /// Gets the data source
        /// </summary>
        BiDataSourceDefinition DataSource { get; }

        /// <summary>
        /// Open the data integrator connection for reading
        /// </summary>
        void OpenRead();

        /// <summary>
        /// Open the data integrator connection for writing
        /// </summary>
        void OpenWrite();

        /// <summary>
        /// Close the current context
        /// </summary>
        void Close();

        /// <summary>
        /// Determines if the target database exists
        /// </summary>
        bool DatabaseExists();

        /// <summary>
        /// Creates the target database or data structure which the provider uses
        /// </summary>
        void CreateDatabase();

        /// <summary>
        /// Drops the target object, database or other structure
        /// </summary>
        void DropDatabase();

        /// <summary>
        /// Begin a transaction
        /// </summary>
        /// <returns>The transaction control object</returns>
        IDisposable BeginTransaction();

        /// <summary>
        /// Commit the transaction
        /// </summary>
        void CommitTransaction();

        /// <summary>
        /// Commit the transaction
        /// </summary>
        void RollbackTransaction();

        /// <summary>
        /// Returns true if the object exists in the metadata 
        /// </summary>
        bool Exists(BiSchemaObjectDefinition objectToCheck);

        /// <summary>
        /// True if the <paramref name="objectToCheck"/> needs to be updated
        /// </summary>
        bool NeedsMigration(BiSchemaObjectDefinition objectToCheck);

        /// <summary>
        /// Creates <paramref name="objectToCreate"/> in the database
        /// </summary>
        /// <param name="objectToCreate">The object definition to be created</param>
        void RecreateObject(BiSchemaObjectDefinition objectToCreate);

        /// <summary>
        /// Truncate <paramref name="objectToTruncate"/>
        /// </summary>
        /// <param name="objectToTruncate">The object whose data is to be truncated</param>
        void TruncateObject(BiSchemaObjectDefinition objectToTruncate);

        /// <summary>
        /// Drop the <paramref name="objectToDrop"/> from the database
        /// </summary>
        /// <param name="objectToDrop">The object to be dropped</param>
        void DropObject(BiSchemaObjectDefinition objectToDrop);

        /// <summary>
        /// Execute a query <paramref name="queryToExecute"/> returning the results
        /// </summary>
        /// <param name="queryToExecute">The SQL to execute</param>
        /// <param name="outputSchema">The output schema</param>
        /// <returns>The results loaded from the database</returns>
        IEnumerable<dynamic> Query(IEnumerable<BiSqlDefinition> queryToExecute, BiSchemaTableDefinition outputSchema = null);

        /// <summary>
        /// Inserts the specified <paramref name="dataToInsert"/> in <paramref name="target"/>
        /// </summary>
        /// <param name="target">The target object where data should be manipulated</param>
        /// <param name="dataToInsert">The data to be inserted</param>
        /// <returns>The inserted data</returns>
        dynamic Insert(BiSchemaTableDefinition target, dynamic dataToInsert);

        /// <summary>
        /// Inserts or updates the <paramref name="dataToInsert"/> in <paramref name="target"/>
        /// </summary>
        /// <param name="target">The target schema object where the data should be inserted or updated</param>
        /// <param name="dataToInsert">The data to insert or update</param>
        /// <returns>The updated data</returns>
        dynamic InsertOrUpdate(BiSchemaTableDefinition target, dynamic dataToInsert);

        /// <summary>
        /// Update the <paramref name="dataToUpdate"/> in <paramref name="target"/>
        /// </summary>
        /// <param name="target">Where data should be updated</param>
        /// <param name="dataToUpdate">The data which should be updated</param>
        /// <returns>The updated data</returns>
        dynamic Update(BiSchemaTableDefinition target, dynamic dataToUpdate);

        /// <summary>
        /// Deletes <paramref name="dataToDelete"/> from <paramref name="target"/>
        /// </summary>
        /// <param name="target">The schema object where data is to be deleted</param>
        /// <param name="dataToDelete">The data to be deleted</param>
        /// <returns>The deleted records </returns>
        dynamic Delete(BiSchemaTableDefinition target, dynamic dataToDelete);

        /// <summary>
        /// Execute a query and don't return the result
        /// </summary>
        /// <param name="sql">The sql definition</param>
        void ExecuteNonQuery(BiSqlDefinition sql);

    }
}