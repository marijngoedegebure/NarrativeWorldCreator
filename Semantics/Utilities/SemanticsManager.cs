/**************************************************************************
 * 
 * SemanticsManager.cs
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
using System.Globalization;
using Common;
using Semantics.Abstractions;
using Semantics.Components;
using Semantics.Data;
using Attribute = Semantics.Abstractions.Attribute;

namespace Semantics.Utilities
{

    #region Class: SemanticsManager
    /// <summary>
    /// A manager for all semantics.
    /// </summary>
    public static class SemanticsManager
    {

        #region Event: SpecialAttributeChanged
        /// <summary>
        /// A handler for a changed special attribute.
        /// </summary>
        /// <param name="specialAttribute">The special attribute that was changed.</param>
        /// <param name="attribute">The attribute that has been assigned to the special attribute.</param>
        public delegate void SpecialAttributesHandler(SpecialAttributes specialAttribute, Attribute attribute);

        /// <summary>
        /// Invoked when the attribute of a special attribute changes.
        /// </summary>
        public static event SpecialAttributesHandler SpecialAttributeChanged;
        #endregion Event: SpecialAttributeChanged

        #region Event: SpecialUnitCategoryChanged
        /// <summary>
        /// A handler for a changed special unit category.
        /// </summary>
        /// <param name="specialUnitCategory">The special unit category that was changed.</param>
        /// <param name="unitCategory">The unit category that has been assigned to the special unit category.</param>
        public delegate void SpecialUnitCategoriesHandler(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory);

        /// <summary>
        /// Invoked when the unit category of a special unit category changes.
        /// </summary>
        public static event SpecialUnitCategoriesHandler SpecialUnitCategoryChanged;
        #endregion Event: SpecialUnitCategoryChanged

        #region Field: nodeModels
        /// <summary>
        /// A dictionary with all cached nodes per type.
        /// </summary>
        private static Dictionary<Type, NodeModel> nodeModels = new Dictionary<Type, NodeModel>();
        #endregion Field: nodeModels

        #region Field: nodeAddedHandler
        /// <summary>
        /// A handler for an added node in the database.
        /// </summary>
        private static Database.NodeHandler nodeAddedHandler = null;
        #endregion Field: nodeAddedHandler

        #region Field: nodeRemovedHandler
        /// <summary>
        /// A handler for a removed node in the database.
        /// </summary>
        private static Database.NodeHandler nodeRemovedHandler = null;
        #endregion Field: nodeRemovedHandler

        #region Method: GetNodeModel<T>()
        /// <summary>
        /// Gets the node model with all nodes of the given type.
        /// </summary>
        /// <typeparam name="T">The type to get the nodes of.</typeparam>
        /// <returns>The node model with all nodes of the given type.</returns>
        public static NodeModel GetNodeModel<T>()
            where T : Node
        {
            NodeModel nodeModel;

            // Try to get the node model from the cache
            Type type = typeof(T);
            if (!nodeModels.TryGetValue(type, out nodeModel))
            {
                // Otherwise, create a new node model
                nodeModel = new NodeModel();

                // Retrieve its nodes from the database
                ObservableCollection<Node> nodes = new ObservableCollection<Node>();
                List<T> allNodes = DatabaseSearch.GetNodes<T>(true);
                allNodes.Sort(delegate(T n1, T n2) { return string.Compare(((Node)(object)n1).DefaultName, ((Node)(object)n2).DefaultName, CommonSettings.Culture, CompareOptions.None); });
                foreach (T node in allNodes)
                    nodes.Add((Node)(object)node);
                nodeModel.Nodes = nodes;

                // Set the root nodes
                ObservableCollection<Node> rootNodes = new ObservableCollection<Node>();
                List<T> allRootNodes = DatabaseSearch.GetRootNodes<T>();
                allRootNodes.Sort(delegate(T n1, T n2) { return string.Compare(((Node)(object)n1).DefaultName, ((Node)(object)n2).DefaultName, CommonSettings.Culture, CompareOptions.None); });
                foreach (T node in allRootNodes)
                    rootNodes.Add((Node)(object)node);
                nodeModel.RootNodes = rootNodes;

                // Listen to added and removed node events
                if (nodeAddedHandler == null)
                {
                    nodeAddedHandler = new Database.NodeHandler(Database_NodeAdded);
                    Database.Current.NodeAdded += nodeAddedHandler;
                }
                if (nodeRemovedHandler == null)
                {
                    nodeRemovedHandler = new Database.NodeHandler(Database_NodeRemoved);
                    Database.Current.NodeRemoved += nodeRemovedHandler;
                }

                // Cache it
                nodeModels.Add(type, nodeModel);
            }

            return nodeModel;
        }
        #endregion Method: GetNodeModel<T>()

        #region Method: Database_NodeAdded(Node node)
        /// <summary>
        /// Adds a node to the correct existing node model when it is added to the database.
        /// </summary>
        /// <param name="node">The node to add.</param>
        private static void Database_NodeAdded(Node node)
        {
            if (node != null)
            {
                Type type = node.GetType();
                NodeModel nodeModel;
                if (nodeModels.TryGetValue(type, out nodeModel))
                    nodeModel.AddNode(node);
            }
        }
        #endregion Method: Database_NodeAdded(Node node)

        #region Method: Database_NodeRemoved(Node node)
        /// <summary>
        /// Handles removed nodes.
        /// </summary>
        /// <param name="node">The removed node.</param>
        private static void Database_NodeRemoved(Node node)
        {
            if (node != null)
            {
                // Remove the node from the correct existing node model when it is removed from the database
                Type type = node.GetType();
                NodeModel nodeModel;
                if (nodeModels.TryGetValue(type, out nodeModel))
                    nodeModel.RemoveNode(node);

                // Reset the special attribute/unit category when it is removed
                if (node is Attribute)
                {
                    SpecialAttributes? specialAttribute = Database.Current.Select<SpecialAttributes?>(GenericTables.SpecialAttributes, Columns.SpecialAttribute, Columns.Attribute, node);
                    if (specialAttribute != null)
                        SetSpecialAttribute((SpecialAttributes)specialAttribute, null);
                }
                else if (node is UnitCategory)
                {
                    SpecialUnitCategories? specialUnitCategory = Database.Current.Select<SpecialUnitCategories?>(GenericTables.SpecialUnitCategories, Columns.SpecialUnitCategory, Columns.UnitCategory, node);
                    if (specialUnitCategory != null)
                        SetSpecialUnitCategory((SpecialUnitCategories)specialUnitCategory, null);
                }
            }
        }
        #endregion Method: Database_NodeRemoved(Node node)

        #region Method: Uninitialize()
        /// <summary>
        /// Uninitialize the node models.
        /// </summary>
        internal static void Uninitialize()
        {
            // Clear all node models
            foreach (NodeModel nodeModel in nodeModels.Values)
                nodeModel.Clear();
            nodeModels.Clear();

            // Remove the handlers
            if (nodeAddedHandler != null)
            {
                Database.Current.NodeAdded -= nodeAddedHandler;
                nodeAddedHandler = null;
            }
            if (nodeRemovedHandler != null)
            {
                Database.Current.NodeRemoved -= nodeRemovedHandler;
                nodeRemovedHandler = null;
            }
        }
        #endregion Method: Uninitialize()

        #region Method: GetSpecialAttribute(SpecialAttributes specialAttribute)
        /// <summary>
        /// Get the attribute that has been assigned to the given special attribute type.
        /// </summary>
        /// <param name="specialAttribute">The special attribute.</param>
        /// <returns>The attribute that has been assigned to the special attribute type.</returns>
        public static Attribute GetSpecialAttribute(SpecialAttributes specialAttribute)
        {
            if (Database.Current != null)
            {
                // Subscribe to removed nodes
                if (nodeRemovedHandler == null)
                {
                    nodeRemovedHandler = new Database.NodeHandler(Database_NodeRemoved);
                    Database.Current.NodeRemoved += nodeRemovedHandler;
                }

                // Get the special attribute
                return Database.Current.Select<Attribute>(GenericTables.SpecialAttributes, Columns.Attribute, Columns.SpecialAttribute, specialAttribute);
            }
            return null;
        }
        #endregion Method: GetSpecialAttribute(SpecialAttributes specialAttribute)

        #region Method: SetSpecialAttribute(SpecialAttributes specialAttribute, Attribute attribute)
        /// <summary>
        /// Set the special attribute of the given type with the given attribute.
        /// </summary>
        /// <param name="specialAttribute">The special attribute.</param>
        /// <param name="attribute">The attribute to assign to the special attribute; can be null to remove the current value.</param>
        /// <returns>Returns whether the assignment has been successful.</returns>
        public static Message SetSpecialAttribute(SpecialAttributes specialAttribute, Attribute attribute)
        {
            if (Database.Current != null)
            {
                Attribute currentAttribute = GetSpecialAttribute(specialAttribute);
                if (currentAttribute == null)
                    Database.Current.Insert(GenericTables.SpecialAttributes, new string[] { Columns.SpecialAttribute, Columns.Attribute }, new object[] { specialAttribute, attribute });
                else
                {
                    if (attribute == null)
                        Database.Current.Remove(GenericTables.SpecialAttributes, Columns.Attribute, currentAttribute);
                    else
                        Database.Current.Update(GenericTables.SpecialAttributes, Columns.Attribute, attribute, Columns.SpecialAttribute, specialAttribute);
                }

                if (SpecialAttributeChanged != null)
                    SpecialAttributeChanged(specialAttribute, attribute);

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: SetSpecialAttribute(SpecialAttributes specialAttribute, Attribute attribute)

        #region Method: GetSpecialUnitCategory(SpecialUnitCategories specialUnitCategory)
        /// <summary>
        /// Get the unit category that has been assigned to the given special unit category type.
        /// </summary>
        /// <param name="specialUnitCategory">The special unit category.</param>
        /// <returns>The unit category that has been assigned to the special unit category type.</returns>
        public static UnitCategory GetSpecialUnitCategory(SpecialUnitCategories specialUnitCategory)
        {
            if (Database.Current != null)
            {
                // Subscribe to removed nodes
                if (nodeRemovedHandler == null)
                {
                    nodeRemovedHandler = new Database.NodeHandler(Database_NodeRemoved);
                    Database.Current.NodeRemoved += nodeRemovedHandler;
                }

                // Get the special unit category
                return Database.Current.Select<UnitCategory>(GenericTables.SpecialUnitCategories, Columns.UnitCategory, Columns.SpecialUnitCategory, specialUnitCategory);
            }
            return null;
        }
        #endregion Method: GetSpecialUnitCategory(SpecialUnitCategories specialAttribute)

        #region Method: SetSpecialUnitCategory(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)
        /// <summary>
        /// Set the special unit category of the given type with the given unit category.
        /// </summary>
        /// <param name="specialUnitCategory">The special unit category.</param>
        /// <param name="unitCategory">The unit category to assign to the special unit category; can be null to remove the current value.</param>
        /// <returns>Returns whether the assignment has been successful.</returns>
        public static Message SetSpecialUnitCategory(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)
        {
            if (Database.Current != null)
            {
                UnitCategory currentUnitCategory = GetSpecialUnitCategory(specialUnitCategory);
                if (currentUnitCategory == null)
                    Database.Current.Insert(GenericTables.SpecialUnitCategories, new string[] { Columns.SpecialUnitCategory, Columns.UnitCategory }, new object[] { specialUnitCategory, unitCategory });
                else
                {
                    if (unitCategory == null)
                        Database.Current.Remove(GenericTables.SpecialUnitCategories, Columns.UnitCategory, currentUnitCategory);
                    else
                        Database.Current.Update(GenericTables.SpecialUnitCategories, Columns.UnitCategory, unitCategory, Columns.SpecialUnitCategory, specialUnitCategory);
                }

                if (SpecialUnitCategoryChanged != null)
                    SpecialUnitCategoryChanged(specialUnitCategory, unitCategory);

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: SetSpecialUnitCategory(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)

    }
    #endregion Class: SemanticsManager

}