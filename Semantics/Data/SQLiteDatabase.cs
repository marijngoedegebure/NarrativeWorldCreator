/**************************************************************************
 * 
 * SQLiteDatabase.cs
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
using System.Data;
using System.Data.SQLite;
using System.IO;
using Common;
using Semantics.Interfaces;
using Semantics.Utilities;

namespace Semantics.Data
{

	#region Class: SQLiteDatabase
	/// <summary>
	/// A SQLite database.
	/// </summary>
	public class SQLiteDatabase : IDatabase
	{

		#region Properties and Fields

		#region Field: semanticsConnection
		/// <summary>
		/// The SQLite connection for the semantics database.
		/// </summary>
		private SQLiteConnection semanticsConnection = null;
		#endregion Field: semanticsConnection

		#region Field: currentValuesConnection
		/// <summary>
		/// The SQLite connection for the current values database.
		/// </summary>
		private SQLiteConnection currentValuesConnection = null;
		#endregion Field: currentValuesConnection

		#region Field: valuesConnections
		/// <summary>
		/// The SQLite connections for the values databases.
		/// </summary>
		private List<SQLiteConnection> valuesConnections = new List<SQLiteConnection>();
		#endregion Field: valuesConnections

		#region Field: currentLocalizationConnection
		/// <summary>
		/// The SQLite connection for the current localization database.
		/// </summary>
		private SQLiteConnection currentLocalizationConnection = null;
		#endregion Field: currentLocalizationConnection

		#region Field: localizationConnections
		/// <summary>
		/// The SQLite connections for the localization databases.
		/// </summary>
		private List<SQLiteConnection> localizationConnections = new List<SQLiteConnection>();
		#endregion Field: localizationConnections

		#region Field: connectionString
		/// <summary>
		/// The connection string.
		/// </summary>
		private const string connectionString = "Data Source=";
		#endregion Field: connectionString

		#region Field: semanticsDatabaseExtension
		/// <summary>
		/// The extension for the semantics database files.
		/// </summary>
		public const string semanticsDatabaseExtension = ".sdb";
		#endregion Field: semanticsDatabaseExtension

		#region Field: valuesDatabaseExtension
		/// <summary>
		/// The extension for the values database files.
		/// </summary>
		public const string valuesDatabaseExtension = ".vdb";
		#endregion Field: valuesDatabaseExtension

		#region Field: localizationDatabaseExtension
		/// <summary>
		/// The extension for the localization database files.
		/// </summary>
		public const string localizationDatabaseExtension = ".ldb";
		#endregion Field: localizationDatabaseExtension

		#region Field: workingCopyExtension
		/// <summary>
		/// The extension for a working copy file.
		/// </summary>
		private const string workingCopyExtension = ".temp";
		#endregion Field: workingCopyExtension

		#region Field: queryBeginCommit
		/// <summary>
		/// Indicates whether begin/commit mode is turned on.
		/// </summary>
		private bool queryBeginCommit = false;
		#endregion Field: queryBeginCommit

		#region Field: querySemanticsStack
		/// <summary>
		/// A stack with combined queries for all semantics database queries.
		/// </summary>
		private Stack<string> querySemanticsStack = new Stack<string>();
		#endregion Field: querySemanticsStack

		#region Field: queryValuesStack
		/// <summary>
		/// A stack with combined queries for all values database queries.
		/// </summary>
		private Stack<string> queryValuesStack = new Stack<string>();
		#endregion Field: queryValuesStack

		#region Field: queryLocalizationStack
		/// <summary>
		/// A stack with combined queries for all localization database queries.
		/// </summary>
		private Stack<string> queryLocalizationStack = new Stack<string>();
		#endregion Field: queryLocalizationStack

		#region Field: queryAllBeginCommit
		/// <summary>
		/// Indicates whether begin/commit mode is turned on.
		/// </summary>
		private bool queryAllBeginCommit = false;
		#endregion Field: queryAllBeginCommit

		#region Field: queryAllSemanticsStack
		/// <summary>
		/// A stack with combined queries for all semantics database queries.
		/// </summary>
		private Stack<string> queryAllSemanticsStack = new Stack<string>();
		#endregion Field: queryAllSemanticsStack

		#region Field: queryAllValuesStack
		/// <summary>
		/// A stack with combined queries for all values database queries.
		/// </summary>
		private Stack<string> queryAllValuesStack = new Stack<string>();
		#endregion Field: queryAllValuesStack

		#region Field: queryAllLocalizationStack
		/// <summary>
		/// A stack with combined queries for all localization database queries.
		/// </summary>
		private Stack<string> queryAllLocalizationStack = new Stack<string>();
		#endregion Field: queryAllLocalizationStack

		#endregion Properties and Fields

		#region Constructor: SQLiteDatabase()
		/// <summary>
		/// Creates a SQLite database.
		/// </summary>
		public SQLiteDatabase()
		{
			AppDomain.CurrentDomain.ProcessExit += new EventHandler(Uninitialize);
		}
		#endregion Constructor: SQLiteDatabase() 

		#region Method Group: General

		#region Method: AddExtension(DatabaseType databaseType, string database)
		/// <summary>
		/// Check whether the database has an extension for the given type, and if not, add it.
		/// </summary>
		/// <param name="databaseType">The type of the database.</param>
		/// <param name="database">The database.</param>
		/// <returns>The database with the correct extension.</returns>
		private string AddExtension(DatabaseType databaseType, string database)
		{
			if (database != null)
			{
				switch (databaseType)
				{
					case DatabaseType.Semantics:
						if (!database.EndsWith(semanticsDatabaseExtension, true, CommonSettings.Culture))
							database += semanticsDatabaseExtension;
						break;
					case DatabaseType.Values:
						if (!database.EndsWith(valuesDatabaseExtension, true, CommonSettings.Culture))
							database += valuesDatabaseExtension;
						break;
					case DatabaseType.Localization:
						if (!database.EndsWith(localizationDatabaseExtension, true, CommonSettings.Culture))
							database += localizationDatabaseExtension;
						break;
					default:
						break;
				}
			}
			return database;
		}
		#endregion Method: AddExtension(DatabaseType databaseType, string database)

		#region Method: GetFullPath(String file)
		/// <summary>
		/// Get the full path of the given file.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <returns>The full path of the file.</returns>
		private String GetFullPath(String file)
		{
			if (file != null)
			{
				if (!Path.IsPathRooted(file))
				{
					if (file.Contains("/") || file.Contains("\\"))
						file = Path.GetFullPath(BasicFunctionality.GetAppDirectory() + file);
					else
						file = Path.GetFullPath(SemanticsSettings.Files.DatabasePath + file);
				}
			}
			return file;
		}
		#endregion Method: GetFullPath(String file)

		#region Method: Create(DatabaseType databaseType, string database, string queryFile)
		/// <summary>
		/// Create a new database of the given type from the given query file.
		/// </summary>
		/// <param name="databaseType">The type of the database.</param>
		/// <param name="database">The database to create.</param>
		/// <param name="queryFile">The path and name of a file which contains SQL queries to create tables.</param>
		/// <returns>Returns whether the database has been created successfully.</returns>
		public Message Create(DatabaseType databaseType, string database, string queryFile)
		{
			if (database != null && queryFile != null)
			{
				try
				{
					// Get the database file with the extension and a full path
					database = AddExtension(databaseType, database);
					database = GetFullPath(database);

					// For values or localization databases, check whether there is one already
					string existingDatabaseFile = null;
					switch (databaseType)
					{
						case DatabaseType.Semantics:
							break;
						case DatabaseType.Values:
							if (this.currentValuesConnection != null)
								existingDatabaseFile = this.currentValuesConnection.ConnectionString.Remove(0, connectionString.Length);
							break;
						case DatabaseType.Localization:
							if (this.currentLocalizationConnection != null)
								existingDatabaseFile = this.currentLocalizationConnection.ConnectionString.Remove(0, connectionString.Length);
							break;
						default:
							break;
					}
					if (existingDatabaseFile != null)
					{
						// If a database exists, copy it to the new database
						File.Copy(existingDatabaseFile, database, true);
					}
					else
					{
						// Otherwise, create the database file and add the tables
						SQLiteConnection.CreateFile(database);
						AddTables(databaseType, database, queryFile);
					}

					return Message.CreationSuccess;
				}
				catch (Exception)
				{
				}
			}
			return Message.CreationFail;
		}
		#endregion Method: Create(DatabaseType databaseType, string database, string queryFile)
		
		#region Method: Create(DatabaseType databaseType, string database, string[] queryFiles)
		/// <summary>
		/// Create a new database of the given type from the given query files.
		/// </summary>
		/// <param name="databaseType">The type of the database.</param>
		/// <param name="database">The database to create.</param>
		/// <param name="queryFiles">The path and name of a files which contains SQL queries to create tables.</param>
		/// <returns>Returns whether the database has been created successfully.</returns>
		public Message Create(DatabaseType databaseType, string database, string[] queryFiles)
		{
			// Create a temporary file that combines the SQL files
			String queryFile = MergeIntoTempFile(queryFiles);

			if (queryFile != null)
			{
				// Create the database
				Message message = Create(databaseType, database, queryFile);

				// The temporary file is not necessary anymore
				File.Delete(queryFile);

				return message;
			}

			return Message.CreationFail;
		}
		#endregion Method: Create(DatabaseType databaseType, string database, string[] queryFiles)

		#region Method: MergeIntoTempFile(String[] files)
		/// <summary>
		/// Merge the files into one temporary file.
		/// </summary>
		/// <param name="files">An array with the files to merge.</param>
		/// <returns>A merged file from all files.</returns>
		private String MergeIntoTempFile(String[] files)
		{
			String tempFile = null;

			if (files != null)
			{
				try
				{
					String path = BasicFunctionality.GetExecutablePath();
					tempFile = path + "temp.sql";
					StreamWriter writer = new StreamWriter(tempFile);

					for (int i = 0; i < files.Length; i++)
					{
						String file = files[i];
						if (!Path.IsPathRooted(file))
							file = path + file;
						StreamReader reader = null;
						if (File.Exists(file))
						{
							reader = new StreamReader(file);
							writer.Write(reader.ReadToEnd());
							reader.Close();
						}
					}
					writer.Flush();
					writer.Close();
					writer.Dispose();
				}
				catch (Exception)
				{
				}
			}

			return tempFile;
		}
		#endregion Method: MergeIntoTempFile(String[] files)

		#region Method: AddTables(DatabaseType databaseType, String database, string queryFile)
		/// <summary>
		/// Add the tables from the given query file to the given database of the given type.
		/// </summary>
		/// <param name="databaseType">The type of the database.</param>
		/// <param name="database">The database to add tables to.</param>
		/// <param name="queryFile">The path and name of a file which contains SQL queries to create tables.</param>
		/// <returns>Returns whether the tables have been added successfully.</returns>
		public Message AddTables(DatabaseType databaseType, String database, string queryFile)
		{
			Message message = Message.CreationFail;

			// Get the database file with the extension and a full path
			database = AddExtension(databaseType, database);
			database = GetFullPath(database);

			// Execute the queries to create the tables
			SQLiteConnection connection = new SQLiteConnection(connectionString + database);
			if (connection != null)
			{
				StreamReader reader = null;
				try
				{
					connection.Open();
					if (connection.State == ConnectionState.Open)
					{
						queryFile = GetFullPath(queryFile);
						reader = new StreamReader(queryFile);
						if (reader != null)
						{
							string query = "BEGIN;\n" + reader.ReadToEnd() + "\nCOMMIT;";
							SQLiteCommand command = connection.CreateCommand();
							command.CommandText = query;
							command.ExecuteNonQuery();

							message = Message.CreationSuccess;
						}
					}
				}
				catch (Exception)
				{
				}
				finally
				{
					if (reader != null)
						reader.Close();
					connection.Close();
					connection.Dispose();
				}
			}

			return message;
		}
		#endregion Method: AddTables(DatabaseType databaseType, String database, string queryFile)

		#region Method: Initialize(DatabaseType databaseType, string database)
		/// <summary>
		/// Initialize the database of the given type.
		/// </summary>
		/// <param name="databaseType">The type of the database.</param>
		/// <param name="database">The database to initialize.</param>
		/// <returns>Returns whether the database has been initialized successfully.</returns>
		public Message Initialize(DatabaseType databaseType, string database)
		{
			try
			{
				if (database != null)
				{
					// Check the extension
					database = AddExtension(databaseType, database);

					// Get the full path
					database = GetFullPath(database);

					// Check whether the database has already been initialized
					if (IsInitialized(databaseType, database))
						return Message.AlreadyInitialized;

					if (File.Exists(database))
					{
						// Make a working copy of the file
						File.Copy(database, database + workingCopyExtension, true);

						// Set up the connection
						SQLiteConnection connection = new SQLiteConnection(connectionString + database + workingCopyExtension);
						if (connection != null)
						{
							connection.Open();
							switch (databaseType)
							{
								case DatabaseType.Semantics:
									if (this.semanticsConnection != null)
										this.semanticsConnection.Close();
									this.semanticsConnection = connection;
									break;

								case DatabaseType.Values:
									this.valuesConnections.Add(connection);
									if (this.currentValuesConnection == null)
										this.currentValuesConnection = connection;
									break;

								case DatabaseType.Localization:
									this.localizationConnections.Add(connection);
									if (this.currentLocalizationConnection == null)
										this.currentLocalizationConnection = connection;
									break;

								default:
									break;
							}

							return Message.InitializationSuccess;
						}
					}
				}
			}
			catch (Exception)
			{
			}
			return Message.InitializationFail;
		}
		#endregion Method: Initialize(DatabaseType databaseType, string database)

		#region Method: IsInitialized(DatabaseType databaseType, string databaseFile)
		/// <summary>
		/// Check whether the database of the given type and with the given file has already been initialized.
		/// </summary>
		/// <param name="databaseType">The type of the database.</param>
		/// <param name="databaseFile">The path and file name of the database to check.</param>
		/// <returns>Returns whether the database has been initialized.</returns>
		private bool IsInitialized(DatabaseType databaseType, string databaseFile)
		{
			if (databaseFile != null)
			{
				string stringToCheck = connectionString + databaseFile + workingCopyExtension;

				switch (databaseType)
				{
					case DatabaseType.Semantics:
						if (this.semanticsConnection == null)
							return false;
						else
							return stringToCheck.Equals(this.semanticsConnection.ConnectionString);

					case DatabaseType.Values:
						foreach (SQLiteConnection valuesConnection in this.valuesConnections)
						{
							if (stringToCheck.Equals(valuesConnection.ConnectionString))
								return true;
						}
						return false;

					case DatabaseType.Localization:
						foreach (SQLiteConnection localizationConnection in this.localizationConnections)
						{
							if (stringToCheck.Equals(localizationConnection.ConnectionString))
								return true;
						}
						return false;

					default:
						break;
				}
			}
			return false;
		}
		#endregion Method: IsInitialized(DatabaseType databaseType, string databaseFile)

		#region Method: Uninitialize()
		/// <summary>
		/// Uninitialize the database.
		/// </summary>
		/// <returns>Returns whether the database had been uninitialized successfully.</returns>
		public Message Uninitialize()
		{
#if !DEBUG
			try
#endif
			{
				// Close the connections and remove the working copies
				if (this.semanticsConnection != null)
				{
					string file = this.semanticsConnection.ConnectionString.Remove(0, connectionString.Length);
					this.semanticsConnection.Close();
					this.semanticsConnection.Dispose();
					this.semanticsConnection = null;
					if (File.Exists(file))
						File.Delete(file);
				}

				foreach (SQLiteConnection connection in this.valuesConnections)
				{
					connection.Close();
					string file = connection.ConnectionString.Remove(0, connectionString.Length);
					if (File.Exists(file))
						File.Delete(file);
				}
				this.valuesConnections.Clear();
				this.currentValuesConnection = null;

				foreach (SQLiteConnection connection in this.localizationConnections)
				{
					connection.Close();
					string file = connection.ConnectionString.Remove(0, connectionString.Length);
					if (File.Exists(file))
						File.Delete(file);
				}
				this.localizationConnections.Clear();
				this.currentLocalizationConnection = null;

				return Message.UninitializationSuccess;
			}
#if !DEBUG
			catch (Exception)
			{
				return Message.UninitializationFail;
			}
#endif
		}

		/// <summary>
		/// Uninitialize the database.
		/// </summary>
		protected void Uninitialize(object sender, EventArgs e)
		{
			Uninitialize();
		}
		#endregion Method: Uninitialize()

		#region Method: SwitchToValuesDatabase(string database)
		/// <summary>
		/// Switch to the given values database.
		/// </summary>
		/// <param name="database">The values database to switch to.</param>
		/// <returns>Returns whether the values database has been switched successfully.</returns>
		public Message SwitchToValuesDatabase(string database)
		{
			if (database != null)
			{
				SQLiteConnection connection = GetConnection(DatabaseType.Values, database);
				if (connection != null)
				{
					this.currentValuesConnection = connection;
					return Message.SwitchSuccess;
				}
			}
			return Message.SwitchFail;
		}
		#endregion Method: SwitchToValuesDatabase(string database)

		#region Method: SwitchToLocalizationDatabase(string database)
		/// <summary>
		/// Switch to the given localization database.
		/// </summary>
		/// <param name="database">The localization database to switch to.</param>
		/// <returns>Returns whether the localization database has been switched successfully.</returns>
		public Message SwitchToLocalizationDatabase(string database)
		{
			if (database != null)
			{
				SQLiteConnection connection = GetConnection(DatabaseType.Localization, database);
				if (connection != null)
				{
					this.currentLocalizationConnection = connection;
					return Message.SwitchSuccess;
				}
			}
			return Message.SwitchFail;
		}
		#endregion Method: SwitchToLocalizationDatabase(string database)

		#region Method: Save()
		/// <summary>
		/// Save the databases.
		/// </summary>
		/// <returns>Returns whether the databases have been saved successfully.</returns>
		public Message Save()
		{
			try
			{
				// Overwrite the database files with the working copies
				if (this.semanticsConnection != null)
				{
					string workingCopyFile = this.semanticsConnection.ConnectionString.Remove(0, connectionString.Length);
					string file = workingCopyFile.Substring(0, workingCopyFile.Length - workingCopyExtension.Length);
					File.Copy(workingCopyFile, file, true);
				}

				foreach (SQLiteConnection valuesConnection in this.valuesConnections)
				{
					string workingCopyFile = valuesConnection.ConnectionString.Remove(0, connectionString.Length);
					string file = workingCopyFile.Substring(0, workingCopyFile.Length - workingCopyExtension.Length);
					File.Copy(workingCopyFile, file, true);
				}

				foreach (SQLiteConnection localizationConnection in this.localizationConnections)
				{
					string workingCopyFile = localizationConnection.ConnectionString.Remove(0, connectionString.Length);
					string file = workingCopyFile.Substring(0, workingCopyFile.Length - workingCopyExtension.Length);
					File.Copy(workingCopyFile, file, true);
				}
				
				return Message.SaveSuccess;
			}
			catch (Exception)
			{
				return Message.SaveFail;
			}
		}
		#endregion Method: Save()

		#region Method: SaveAs(string database)
		/// <summary>
		/// Save the database as new database.
		/// </summary>
		/// <param name="database">The new database name.</param>
		/// <returns>Returns whether the databases have been saved successfully.</returns>
		public Message SaveAs(string database)
		{
			try
			{
				database = Path.GetFileNameWithoutExtension(database);

				// Store the working copies in the new files
				if (this.semanticsConnection != null)
				{
					string workingCopyFile = this.semanticsConnection.ConnectionString.Remove(0, connectionString.Length);
					File.Copy(workingCopyFile, database + semanticsDatabaseExtension, true);
				}

				foreach (SQLiteConnection valuesConnection in this.valuesConnections)
				{
					string workingCopyFile = valuesConnection.ConnectionString.Remove(0, connectionString.Length);
					File.Copy(workingCopyFile, database + valuesDatabaseExtension, true);
				}

				foreach (SQLiteConnection localizationConnection in this.localizationConnections)
				{
					string workingCopyFile = localizationConnection.ConnectionString.Remove(0, connectionString.Length);
					File.Copy(workingCopyFile, database + localizationDatabaseExtension, true);
				}

				return Message.SaveSuccess;
			}
			catch (Exception)
			{
				return Message.SaveFail;
			}
		}
		#endregion Method: SaveAs(string database)

		#region Method: Remove(DatabaseType databaseType, string database)
		/// <summary>
		/// Remove the database of the given type.
		/// </summary>
		/// <param name="databaseType">The type of the database.</param>
		/// <param name="database">The database to remove.</param>
		/// <returns>Returns whether the database has been removed successfully.</returns>
		public Message Remove(DatabaseType databaseType, string database)
		{
			if (database != null)
			{
				// Remove the correct database
				switch (databaseType)
				{
					case DatabaseType.Semantics:
						// The semantics connection cannot be removed
						return Message.RemovalNotAllowed;

					case DatabaseType.Values:
						// There has to be at least one values connection
						if (this.valuesConnections.Count == 1)
							return Message.RemovalNotAllowed;

						SQLiteConnection valuesConnection = GetConnection(DatabaseType.Values, database);
						if (valuesConnection != null)
						{
							// Close and remove the connection
							valuesConnection.Close();
							this.valuesConnections.Remove(valuesConnection);

							// If it was the current connection, switch to another one
							if (valuesConnection.Equals(this.currentValuesConnection))
								SwitchToValuesDatabase(this.valuesConnections[0].ConnectionString.Remove(0, connectionString.Length));

							// Remove the temporary and original file
							string file = valuesConnection.ConnectionString.Remove(0, connectionString.Length);
							if (File.Exists(file))
								File.Delete(file);
							file = file.Substring(0, file.Length - workingCopyExtension.Length);
							if (File.Exists(file))
								File.Delete(file);
						}
						break;

					case DatabaseType.Localization:
						// There has to be at least one localization connection
						if (this.localizationConnections.Count == 1)
							return Message.RemovalNotAllowed;

						SQLiteConnection localizationConnection = GetConnection(DatabaseType.Localization, database);
						if (localizationConnection != null)
						{
							// Close and remove the connection
							localizationConnection.Close();
							this.localizationConnections.Remove(localizationConnection);

							// If it was the current connection, switch to another one
							if (localizationConnection.Equals(this.currentLocalizationConnection))
								SwitchToLocalizationDatabase(this.localizationConnections[0].ConnectionString.Remove(0, connectionString.Length));

							// Remove the temporary and original file
							string file = localizationConnection.ConnectionString.Remove(0, connectionString.Length);
							if (File.Exists(file))
								File.Delete(file);
							file = file.Substring(0, file.Length - workingCopyExtension.Length);
							if (File.Exists(file))
								File.Delete(file);
						}
						break;

					default:
						break;
				}

				return Message.RemovalSuccess;
			}

			return Message.RemovalFail;
		}
		#endregion Method: Remove(DatabaseType databaseType, string database)

		#region Method: Delete(DatabaseType databaseType, string database)
		/// <summary>
		/// Delete the database of the given type. Will uninitialize the database first!
		/// </summary>
		/// <param name="databaseType">The type of the database.</param>
		/// <param name="database">The database to remove.</param>
		public void Delete(DatabaseType databaseType, string database)
		{
			if (database != null)
			{
				try
				{
					// Check the extension and path
					database = AddExtension(databaseType, database);
					database = GetFullPath(database);

					// Uninitialize
					Uninitialize();

					// Delete the file
					if (File.Exists(database))
						File.Delete(database);
				}
				catch (Exception)
				{
				}
			}
		}
		#endregion Method: Delete(DatabaseType databaseType, string database)

		#region Method: GetConnection(DatabaseType databaseType, string databaseFile)
		/// <summary>
		/// Get the connection for the database file of the given type.
		/// </summary>
		/// <param name="databaseType">The type of the database.</param>
		/// <param name="databaseFile">The path and file name of the database to get the connection of.</param>
		/// <returns>The connection of the database file.</returns>
		private SQLiteConnection GetConnection(DatabaseType databaseType, string databaseFile)
		{
			if (databaseFile != null)
			{
				databaseFile = GetFullPath(databaseFile);

				switch (databaseType)
				{
					case DatabaseType.Semantics:
						return this.semanticsConnection;

					case DatabaseType.Values:
						foreach (SQLiteConnection valuesConnection in this.valuesConnections)
						{
							if (valuesConnection.ConnectionString.Contains(databaseFile + valuesDatabaseExtension))
								return valuesConnection;
						}
						return null;

					case DatabaseType.Localization:
						foreach (SQLiteConnection localizationConnection in this.localizationConnections)
						{
							if (localizationConnection.ConnectionString.Contains(databaseFile + localizationDatabaseExtension))
								return localizationConnection;
						}
						return null;

					default:
						break;
				}
			}
			return null;
		}
		#endregion Method: GetConnection(DatabaseType databaseType, string databaseFile)

		#region Method: GetConnection(DatabaseType databaseType)
		/// <summary>
		/// Get the SQLite connection for the given database type.
		/// </summary>
		/// <param name="databaseType">The database type to get the connection of.</param>
		/// <returns>The connection of the database type.</returns>
		private SQLiteConnection GetConnection(DatabaseType databaseType)
		{
			switch (databaseType)
			{
				case DatabaseType.Semantics:
					return this.semanticsConnection;
				case DatabaseType.Values:
					return this.currentValuesConnection;
				case DatabaseType.Localization:
					return this.currentLocalizationConnection;
				default:
					throw new Exception(Exceptions.NoDatabaseConnection);
			}
		}
		#endregion Method: GetConnection(DatabaseType databaseType)

		#region Method: GetColumns(DatabaseType databaseType, string table)
		/// <summary>
		/// Get all columns of the given table.
		/// </summary>
		/// <param name="databaseType">The database type.</param>
		/// <param name="table">The table to get the columns of.</param>
		/// <returns>The columns of the table.</returns>
		public List<string> GetColumns(DatabaseType databaseType, string table)
		{
			List<string> columns = new List<string>();

			SQLiteConnection connection = GetConnection(databaseType);
			if (connection.State == ConnectionState.Open)
			{
				DataTable tableInfo = connection.GetSchema("Columns", new string[] { null, null, table });
				if (tableInfo != null)
				{
					foreach (DataRow dataRow in tableInfo.Rows)
					{
						// Get the column
						columns.Add((string)dataRow["column_name"]);
					}
				}
			}
			return columns;
		}
		#endregion Method: GetColumns(DatabaseType databaseType, string table)

		#endregion Method Group: General

		#region Method Group: SQLite Queries

		#region Method: Query(DatabaseType databaseType, string query)
		/// <summary>
		/// Query a string. In case of a localization database, only use the current local connection.
		/// </summary>
		/// <param name="databaseType">The database type.</param>
		/// <param name="query">The query.</param>
		/// <returns>Returns whether the query has been executed successfully.</returns>
		public bool Query(DatabaseType databaseType, string query)
		{
			bool success = false;

			if (!string.IsNullOrEmpty(query))
			{
				// Store the query if the begin/commit mode is turned on
				if (this.queryBeginCommit)
				{
					switch (databaseType)
					{
						case DatabaseType.Semantics:
							string querySemantics = this.querySemanticsStack.Pop();
							querySemantics += query + "\n";
							this.querySemanticsStack.Push(querySemantics);
							break;
						case DatabaseType.Values:
							string queryValues = this.queryValuesStack.Pop();
							queryValues += query + "\n";
							this.queryValuesStack.Push(queryValues);
							break;
						case DatabaseType.Localization:
							string queryLocalization = this.queryLocalizationStack.Pop();
							queryLocalization += query + "\n";
							this.queryLocalizationStack.Push(queryLocalization);
							break;
						default:
							break;
					}
					return true;
				}
				// Otherwise, commit immediately
				else
				{
					// Get all connections of the database type
					List<SQLiteConnection> connections = new List<SQLiteConnection>();
					switch (databaseType)
					{
						case DatabaseType.Semantics:
							connections.Add(this.semanticsConnection);
							break;
						case DatabaseType.Values:
							connections = this.valuesConnections;
							break;
						case DatabaseType.Localization:
							connections.Add(this.currentLocalizationConnection);
							break;
						default:
							break;
					}

					// Execute the query on all databases of the given database type
					foreach (SQLiteConnection connection in connections)
					{
						if (Query(connection, query))
							success = true;
					}
				}
			}

			return success;
		}
		#endregion Method: Query(DatabaseType databaseType, string query)
		
		#region Method: QueryAll(DatabaseType databaseType, string query)
		/// <summary>
		/// Query a string on all databases.
		/// </summary>
		/// <param name="databaseType">The database type.</param>
		/// <param name="query">The query.</param>
		/// <returns>Returns whether the query has been executed successfully.</returns>
		public bool QueryAll(DatabaseType databaseType, string query)
		{
			bool success = false;

			if (!string.IsNullOrEmpty(query))
			{
				// Store the query if the begin/commit mode is turned on
				if (this.queryAllBeginCommit)
				{
					switch (databaseType)
					{
						case DatabaseType.Semantics:
							string queryAllSemantics = this.queryAllSemanticsStack.Pop();
							queryAllSemantics += query + "\n";
							this.queryAllSemanticsStack.Push(queryAllSemantics);
							break;
						case DatabaseType.Values:
							string queryAllValues = this.queryAllValuesStack.Pop();
							queryAllValues += query + "\n";
							this.queryAllValuesStack.Push(queryAllValues);
							break;
						case DatabaseType.Localization:
							string queryAllLocalization = this.queryAllLocalizationStack.Pop();
							queryAllLocalization += query + "\n";
							this.queryAllLocalizationStack.Push(queryAllLocalization);
							break;
						default:
							break;
					}
					return true;
				}
				// Otherwise, commit immediately
				else
				{
					// Get all connections of the database type
					List<SQLiteConnection> connections = new List<SQLiteConnection>();
					switch (databaseType)
					{
						case DatabaseType.Semantics:
							connections.Add(this.semanticsConnection);
							break;
						case DatabaseType.Values:
							connections = this.valuesConnections;
							break;
						case DatabaseType.Localization:
							connections = this.localizationConnections;
							break;
						default:
							break;
					}

					// Execute the query on all databases of the given database type
					foreach (SQLiteConnection connection in connections)
					{
						if (Query(connection, query))
							success = true;
					}
				}
			}

			return success;
		}
		#endregion Method: QueryAll(DatabaseType databaseType, string query)

		#region Method: QuerySelect(DatabaseType databaseType, string query)
		/// <summary>
		/// Select the entry that corresponds to the query.
		/// </summary>
		/// <param name="databaseType">The database type.</param>
		/// <param name="query">The query.</param>
		/// <returns>Returns the entry that corresponds to the query.</returns>
		public object QuerySelect(DatabaseType databaseType, string query)
		{
			SQLiteConnection connection = GetConnection(databaseType);
			try
			{
				if (connection != null && !string.IsNullOrEmpty(query))
				{
					if (connection.State == ConnectionState.Open)
					{
						SQLiteCommand command = connection.CreateCommand();
						command.CommandText = query;
						object obj = command.ExecuteScalar();
						if (obj is DBNull)
							return null;
						return obj;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			return null;
		}
		#endregion Method: QuerySelect(DatabaseType databaseType, string query)

		#region Method: QuerySelectAll(DatabaseType databaseType, string query)
		/// <summary>
		/// Select all entries that correspond to the query.
		/// </summary>
		/// <param name="databaseType">The database type.</param>
		/// <param name="query">The query.</param>
		/// <returns>Returns the entries that correspond to the query.</returns>
		public List<object> QuerySelectAll(DatabaseType databaseType, string query)
		{
			List<object> selections = new List<object>();
			SQLiteConnection connection = GetConnection(databaseType);
			try
			{
				if (connection != null && !string.IsNullOrEmpty(query))
				{
					if (connection.State == ConnectionState.Open)
					{
						SQLiteCommand command = connection.CreateCommand();
						command.CommandText = query;

						SQLiteDataReader reader = command.ExecuteReader();
						if (reader != null)
						{
							while (reader.Read())
							{
								object obj = reader[0];
								if (obj is DBNull)
									selections.Add(null);
								else
									selections.Add(obj);
							}
							reader.Close();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
			return selections;
		}
		#endregion Method: QuerySelectAll(DatabaseType databaseType, string query)

		#region Method: QuerySelectAllColumns(DatabaseType databaseType, string query, int columns)
		/// <summary>
		/// Select all entries that correspond to the query.
		/// </summary>
		/// <param name="databaseType">The database type.</param>
		/// <param name="query">The query.</param>
		/// <param name="query">The number of columns to consider in each row, starting from the first.</param>
		/// <returns>Returns the entries that correspond to the query.</returns>
		public List<object[]> QuerySelectAllColumns(DatabaseType databaseType, string query, int columns)
		{
			List<object[]> selections = new List<object[]>();
			SQLiteConnection connection = GetConnection(databaseType);
			if (connection != null && !string.IsNullOrEmpty(query))
			{
				if (connection.State == ConnectionState.Open)
				{
					SQLiteCommand command = connection.CreateCommand();
					command.CommandText = query;

					SQLiteDataReader reader = command.ExecuteReader();
					if (reader != null)
					{
						while (reader.Read())
						{
							object[] results = new object[columns];
							reader.GetValues(results);
							for (int i = 0; i < results.Length; i++)
							{
								if (results[i] is DBNull)
									results[i] = null;
							}
							selections.Add(results);
						}
						reader.Close();
					}
				}
			}
			return selections;
		}
		#endregion Method: QuerySelectAllColumns(DatabaseType databaseType, string query)

		#region Method: QueryBegin()
		/// <summary>
		/// Begins a combined query. Until QueryCommit() is called, all queries will be temporarily stored, and not yet executed.
		/// </summary>
		public void QueryBegin()
		{
			this.queryBeginCommit = true;
			this.querySemanticsStack.Push(string.Empty);
			this.queryValuesStack.Push(string.Empty);
			this.queryLocalizationStack.Push(string.Empty);
		}
		#endregion Method: QueryBegin()

		#region Method: QueryCommit()
		/// <summary>
		/// Commits all temporarily stored queries. Use only after calling QueryBegin()!
		/// </summary>
		public void QueryCommit()
		{
			if (this.queryBeginCommit)
			{
				// Get the query strings, add the begin/commit tags and perform the queries
				string querySemantics = this.querySemanticsStack.Pop();
				if (!string.IsNullOrEmpty(querySemantics))
				{
					querySemantics = "BEGIN;\n" + querySemantics + "COMMIT;";
					Query(this.semanticsConnection, querySemantics);
				}

				string queryValues = this.queryValuesStack.Pop();
				if (!string.IsNullOrEmpty(queryValues))
				{
					queryValues = "BEGIN;\n" + queryValues  + "COMMIT;";
					foreach (SQLiteConnection connection in this.valuesConnections)
						Query(connection, queryValues);
				}


				string queryLocalization = this.queryLocalizationStack.Pop();
				if (!string.IsNullOrEmpty(queryLocalization))
				{
					queryLocalization = "BEGIN;\n" + queryLocalization + "COMMIT;";
					Query(this.currentLocalizationConnection, queryLocalization);
				}
			}

			// Reset
			this.queryBeginCommit = this.querySemanticsStack.Count != 0;
		}
		#endregion Method: QueryCommit()

		#region Method: QueryAllBegin()
		/// <summary>
		/// Begins a combined query. Until QueryAllCommit() is called, all queries will be temporarily stored, and not yet executed.
		/// </summary>
		public void QueryAllBegin()
		{
			this.queryAllBeginCommit = true;
			this.queryAllSemanticsStack.Push(string.Empty);
			this.queryAllValuesStack.Push(string.Empty);
			this.queryAllLocalizationStack.Push(string.Empty);
		}
		#endregion Method: QueryAllBegin()

		#region Method: QueryAllCommit()
		/// <summary>
		/// Commits all temporarily stored queries. Use only after calling QueryAllBegin()!
		/// </summary>
		public void QueryAllCommit()
		{
			if (this.queryAllBeginCommit)
			{
				// Get the query strings, add the begin/commit tags and perform the queries
				string queryAllSemantics = this.queryAllSemanticsStack.Pop();
				if (!string.IsNullOrEmpty(queryAllSemantics))
				{
					queryAllSemantics = "BEGIN;\n" + queryAllSemantics + "COMMIT;";
					Query(this.semanticsConnection, queryAllSemantics);
				}

				string queryAllValues = this.queryAllValuesStack.Pop();
				if (!string.IsNullOrEmpty(queryAllValues))
				{
					queryAllValues = "BEGIN;\n" + queryAllValues + "COMMIT;";
					foreach (SQLiteConnection connection in this.valuesConnections)
						Query(connection, queryAllValues);
				}


				string queryAllLocalization = this.queryAllLocalizationStack.Pop();
				if (!string.IsNullOrEmpty(queryAllLocalization))
				{
					queryAllLocalization = "BEGIN;\n" + queryAllLocalization + "COMMIT;";
					foreach (SQLiteConnection connection in this.localizationConnections)
						Query(connection, queryAllLocalization);
				}
			}

			// Reset
			this.queryAllBeginCommit = this.queryAllSemanticsStack.Count != 0;
		}
		#endregion Method: QueryAllCommit()

		#region Method: Query(SQLiteConnection connection, string query)
		/// <summary>
		/// Perform a query on the given connection.
		/// </summary>
		/// <param name="connection">The connection.</param>
		/// <param name="query">The query.</param>
		/// <returns>Returns whether the query has been successful.</returns>
		private bool Query(SQLiteConnection connection, string query)
		{
			if (!string.IsNullOrEmpty(query) && connection != null && connection.State == ConnectionState.Open)
			{
				try
				{
					SQLiteCommand command = connection.CreateCommand();
					command.CommandText = query;
					command.ExecuteNonQuery();
					return true;
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
			}
			return false;
		}
		#endregion Method: Query(SQLiteConnection connection, string query)

		#endregion Method Group: SQLite Queries

	}
	#endregion Class: SQLiteDatabase
		
}