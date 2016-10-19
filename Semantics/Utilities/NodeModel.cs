/**************************************************************************
 * 
 * NodeModel.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Semantics.Components;

namespace Semantics.Utilities
{

    #region Class: NodeModel
    /// <summary>
    /// A node model stores nodes and root nodes of a particular type as observable collections.
    /// </summary>
    public class NodeModel : PropertyChangedComponent
    {

        #region Properties and Fields

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

        #region Property: Nodes
        /// <summary>
        /// The collection of nodes of this model.
        /// </summary>
        private ObservableCollection<Node> nodes = new ObservableCollection<Node>();

        /// <summary>
        /// Gets the collection of nodes of this model.
        /// </summary>
        public ObservableCollection<Node> Nodes
        {
            get
            {
                return nodes;
            }
            internal set
            {
                nodes = value;
                NotifyPropertyChanged("Nodes");
            }
        }
        #endregion Property: Nodes

        #region Property: RootNodes
        /// <summary>
        /// The collection of root nodes of this model.
        /// </summary>
        private ObservableCollection<Node> rootNodes = new ObservableCollection<Node>();

        /// <summary>
        /// The collection of root nodes of this model.
        /// </summary>
        public ObservableCollection<Node> RootNodes
        {
            get
            {
                return rootNodes;
            }
            internal set
            {
                // Unsubscribe from node changes
                if (rootNodes != null)
                {
                    foreach (Node node in rootNodes)
                        node.PropertyChanged -= this.propertyChangedHandler;
                }

                rootNodes = value;
                NotifyPropertyChanged("RootNodes");

                // Subscribe to node changes
                if (rootNodes != null)
                {
                    foreach (Node node in rootNodes)
                        node.PropertyChanged += this.propertyChangedHandler;
                }
            }
        }
        #endregion Property: RootNodes

        #region Field: propertyChangedHandler
        /// <summary>
        /// A handler for changed node properties.
        /// </summary>
        private PropertyChangedEventHandler propertyChangedHandler;
        #endregion Field: propertyChangedHandler
        
        #endregion Properties and Fields

        #region Constructor: NodeModel()
        /// <summary>
        /// Creates a new node model.
        /// </summary>
        public NodeModel()
        {
            this.propertyChangedHandler = new PropertyChangedEventHandler(node_PropertyChanged);
        }
        #endregion Constructor: NodeModel() 

        #region Method: AddNode(Node node)
        /// <summary>
        /// Adds a node.
        /// </summary>
        /// <param name="node">The node to add.</param>
        public void AddNode(Node node)
        {
            if (node != null)
            {
                this.nodes.Add(node);
                NotifyPropertyChanged("Nodes");

                if (node.IsRoot)
                {
                    this.rootNodes.Add(node);
                    NotifyPropertyChanged("RootNodes");
                }

                node.PropertyChanged += this.propertyChangedHandler;

                if (NodeAdded != null)
                    NodeAdded(node);
            }
        }
        #endregion Method: AddNode(Node node)

        #region Method: RemoveNode(Node node)
        /// <summary>
        /// Removes a node.
        /// </summary>
        /// <param name="node">The node to remove.</param>
        public void RemoveNode(Node node)
        {
            if (node != null)
            {
                this.nodes.Remove(node);
                NotifyPropertyChanged("Nodes");

                if (node.IsRoot)
                {
                    this.rootNodes.Remove(node);
                    NotifyPropertyChanged("RootNodes");
                }

                node.PropertyChanged -= this.propertyChangedHandler;

                if (NodeRemoved != null)
                    NodeRemoved(node);
            }
        }
        #endregion Method: RemoveNode(Node node)

        #region Method: GetNodes()
        /// <summary>
        /// Get all the nodes of this model.
        /// </summary>
        /// <returns>A read-only collection of all nodes.</returns>
        public ReadOnlyCollection<Node> GetNodes()
        {
            List<Node> nodeList = new List<Node>();
            foreach (Node node in nodes)
                nodeList.Add(node);
            return nodeList.AsReadOnly();
        }
        #endregion Method: GetNodes()

        #region Method: GetRootNodes()
        /// <summary>
        /// Get all the root nodes of this model.
        /// </summary>
        /// <returns>A read-only collection of all root nodes.</returns>
        public ReadOnlyCollection<Node> GetRootNodes()
        {
            List<Node> nodeList = new List<Node>();
            foreach (Node node in rootNodes)
                nodeList.Add(node);
            return nodeList.AsReadOnly();
        }
        #endregion Method: GetRootNodes()

        #region Method: Sort()
        /// <summary>
        /// Sort the nodes.
        /// </summary>
        public void Sort()
        {
            List<Node> allNodes = new List<Node>();
            allNodes.AddRange(this.nodes);
            allNodes.Sort();
            this.nodes.Clear();
            foreach (Node allNode in allNodes)
                this.nodes.Add(allNode);
        }
        #endregion Method: Sort()
		
        #region Method: Clear()
        /// <summary>
        /// Clears the node model.
        /// </summary>
        internal void Clear()
        {
            if (this.nodes != null)
                this.nodes.Clear();
            if (this.rootNodes != null)
                this.rootNodes.Clear();
        }
        #endregion Method: Clear()

        #region Method: node_PropertyChanged(object sender, PropertyChangedEventArgs e)
        /// <summary>
        /// Update the root nodes when a parent changes.
        /// </summary>
        private void node_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("PersonalParents"))
            {
                Node node = sender as Node;
                if (node != null)
                {
                    if (node.IsRoot)
                    {
                        if (!this.rootNodes.Contains(node))
                        {
                            this.rootNodes.Add(node);
                            NotifyPropertyChanged("RootNodes");
                        }
                    }
                    else
                    {
                        if (this.rootNodes.Contains(node))
                        {
                            this.rootNodes.Remove(node);
                            NotifyPropertyChanged("RootNodes");
                        }
                    }
                }

            }
        }
        #endregion Method: node_PropertyChanged(object sender, PropertyChangedEventArgs e)

    }
    #endregion Class: NodeModel

}