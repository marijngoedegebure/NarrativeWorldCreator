/**************************************************************************
 * 
 * Content.cs
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
using Common;
using GameSemantics.Data;
using Semantics.Abstractions;
using Semantics.Components;
using Semantics.Data;
using Semantics.Utilities;

namespace GameSemantics.GameContent
{

    #region Class: Content
    /// <summary>
    /// Content stores the file name that represents it, and its predicate types.
    /// </summary>
    public abstract class Content : Node, IComparable<Content>
    {

        #region Properties and Fields

        #region Property: File
        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        public String File
        {
            get
            {
                return GameDatabase.Current.Select<String>(this.ID, GameTables.Content, GameColumns.File);
            }
            set
            {
                // Remove the current file from the names
                String currentFile = this.File;
                if (currentFile != null)
                    RemoveName(BasicFunctionality.GetFile(currentFile));

                // Update the file
                GameDatabase.Current.Update(this.ID, GameTables.Content, GameColumns.File, value);
                NotifyPropertyChanged("File");

                // Add the new file to the names
                AddName(BasicFunctionality.GetFile(value));
            }
        }
        #endregion Property: File

        #region Property: PredicateTypes
        /// <summary>
        /// Gets the personal and inherited predicate types of the content.
        /// </summary>
        public ReadOnlyCollection<PredicateType> PredicateTypes
        {
            get
            {
                List<PredicateType> predicateTypes = new List<PredicateType>();
                predicateTypes.AddRange(this.PersonalPredicateTypes);
                predicateTypes.AddRange(this.InheritedPredicateTypes);
                return predicateTypes.AsReadOnly();
            }
        }
        #endregion Property: PredicateTypes

        #region Property: PersonalPredicateTypes
        /// <summary>
        /// Gets the personal predicate types.
        /// </summary>
        public ReadOnlyCollection<PredicateType> PersonalPredicateTypes
        {
            get
            {
                return GameDatabase.Current.SelectAll<PredicateType>(this.ID, GameTables.ContentPredicateType, GameColumns.PredicateType).AsReadOnly();
            }
        }
        #endregion Property: PersonalPredicateTypes

        #region Property: InheritedPredicateTypes
        /// <summary>
        /// Gets the inherited predicate types.
        /// </summary>
        public ReadOnlyCollection<PredicateType> InheritedPredicateTypes
        {
            get
            {
                List<PredicateType> inheritedPredicateTypes = new List<PredicateType>();
                foreach (Node parent in this.PersonalParents)
                    inheritedPredicateTypes.AddRange(((Content)parent).PredicateTypes);
                return inheritedPredicateTypes.AsReadOnly();
            }
        }
        #endregion Property: InheritedPredicateTypes

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: Content()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static Content()
        {
            // Predicate types
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(GameColumns.PredicateType, new Tuple<Type, EntryType>(typeof(PredicateType), EntryType.Unique));
            GameDatabase.Current.AddTableDefinition(GameTables.ContentPredicateType, typeof(Content), dict);
        }
        #endregion Static Constructor: Content()

        #region Constructor: Content()
        /// <summary>
        /// Creates new content.
        /// </summary>
        protected Content()
            : base()
        {
        }
        #endregion Constructor: Content()

        #region Constructor: Content(uint id)
        /// <summary>
        /// Creates new content from the given ID.
        /// </summary>
        /// <param name="id">The ID to create content from.</param>
        protected Content(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Content(uint id)

        #region Constructor: Content(string file)
        /// <summary>
        /// Creates new content with the given file.
        /// </summary>
        /// <param name="file">The file to assign to the content.</param>
        protected Content(string file)
            : base(BasicFunctionality.GetFile(file))
        {
            GameDatabase.Current.StartChange();

            this.File = file;

            GameDatabase.Current.StopChange();
        }
        #endregion Constructor: Content(string file)

        #region Constructor: Content(Content content)
        /// <summary>
        /// Clones content.
        /// </summary>
        /// <param name="content">The content to clone.</param>
        protected Content(Content content)
            : base(content)
        {
            if (content != null)
            {
                GameDatabase.Current.StartChange();

                this.File = content.File;
                foreach (PredicateType predicateType in content.PersonalPredicateTypes)
                    AddPredicateType(new PredicateType(predicateType));

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: Content(Content content)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddPredicateType(PredicateType predicateType)
        /// <summary>
        /// Adds a predicate type.
        /// </summary>
        /// <param name="predicateType">The predicate type to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddPredicateType(PredicateType predicateType)
        {
            if (predicateType != null)
            {
                // If the predicate type is already available in all predicate types, there is no use to add it
                if (HasPredicateType(predicateType))
                    return Message.RelationExistsAlready;

                // Add the predicate type to the predicate types
                GameDatabase.Current.Insert(this.ID, GameTables.ContentPredicateType, GameColumns.PredicateType, predicateType);
                NotifyPropertyChanged("PersonalPredicateTypes");
                NotifyPropertyChanged("PredicateTypes");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddPredicateType(PredicateType predicateType)

        #region Method: RemovePredicateType(PredicateType predicateType)
        /// <summary>
        /// Removes a predicate type.
        /// </summary>
        /// <param name="predicateType">The predicate type to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemovePredicateType(PredicateType predicateType)
        {
            if (predicateType != null)
            {
                if (HasPredicateType(predicateType))
                {
                    // Remove the predicate type
                    GameDatabase.Current.Remove(this.ID, GameTables.ContentPredicateType, GameColumns.PredicateType, predicateType);
                    NotifyPropertyChanged("PersonalPredicateTypes");
                    NotifyPropertyChanged("PredicateTypes");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemovePredicateType(PredicateType predicateType)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasPredicateType(PredicateType predicateType)
        /// <summary>
        /// Checks if this content has the given predicate type.
        /// </summary>
        /// <param name="predicateType">The predicate type to check.</param>
        /// <returns>Returns true when the content has the predicate type.</returns>
        public bool HasPredicateType(PredicateType predicateType)
        {
            if (predicateType != null)
            {
                foreach (PredicateType myPredicateType in this.PredicateTypes)
                {
                    if (myPredicateType.Equals(predicateType))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasPredicateType(PredicateType predicateType)

        #region Method: Clone()
        /// <summary>
        /// Clones the content.
        /// </summary>
        /// <returns>A clone of the content.</returns>
        public new Content Clone()
        {
            return base.Clone() as Content;
        }
        #endregion Method: Clone()

        #region Method: CompareTo(Content other)
        /// <summary>
        /// Compares the content to the other content.
        /// </summary>
        /// <param name="other">The content to compare to this content.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Content other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(Content other)

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>The string representation</returns>
        public override String ToString()
        {
            if (this.File == null)
                return base.ToString();
            return BasicFunctionality.GetFile(this.File);
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: Content

    #region Class: ContentValued
    /// <summary>
    /// A valued version of content.
    /// </summary>
    public abstract class ContentValued : NodeValued
    {

        #region Properties and Fields

        #region Property: Content
        /// <summary>
        /// Gets the content of which this is valued content.
        /// </summary>
        public Content Content
        {
            get
            {
                return this.Node as Content;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: Content

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ContentValued(uint id)
        /// <summary>
        /// Creates new valued content from the given ID.
        /// </summary>
        /// <param name="id">The ID to create valued content from.</param>
        protected ContentValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ContentValued(uint id)

        #region Constructor: ContentValued(ContentValued contentValued)
        /// <summary>
        /// Clones the valued content.
        /// </summary>
        /// <param name="contentValued">The valued content to clone.</param>
        protected ContentValued(ContentValued contentValued)
            : base(contentValued)
        {
        }
        #endregion Constructor: ContentValued(ContentValued contentValued)

        #region Constructor: ContentValued(Content content)
        /// <summary>
        /// Creates new valued content from the given content.
        /// </summary>
        /// <param name="content">The content to create valued content from.</param>
        protected ContentValued(Content content)
            : base(content)
        {
        }
        #endregion Constructor: ContentValued(Content content)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the valued content.
        /// </summary>
        /// <returns>A clone of the valued content.</returns>
        public new ContentValued Clone()
        {
            return base.Clone() as ContentValued;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: ContentValued

    #region Class: ContentCondition
    /// <summary>
    /// A condition on content.
    /// </summary>
    public abstract class ContentCondition : NodeCondition
    {

        #region Properties and Fields

        #region Property: Content
        /// <summary>
        /// Gets or sets the required content.
        /// </summary>
        public Content Content
        {
            get
            {
                return this.Node as Content;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Content

        #region Property: File
        /// <summary>
        /// Gets or sets the required file.
        /// </summary>
        public String File
        {
            get
            {
                return GameDatabase.Current.Select<String>(this.ID, GameValueTables.ContentCondition, GameColumns.File);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.ContentCondition, GameColumns.File, value);
                NotifyPropertyChanged("File");
            }
        }
        #endregion Property: File

        #region Property: PredicateTypes
        /// <summary>
        /// Gets the required predicate types.
        /// </summary>
        public ReadOnlyCollection<PredicateType> PredicateTypes
        {
            get
            {
                return GameDatabase.Current.SelectAll<PredicateType>(this.ID, GameValueTables.ContentConditionPredicateType, GameColumns.PredicateType).AsReadOnly();
            }
        }
        #endregion Property: PredicateTypes

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: ContentCondition()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static ContentCondition()
        {
            // Predicate types
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.PredicateType, new Tuple<Type, EntryType>(typeof(PredicateType), EntryType.Intermediate));
            GameDatabase.Current.AddTableDefinition(GameValueTables.ContentConditionPredicateType, typeof(ContentCondition), dict);
        }
        #endregion Static Constructor: ContentCondition()

        #region Constructor: ContentCondition()
        /// <summary>
        /// Creates a new content condition.
        /// </summary>
        protected ContentCondition()
            : base()
        {
        }
        #endregion Constructor: ContentCondition()

        #region Constructor: ContentCondition(uint id)
        /// <summary>
        /// Creates a new content condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the content condition from.</param>
        protected ContentCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ContentCondition(uint id)

        #region Constructor: ContentCondition(ContentCondition contentCondition)
        /// <summary>
        /// Clones a content condition.
        /// </summary>
        /// <param name="contentCondition">The content condition to clone.</param>
        protected ContentCondition(ContentCondition contentCondition)
            : base(contentCondition)
        {
            if (contentCondition != null)
            {
                GameDatabase.Current.StartChange();

                this.File = contentCondition.File;

                foreach (PredicateType predicateType in contentCondition.PredicateTypes)
                    AddPredicateType(predicateType);

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: ContentCondition(ContentCondition contentCondition)

        #region Constructor: ContentCondition(Content content)
        /// <summary>
        /// Creates a condition for the given content.
        /// </summary>
        /// <param name="content">The content to create a condition for.</param>
        protected ContentCondition(Content content)
            : base(content)
        {
        }
        #endregion Constructor: ContentCondition(Content content)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddPredicateType(PredicateType predicateType)
        /// <summary>
        /// Adds a predicate type to the content condition.
        /// </summary>
        /// <param name="predicateType">The predicate type to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddPredicateType(PredicateType predicateType)
        {
            if (predicateType != null)
            {
                // If the predicate type is already there, there's no use to add it again
                if (HasPredicateType(predicateType))
                    return Message.RelationExistsAlready;

                // Add the predicate type
                GameDatabase.Current.Insert(this.ID, GameValueTables.ContentConditionPredicateType, GameColumns.PredicateType, predicateType);
                NotifyPropertyChanged("PredicateTypes");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddPredicateType(PredicateType predicateType)

        #region Method: RemovePredicateType(PredicateType predicateType)
        /// <summary>
        /// Removes a predicate type from the content condition.
        /// </summary>
        /// <param name="predicateType">The predicate type to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemovePredicateType(PredicateType predicateType)
        {
            if (predicateType != null)
            {
                if (HasPredicateType(predicateType))
                {
                    // Remove the predicate type
                    GameDatabase.Current.Remove(this.ID, GameValueTables.ContentConditionPredicateType, GameColumns.PredicateType, predicateType);
                    NotifyPropertyChanged("PredicateTypes");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemovePredicateType(PredicateType predicateType)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasPredicateType(PredicateType predicateType)
        /// <summary>
        /// Checks if this content condition has the given predicate type.
        /// </summary>
        /// <param name="predicateType">The predicate type to check.</param>
        /// <returns>Returns true when this content condition has the predicate type.</returns>
        public bool HasPredicateType(PredicateType predicateType)
        {
            if (predicateType != null)
            {
                foreach (PredicateType myPredicateType in this.PredicateTypes)
                {
                    if (predicateType.Equals(myPredicateType))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasPredicateType(PredicateType predicateType)

        #region Method: Remove()
        /// <summary>
        /// Remove the content condition.
        /// </summary>
        public override void Remove()
        {
            GameDatabase.Current.StartChange();

            // Remove the predicate types
            GameDatabase.Current.Remove(this.ID, GameValueTables.ContentConditionPredicateType);

            base.Remove();

            GameDatabase.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: Clone()
        /// <summary>
        /// Clones the content condition.
        /// </summary>
        /// <returns>A clone of the content condition.</returns>
        public new ContentCondition Clone()
        {
            return base.Clone() as ContentCondition;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: ContentCondition

    #region Class: ContentChange
    /// <summary>
    /// A change on content.
    /// </summary>
    public abstract class ContentChange : NodeChange
    {

        #region Properties and Fields

        #region Property: Content
        /// <summary>
        /// Gets or sets the affected content.
        /// </summary>
        public Content Content
        {
            get
            {
                return this.Node as Content;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Content

        #region Property: File
        /// <summary>
        /// Gets or sets the file to change to.
        /// </summary>
        public String File
        {
            get
            {
                return GameDatabase.Current.Select<String>(this.ID, GameValueTables.ContentChange, GameColumns.File);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.ContentChange, GameColumns.File, value);
                NotifyPropertyChanged("File");
            }
        }
        #endregion Property: File
        
        #region Property: PredicateTypesToAdd
        /// <summary>
        /// Gets the predicate types that should be added during the change.
        /// </summary>
        public ReadOnlyCollection<PredicateType> PredicateTypesToAdd
        {
            get
            {
                return GameDatabase.Current.SelectAll<PredicateType>(this.ID, GameValueTables.ContentChangePredicateTypeToAdd, GameColumns.PredicateTypeToAdd).AsReadOnly();
            }
        }
        #endregion Property: PredicateTypesToAdd

        #region Property: PredicateTypesToRemove
        /// <summary>
        /// Gets the predicate types that should be removed during the change.
        /// </summary>
        public ReadOnlyCollection<PredicateType> PredicateTypesToRemove
        {
            get
            {
                return GameDatabase.Current.SelectAll<PredicateType>(this.ID, GameValueTables.ContentChangePredicateTypeToRemove, GameColumns.PredicateTypeToRemove).AsReadOnly();
            }
        }
        #endregion Property: PredicateTypesToRemove

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: ContentChange()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static ContentChange()
        {
            // Predicate types
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(GameColumns.PredicateTypeToAdd, new Tuple<Type, EntryType>(typeof(PredicateType), EntryType.Intermediate));
            GameDatabase.Current.AddTableDefinition(GameValueTables.ContentChangePredicateTypeToAdd, typeof(ContentChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(GameColumns.PredicateTypeToRemove, new Tuple<Type, EntryType>(typeof(PredicateType), EntryType.Intermediate));
            GameDatabase.Current.AddTableDefinition(GameValueTables.ContentChangePredicateTypeToRemove, typeof(ContentChange), dict);
        }
        #endregion Static Constructor: ContentChange()

        #region Constructor: ContentChange()
        /// <summary>
        /// Creates a content change.
        /// </summary>
        protected ContentChange()
            : base()
        {
        }
        #endregion Constructor: ContentChange()

        #region Constructor: ContentChange(uint id)
        /// <summary>
        /// Creates a new content change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a content change from.</param>
        protected ContentChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ContentChange(uint id)

        #region Constructor: ContentChange(ContentChange contentChange)
        /// <summary>
        /// Clones a content change.
        /// </summary>
        /// <param name="contentChange">The content change to clone.</param>
        protected ContentChange(ContentChange contentChange)
            : base(contentChange)
        {
            if (contentChange != null)
            {
                GameDatabase.Current.StartChange();

                this.File = contentChange.File;

                foreach (PredicateType predicateTypeToAdd in contentChange.PredicateTypesToAdd)
                    AddPredicateTypeToAdd(predicateTypeToAdd);
                foreach (PredicateType predicateTypeToRemove in contentChange.PredicateTypesToRemove)
                    AddPredicateTypeToRemove(predicateTypeToRemove);

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: ContentChange(ContentChange contentChange)

        #region Constructor: ContentChange(Content content)
        /// <summary>
        /// Creates a change for the given content.
        /// </summary>
        /// <param name="content">The content to create a change for.</param>
        protected ContentChange(Content content)
            : base(content)
        {
        }
        #endregion Constructor: ContentChange(Content content)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddPredicateTypeToAdd(PredicateType predicateType)
        /// <summary>
        /// Adds a predicate type to the list with predicate types to add.
        /// </summary>
        /// <param name="predicateType">The predicate type to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddPredicateTypeToAdd(PredicateType predicateType)
        {
            if (predicateType != null)
            {
                // If the predicate type is already available in all predicate types, there is no use to add it
                if (HasPredicateTypeToAdd(predicateType))
                    return Message.RelationExistsAlready;

                // Add the predicate type to the predicate types
                GameDatabase.Current.Insert(this.ID, GameValueTables.ContentChangePredicateTypeToAdd, GameColumns.PredicateTypeToAdd, predicateType);
                NotifyPropertyChanged("PredicateTypesToAdd");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddPredicateTypeToAdd(PredicateType predicateType)

        #region Method: RemovePredicateTypeToAdd(PredicateType predicateType)
        /// <summary>
        /// Removes a predicate type from the list of predicate types to add.
        /// </summary>
        /// <param name="predicateType">The predicate type to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemovePredicateTypeToAdd(PredicateType predicateType)
        {
            if (predicateType != null)
            {
                if (HasPredicateTypeToAdd(predicateType))
                {
                    // Remove the predicate type
                    GameDatabase.Current.Remove(this.ID, GameValueTables.ContentChangePredicateTypeToAdd, GameColumns.PredicateTypeToAdd, predicateType);
                    NotifyPropertyChanged("PredicateTypesToAdd");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemovePredicateTypeToAdd(PredicateType predicateType)

        #region Method: AddPredicateTypeToRemove(PredicateType predicateType)
        /// <summary>
        /// Adds a predicate type to the list with predicate types to remove.
        /// </summary>
        /// <param name="predicateType">The predicate type to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddPredicateTypeToRemove(PredicateType predicateType)
        {
            if (predicateType != null)
            {
                // If the predicate type is already available in all predicate types, there is no use to add it
                if (HasPredicateTypeToRemove(predicateType))
                    return Message.RelationExistsAlready;

                // Add the predicate type to the predicate types
                GameDatabase.Current.Insert(this.ID, GameValueTables.ContentChangePredicateTypeToRemove, GameColumns.PredicateTypeToRemove, predicateType);
                NotifyPropertyChanged("PredicateTypesToRemove");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddPredicateTypeToRemove(PredicateType predicateType)

        #region Method: RemovePredicateTypeToRemove(PredicateType predicateType)
        /// <summary>
        /// Removes a predicate type from the list of predicate types to remove.
        /// </summary>
        /// <param name="predicateType">The predicate type to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemovePredicateTypeToRemove(PredicateType predicateType)
        {
            if (predicateType != null)
            {
                if (HasPredicateTypeToRemove(predicateType))
                {
                    // Remove the predicate type
                    GameDatabase.Current.Remove(this.ID, GameValueTables.ContentChangePredicateTypeToRemove, GameColumns.PredicateTypeToRemove, predicateType);
                    NotifyPropertyChanged("PredicateTypesToRemove");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemovePredicateTypeToRemove(PredicateType predicateType)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasPredicateTypeToAdd(PredicateType predicateType)
        /// <summary>
        /// Checks if this content change has the given predicate type to add.
        /// </summary>
        /// <param name="predicateType">The predicate type to check.</param>
        /// <returns>Returns true when the content change has the predicate type to add.</returns>
        public bool HasPredicateTypeToAdd(PredicateType predicateType)
        {
            if (predicateType != null)
            {
                foreach (PredicateType predicateTypeToAdd in this.PredicateTypesToAdd)
                {
                    if (predicateType.Equals(predicateTypeToAdd))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasPredicateTypeToAdd(PredicateType predicateType)

        #region Method: HasPredicateTypeToRemove(PredicateType predicateType)
        /// <summary>
        /// Checks if this content change has the given predicate type to remove.
        /// </summary>
        /// <param name="predicateType">The predicate type to check.</param>
        /// <returns>Returns true when the content change has the predicate type to remove.</returns>
        public bool HasPredicateTypeToRemove(PredicateType predicateType)
        {
            if (predicateType != null)
            {
                foreach (PredicateType predicateTypeToRemove in this.PredicateTypesToRemove)
                {
                    if (predicateType.Equals(predicateTypeToRemove))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasPredicateTypeToRemove(PredicateType predicateType)

        #region Method: Remove()
        /// <summary>
        /// Remove the content change.
        /// </summary>
        public override void Remove()
        {
            GameDatabase.Current.StartChange();

            // Remove the predicate types
            GameDatabase.Current.Remove(this.ID, GameValueTables.ContentChangePredicateTypeToAdd);
            GameDatabase.Current.Remove(this.ID, GameValueTables.ContentChangePredicateTypeToRemove);

            base.Remove();

            GameDatabase.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: Clone()
        /// <summary>
        /// Clones the content change.
        /// </summary>
        /// <returns>A clone of the content change.</returns>
        public new ContentChange Clone()
        {
            return base.Clone() as ContentChange;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: ContentChange

    #region Class: StaticContent
    /// <summary>
    /// Static content.
    /// </summary>
    public abstract class StaticContent : Content, IComparable<StaticContent>
    {

        #region Method Group: Constructors

        #region Constructor: StaticContent()
        /// <summary>
        /// Creates new static content.
        /// </summary>
        protected StaticContent()
            : base()
        {
        }
        #endregion Constructor: StaticContent()

        #region Constructor: StaticContent(uint id)
        /// <summary>
        /// Creates new static content from the given ID.
        /// </summary>
        /// <param name="id">The ID to create static content from.</param>
        protected StaticContent(uint id)
            : base(id)
        {
        }
        #endregion Constructor: StaticContent(uint id)

        #region Constructor: StaticContent(string file)
        /// <summary>
        /// Creates new static content with the given file.
        /// </summary>
        /// <param name="file">The file to assign to the static content.</param>
        protected StaticContent(string file)
            : base(file)
        {
        }
        #endregion Constructor: StaticContent(string file)

        #region Constructor: StaticContent(StaticContent staticContent)
        /// <summary>
        /// Clones static content.
        /// </summary>
        /// <param name="staticContent">The static content to clone.</param>
        protected StaticContent(StaticContent staticContent)
            : base(staticContent)
        {
        }
        #endregion Constructor: StaticContent(StaticContent staticContent)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the static content.
        /// </summary>
        /// <returns>A clone of the static content.</returns>
        public new StaticContent Clone()
        {
            return base.Clone() as StaticContent;
        }
        #endregion Method: Clone()

        #region Method: CompareTo(StaticContent other)
        /// <summary>
        /// Compares the static content to the other static content.
        /// </summary>
        /// <param name="other">The static content to compare to this static content.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(StaticContent other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(StaticContent other)

        #endregion Method Group: Other

    }
    #endregion Class: StaticContent

    #region Class: StaticContentValued
    /// <summary>
    /// A valued version of static content.
    /// </summary>
    public abstract class StaticContentValued : ContentValued
    {

        #region Properties and Fields

        #region Property: StaticContent
        /// <summary>
        /// Gets the static content of which this is valued static content.
        /// </summary>
        public StaticContent StaticContent
        {
            get
            {
                return this.Node as StaticContent;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: StaticContent

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: StaticContentValued(uint id)
        /// <summary>
        /// Creates new valued static content from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the valued static content from.</param>
        protected StaticContentValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: StaticContentValued(uint id)

        #region Constructor: StaticContentValued(StaticContentValued staticContentValued)
        /// <summary>
        /// Clones valued static content.
        /// </summary>
        /// <param name="staticContentValued">The valued static content to clone.</param>
        protected StaticContentValued(StaticContentValued staticContentValued)
            : base(staticContentValued)
        {
        }
        #endregion Constructor: StaticContentValued(StaticContentValued staticContentValued)

        #region Constructor: StaticContentValued(StaticContent staticContent)
        /// <summary>
        /// Creates new valued static content from the given static content.
        /// </summary>
        /// <param name="staticContent">The static content to create a valued static content from.</param>
        protected StaticContentValued(StaticContent staticContent)
            : base(staticContent)
        {
        }
        #endregion Constructor: StaticContentValued(StaticContent staticContent)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the static valued content.
        /// </summary>
        /// <returns>A clone of the static valued content.</returns>
        public new StaticContentValued Clone()
        {
            return base.Clone() as StaticContentValued;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: StaticContentValued

    #region Class: StaticContentCondition
    /// <summary>
    /// A condition on static content.
    /// </summary>
    public abstract class StaticContentCondition : ContentCondition
    {

        #region Properties and Fields

        #region Property: StaticContent
        /// <summary>
        /// Gets or sets the required static content.
        /// </summary>
        public StaticContent StaticContent
        {
            get
            {
                return this.Node as StaticContent;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: StaticContent

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: StaticContentCondition()
        /// <summary>
        /// Creates a new static content condition.
        /// </summary>
        protected StaticContentCondition()
            : base()
        {
        }
        #endregion Constructor: StaticContentCondition()

        #region Constructor: StaticContentCondition(uint id)
        /// <summary>
        /// Creates a new static content condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the static content condition from.</param>
        protected StaticContentCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: StaticContentCondition(uint id)

        #region Constructor: StaticContentCondition(StaticContentCondition staticContentCondition)
        /// <summary>
        /// Clones an static content condition.
        /// </summary>
        /// <param name="staticContentCondition">The static content condition to clone.</param>
        protected StaticContentCondition(StaticContentCondition staticContentCondition)
            : base(staticContentCondition)
        {
        }
        #endregion Constructor: StaticContentCondition(StaticContentCondition staticContentCondition)

        #region Constructor: StaticContentCondition(StaticContent staticContent)
        /// <summary>
        /// Creates a condition for the given static content.
        /// </summary>
        /// <param name="staticContent">The static content to create a condition for.</param>
        protected StaticContentCondition(StaticContent staticContent)
            : base(staticContent)
        {
        }
        #endregion Constructor: StaticContentCondition(StaticContent staticContent)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the static content condition.
        /// </summary>
        /// <returns>A clone of the static content condition.</returns>
        public new StaticContentCondition Clone()
        {
            return base.Clone() as StaticContentCondition;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: StaticContentCondition

    #region Class: StaticContentChange
    /// <summary>
    /// A change on static content.
    /// </summary>
    public abstract class StaticContentChange : ContentChange
    {

        #region Properties and Fields

        #region Property: StaticContent
        /// <summary>
        /// Gets or sets the affected static content.
        /// </summary>
        public StaticContent StaticContent
        {
            get
            {
                return this.Node as StaticContent;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: StaticContent

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: StaticContentChange()
        /// <summary>
        /// Creates a static content change.
        /// </summary>
        protected StaticContentChange()
            : base()
        {
        }
        #endregion Constructor: StaticContentChange()

        #region Constructor: StaticContentChange(uint id)
        /// <summary>
        /// Creates a new static content change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a static content change from.</param>
        protected StaticContentChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: StaticContentChange(uint id)

        #region Constructor: StaticContentChange(StaticContentChange staticContentChange)
        /// <summary>
        /// Clones a static content change.
        /// </summary>
        /// <param name="staticContentChange">The static content change to clone.</param>
        protected StaticContentChange(StaticContentChange staticContentChange)
            : base(staticContentChange)
        {
        }
        #endregion Constructor: StaticContentChange(StaticContentChange staticContentChange)

        #region Constructor: StaticContentChange(StaticContent staticContent)
        /// <summary>
        /// Creates a change for the given static content.
        /// </summary>
        /// <param name="staticContent">The static content to create a change for.</param>
        protected StaticContentChange(StaticContent staticContent)
            : base(staticContent)
        {
        }
        #endregion Constructor: StaticContentChange(StaticContent staticContent)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the static content change.
        /// </summary>
        /// <returns>A clone of the static content change.</returns>
        public new StaticContentChange Clone()
        {
            return base.Clone() as StaticContentChange;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: StaticContentChange

    #region Class: DynamicContent
    /// <summary>
    /// Dynamic content.
    /// </summary>
    public abstract class DynamicContent : Content, IComparable<DynamicContent>
    {

        #region Method Group: Constructors

        #region Constructor: DynamicContent()
        /// <summary>
        /// Creates new dynamic content.
        /// </summary>
        protected DynamicContent()
            : base()
        {
        }
        #endregion Constructor: DynamicContent()

        #region Constructor: DynamicContent(uint id)
        /// <summary>
        /// Creates new dynamic content from the given ID.
        /// </summary>
        /// <param name="id">The ID to create dynamic content from.</param>
        protected DynamicContent(uint id)
            : base(id)
        {
        }
        #endregion Constructor: DynamicContent(uint id)

        #region Constructor: DynamicContent(string file)
        /// <summary>
        /// Creates new dynamic content with the given file.
        /// </summary>
        /// <param name="file">The file to assign to the dynamic content.</param>
        protected DynamicContent(string file)
            : base(file)
        {
        }
        #endregion Constructor: DynamicContent(string file)

        #region Constructor: DynamicContent(DynamicContent dynamicContent)
        /// <summary>
        /// Clones dynamic content.
        /// </summary>
        /// <param name="dynamicContent">The dynamic content to clone.</param>
        protected DynamicContent(DynamicContent dynamicContent)
            : base(dynamicContent)
        {
        }
        #endregion Constructor: DynamicContent(DynamicContent dynamicContent)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the dynamic content.
        /// </summary>
        /// <returns>A clone of the dynamic content.</returns>
        public new DynamicContent Clone()
        {
            return base.Clone() as DynamicContent;
        }
        #endregion Method: Clone()

        #region Method: CompareTo(DynamicContent other)
        /// <summary>
        /// Compares the dynamic content to the other dynamic content.
        /// </summary>
        /// <param name="other">The dynamic content to compare to this dynamic content.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(DynamicContent other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(DynamicContent other)

        #endregion Method Group: Other

    }
    #endregion Class: DynamicContent

    #region Class: DynamicContentValued
    /// <summary>
    /// A valued version of dynamic content.
    /// </summary>
    public abstract class DynamicContentValued : ContentValued
    {

        #region Properties and Fields

        #region Property: DynamicContent
        /// <summary>
        /// Gets the dynamic content of which this is valued dynamic content.
        /// </summary>
        public DynamicContent DynamicContent
        {
            get
            {
                return this.Node as DynamicContent;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: DynamicContent

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: DynamicContentValued(uint id)
        /// <summary>
        /// Creates new valued dynamic content from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the valued dynamic content from.</param>
        protected DynamicContentValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: DynamicContentValued(uint id)

        #region Constructor: DynamicContentValued(DynamicContentValued dynamicContentValued)
        /// <summary>
        /// Clones valued dynamic content.
        /// </summary>
        /// <param name="dynamicContentValued">The valued dynamic content to clone.</param>
        protected DynamicContentValued(DynamicContentValued dynamicContentValued)
            : base(dynamicContentValued)
        {
        }
        #endregion Constructor: DynamicContentValued(DynamicContentValued dynamicContentValued)

        #region Constructor: DynamicContentValued(DynamicContent dynamicContent)
        /// <summary>
        /// Creates new valued dynamic content from the given dynamic content.
        /// </summary>
        /// <param name="dynamicContent">The dynamic content to create a valued dynamic content from.</param>
        protected DynamicContentValued(DynamicContent dynamicContent)
            : base(dynamicContent)
        {
        }
        #endregion Constructor: DynamicContentValued(DynamicContent dynamicContent)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the dynamic valued content.
        /// </summary>
        /// <returns>A clone of the dynamic valued content.</returns>
        public new DynamicContentValued Clone()
        {
            return base.Clone() as DynamicContentValued;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: DynamicContentValued

    #region Class: DynamicContentCondition
    /// <summary>
    /// A condition on dynamic content.
    /// </summary>
    public abstract class DynamicContentCondition : ContentCondition
    {

        #region Properties and Fields

        #region Property: DynamicContent
        /// <summary>
        /// Gets or sets the required dynamic content.
        /// </summary>
        public DynamicContent DynamicContent
        {
            get
            {
                return this.Node as DynamicContent;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: DynamicContent

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: DynamicContentCondition()
        /// <summary>
        /// Creates a new dynamic content condition.
        /// </summary>
        protected DynamicContentCondition()
            : base()
        {
        }
        #endregion Constructor: DynamicContentCondition()

        #region Constructor: DynamicContentCondition(uint id)
        /// <summary>
        /// Creates a new dynamic content condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the dynamic content condition from.</param>
        protected DynamicContentCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: DynamicContentCondition(uint id)

        #region Constructor: DynamicContentCondition(DynamicContentCondition dynamicContentCondition)
        /// <summary>
        /// Clones an dynamic content condition.
        /// </summary>
        /// <param name="dynamicContentCondition">The dynamic content condition to clone.</param>
        protected DynamicContentCondition(DynamicContentCondition dynamicContentCondition)
            : base(dynamicContentCondition)
        {
        }
        #endregion Constructor: DynamicContentCondition(DynamicContentCondition dynamicContentCondition)

        #region Constructor: DynamicContentCondition(DynamicContent dynamicContent)
        /// <summary>
        /// Creates a condition for the given dynamic content.
        /// </summary>
        /// <param name="dynamicContent">The dynamic content to create a condition for.</param>
        protected DynamicContentCondition(DynamicContent dynamicContent)
            : base(dynamicContent)
        {
        }
        #endregion Constructor: DynamicContentCondition(DynamicContent dynamicContent)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the dynamic content condition.
        /// </summary>
        /// <returns>A clone of the dynamic content condition.</returns>
        public new DynamicContentCondition Clone()
        {
            return base.Clone() as DynamicContentCondition;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: DynamicContentCondition

    #region Class: DynamicContentChange
    /// <summary>
    /// A change on dynamic content.
    /// </summary>
    public abstract class DynamicContentChange : ContentChange
    {

        #region Properties and Fields

        #region Property: DynamicContent
        /// <summary>
        /// Gets or sets the affected dynamic content.
        /// </summary>
        public DynamicContent DynamicContent
        {
            get
            {
                return this.Node as DynamicContent;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: DynamicContent

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: DynamicContentChange()
        /// <summary>
        /// Creates a dynamic content change.
        /// </summary>
        protected DynamicContentChange()
            : base()
        {
        }
        #endregion Constructor: DynamicContentChange()

        #region Constructor: DynamicContentChange(uint id)
        /// <summary>
        /// Creates a new dynamic content change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a dynamic content change from.</param>
        protected DynamicContentChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: DynamicContentChange(uint id)

        #region Constructor: DynamicContentChange(DynamicContentChange dynamicContentChange)
        /// <summary>
        /// Clones a dynamic content change.
        /// </summary>
        /// <param name="dynamicContentChange">The dynamic content change to clone.</param>
        protected DynamicContentChange(DynamicContentChange dynamicContentChange)
            : base(dynamicContentChange)
        {
        }
        #endregion Constructor: DynamicContentChange(DynamicContentChange dynamicContentChange)

        #region Constructor: DynamicContentChange(DynamicContent dynamicContent)
        /// <summary>
        /// Creates a change for the given dynamic content.
        /// </summary>
        /// <param name="dynamicContent">The dynamic content to create a change for.</param>
        protected DynamicContentChange(DynamicContent dynamicContent)
            : base(dynamicContent)
        {
        }
        #endregion Constructor: DynamicContentChange(DynamicContent dynamicContent)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the dynamic content change.
        /// </summary>
        /// <returns>A clone of the dynamic content change.</returns>
        public new DynamicContentChange Clone()
        {
            return base.Clone() as DynamicContentChange;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: DynamicContentChange

}