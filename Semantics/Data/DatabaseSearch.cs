/**************************************************************************
 * 
 * DatabaseSearch.cs
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
using System.Collections.ObjectModel;
using Semantics.Abstractions;
using Semantics.Components;
using Semantics.Entities;
using Action = Semantics.Abstractions.Action;
using Attribute = Semantics.Abstractions.Attribute;

namespace Semantics.Data
{

    #region Class: DatabaseSearch
    /// <summary>
    /// The database search contains all kinds of specific search methods through the database.
    /// </summary>
    public static class DatabaseSearch
    {

        #region Method: GetNodes<T>()
        /// <summary>
        /// Get all the nodes of the given type.
        /// </summary>
        /// <typeparam name="T">The type to get the nodes of.</typeparam>
        /// <returns>All the nodes of the given type.</returns>
        public static List<T> GetNodes<T>()
            where T : Node
        {
            return GetNodes<T>(false);
        }
        #endregion Method: GetNodes<T>()

        #region Method: GetNodes<T>(bool setNames)
        /// <summary>
        /// Get all the nodes of the given type.
        /// </summary>
        /// <typeparam name="T">The type to get the nodes of.</typeparam>
        /// <param name="setNames">If true, all node names will be set immediately.</typeparam>
        /// <returns>All the nodes of the given type.</returns>
        public static List<T> GetNodes<T>(bool setNames)
            where T : Node
        {
            List<T> nodes = new List<T>();

            // Get all available names
            List<object[]> allNames = null;
            if (setNames)
            {
                string query = "SELECT " + Columns.ID + ", " + Columns.Name + ", " + Columns.IsDefaultName + " FROM " + LocalizationTables.NodeName + " ORDER BY " + Columns.ID + ";";
                allNames = Database.Current.IDatabase.QuerySelectAllColumns(DatabaseType.Localization, query, 3);
            }

            // Get all possible non-abstract derived types of T
            foreach (Type type in Database.Current.GetNonAbstractTypes(typeof(T)))
            {
                // Get all node IDs of the type
                string query = "SELECT " + Columns.ID + " FROM " + GenericTables.Node + " WHERE " + Columns.Type + " = '" + type.Name + "' ORDER BY " + Columns.ID + ";";
                List<object> nodeIDsOfType = Database.Current.IDatabase.QuerySelectAll(DatabaseType.Semantics, query);

                // Convert the IDs to new nodes
                nodes.AddRange(Database.Current.ConvertObjects<T>(nodeIDsOfType, type));

                // Set the names
                if (setNames)
                {
                    // Go through both lists and map the IDs with the names
                    if (nodeIDsOfType.Count > 0 && allNames.Count > 0)
                    {
                        int index1 = 0;
                        int index2 = 0;
                        long lastId = (long)nodeIDsOfType[nodeIDsOfType.Count - 1];
                        long id1 = (long)nodeIDsOfType[0];
                        long id2 = (long)allNames[0][0];
                        int allCount = allNames.Count;

                        // Go through the lists
                        while (index1 < nodeIDsOfType.Count && index2 < allCount)
                        {
                            if (id1 == id2)
                            {
                                T node = nodes[index1];

                                // Add a name
                                node.names = new List<string>();
                                string name = (string)allNames[index2][1];
                                node.names.Add(name);

                                // Check whether this name is the default name
                                if ((bool)allNames[index2][2])
                                    node.defaultName = name;

                                // Get the other names
                                if (index2 + 1 < allCount)
                                {
                                    do
                                    {
                                        ++index2;
                                        id2 = (long)allNames[index2][0];
                                        if (id2 == id1)
                                        {
                                            // Add another name and check whether it is the default one
                                            string otherName = (string)allNames[index2][1];
                                            node.names.Add(otherName);
                                            if ((bool)allNames[index2][2])
                                                node.defaultName = otherName;
                                        }
                                    } while (id2 == id1 && index2 + 1 < allCount);
                                }

                                // Go to the next one
                                ++index1;
                                if (index1 < nodeIDsOfType.Count)
                                    id1 = (long)nodeIDsOfType[index1];
                            }
                            else
                            {
                                // Go to the next one
                                ++index2;
                                if (index2 < allCount)
                                    id2 = (long)allNames[index2][0];
                            }
                        }
                    }
                }
            }

            return nodes;
        }
        #endregion Method: GetNodes<T>(bool setNames)

        #region Method: GetNodesWithLocalizationQuery<T>(string query)
        /// <summary>
        /// Get all nodes of the given type, while using the given query in the localization database.
        /// </summary>
        /// <typeparam name="T">The type of the node.</typeparam>
        /// <param name="query">The query for the localization database.</param>
        /// <returns>The nodes of the given type and what was defined in the query.</returns>
        private static List<T> GetNodesWithLocalizationQuery<T>(string query)
            where T : Node
        {
            List<T> nodes = new List<T>();
            List<object> nodeIDs = new List<object>();

            // Get all nodes with the given localization query
            List<object> localizedNodeIDs = Database.Current.IDatabase.QuerySelectAll(DatabaseType.Localization, query);

            foreach (Type type in Database.Current.GetNonAbstractTypes(typeof(T)))
            {
                // Get all nodes of the type
                query = "SELECT " + Columns.ID + " FROM " + GenericTables.Node + " WHERE " + Columns.Type + " = '" + type.Name + "';";
                List<object> nodeIDsOfType = Database.Current.IDatabase.QuerySelectAll(DatabaseType.Semantics, query);

                // Keep the nodes that are in both tables
                foreach (object obj in localizedNodeIDs)
                {
                    if (nodeIDsOfType.Contains(obj))
                        nodeIDs.Add(obj);
                }
            }

            // Convert the nodes
            nodes.AddRange(Database.Current.ConvertObjects<T>(nodeIDs, typeof(T)));
            return nodes;
        }
        #endregion Method: GetNodesWithLocalizationQuery<T>(string query)

        #region Method: GetNodes<T>(string name)
        /// <summary>
        /// Get the node of the given type with the given name.
        /// </summary>
        /// <typeparam name="T">The type of the node.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns>The node of the given type with the given name.</returns>
        public static List<T> GetNodes<T>(string name)
            where T : Node
        {
            if (name != null)
            {
                // Get all nodes with the exact same name as the given one
                string query = "SELECT " + Columns.ID + " FROM " + LocalizationTables.NodeName + " WHERE " + Columns.Name + " = '" + name.Replace("'", "''") + "';";
                return GetNodesWithLocalizationQuery<T>(query);
            }
            return new List<T>();
        }
        #endregion Method: GetNodes<T>(string name)

        #region Method: GetNodes<T>(string name, bool matchWholeWord)
        /// <summary>
        /// Get the node of the given type with the given name.
        /// </summary>
        /// <typeparam name="T">The type of the node.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="matchWholeWord">Indicates whether the entire name should match.</param>
        /// <returns>The node of the given type with the given name.</returns>
        public static List<T> GetNodes<T>(string name, bool matchWholeWord)
            where T : Node
        {
            if (name != null)
            {
                if (matchWholeWord)
                    return GetNodes<T>(name);

                // Get all nodes with a name like the given one
                string query = "SELECT " + Columns.ID + " FROM " + LocalizationTables.NodeName + " WHERE " + Columns.Name + " LIKE '%" + name.Replace("'", "''") + "%';";
                return GetNodesWithLocalizationQuery<T>(query);
            }
            return new List<T>();
        }
        #endregion Method: GetNodes<T>(string name, bool matchWholeWord)

        #region Method: GetNodes(string name)
        /// <summary>
        /// Get the node with the given name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The node with the given name.</returns>
        public static List<Node> GetNodes(string name)
        {
            if (name != null)
            {
                string query = "SELECT " + Columns.ID + " FROM " + LocalizationTables.NodeName + " WHERE " + Columns.Name + " = '" + name.Replace("'", "''") + "';";
                return Database.Current.ConvertObjects<Node>(Database.Current.IDatabase.QuerySelectAll(DatabaseType.Localization, query), typeof(Node));
            }
            return new List<Node>();
        }
        #endregion Method: GetNodes(string name)

        #region Method: GetNodes(string name, bool matchWholeWord)
        /// <summary>
        /// Get the node with the given name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="matchWholeWord">Indicates whether the entire name should match.</param>
        /// <returns>The node with the given name.</returns>
        public static List<Node> GetNodes(string name, bool matchWholeWord)
        {
            if (name != null)
            {
                if (matchWholeWord)
                    return GetNodes(name);

                string query = "SELECT " + Columns.ID + " FROM " + LocalizationTables.NodeName + " WHERE " + Columns.Name + " LIKE '%" + name.Replace("'", "''") + "%';";
                return Database.Current.ConvertObjects<Node>(Database.Current.IDatabase.QuerySelectAll(DatabaseType.Localization, query), typeof(Node));
            }
            return new List<Node>();
        }
        #endregion Method: GetNodes(string name, bool matchWholeWord)

        #region Method: GetNodesWithTag<T>(string tag)
        /// <summary>
        /// Get the node of the given type with the given tag.
        /// </summary>
        /// <typeparam name="T">The type of the node.</typeparam>
        /// <param name="tag">The tag.</param>
        /// <returns>The node of the given type with the given tag.</returns>
        public static List<T> GetNodesWithTag<T>(string tag)
            where T : Node
        {
            if (tag != null)
            {
                string query = "SELECT " + Columns.ID + " FROM " + LocalizationTables.NodeTag + " WHERE " + Columns.Tag + " = '" + tag.Replace("'", "''") + "';";
                return GetNodesWithLocalizationQuery<T>(query);
            }
            return new List<T>();
        }
        #endregion Method: GetNodesWithTag<T>(string tag)

        #region Method: GetNodesWithTag(string tag)
        /// <summary>
        /// Get the nodes with the given tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <returns>The nodes with the given tag.</returns>
        public static List<Node> GetNodesWithTag(string tag)
        {
            if (tag != null)
            {
                string query = "SELECT " + Columns.ID + " FROM " + LocalizationTables.NodeTag + " WHERE " + Columns.Tag + " = '" + tag.Replace("'", "''") + "';";
                return Database.Current.ConvertObjects<Node>(Database.Current.IDatabase.QuerySelectAll(DatabaseType.Localization, query), typeof(Node));
            }
            return new List<Node>();
        }
        #endregion Method: GetNodesWithTag(string tag)

        #region Method: GetNodesOfAuthor<T>(string author)
        /// <summary>
        /// Get the node of the given type of the given author.
        /// </summary>
        /// <typeparam name="T">The type of the node.</typeparam>
        /// <param name="author">The author.</param>
        /// <returns>The node of the given type of the given author.</returns>
        public static List<T> GetNodesOfAuthor<T>(string author)
            where T : Node
        {
            List<T> nodes = new List<T>();

            // Get all metadata of the given author, and the nodes belonging to them
            if (author != null)
            {
                List<Type> types = Database.Current.GetNonAbstractTypes(typeof(T));
                string metadataQuery = "SELECT " + Columns.ID + " FROM " + GenericTables.Metadata + " WHERE " + Columns.Author + " = '" + author.Replace("'", "''") + "';";
                foreach (Metadata metadata in Database.Current.ConvertObjects<Metadata>(Database.Current.IDatabase.QuerySelectAll(DatabaseType.Semantics, metadataQuery), typeof(Metadata)))
                {
                    foreach (Type type in types)
                    {
                        string query = "SELECT " + Columns.ID + " FROM " + GenericTables.Node + " WHERE " + Columns.Type + " = '" + type.Name + "' AND " + Columns.Metadata + " = '" + metadata.ID + "';";
                        nodes.AddRange(Database.Current.ConvertObjects<T>(Database.Current.IDatabase.QuerySelectAll(DatabaseType.Semantics, query), typeof(T)));
                    }
                }
            }

            return nodes;
        }
        #endregion Method: GetNodesOfAuthor<T>(string author)

        #region Method: GetNodesOfAuthor(string author)
        /// <summary>
        /// Get the nodes of the given author.
        /// </summary>
        /// <param name="author">The author.</param>
        /// <returns>The nodes of the given author.</returns>
        public static List<Node> GetNodesOfAuthor(string author)
        {
            List<Node> nodes = new List<Node>();

            // Get all metadata of the given author, and the nodes belonging to them
            if (author != null)
            {
                string metadataQuery = "SELECT " + Columns.ID + " FROM " + GenericTables.Metadata + " WHERE " + Columns.Author + " = '" + author.Replace("'", "''") + "';";
                foreach (Metadata metadata in Database.Current.ConvertObjects<Metadata>(Database.Current.IDatabase.QuerySelectAll(DatabaseType.Semantics, metadataQuery), typeof(Metadata)))
                {
                    string query = "SELECT " + Columns.ID + " FROM " + GenericTables.Node + " WHERE " + Columns.Metadata + " = '" + metadata.ID + "';";
                    nodes.AddRange(Database.Current.ConvertObjects<Node>(Database.Current.IDatabase.QuerySelectAll(DatabaseType.Semantics, query), typeof(Node)));
                }
            }

            return nodes;
        }
        #endregion Method: GetNodesOfAuthor(string author)

        #region Method: GetRootNodes()
        /// <summary>
        /// Get all the nodes without parents.
        /// </summary>
        /// <returns>All the nodes without parents.</returns>
        public static List<Node> GetRootNodes()
        {
            string query = "SELECT " + Columns.ID + " FROM " + GenericTables.Node + " AS t1 WHERE t1." + Columns.ID + " NOT IN (SELECT t2." + Columns.ID + " FROM " + GenericTables.NodeParent + " AS t2);";
            return Database.Current.ConvertObjects<Node>(Database.Current.IDatabase.QuerySelectAll(DatabaseType.Semantics, query), typeof(Node));
        }
        #endregion Method: GetRootNodes()

        #region Method: GetRootNodes<T>()
        /// <summary>
        /// Get all the nodes of the given type without parents.
        /// </summary>
        /// <typeparam name="T">The type of the node.</typeparam>
        /// <returns>All the nodes of the given type without parents.</returns>
        public static List<T> GetRootNodes<T>()
            where T : Node
        {
            List<T> rootNodes = new List<T>();
            foreach (Type type in Database.Current.GetNonAbstractTypes(typeof(T)))
            {
                string query = "SELECT " + Columns.ID + " FROM " + GenericTables.Node + " AS t1 WHERE t1." + Columns.Type + " = '" + type.Name + "' AND " + Columns.ID + " NOT IN (SELECT t2." + Columns.ID + " FROM " + GenericTables.NodeParent + " AS t2);";
                rootNodes.AddRange(Database.Current.ConvertObjects<T>(Database.Current.IDatabase.QuerySelectAll(DatabaseType.Semantics, query), typeof(T)));
            }
            return rootNodes;
        }
        #endregion Method: GetRootNodes<T>()

        #region Method: GetLeafNodes()
        /// <summary>
        /// Get all the nodes without children.
        /// </summary>
        /// <returns>All the nodes without children.</returns>
        public static List<Node> GetLeafNodes()
        {
            string query = "SELECT " + Columns.ID + " FROM " + GenericTables.Node + " AS t1 WHERE t1." + Columns.ID + " NOT IN (SELECT t2." + Columns.Parent + " FROM " + GenericTables.NodeParent + " AS t2);";
            return Database.Current.ConvertObjects<Node>(Database.Current.IDatabase.QuerySelectAll(DatabaseType.Semantics, query), typeof(Node));
        }
        #endregion Method: GetLeafNodes()

        #region Method: GetLeafNodes<T>()
        /// <summary>
        /// Get all the nodes of the given type without children.
        /// </summary>
        /// <typeparam name="T">The type of the node.</typeparam>
        /// <returns>All the nodes of the given type without children.</returns>
        public static List<T> GetLeafNodes<T>()
            where T : Node
        {
            List<T> leafNodes = new List<T>();
            foreach (Type type in Database.Current.GetNonAbstractTypes(typeof(T)))
            {
                string query = "SELECT " + Columns.ID + " FROM " + GenericTables.Node + " AS t1 WHERE t1." + Columns.Type + " = '" + type.Name + "' AND " + Columns.ID + " NOT IN (SELECT t2." + Columns.Parent + " FROM " + GenericTables.NodeParent + " AS t2);";
                leafNodes.AddRange(Database.Current.ConvertObjects<T>(Database.Current.IDatabase.QuerySelectAll(DatabaseType.Semantics, query), typeof(T)));
            }
            return leafNodes;
        }
        #endregion Method: GetLeafNodes<T>()

        #region Method: GetNode(uint id)
        /// <summary>
        /// Get the node with the given ID.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <returns>The node of the given type with the given ID.</returns>
        public static Node GetNode(uint id)
        {
            Type type = Database.Current.GetExactType(Database.Current.Select<string>(id, GenericTables.Node, Columns.Type));
            if (type != null)
                return Database.Current.ConvertToObject<Node>(id, type);

            return Database.Current.ConvertObject<Node>(id, typeof(Node));
        }
        #endregion Method: GetNode(uint id)

        #region Method: GetNode<T>(uint id)
        /// <summary>
        /// Get the node with the given ID.
        /// </summary>
        /// <typeparam name="T">The type of the node.</typeparam>
        /// <param name="id">The ID.</param>
        /// <returns>The node of the given type with the given ID.</returns>
        public static T GetNode<T>(uint id)
            where T : Node
        {
            if (typeof(T) == typeof(Node))
            {
                Type type = Database.Current.GetExactType(Database.Current.Select<string>(id, GenericTables.Node, Columns.Type));
                if (type != null)
                    return Database.Current.ConvertToObject<T>(id, type);
            }

            return Database.Current.ConvertObject<T>(id, typeof(T));
        }
        #endregion Method: GetNode<T>(uint id)

        #region Method: GetNodeWithLocalizationQuery<T>(string query)
        /// <summary>
        /// Get the node of the given type, while using the given query in the localization database.
        /// </summary>
        /// <typeparam name="T">The type of the node.</typeparam>
        /// <param name="query">The query for the localization database.</param>
        /// <returns>The node of the given type and what was defined in the query.</returns>
        private static T GetNodeWithLocalizationQuery<T>(string query)
            where T : Node
        {
            // Get the IDs of the nodes with the given query
            List<object> localizedNodeIDs = Database.Current.IDatabase.QuerySelectAll(DatabaseType.Localization, query);

            // Get all possible types
            List<Type> types = Database.Current.GetNonAbstractTypes(typeof(T));
            List<string> typeNames = new List<string>();
            foreach (Type type in types)
                typeNames.Add(type.Name);

            // Check whether one of the found node IDs is of the possible type
            foreach (object localizedNodeId in localizedNodeIDs)
            {
                query = "SELECT " + Columns.Type + " FROM " + GenericTables.Node + " WHERE " + Columns.ID + " = '" + localizedNodeId + "';";
                object typeName = Database.Current.IDatabase.QuerySelect(DatabaseType.Semantics, query);
                if (typeName is string && typeNames.Contains((string)typeName))
                    return Database.Current.ConvertObject<T>(localizedNodeId, typeof(T));
            }

            return default(T);
        }
        #endregion Method: GetNodeWithLocalizationQuery<T>(string query)

        #region Method: GetNode<T>(string name)
        /// <summary>
        /// Get the node of the given type with the given name.
        /// </summary>
        /// <typeparam name="T">The type of the node.</typeparam>
        /// <param name="name">The name.</param>
        /// <returns>The node of the given type with the given name.</returns>
        public static T GetNode<T>(string name)
            where T : Node
        {
            if (name != null)
            {
                string query = "SELECT " + Columns.ID + " FROM " + LocalizationTables.NodeName + " WHERE " + Columns.Name + " = '" + name.Replace("'", "''") + "';";
                return GetNodeWithLocalizationQuery<T>(query);
            }
            return default(T);
        }
        #endregion Method: GetNode<T>(string name)

        #region Method: GetNode<T>(string name, bool matchWholeWord)
        /// <summary>
        /// Get the node of the given type with the given name.
        /// </summary>
        /// <typeparam name="T">The type of the node.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="matchWholeWord">Indicates whether the entire name should match.</param>
        /// <returns>The node of the given type with the given name.</returns>
        public static T GetNode<T>(string name, bool matchWholeWord)
            where T : Node
        {
            if (name != null)
            {
                if (matchWholeWord)
                    return GetNode<T>(name);

                string query = "SELECT " + Columns.ID + " FROM " + LocalizationTables.NodeName + " WHERE " + Columns.Name + " LIKE '%" + name.Replace("'", "''") + "%';";
                return GetNodeWithLocalizationQuery<T>(query);
            }
            return default(T);
        }
        #endregion Method: GetNode<T>(string name, bool matchWholeWord)

        #region Method: GetNode(string name)
        /// <summary>
        /// Get the node with the given name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The node with the given name.</returns>
        public static Node GetNode(string name)
        {
            if (name != null)
            {
                string query = "SELECT " + Columns.ID + " FROM " + LocalizationTables.NodeName + " WHERE " + Columns.Name + " = '" + name.Replace("'", "''") + "';";
                return Database.Current.ConvertObject<Node>(Database.Current.IDatabase.QuerySelect(DatabaseType.Localization, query), typeof(Node));
            }
            return null;
        }
        #endregion Method: GetNode(string name)

        #region Method: GetNode(string name, bool matchWholeWord)
        /// <summary>
        /// Get the node with the given name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="matchWholeWord">Indicates whether the entire name should match.</param>
        /// <returns>The node with the given name.</returns>
        public static Node GetNode(string name, bool matchWholeWord)
        {
            if (name != null)
            {
                if (matchWholeWord)
                    return GetNode(name);

                string query = "SELECT " + Columns.ID + " FROM " + LocalizationTables.NodeName + " WHERE " + Columns.Name + " LIKE '%" + name.Replace("'", "''") + "%';";
                return Database.Current.ConvertObject<Node>(Database.Current.IDatabase.QuerySelect(DatabaseType.Localization, query), typeof(Node));
            }
            return null;
        }
        #endregion Method: GetNode(string name, bool matchWholeWord)

        #region Method: GetTags()
        /// <summary>
        /// Get all the tags that have been assigned to nodes.
        /// </summary>
        public static List<String> GetTags()
        {
            return Database.Current.SelectAll<String>(true, LocalizationTables.NodeTag, Columns.Tag);
        }
        #endregion Method: GetTags()

        #region Method: GetAuthors()
        /// <summary>
        /// Get all the authors that have created nodes.
        /// </summary>
        public static List<String> GetAuthors()
        {
            return Database.Current.SelectAll<String>(true, GenericTables.Metadata, Columns.Author);
        }
        #endregion Method: GetAuthors()

        #region Method: GetCompounds(Substance substance)
        /// <summary>
        /// Get the compounds with the given substance.
        /// </summary>
        /// <param name="substance">The substance.</param>
        /// <returns>The compounds with the substance.</returns>
        public static ReadOnlyCollection<Compound> GetCompounds(Substance substance)
        {
            List<Compound> compounds = new List<Compound>();

            // Get all valued substances of the given substance, and the compound belonging to them
            if (substance != null)
            {
                foreach (SubstanceValued substanceValued in Database.Current.SelectAll<SubstanceValued>(ValueTables.NodeValued, Columns.Node, substance.ID))
                {
                    foreach (Compound compound in Database.Current.SelectAll<Compound>(GenericTables.CompoundSubstance, Columns.SubstanceValued, substanceValued.ID))
                    {
                        if (!compounds.Contains(compound))
                            compounds.Add(compound);
                    }
                }
            }

            return compounds.AsReadOnly();
        }
        #endregion Method: GetCompounds(Substance substance)

        #region Method: GetEntities(Action action)
        /// <summary>
        /// Get the entities that can perform the given action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The entities that can perform the action.</returns>
        public static ReadOnlyCollection<Entity> GetEntities(Action action)
        {
            List<Entity> entities = new List<Entity>();

            // Get all actors of the events that are based on the given action
            if (action != null)
            {
                foreach (Event even in Database.Current.SelectAll<Event>(GenericTables.Event, Columns.Action, action.ID))
                {
                    Entity entity = even.Actor;
                    if (!entities.Contains(entity))
                        entities.Add(entity);
                }
            }

            return entities.AsReadOnly();
        }
        #endregion Method: GetEntities(Action action)

        #region Method: GetEntities(Attribute attribute)
        /// <summary>
        /// Get the entities with the given attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns>The entities with the attribute.</returns>
        public static ReadOnlyCollection<Entity> GetEntities(Attribute attribute)
        {
            List<Entity> entities = new List<Entity>();

            // Get all valued attributes of the given attribute, and the entities belonging to them
            if (attribute != null)
            {
                foreach (AttributeValued attributeValued in Database.Current.SelectAll<AttributeValued>(ValueTables.NodeValued, Columns.Node, attribute.ID))
                {
                    foreach (Entity entity in Database.Current.SelectAll<Entity>(GenericTables.EntityAttribute, Columns.AttributeValued, attributeValued.ID))
                    {
                        if (!entities.Contains(entity))
                            entities.Add(entity);
                    }
                }
            }

            return entities.AsReadOnly();
        }
        #endregion Method: GetEntities(Attribute attribute)

        #region Method: GetEntities(Group group)
        /// <summary>
        /// Get the entities that belong to the given group.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <returns>The entities that belong to the group.</returns>
        public static ReadOnlyCollection<Entity> GetEntities(Group group)
        {
            if (group != null)
                return group.Entities;

            return new List<Entity>().AsReadOnly();
        }
        #endregion Method: GetEntities(Group group)

        #region Method: GetEntities(RelationshipType relationshipType)
        /// <summary>
        /// Get the entities that are the source of a relationship of the given type.
        /// </summary>
        /// <param name="relationshipType">The relationship type.</param>
        /// <returns>The entities that are the source of a relationship of the given type.</returns>
        public static ReadOnlyCollection<Entity> GetEntities(RelationshipType relationshipType)
        {
            List<Entity> entities = new List<Entity>();

            // Get all sources of the relationships of the given type
            if (relationshipType != null)
            {
                foreach (Relationship relationship in Database.Current.SelectAll<Relationship>(GenericTables.Relationship, Columns.RelationshipType, relationshipType.ID))
                {
                    Entity entity = relationship.Source;
                    if (!entities.Contains(entity))
                        entities.Add(entity);
                }
            }

            return entities.AsReadOnly();
        }
        #endregion Method: GetEntities(RelationshipType relationshipType)

        #region Method: GetEntities(PredicateType predicateType)
        /// <summary>
        /// Get the entities that are the actor of a predicate of the given type.
        /// </summary>
        /// <param name="predicateType">The predicate type.</param>
        /// <returns>The entities that are the actor of a predicate of the given type.</returns>
        public static ReadOnlyCollection<Entity> GetEntities(PredicateType predicateType)
        {
            List<Entity> entities = new List<Entity>();

            // Get all actors of the predicates of the given type
            if (predicateType != null)
            {
                foreach (Predicate predicate in Database.Current.SelectAll<Predicate>(GenericTables.Predicate, Columns.PredicateType, predicateType.ID))
                {
                    Entity entity = predicate.Actor;
                    if (!entities.Contains(entity))
                        entities.Add(entity);
                }
            }

            return entities.AsReadOnly();
        }
        #endregion Method: GetEntities(PredicateType predicateType)

        #region Method: GetEntities(State state)
        /// <summary>
        /// Get the entities with the given state.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>The entities with the state.</returns>
        public static ReadOnlyCollection<Entity> GetEntities(State state)
        {
            List<Entity> entities = new List<Entity>();

            // Get all state groups that have the particular state, and the entities with those state groups
            if (state != null)
            {
                foreach (StateGroup stateGroup in Database.Current.SelectAll<StateGroup>(GenericTables.StateGroupState, Columns.State, state.ID))
                {
                    foreach (Entity entity in Database.Current.SelectAll<Entity>(GenericTables.EntityStateGroup, Columns.StateGroup, stateGroup.ID))
                    {
                        if (!entities.Contains(entity))
                            entities.Add(entity);
                    }
                }
            }

            return entities.AsReadOnly();
        }
        #endregion Method: GetEntities(State state)

        #region Method: GetMatter(Element element)
        /// <summary>
        /// Get the matter with the given element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>The matter with the element.</returns>
        public static ReadOnlyCollection<Matter> GetMatter(Element element)
        {
            List<Matter> matter = new List<Matter>();

            // Get all valued elements of the given element, and the matter belonging to them
            if (element != null)
            {
                foreach (ElementValued elementValued in Database.Current.SelectAll<ElementValued>(ValueTables.NodeValued, Columns.Node, element.ID))
                {
                    foreach (Matter mat in Database.Current.SelectAll<Matter>(GenericTables.MatterElement, Columns.ElementValued, elementValued.ID))
                    {
                        if (!matter.Contains(mat))
                            matter.Add(mat);
                    }
                }
            }

            return matter.AsReadOnly();
        }
        #endregion Method: GetMatter(Element element)

        #region Method: GetMixtures(Mixture mixture)
        /// <summary>
        /// Get the mixtures with the given mixture.
        /// </summary>
        /// <param name="mixture">The mixture.</param>
        /// <returns>The mixtures with the mixture.</returns>
        public static ReadOnlyCollection<Mixture> GetMixtures(Mixture mixture)
        {
            List<Mixture> mixtures = new List<Mixture>();

            // Get all valued mixtures of the given mixture, and the mixture belonging to them
            if (mixture != null)
            {
                foreach (MixtureValued mixtureValued in Database.Current.SelectAll<MixtureValued>(ValueTables.NodeValued, Columns.Node, mixture.ID))
                {
                    foreach (Mixture mix in Database.Current.SelectAll<Mixture>(GenericTables.MixtureMixture, Columns.MixtureValued, mixtureValued.ID))
                    {
                        if (!mixtures.Contains(mix))
                            mixtures.Add(mix);
                    }
                }
            }

            return mixtures.AsReadOnly();
        }
        #endregion Method: GetMixtures(Mixture mixture)

        #region Method: GetMixtures(Substance substance)
        /// <summary>
        /// Get the mixtures with the given substance.
        /// </summary>
        /// <param name="substance">The substance.</param>
        /// <returns>The mixtures with the substance.</returns>
        public static ReadOnlyCollection<Mixture> GetMixtures(Substance substance)
        {
            List<Mixture> mixtures = new List<Mixture>();

            // Get all valued substances of the given substance, and the mixture belonging to them
            if (substance != null)
            {
                foreach (SubstanceValued substanceValued in Database.Current.SelectAll<SubstanceValued>(ValueTables.NodeValued, Columns.Node, substance.ID))
                {
                    foreach (Mixture mixture in Database.Current.SelectAll<Mixture>(GenericTables.MixtureSubstance, Columns.SubstanceValued, substanceValued.ID))
                    {
                        if (!mixtures.Contains(mixture))
                            mixtures.Add(mixture);
                    }
                }
            }

            return mixtures.AsReadOnly();
        }
        #endregion Method: GetMixtures(Substance substance)

        #region Method: GetPhysicalObjects(Scene scene)
        /// <summary>
        /// Get the physical objects in the given scene.
        /// </summary>
        /// <param name="scene">The scene.</param>
        /// <returns>The physical objects in the scene.</returns>
        public static ReadOnlyCollection<PhysicalObject> GetPhysicalObjects(Scene scene)
        {
            List<PhysicalObject> physicalObjects = new List<PhysicalObject>();

            // Get all physical objects of the valued physical objects in the scene
            if (scene != null)
            {
                foreach (PhysicalObjectValued physicalObjectValued in scene.PhysicalObjects)
                {
                    PhysicalObject physicalObject = physicalObjectValued.PhysicalObject;
                    if (!physicalObjects.Contains(physicalObject))
                        physicalObjects.Add(physicalObject);
                }
            }

            return physicalObjects.AsReadOnly();
        }
        #endregion Method: GetPhysicalObjects(Scene scene)

        #region Method: GetPhysicalObjects(Space space)
        /// <summary>
        /// Get the physical objects with the given space.
        /// </summary>
        /// <param name="space">The space.</param>
        /// <returns>The physical objects with the space.</returns>
        public static ReadOnlyCollection<PhysicalObject> GetPhysicalObjects(Space space)
        {
            List<PhysicalObject> physicalObjects = new List<PhysicalObject>();

            // Get all valued spaces of the given space, and the physical objects belonging to them
            if (space != null)
            {
                foreach (SpaceValued spaceValued in Database.Current.SelectAll<SpaceValued>(ValueTables.NodeValued, Columns.Node, space.ID))
                {
                    foreach (PhysicalObject physicalObject in Database.Current.SelectAll<PhysicalObject>(GenericTables.PhysicalObjectSpace, Columns.SpaceValued, spaceValued.ID))
                    {
                        if (!physicalObjects.Contains(physicalObject))
                            physicalObjects.Add(physicalObject);
                    }
                }
            }

            return physicalObjects.AsReadOnly();
        }
        #endregion Method: GetPhysicalObjects(Space space)

        #region Method: GetTangibleObjects(Matter matter)
        /// <summary>
        /// Get the tangible objects with the given matter.
        /// </summary>
        /// <param name="matter">The matter.</param>
        /// <returns>The tangible objects with the matter.</returns>
        public static ReadOnlyCollection<TangibleObject> GetTangibleObjects(Matter matter)
        {
            List<TangibleObject> tangibleObjects = new List<TangibleObject>();

            // Get all valued matter of the given matter, and the tangible objects belonging to them
            if (matter != null)
            {
                foreach (MatterValued matterValued in Database.Current.SelectAll<MatterValued>(ValueTables.NodeValued, Columns.Node, matter.ID))
                {
                    foreach (TangibleObject tangibleObject in Database.Current.SelectAll<TangibleObject>(GenericTables.TangibleObject, Columns.MatterValued, matterValued.ID))
                    {
                        if (!tangibleObjects.Contains(tangibleObject))
                            tangibleObjects.Add(tangibleObject);
                    }
                }
            }

            return tangibleObjects.AsReadOnly();
        }
        #endregion Method: GetTangibleObjects(Matter matter)

        #region Method: GetScenes(PhysicalObject physicalObject)
        /// <summary>
        /// Get the scenes with the given physical object.
        /// </summary>
        /// <param name="physicalObject">The physical object.</param>
        /// <returns>The scenes with the physical object.</returns>
        public static ReadOnlyCollection<Scene> GetScenes(PhysicalObject physicalObject)
        {
            List<Scene> scenes = new List<Scene>();

            if (physicalObject != null)
            {
                // Get all valued physical objects of the given physical object, and get all scenes having them
                foreach (PhysicalObjectValued physicalObjectValued in Database.Current.SelectAll<PhysicalObjectValued>(ValueTables.PhysicalObjectValued, Columns.PhysicalObject, physicalObject.ID))
                {
                    foreach (Scene scene in Database.Current.SelectAll<Scene>(GenericTables.ScenePhysicalObject, Columns.PhysicalObjectValued, physicalObjectValued.ID))
                    {
                        if (!scenes.Contains(scene))
                            scenes.Add(scene);
                    }
                }
            }

            return scenes.AsReadOnly();
        }
        #endregion Method: GetScenes(PhysicalObject physicalObject)

    }
    #endregion Class: DatabaseSearch

}