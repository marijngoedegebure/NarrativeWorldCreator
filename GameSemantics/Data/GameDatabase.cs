/**************************************************************************
 * 
 * GameDatabase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;
using Common;
using GameSemantics.Utilities;
using Semantics.Components;
using Semantics.Data;
using Semantics.Utilities;

namespace GameSemantics.Data
{

    #region Class: GameDatabase
    /// <summary>
    /// The game database is able to load, modify, and save (the relations between) game objects.
    /// </summary>
    public class GameDatabase : Database
    {

        #region Properties and Fields

        #region Property: Current
        /// <summary>
        /// Gets the current game database. Make sure to call Initialize() first!
        /// </summary>
        public static new GameDatabase Current
        {
            get
            {
                return Database.Current as GameDatabase;
            }
        }
        #endregion Property: Current

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: GameDatabase()
        /// <summary>
        /// Creates a new game database.
        /// </summary>
        protected GameDatabase()
        {
            // Make the database aware of table names and node types
            AddSemanticsResourceSet(GameTables.ResourceManager.GetResourceSet(CommonSettings.Culture, true, true));
            AddValuesResourceSet(GameValueTables.ResourceManager.GetResourceSet(CommonSettings.Culture, true, true));
            AddNamesResourceManager(GameSemantics.Utilities.NewNames.ResourceManager);
            AddAssembly(Assembly.GetExecutingAssembly());
        }
        #endregion Constructor: GameDatabase()

        #region Method: Initialize()
        /// <summary>
        /// Initialize the game database.
        /// </summary>
        public static new void Initialize()
        {
            if (Database.Current == null)
            {
                // Make this game database the current database
                Database.Current = new GameDatabase();
            }
        }
        #endregion Method: Initialize()

        #endregion Method Group: Constructors

        #region Method Group: Projects

        #region Method: CreateProject(String file)
        /// <summary>
        /// Create a new database project with the given file name.
        /// </summary>
        /// <param name="file">The database project file to create.</param>
        /// <returns>Returns whether the database project has been created successfully.</returns>
        public static new Message CreateProject(String file)
        {
            // Initialize the database when this has not been done before
            if (Current == null)
                Initialize();

            return Database.CreateProject(file);
        }
        #endregion Method: CreateProject(String file)

        #region Method: LoadProject(String file)
        /// <summary>
        /// Load the database project with the given file name.
        /// </summary>
        /// <param name="file">The database project file to load.</param>
        /// <returns>Returns whether the database project has been loaded successfully.</returns>
        public static new Message LoadProject(String file)
        {
            // Initialize the database when this has not been done before
            if (Current == null)
                Initialize();

            return Database.LoadProject(file);
        }
        #endregion Method: LoadProject(String file)

        #region Method: CreateSemanticsDatabase(String name)
        /// <summary>
        /// Create a new semantics database with the given name.
        /// </summary>
        /// <param name="name">The semantics database to create.</param>
        /// <returns>Returns whether the database has been created successfully.</returns>
        protected override Message CreateSemanticsDatabase(String name)
        {
            if (name != null)
            {
                Message message = this.IDatabase.Create(DatabaseType.Semantics, name, new String[] { SemanticsSettings.Files.DatabasePath + SemanticsSettings.Files.SemanticsSQLFile, SemanticsSettings.Files.DatabasePath + GameSemanticsSettings.Files.SemanticsSQLFile });

                if (message == Message.CreationSuccess)
                    AddDatabaseToProject(DatabaseType.Semantics, name);

                return message;
            }

            return Message.CreationFail;
        }
        #endregion Method: CreateSemanticsDatabase(String name)

        #region Method: CreateValuesDatabase(String name)
        /// <summary>
        /// Create a new values database with the given name.
        /// </summary>
        /// <param name="name">The values database to create.</param>
        /// <returns>Returns whether the database has been created successfully.</returns>
        public override Message CreateValuesDatabase(String name)
        {
            if (name != null)
            {
                Message message = this.IDatabase.Create(DatabaseType.Values, name, new String[] { SemanticsSettings.Files.DatabasePath + SemanticsSettings.Files.ValuesSQLFile, SemanticsSettings.Files.DatabasePath + GameSemanticsSettings.Files.ValuesSQLFile });

                if (message == Message.CreationSuccess)
                    AddDatabaseToProject(DatabaseType.Values, name);

                return message;
            }

            return Message.CreationFail;
        }
        #endregion Method: CreateValuesDatabase(String name)

        #region Method: CreateLocalizationDatabase(String name)
        /// <summary>
        /// Create a new localization database with the given name.
        /// </summary>
        /// <param name="name">The localization database to create.</param>
        /// <returns>Returns whether the database has been created successfully.</returns>
        public override Message CreateLocalizationDatabase(String name)
        {
            if (name != null)
            {
                Message message = this.IDatabase.Create(DatabaseType.Localization, name, new String[] { SemanticsSettings.Files.DatabasePath + SemanticsSettings.Files.LocalizationSQLFile, SemanticsSettings.Files.DatabasePath + GameSemanticsSettings.Files.LocalizationSQLFile });

                if (message == Message.CreationSuccess)
                    AddDatabaseToProject(DatabaseType.Localization, name);

                return message;
            }

            return Message.CreationFail;
        }
        #endregion Method: CreateLocalizationDatabase(String name)

        #region Method: UpgradeToGameSemanticsProject(String file)
        /// <summary>
        /// Upgrade the semantics project from the given file to a game semantics project.
        /// </summary>
        /// <param name="file">The database project file to upgrade.</param>
        /// <returns>Returns whether the database project has been upgraded successfully.</returns>
        public static Message UpgradeToGameSemanticsProject(String file)
        {
            if (file != null)
            {
                try
                {
                    // Get the full project file
                    string project = GetFullProjectPath(file);

                    if (File.Exists(project))
                    {
                        // Load the project file
                        XmlDocument doc = new XmlDocument();
                        doc.Load(project);

                        // Go through the file and read the database names
                        String semanticsDatabase = null;
                        List<String> valuesDatabases = new List<String>();
                        List<String> localizationDatabases = new List<String>();
                        foreach (XmlNode databasesNode in doc.ChildNodes)
                        {
                            if (databasesNode.Name.Equals("Databases"))
                            {
                                foreach (XmlNode node in databasesNode.ChildNodes)
                                {
                                    if (node.Name.Equals("Semantics"))
                                        semanticsDatabase = node.Attributes["Database"].Value;
                                    else if (node.Name.Equals("Values"))
                                    {
                                        foreach (XmlNode subNode in node.ChildNodes)
                                            valuesDatabases.Add(subNode.InnerText);
                                    }
                                    else if (node.Name.Equals("Localization"))
                                    {
                                        foreach (XmlNode subNode in node.ChildNodes)
                                            localizationDatabases.Add(subNode.InnerText);
                                    }
                                }
                                break;
                            }
                        }

                        String path = Path.GetDirectoryName(project) + Path.DirectorySeparatorChar;

                        // Upgrade the databases by adding the additional tables
                        Message message = Current.IDatabase.AddTables(DatabaseType.Semantics, path + semanticsDatabase, SemanticsSettings.Files.DatabasePath + GameSemanticsSettings.Files.SemanticsSQLFile);
                        foreach (String valuesDatabase in valuesDatabases)
                            Current.IDatabase.AddTables(DatabaseType.Values, path + valuesDatabase, SemanticsSettings.Files.DatabasePath + GameSemanticsSettings.Files.ValuesSQLFile);
                        foreach (String localizationDatabase in localizationDatabases)
                            Current.IDatabase.AddTables(DatabaseType.Localization, path + localizationDatabase, SemanticsSettings.Files.DatabasePath + GameSemanticsSettings.Files.LocalizationSQLFile);

                        return message;
                    }
                }
                catch (Exception)
                {
                }
            }
            return Message.CreationFail;
        }
        #endregion Method: UpgradeToGameSemanticsProject(String file)

        #endregion Method Group: Projects

        #region Method Group: General

        #region Method: AddTableDefinition(string table, Dictionary<string, Type owner, Tuple<Type, EntryType>> columnsTypes)
        /// <summary>
        /// Define which types can be found in which columns of which table. Only useful for referenced nodes.
        /// </summary>
        /// <param name="table">The table to add the definition for.</param>
        /// <param name="owner">The owner of the table.</param>
        /// <param name="columnsTypes">A dictionary with column names as keys, and tuples as values. The tuples contain the type of the column, and the type of the entry.</param>
        protected internal new void AddTableDefinition(string table, Type owner, Dictionary<string, Tuple<Type, EntryType>> columnsTypes)
        {
            base.AddTableDefinition(table, owner, columnsTypes);
        }
        #endregion Method: AddTableDefinition(string table, Dictionary<string, Type owner, Tuple<Type, EntryType>> columnsTypes)

        #region Method: StartChange()
        /// <summary>
        /// Indicate that multiple changes will be executed on the database.
        /// </summary>
        protected internal new void StartChange()
        {
            base.StartChange();
        }
        #endregion Method: StartChange()

        #region Method: StopChange()
        /// <summary>
        /// Indicate that multiple changes have finished executing on the database.
        /// </summary>
        protected internal new void StopChange()
        {
            base.StopChange();
        }
        #endregion Method: StopChange()

        #region Method: StartRemove()
        /// <summary>
        /// Indicate that a node removal is started.
        /// </summary>
        protected internal new void StartRemove()
        {
            base.StartRemove();
        }
        #endregion Method: StartRemove()

        #endregion Method Group: General

        #region Method Group: Select

        #region Method: Select<T>(uint id, string table, string column)
        /// <summary>
        /// Select an item of the given type from a table for the given ID.
        /// </summary>
        /// <typeparam name="T">The type the item should be of.</typeparam>
        /// <param name="id">The ID.</param>
        /// <param name="table">The table to select an item from.</param>
        /// <param name="column">The column to look in.</param>
        /// <returns>The object that was found.</returns>
        protected internal new T Select<T>(uint id, string table, string column)
        {
            return base.Select<T>(id, table, column);
        }
        #endregion Method: Select<T>(uint id, string table, string column)

        #region Method: Select<T>(uint id, string table, string column, string conditionColumn, object value)
        /// <summary>
        /// Select an item of the given type from the column for the given ID, having the given value in the condition column.
        /// </summary>
        /// <typeparam name="T">The type the item should be of.</typeparam>
        /// <param name="id">The ID.</param>
        /// <param name="table">The table to select an item from.</param>
        /// <param name="column">The column to look in.</param>
        /// <param name="conditionColumn">The column to define a condition for.</param>
        /// <param name="value">The value that should be in the condition column.</param>
        /// <returns>The object that was found.</returns>
        protected internal new T Select<T>(uint id, string table, string column, string conditionColumn, object value)
        {
            return base.Select<T>(id, table, column, conditionColumn, value);
        }
        #endregion Method: Select<T>(uint id, string table, string column, string conditionColumn, object value)

        #region Method: SelectAll<T>(uint id, string table, string column)
        /// <summary>
        /// Select all items from the given table for the given ID.
        /// </summary>
        /// <typeparam name="T">The type the items should be of.</typeparam>
        /// <param name="id">The ID.</param>
        /// <param name="table">The table to select the items from.</param>
        /// <param name="column">The column to look in.</param>
        /// <returns>The objects that were found.</returns>
        protected internal new List<T> SelectAll<T>(uint id, string table, string column)
        {
            return base.SelectAll<T>(id, table, column);
        }
        #endregion Method: SelectAll<T>(uint id, string table, string column)

        #region Method: SelectAll<T>(string table, string column, object value)
        /// <summary>
        /// Select all items in the given table where the given column has the given value.
        /// </summary>
        /// <typeparam name="T">The type the items should be of.</typeparam>
        /// <param name="table">The table to select the items from.</param>
        /// <param name="column1">The first column to get the results from.</param>
        /// <param name="column2">The second column to get the results from.</param>
        /// <returns>The objects that were found, stored in a dictionary with column1-column2 key-value pairs.</returns>
        protected internal new List<T> SelectAll<T>(string table, string column, object value)
        {
            return base.SelectAll<T>(table, column, value);
        }
        #endregion Method: SelectAll<T>(string table, string column, object value)

        #region Method: SelectAll<T, T2>(uint id, string table, string column1, string column2)
        /// <summary>
        /// Select all items in the given columns from the given table for the given ID.
        /// </summary>
        /// <typeparam name="T">The type the items should be of.</typeparam>
        /// <param name="id">The ID.</param>
        /// <param name="table">The table to select the items from.</param>
        /// <param name="column">The column to look in.</param>
        /// <returns>The objects that were found.</returns>
        protected internal new Dictionary<T, T2> SelectAll<T, T2>(uint id, string table, string column1, string column2)
        {
            return base.SelectAll<T, T2>(id, table, column1, column2);
        }
        #endregion Method: SelectAll<T, T2>(uint id, string table, string column1, string column2)

        #endregion Method Group: Select

        #region Method Group: Update/Insert/Remove

        #region Method: Update(uint id, string table, string column, object value)
        /// <summary>
        /// Update an item from a table.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <param name="table">The table to update an item in.</param>
        /// <param name="column">The column to update.</param>
        /// <param name="value">The value to put in the column.</param>
        protected internal new void Update(uint id, string table, string column, object value)
        {
            base.Update(id, table, column, value);
        }
        #endregion Method: Update(uint id, string table, string column, object value)

        #region Method: Update(uint id, string table, string column, object oldValue, object newValue)
        /// <summary>
        /// Update the old value of an item from a table with a new one.
        /// </summary>
        /// <param name="id">The ID</param>
        /// <param name="table">The table to update an item in.</param>
        /// <param name="column">The column to update.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value to replace the old one.</param>
        protected internal new void Update(uint id, string table, string column, object oldValue, object newValue)
        {
            base.Update(id, table, column, oldValue, newValue);
        }
        #endregion Method: Update(uint id, string table, string column, object oldValue, object newValue)

        #region Method: Update(uint id, string table, string column, object value, string conditionColumn, object conditionValue)
        /// <summary>
        /// Update an item from a table.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <param name="table">The table to update an item in.</param>
        /// <param name="column">The column to update.</param>
        /// <param name="value">The value to put in the column.</param>
        /// <param name="conditionColumn">The column to define a condition for.</param>
        /// <param name="conditionValue">The value that should be in the condition column.</param>
        protected internal new void Update(uint id, string table, string column, object value, string conditionColumn, object conditionValue)
        {
            base.Update(id, table, column, value, conditionColumn, conditionValue);
        }
        #endregion Method: Update(uint id, string table, string column, object value, string conditionColumn, object conditionValue)

        #region Method: Insert(uint id, string table, string column, object value)
        /// <summary>
        /// Insert a new item to the table with the given ID and put the given value in the given column.
        /// </summary>
        /// <param name="id">The ID of the new item to insert.</param>
        /// <param name="table">The table to insert in.</param>
        /// <param name="column">The column to assign a value to.</param>
        /// <param name="value">The value to assign to a column.</param>
        protected internal new void Insert(uint id, string table, string column, object value)
        {
            base.Insert(id, table, column, value);
        }
        #endregion Method: Insert(uint id, string table, string column, object value)

        #region Method: Insert(uint id, string table, string[] columns, object[] values)
        /// <summary>
        /// Insert a new item to the table with the given ID and the given values in the given columns.
        /// </summary>
        /// <param name="id">The ID of the new item to insert.</param>
        /// <param name="table">The table to insert in.</param>
        /// <param name="columns">The columns to assign a value to.</param>
        /// <param name="values">The values to assign to the columns.</param>
        protected internal new void Insert(uint id, string table, string[] columns, object[] values)
        {
            base.Insert(id, table, columns, values);
        }
        #endregion Method: Insert(uint id, string table, string[] columns, object[] values)

        #region Method: Remove(uint id, string table)
        /// <summary>
        ///  Remove the given ID from the given table.
        /// </summary>
        /// <param name="id">The ID to remove.</param>
        /// <param name="table">The table to remove the ID from.</param>
        protected internal new void Remove(uint id, string table)
        {
            base.Remove(id, table);
        }
        #endregion Method: Remove(uint id, string table)

        #region Method: Remove(uint id, string table, string column, object value)
        /// <summary>
        /// Remove the entry of the ID in the given table where the given value is in the given column
        /// </summary>
        /// <param name="id">The ID to remove.</param>
        /// <param name="table">The table to remove the ID from.</param>
        /// <param name="column">The column that should have a particular value.</param>
        /// <param name="value">The value that should be in the column.</param>
        protected internal new void Remove(uint id, string table, string column, object value)
        {
            base.Remove(id, table, column, value);
        }
        #endregion Method: Remove(uint id, string table, string column, object value)

        #region Method: Remove(uint id, string table, string column, object value, bool keepReference)
        /// <summary>
        /// Remove the entry of the ID in the given table where the given value is in the given column.
        /// </summary>
        /// <param name="id">The ID to remove.</param>
        /// <param name="table">The table to remove the ID from.</param>
        /// <param name="column">The column that should have a particular value.</param>
        /// <param name="value">The value that should be in the column.</param>
        /// <param name="keepReference">Indicates whether reference entries should be kept.</param>
        protected internal new void Remove(uint id, string table, string column, object value, bool keepReference)
        {
            base.Remove(id, table, column, value, keepReference);
        }
        #endregion Method: Remove(uint id, string table, string column, object value, bool keepReference)

        #endregion Method Group: Update/Insert/Remove

        #region Method Group: Add/Remove

        #region Method: Remove(IdHolder idHolder)
        /// <summary>
        /// Remove the given ID holder.
        /// </summary>
        /// <param name="idHolder">The ID holder to remove.</param>
        protected internal new void Remove(IdHolder idHolder)
        {
            base.Remove(idHolder);
        }
        #endregion Method: Remove(IdHolder idHolder)

        #endregion Method Group: Add/Remove

    }
    #endregion Class: GameDatabase

}