/**************************************************************************
 * 
 * IDatabase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.Collections.Generic;
using Semantics.Data;
using Semantics.Utilities;

namespace Semantics.Interfaces
{

    #region Interface: IDatabase
    /// <summary>
    /// An interface for the database.
    /// </summary>
    public interface IDatabase
    {

        /// <summary>
        /// Create a new database with the given path/name.
        /// </summary>
        /// <param name="databaseType">The type of the database.</param>
        /// <param name="database">The database to create.</param>
        /// <param name="queryFile">The path and name of a file which contains SQL queries to create tables.</param>
        /// <returns>Returns whether the database has been created successfully.</returns>
        Message Create(DatabaseType databaseType, string database, string queryFile);

        /// <summary>
        /// Create a new database of the given type from the given query files.
        /// </summary>
        /// <param name="databaseType">The type of the database.</param>
        /// <param name="database">The database to create.</param>
        /// <param name="queryFiles">The path and name of a files which contains SQL queries to create tables.</param>
        /// <returns>Returns whether the database has been created successfully.</returns>
        Message Create(DatabaseType databaseType, string database, string[] queryFiles);

        /// <summary>
        /// Add the tables from the given query file to the given database of the given type.
        /// </summary>
        /// <param name="databaseType">The type of the database.</param>
        /// <param name="database">The database to add tables to.</param>
        /// <param name="queryFile">The path and name of a file which contains SQL queries to create tables.</param>
        /// <returns>Returns whether the tables have been added successfully.</returns>
        Message AddTables(DatabaseType databaseType, string database, string queryFile);

        /// <summary>
        /// Initialize the database of the given type.
        /// </summary>
        /// <param name="databaseType">The type of the database.</param>
        /// <param name="database">The database to initialize.</param>
        /// <returns>Returns whether the database has been initialized successfully.</returns>
        Message Initialize(DatabaseType databaseType, string database);

        /// <summary>
        /// Uninitialize the database.
        /// </summary>
        /// <returns>Returns whether the database had been uninitialized successfully.</returns>
        Message Uninitialize();

        /// <summary>
        /// Switch to the given values database.
        /// </summary>
        /// <param name="database">The values database to switch to.</param>
        /// <returns>Returns whether the values database has been switched successfully.</returns>
        Message SwitchToValuesDatabase(string database);

        /// <summary>
        /// Switch to the given localization database.
        /// </summary>
        /// <param name="database">The localization database to switch to.</param>
        /// <returns>Returns whether the localization database has been switched successfully.</returns>
        Message SwitchToLocalizationDatabase(string database);

        /// <summary>
        /// Save the databases.
        /// </summary>
        /// <returns>Returns whether the databases have been saved successfully.</returns>
        Message Save();

        /// <summary>
        /// Save the database as new database.
        /// </summary>
        /// <param name="database">The new database name.</param>
        /// <returns>Returns whether the databases have been saved successfully.</returns>
        Message SaveAs(string database);

        /// <summary>
        /// Remove the database of the given type.
        /// </summary>
        /// <param name="databaseType">The type of the database.</param>
        /// <param name="database">The database to remove.</param>
        /// <returns>Returns whether the database has been removed successfully.</returns>
        Message Remove(DatabaseType databaseType, string database);

        /// <summary>
        /// Delete the database of the given type. Will uninitialize the database first!
        /// </summary>
        /// <param name="databaseType">The type of the database.</param>
        /// <param name="database">The database to remove.</param>
        void Delete(DatabaseType databaseType, string database);

        /// <summary>
        /// Get all columns of the given table.
        /// </summary>
        /// <param name="databaseType">The database type.</param>
        /// <param name="table">The table to get the columns of.</param>
        /// <returns>The columns of the table.</returns>
        List<string> GetColumns(DatabaseType databaseType, string table);

        /// <summary>
        /// Query a string. In case of a localization database, only use the current one.
        /// </summary>
        /// <param name="databaseType">The database type.</param>
        /// <param name="query">The query.</param>
        /// <returns>Returns whether the query has been executed successfully.</returns>
        bool Query(DatabaseType databaseType, string query);

        /// <summary>
        /// Query a string on all databases.
        /// </summary>
        /// <param name="databaseType">The database type.</param>
        /// <param name="query">The query.</param>
        /// <returns>Returns whether the query has been executed successfully.</returns>
        bool QueryAll(DatabaseType databaseType, string query);

        /// <summary>
        /// Select the entry that corresponds to the query.
        /// </summary>
        /// <param name="databaseType">The database type.</param>
        /// <param name="query">The query.</param>
        /// <returns>Returns the entry that corresponds to the query.</returns>
        object QuerySelect(DatabaseType databaseType, string query);

        /// <summary>
        /// Select all entries that correspond to the query.
        /// </summary>
        /// <param name="databaseType">The database type.</param>
        /// <param name="query">The query.</param>
        /// <returns>Returns the entries that correspond to the query.</returns>
        List<object> QuerySelectAll(DatabaseType databaseType, string query);

        /// <summary>
        /// Select all entries that correspond to the query.
        /// </summary>
        /// <param name="databaseType">The database type.</param>
        /// <param name="query">The query.</param>
        /// <param name="query">The number of columns to consider in each row, starting from the first.</param>
        /// <returns>Returns the entries that correspond to the query.</returns>
        List<object[]> QuerySelectAllColumns(DatabaseType databaseType, string query, int columns);

        /// <summary>
        /// Begins a combined query. Until QueryCommit() is called, all queries will be temporarily stored, and not yet executed.
        /// </summary>
        void QueryBegin();

        /// <summary>
        /// Commits all temporarily stored queries. Use only after calling QueryBegin()!
        /// </summary>
        void QueryCommit();

        /// <summary>
        /// Begins a combined query. Until QueryAllCommit() is called, all queries will be temporarily stored, and not yet executed.
        /// </summary>
        void QueryAllBegin();

        /// <summary>
        /// Commits all temporarily stored queries. Use only after calling QueryAllBegin()!
        /// </summary>
        void QueryAllCommit();

    }
    #endregion Interface: IDatabase
		
}