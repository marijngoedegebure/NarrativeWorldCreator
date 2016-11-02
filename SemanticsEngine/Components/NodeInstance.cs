/**************************************************************************
 * 
 * NodeInstance.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Semantics.Components;
using Semantics.Utilities;
using SemanticsEngine.Interfaces;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{
    
    #region Class: NodeInstance
    /// <summary>
    /// An instance of a node.
    /// </summary>
    public abstract class NodeInstance : Instance
    {

        #region Properties and Fields

        #region Property: NodeBase
        /// <summary>
        /// The node base of which this is a node instance.
        /// </summary>
        private NodeBase nodeBase = null;

        /// <summary>
        /// Gets the node base of which this is a node instance.
        /// </summary>
        public NodeBase NodeBase
        {
            get
            {
                if (nodeBase == null)
                {
                    NodeBase nodeBase2 = this.Base as NodeBase;
                    if (nodeBase2 != null)
                        nodeBase = nodeBase2;

                    NodeValuedBase nodeValuedBase = this.Base as NodeValuedBase;
                    if (nodeValuedBase != null)
                        nodeBase = nodeValuedBase.NodeBase;
                }

                return nodeBase;
            }
        }
        #endregion Property: NodeBase

        #region Property: DefaultName
        /// <summary>
        /// Gets or sets the default name of the instance.
        /// </summary>
        public String DefaultName
        {
            get
            {
                String name = GetProperty<String>("DefaultName", null);
                if (name != null)
                    return name;
                
                if (this.NodeBase != null)
                    return this.NodeBase.DefaultName;
                
                return String.Empty;
            }
            set
            {
                SetProperty("DefaultName", value);
            }
        }
        #endregion Property: DefaultName

        #region Property: Name
        /// <summary>
        /// Gets or sets the default name of the instance.
        /// </summary>
        public String Name
        {
            get
            {
                return this.DefaultName;
            }
            set
            {
                this.DefaultName = value;
                NotifyPropertyChanged("Name");
            }
        }
        #endregion Property: Name

        #region Property: Names
        /// <summary>
        /// Gets all the names of the instance.
        /// </summary>
        public ReadOnlyCollection<String> Names
        {
            get
            {
                if (this.NodeBase != null)
                    return this.NodeBase.Names;
                return new List<String>(0).AsReadOnly();
            }
        }
        #endregion Property: Names

        #region Property: Description
        /// <summary>
        /// Gets the description of the instance.
        /// </summary>
        public String Description
        {
            get
            {
                String description = GetProperty<String>("Description", null);
                if (description != null)
                    return description;

                if (this.NodeBase != null)
                    return this.NodeBase.Description;

                return String.Empty;
            }
            set
            {
                SetProperty("Description", value);
            }
        }
        #endregion Property: Description

        #region Property: Interfaces
        /// <summary>
        /// The interfaces of the instance.
        /// </summary>
        private INodeInstance[] interfaces = null;

        /// <summary>
        /// Gets the interfaces of the instance.
        /// </summary>
        public ReadOnlyCollection<INodeInstance> Interfaces
        {
            get
            {
                if (interfaces != null)
                    return new ReadOnlyCollection<INodeInstance>(interfaces);

                return new List<INodeInstance>(0).AsReadOnly();
            }
        }
        #endregion Property: Interfaces

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: NodeInstance(NodeBase nodeBase)
        /// <summary>
        /// Creates a new node instance from the given node base.
        /// </summary>
        /// <param name="nodeBase">The node base to create the node instance from.</param>
        protected NodeInstance(NodeBase nodeBase)
            : base(nodeBase)
        {
        }
        #endregion Constructor: NodeInstance(NodeBase nodeBase)

        #region Constructor: NodeInstance(NodeValuedBase nodeValuedBase)
        /// <summary>
        /// Creates a new node instance from the given valued node base.
        /// </summary>
        /// <param name="nodeValuedBase">The valued node base to create the node instance from.</param>
        protected NodeInstance(NodeValuedBase nodeValuedBase)
            : base(nodeValuedBase)
        {
        }
        #endregion Constructor: NodeInstance(NodeValuedBase nodeValuedBase)

        #region Constructor: NodeInstance(NodeInstance nodeInstance)
        /// <summary>
        /// Clones a node instance.
        /// </summary>
        /// <param name="nodeInstance">The node instance to clone.</param>
        protected NodeInstance(NodeInstance nodeInstance)
            : base(nodeInstance)
        {
            if (nodeInstance != null)
            {
            }
        }
        #endregion Constructor: NodeInstance(NodeInstance nodeInstance)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddInterface(INodeInstance nodeInstanceInterface)
        /// <summary>
        /// Adds an interface.
        /// </summary>
        /// <param name="nodeInstanceInterface">The interface to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddInterface(INodeInstance nodeInstanceInterface)
        {
            if (nodeInstanceInterface != null)
            {
                // If the interface is already available in all interfaces, there is no use to add it
                if (this.Interfaces.Contains(nodeInstanceInterface))
                    return Message.RelationExistsAlready;

                // Add the interface
                Utils.AddToArray<INodeInstance>(ref this.interfaces, nodeInstanceInterface);
                NotifyPropertyChanged("Interfaces");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddInterface(INodeInstance nodeInstanceInterface)

        #region Method: RemoveInterface(INodeInstance nodeInstanceInterface)
        /// <summary>
        /// Removes an interface.
        /// </summary>
        /// <param name="nodeInstanceInterface">The interface to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveInterface(INodeInstance nodeInstanceInterface)
        {
            if (nodeInstanceInterface != null)
            {
                if (this.Interfaces.Contains(nodeInstanceInterface))
                {
                    // Remove the interface
                    Utils.RemoveFromArray<INodeInstance>(ref this.interfaces, nodeInstanceInterface);
                    NotifyPropertyChanged("Interfaces");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveInterface(INodeInstance nodeInstanceInterface)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasName(String name)
        /// <summary>
        /// Checks whether this node instance has the given name.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns>Returns whether this node instance has the given name.</returns>
        public bool HasName(String name)
        {
            return this.Names.Contains(name);
        }
        #endregion Method: HasName(String name)

        #region Method: IsNodeOf(NodeBase nodeBase)
        /// <summary>
        /// Indicates whether this node instance is of the given node base.
        /// </summary>
        /// <param name="nodeBase">The node base to check.</param>
        /// <returns>Returns whether this node instance is of the given node base.</returns>
        public bool IsNodeOf(NodeBase nodeBase)
        {
            if (this.NodeBase != null)
                return this.NodeBase.IsNodeOf(nodeBase);
            return false;
        }
        #endregion Method: IsNodeOf(NodeBase node)
        
        #region Method: IsNodeOf(Node node)
        /// <summary>
        /// Indicates whether this node instance is of the given node.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <returns>Returns whether this node instance is of the given node.</returns>
        public bool IsNodeOf(Node node)
        {
            if (node != null && this.NodeBase != null)
                return this.NodeBase.IsNodeOf(BaseManager.Current.GetBase<NodeBase>(node));
            return false;
        }
        #endregion Method: IsNodeOf(Node node)

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the given condition satisfies the node instance.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the node instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the condition satisfies the node instance.</returns>
        public virtual bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (conditionBase != null)
            {
                NodeConditionBase nodeConditionBase = conditionBase as NodeConditionBase;
                if (nodeConditionBase != null)
                {
                    return nodeConditionBase.NodeBase == null ||
                          (nodeConditionBase.NodeSign == EqualitySign.Equal && this.NodeBase.IsNodeOf(nodeConditionBase.NodeBase)) ||
                          (nodeConditionBase.NodeSign == EqualitySign.NotEqual && !this.NodeBase.IsNodeOf(nodeConditionBase.NodeBase));
                }
            }
            return false;
        }
        #endregion Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the given change to the node instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the node instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        protected internal virtual bool Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (changeBase != null)
            {
                NodeChangeBase nodeChangeBase = changeBase as NodeChangeBase;
                if (nodeChangeBase != null)
                {
                    if (nodeChangeBase.NodeBase == null || IsNodeOf(nodeChangeBase.NodeBase))
                        return true;
                }
            }
            
            return false;
        }
        #endregion Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return this.DefaultName;
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: NodeInstance

}