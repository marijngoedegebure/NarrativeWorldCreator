/**************************************************************************
 * 
 * ContentBase.cs
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
using GameSemantics.GameContent;
using GameSemanticsEngine.Tools;
using Semantics.Abstractions;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Components;

namespace GameSemanticsEngine.GameContent
{

    #region Class: ContentBase
    /// <summary>
    /// A base of content.
    /// </summary>
    public abstract class ContentBase : NodeBase
    {

        #region Properties and Fields

        #region Property: Content
        /// <summary>
        /// Gets the content of which this is a content base.
        /// </summary>
        protected internal Content Content
        {
            get
            {
                return this.Node as Content;
            }
        }
        #endregion Property: Content

        #region Property: File
        /// <summary>
        /// The name of the file.
        /// </summary>
        private String file = String.Empty;

        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        public String File
        {
            get
            {
                return file;
            }
        }
        #endregion Property: File

        #region Property: PredicateTypes
        /// <summary>
        /// The predicate types.
        /// </summary>
        private PredicateTypeBase[] predicateTypes = null;

        /// <summary>
        /// Gets the predicate types.
        /// </summary>
        public ReadOnlyCollection<PredicateTypeBase> PredicateTypes
        {
            get
            {
                if (predicateTypes == null)
                {
                    LoadPredicateTypes();
                    if (predicateTypes == null)
                        return new List<PredicateTypeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<PredicateTypeBase>(predicateTypes);
            }
        }

        /// <summary>
        /// Loads the predicate types.
        /// </summary>
        private void LoadPredicateTypes()
        {
            if (this.Content != null)
            {
                List<PredicateTypeBase> predicateTypeBases = new List<PredicateTypeBase>();
                foreach (PredicateType predicateType in this.Content.PredicateTypes)
                    predicateTypeBases.Add(GameBaseManager.Current.GetBase<PredicateTypeBase>(predicateType));
                predicateTypes = predicateTypeBases.ToArray();
            }
        }
        #endregion Property: PredicateTypes

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ContentBase(Content content)
        /// <summary>
        /// Creates a new content base from the given content.
        /// </summary>
        /// <param name="content">The content to create a content base from.</param>
        protected ContentBase(Content content)
            : base(content)
        {
            if (content != null)
            {
                this.file = content.File;

                if (GameBaseManager.PreloadProperties)
                    LoadPredicateTypes();
            }
        }
        #endregion Constructor: ContentBase(Content content)

        #endregion Method Group: Constructors

    }
    #endregion Class: ContentBase

    #region Class: ContentValuedBase
    /// <summary>
    /// A base of valued content.
    /// </summary>
    public class ContentValuedBase : NodeValuedBase
    {

        #region Properties and Fields

        #region Property: ContentValued
        /// <summary>
        /// Gets the valued content of which this is a valued content base.
        /// </summary>
        protected internal ContentValued ContentValued
        {
            get
            {
                return this.NodeValued as ContentValued;
            }
        }
        #endregion Property: ContentValued

        #region Property: ContentBase
        /// <summary>
        /// Gets the content base.
        /// </summary>
        public ContentBase ContentBase
        {
            get
            {
                return this.NodeBase as ContentBase;
            }
        }
        #endregion Property: ContentBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ContentValuedBase(ContentValued contentValued)
        /// <summary>
        /// Create a valued content base from the given valued content.
        /// </summary>
        /// <param name="contentValued">The valued content to create a valued content base from.</param>
        protected ContentValuedBase(ContentValued contentValued)
            : base(contentValued)
        {
        }
        #endregion Constructor: ContentValuedBase(ContentValued contentValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: ContentValuedBase

    #region Class: ContentConditionBase
    /// <summary>
    /// A condition on content.
    /// </summary>
    public abstract class ContentConditionBase : NodeConditionBase
    {

        #region Properties and Fields

        #region Property: ContentCondition
        /// <summary>
        /// Gets the content condition of which this is a content condition base.
        /// </summary>
        protected internal ContentCondition ContentCondition
        {
            get
            {
                return this.Condition as ContentCondition;
            }
        }
        #endregion Property: ContentCondition

        #region Property: ContentBase
        /// <summary>
        /// Gets the content base of which this is a content condition base.
        /// </summary>
        public ContentBase ContentBase
        {
            get
            {
                return this.NodeBase as ContentBase;
            }
        }
        #endregion Property: ContentBase

        #region Property: File
        /// <summary>
        /// The required file.
        /// </summary>
        private String file = null;

        /// <summary>
        /// Gets the required file.
        /// </summary>
        public String File
        {
            get
            {
                return file;
            }
        }
        #endregion Property: File

        #region Property: PredicateTypes
        /// <summary>
        /// The required predicate types.
        /// </summary>
        private PredicateTypeBase[] predicateTypes = null;

        /// <summary>
        /// Gets the required predicate types.
        /// </summary>
        public ReadOnlyCollection<PredicateTypeBase> PredicateTypes
        {
            get
            {
                if (predicateTypes == null)
                {
                    LoadPredicateTypes();
                    if (predicateTypes == null)
                        return new List<PredicateTypeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<PredicateTypeBase>(predicateTypes);
            }
        }

        /// <summary>
        /// Loads the required predicate types.
        /// </summary>
        private void LoadPredicateTypes()
        {
            if (this.ContentCondition != null)
            {
                List<PredicateTypeBase> predicateTypeBases = new List<PredicateTypeBase>();
                foreach (PredicateType predicateType in this.ContentCondition.PredicateTypes)
                    predicateTypeBases.Add(GameBaseManager.Current.GetBase<PredicateTypeBase>(predicateType));
                predicateTypes = predicateTypeBases.ToArray();
            }
        }
        #endregion Property: PredicateTypes

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ContentConditionBase(ContentCondition contentCondition)
        /// <summary>
        /// Creates a base of the given content condition.
        /// </summary>
        /// <param name="contentCondition">The content condition to create a base of.</param>
        protected ContentConditionBase(ContentCondition contentCondition)
            : base(contentCondition)
        {
            if (contentCondition != null)
            {
                this.file = contentCondition.File;

                if (GameBaseManager.PreloadProperties)
                    LoadPredicateTypes();
            }
        }
        #endregion Constructor: ContentConditionBase(ContentCondition contentCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: ContentConditionBase

    #region Class: ContentChangeBase
    /// <summary>
    /// A change on content.
    /// </summary>
    public class ContentChangeBase : NodeChangeBase
    {

        #region Properties and Fields

        #region Property: ContentChange
        /// <summary>
        /// Gets the content change of which this is a content change base.
        /// </summary>
        protected internal ContentChange ContentChange
        {
            get
            {
                return this.Change as ContentChange;
            }
        }
        #endregion Property: ContentChange

        #region Property: ContentBase
        /// <summary>
        /// Gets the affected content base.
        /// </summary>
        public ContentBase ContentBase
        {
            get
            {
                return this.NodeBase as ContentBase;
            }
        }
        #endregion Property: ContentBase

        #region Property: File
        /// <summary>
        /// The file to change to.
        /// </summary>
        private String file = null;
        
        /// <summary>
        /// Gets the file to change to.
        /// </summary>
        public String File
        {
            get
            {
                return file;
            }
        }
        #endregion Property: File

        #region Property: PredicateTypesToAdd
        /// <summary>
        /// The predicate types that should be added during the change.
        /// </summary>
        private PredicateTypeBase[] predicateTypesToAdd = null;

        /// <summary>
        /// Gets the predicate types that should be added during the change.
        /// </summary>
        public ReadOnlyCollection<PredicateTypeBase> PredicateTypesToAdd
        {
            get
            {
                if (predicateTypesToAdd == null)
                {
                    LoadPredicateTypesToAdd();
                    if (predicateTypesToAdd == null)
                        return new List<PredicateTypeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<PredicateTypeBase>(predicateTypesToAdd);
            }
        }

        /// <summary>
        /// Loads the predicate types that should be added during the change.
        /// </summary>
        private void LoadPredicateTypesToAdd()
        {
            if (this.ContentChange != null)
            {
                List<PredicateTypeBase> predicateTypeBases = new List<PredicateTypeBase>();
                foreach (PredicateType predicateType in this.ContentChange.PredicateTypesToAdd)
                    predicateTypeBases.Add(GameBaseManager.Current.GetBase<PredicateTypeBase>(predicateType));
                predicateTypesToAdd = predicateTypeBases.ToArray();
            }
        }
        #endregion Property: PredicateTypesToAdd

        #region Property: PredicateTypesToRemove
        /// <summary>
        /// The predicate types that should be removed during the change.
        /// </summary>
        private PredicateTypeBase[] predicateTypesToRemove = null;

        /// <summary>
        /// Gets the predicate types that should be removed during the change.
        /// </summary>
        public ReadOnlyCollection<PredicateTypeBase> PredicateTypesToRemove
        {
            get
            {
                if (predicateTypesToRemove == null)
                {
                    LoadPredicateTypesToRemove();
                    if (predicateTypesToRemove == null)
                        return new List<PredicateTypeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<PredicateTypeBase>(predicateTypesToRemove);
            }
        }

        /// <summary>
        /// Loads the predicate types that should be removed during the change.
        /// </summary>
        private void LoadPredicateTypesToRemove()
        {
            if (this.ContentChange != null)
            {
                List<PredicateTypeBase> predicateTypeBases = new List<PredicateTypeBase>();
                foreach (PredicateType predicateType in this.ContentChange.PredicateTypesToRemove)
                    predicateTypeBases.Add(GameBaseManager.Current.GetBase<PredicateTypeBase>(predicateType));
                predicateTypesToRemove = predicateTypeBases.ToArray();
            }
        }
        #endregion Property: PredicateTypesToRemove

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ContentChangeBase(ContentChange contentChange)
        /// <summary>
        /// Creates a base of the given content change.
        /// </summary>
        /// <param name="contentChange">The content change to create a base of.</param>
        protected ContentChangeBase(ContentChange contentChange)
            : base(contentChange)
        {
            if (contentChange != null)
            {
                this.file = contentChange.File;

                if (GameBaseManager.PreloadProperties)
                {
                    LoadPredicateTypesToAdd();
                    LoadPredicateTypesToRemove();
                }
            }
        }
        #endregion Constructor: ContentChangeBase(ContentChange contentChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: ContentChangeBase

    #region Class: StaticContentBase
    /// <summary>
    /// A base of static content.
    /// </summary>
    public abstract class StaticContentBase : ContentBase
    {

        #region Properties and Fields

        #region Property: StaticContent
        /// <summary>
        /// Gets the static content of which this is a static content base.
        /// </summary>
        protected internal StaticContent StaticContent
        {
            get
            {
                return this.Node as StaticContent;
            }
        }
        #endregion Property: StaticContent

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: StaticContentBase(StaticContent staticContent)
        /// <summary>
        /// Creates a new static content base from the given static content.
        /// </summary>
        /// <param name="staticContent">The static content to create a static content base from.</param>
        protected StaticContentBase(StaticContent staticContent)
            : base(staticContent)
        {
        }
        #endregion Constructor: StaticContentBase(StaticContent staticContent)

        #endregion Method Group: Constructors

    }
    #endregion Class: StaticContentBase

    #region Class: StaticContentValuedBase
    /// <summary>
    /// A base of valued static content.
    /// </summary>
    public abstract class StaticContentValuedBase : ContentValuedBase
    {

        #region Properties and Fields

        #region Property: StaticContentValued
        /// <summary>
        /// Gets the valued static content of which this is a valued static content base.
        /// </summary>
        protected internal StaticContentValued StaticContentValued
        {
            get
            {
                return this.NodeValued as StaticContentValued;
            }
        }
        #endregion Property: StaticContentValued

        #region Property: StaticContentBase
        /// <summary>
        /// Gets the static content base.
        /// </summary>
        public StaticContentBase StaticContentBase
        {
            get
            {
                return this.NodeBase as StaticContentBase;
            }
        }
        #endregion Property: StaticContentBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: StaticContentValuedBase(StaticContentValued staticContentValued)
        /// <summary>
        /// Create a valued static content base from the given valued static content.
        /// </summary>
        /// <param name="staticContentValued">The valued static content to create a valued static content base from.</param>
        protected StaticContentValuedBase(StaticContentValued staticContentValued)
            : base(staticContentValued)
        {
        }
        #endregion Constructor: StaticContentValuedBase(StaticContentValued staticContentValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: StaticContentValuedBase

    #region Class: StaticContentConditionBase
    /// <summary>
    /// A condition on static content.
    /// </summary>
    public abstract class StaticContentConditionBase : ContentConditionBase
    {

        #region Properties and Fields

        #region Property: StaticContentCondition
        /// <summary>
        /// Gets the static content condition of which this is a static content condition base.
        /// </summary>
        protected internal StaticContentCondition StaticContentCondition
        {
            get
            {
                return this.Condition as StaticContentCondition;
            }
        }
        #endregion Property: StaticContentCondition

        #region Property: StaticContentBase
        /// <summary>
        /// Gets the static content base of which this is a static content condition base.
        /// </summary>
        public StaticContentBase StaticContentBase
        {
            get
            {
                return this.NodeBase as StaticContentBase;
            }
        }
        #endregion Property: StaticContentBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: StaticContentConditionBase(StaticContentCondition staticContentCondition)
        /// <summary>
        /// Creates a base of the given static content condition.
        /// </summary>
        /// <param name="staticContentCondition">The static content condition to create a base of.</param>
        protected StaticContentConditionBase(StaticContentCondition staticContentCondition)
            : base(staticContentCondition)
        {
        }
        #endregion Constructor: StaticContentConditionBase(StaticContentCondition staticContentCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: StaticContentConditionBase

    #region Class: StaticContentChangeBase
    /// <summary>
    /// A change on static content.
    /// </summary>
    public abstract class StaticContentChangeBase : ContentChangeBase
    {

        #region Properties and Fields

        #region Property: StaticContentChange
        /// <summary>
        /// Gets the static content change of which this is a static content change base.
        /// </summary>
        protected internal StaticContentChange StaticContentChange
        {
            get
            {
                return this.Change as StaticContentChange;
            }
        }
        #endregion Property: StaticContentChange

        #region Property: StaticContentBase
        /// <summary>
        /// Gets the affected static content base.
        /// </summary>
        public StaticContentBase StaticContentBase
        {
            get
            {
                return this.NodeBase as StaticContentBase;
            }
        }
        #endregion Property: StaticContentBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: StaticContentChangeBase(StaticContentChange staticContentChange)
        /// <summary>
        /// Creates a base of the given static content change.
        /// </summary>
        /// <param name="staticContentChange">The static content change to create a base of.</param>
        protected StaticContentChangeBase(StaticContentChange staticContentChange)
            : base(staticContentChange)
        {
        }
        #endregion Constructor: StaticContentChangeBase(StaticContentChange staticContentChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: StaticContentChangeBase

    #region Class: DynamicContentBase
    /// <summary>
    /// A base of dynamic content.
    /// </summary>
    public abstract class DynamicContentBase : ContentBase
    {

        #region Properties and Fields

        #region Property: DynamicContent
        /// <summary>
        /// Gets the dynamic content of which this is a dynamic content base.
        /// </summary>
        protected internal DynamicContent DynamicContent
        {
            get
            {
                return this.Node as DynamicContent;
            }
        }
        #endregion Property: DynamicContent

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: DynamicContentBase(DynamicContent dynamicContent)
        /// <summary>
        /// Creates a new dynamic content base from the given dynamic content.
        /// </summary>
        /// <param name="dynamicContent">The dynamic content to create a dynamic content base from.</param>
        protected DynamicContentBase(DynamicContent dynamicContent)
            : base(dynamicContent)
        {
        }
        #endregion Constructor: DynamicContentBase(DynamicContent dynamicContent)

        #endregion Method Group: Constructors

    }
    #endregion Class: DynamicContentBase

    #region Class: DynamicContentValuedBase
    /// <summary>
    /// A base of valued dynamic content.
    /// </summary>
    public abstract class DynamicContentValuedBase : ContentValuedBase
    {

        #region Properties and Fields

        #region Property: DynamicContentValued
        /// <summary>
        /// Gets the valued dynamic content of which this is a valued dynamic content base.
        /// </summary>
        protected internal DynamicContentValued DynamicContentValued
        {
            get
            {
                return this.NodeValued as DynamicContentValued;
            }
        }
        #endregion Property: DynamicContentValued

        #region Property: DynamicContentBase
        /// <summary>
        /// Gets the dynamic content base.
        /// </summary>
        public DynamicContentBase DynamicContentBase
        {
            get
            {
                return this.NodeBase as DynamicContentBase;
            }
        }
        #endregion Property: DynamicContentBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: DynamicContentValuedBase(DynamicContentValued dynamicContentValued)
        /// <summary>
        /// Create a valued dynamic content base from the given valued dynamic content.
        /// </summary>
        /// <param name="dynamicContentValued">The valued dynamic content to create a valued dynamic content base from.</param>
        protected DynamicContentValuedBase(DynamicContentValued dynamicContentValued)
            : base(dynamicContentValued)
        {
        }
        #endregion Constructor: DynamicContentValuedBase(DynamicContentValued dynamicContentValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: DynamicContentValuedBase

    #region Class: DynamicContentConditionBase
    /// <summary>
    /// A condition on dynamic content.
    /// </summary>
    public abstract class DynamicContentConditionBase : ContentConditionBase
    {

        #region Properties and Fields

        #region Property: DynamicContentCondition
        /// <summary>
        /// Gets the dynamic content condition of which this is a dynamic content condition base.
        /// </summary>
        protected internal DynamicContentCondition DynamicContentCondition
        {
            get
            {
                return this.Condition as DynamicContentCondition;
            }
        }
        #endregion Property: DynamicContentCondition

        #region Property: DynamicContentBase
        /// <summary>
        /// Gets the dynamic content base of which this is a dynamic content condition base.
        /// </summary>
        public DynamicContentBase DynamicContentBase
        {
            get
            {
                return this.NodeBase as DynamicContentBase;
            }
        }
        #endregion Property: DynamicContentBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: DynamicContentConditionBase(DynamicContentCondition dynamicContentCondition)
        /// <summary>
        /// Creates a base of the given dynamic content condition.
        /// </summary>
        /// <param name="dynamicContentCondition">The dynamic content condition to create a base of.</param>
        protected DynamicContentConditionBase(DynamicContentCondition dynamicContentCondition)
            : base(dynamicContentCondition)
        {
        }
        #endregion Constructor: DynamicContentConditionBase(DynamicContentCondition dynamicContentCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: DynamicContentConditionBase

    #region Class: DynamicContentChangeBase
    /// <summary>
    /// A change on dynamic content.
    /// </summary>
    public abstract class DynamicContentChangeBase : ContentChangeBase
    {

        #region Properties and Fields

        #region Property: DynamicContentChange
        /// <summary>
        /// Gets the dynamic content change of which this is a dynamic content change base.
        /// </summary>
        protected internal DynamicContentChange DynamicContentChange
        {
            get
            {
                return this.Change as DynamicContentChange;
            }
        }
        #endregion Property: DynamicContentChange

        #region Property: DynamicContentBase
        /// <summary>
        /// Gets the affected dynamic content base.
        /// </summary>
        public DynamicContentBase DynamicContentBase
        {
            get
            {
                return this.NodeBase as DynamicContentBase;
            }
        }
        #endregion Property: DynamicContentBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: DynamicContentChangeBase(DynamicContentChange dynamicContentChange)
        /// <summary>
        /// Creates a base of the given dynamic content change.
        /// </summary>
        /// <param name="dynamicContentChange">The dynamic content change to create a base of.</param>
        protected DynamicContentChangeBase(DynamicContentChange dynamicContentChange)
            : base(dynamicContentChange)
        {
        }
        #endregion Constructor: DynamicContentChangeBase(DynamicContentChange dynamicContentChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: DynamicContentChangeBase

}