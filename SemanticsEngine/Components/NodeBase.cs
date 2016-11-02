/**************************************************************************
 * 
 * NodeBase.cs
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
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: NodeBase
    /// <summary>
    /// A base of a node.
    /// </summary>
    public abstract class NodeBase : Base
    {

        #region Properties and Fields

        #region Property: Node
        /// <summary>
        /// The node of which this is a node base.
        /// </summary>
        private Node node = null;

        /// <summary>
        /// Gets the node of which this is a node base.
        /// </summary>
        protected internal Node Node
        {
            get
            {
                return node;
            }
        }
        #endregion Property: Node

        #region Property: Name
        /// <summary>
        /// The default name of the base.
        /// </summary>
        private String defaultName = null;

        /// <summary>
        /// Gets the default name of the base.
        /// </summary>
        public String DefaultName
        {
            get
            {
                return defaultName;
            }
        }
        #endregion Property: Name

        #region Property: Names
        /// <summary>
        /// All the names of the node.
        /// </summary>
        private String[] names = null;

        /// <summary>
        /// Gets all the names of the node.
        /// </summary>
        public ReadOnlyCollection<String> Names
        {
            get
            {
                if (names == null)
                {
                    LoadNames();
                    if (names == null)
                        return new List<String>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<String>(names);
            }
        }

        /// <summary>
        /// Loads the names.
        /// </summary>
        private void LoadNames()
        {
            if (this.Node != null)
            {
                List<String> nodeNames = new List<String>();
                foreach (String nodeName in this.Node.Names)
                    nodeNames.Add(nodeName);
                names = nodeNames.ToArray();
            }
        }
        #endregion Property: Names

        #region Property: Description
        /// <summary>
        /// The description of the base.
        /// </summary>
        private String description = null;

        /// <summary>
        /// Gets the description of the base.
        /// </summary>
        public String Description
        {
            get
            {
                if (description == null)
                    LoadDescription();
                return description;
            }
        }

        /// <summary>
        /// Loads the description.
        /// </summary>
        private void LoadDescription()
        {
            if (this.Node != null)
                this.description = this.Node.Description;
        }
        #endregion Property: Description

        #region Property: Parents
        /// <summary>
        /// Gets the personal and inherited parents of the node.
        /// </summary>
        public IEnumerable<NodeBase> Parents
        {
            get
            {
                foreach (NodeBase personalParent in this.PersonalParents)
                {
                    yield return personalParent;

                    foreach (NodeBase inheritedParent in personalParent.Parents)
                        yield return inheritedParent;
                }
            }
        }
        #endregion Property: Parents

        #region Property: PersonalParents
        /// <summary>
        /// The personal parents of the node.
        /// </summary>
        private NodeBase[] personalParents = null;

        /// <summary>
        /// Gets the personal parents of the node.
        /// </summary>
        public IEnumerable<NodeBase> PersonalParents
        {
            get
            {
                if (personalParents == null)
                    LoadPersonalParents();
                foreach (NodeBase personalParent in personalParents)
                    yield return personalParent;
            }
        }

        /// <summary>
        /// Loads the personal parents.
        /// </summary>
        private void LoadPersonalParents()
        {
            if (this.Node != null)
            {
                List<NodeBase> nodeBases = new List<NodeBase>();
                foreach (Node parent in this.Node.PersonalParents)
                    nodeBases.Add(BaseManager.Current.GetBase<NodeBase>(parent));
                personalParents = nodeBases.ToArray();
            }
            else
                personalParents = new NodeBase[] { }; 
        }
        #endregion Property: PersonalParents

        #region Property: InheritedParents
        /// <summary>
        /// Gets the inherited parents of the node.
        /// </summary>
        public IEnumerable<NodeBase> InheritedParents
        {
            get
            {
                foreach (NodeBase parent in this.PersonalParents)
                {
                    foreach (NodeBase inheritedParent in parent.Parents)
                        yield return inheritedParent;
                }
            }
        }
        #endregion Property: InheritedParents

        #region Property: Children
        /// <summary>
        /// Gets the personal and inherited children of the node.
        /// </summary>
        public IEnumerable<NodeBase> Children
        {
            get
            {
                foreach (NodeBase personalChild in this.PersonalChildren)
                {
                    yield return personalChild;

                    foreach (NodeBase inheritedChild in personalChild.Children)
                        yield return inheritedChild;
                }
            }
        }
        #endregion Property: Children

        #region Property: PersonalChildren
        /// <summary>
        /// The personal children of the node.
        /// </summary>
        private NodeBase[] personalChildren = null;

        /// <summary>
        /// Gets the personal children of the node.
        /// </summary>
        public IEnumerable<NodeBase> PersonalChildren
        {
            get
            {
                if (personalChildren == null)
                    LoadPersonalChildren();
                foreach (NodeBase personalChild in personalChildren)
                    yield return personalChild;
            }
        }

        /// <summary>
        /// Loads the personal children.
        /// </summary>
        private void LoadPersonalChildren()
        {
            if (this.Node != null)
            {
                List<NodeBase> nodeBases = new List<NodeBase>();
                foreach (Node child in this.Node.PersonalChildren)
                    nodeBases.Add(BaseManager.Current.GetBase<NodeBase>(child));
                personalChildren = nodeBases.ToArray();
            }
        }
        #endregion Property: PersonalChildren

        #region Property: InheritedChildren
        /// <summary>
        /// Gets the inherited children of the node.
        /// </summary>
        public IEnumerable<NodeBase> InheritedChildren
        {
            get
            {
                foreach (NodeBase child in this.PersonalChildren)
                {
                    foreach (NodeBase inheritedChild in child.Children)
                        yield return inheritedChild;
                }
            }
        }
        #endregion Property: InheritedChildren

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: NodeBase(Node node)
        /// <summary>
        /// Creates a new node base from the given node.
        /// </summary>
        /// <param name="node">The node to create a node base from.</param>
        protected NodeBase(Node node)
            : base(node)
        {
            if (node != null)
            {
                this.node = node;
                this.defaultName = this.Node.DefaultName;

                if (BaseManager.PreloadProperties)
                {
                    LoadNames();
                    LoadDescription();
                    LoadPersonalParents();
                    LoadPersonalChildren();
                }
            }
        }
        #endregion Constructor: NodeBase(Node node)

        #region Constructor: NodeBase(NodeValued nodeValued)
        /// <summary>
        /// Creates a new node base from the given valued node.
        /// </summary>
        /// <param name="nodeValued">The valued node to create a node base from.</param>
        protected NodeBase(NodeValued nodeValued)
            : this(nodeValued.Node)
        {
        }
        #endregion Constructor: NodeBase(NodeValued nodeValued)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: HasName(String name)
        /// <summary>
        /// Checks whether this node base has the given name.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns>Returns whether this node base has the given name.</returns>
        public bool HasName(String name)
        {
            return this.Names.Contains(name);
        }
        #endregion Method: HasName(String name)

        #region Method: HasParent(NodeBase parent)
        /// <summary>
        /// Checks whether this node base has the given parent as one of its parents.
        /// </summary>
        /// <param name="parent">The parent to check.</param>
        /// <returns>Returns whether this node base has the given parent as one of its parents.</returns>
        protected internal bool HasParent(NodeBase parent)
        {
            foreach (NodeBase p in this.Parents)
            {
                if (p.ID == parent.ID)
                    return true;
            }

            return false;
        }
        #endregion Method: HasParent(NodeBase parent)
        
        #region Method: HasChild(NodeBase child)
        /// <summary>
        /// Checks whether this node base has the given child as one of its children.
        /// </summary>
        /// <param name="child">The child to check.</param>
        /// <returns>Returns whether this node base has the given child as one of its children.</returns>
        protected internal bool HasChild(NodeBase child)
        {
            foreach (NodeBase c in this.Children)
            {
                if (c.ID == child.ID)
                    return true;
            }

            return false;
        }
        #endregion Method: HasChild(NodeBase child)

        #region Method: IsNodeOf(NodeBase nodeBase)
        /// <summary>
        /// Indicates whether this node base is of the given node base.
        /// </summary>
        /// <param name="nodeBase">The node base to check.</param>
        /// <returns>Returns whether this node base is of the given node base.</returns>
        public bool IsNodeOf(NodeBase nodeBase)
        {
            if (nodeBase != null)
            {
                if (nodeBase.ID == this.ID || HasParent(nodeBase))
                    return true;
            }
            return false;
        }
        #endregion Method: IsNodeOf(NodeBase nodeBase)

        #region Method: IsNodeOfDepth(NodeBase nodeBase)
        /// <summary>
        /// Indicates whether this node base is of the given node base, and how many nodes/levels/parents deep.
        /// </summary>
        /// <param name="nodeBase">The node base to check.</param>
        /// <returns>Returns the depth: -1 is this node base is not of the given node base; 0 when they are the same, or a positive value for the number of parents.</returns>
        public int IsNodeOfDepth(NodeBase nodeBase)
        {
            if (nodeBase != null)
                return GetDepth(nodeBase, 0);
            return -1;
        }

        /// <summary>
        /// Get the depth of the given node base, compared to this one.
        /// </summary>
        /// <param name="nodeBase">The node base to check.</param>
        /// <param name="depth">The depth: the difference in level between this node and the given one.</param>
        /// <returns>The depth.</returns>
        private int GetDepth(NodeBase nodeBase, int depth)
        {
            if (nodeBase != null)
            {
                if (nodeBase.ID == this.ID)
                    return depth;
                else
                {
                    if (new List<NodeBase>(this.PersonalParents).Count == 0)
                        return -1;

                    depth++;

                    foreach (NodeBase parent in this.PersonalParents)
                    {
                        depth = parent.GetDepth(nodeBase, depth);
                        if (depth != -1)
                            return depth;
                    }
                }
            }
            return depth;
        }
        #endregion Method: IsNodeOfDepth(NodeBase nodeBase)

        #region Method: IsNodeOf(Node node)
        /// <summary>
        /// Indicates whether this node base is of the given node.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <returns>Returns whether this node base is of the given node.</returns>
        public bool IsNodeOf(Node node)
        {
            if (node != null)
                return IsNodeOf(BaseManager.Current.GetBase<NodeBase>(node));
            return false;
        }
        #endregion Method: IsNodeOf(Node node)

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
    #endregion Class: NodeBase

    #region Class: NodeValuedBase
    /// <summary>
    /// A base of a valued node.
    /// </summary>
    public abstract class NodeValuedBase : Base
    {

        #region Properties and Fields

        #region Property: NodeValued
        /// <summary>
        /// The valued node of which this is a node base.
        /// </summary>
        private NodeValued nodeValued = null;

        /// <summary>
        /// Gets the valued node of which this is a node base.
        /// </summary>
        protected internal NodeValued NodeValued
        {
            get
            {
                return nodeValued;
            }
        }
        #endregion Property: NodeValued

        #region Property: NodeBase
        /// <summary>
        /// The node of which this is a valued node base.
        /// </summary>
        private NodeBase nodeBase = null;

        /// <summary>
        /// Gets the node of which this is a valued node base.
        /// </summary>
        public NodeBase NodeBase
        {
            get
            {
                if (nodeBase == null && this.NodeValued != null)
                    nodeBase = BaseManager.Current.GetBase<NodeBase>(this.NodeValued.Node);
                return nodeBase;
            }
        }
        #endregion Property: NodeBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: NodeValuedBase(NodeValued nodeValued)
        /// <summary>
        /// Creates a new valued node base from the given valued node.
        /// </summary>
        /// <param name="nodeValued">The valued node to create a valued node base from.</param>
        protected NodeValuedBase(NodeValued nodeValued)
            : base(nodeValued)
        {
            this.nodeValued = nodeValued;
        }
        #endregion Constructor: NodeValuedBase(NodeValued nodeValued)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            if (this.NodeBase != null)
                return this.NodeBase.DefaultName;
            return base.ToString();
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: NodeValuedBase

    #region Class: NodeConditionBase
    /// <summary>
    /// A condition on a node.
    /// </summary>
    public abstract class NodeConditionBase : ConditionBase
    {

        #region Properties and Fields

        #region Property: NodeCondition
        /// <summary>
        /// Gets the node condition of which this is a node condition base.
        /// </summary>
        protected internal NodeCondition NodeCondition
        {
            get
            {
                return this.Condition as NodeCondition;
            }
        }
        #endregion Property: NodeCondition

        #region Property: NodeBase
        /// <summary>
        /// The required node.
        /// </summary>
        private NodeBase node = null;
        
        /// <summary>
        /// Gets the required node.
        /// </summary>
        public NodeBase NodeBase
        {
            get
            {
                return node;
            }
        }
        #endregion Property: NodeBase

        #region Property: NodeSign
        /// <summary>
        /// The equality sign of the node in the condition.
        /// </summary>
        private EqualitySign nodeSign = default(EqualitySign);

        /// <summary>
        /// Gets the equality sign of the node in the condition.
        /// </summary>
        public EqualitySign NodeSign
        {
            get
            {
                return nodeSign;
            }
        }
        #endregion Property: NodeSign

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: NodeConditionBase(NodeCondition nodeCondition)
        /// <summary>
        /// Creates a base of the given node condition.
        /// </summary>
        /// <param name="nodeCondition">The node condition to create a base of.</param>
        protected NodeConditionBase(NodeCondition nodeCondition)
            : base(nodeCondition)
        {
            if (nodeCondition != null)
            {
                this.node = BaseManager.Current.GetBase<NodeBase>(nodeCondition.Node);
                this.nodeSign = nodeCondition.NodeSign;
            }
        }
        #endregion Constructor: NodeConditionBase(NodeCondition nodeCondition)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            if (this.NodeBase != null)
                return this.NodeBase.DefaultName;
            return base.ToString();
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: NodeConditionBase

    #region Class: NodeChangeBase
    /// <summary>
    /// A change on a node.
    /// </summary>
    public abstract class NodeChangeBase : ChangeBase
    {

        #region Properties and Fields

        #region Property: NodeChange
        /// <summary>
        /// Gets the node change of which this is a node change base.
        /// </summary>
        protected internal NodeChange NodeChange
        {
            get
            {
                return this.Change as NodeChange;
            }
        }
        #endregion Property: NodeChange

        #region Property: NodeBase
        /// <summary>
        /// The affected node base.
        /// </summary>
        private NodeBase node = null;
        
        /// <summary>
        /// Gets the affected node base.
        /// </summary>
        public NodeBase NodeBase
        {
            get
            {
                return node;
            }
        }
        #endregion Property: NodeBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: NodeChangeBase(NodeChange nodeChange)
        /// <summary>
        /// Creates a base from the given node change.
        /// </summary>
        /// <param name="nodeChange">The node change to create a base from.</param>
        protected NodeChangeBase(NodeChange nodeChange)
            : base(nodeChange)
        {
            if (nodeChange != null)
                this.node = BaseManager.Current.GetBase<NodeBase>(nodeChange.Node);
        }
        #endregion Constructor: NodeChangeBase(NodeChange nodeChange)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            if (this.NodeBase != null)
                return this.NodeBase.DefaultName;
            return base.ToString();
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: NodeChangeBase

}