/**************************************************************************
 * 
 * Database.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Xml;
using Common;
using Semantics.Components;
using Semantics.Interfaces;
using Semantics.Utilities;
using Path = System.IO.Path;

namespace Semantics.Data
{

    #region Enum: EntryType
    /// <summary>
    /// The type of a database entry.
    /// </summary>
    public enum EntryType
    {
        /// <summary>
        /// On removal, the target should be removed as well (so it's entire database entry).
        /// </summary>
        Unique,

        /// <summary>
        /// On removal, the database entry should become NULL.
        /// </summary>
        Nullable,

        /// <summary>
        /// On removal, the target should be removed, but only from the intermediate table declared in the table definition.
        /// </summary>
        Intermediate
    }
    #endregion Enum: EntryType

    #region Enum: DatabaseType
    /// <summary>
    /// The type of the database.
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>
        /// The semantics database.
        /// </summary>
        Semantics,

        /// <summary>
        /// The values database.
        /// </summary>
        Values,

        /// <summary>
        /// The localization database.
        /// </summary>
        Localization
    }
    #endregion Enum: DatabaseType
		
    #region Class: Database
    /// <summary>
    /// The database is able to load, modify, and save (the relations between) all kinds of nodes.
    /// </summary>
    public class Database
    {

        #region Events, Properties, and Fields

        #region Event: DatabaseHandler
        /// <summary>
        /// A handler for the DatabaseChanged event.
        /// </summary>
        public delegate void DatabaseHandler();

        /// <summary>
        /// An event for a changed database.
        /// </summary>
        public event DatabaseHandler DatabaseChanged;
        #endregion Event: DatabaseHandler

        #region Event: NodeHandler
        /// <summary>
        /// A handler for the added and removed node events.
        /// </summary>
        /// <param name="node">The node.</param>
        public delegate void NodeHandler(Node node);

        /// <summary>
        /// An event for an added node.
        /// </summary>
        public event NodeHandler NodeAdded;

        /// <summary>
        /// An event for a removed node.
        /// </summary>
        public event NodeHandler NodeRemoved;
        #endregion Event: NodeHandler

        #region Event: ProjectHandler
        /// <summary>
        /// A handler for the ProjectCreated, ProjectLoaded, ProjectSaved, ProjectClosed, and ProjectDeleted events.
        /// </summary>
        /// <param name="project">The project.</param>
        public delegate void ProjectHandler(string project);

        /// <summary>
        /// An event for a created project.
        /// </summary>
        public static event ProjectHandler ProjectCreated;

        /// <summary>
        /// An event for a loaded project.
        /// </summary>
        public static event ProjectHandler ProjectLoaded;

        /// <summary>
        /// An event for a saved project.
        /// </summary>
        public static event ProjectHandler ProjectSaved;

        /// <summary>
        /// An event for a closed project.
        /// </summary>
        public static event ProjectHandler ProjectClosed;

        /// <summary>
        /// An event for a deleted project.
        /// </summary>
        public static event ProjectHandler ProjectDeleted;
        #endregion Event: ProjectHandler

        #region Property: IDatabase
        /// <summary>
        /// The database interface.
        /// </summary>
        private IDatabase iDatabase = new SQLiteDatabase();

        /// <summary>
        /// The database interface.
        /// </summary>
        public IDatabase IDatabase
        {
            get
            {
                return iDatabase;
            }
            set
            {
                if (iDatabase == null)
                    throw new Exception("IDatabase cannot be null!");
                iDatabase = value;
            }
        }
        #endregion Property: IDatabase

        #region Property: Current
        /// <summary>
        /// The current database.
        /// </summary>
        private static Database current = null;

        /// <summary>
        /// Gets the current database.
        /// </summary>
        public static Database Current
        {
            get
            {
                return current;
            }
            protected set
            {
                current = value;
            }
        }
        #endregion Property: Current

        #region Property: Project
        /// <summary>
        /// The loaded project.
        /// </summary>
        private static String project = String.Empty;

        /// <summary>
        /// Gets the loaded project.
        /// </summary>
        public static String Project
        {
            get
            {
                return project;
            }
        }
        #endregion Property: Project

        #region Property: IsProjectLoaded
        /// <summary>
        /// Indicates whether a database project has loaded.
        /// </summary>
        private static bool isProjectLoaded = false;

        /// <summary>
        /// Gets the value that indicates whether a database project has loaded.
        /// </summary>
        public static bool IsProjectLoaded
        {
            get
            {
                return isProjectLoaded;
            }
        }
        #endregion Property: IsProjectLoaded

        #region Property: Assemblies
        /// <summary>
        /// The assemblies in which all nodes are present.
        /// </summary>
        private List<Assembly> assemblies = new List<Assembly>();

        /// <summary>
        /// Gets the assemblies in which all nodes are present.
        /// </summary>
        public static ReadOnlyCollection<Assembly> Assemblies
        {
            get
            {
                if (Current != null)
                    return Current.assemblies.AsReadOnly();
                return new List<Assembly>().AsReadOnly();
            }
        }
        #endregion Property: Assemblies

        #region Property: CanUndo
        /// <summary>
        /// Gets the value that indicates whether there is an action that can be undone.
        /// </summary>
        public bool CanUndo
        {
            get
            {
                return this.undoStack.Count > 0;
            }
        }
        #endregion Property: CanUndo

        #region Property: CanRedo
        /// <summary>
        /// Gets the value that indicates whether there is an action that can be redone.
        /// </summary>
        public bool CanRedo
        {
            get
            {
                return this.redoStack.Count > 0;
            }
        }
        #endregion Property: CanRedo

        #region Property: SemanticsDatabase
        /// <summary>
        /// The semantics database.
        /// </summary>
        private String semanticsDatabase = null;

        /// <summary>
        /// Gets the semantics database.
        /// </summary>
        public String SemanticsDatabase
        {
            get
            {
                return semanticsDatabase;
            }
        }
        #endregion Property: SemanticsDatabase

        #region Property: ValuesDatabases
        /// <summary>
        /// The values databases.
        /// </summary>
        private List<String> valuesDatabases = new List<String>();

        /// <summary>
        /// Gets the values databases.
        /// </summary>
        public ReadOnlyCollection<String> ValuesDatabases
        {
            get
            {
                return valuesDatabases.AsReadOnly();
            }
        }
        #endregion Property: ValuesDatabases

        #region Property: CurrentValuesDatabase
        /// <summary>
        /// The current values database.
        /// </summary>
        private String currentValuesDatabase = null;

        /// <summary>
        /// Gets the current values database.
        /// </summary>
        public String CurrentValuesDatabase
        {
            get
            {
                return currentValuesDatabase;
            }
        }
        #endregion Property: CurrentValuesDatabase

        #region Property: LocalizationDatabases
        /// <summary>
        /// The localization databases.
        /// </summary>
        private List<String> localizationDatabases = new List<String>();

        /// <summary>
        /// Gets the localization databases.
        /// </summary>
        public ReadOnlyCollection<String> LocalizationDatabases
        {
            get
            {
                return localizationDatabases.AsReadOnly();
            }
        }
        #endregion Property: LocalizationDatabases

        #region Property: CurrentLocalizationDatabase
        /// <summary>
        /// The current localization database.
        /// </summary>
        private String currentLocalizationDatabase = null;

        /// <summary>
        /// Gets the current localization database.
        /// </summary>
        public String CurrentLocalizationDatabase
        {
            get
            {
                return currentLocalizationDatabase;
            }
        }
        #endregion Property: CurrentLocalizationDatabase

        #region Field: tableDatabaseType
        /// <summary>
        /// A dictionary to store the database type for each table.
        /// </summary>
        private Dictionary<string, DatabaseType> tableDatabaseType = new Dictionary<string, DatabaseType>();
        #endregion Field: tableDatabaseType

        #region Field: resourceTableTable
        /// <summary>
        /// A dictionary to store resource table names and the corresponding table.
        /// </summary>
        private Dictionary<string, string> resourceTableTable = new Dictionary<string,string>();
        #endregion Field: resourceTableTable
        
        #region Field: namesResourceManagers
        /// <summary>
        /// The resource managers for the new names for nodes.
        /// </summary>
        private List<ResourceManager> namesResourceManagers = new List<ResourceManager>();
        #endregion Field: namesResourceManagers

        #region Field: tableColumnType
        /// <summary>
        /// A list that defines which types can be found in which columns of which table.
        /// </summary>
        private List<Tuple<string, Tuple<Type, Dictionary<string, Tuple<Type, EntryType>>>>> tableColumnType = new List<Tuple<string, Tuple<Type, Dictionary<string, Tuple<Type, EntryType>>>>>();
        #endregion Field: tableColumnType

        #region Field: memory
        /// <summary>
        /// A dictionary for ID holders that have been loaded before; the key contains the type, the value a dictionary with IDs and the ID holders.
        /// </summary>
        private Dictionary<Type, Dictionary<uint, object>> memory = new Dictionary<Type, Dictionary<uint, object>>();
        #endregion Field: memory

        #region Field: undoStack
        /// <summary>
        /// The stack with tuples having a unique identifier, and a list of query tuples that can be undone; the second entry of the tuple is the redo command, the third entry is the undo command.
        /// </summary>
        protected Stack<Tuple<int, List<Tuple<DatabaseType, string, string>>>> undoStack = new Stack<Tuple<int, List<Tuple<DatabaseType, string, string>>>>();
        #endregion Field: undoStack

        #region Field: redoStack
        /// <summary>
        /// The stack with tuples having a unique identifier, and a list of query tuples that can be redone; the second entry of the tuple is the redo command, the third entry is the undo command.
        /// </summary>
        protected Stack<Tuple<int, List<Tuple<DatabaseType, string, string>>>> redoStack = new Stack<Tuple<int, List<Tuple<DatabaseType, string, string>>>>();
        #endregion Field: redoStack

        #region Field: undoRedoCounter
		/// <summary>
		/// A counter for the undo and redo stack.
		/// </summary>
		private int undoRedoCounter = 0;
		#endregion Field: undoRedoCounter

        #region Field: additionMoments
        /// <summary>
        /// The moments (counted by the undo/redo counter) when a node was added.
        /// </summary>
        private List<Tuple<int, uint>> additionMoments = new List<Tuple<int,uint>>();
        #endregion Field: additionMoments

        #region Field: removalMoments
        /// <summary>
        /// The moments (counted by the undo/redo counter) when a node was removed.
        /// </summary>
        private List<Tuple<int, uint>> removalMoments = new List<Tuple<int, uint>>();
        #endregion Field: removalMoments

        #region Field: numberOfStartedChanges
        /// <summary>
        /// The number of started changes on the database.
        /// </summary>
        private int numberOfStartedChanges = 0;
        #endregion Field: numberOfStartedChanges

        #region Field: nonAbstractTypes
        /// <summary>
        /// A dictionary to store all non abstract types of the key type.
        /// </summary>
        private Dictionary<Type, List<Type>> nonAbstractTypes = new Dictionary<Type,List<Type>>();
        #endregion Field: nonAbstractTypes

        #region Field: deepestTypes
        /// <summary>
        /// A dictionary with the deepest type of ID of the given type.
        /// </summary>
        private Dictionary<uint, Dictionary<Type, Type>> deepestTypes = new Dictionary<uint, Dictionary<Type, Type>>();
        #endregion Field: deepestTypes

        #region Field: constructors
        /// <summary>
        /// A dictionary with constructor infos with the 'uint' parameter for ID holders.
        /// </summary>
        private Dictionary<Type, ConstructorInfo> constructors = new Dictionary<Type,ConstructorInfo>();
        #endregion Field: constructors

        #region Field: nameTypes
        /// <summary>
        /// A dictionary to store all types by their name.
        /// </summary>
        private Dictionary<string, Type> nameTypes = new Dictionary<string, Type>();
        #endregion Field: nameTypes

        #endregion Events, Properties, and Fields

        #region Method Group: Constructors

        #region Constructor: Database()
        /// <summary>
        /// Creates a new database.
        /// </summary>
        public Database()
        {
            Thread.CurrentThread.CurrentCulture = CommonSettings.Culture;

            // Make the database aware of table names, node types, and new names
            AddSemanticsResourceSet(GenericTables.ResourceManager.GetResourceSet(CommonSettings.Culture, true, true));
            AddValuesResourceSet(ValueTables.ResourceManager.GetResourceSet(CommonSettings.Culture, true, true));
            AddLocalizationResourceSet(LocalizationTables.ResourceManager.GetResourceSet(CommonSettings.Culture, true, true));
            AddNamesResourceManager(NewNames.ResourceManager);
            AddAssembly(Assembly.GetExecutingAssembly());
        }
        #endregion Constructor: Database()

        #region Method: Initialize()
        /// <summary>
        /// Initialize the database.
        /// </summary>
        public static void Initialize()
        {
            if (Current == null)
            {
                // Make this database the current database
                Current = new Database();
            }
        }
        #endregion Method: Initialize()

        #region Method: Initialize(DatabaseType databaseType, string database)
        /// <summary>
        /// Initialize the database of the given type.
        /// </summary>
        /// <param name="databaseType">The type of the database.</param>
        /// <param name="database">The database to initialize.</param>
        /// <returns>Returns whether the database has been initialized successfully.</returns>
        public void Initialize(DatabaseType databaseType, string database)
        {
            this.IDatabase.Initialize(databaseType, database);
        }
        #endregion Method: Initialize(DatabaseType databaseType, string database)

        #endregion Method Group: Constructors

        #region Method Group: Projects

        #region Method: CreateProject(String file)
        /// <summary>
        /// Create a new database project with the given file name.
        /// </summary>
        /// <param name="file">The database project file to create.</param>
        /// <returns>Returns whether the database project has been created successfully.</returns>
        public static Message CreateProject(String file)
        {
            if (file != null)
            {
                try
                {
                    // First close a possibly loaded project
                    if (Current != null && IsProjectLoaded)
                        CloseProject();

                    // Get the correct file names
                    project = GetFullProjectPath(file);
                    String path = Path.GetDirectoryName(project) + Path.DirectorySeparatorChar;
                    String fileName = Path.GetFileNameWithoutExtension(file);
                    
                    // Create three database files
                    Initialize();
                    Message message = Current.CreateSemanticsDatabase(path + fileName);
                    if (message == Message.CreationFail)
                        return message;
                    message = Current.CreateValuesDatabase(path + fileName);
                    if (message == Message.CreationFail)
                        return message;
                    message = Current.CreateLocalizationDatabase(path + fileName);
                    if (message == Message.CreationFail)
                        return message;
                    CloseProject();

                    // Throw an event
                    try
                    {
                        if (ProjectCreated != null)
                            ProjectCreated(project);
                    }
                    catch (Exception)
                    {
                    }

                    return Message.CreationSuccess;
                }
                catch (Exception)
                {
                }
            }
            return Message.CreationFail;
        }
        #endregion Method: CreateProject(String file)

        #region Method: LoadProject(String file)
        /// <summary>
        /// Load the database project with the given file name.
        /// </summary>
        /// <param name="file">The database project file to load.</param>
        /// <returns>Returns whether the database project has been loaded successfully.</returns>
        public static Message LoadProject(String file)
        {
            if (file != null)
            {
                // Check whether the project to load is not the same as the loaded one
                string fullProjectPath = GetFullProjectPath(file);
                if (fullProjectPath.Equals(project))
                    return Message.AlreadyInitialized;

                try
                {
                    // First close a possibly loaded project
                    if (Current != null && IsProjectLoaded)
                        CloseProject();

                    // Initialize the database when this has not been done before
                    if (Current == null)
                        Initialize();

                    // Get the full project file
                    project = fullProjectPath;

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

                        // Initialize the databases
                        Message message = Current.IDatabase.Initialize(DatabaseType.Semantics, path + semanticsDatabase);
                        foreach (String valuesDatabase in valuesDatabases)
                        {
                            if (Current.IDatabase.Initialize(DatabaseType.Values, path + valuesDatabase) == Message.InitializationSuccess)
                            {
                                Current.valuesDatabases.Add(valuesDatabase);
                                if (Current.currentValuesDatabase == null)
                                    Current.currentValuesDatabase = valuesDatabase;
                            }
                        }
                        foreach (String localizationDatabase in localizationDatabases)
                        {
                            if (Current.IDatabase.Initialize(DatabaseType.Localization, path + localizationDatabase) == Message.InitializationSuccess)
                            {
                                Current.localizationDatabases.Add(localizationDatabase);
                                if (Current.currentLocalizationDatabase == null)
                                    Current.currentLocalizationDatabase = localizationDatabase;
                            }
                        }

                        if (message == Message.InitializationSuccess)
                        {
                            Current.semanticsDatabase = semanticsDatabase;
                            isProjectLoaded = true;
                            try
                            {
                                if (ProjectLoaded != null)
                                    ProjectLoaded(project);
                            }
                            catch (Exception)
                            {
                            }
                        }

                        return message;
                    }
                }
                catch (Exception)
                {
                }
            }

            return Message.InitializationFail;
        }
        #endregion Method: LoadProject(String file)

        #region Method: SaveProject()
        /// <summary>
        /// Save the database project.
        /// </summary>
        /// <returns>Returns whether the database project has been saved successfully.</returns>
        public static Message SaveProject()
        {
            Message message = Current.IDatabase.Save();

            if (message == Message.SaveSuccess)
            {
                try
                {
                    if (ProjectSaved != null)
                        ProjectSaved(project);
                }
                catch (Exception)
                {
                }
            }

            return message;
        }
        #endregion Method: SaveProject()

        #region Method: SaveProjectAs(string file)
        /// <summary>
        /// Save the database project as a new project.
        /// Note: will only work when there is one database of each type (semantics, values, localization)!
        /// </summary>
        /// <param name="file">The path and file name of the new project.</param>
        /// <returns>Returns whether the database project has been saved successfully.</returns>
        public static Message SaveProjectAs(string file)
        {
            try
            {
                if (file != null && project != null)
                {
                    if (File.Exists(project))
                    {
                        // Copy the databases
                        Current.IDatabase.SaveAs(file);

                        // Create the new project file
                        file = GetFullProjectPath(file);
                        string databaseFile = Path.GetFileNameWithoutExtension(file);

                        XmlTextWriter xmlWriter = new XmlTextWriter(file, Encoding.UTF8);
                        xmlWriter.Formatting = Formatting.Indented;
                        xmlWriter.WriteStartElement("Databases");
                        xmlWriter.WriteEndElement();
                        xmlWriter.Close();

                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(file);
                        XmlNode root = xmlDoc.DocumentElement;

                        // Create and append child nodes for all file names
                        XmlElement semanticsNode = xmlDoc.CreateElement("Semantics");
                        semanticsNode.SetAttribute("Database", databaseFile);
                        root.AppendChild(semanticsNode);

                        XmlElement valuesNode = xmlDoc.CreateElement("Values");
                        XmlElement valuesElement = xmlDoc.CreateElement("Database");
                        valuesElement.InnerText = databaseFile;
                        valuesNode.AppendChild(valuesElement);
                        root.AppendChild(valuesNode);

                        XmlElement localizationNode = xmlDoc.CreateElement("Localization");
                        XmlElement localizationElement = xmlDoc.CreateElement("Database");
                        localizationElement.InnerText = databaseFile;
                        localizationNode.AppendChild(localizationElement);
                        root.AppendChild(localizationNode);

                        // Save the file
                        xmlDoc.Save(file);

                        // Throw an event
                        try
                        {
                            if (ProjectSaved != null)
                                ProjectSaved(project);
                        }
                        catch (Exception)
                        {
                        }

                        return Message.SaveSuccess;
                    }
                }
            }
            catch (Exception)
            {
            }
            return Message.SaveFail;
        }
        #endregion Method: SaveProjectAs(string file)

        #region Method: CloseProject()
        /// <summary>
        /// Close the current database project.
        /// </summary>
        /// <returns>Returns whether the database project has been closed successfully.</returns>
        public static Message CloseProject()
        {
            isProjectLoaded = false;

            Current.semanticsDatabase = null;
            Current.currentValuesDatabase = null;
            Current.valuesDatabases.Clear();
            Current.currentLocalizationDatabase = null;
            Current.localizationDatabases.Clear();
            Current.memory.Clear();
            Current.undoStack.Clear();
            Current.redoStack.Clear();

            try
            {
                if (ProjectClosed != null)
                    ProjectClosed(project);
            }
            catch (Exception)
            {
            }

            project = null;

            SemanticsManager.Uninitialize();

            return Current.IDatabase.Uninitialize();
        }
        #endregion Method: CloseProject()

        #region Method: DeleteProject(String file)
        /// <summary>
        /// Delete the database project with the given file name. Note that all databases will be removed as well!
        /// Also note that the project will be closed in the case it is loaded.
        /// </summary>
        /// <param name="file">The database project file to delete.</param>
        /// <returns>Returns whether the database project has been deleted successfully.</returns>
        public static Message DeleteProject(String file)
        {
            if (file != null)
            {
                try
                {
                    // First close a possibly loaded project
                    if (Current != null && IsProjectLoaded)
                        CloseProject();

                    // Get the full project file
                    project = GetFullProjectPath(file);

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

                        // Delete the databases
                        Current.IDatabase.Delete(DatabaseType.Semantics, path + semanticsDatabase);
                        foreach (String valuesDatabase in valuesDatabases)
                            Current.IDatabase.Delete(DatabaseType.Values, path + valuesDatabase);
                        foreach (String localizationDatabase in localizationDatabases)
                            Current.IDatabase.Delete(DatabaseType.Localization, path + localizationDatabase);

                        // Delete the project file
                        File.Delete(project);

                        // Thrown an event
                        try
                        {
                            if (ProjectDeleted != null)
                                ProjectDeleted(project);
                        }
                        catch (Exception)
                        {
                        }

                        return Message.RemovalSuccess;
                    }
                }
                catch (Exception)
                {
                }
            }

            return Message.RemovalFail;
        }
        #endregion Method: DeleteProject(String file)

        #region Method: GetFullProjectPath(String file)
        /// <summary>
        /// Get the full path, including extension, of the given project file.
        /// </summary>
        protected static String GetFullProjectPath(String file)
        {
            if (file != null)
            {
                if (!file.EndsWith(SemanticsSettings.Files.DatabaseProjectExtension, true, CommonSettings.Culture))
                    file += SemanticsSettings.Files.DatabaseProjectExtension;
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
        #endregion Method: GetFullProjectPath(String file)

        #region Method: UpdateProjectFile()
        /// <summary>
        /// Update the project file.
        /// </summary>
        protected void UpdateProjectFile()
        {
            if (project != null)
            {
                // Create a database project file
                XmlTextWriter xmlWriter = new XmlTextWriter(project, Encoding.UTF8);
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.WriteStartElement("Databases");
                xmlWriter.WriteEndElement();
                xmlWriter.Close();
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(project);
                XmlNode root = xmlDoc.DocumentElement;

                // Create and append child nodes for all file names
                XmlElement semanticsNode = xmlDoc.CreateElement("Semantics");
                if (this.SemanticsDatabase != null)
                    semanticsNode.SetAttribute("Database", this.SemanticsDatabase);
                root.AppendChild(semanticsNode);

                XmlElement valuesNode = xmlDoc.CreateElement("Values");
                foreach (String valuesDatabase in this.ValuesDatabases)
                {
                    XmlElement valuesElement = xmlDoc.CreateElement("Database");
                    valuesElement.InnerText = valuesDatabase;
                    valuesNode.AppendChild(valuesElement);
                }
                root.AppendChild(valuesNode);

                XmlElement localizationNode = xmlDoc.CreateElement("Localization");
                foreach (String localizationDatabase in this.LocalizationDatabases)
                {
                    XmlElement localizationElement = xmlDoc.CreateElement("Database");
                    localizationElement.InnerText = localizationDatabase;
                    localizationNode.AppendChild(localizationElement);
                }
                root.AppendChild(localizationNode);

                // Save the file
                xmlDoc.Save(project);
            }
        }
        #endregion Method: UpdateProjectFile()

        #region Method: CreateSemanticsDatabase(String name)
        /// <summary>
        /// Create a new semantics database with the given name.
        /// </summary>
        /// <param name="name">The semantics database to create.</param>
        /// <returns>Returns whether the database has been created successfully.</returns>
        protected virtual Message CreateSemanticsDatabase(String name)
        {
            if (name != null)
            {
                Message message = IDatabase.Create(DatabaseType.Semantics, name, SemanticsSettings.Files.DatabasePath + SemanticsSettings.Files.SemanticsSQLFile);

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
        public virtual Message CreateValuesDatabase(String name)
        {
            if (name != null)
            {
                // Create the database
                Message message = IDatabase.Create(DatabaseType.Values, name, SemanticsSettings.Files.DatabasePath + SemanticsSettings.Files.ValuesSQLFile);

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
        public virtual Message CreateLocalizationDatabase(String name)
        {
            if (name != null)
            {
                // Create the database
                Message message = IDatabase.Create(DatabaseType.Localization, name, SemanticsSettings.Files.DatabasePath + SemanticsSettings.Files.LocalizationSQLFile);

                if (message == Message.CreationSuccess)
                    AddDatabaseToProject(DatabaseType.Localization, name);

                return message;
            }

            return Message.InitializationFail;
        }
        #endregion Method: CreateLocalizationDatabase(String name)

        #region Method: AddDatabaseToProject(DatabaseType databaseType, String name)
        /// <summary>
        /// Add the database of the given type with the given name to the project.
        /// </summary>
        /// <param name="databaseType">The type of the database.</param>
        /// <param name="name">The name of the database.</param>
        protected void AddDatabaseToProject(DatabaseType databaseType, String name)
        {
            if (name != null)
            {
                // Initialize the database
                this.IDatabase.Initialize(databaseType, name);

                // Add the database
                name = Path.GetFileName(name);
                switch (databaseType)
                {
                    case DatabaseType.Semantics:
                        this.semanticsDatabase = name;
                        break;
                    case DatabaseType.Values:
                        this.valuesDatabases.Add(name);
                        break;
                    case DatabaseType.Localization:
                        this.localizationDatabases.Add(name);
                        break;
                    default:
                        break;
                }

                // Update the project file
                UpdateProjectFile();
            }
        }
        #endregion Method: AddDatabaseToProject(DatabaseType databaseType, String name)

        #region Method: SwitchToValuesDatabase(string database)
        /// <summary>
        /// Switch to the given values database.
        /// </summary>
        /// <param name="database">The values database to switch to.</param>
        /// <returns>Returns whether the values database has been switched successfully.</returns>
        public Message SwitchToValuesDatabase(string database)
        {
            Message message = this.IDatabase.SwitchToValuesDatabase(database);

            if (message == Message.SwitchSuccess)
            {
                this.currentValuesDatabase = database;
                this.memory.Clear();
            }

            return message;
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
            // Switch
            Message message = this.IDatabase.SwitchToLocalizationDatabase(database);
            if (message == Message.SwitchSuccess)
            {
                this.currentLocalizationDatabase = database;
                this.memory.Clear();
            }

            return message;
        }
        #endregion Method: SwitchToLocalizationDatabase(string database)

        #region Method: RemoveValuesDatabase(String name)
        /// <summary>
        /// Remove the values database with the given name.
        /// </summary>
        /// <param name="name">The values database to remove.</param>
        /// <returns>Returns whether the database has been removed successfully.</returns>
        public Message RemoveValuesDatabase(String name)
        {
            if (name != null)
            {
                // Remove the database
                if (this.IDatabase.Remove(DatabaseType.Values, name) == Message.RemovalSuccess)
                {
                    this.valuesDatabases.Remove(name);
                    UpdateProjectFile();

                    if (name.Equals(this.currentValuesDatabase) && this.valuesDatabases.Count > 0)
                        this.currentValuesDatabase = this.valuesDatabases[0];

                    return Message.RemovalSuccess;
                }
            }

            return Message.RemovalFail;
        }
        #endregion Method: RemoveValuesDatabase(String name)

        #region Method: RemoveLocalizationDatabase(String name)
        /// <summary>
        /// Remove the localization database with the given name.
        /// </summary>
        /// <param name="name">The localization database to remove.</param>
        /// <returns>Returns whether the database has been removed successfully.</returns>
        public Message RemoveLocalizationDatabase(String name)
        {
            if (name != null)
            {
                // Remove the database
                if (this.IDatabase.Remove(DatabaseType.Localization, name) == Message.RemovalSuccess)
                {
                    this.localizationDatabases.Remove(name);
                    UpdateProjectFile();

                    if (name.Equals(this.currentLocalizationDatabase) && this.localizationDatabases.Count > 0)
                        this.currentLocalizationDatabase = this.localizationDatabases[0];

                    return Message.RemovalSuccess;
                }
            }

            return Message.RemovalFail;
        }
        #endregion Method: RemoveLocalizationDatabase(String name)

        #endregion Method Group: Projects

        #region Method Group: General

        #region Method: AddSemanticsResourceSet(ResourceSet resourceSet)
        /// <summary>
        /// Add a resource set in which the semantic tables are listed.
        /// </summary>
        /// <param name="resourceSet">A resource set with referred tables as keys, and actual SQLite table names as values.</param>
        protected void AddSemanticsResourceSet(ResourceSet resourceSet)
        {
            if (resourceSet != null)
            {
                foreach (DictionaryEntry entry in resourceSet)
                {
                    string key = (string)entry.Key;
                    if (!this.resourceTableTable.ContainsKey(key))
                    {
                        this.resourceTableTable.Add(key, (string)entry.Value);
                        this.tableDatabaseType.Add((string)entry.Value, DatabaseType.Semantics);
                    }
                }
            }
        }
        #endregion Method: AddSemanticsResourceSet(ResourceSet resourceSet)

        #region Method: AddValuesResourceSet(ResourceSet resourceSet)
        /// <summary>
        /// Add a resource set in which the value tables are listed.
        /// </summary>
        /// <param name="resourceSet">A resource set with referred tables as keys, and actual SQLite table names as values.</param>
        protected void AddValuesResourceSet(ResourceSet resourceSet)
        {
            if (resourceSet != null)
            {
                foreach (DictionaryEntry entry in resourceSet)
                {
                    string key = (string)entry.Key;
                    if (!this.resourceTableTable.ContainsKey(key))
                    {
                        this.resourceTableTable.Add(key, (string)entry.Value);
                        this.tableDatabaseType.Add((string)entry.Value, DatabaseType.Values);
                    }
                }
            }
        }
        #endregion Method: AddValuesResourceSet(ResourceSet resourceSet)

        #region Method: AddLocalizationResourceSet(ResourceSet resourceSet)
        /// <summary>
        /// Add a resource set in which the localization tables are listed.
        /// </summary>
        /// <param name="resourceSet">A resource set with referred tables as keys, and actual SQLite table names as values.</param>
        protected void AddLocalizationResourceSet(ResourceSet resourceSet)
        {
            if (resourceSet != null)
            {
                foreach (DictionaryEntry entry in resourceSet)
                {
                    string key = (string)entry.Key;
                    if (!this.resourceTableTable.ContainsKey(key))
                    {
                        this.resourceTableTable.Add(key, (string)entry.Value);
                        this.tableDatabaseType.Add((string)entry.Value, DatabaseType.Localization);
                    }
                }
            }
        }
        #endregion Method: AddLocalizationResourceSet(ResourceSet resourceSet)

        #region Method: AddNamesResourceManager(ResourceManager resourceManager)
        /// <summary>
        /// Add a resource manager in which the new names of nodes are listed.
        /// </summary>
        /// <param name="resourceManager">A resource manager with referred type names as keys, and actual new names as values.</param>
        protected void AddNamesResourceManager(ResourceManager resourceManager)
        {
            if (resourceManager != null)
                this.namesResourceManagers.Add(resourceManager);
        }
        #endregion Method: AddNamesResourceManager(ResourceManager resourceManager)

        #region Method: AddAssembly(Assembly assembly)
        /// <summary>
        /// Add an assembly in which nodes are present.
        /// </summary>
        /// <param name="assembly">The assembly that contains one or more nodes.</param>
        protected void AddAssembly(Assembly assembly)
        {
            if (assembly != null && !this.assemblies.Contains(assembly))
                this.assemblies.Add(assembly);
        }
        #endregion Method: AddAssembly(Assembly assembly)

        #region Method: AddTableDefinition(string table, Dictionary<string, Type owner, Tuple<Type, EntryType>> columnsTypes)
        /// <summary>
        /// Define which types can be found in which columns of which table. Only useful for referenced nodes.
        /// </summary>
        /// <param name="table">The table to add the definition for.</param>
        /// <param name="owner">The owner of the table.</param>
        /// <param name="columnsTypes">A dictionary with column names as keys, and tuples as values. The tuples contain the type of the column, and the type of the entry.</param>
        protected internal void AddTableDefinition(string table, Type owner, Dictionary<string, Tuple<Type, EntryType>> columnsTypes)
        {
            this.tableColumnType.Add(new Tuple<string, Tuple<Type, Dictionary<string, Tuple<Type, EntryType>>>>(table, new Tuple<Type, Dictionary<string, Tuple<Type, EntryType>>>(owner, columnsTypes)));
        }
        #endregion Method: AddTableDefinition(string table, Dictionary<string, Type owner, Tuple<Type, EntryType>> columnsTypes)

        #region Method: GetDatabaseType(string table)
        /// <summary>
        /// Get the correct database type for the given table.
        /// </summary>
        /// <param name="table">The table to get the database type of.</param>
        /// <returns>The database type for the given table.</returns>
        protected DatabaseType GetDatabaseType(string table)
        {
            if (table != null)
            {
                DatabaseType databaseType;
                if (this.tableDatabaseType.TryGetValue(table, out databaseType))
                    return databaseType;

                throw new Exception(Exceptions.NoDatabaseConnection);
            }

            throw new Exception(Exceptions.TableDoesNotExist);
        }
        #endregion Method: GetDatabaseType(string table)

        #region Method: GetTable(string resourceTable)
        /// <summary>
        /// Get the table name for the table that is stored as a resource.
        /// </summary>
        /// <param name="resourceTable">The table name in the resources.</param>
        /// <returns>The actual table name of the table.</returns>
        private string GetTable(string resourceTable)
        {
            string table = null;
            this.resourceTableTable.TryGetValue(resourceTable, out table);
            return table;
        }
        #endregion Method: GetTable(string resourceTable)

        #region Method: GetNewNodeName(Type type)
        /// <summary>
        /// Get the new name for a node of the given type.
        /// </summary>
        /// <param name="type">The type of the node.</param>
        /// <returns>The new name for a node of the given type.</returns>
        public string GetNewNodeName(Type type)
        {
            if (type != null)
                return GetNewNodeName(type.Name);

            return SemanticsSettings.General.NewName;
        }
        #endregion Method: GetNewNodeName(Type type)

        #region Method: GetNewNodeName(string resourceName)
        /// <summary>
        /// Get the new name for a node, which is stored as a resource.
        /// </summary>
        /// <param name="resourceName">The resource name for the node.</param>
        /// <returns>The new name for the node with the resource name.</returns>
        internal string GetNewNodeName(string resourceName)
        {
            foreach (ResourceManager resourceManager in this.namesResourceManagers)
            {
                string name = resourceManager.GetString(resourceName, NewNames.Culture);
                if (name != null)
                    return name;
            }

            return SemanticsSettings.General.NewName;
        }
        #endregion Method: GetNewNodeName(string resourceName)

        #region Method: Undo()
        /// <summary>
        /// Undo the previous database modification.
        /// </summary>
        public void Undo()
        {
            if (CanUndo)
            {
                // Pop the topmost tuple list from the stack
                Tuple<int, List<Tuple<DatabaseType, string, string>>> undoTuple = this.undoStack.Pop();
                List<Tuple<DatabaseType, string, string>> list = undoTuple.Item2;

                // Check whether this action added a node, meaning it should be removed now
                bool skip = false;
                if (NodeRemoved != null)
                {
                    foreach (Tuple<int, uint> tuple in this.additionMoments)
                    {
                        if (tuple.Item1.Equals(undoTuple.Item1))
                        {
                            NodeRemoved(DatabaseSearch.GetNode(tuple.Item2));
                            this.additionMoments.Remove(tuple);
                            this.removalMoments.Add(new Tuple<int, uint>(this.undoRedoCounter, tuple.Item2));
                            skip = true;
                            break;
                        }
                    }
                }

                // Execute the undo queries
                foreach (Tuple<DatabaseType, string, string> tuple in list)
                    this.IDatabase.Query(tuple.Item1, tuple.Item3);

                // Push the undo tuple on top of the redo stack
                this.redoStack.Push(undoTuple);

                // Check whether this action removed a node, meaning it should be added now
                if (!skip && NodeAdded != null)
                {
                    foreach (Tuple<int, uint> tuple in this.removalMoments)
                    {
                        if (tuple.Item1.Equals(undoTuple.Item1))
                        {
                            NodeAdded(DatabaseSearch.GetNode(tuple.Item2));
                            this.removalMoments.Remove(tuple);
                            this.additionMoments.Add(new Tuple<int, uint>(this.undoRedoCounter, tuple.Item2));
                            break;
                        }
                    }
                }
            }
        }
        #endregion Method: Undo()

        #region Method: Redo()
        /// <summary>
        /// Redo the previous database modification.
        /// </summary>
        public void Redo()
        {
            if (CanRedo)
            {
                // Pop the topmost tuple list from the stack
                Tuple<int, List<Tuple<DatabaseType, string, string>>> redoTuple = this.redoStack.Pop();
                List<Tuple<DatabaseType, string, string>> list = redoTuple.Item2;

                // Check whether this action removed a node
                if (NodeRemoved != null)
                {
                    foreach (Tuple<int, uint> tuple in this.removalMoments)
                    {
                        if (tuple.Item1.Equals(redoTuple.Item1))
                        {
                            NodeRemoved(DatabaseSearch.GetNode(tuple.Item2));
                            this.removalMoments.Remove(tuple);
                            this.removalMoments.Add(new Tuple<int, uint>(this.undoRedoCounter, tuple.Item2));
                            break;
                        }
                    }
                }

                // Execute the redo queries
                foreach (Tuple<DatabaseType, string, string> tuple in list)
                    this.IDatabase.Query(tuple.Item1, tuple.Item2);

                // Push the redo tuple on top of the undo stack
                this.undoStack.Push(redoTuple);

                // Check whether this action added a node
                if (NodeAdded != null)
                {
                    foreach (Tuple<int, uint> tuple in this.additionMoments)
                    {
                        if (tuple.Item1.Equals(redoTuple.Item1))
                        {
                            NodeAdded(DatabaseSearch.GetNode(tuple.Item2));
                            this.additionMoments.Remove(tuple);
                            this.additionMoments.Add(new Tuple<int, uint>(this.undoRedoCounter, tuple.Item2));
                            break;
                        }
                    }
                }
            }
        }
        #endregion Method: Redo()

        #region Method: StartChange()
        /// <summary>
        /// Indicate that multiple changes will be executed on the database.
        /// </summary>
        protected internal void StartChange()
        {
            this.numberOfStartedChanges++;
        }
        #endregion Method: StartChange()

        #region Method: StopChange()
        /// <summary>
        /// Indicate that multiple changes have finished executing on the database.
        /// </summary>
        protected internal void StopChange()
        {
            this.numberOfStartedChanges--;
        }
        #endregion Method: StopChange()

        #region Method: StartRemove()
        /// <summary>
        /// Indicate that a node removal is started.
        /// </summary>
        protected internal void StartRemove()
        {
            // Add a new tuple list to the undo stack
            List<Tuple<DatabaseType, string, string>> list = new List<Tuple<DatabaseType, string, string>>();
            this.undoRedoCounter++;
            this.undoStack.Push(new Tuple<int, List<Tuple<DatabaseType, string, string>>>(this.undoRedoCounter, list));
        }
        #endregion Method: StartRemove()

        #region Method: QueryChange(DatabaseType databaseType, string query, string undoQuery)
        /// <summary>
        /// Perform a query and add both query and the undo query to the redo and undo stack.
        /// </summary>
        /// <param name="databaseType">The database type.</param>
        /// <param name="query">The query.</param>
        /// <param name="undoQuery">The undo query.</param>
        /// <returns>Returns whether the query has been executed successfully.</returns>
        public bool QueryChange(DatabaseType databaseType, string query, string undoQuery)
        {
            if (this.IDatabase.Query(databaseType, query))
            {
                SetUndoRedo(databaseType, query, undoQuery);

                if (DatabaseChanged != null)
                    DatabaseChanged();

                return true;
            }
            return false;
        }
        #endregion Method: QueryChange(DatabaseType databaseType, string query, string undoQuery)

        #region Method: QueryInsertRemoveChange(DatabaseType databaseType, string query, string undoQuery)
        /// <summary>
        /// Perform a query and add both query and the undo query to the redo and undo stack.
        /// </summary>
        /// <param name="databaseType">The database type.</param>
        /// <param name="query">The query.</param>
        /// <param name="undoQuery">The undo query.</param>
        /// <returns>Returns whether the query has been executed successfully.</returns>
        public bool QueryInsertRemoveChange(DatabaseType databaseType, string query, string undoQuery)
        {
            if (this.IDatabase.QueryAll(databaseType, query))
            {
                SetUndoRedo(databaseType, query, undoQuery);

                if (DatabaseChanged != null)
                    DatabaseChanged();

                return true;
            }
            return false;
        }
        #endregion Method: QueryInsertRemoveChange(DatabaseType databaseType, string query, string undoQuery)

        #region Method: SetUndoRedo(DatabaseType databaseType, string query, string undoQuery)
        /// <summary>
        /// Set the undo and redo stack after a query change.
        /// </summary>
        /// <param name="databaseType">The database type.</param>
        /// <param name="query">The query.</param>
        /// <param name="undoQuery">The undo query.</param>
        private void SetUndoRedo(DatabaseType databaseType, string query, string undoQuery)
        {
            this.redoStack.Clear();

            if (this.numberOfStartedChanges == 0 || this.undoStack.Count == 0)
            {
                // Add a new tuple list to the undo stack
                List<Tuple<DatabaseType, string, string>> list = new List<Tuple<DatabaseType, string, string>>();
                list.Add(new Tuple<DatabaseType, string, string>(databaseType, query, undoQuery));
                this.undoRedoCounter++;
                this.undoStack.Push(new Tuple<int, List<Tuple<DatabaseType, string, string>>>(this.undoRedoCounter, list));
            }
            else
            {
                // Add a new tuple to the topmost list on the undo stack
                if (this.undoStack.Count > 0)
                    this.undoStack.Peek().Item2.Add(new Tuple<DatabaseType, string, string>(databaseType, query, undoQuery));
            }
        }
        #endregion Method: SetUndoRedo(DatabaseType databaseType, string query, string undoQuery)
		
        #region Method: QueryBegin()
        /// <summary>
        /// Begins a combined query. Until QueryCommit() is called, all queries will be temporarily stored, and not yet executed.
        /// </summary>
        public void QueryBegin()
        {
            this.IDatabase.QueryBegin();
        }
        #endregion Method: QueryBegin()

        #region Method: QueryCommit()
        /// <summary>
        /// Commits all temporarily stored queries. Use only after calling QueryBegin()!
        /// </summary>
        public void QueryCommit()
        {
            this.IDatabase.QueryCommit();
        }
        #endregion Method: QueryCommit()

        #region Method: QueryAllBegin()
        /// <summary>
        /// Begins a combined query. Until QueryAllCommit() is called, all queries will be temporarily stored, and not yet executed.
        /// </summary>
        public void QueryAllBegin()
        {
            this.IDatabase.QueryAllBegin();
        }
        #endregion Method: QueryAllBegin()

        #region Method: QueryAllCommit()
        /// <summary>
        /// Commits all temporarily stored queries. Use only after calling QueryAllBegin()!
        /// </summary>
        public void QueryAllCommit()
        {
            this.IDatabase.QueryAllCommit();
        }
        #endregion Method: QueryAllCommit()

        #endregion Method Group: General

        #region Method Group: Select

        #region Method: Select(string table, string column)
        /// <summary>
        /// Select an item from a table.
        /// </summary>
        /// <param name="table">The table to select an item from.</param>
        /// <param name="column">The column to look in.</param>
        /// <returns>The object that was found.</returns>
        private object Select(string table, string column)
        {
            return this.IDatabase.QuerySelect(GetDatabaseType(table), "SELECT " + column + " FROM " + table + ";");
        }
        #endregion Method: Select(string table, string column)

        #region Method: Select(string table, string column, string condition)
        /// <summary>
        /// Select an item from a table given a condition.
        /// </summary>
        /// <param name="table">The table to select an item from.</param>
        /// <param name="column">The column to look in.</param>
        /// <param name="condition">The condition on the items.</param>
        /// <returns>The object that was found.</returns>
        private object Select(string table, string column, string condition)
        {
            return this.IDatabase.QuerySelect(GetDatabaseType(table), "SELECT " + column + " FROM " + table + " WHERE " + condition + ";");
        }
        #endregion Method: Select(string table, string column, string condition)

        #region Method: Select(string table, string column, string conditionColumn, object value)
        /// <summary>
        /// Select an item from the column, having the given value in the condition column.
        /// </summary>
        /// <param name="table">The table to select an item from.</param>
        /// <param name="column">The column to look in.</param>
        /// <param name="conditionColumn">The column to define a condition for.</param>
        /// <param name="value">The value that should be in the condition column.</param>
        /// <returns>The object that was found.</returns>
        private object Select(string table, string column, string conditionColumn, object value)
        {
            value = ConvertValue(value);

            return this.IDatabase.QuerySelect(GetDatabaseType(table), "SELECT " + column + " FROM " + table + " WHERE " + conditionColumn + " = '" + value + "';");
        }
        #endregion Method: Select(string table, string column, string conditionColumn, object value)

        #region Method: Select(uint id, string table, string column)
        /// <summary>
        /// Select the first entry from a table for the given ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <param name="table">The table to select an item from.</param>
        /// <param name="column">The column to look in.</param>
        /// <returns>The object that was found.</returns>
        private object Select(uint id, string table, string column)
        {
            return this.IDatabase.QuerySelect(GetDatabaseType(table), "SELECT " + column + " FROM " + table + " WHERE " + Columns.ID + " = '" + id + "';");
        }
        #endregion Method: Select(uint id, string table, string column)

        #region Method: Select<T>(string table, string column)
        /// <summary>
        /// Select an item of the given type from a table.
        /// </summary>
        /// <typeparam name="T">The type the return item should be of.</typeparam>
        /// <param name="table">The table to select an item from.</param>
        /// <param name="column">The column to look in.</param>
        /// <returns>The object that was found.</returns>
        private T Select<T>(string table, string column)
        {
            return ConvertObject<T>(this.IDatabase.QuerySelect(GetDatabaseType(table), "SELECT " + column + " FROM " + table + ";"), typeof(T));
        }
        #endregion Method: Select<T>(string table, string column, string condition)

        #region Method: Select<T>(string table, string column, string condition)
        /// <summary>
        /// Select an item of the given type from a table given a condition.
        /// </summary>
        /// <typeparam name="T">The type the return item should be of.</typeparam>
        /// <param name="table">The table to select an item from.</param>
        /// <param name="column">The column to look in.</param>
        /// <param name="condition">The condition on the items.</param>
        /// <returns>The object that was found.</returns>
        private T Select<T>(string table, string column, string condition)
        {
            return ConvertObject<T>(this.IDatabase.QuerySelect(GetDatabaseType(table), "SELECT " + column + " FROM " + table + " WHERE " + condition + ";"), typeof(T));
        }
        #endregion Method: Select<T>(string table, string column, string condition)

        #region Method: Select<T>(string table, string column, string conditionColumn, object value)
        /// <summary>
        /// Select an item of the given type from the column, having the given value in the condition column.
        /// </summary>
        /// <typeparam name="T">The type the item should be of.</typeparam>
        /// <param name="table">The table to select an item from.</param>
        /// <param name="column">The column to look in.</param>
        /// <param name="conditionColumn">The column to define a condition for.</param>
        /// <param name="value">The value that should be in the condition column.</param>
        /// <returns>The object that was found.</returns>
        protected internal T Select<T>(string table, string column, string conditionColumn, object value)
        {
            value = ConvertValue(value);

            return ConvertObject<T>(this.IDatabase.QuerySelect(GetDatabaseType(table), "SELECT " + column + " FROM " + table + " WHERE " + conditionColumn + " = '" + value + "';"), typeof(T));
        }
        #endregion Method: Select<T>(string table, string column, string conditionColumn, object value)

        #region Method: Select<T>(uint id, string table, string column)
        /// <summary>
        /// Select an item of the given type from a table for the given ID.
        /// </summary>
        /// <typeparam name="T">The type the item should be of.</typeparam>
        /// <param name="id">The ID.</param>
        /// <param name="table">The table to select an item from.</param>
        /// <param name="column">The column to look in.</param>
        /// <returns>The object that was found.</returns>
        protected internal T Select<T>(uint id, string table, string column)
        {
            return ConvertObject<T>(Select(id, table, column), typeof(T));
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
        protected internal T Select<T>(uint id, string table, string column, string conditionColumn, object value)
        {
            value = ConvertValue(value);

            return ConvertObject<T>(this.IDatabase.QuerySelect(GetDatabaseType(table), "SELECT " + column + " FROM " + table + " WHERE " + Columns.ID + " = '" + id + "' AND " + conditionColumn + " = '" + value + "';"), typeof(T));
        }
        #endregion Method: Select<T>(uint id, string table, string column, string conditionColumn, object value)

        #region Method: SelectAll(uint id, string table, string column)
        /// <summary>
        /// Select all items from a table for the given ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <param name="table">The table to select the items from.</param>
        /// <param name="column">The column to look in.</param>
        /// <returns>The objects that were found.</returns>
        private List<object> SelectAll(uint id, string table, string column)
        {
            return this.IDatabase.QuerySelectAll(GetDatabaseType(table), "SELECT " + column + " FROM " + table + " WHERE " + Columns.ID + " = '" + id + "';");
        }
        #endregion Method: SelectAll(uint id, string table, string column)
        
        #region Method: SelectAll<T>(uint id, string table, string column)
        /// <summary>
        /// Select all items in the given column from the given table for the given ID.
        /// </summary>
        /// <typeparam name="T">The type the items should be of.</typeparam>
        /// <param name="id">The ID.</param>
        /// <param name="table">The table to select the items from.</param>
        /// <param name="column">The column to look in.</param>
        /// <returns>The objects that were found.</returns>
        public List<T> SelectAll<T>(uint id, string table, string column)
        {
            // Example:
            // --------------------------------
            // physical_object_valued_attribute
            // --------------------------------
            // ID  |  valued_attribute
            // --------------------------------
            // 1   |  123456
            // 1   |  987654
            // 2   |  123456
            // 2   |  111112
            // 3   |  111112
            //
            // id = 2; table = physical_object_valued_attribute; column = valued_attribute
            // ---> 123456, 111112

            // Find all entries and convert them to the requested type
            return ConvertObjects<T>(SelectAll(id, table, column), typeof(T));
        }
        #endregion Method: SelectAll<T>(uint id, string table, string column)

        #region Method: SelectAll<T>(string table)
        /// <summary>
        /// Select all items in the given table
        /// </summary>
        /// <typeparam name="T">The type the items should be of.</typeparam>
        /// <param name="table">The table to select the items from.</param>
        /// <returns>The objects that were found.</returns>
        public List<T> SelectAll<T>(string table)
        {
            return ConvertObjects<T>(this.IDatabase.QuerySelectAll(GetDatabaseType(table), "SELECT " + Columns.ID + " FROM " + table + ";"), typeof(T));
        }
        #endregion Method: SelectAll<T>(string table)

        #region Method: SelectAll<T>(string table, string column, object value)
        /// <summary>
        /// Select all items in the given table where the given column has the given value.
        /// </summary>
        /// <typeparam name="T">The type the items should be of.</typeparam>
        /// <param name="table">The table to select the items from.</param>
        /// <param name="column">The column to look in.</param>
        /// <param name="value">The value that should be in the column.</param>
        /// <returns>The objects that were found.</returns>
        protected internal List<T> SelectAll<T>(string table, string column, object value)
        {
            return SelectAll<T>(table, column, value, typeof(T));
        }
        #endregion Method: SelectAll<T>(string table, string column, object value)

        #region Method: SelectAll<T>(string table, string column, object value, Type type)
        /// <summary>
        /// Select all items in the given table where the given column has the given value.
        /// </summary>
        /// <typeparam name="T">The type the items should get, can be abstract.</typeparam>
        /// <param name="table">The table to select the items from.</param>
        /// <param name="column">The column to look in.</param>
        /// <param name="value">The value that should be in the column.</param>
        /// <param name="type">The type of the objects, should not be abstract.</param>
        /// <returns>The objects that were found.</returns>
        private List<T> SelectAll<T>(string table, string column, object value, Type type)
        {
            // Example:
            // --------------------------------
            // physical_object_valued_attribute
            // --------------------------------
            // ID  |  valued_attribute
            // --------------------------------
            // 1   |  123456
            // 1   |  987654
            // 2   |  123456
            // 2   |  111112
            // 3   |  111112
            //
            // table = physical_object_valued_attribute; column = valued_attribute, value = 111112
            // ---> 2, 3

            value = ConvertValue(value);

            // Get all objects that have the given value in the given column
            return ConvertObjects<T>(this.IDatabase.QuerySelectAll(GetDatabaseType(table), "SELECT " + Columns.ID + " FROM " + table + " WHERE " + column + " = '" + value + "';"), type);
        }
        #endregion Method: SelectAll<T>(string table, string column, object value, Type type)

        #region Method: SelectAll<T>(string table, string column, object value)
        /// <summary>
        /// Select all items in the given table where the given columns have the given values.
        /// </summary>
        /// <typeparam name="T">The type the items should be of.</typeparam>
        /// <param name="table">The table to select the items from.</param>
        /// <param name="columns">The columns to look in.</param>
        /// <param name="values">The values that should be in the column.</param>
        /// <returns>The objects that were found.</returns>
        protected internal List<T> SelectAll<T>(string table, string[] columns, object[] values)
        {
            return SelectAll<T>(table, columns, values, typeof(T));
        }
        #endregion Method: SelectAll<T>(string table, string column, object value)

        #region Method: SelectAll<T>(string table, string column, object value, Type type)
        /// <summary>
        /// Select all items in the given table where the given columns have the given values.
        /// </summary>
        /// <typeparam name="T">The type the items should get, can be abstract.</typeparam>
        /// <param name="table">The table to select the items from.</param>
        /// <param name="columns">The columns to look in.</param>
        /// <param name="values">The values that should be in the columns.</param>
        /// <param name="type">The type of the objects, should not be abstract.</param>
        /// <returns>The objects that were found.</returns>
        private List<T> SelectAll<T>(string table, string[] columns, object[] values, Type type)
        {
            // Example:
            // --------------------------------
            // ID  |  column1   |  column2
            // --------------------------------
            // 1   |  123456    |  true
            // 1   |  987654    |  false
            // 2   |  123456    |  true
            // 2   |  111112    |  false
            // 3   |  111112    |  true
            //
            // values[0] = 111112; values[1] = true
            // ---> 3

            if (columns != null && values != null && columns.Length == values.Length && columns.Length > 0)
            {
                List<object> convertedValues = new List<object>();
                foreach (object obj in values)
                    convertedValues.Add(ConvertValue(obj));

                string query = "SELECT " + Columns.ID + " FROM " + table + " WHERE ";
                for (int i = 0; i < columns.Length; i++)
                {
                    if (i != 0)
                        query += " AND ";
                    query += columns[i] + "='" + convertedValues[i] + "'";
                }
                query += ";";

                // Get all objects that have the given value in the given column
                return ConvertObjects<T>(this.IDatabase.QuerySelectAll(GetDatabaseType(table), query), type);
            }
            return null;
        }
        #endregion Method: SelectAll<T>(string table, string column, object value, Type type)

        #region Method: SelectAll<T>(bool distinct, string table, string column)
        /// <summary>
        /// Select all DISTINCT column entries in the given table.
        /// </summary>
        /// <typeparam name="T">The type the items should get, can be abstract.</typeparam>
        /// <param name="distinct">Indicates whether the distinct columns should be selected.</param>
        /// <param name="table">The table to select the items from.</param>
        /// <param name="column">The column to look in.</param>
        /// <returns>The objects that were found.</returns>
        protected internal List<T> SelectAll<T>(bool distinct, string table, string column)
        {
            // Example:
            // --------------------------------
            // physical_object_valued_attribute
            // --------------------------------
            // ID  |  valued_attribute
            // --------------------------------
            // 1   |  123456
            // 1   |  987654
            // 2   |  123456
            // 2   |  111112
            // 3   |  111112
            //
            // table = physical_object_valued_attribute; column = valued_attribute
            // ---> 123456, 987654, 111112
            string dist = "";
            if (distinct)
                dist = "DISTINCT ";
            return ConvertObjects<T>(this.IDatabase.QuerySelectAll(GetDatabaseType(table), "SELECT " + dist + column + " FROM " + table + ";"), typeof(T));
        }
        #endregion Method: SelectAll<T>(bool distinct, string table, string column)

        #region Method: SelectAll<T>(bool distinct, uint id, string table, string column)
        /// <summary>
        /// Select all DISTINCT column entries in the given table.
        /// </summary>
        /// <typeparam name="T">The type the items should get, can be abstract.</typeparam>
        /// <param name="distinct">Indicates whether the distinct columns should be selected.</param>
        /// <param name="table">The table to select the items from.</param>
        /// <param name="column">The column to look in.</param>
        /// <returns>The objects that were found.</returns>
        protected internal List<T> SelectAll<T>(bool distinct, uint id, string table, string column)
        {
            // Example:
            // --------------------------------
            // physical_object_valued_attribute
            // --------------------------------
            // ID  |  valued_attribute
            // --------------------------------
            // 1   |  123456
            // 1   |  987654
            // 2   |  123456
            // 2   |  111112
            // 3   |  111112
            //
            // table = physical_object_valued_attribute; column = valued_attribute
            // ---> 123456, 987654, 111112
            string dist = "";
            if (distinct)
                dist = "DISTINCT ";
            return ConvertObjects<T>(this.IDatabase.QuerySelectAll(GetDatabaseType(table), "SELECT " + dist + column + " FROM " + table + " WHERE " + Columns.ID + " = '" + id + "';"), 
                typeof(T));
        }
        #endregion Method: SelectAll<T>(bool distinct, uint id, string table, string column)

        #region Method: SelectAll<T>(string column, string table, string conditionColumn, object value)
        /// <summary>
        /// Select all items of the given type from the given column in the given table, where the given condition column has the given value.
        /// </summary>
        /// <typeparam name="T">The type the items should be of.</typeparam>
        /// <param name="column">The column to look in.</param>
        /// <param name="table">The table to select the items from.</param>
        /// <param name="conditionColumn">The column to define a condition for.</param>
        /// <param name="value">The value that should be in the column.</param>
        /// <returns>The objects that were found.</returns>
        protected internal List<T> SelectAll<T>(string column, string table, string conditionColumn, object value)
        {
            // Example:
            // -------------------------------------------------
            // tangible_object_connection_item
            // -------------------------------------------------
            // ID  |  tangible_object  |  tangible_object_valued
            // -------------------------------------------------
            // 1   |  123456           |  14789
            // 1   |  987654           |  96321
            // 2   |  123456           |  41236
            // 2   |  111112           |  75319
            // 3   |  111112           |  26843
            //
            // column = tangible_object_valued, condition_column = tangible_object, value = 111112
            // ---> 75319, 26843

            value = ConvertValue(value);
            
            // Get all objects of the column where the condition column has the value
            return ConvertObjects<T>(this.IDatabase.QuerySelectAll(GetDatabaseType(table), "SELECT " + column + " FROM " + table + " WHERE " + conditionColumn + " = '" + value + "';"), typeof(T));
        }
        #endregion Method: SelectAll<T>(string column, string table, string conditionColumn, object value)

        #region Method: SelectAll<T, T2>(uint id, string table, string column1, string column2)
        /// <summary>
        /// Select all items in the given columns from the given table for the given ID.
        /// </summary>
        /// <typeparam name="T">The type the items should be of.</typeparam>
        /// <param name="id">The ID.</param>
        /// <param name="table">The table to select the items from.</param>
        /// <param name="column1">The first column to get the results from.</param>
        /// <param name="column2">The second column to get the results from.</param>
        /// <returns>The objects that were found, stored in a dictionary with column1-column2 key-value pairs.</returns>
        protected internal Dictionary<T, T2> SelectAll<T, T2>(uint id, string table, string column1, string column2)
        {
            // Example:
            // --------------------------------
            // ID  |  column1  |  column2
            // --------------------------------
            // 1   |  123456   |  333333
            // 1   |  987654   |  444444
            // 2   |  123456   |  555555
            // 2   |  111112   |  666666
            // 3   |  111112   |  777777
            //
            // id = 2
            // ---> <123456, 555555>
            //      <111112, 666666>

            // First get everything from the first column
            List<T> column1Results = ConvertObjects<T>(SelectAll(id, table, column1), typeof(T));

            // Then get the results from the second column
            List<T2> column2Results = ConvertObjects<T2>(SelectAll(id, table, column2), typeof(T2));

            // Create a dictionary to return
            Dictionary<T, T2> dictionary = new Dictionary<T, T2>();
            if (column1Results.Count == column2Results.Count)
            {
                for (int i = 0; i < column1Results.Count; i++)
                    dictionary.Add(column1Results[i], column2Results[i]);
            }

            return dictionary;
        }
        #endregion Method: SelectAll<T, T2>(uint id, string table, string column1, string column2)

        #endregion Method Group: Select

        #region Method Group: Conversions

        #region Method: ConvertObject<T>(object obj, Type type)
        /// <summary>
        /// Convert the given object to the given type.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="obj">The object to convert.</param>
        /// <param name="type">The type of the objects.</param>
        /// <returns>The converted object.</returns>
        protected internal T ConvertObject<T>(object obj, Type type)
        {
            if (obj != null)
            {
                List<object> list = new List<object>(1);
                list.Add(obj);
                return ConvertObjects<T>(list, type)[0];
            }
            return default(T);
        }
        #endregion Method: ConvertObject<T>(object obj, Type type)
        
        #region Method: ConvertObjects<T>(List<object> objects, Type type)
        /// <summary>
        /// Convert the objects to the given type.
        /// </summary>
        /// <typeparam name="T">The type to convert to, can be abstract.</typeparam>
        /// <param name="objects">The objects to convert.</param>
        /// <param name="type">The type of the objects, should not be abstract.</param>
        /// <returns>The converted objects.</returns>
        internal List<T> ConvertObjects<T>(List<object> objects, Type type)
        {
            List<T> objectsToReturn = new List<T>();

            if (objects != null && objects.Count > 0)
            {
                try
                {
                    // Convert all nulls to their default value
                    for (int i = 0; i < objects.Count; i++)
                    {
                        if (objects[i] == null)
                        {
                            objectsToReturn.Add(default(T));
                            objects.RemoveAt(i);
                            i--;
                        }
                    }

                    // When the type is nullable, make sure to pick the underlying type
                    if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                        type = Nullable.GetUnderlyingType(type);
                    
                    // Make the correct conversion
                    Type baseType = type;
                    while (baseType != null)
                    {
                        if (baseType.Equals(typeof(IdHolder)))
                        {
                            foreach (object obj in objects)
                            {
                                // We are looking for a specific ID holder
                                if (obj is long)
                                    objectsToReturn.Add(ConvertToObject<T>(Convert.ToUInt32((long)obj), type));
                                else if (obj is uint)
                                    objectsToReturn.Add(ConvertToObject<T>((uint)obj, type));
                            }
                        }
                        baseType = baseType.BaseType;
                    }
                    if (type.Equals(typeof(object)))
                    {
                        foreach (object obj in objects)
                            objectsToReturn.Add((T)obj);
                    }
                    else if (type.Equals(typeof(uint)))
                    {
                        foreach (object obj in objects)
                        {
                            if (obj is long)
                                objectsToReturn.Add((T)(object)Convert.ToUInt32((long)obj));
                            else if (obj is uint)
                                objectsToReturn.Add((T)(object)Convert.ToUInt32((uint)obj));
                            else if (obj is int)
                                objectsToReturn.Add((T)(object)Convert.ToUInt32((int)obj));
                            else if (obj is float)
                                objectsToReturn.Add((T)(object)Convert.ToUInt32((float)obj));
                            else if (obj is double)
                                objectsToReturn.Add((T)(object)Convert.ToUInt32((double)obj));
                            else if (obj is byte)
                                objectsToReturn.Add((T)(object)Convert.ToUInt32((byte)obj));
                            else if (obj is short)
                                objectsToReturn.Add((T)(object)Convert.ToUInt32((short)obj));
                        }
                    }
                    else if (type.Equals(typeof(int)))
                    {
                        foreach (object obj in objects)
                        {
                            if (obj is long)
                                objectsToReturn.Add((T)(object)Convert.ToInt32((long)obj));
                            else if (obj is uint)
                                objectsToReturn.Add((T)(object)Convert.ToInt32((uint)obj));
                            else if (obj is int)
                                objectsToReturn.Add((T)(object)Convert.ToInt32((int)obj));
                            else if (obj is float)
                                objectsToReturn.Add((T)(object)Convert.ToInt32((float)obj));
                            else if (obj is double)
                                objectsToReturn.Add((T)(object)Convert.ToInt32((double)obj));
                            else if (obj is byte)
                                objectsToReturn.Add((T)(object)Convert.ToInt32((byte)obj));
                            else if (obj is short)
                                objectsToReturn.Add((T)(object)Convert.ToInt32((short)obj));
                        }
                    }
                    else if (type.Equals(typeof(float)))
                    {
                        foreach (object obj in objects)
                        {
                            if (obj is long)
                                objectsToReturn.Add((T)(object)Convert.ToSingle((long)obj));
                            else if (obj is uint)
                                objectsToReturn.Add((T)(object)Convert.ToSingle((uint)obj));
                            else if (obj is int)
                                objectsToReturn.Add((T)(object)Convert.ToSingle((int)obj));
                            else if (obj is float)
                                objectsToReturn.Add((T)(object)Convert.ToSingle((float)obj));
                            else if (obj is double)
                                objectsToReturn.Add((T)(object)Convert.ToSingle((double)obj));
                            else if (obj is byte)
                                objectsToReturn.Add((T)(object)Convert.ToSingle((byte)obj));
                            else if (obj is short)
                                objectsToReturn.Add((T)(object)Convert.ToSingle((short)obj));
                        }
                    }
                    else if (type.Equals(typeof(double)))
                    {
                        foreach (object obj in objects)
                        {
                            if (obj is long)
                                objectsToReturn.Add((T)(object)Convert.ToDouble((long)obj));
                            else if (obj is uint)
                                objectsToReturn.Add((T)(object)Convert.ToDouble((uint)obj));
                            else if (obj is int)
                                objectsToReturn.Add((T)(object)Convert.ToDouble((int)obj));
                            else if (obj is float)
                                objectsToReturn.Add((T)(object)Convert.ToDouble((float)obj));
                            else if (obj is double)
                                objectsToReturn.Add((T)(object)Convert.ToDouble((double)obj));
                            else if (obj is byte)
                                objectsToReturn.Add((T)(object)Convert.ToDouble((byte)obj));
                            else if (obj is short)
                                objectsToReturn.Add((T)(object)Convert.ToDouble((short)obj));
                        }
                    }
                    else if (type.Equals(typeof(long)))
                    {
                        foreach (object obj in objects)
                        {
                            if (obj is long)
                                objectsToReturn.Add((T)(object)Convert.ToInt64((long)obj));
                            else if (obj is uint)
                                objectsToReturn.Add((T)(object)Convert.ToInt64((uint)obj));
                            else if (obj is int)
                                objectsToReturn.Add((T)(object)Convert.ToInt64((int)obj));
                            else if (obj is float)
                                objectsToReturn.Add((T)(object)Convert.ToInt64((float)obj));
                            else if (obj is double)
                                objectsToReturn.Add((T)(object)Convert.ToInt64((double)obj));
                            else if (obj is byte)
                                objectsToReturn.Add((T)(object)Convert.ToInt64((byte)obj));
                            else if (obj is short)
                                objectsToReturn.Add((T)(object)Convert.ToInt64((short)obj));
                        }
                    }
                    else if (type.Equals(typeof(short)))
                    {
                        foreach (object obj in objects)
                        {
                            if (obj is long)
                                objectsToReturn.Add((T)(object)Convert.ToInt16((long)obj));
                            else if (obj is uint)
                                objectsToReturn.Add((T)(object)Convert.ToInt16((uint)obj));
                            else if (obj is int)
                                objectsToReturn.Add((T)(object)Convert.ToInt16((int)obj));
                            else if (obj is float)
                                objectsToReturn.Add((T)(object)Convert.ToInt16((float)obj));
                            else if (obj is double)
                                objectsToReturn.Add((T)(object)Convert.ToInt16((double)obj));
                            else if (obj is byte)
                                objectsToReturn.Add((T)(object)Convert.ToInt16((byte)obj));
                            else if (obj is short)
                                objectsToReturn.Add((T)(object)Convert.ToInt16((short)obj));
                        }
                    }
                    else if (type.Equals(typeof(byte)))
                    {
                        foreach (object obj in objects)
                        {
                            if (obj is long)
                                objectsToReturn.Add((T)(object)Convert.ToByte((long)obj));
                            else if (obj is uint)
                                objectsToReturn.Add((T)(object)Convert.ToByte((uint)obj));
                            else if (obj is int)
                                objectsToReturn.Add((T)(object)Convert.ToByte((int)obj));
                            else if (obj is float)
                                objectsToReturn.Add((T)(object)Convert.ToByte((float)obj));
                            else if (obj is double)
                                objectsToReturn.Add((T)(object)Convert.ToByte((double)obj));
                            else if (obj is byte)
                                objectsToReturn.Add((T)(object)Convert.ToByte((byte)obj));
                            else if (obj is short)
                                objectsToReturn.Add((T)(object)Convert.ToByte((short)obj));
                        }
                    }
                    else if (type.Equals(typeof(String)))
                    {
                        foreach (object obj in objects)
                        {
                            if (obj is string)
                                objectsToReturn.Add((T)obj);
                        }
                    }
                    else if (type.BaseType.Equals(typeof(Enum)))
                    {
                        foreach (object obj in objects)
                        {
                            string str = obj as string;
                            if (str != null)
                                objectsToReturn.Add((T)Enum.Parse(type, str));
                        }
                    }
                    else if (type.Equals(typeof(DateTime)))
                    {
                        foreach (object obj in objects)
                        {
                            string str = obj as string;
                            if (str != null)
                            {
                                string[] yearMonthDay = str.Split('-');
                                objectsToReturn.Add((T)(object)new DateTime(int.Parse(yearMonthDay[0], CommonSettings.Culture), int.Parse(yearMonthDay[1], CommonSettings.Culture), int.Parse(yearMonthDay[2], CommonSettings.Culture)));
                            }
                        }
                    }
                    else if (type.GetInterface(typeof(IStringRepresentative).FullName) != null)
                    {
                        ConstructorInfo constructorInfo = null;
                        foreach (object obj in objects)
                        {
                            string str = obj as string;
                            if (str != null)
                            {
                                if (constructorInfo == null)
                                    constructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { }, null);
                                IStringRepresentative createdObject = (IStringRepresentative)(T)constructorInfo.Invoke(new object[] { });
                                createdObject.LoadString(str);
                                objectsToReturn.Add((T)createdObject);
                            }
                        }
                    }
                    else if (type.Equals(typeof(bool)))
                    {
                        foreach (object obj in objects)
                        {
                            if (obj is bool)
                                objectsToReturn.Add((T)(object)Convert.ToBoolean((bool)obj));
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
            return objectsToReturn;
        }
        #endregion Method: ConvertObjects<T>(List<object> objects, Type type)
        
        #region Method: ConvertToObject<T>(uint id, Type type)
        /// <summary>
        /// Convert the ID from the given type to an object of the given type.
        /// </summary>
        /// <typeparam name="T">The type the object should get, can be abstract.</typeparam>
        /// <param name="id">The ID from which the object should be converted.</param>
        /// <param name="type">The type of the ID that should be converted.</param>
        /// <returns>The object.</returns>
        internal T ConvertToObject<T>(uint id, Type type)
        {
            // Convert abstract types
            if (type.IsAbstract)
            {
                // Check the deepest table in which the id is present
                type = GetDeepestType(id, type);
                if (type == null)
                    return default(T);
            }

            // Check whether the type has been created before and is now in the memory
            if (this.memory.ContainsKey(type))
            {
                if (this.memory[type].ContainsKey(id))
                    return (T)this.memory[type][id];
            }
            
            // Create an object of that type with the ID
            ConstructorInfo constructorInfo;
            if (!this.constructors.TryGetValue(type, out constructorInfo))
            {
                constructorInfo = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(uint) }, null);
                this.constructors.Add(type, constructorInfo);
            }
            T createdObject = (T)constructorInfo.Invoke(new object[] { id });
            
            // Add that object to the memory
            if (!this.memory.ContainsKey(type))
                this.memory.Add(type, new Dictionary<uint, object>());
            this.memory[type].Add(id, createdObject);
            
            return createdObject;
        }
        #endregion Method: ConvertToObject<T>(uint id, Type type)

        #region Method: GetExactType(string typeName)
        /// <summary>
        /// Get the exact type for the given type name.
        /// </summary>
        /// <param name="typeName">The type name.</param>
        /// <returns>The type.</returns>
        public Type GetExactType(string typeName)
        {
            // Try to get the type from the memory
            Type nameType;
            if (this.nameTypes.TryGetValue(typeName, out nameType))
                return nameType;

            // Try to get a known type
            foreach (Assembly assembly in this.assemblies)
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.Name.Equals(typeName))
                    {
                        // Store and return the type
                        this.nameTypes.Add(typeName, type);
                        return type;
                    }
                }
            }

            // Try to convert the type in a normal way
            try
            {
                return Type.GetType(typeName);
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion Method: GetExactType(string typeName)

        #region Method: ConvertValue(object value)
        /// <summary>
        /// Convert a value to a value that can be written to the database.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>A value that can be written to the database.</returns>
        private static object ConvertValue(object value)
        {
            // IdHolders should be stored as IDs
            IdHolder idHolder = value as IdHolder;
            if (idHolder != null)
                return idHolder.ID;

            // Convert a possible boolean value to 1 or 0
            if (value is bool)
                return (bool)value ? 1 : 0;

            // Replace apostrophs by double ones, as SQLite won't be able to parse them otherwise
            string str = value as string;
            if (str != null)
                return str.Replace("'", "''");

            // String representatives are saved in a string
            IStringRepresentative stringRepresentative = value as IStringRepresentative;
            if (stringRepresentative != null)
                return stringRepresentative.CreateString();

            // Only save the dates
            if (value is DateTime)
            {
                DateTime dateTime = (DateTime)value;
                return "" + dateTime.Year + "-" + dateTime.Month + "-" + dateTime.Day;
            }

            return value;
        }
        #endregion Method: ConvertValue(object value)
        
        #region Method: GetDeepestType(uint id, Type type)
        /// <summary>
        /// Return the deepest non-abstract ID holder type for the given ID of the given type.
        /// </summary>
        /// <param name="id">The ID to get the deepest type of.</param>
        /// <param name="type">The known type of the ID holder.</param>
        /// <returns>The deepest type.</returns>
        private Type GetDeepestType(uint id, Type type)
        {
            // Try to get the deepest type from the memory
            Type deepestType = null;
            Dictionary<Type, Type> deepestTypeDictionary;
            if (this.deepestTypes.TryGetValue(id, out deepestTypeDictionary))
            {
                if (deepestTypeDictionary.TryGetValue(type, out deepestType))
                    return deepestType;
            }

            // Get the node type
            if (type == typeof(Node))
            {
                string nodeType = Select<string>(id, GenericTables.Node, Columns.Type);
                if (nodeType != null)
                    deepestType = GetExactType(nodeType);
            }
            // Otherwise, get all valid types
            else
            {
                List<Type> validTypes = new List<Type>();
                foreach (Type derivedType in GetNonAbstractTypes(type))
                {
                    try
                    {
                        string table = GetTable(derivedType.Name);
                        if (table != null && Count(id, table, false) > 0)
                            validTypes.Add(derivedType);
                    }
                    catch (Exception)
                    {
                    }
                }

                // Get the deepest type
                if (validTypes.Count == 0)
                    return null;
                else
                {
                    deepestType = validTypes[0];
                    for (int i = 1; i < validTypes.Count; i++)
                    {
                        if (validTypes[i].IsSubclassOf(type))
                            deepestType = validTypes[i];
                    }
                }
            }

            // Store it in the dictionary for future lookup
            if (deepestTypeDictionary == null && deepestType != null)
            {
                deepestTypeDictionary = new Dictionary<Type,Type>();
                deepestTypeDictionary.Add(type, deepestType);
                this.deepestTypes.Add(id, deepestTypeDictionary);
            }
            else
                this.deepestTypes[id].Add(type, deepestType);
            
            return deepestType;
        }
        #endregion Method: GetDeepestType(uint id, Type type)

        #region Method: GetNonAbstractTypes(Type type)
        /// <summary>
        /// Get the non-abstract types of the given type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The same type when it is non-abstract, or the non-abstract derived type when it is.</returns>
        public List<Type> GetNonAbstractTypes(Type type)
        {
            List<Type> types = null;

            // Try to get the types from the dictionary
            if (!this.nonAbstractTypes.TryGetValue(type, out types))
            {
                // Otherwise, find and store them
                types = new List<Type>();
                if (type.IsAbstract)
                    types.AddRange(BasicFunctionality.GetDerivedTypes(type, this.assemblies.AsReadOnly()));
                else
                    types.Add(type);
                this.nonAbstractTypes.Add(type, types);
            }
            
            return types;
        }
        #endregion Method: GetNonAbstractTypes(Type type)
        
        #endregion Method Group: Conversions

        #region Method Group: Update/Insert/Remove

        #region Method: Update(uint id, string table, string column, object value)
        /// <summary>
        /// Update an item from a table.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <param name="table">The table to update an item in.</param>
        /// <param name="column">The column to update.</param>
        /// <param name="value">The value to put in the column.</param>
        protected internal void Update(uint id, string table, string column, object value)
        {
            string query = string.Empty;
            string undoQuery = string.Empty;

            // Create the query
            if (value == null)
                query = "UPDATE " + table + " SET " + column + " = NULL WHERE " + Columns.ID + " = '" + id + "';";
            else
            {
                value = ConvertValue(value);
                query = "UPDATE " + table + " SET " + column + " = '" + value + "' WHERE " + Columns.ID + " = '" + id + "';";
            }

            // Create the undo query
            object val = Select(id, table, column);
            if (val == null)
                undoQuery = "UPDATE " + table + " SET " + column + " = NULL WHERE " + Columns.ID + " = '" + id + "';";
            else
                undoQuery = "UPDATE " + table + " SET " + column + " = '" + val + "' WHERE " + Columns.ID + " = '" + id + "';";

            // Perform the query
            QueryChange(GetDatabaseType(table), query, undoQuery);
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
        protected internal void Update(uint id, string table, string column, object oldValue, object newValue)
        {
            if (oldValue != newValue)
            {
                string query = string.Empty;
                string undoQuery = string.Empty;

                oldValue = ConvertValue(oldValue);
                newValue = ConvertValue(newValue);

                if (newValue == null)
                {
                    query = "UPDATE " + table + " SET " + column + " = NULL WHERE " + Columns.ID + " = '" + id + "' AND " + column + " = '" + oldValue + "';";
                    undoQuery = "UPDATE " + table + " SET " + column + " = '" + oldValue + "' WHERE " + Columns.ID + " = '" + id + "' AND " + column + " = NULL;";
                }
                else
                {
                    if (oldValue == null)
                    {
                        query = "UPDATE " + table + " SET " + column + " = '" + newValue + "' WHERE " + Columns.ID + " = '" + id + "' AND " + column + " = NULL;";
                        undoQuery = "UPDATE " + table + " SET " + column + " = NULL WHERE " + Columns.ID + " = '" + id + "' AND " + column + " = '" + newValue + "';";
                    }
                    else
                    {
                        query = "UPDATE " + table + " SET " + column + " = '" + newValue + "' WHERE " + Columns.ID + " = '" + id + "' AND " + column + " = '" + oldValue + "';";
                        undoQuery = "UPDATE " + table + " SET " + column + " = '" + oldValue + "' WHERE " + Columns.ID + " = '" + id + "' AND " + column + " = '" + newValue + "';";
                    }
                }

                QueryChange(GetDatabaseType(table), query, undoQuery);
            }
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
        protected internal void Update(uint id, string table, string column, object value, string conditionColumn, object conditionValue)
        {
            string query = string.Empty;
            string undoQuery = string.Empty;

            // Create the query
            if (value == null)
            {
                if (conditionValue == null)
                    query = "UPDATE " + table + " SET " + column + " = NULL WHERE " + Columns.ID + " = '" + id + "' AND " + conditionColumn + " = NULL;";
                else
                {
                    conditionValue = ConvertValue(conditionValue);
                    query = "UPDATE " + table + " SET " + column + " = NULL WHERE " + Columns.ID + " = '" + id + "' AND " + conditionColumn + " = '" + conditionValue + "';";
                }
            }
            else
            {
                value = ConvertValue(value);
                if (conditionValue == null)
                    query = "UPDATE " + table + " SET " + column + " = '" + value + "' WHERE " + Columns.ID + " = '" + id + "' AND " + conditionColumn + " = NULL;";
                else
                {
                    conditionValue = ConvertValue(conditionValue);
                    query = "UPDATE " + table + " SET " + column + " = '" + value + "' WHERE " + Columns.ID + " = '" + id + "' AND " + conditionColumn + " = '" + conditionValue + "';";
                }
            }

            // Create the undo query
            object val = Select(id, table, column);
            if (val == null)
            {
                if (conditionValue == null)
                    undoQuery = "UPDATE " + table + " SET " + column + " = NULL WHERE " + Columns.ID + " = '" + id + "' AND " + conditionColumn + " = NULL;";
                else
                {
                    conditionValue = ConvertValue(conditionValue);
                    undoQuery = "UPDATE " + table + " SET " + column + " = NULL WHERE " + Columns.ID + " = '" + id + "' AND " + conditionColumn + " = '" + conditionValue + "';";
                }
            }
            else
            {
                if (conditionValue == null)
                    undoQuery = "UPDATE " + table + " SET " + column + " = '" + val + "' WHERE " + Columns.ID + " = '" + id + "' AND " + conditionColumn + " = NULL;";
                else
                {
                    conditionValue = ConvertValue(conditionValue);
                    undoQuery = "UPDATE " + table + " SET " + column + " = '" + val + "' WHERE " + Columns.ID + " = '" + id + "' AND " + conditionColumn + " = '" + conditionValue + "';";
                }
            }

            // Perform the query
            QueryChange(GetDatabaseType(table), query, undoQuery);
        }
        #endregion Method: Update(uint id, string table, string column, object value, string conditionColumn, object conditionValue)

        #region Method: Update(string table, string column, object oldValue, object newValue)
        /// <summary>
        /// Update the old value of an item from a table with a new one.
        /// </summary>
        /// <param name="table">The table to update an item in.</param>
        /// <param name="column">The column to update.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value to replace the old one.</param>
        private void Update(string table, string column, object oldValue, object newValue)
        {
            if (oldValue != newValue)
            {
                string query = string.Empty;
                string undoQuery = string.Empty;

                oldValue = ConvertValue(oldValue);
                newValue = ConvertValue(newValue);

                if (newValue == null)
                {
                    query = "UPDATE " + table + " SET " + column + " = NULL WHERE " + column + " = '" + oldValue + "';";
                    undoQuery = "UPDATE " + table + " SET " + column + " = '" + oldValue + "' WHERE " + column + " = NULL;";
                }
                else
                {
                    if (oldValue == null)
                    {
                        query = "UPDATE " + table + " SET " + column + " = '" + newValue + "' WHERE " + column + " = NULL;";
                        undoQuery = "UPDATE " + table + " SET " + column + " = NULL WHERE " + column + " = '" + newValue + "';";
                    }
                    else
                    {
                        query = "UPDATE " + table + " SET " + column + " = '" + newValue + "' WHERE " + column + " = '" + oldValue + "';";
                        undoQuery = "UPDATE " + table + " SET " + column + " = '" + oldValue + "' WHERE " + column + " = '" + newValue + "';";
                    }
                }

                QueryChange(GetDatabaseType(table), query, undoQuery);
            }
        }
        #endregion Method: Update(string table, string column, object oldValue, object newValue)

        #region Method: Update(string table, string column, object oldValue, object newValue)
        /// <summary>
        /// Update the old value of an item from a table with a new one.
        /// </summary>
        /// <param name="table">The table to update an item in.</param>
        /// <param name="column">The column to update.</param>
        /// <param name="value">The new value.</param>
        /// <param name="conditionColumn">The column to define a condition for.</param>
        /// <param name="conditionValue">The value that should be in the condition column.</param>
        protected internal void Update(string table, string column, object value, string conditionColumn, object conditionValue)
        {
            string query = string.Empty;
            string undoQuery = string.Empty;

            value = ConvertValue(value);
            conditionValue = ConvertValue(conditionValue);

            // Create the query
            if (value == null)
                query = "UPDATE " + table + " SET " + column + " = NULL WHERE " + conditionColumn + " = '" + conditionValue + "';";
            else
            {
                if (conditionValue == null)
                    query = "UPDATE " + table + " SET " + column + " = '" + value + "' WHERE " + conditionColumn + " = NULL;";
                else
                    query = "UPDATE " + table + " SET " + column + " = '" + value + "' WHERE " + conditionColumn + " = '" + conditionValue + "';";
            }

            // Create the undo query
            object val = Select(table, column, conditionColumn, conditionValue);
            if (val == null)
                undoQuery = "UPDATE " + table + " SET " + column + " = NULL WHERE " + conditionColumn + " = '" + conditionValue + "';";
            else
                undoQuery = "UPDATE " + table + " SET " + column + " = '" + val + "' WHERE " + conditionColumn + " = '" + conditionValue + "';";

            QueryChange(GetDatabaseType(table), query, undoQuery);
        }
        #endregion Method: Update(string table, string column, object oldValue, object newValue)

        #region Method: Insert(uint id, string table)
        /// <summary>
        /// Insert a new ID in the table.
        /// </summary>
        /// <param name="id">The ID to insert.</param>
        /// <param name="table">The table to insert the ID.</param>
        protected internal void Insert(uint id, string table)
        {
            string query = "INSERT INTO " + table + " (" + Columns.ID + ") VALUES('" + id + "');";
            string undoQuery = "DELETE FROM " + table + " WHERE " + Columns.ID + " = '" + id + "';";

            QueryInsertRemoveChange(GetDatabaseType(table), query, undoQuery);
        }
        #endregion Method: Insert(uint id, string table)

        #region Method: Insert(uint id, string table, string column, object value)
        /// <summary>
        /// Insert a new item to the table with the given ID and put the given value in the given column.
        /// </summary>
        /// <param name="id">The ID of the new item to insert.</param>
        /// <param name="table">The table to insert in.</param>
        /// <param name="column">The column to assign a value to.</param>
        /// <param name="value">The value to assign to a column.</param>
        protected internal void Insert(uint id, string table, string column, object value)
        {
            value = ConvertValue(value);

            string query = "INSERT INTO " + table + " (" + Columns.ID + "," + column + ") VALUES('" + id + "','" + value + "');";
            string undoQuery = "DELETE FROM " + table + " WHERE " + Columns.ID + " = '" + id + "';";

            QueryInsertRemoveChange(GetDatabaseType(table), query, undoQuery);
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
        protected internal void Insert(uint id, string table, string[] columns, object[] values)
        {
            if (columns != null && values != null && columns.Length == values.Length && columns.Length > 0)
            {
                List<object> convertedValues = new List<object>();
                foreach (object obj in values)
                    convertedValues.Add(ConvertValue(obj));

                string query = "INSERT INTO " + table + " (" + Columns.ID;
                for (int i = 0; i < columns.Length; i++)
                {
                    query += ",";
                    query += columns[i];
                }
                query += ") VALUES('" + id + "'";
                for (int i = 0; i < convertedValues.Count; i++)
                {
                    query += ",'";
                    query += convertedValues[i];
                    query += "'";
                }
                query += ");";

                string undoQuery = "DELETE FROM " + table + " WHERE " + Columns.ID + " = '" + id + "';";

                QueryInsertRemoveChange(GetDatabaseType(table), query, undoQuery);
            }
        }
        #endregion Method: Insert(uint id, string table, string[] columns, object[] values)

        #region Method: Insert(string table, string column, object value)
        /// <summary>
        /// Insert a new item to the table and put the given value in the given column.
        /// </summary>
        /// <param name="table">The table to insert in.</param>
        /// <param name="column">The column to assign a value to.</param>
        /// <param name="value">The value to assign to a column.</param>
        protected internal void Insert(string table, string column, object value)
        {
            value = ConvertValue(value);

            string query = "INSERT INTO " + table + " (" + column + ") VALUES('" + value + "');";
            string undoQuery = "DELETE FROM " + table + " WHERE " + column + " = '" + value + "';";

            QueryInsertRemoveChange(GetDatabaseType(table), query, undoQuery);
        }
        #endregion Method: Insert(string table, string column, object value)

        #region Method: Insert(string table, string[] columns, object[] values)
        /// <summary>
        /// Insert a new item to the table and the given values in the given columns.
        /// </summary>
        /// <param name="table">The table to insert in.</param>
        /// <param name="columns">The columns to assign a value to.</param>
        /// <param name="values">The values to assign to the columns.</param>
        protected internal void Insert(string table, string[] columns, object[] values)
        {
            if (columns != null && values != null && columns.Length == values.Length && columns.Length > 0)
            {
                List<object> convertedValues = new List<object>();
                foreach (object obj in values)
                    convertedValues.Add(ConvertValue(obj));

                string query = "INSERT INTO " + table + " (";
                for (int i = 0; i < columns.Length; i++)
                {
                    if (i != 0)
                        query += ",";
                    query += columns[i];
                }
                query += ") VALUES('";
                for (int i = 0; i < convertedValues.Count; i++)
                {
                    if (i != 0)
                        query += ",'";
                    query += convertedValues[i];
                    query += "'";
                }
                query += ");";

                string undoQuery = "DELETE FROM " + table + " WHERE " + columns[0] + " = '" + values[0] + "';";

                QueryInsertRemoveChange(GetDatabaseType(table), query, undoQuery);
            }
        }
        #endregion Method: Insert(string table, string[] columns, object[] values)

        #region Method: RemoveAll(string table)
        /// <summary>
        ///  Remove all values from the given table.
        /// </summary>
        /// <param name="table">The table to remove the values from.</param>
        protected internal void RemoveAll<T>(string table)
        {
            List<T> all = SelectAll<T>(false, table, Columns.ID);
            foreach (T t in all)
            {
                IdHolder i = t as IdHolder;
                Remove(i);
            }
        }
        #endregion Method: RemoveAll(string table)

        #region Method: Remove(uint id, string table)
        /// <summary>
        ///  Remove the given ID from the given table.
        /// </summary>
        /// <param name="id">The ID to remove.</param>
        /// <param name="table">The table to remove the ID from.</param>
        protected internal void Remove(uint id, string table)
        {
            string query = "DELETE FROM " + table + " WHERE " + Columns.ID + " = '" + id + "';";
            string undoQuery = CreateUndoRemoveQuery(table, Columns.ID, id);

            QueryInsertRemoveChange(GetDatabaseType(table), query, undoQuery);
        }
        #endregion Method: Remove(uint id, string table)

        #region Method: Remove(uint id, string table, string column, object value)
        /// <summary>
        /// Remove the entry of the ID in the given table where the given value is in the given column.
        /// </summary>
        /// <param name="id">The ID to remove.</param>
        /// <param name="table">The table to remove the ID from.</param>
        /// <param name="column">The column that should have a particular value.</param>
        /// <param name="value">The value that should be in the column.</param>
        protected internal void Remove(uint id, string table, string column, object value)
        {
            Remove(id, table, column, value, false);
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
        protected internal void Remove(uint id, string table, string column, object value, bool keepReference)
        {
            object originalValue = value;
            IdHolder idHolder = originalValue as IdHolder;
            if (idHolder != null)
                value = idHolder.ID;

            // Remove the value from the table
            string query = "DELETE FROM " + table + " WHERE " + Columns.ID + " = '" + id + "' AND " + column + " = '" + value + "';";
            string undoQuery = CreateUndoRemoveQuery(table, column, value);
            QueryInsertRemoveChange(GetDatabaseType(table), query, undoQuery);

            // If the value is an ID holder, but no node, there's just one reference, which should be removed as well
            if (!keepReference && idHolder != null && !(originalValue is Node))
                idHolder.Remove();
        }
        #endregion Method: Remove(uint id, string table, string column, object value, bool keepReference)

        #region Method: Remove(string table, string column, object value)
        /// <summary>
        /// Remove the entry from the table where the given value is in the given column.
        /// </summary>
        /// <param name="table">The table to remove the entry from.</param>
        /// <param name="column">The column in which the value should be.</param>
        /// <param name="value">The value for which the entry should be removed.</param>
        protected internal void Remove(string table, string column, object value)
        {
            value = ConvertValue(value);

            string query = "DELETE FROM " + table + " WHERE " + column + " = '" + value + "';";
            string undoQuery = CreateUndoRemoveQuery(table, column, value);

            QueryInsertRemoveChange(GetDatabaseType(table), query, undoQuery);
        }
        #endregion Method: Remove(string table, string column, object value)

        #region Method: CreateUndoRemoveQuery(string table, string column, object value)
        /// <summary>
        /// Create the undo query for a removal.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="column">The column in which the value should be.</param>
        /// <param name="value">The value that should be in the column.</param>
        /// <returns>The undo query.</returns>
        private string CreateUndoRemoveQuery(string table, string column, object value)
        {
            // Get the columns for the given table
            DatabaseType databaseType = GetDatabaseType(table);
            List<string> tableColumns = this.IDatabase.GetColumns(databaseType, table);

            // Create the undo query by selecting the values corresponding to the columns
            if (tableColumns.Count > 0)
            {
                string columns = string.Empty;
                string values = string.Empty;

                for (int i = 0; i < tableColumns.Count; i++)
                {
                    // Get the column
                    string tableColumn = tableColumns[i];

                    // Get the value
                    object val = null;
                    if (value == null)
                        val = this.IDatabase.QuerySelect(databaseType, "SELECT " + tableColumn + " FROM " + table + " WHERE " + column + " = NULL;");
                    else
                    {
                        value = ConvertValue(value);
                        val = this.IDatabase.QuerySelect(databaseType, "SELECT " + tableColumn + " FROM " + table + " WHERE " + column + " = '" + value + "';");
                    }

                    // If there is no ID, there is no undo query
                    if (Columns.ID.Equals(tableColumn) && val == null)
                        return string.Empty;

                    // Add the column
                    if (i != 0)
                        columns += ",";
                    columns += tableColumn;

                    // Add the value
                    if (i != 0)
                        values += ",";
                    if (val == null)
                        values += "NULL";
                    else
                        values += "'" + ConvertValue(val) + "'";
                }

                return "INSERT INTO " + table + " (" + columns + ") VALUES(" + values + ");";
            }
            return string.Empty;
        }
        #endregion Method: CreateUndoRemoveQuery(string table, string column, object value)

        #endregion Method Group: Update/Insert/Remove

        #region Method Group: Count

        #region Method: Count(uint id, string table, bool distinct)
        /// <summary>
        /// Count the number of instances of the ID in the given table.
        /// </summary>
        /// <param name="id">The ID to count.</param>
        /// <param name="table">The table in which to count.</param>
        /// <param name="distinct">Indicates whether the count should be distinct.</param>
        /// <returns>The number of instances of the ID in the given table.</returns>
        protected internal int Count(uint id, string table, bool distinct)
        {
            string dist = "";
            if (distinct)
                dist = "DISTINCT ";
            return Select<int>(table, "COUNT(" + dist + Columns.ID + ")", Columns.ID + " = '" + id + "'");
        }
        #endregion Method: Count(uint id, string table, bool distinct)

        #region Method: Count(uint id, string table, string column, bool distinct)
        /// <summary>
        /// Count the number of instances of the ID in the given table and column.
        /// </summary>
        /// <param name="id">The ID to count.</param>
        /// <param name="table">The table in which to count.</param>
        /// <param name="column">The column to look for the ID.</param>
        /// <param name="distinct">Indicates whether the count should be distinct.</param>
        /// <returns>The number of instances of the ID in the given table and column.</returns>
        protected internal int Count(uint id, string table, string column, bool distinct)
        {
            string dist = "";
            if (distinct)
                dist = "DISTINCT ";
            return Select<int>(table, "COUNT(" + dist + Columns.ID + ")", column + " = '" + id + "';");
        }
        #endregion Method: Count(uint id, string table, string column, bool distinct)

        #region Method: Count(string table, string column, bool distinct)
        /// <summary>
        /// Count the number of values in the given column, at the given table.
        /// </summary>
        /// <param name="table">The table in which to count.</param>
        /// <param name="column">The column to count</param>
        /// <param name="distinct">Indicates whether the count should be distinct.</param>
        /// <returns>The number of instances in the given table and column.</returns>
        protected internal int Count(string table, string column, bool distinct)
        {
            string dist = "";
            if (distinct)
                dist = "DISTINCT ";
            return Select<int>(table, "COUNT(" + dist + column + ")");
        }
        #endregion Method: Count(string table, string column, bool distinct)

        #endregion Method Group: Count

        #region Method Group: Add/Remove
        
        #region Method: Add(IdHolder idHolder)
        /// <summary>
        /// Add a new ID holder to the database.
        /// </summary>
        /// <param name="idHolder">The ID holder to add.</param>
        protected internal void Add(IdHolder idHolder)
        {
            Add(idHolder, null);
        }
        
        /// <summary>
        /// Add a new ID holder to the database.
        /// </summary>
        /// <param name="idHolder">The ID holder to add.</param>
        /// <param name="name">The name to assign to the ID holder.</param>
        protected internal void Add(IdHolder idHolder, string name)
        {
            uint id = 0;

            if (idHolder != null && idHolder.ID == 0)
            {
                // Get the type of the ID holder
                Type type = idHolder.GetType();
                string typeName = type.Name;

                // Get the correct table name
                string table = null;
                Node node = idHolder as Node;
                if (node != null)
                    table = GenericTables.Node;
                else if (idHolder is NodeValued)
                    table = ValueTables.NodeValued;
                else if (idHolder is Condition)
                    table = ValueTables.Condition;
                else if (idHolder is Effect)
                    table = ValueTables.Effect;
                else if (idHolder is Value)
                    table = ValueTables.Value;
                else if (idHolder is NumericalValueRange)
                    table = ValueTables.NumericalValueRange;
                else if (idHolder is Reference)
                    table = ValueTables.Reference;
                else if (idHolder is Variable)
                    table = ValueTables.Variable;
                else
                    table = GetTable(typeName);

                if (table != null)
                {
                    // Get the maximum ID of the table, so the maximum + 1 can become the ID
                    object selection = Select(table, "MAX(" + Columns.ID + ")");
                    if (selection is long)
                        id = Convert.ToUInt32((long)selection + 1);
                    else
                        id = 1;

                    // Set the ID
                    idHolder.ID = id;

                    StartChange();

                    // Add the ID holder to its own table, and all its base tables, except ID holder itself
                    Type baseType = type;
                    QueryAllBegin();
                    while (baseType != typeof(IdHolder))
                    {
                        Insert(id, GetTable(baseType.Name));
                        baseType = baseType.BaseType;
                    }
                    QueryAllCommit();

                    // Add nodes to their tables
                    if (node != null)
                    {
                        // Update the type of the node
                        Update(id, GenericTables.Node, Columns.Type, typeName);

                        // Add the (default new) name of the node to the name table
                        if (name == null)
                            Insert(id, LocalizationTables.NodeName, new string[] { Columns.Name, Columns.IsDefaultName }, new object[] { GetNewNodeName(typeName), true });
                        else
                            Insert(id, LocalizationTables.NodeName, new string[] { Columns.Name, Columns.IsDefaultName }, new object[] { name, true });

                        node.names = null;
                        node.defaultName = null;

                        // Add the node to the description table
                        Insert(id, LocalizationTables.NodeDescription);

                        // Throw an event
                        if (NodeAdded != null)
                            NodeAdded(node);
                    }

                    // Remember during which undo/redo moment this node was added
                    if (node != null)
                        this.additionMoments.Add(new Tuple<int, uint>(this.undoRedoCounter, id));

                    StopChange();
                }

                // Add the ID holder to the memory
                if (!this.memory.ContainsKey(type))
                    this.memory.Add(type, new Dictionary<uint, object>());
                this.memory[type].Add(id, idHolder);
            }
        }
        #endregion Method: Add(IdHolder idHolder)

        #region Method: Remove(IdHolder idHolder)
        /// <summary>
        /// Remove the given ID holder.
        /// </summary>
        /// <param name="idHolder">The ID holder to remove.</param>
        protected internal void Remove(IdHolder idHolder)
        {
            if (idHolder != null)
            {
                Node node = idHolder as Node;

                // First throw an event to let others know that this node is going to be removed
                if (node != null && NodeRemoved != null)
                    NodeRemoved(node);

                StartChange();

                // Remove the ID holder from their specific tables, all the way up until ID holder
                Type type = idHolder.GetType();
                Type baseType = type;
                QueryAllBegin();
                while (baseType != typeof(IdHolder))
                {
                    Remove(idHolder.ID, GetTable(baseType.Name));
                    baseType = baseType.BaseType;
                }
                QueryAllCommit();

                // Remove relations
                if (node != null)
                {
                    // Check whether the node is present in another table; if so, the entry should be removed or set to NULL
                    for (int i = 0; i < this.tableColumnType.Count; i++)
                    {
                        string key = this.tableColumnType[i].Item1;
                        Tuple<Type, Dictionary<string, Tuple<Type, EntryType>>> tuple = this.tableColumnType[i].Item2;
                        Dictionary<string, Tuple<Type, EntryType>>.Enumerator enumerator2 = tuple.Item2.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            Tuple<Type, EntryType> tuple2 = enumerator2.Current.Value;
                            if (type.Equals(tuple2.Item1) || type.IsSubclassOf(tuple2.Item1))
                            {
                                switch (tuple2.Item2)
                                {
                                    case EntryType.Unique:
                                        // Remove the entry in the found table where the ID is in the found column
                                        uint id = ConvertObject<uint>(Select(key, Columns.ID, enumerator2.Current.Key + "='" + node.ID + "'"), typeof(uint));
                                        Type deepestType = GetDeepestType(id, tuple.Item1);
                                        if (deepestType != null)
                                        {
                                            IdHolder dependentIdHolder = ConvertToObject<IdHolder>(id, deepestType);
                                            if (dependentIdHolder != null)
                                                dependentIdHolder.Remove();
                                        }
                                        break;
                                    case EntryType.Nullable:
                                        // Set the entry in the found table where the ID is in the found column to NULL
                                        Update(key, enumerator2.Current.Key, node.ID, null);
                                        break;
                                    case EntryType.Intermediate:
                                        Remove(key, enumerator2.Current.Key, node.ID);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }

                // Remove the ID holder from the memory
                if (this.memory.ContainsKey(type) && this.memory[type].ContainsKey(idHolder.ID))
                    this.memory[type].Remove(idHolder.ID);

                // Remove from deepest type
                Dictionary<Type, Type> deepestTypeDictionary;
                if (this.deepestTypes.TryGetValue(idHolder.ID, out deepestTypeDictionary))
                {
                    foreach (KeyValuePair<Type, Type> pair in deepestTypeDictionary)
                    {
                        if (type.Equals(pair.Key) || type.Equals(pair.Value))
                        {
                            deepestTypeDictionary.Remove(pair.Key);
                            break;
                        }
                    }
                    if (deepestTypeDictionary.Count == 0)
                        this.deepestTypes.Remove(idHolder.ID);
                }

                // Remember during which undo/redo moment this node was removed
                if (node != null)
                    this.removalMoments.Add(new Tuple<int, uint>(this.undoRedoCounter, node.ID));

                StopChange();
            }
        }
        #endregion Method: Remove(IdHolder idHolder)

        #endregion Method Group: Add/Remove

    }
    #endregion Class: Database

}
