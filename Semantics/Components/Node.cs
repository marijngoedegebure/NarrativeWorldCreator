/**************************************************************************
 * 
 * Node.cs
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
using System.Reflection;
using Common;
using Semantics.Data;
using Semantics.Interfaces;
using Semantics.Utilities;

namespace Semantics.Components
{

    #region Class: Node
    /// <summary>
    /// A node with names, a description, and tags.
    /// </summary>
    public abstract class Node : IdHolder, IComparable<Node>
    {

        #region Properties and Fields

        #region Property: Names
        /// <summary>
        /// The names of this node.
        /// </summary>
        internal List<String> names = null;

        /// <summary>
        /// Gets the names of this node.
        /// </summary>
        public ReadOnlyCollection<String> Names
        {
            get
            {
                if (names == null)
                    names = Database.Current.SelectAll<String>(this.ID, LocalizationTables.NodeName, Columns.Name);
                return names.AsReadOnly();
            }
        }
        #endregion Property: Names

        #region Property: DefaultName
        /// <summary>
        /// The default name of this node.
        /// </summary>
        internal String defaultName = null;

        /// <summary>
        /// Gets or sets the default name of this node. Setting only works when the node has the name.
        /// </summary>
        public String DefaultName
        {
            get
            {
                if (defaultName == null)
                    defaultName = Database.Current.Select<String>(this.ID, LocalizationTables.NodeName, Columns.Name, Columns.IsDefaultName, true);
                return defaultName;
            }
            set
            {
                if (HasName(value))
                {
                    // Reset the current default name and set the new one
                    Database.Current.Update(this.ID, LocalizationTables.NodeName, Columns.IsDefaultName, false, Columns.IsDefaultName, true);
                    Database.Current.Update(this.ID, LocalizationTables.NodeName, Columns.IsDefaultName, true, Columns.Name, value);

                    this.defaultName = null;
                    NotifyPropertyChanged("DefaultName");
                }
            }
        }
        #endregion Property: DefaultName

        #region Property: Description
        /// <summary>
        /// Gets or sets the description of the node.
        /// </summary>
        public String Description
        {
            get
            {
                return Database.Current.Select<String>(this.ID, LocalizationTables.NodeDescription, Columns.Description);
            }
            set
            {
                Database.Current.Update(this.ID, LocalizationTables.NodeDescription, Columns.Description, value);
                NotifyPropertyChanged("Description");
            }
        }
        #endregion Property: Description

        #region Property: Metadata
        /// <summary>
        /// Gets the metadata of the node.
        /// </summary>
        public Metadata Metadata
        {
            get
            {
                return Database.Current.Select<Metadata>(this.ID, GenericTables.Node, Columns.Metadata);
            }
            private set
            {
                Database.Current.Update(this.ID, GenericTables.Node, Columns.Metadata, value);
                NotifyPropertyChanged("Metadata");
            }
        }
        #endregion Property: Metadata

        #region Property: Tags
        /// <summary>
        /// The tags of this node.
        /// </summary>
        private List<String> tags = null;

        /// <summary>
        /// Gets the tags of this node.
        /// </summary>
        public ReadOnlyCollection<String> Tags
        {
            get
            {
                if (tags == null)
                    tags = Database.Current.SelectAll<String>(this.ID, LocalizationTables.NodeTag, Columns.Tag);
                return tags.AsReadOnly();
            }
        }
        #endregion Property: Tags

        #region Property: Parents
        /// <summary>
        /// Gets the personal and inherited parents of the node.
        /// </summary>
        public ReadOnlyCollection<Node> Parents
        {
            get
            {
                List<Node> parents = new List<Node>();
                parents.AddRange(this.PersonalParents);
                parents.AddRange(this.InheritedParents);
                return parents.AsReadOnly();
            }
        }
        #endregion Property: Parents

        #region Property: PersonalParents
        /// <summary>
        /// The personal parents of the node.
        /// </summary>
        private List<Node> personalParents = null;
        
        /// <summary>
        /// Gets the personal parents of the node.
        /// </summary>
        public ReadOnlyCollection<Node> PersonalParents
        {
            get
            {
                if (personalParents == null)
                    personalParents = Database.Current.SelectAll<Node>(this.ID, GenericTables.NodeParent, Columns.Parent);
                return personalParents.AsReadOnly();
            }
        }
        #endregion Property: PersonalParents

        #region Property: InheritedParents
        /// <summary>
        /// Gets the inherited parents of the node.
        /// </summary>
        public ReadOnlyCollection<Node> InheritedParents
        {
            get
            {
                List<Node> inheritedParents = new List<Node>();
                foreach (Node parent in this.PersonalParents)
                    inheritedParents.AddRange(parent.Parents);
                return inheritedParents.AsReadOnly();
            }
        }
        #endregion Property: InheritedParents

        #region Property: Children
        /// <summary>
        /// Gets the personal and inherited children of the node.
        /// </summary>
        public ReadOnlyCollection<Node> Children
        {
            get
            {
                List<Node> children = new List<Node>();
                children.AddRange(this.PersonalChildren);
                children.AddRange(this.InheritedChildren);
                return children.AsReadOnly();
            }
        }
        #endregion Property: Children

        #region Property: PersonalChildren
        /// <summary>
        /// The personal children of the node.
        /// </summary>
        private List<Node> personalChildren = null;

        /// <summary>
        /// Gets the personal children of the node.
        /// </summary>
        public ReadOnlyCollection<Node> PersonalChildren
        {
            get
            {
                if (personalChildren == null)
                    personalChildren = Database.Current.SelectAll<Node>(GenericTables.NodeParent, Columns.Parent, this.ID);
                return personalChildren.AsReadOnly();
            }
        }
        #endregion Property: PersonalChildren

        #region Property: InheritedChildren
        /// <summary>
        /// Gets the inherited children of the node.
        /// </summary>
        public ReadOnlyCollection<Node> InheritedChildren
        {
            get
            {
                List<Node> inheritedChildren = new List<Node>();
                foreach (Node child in this.PersonalChildren)
                    inheritedChildren.AddRange(child.Children);
                return inheritedChildren.AsReadOnly();
            }
        }
        #endregion Property: InheritedChildren

        #region Property: IsLeaf
        /// <summary>
        /// Returns whether this node is a leaf (has no children).
        /// </summary>
        private bool? isLeaf = null;
        
        /// <summary>
        /// Gets the value that indicates whether this node is a leaf (has no children).
        /// </summary>
        public bool IsLeaf
        {
            get
            {
                if (isLeaf == null)
                    isLeaf = Database.Current.Count(this.ID, GenericTables.NodeParent, Columns.Parent, false) == 0;
                return (bool)isLeaf;
            }
        }
        #endregion Property: IsLeaf

        #region Property: IsRoot
        /// <summary>
        /// Returns whether this node is a root (has no parents).
        /// </summary>
        private bool? isRoot = null;
        
        /// <summary>
        /// Gets the value that indicates whether this node is a root (has no parents).
        /// </summary>
        public bool IsRoot
        {
            get
            {
                if (isRoot == null)
                    isRoot = Database.Current.Count(this.ID, GenericTables.NodeParent, false) == 0;
                return (bool)isRoot;
            }
        }
        #endregion Property: IsRoot

        #region Property: Extensions
        /// <summary>
        /// All the extensions of the node.
        /// </summary>
        private Dictionary<Type, INodeExtension> extensions = new Dictionary<Type, INodeExtension>();

        /// <summary>
        /// Gets all the extensions of the node.
        /// </summary>
        public ReadOnlyCollection<INodeExtension> Extensions
        {
            get
            {
                return new List<INodeExtension>(extensions.Values).AsReadOnly();
            }
        }
        #endregion Property: Extensions

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: Node()
        /// <summary>
        /// Creates a new node.
        /// </summary>
        protected Node()
            : base()
        {
            Database.Current.StartChange();

            this.Metadata = new Metadata();

            Database.Current.StopChange();
        }
        #endregion Constructor: Node()

        #region Constructor: Node(uint id)
        /// <summary>
        /// Creates a new node from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new node from.</param>
        protected Node(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Node(uint id)

        #region Constructor: Node(string name)
        /// <summary>
        /// Creates a new node with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the node.</param>
        protected Node(string name)
            : base(name)
        {
            Database.Current.StartChange();

            this.Metadata = new Metadata();

            Database.Current.StopChange();
        }
        #endregion Constructor: Node(string name)

        #region Constructor: Node(Node node)
        /// <summary>
        /// Clones a node.
        /// </summary>
        /// <param name="node">The node to clone.</param>
        protected Node(Node node)
            : base()
        {
            if (node != null)
            {
                Database.Current.StartChange();

                foreach (String name in node.Names)
                    AddName(name);
                this.Description = node.Description;
                this.Metadata = new Metadata(node.Metadata);
                foreach (String tag in node.Tags)
                    AddTag(tag);
                foreach (Node parent in node.Parents)
                    AddChildParentRelation(this, parent);

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Node(Node node)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddName(String name)
        /// <summary>
        /// Adds the given name.
        /// </summary>
        /// <param name="name">The name to add.</param>
        /// <returns>Returns whether the name has been successfully added.</returns>
        public virtual Message AddName(String name)
        {
            if (name != null && !name.Equals(String.Empty))
            {
                // Check whether the name is already there
                if (this.Names.Contains(name))
                    return Message.RelationExistsAlready;

                // Remove the default name when it's still there
                bool isDefaultName = false;
                String newNodeName = Database.Current.GetNewNodeName(this.GetType().Name);
                for (int i = 0; i < this.Names.Count; i++)
                {
                    String existingName = this.Names[i];
                    if (existingName.Equals(newNodeName))
                    {
                        Database.Current.Remove(this.ID, LocalizationTables.NodeName, Columns.Name, existingName);
                        isDefaultName = true;
                    }
                }

                // Add the new name
                Database.Current.Insert(this.ID, LocalizationTables.NodeName, new string[] { Columns.Name, Columns.IsDefaultName }, new object[] { name, isDefaultName });
                this.names = null;
                if (isDefaultName)
                {
                    this.defaultName = null;
                    NotifyPropertyChanged("DefaultName");
                }
                NotifyPropertyChanged("Names");

                return Message.RelationSuccess;
            }

            return Message.RelationFail;
        }
        #endregion Method: AddName(String name)

        #region Method: ChangeName(String oldName, String newName)
        /// <summary>
        /// Changes the old name to the new name.
        /// </summary>
        /// <param name="oldName">The old name to be changed.</param>
        /// <param name="newName">The new name.</param>
        /// <returns>Returns whether the name has been successfully changed.</returns>
        public Message ChangeName(String oldName, String newName)
        {
            if (oldName != null && newName != null)
            {
                if (oldName.Equals(newName))
                    return Message.RelationExistsAlready;

                if (this.Names.Contains(oldName) && !this.Names.Contains(newName))
                {
                    Database.Current.Update(this.ID, LocalizationTables.NodeName, Columns.Name, oldName, newName);
                    this.defaultName = null;
                    this.names = null;
                    NotifyPropertyChanged("DefaultName");
                    NotifyPropertyChanged("Names");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: ChangeName(String oldName, String newName)

        #region Method: RemoveName(String name)
        /// <summary>
        /// Removes the given name from the node.
        /// </summary>
        /// <param name="name">The name to remove.</param>
        /// <returns>Returns whether the name has been successfully removed.</returns>
        public Message RemoveName(String name)
        {
            if (name != null)
            {
                if (this.Names.Contains(name))
                {
                    String defName = this.DefaultName;

                    // Remove the name, but make sure that at least one remains
                    if (this.Names.Count > 1)
                        Database.Current.Remove(this.ID, LocalizationTables.NodeName, Columns.Name, name);
                    else
                        Database.Current.Update(this.ID, LocalizationTables.NodeName, Columns.Name, Database.Current.GetNewNodeName(this.GetType().Name));

                    this.names = null;
                    NotifyPropertyChanged("Names");

                    // If the removed name is the same as the default name, pick a new one
                    if (name.Equals(defName))
                        this.DefaultName = this.Names[0];

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveName(String name)

        #region Method: AddTag(String tag)
        /// <summary>
        /// Adds the given tag.
        /// </summary>
        /// <param tag="tag">The tag to add.</param>
        /// <returns>Returns whether the tag has been successfully added.</returns>
        public Message AddTag(String tag)
        {
            if (tag != null)
            {
                // Check whether the tag is already there
                if (this.Tags.Contains(tag))
                    return Message.RelationExistsAlready;

                // Add the new tag
                Database.Current.Insert(this.ID, LocalizationTables.NodeTag, Columns.Tag, tag);
                this.tags = null;
                NotifyPropertyChanged("Tags");

                return Message.RelationSuccess;
            }

            return Message.RelationFail;
        }
        #endregion Method: AddTag(String tag)

        #region Method: ChangeTag(String oldTag, String newTag)
        /// <summary>
        /// Changes the old tag to the new tag.
        /// </summary>
        /// <param tag="oldTag">The old tag to be changed.</param>
        /// <param tag="newTag">The new tag.</param>
        /// <returns>Returns whether the tag has been successfully changed.</returns>
        public Message ChangeTag(String oldTag, String newTag)
        {
            if (oldTag != null && newTag != null)
            {
                if (oldTag.Equals(newTag))
                    return Message.RelationExistsAlready;

                if (this.Tags.Contains(oldTag) && !this.Tags.Contains(newTag))
                {
                    Database.Current.Update(this.ID, LocalizationTables.NodeTag, Columns.Tag, oldTag, newTag);
                    this.tags = null;
                    NotifyPropertyChanged("Tags");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: ChangeTag(String oldTag, String newTag)

        #region Method: RemoveTag(String tag)
        /// <summary>
        /// Removes the given tag from the node.
        /// </summary>
        /// <param tag="tag">The tag to remove.</param>
        /// <returns>Returns whether the tag has been successfully removed.</returns>
        public Message RemoveTag(String tag)
        {
            if (tag != null)
            {
                if (this.Tags.Contains(tag))
                {
                    // Remove the tag
                    Database.Current.Remove(this.ID, LocalizationTables.NodeTag, Columns.Tag, tag);
                    this.tags = null;
                    NotifyPropertyChanged("Tags");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveTag(String tag)

        #region Method: AddChild(Node child)
        /// <summary>
        /// Adds a child-parent relation between this node and the given child.
        /// </summary>
        /// <param name="child">The child node.</param>
        /// <returns>Returns whether the relation has been established successfully.</returns>
        public Message AddChild(Node child)
        {
            return AddChildParentRelation(child, this);
        }
        #endregion Method: AddChild(Node child)

        #region Method: RemoveChild(Node child)
        /// <summary>
        /// Removes the child-parent relation between this node and the given child.
        /// </summary>
        /// <param name="child">The child node.</param>
        /// <returns>Returns whether the relation has been removed successfully.</returns>
        public Message RemoveChild(Node child)
        {
            return RemoveChildParentRelation(child, this);
        }
        #endregion Method: RemoveChild(Node child)

        #region Method: AddParent(Node child)
        /// <summary>
        /// Adds a child-parent relation between this node and the given parent.
        /// </summary>
        /// <param name="parent">The parent node.</param>
        /// <returns>Returns whether the relation has been established successfully.</returns>
        public Message AddParent(Node parent)
        {
            return AddChildParentRelation(this, parent);
        }
        #endregion Method: AddParent(Node parent)

        #region Method: RemoveParent(Node parent)
        /// <summary>
        /// Removes the child-parent relation between this node and the given parent.
        /// </summary>
        /// <param name="parent">The parent node.</param>
        /// <returns>Returns whether the relation has been removed successfully.</returns>
        public Message RemoveParent(Node parent)
        {
            return RemoveChildParentRelation(this, parent);
        }
        #endregion Method: RemoveParent(Node parent)

        #region Method: AddChildParentRelation(Node child, Node parent)
        /// <summary>
        /// Creates a child parent relation.
        /// </summary>
        /// <param name="child">The child node.</param>
        /// <param name="parent">The parent node.</param>
        /// <returns>Returns whether the relation has been established successfully.</returns>
        public static Message AddChildParentRelation(Node child, Node parent)
        {
            return AddChildParentRelation(child, parent, false);
        }

        /// <summary>
        /// Creates a child parent relation.
        /// </summary>
        /// <param name="child">The child node.</param>
        /// <param name="parent">The parent node.</param>
        /// <param name="ignoreTypeCheck">Indicates whether a type check should be ignored.</param>
        /// <returns>Returns whether the relation has been established successfully.</returns>
        protected static Message AddChildParentRelation(Node child, Node parent, bool ignoreTypeCheck)
        {
            if (child != null && parent != null)
            {
                // Check if the child and parent are of the same type, and whether they are not the same
                if (ignoreTypeCheck || (child.GetType().Equals(parent.GetType()) && !child.Equals(parent)))
                {
                    // Check if the child and parent relation does not exist already
                    if (!child.HasParent(parent) && !parent.HasChild(child) && !child.HasChild(parent) && !parent.HasParent(child))
                    {
                        // Add the parent to the child
                        Database.Current.Insert(child.ID, GenericTables.NodeParent, Columns.Parent, parent);
                        child.personalParents = null;
                        child.isRoot = null;
                        parent.personalChildren = null;
                        parent.isLeaf = null;

                        child.NotifyPropertyChanged("PersonalParents");
                        child.NotifyPropertyChanged("Parents");
                        parent.NotifyPropertyChanged("PersonalChildren");
                        parent.NotifyPropertyChanged("Children");

                        return Message.RelationSuccess;
                    }
                    return Message.RelationExistsAlready;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: AddChildParentRelation(Node child, Node parent)

        #region Method: RemoveChildParentRelation(Node child, Node parent)
        /// <summary>
        /// Removes a child parent relation.
        /// </summary>
        /// <param name="child">The child node.</param>
        /// <param name="parent">The parent node.</param>
        /// <returns>Returns whether the relation has been removed successfully.</returns>
        public static Message RemoveChildParentRelation(Node child, Node parent)
        {
            if (child != null && parent != null && parent.HasChild(child))
            {
                // Remove the parent from the child
                Database.Current.Remove(child.ID, GenericTables.NodeParent, Columns.Parent, parent);
                child.personalParents = null;
                child.isRoot = null;
                parent.personalChildren = null;
                parent.isLeaf = null;

                // Add the parents of parent as the parents of the child
                foreach (Node node in parent.PersonalParents)
                    AddChildParentRelation(child, node);

                child.NotifyPropertyChanged("PersonalParents");
                child.NotifyPropertyChanged("Parents");
                parent.NotifyPropertyChanged("PersonalChildren");
                parent.NotifyPropertyChanged("Children");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveChildParentRelation(Node child, Node parent)

        #region Method: AddExtension(Type type, INodeExtension extension)
        /// <summary>
        /// Add a new extension.
        /// </summary>
        /// <param name="type">The type of the extension.</param>
        /// <param name="extension">The extension to add.</param>
        public Message AddExtension(Type type, INodeExtension extension)
        {
            if (extension != null)
            {
                if (this.extensions.ContainsKey(type))
                    return Message.RelationExistsAlready;

                this.extensions.Add(type, extension);
                NotifyPropertyChanged("Extensions");

                return Message.RelationSuccess;
            }

            return Message.RelationFail;
        }
        #endregion Method: AddExtension(Type type, INodeExtension extension)

        #region Method: RemoveExtension(Type type)
        /// <summary>
        /// Removes an existing extension.
        /// </summary>
        /// <param name="type">The type of the extension to remove.</param>
        public Message RemoveExtension(Type type)
        {
            if (this.extensions.ContainsKey(type))
            {
                this.extensions.Remove(type);
                NotifyPropertyChanged("Extensions");

                return Message.RelationSuccess;
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveExtension(Type type)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasName(String name)
        /// <summary>
        /// Checks whether this node has the given name.
        /// </summary>
        /// <param name="name">The name to check.</param>
        /// <returns>Returns whether this node has the given name.</returns>
        public bool HasName(String name)
        {
            return this.Names.Contains(name);
        }
        #endregion Method: HasName(String name)

        #region Method: HasTag(String tag)
        /// <summary>
        /// Checks whether this node has the given tag.
        /// </summary>
        /// <param tag="tag">The tag to check.</param>
        /// <returns>Returns whether this node has the given tag.</returns>
        public bool HasTag(String tag)
        {
            return this.Tags.Contains(tag);
        }
        #endregion Method: HasTag(String tag)

        #region Method: HasParent(Node parent)
        /// <summary>
        /// Checks whether this node has the given parent as one of its parents.
        /// </summary>
        /// <param name="parent">The parent to check.</param>
        /// <returns>Returns whether this node has the given parent as one of its parents.</returns>
        public bool HasParent(Node parent)
        {
            foreach (Node p in this.PersonalParents)
            {
                if (p.Equals(parent))
                    return true;
                else
                {
                    if (p.HasParent(parent))
                        return true;
                }
            }

            return false;
        }
        #endregion Method: HasParent(Node parent)

        #region Method: HasChild(Node child)
        /// <summary>
        /// Checks whether this node has the given child as one of its children.
        /// </summary>
        /// <param name="child">The child to check.</param>
        /// <returns>Returns whether this node has the given child as one of its children.</returns>
        public bool HasChild(Node child)
        {
            foreach (Node c in this.PersonalChildren)
            {
                if (c.Equals(child))
                    return true;
                else
                {
                    if (c.HasChild(child))
                        return true;
                }
            }

            return false;
        }
        #endregion Method: HasChild(Node child)

        #region Method: IsNodeOf(Node node)
        /// <summary>
        /// Checks whether this node is equal to the given node, or has the given node as one of its parents.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <returns>Returns whether this node is equal to the given node, or has the given node as one of its parents.</returns>
        public bool IsNodeOf(Node node)
        {
            if (node != null)
            {
                if (node.Equals(this) || HasParent(node))
                    return true;
            }
            return false;
        }
        #endregion Method: IsNodeOf(Node node)

        #region Method: CompareTo(Node other)
        /// <summary>
        /// Compares the node to the other node.
        /// </summary>
        /// <param name="other">The node to compare to this node.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Node other)
        {
            if (this == other)
                return 0;
            if (other != null)
                return string.Compare(this.DefaultName, other.DefaultName, false, CommonSettings.Culture);
            return 0;
        }
        #endregion Method: CompareTo(Node other)

        #region Method: Remove()
        /// <summary>
        /// Remove the node.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the relation with the children, so they will get a connection with the parent of this node
            for (int i = 0; i < this.PersonalChildren.Count; i++)
            {
                RemoveChildParentRelation(this.PersonalChildren[i], this);
                i--;
            }
            // Remove the links with parents and children
            foreach (Node parent in this.PersonalParents)
                parent.personalChildren = null;
            Database.Current.Remove(this.ID, GenericTables.NodeParent);
            Database.Current.Remove(GenericTables.NodeParent, Columns.Parent, this.ID);

            // Remove the names, tags, and description
            Database.Current.Remove(this.ID, LocalizationTables.NodeName);
            Database.Current.Remove(this.ID, LocalizationTables.NodeTag);
            Database.Current.Remove(this.ID, LocalizationTables.NodeDescription);

            // Remove the extensions
            foreach (INodeExtension nodeExtension in this.extensions.Values)
                nodeExtension.Remove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: Clone()
        /// <summary>
        /// Clones the node.
        /// </summary>
        /// <returns>A clone of the node.</returns>
        public Node Clone()
        {
            try
            {
                Type type = this.GetType();
                return type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { type }, null).Invoke(new object[] { this }) as Node;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion Method: Clone()

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation of the node, which is its default name.
        /// </summary>
        /// <returns>The string representation of the node, which is its default name.</returns>
        public override string ToString()
        {
            return this.DefaultName;
        }
        #endregion Method: ToString()

        #region Method: GetExtension(Type type)
        /// <summary>
        /// Get the extension of the given type.
        /// </summary>
        /// <param name="type">The type of the extension.</param>
        /// <returns>The extension of the given type.</returns>
        public INodeExtension GetExtension(Type type)
        {
            if(this.extensions.ContainsKey(type))
                return this.extensions[type];

            return null;
        }
        #endregion Method: GetExtension(Type type)

        #endregion Method Group: Other

    }
    #endregion Class: Node

    #region Class: NodeValued
    /// <summary>
    /// A valued version of a node.
    /// </summary>
    public abstract class NodeValued : IdHolder
    {

        #region Properties and Fields

        #region Property: Node
        /// <summary>
        /// Gets the node of which this is a valued node.
        /// </summary>
        public Node Node
        {
            get
            {
                return Database.Current.Select<Node>(this.ID, ValueTables.NodeValued, Columns.Node);
            }
            protected set
            {
                Database.Current.Update(this.ID, ValueTables.NodeValued, Columns.Node, value);
                NotifyPropertyChanged("Node");
            }
        }
        #endregion Property: Node

        #region Property: IsOverridden
        /// <summary>
        /// Gets or sets the value that indicates whether the valued node is overridden.
        /// </summary>
        public bool IsOverridden
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.NodeValued, Columns.IsOverridden);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NodeValued, Columns.IsOverridden, value);
                NotifyPropertyChanged("IsOverridden");
            }
        }
        #endregion Property: IsOverridden

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: NodeValued()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static NodeValued()
        {
            // Node
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Node, new Tuple<Type, EntryType>(typeof(Node), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.NodeValued, typeof(NodeValued), dict);
        }
        #endregion Static Constructor: NodeValued()

        #region Constructor: NodeValued(uint id)
        /// <summary>
        /// Creates a new valued node from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the valued node from.</param>
        protected NodeValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: NodeValued(uint id)

        #region Constructor: NodeValued(NodeValued nodeValued)
        /// <summary>
        /// Clones a valued node.
        /// </summary>
        /// <param name="nodeValued">The valued node to clone.</param>
        protected NodeValued(NodeValued nodeValued)
            : base()
        {
            if (nodeValued != null)
            {
                Database.Current.StartChange();

                this.Node = nodeValued.Node;
                this.IsOverridden = nodeValued.IsOverridden;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: NodeValued(NodeValued nodeValued)

        #region Constructor: NodeValued(Node node)
        /// <summary>
        /// Creates a new valued node from the given node.
        /// </summary>
        /// <param name="node">The node to create a valued node from.</param>
        protected NodeValued(Node node)
            : base()
        {
            Database.Current.StartChange();

            if (node == null)
                throw new NullReferenceException("NodeValued cannot be created with null-reference as node!");
            this.Node = node;
            this.IsOverridden = false;

            Database.Current.StopChange();
        }
        #endregion Constructor: NodeValued(Node node)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the valued node.
        /// </summary>
        /// <returns>A clone of the valued node.</returns>
        public NodeValued Clone()
        {
            try
            {
                Type type = this.GetType();
                return type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { type }, null).Invoke(new object[] { this }) as NodeValued;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion Method: Clone()

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            if (this.Node != null)
                return this.Node.DefaultName;
            return base.ToString();
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: NodeValued

    #region Class: NodeCondition
    /// <summary>
    /// A condition on a node.
    /// </summary>
    public abstract class NodeCondition : Condition
    {

        #region Properties and Fields

        #region Property: Node
        /// <summary>
        /// Gets or sets the required node.
        /// </summary>
        public Node Node
        {
            get
            {
                return Database.Current.Select<Node>(this.ID, ValueTables.NodeCondition, Columns.Node);
            }
            protected set
            {
                Database.Current.Update(this.ID, ValueTables.NodeCondition, Columns.Node, value);
                NotifyPropertyChanged("Node");
            }
        }
        #endregion Property: Node

        #region Property: NodeSign
        /// <summary>
        /// Gets or sets the equality sign of the node in the condition.
        /// </summary>
        public EqualitySign NodeSign
        {
            get
            {
                return Database.Current.Select<EqualitySign>(this.ID, ValueTables.NodeCondition, Columns.NodeSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.NodeCondition, Columns.NodeSign, value);
                NotifyPropertyChanged("NodeSign");
            }
        }
        #endregion Property: NodeSign

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: NodeCondition()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static NodeCondition()
        {
            // Node
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Node, new Tuple<Type, EntryType>(typeof(Node), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.NodeCondition, typeof(NodeCondition), dict);
        }
        #endregion Static Constructor: NodeCondition()

        #region Constructor: NodeCondition()
        /// <summary>
        /// Creates a new node condition.
        /// </summary>
        protected NodeCondition()
            : base()
        {
        }
        #endregion Constructor: NodeCondition()

        #region Constructor: NodeCondition(uint id)
        /// <summary>
        /// Creates a new node condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a node condition from.</param>
        protected NodeCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: NodeCondition(uint id)

        #region Constructor: NodeCondition(NodeCondition nodeCondition)
        /// <summary>
        /// Clones a node condition.
        /// </summary>
        /// <param name="nodeCondition">The node condition to clone.</param>
        protected NodeCondition(NodeCondition nodeCondition)
            : base(nodeCondition)
        {
            if (nodeCondition != null)
            {
                Database.Current.StartChange();

                this.Node = nodeCondition.Node;
                this.NodeSign = nodeCondition.NodeSign;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: NodeCondition(NodeCondition nodeCondition)

        #region Constructor: NodeCondition(Node node)
        /// <summary>
        /// Creates a condition for the given node.
        /// </summary>
        /// <param name="node">The node to create a condition for.</param>
        protected NodeCondition(Node node)
            : base()
        {
            Database.Current.StartChange();

            this.Node = node;
            this.NodeSign = EqualitySign.Equal;

            Database.Current.StopChange();
        }
        #endregion Constructor: NodeCondition(Node node)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the node condition.
        /// </summary>
        /// <returns>A clone of the node condition.</returns>
        public new NodeCondition Clone()
        {
            return base.Clone() as NodeCondition;
        }
        #endregion Method: Clone()

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            if (this.Node != null)
                return this.Node.DefaultName;
            return base.ToString();
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: NodeCondition

    #region Class: NodeChange
    /// <summary>
    /// A change on a node.
    /// </summary>
    public abstract class NodeChange : Change
    {

        #region Properties and Fields

        #region Property: Node
        /// <summary>
        /// Gets the affected node.
        /// </summary>
        public Node Node
        {
            get
            {
                return Database.Current.Select<Node>(this.ID, ValueTables.NodeChange, Columns.Node);
            }
            protected set
            {
                Database.Current.Update(this.ID, ValueTables.NodeChange, Columns.Node, value);
                NotifyPropertyChanged("Node");
            }
        }
        #endregion Property: Node

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: NodeChange()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static NodeChange()
        {
            // Node
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Node, new Tuple<Type, EntryType>(typeof(Node), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.NodeChange, typeof(NodeChange), dict);
        }
        #endregion Static Constructor: NodeChange()

        #region Constructor: NodeChange()
        /// <summary>
        /// Creates a new node change.
        /// </summary>
        protected NodeChange()
            : base()
        {
        }
        #endregion Constructor: NodeChange()

        #region Constructor: NodeChange(uint id)
        /// <summary>
        /// Creates a new node change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a node change from.</param>
        protected NodeChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: NodeChange(uint id)

        #region Constructor: NodeChange(NodeChange nodeChange)
        /// <summary>
        /// Clones a node change.
        /// </summary>
        /// <param name="nodeChange">The node change to clone.</param>
        protected NodeChange(NodeChange nodeChange)
            : base(nodeChange)
        {
            if (nodeChange != null)
            {
                Database.Current.StartChange();

                this.Node = nodeChange.Node;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: NodeChange(NodeChange nodeChange)

        #region Constructor: NodeChange(Node node)
        /// <summary>
        /// Creates a change for the given node.
        /// </summary>
        /// <param name="node">The node to create a change for.</param>
        protected NodeChange(Node node)
            : base()
        {
            Database.Current.StartChange();

            this.Node = node;

            Database.Current.StopChange();
        }
        #endregion Constructor: NodeChange(Node node)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the node change.
        /// </summary>
        /// <returns>A clone of the node change.</returns>
        public new NodeChange Clone()
        {
            return base.Clone() as NodeChange;
        }
        #endregion Method: Clone()

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            if (this.Node != null)
                return this.Node.DefaultName;
            return base.ToString();
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: NodeChange

}