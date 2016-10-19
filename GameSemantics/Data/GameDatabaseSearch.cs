/**************************************************************************
 * 
 * GameDatabaseSearch.cs
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
using System.Collections.ObjectModel;
using GameSemantics.Components;
using GameSemantics.GameContent;
using Semantics.Components;
using Semantics.Data;
using Semantics.Entities;

namespace GameSemantics.Data
{

    #region Class: GameDatabaseSearch
    /// <summary>
    /// The game database search contains all kinds of specific search methods through the game database.
    /// </summary>
    public static class GameDatabaseSearch
    {

        #region Field: disambiguationDictionary
        /// <summary>
        /// A dictionary in which one can define nodes with disambigue names.
        /// </summary>
        private static Dictionary<string, Node> disambiguationDictionary = new Dictionary<string, Node>();
        #endregion Field: disambiguationDictionary

        #region Method: AddDisambiguation(string name, Node node)
        /// <summary>
        /// Add a name/node pair to make a disambiguation between nodes with the same name when using the GetNode(string name) method.
        /// </summary>
        /// <param name="name">The name to use.</param>
        /// <param name="node">The node that will be returned when GetNode(string name) is called.</param>
        public static void AddDisambiguation(string name, Node node)
        {
            if (name != null && node != null)
            {
                if (disambiguationDictionary.ContainsKey(name))
                    disambiguationDictionary[name] = node;
                else
                    disambiguationDictionary.Add(name, node);
            }
        }
        #endregion Method: AddDisambiguation(string name, Node node)

        #region Method: RemoveDisambiguation(string name)
        /// <summary>
        /// Remove the disambiguation with the given name.
        /// </summary>
        /// <param name="name">The name to remove.</param>
        public static void RemoveDisambiguation(string name)
        {
            if (name != null)
                disambiguationDictionary.Remove(name);
        }
        #endregion Method: RemoveDisambiguation(string name)

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
            Node node;
            if (disambiguationDictionary.TryGetValue(name, out node))
            {
                if (node.GetType().Equals(typeof(T)))
                {
                    List<T> nodes = new List<T>();
                    nodes.Add((T)(object)node);
                    return nodes;
                }
            }

            return DatabaseSearch.GetNodes<T>(name);
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
            Node node;
            if (disambiguationDictionary.TryGetValue(name, out node))
            {
                if (node.GetType().Equals(typeof(T)))
                {
                    List<T> nodes = new List<T>();
                    nodes.Add((T)(object)node);
                    return nodes;
                }
            }

            return DatabaseSearch.GetNodes<T>(name, matchWholeWord);
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
            Node node;
            if (disambiguationDictionary.TryGetValue(name, out node))
            {
                List<Node> nodes = new List<Node>();
                nodes.Add(node);
                return nodes;
            }

            return DatabaseSearch.GetNodes(name);
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
            Node node;
            if (disambiguationDictionary.TryGetValue(name, out node))
            {
                List<Node> nodes = new List<Node>();
                nodes.Add(node);
                return nodes;
            }

            return DatabaseSearch.GetNodes(name, matchWholeWord);
        }
        #endregion Method: GetNodes(string name, bool matchWholeWord)

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
            Node node;
            if (disambiguationDictionary.TryGetValue(name, out node))
            {
                if (node.GetType().Equals(typeof(T)))
                    return (T)(object)node;
            }

            return DatabaseSearch.GetNode<T>(name);
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
            Node node;
            if (disambiguationDictionary.TryGetValue(name, out node))
            {
                if (node.GetType().Equals(typeof(T)))
                    return (T)(object)node;
            }

            return DatabaseSearch.GetNode<T>(name, matchWholeWord);
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
            Node node;
            if (disambiguationDictionary.TryGetValue(name, out node))
                return node;

            return DatabaseSearch.GetNode(name);
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
            Node node;
            if (disambiguationDictionary.TryGetValue(name, out node))
                return node;

            return DatabaseSearch.GetNode(name, matchWholeWord);
        }
        #endregion Method: GetNode(string name, bool matchWholeWord)		

        #region Method: GetGameNodes(Matter matter)
        /// <summary>
        /// Get the game nodes for the given matter.
        /// </summary>
        /// <param name="matter">The matter.</param>
        /// <returns>The game nodes for the matter.</returns>
        public static ReadOnlyCollection<GameNode> GetGameNodes(Matter matter)
        {
            List<GameNode> gameNodes = new List<GameNode>();

            // Get all game nodes where the given matter is the node
            gameNodes.AddRange(GameDatabase.Current.SelectAll<GameNode>(GameTables.GameNode, GameColumns.Node, matter));

            return gameNodes.AsReadOnly();
        }
        #endregion Method: GetGameNodes(Matter matter)

        #region Method: GetGameObjects(TangibleObject tangibleObject)
        /// <summary>
        /// Get the game objects for the given tangible object.
        /// </summary>g
        /// <param name="tangibleObject">The tangible object.</param>
        /// <returns>The game objects for the tangible object.</returns>
        public static ReadOnlyCollection<GameObject> GetGameObjects(TangibleObject tangibleObject)
        {
            List<GameObject> gameObjects = new List<GameObject>();

            // Get all game objects with the given tangible object
            gameObjects.AddRange(GameDatabase.Current.SelectAll<GameObject>(GameTables.GameObject, GameColumns.TangibleObject, tangibleObject));

            return gameObjects.AsReadOnly();
        }
        #endregion Method: GetGameObjects(TangibleObject tangibleObject)

        #region Method: GetTangibleObjects(string model)
        /// <summary>
        /// Get the tangibles objects for the given model name
        /// </summary>
        /// <param name="StaticContent">The name of the model</param>
        /// <returns>The tangible objects for the model.</returns>
        public static ReadOnlyCollection<TangibleObject> GetTangibleObjects(string model)
        {
            List<TangibleObject> nodes = new List<TangibleObject>();

            Node node = DatabaseSearch.GetNode(model);
            ReadOnlyCollection<AbstractGameNode> abstractnodes = new List<AbstractGameNode>().AsReadOnly();
            if (node is Model)
                abstractnodes = GetAbstractGameNodes(node as StaticContent);

            foreach (AbstractGameNode n in abstractnodes)
                if (n is GameObject)
                {
                    if(!nodes.Contains((n as GameObject).TangibleObject))
                        nodes.Add((n as GameObject).TangibleObject);
                }
            return nodes.AsReadOnly();
        }
        #endregion Method: GetTangibleObjects(string model)

        #region Method: GetAbstractGameNodes(DynamicContent dynamicContent)
        /// <summary>
        /// Get the abstract game nodes with the given dynamic content.
        /// </summary>
        /// <param name="dynamicContent">The dynamic content.</param>
        /// <returns>The abstract game nodes with the dynamic content.</returns>
        public static ReadOnlyCollection<AbstractGameNode> GetAbstractGameNodes(DynamicContent dynamicContent)
        {
            List<AbstractGameNode> abstractGameNodes = new List<AbstractGameNode>();

            foreach (ContentValued contentValued in GameDatabase.Current.SelectAll<ContentValued>(ValueTables.NodeValued, Columns.Node, dynamicContent.ID))
            {
                if (contentValued is DynamicContentValued)
                {
                    foreach (AbstractGameNode abstractGameNode in GameDatabase.Current.SelectAll<AbstractGameNode>(GameTables.AbstractGameNodeDynamicContent, GameColumns.DynamicContentValued, contentValued.ID))
                    {
                        if (!abstractGameNodes.Contains(abstractGameNode))
                            abstractGameNodes.Add(abstractGameNode);
                    }
                }
            }

            return abstractGameNodes.AsReadOnly();
        }
        #endregion Method: GetAbstractGameNodes(DynamicContent dynamicContent)

        #region Method: GetAbstractGameNodes(StaticContent staticContent)
        /// <summary>
        /// Get the abstract game nodes with the given static content.
        /// </summary>
        /// <param name="staticContent">The static content.</param>
        /// <returns>The abstract game nodes with the static content.</returns>
        public static ReadOnlyCollection<AbstractGameNode> GetAbstractGameNodes(StaticContent staticContent)
        {
            List<AbstractGameNode> abstractGameNodes = new List<AbstractGameNode>();

            foreach (ContentValued contentValued in GameDatabase.Current.SelectAll<ContentValued>(ValueTables.NodeValued, Columns.Node, staticContent.ID))
            {
                if (contentValued is StaticContentValued)
                {
                    foreach (ContentPerView contentPerView in GameDatabase.Current.SelectAll<ContentPerView>(GameTables.ContentPerViewContentValued, GameColumns.ContentValued, contentValued.ID))
                    {
                        foreach (ViewsPerContextType viewPerContextType in GameDatabase.Current.SelectAll<ViewsPerContextType>(GameTables.ViewsPerContextTypeContentPerView, GameColumns.ContentPerView, contentPerView.ID))
                        {
                            foreach (AbstractGameNode abstractGameNode in GameDatabase.Current.SelectAll<AbstractGameNode>(GameTables.AbstractGameNodeViewsPerContextType, GameColumns.ViewsPerContextType, viewPerContextType.ID))
                            {
                                if (!abstractGameNodes.Contains(abstractGameNode))
                                    abstractGameNodes.Add(abstractGameNode);
                            }
                        }
                    }
                }
            }

            return abstractGameNodes.AsReadOnly();
        }
        #endregion Method: GetAbstractGameNodes(StaticContent staticContent)

        #region Method: GetAbstractGameNodes(View view)
        /// <summary>
        /// Get the abstract game nodes with the given view.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <returns>The abstract game nodes with the view.</returns>
        public static ReadOnlyCollection<AbstractGameNode> GetAbstractGameNodes(View view)
        {
            List<AbstractGameNode> abstractGameNodes = new List<AbstractGameNode>();

                foreach (ContentPerView contentPerView in GameDatabase.Current.SelectAll<ContentPerView>(GameTables.ContentPerView, GameColumns.View, view.ID))
                {
                    foreach (ViewsPerContextType viewPerContextType in GameDatabase.Current.SelectAll<ViewsPerContextType>(GameTables.ViewsPerContextTypeContentPerView, GameColumns.ContentPerView, contentPerView.ID))
                    {
                        foreach (AbstractGameNode abstractGameNode in GameDatabase.Current.SelectAll<AbstractGameNode>(GameTables.AbstractGameNodeViewsPerContextType, GameColumns.ViewsPerContextType, viewPerContextType.ID))
                        {
                            if (!abstractGameNodes.Contains(abstractGameNode))
                                abstractGameNodes.Add(abstractGameNode);
                        }
                    }
                }

            return abstractGameNodes.AsReadOnly();
        }
        #endregion Method: GetAbstractGameNodes(View view)

    }
    #endregion Class: GameDatabaseSearch

}