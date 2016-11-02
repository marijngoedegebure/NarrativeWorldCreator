/**************************************************************************
 * 
 * ContentInstance.cs
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
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Components;
using SemanticsEngine.Interfaces;

namespace GameSemanticsEngine.GameContent
{

    #region Class: ContentInstance
    /// <summary>
    /// An instance of content.
    /// </summary>
    public abstract class ContentInstance : NodeInstance
    {

        #region Properties and Fields

        #region Property: ContentBase
        /// <summary>
        /// Gets the content base of which this is a content instance.
        /// </summary>
        public ContentBase ContentBase
        {
            get
            {
                if (this.ContentValuedBase != null)
                    return this.ContentValuedBase.ContentBase;
                return null;
            }
        }
        #endregion Property: ContentBase

        #region Property: ContentValuedBase
        /// <summary>
        /// Gets the valued content base of which this is a content instance.
        /// </summary>
        public ContentValuedBase ContentValuedBase
        {
            get
            {
                return this.Base as ContentValuedBase;
            }
        }
        #endregion Property: ContentValuedBase

        #region Property: File
        /// <summary>
        /// Gets the name of the file.
        /// </summary>
        public String File
        {
            get
            {
                if (this.ContentBase != null)
                    return GetProperty<String>("File", this.ContentBase.File);

                return String.Empty;
            }
            protected set
            {
                if (this.File != value)
                    SetProperty("File", value);
            }
        }
        #endregion Property: File

        #region Property: PredicateTypes
        /// <summary>
        /// Gets the predicate types.
        /// </summary>
        public ReadOnlyCollection<PredicateTypeBase> PredicateTypes
        {
            get
            {
                if (this.ContentBase != null)
                {
                    // Replace the bases by modifications
                    List<PredicateTypeBase> predicateTypes = new List<PredicateTypeBase>(this.ContentBase.PredicateTypes);
                    return ReplaceByModificationsBase<PredicateTypeBase>("PredicateTypes", predicateTypes, null).AsReadOnly();
                }

                return new List<PredicateTypeBase>(0).AsReadOnly();
            }
        }
        #endregion Property: PredicateTypes

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ContentInstance(ContentValuedBase contentValuedBase)
        /// <summary>
        /// Creates a new content instance from the given valued content base.
        /// </summary>
        /// <param name="contentValuedBase">The valued content base to create the content instance from.</param>
        protected ContentInstance(ContentValuedBase contentValuedBase)
            : base(contentValuedBase)
        {
        }
        #endregion Constructor: ContentInstance(ContentValuedBase contentValuedBase)

        #region Constructor: ContentInstance(ContentInstance contentInstance)
        /// <summary>
        /// Clones a content instance.
        /// </summary>
        /// <param name="contentInstance">The content instance to clone.</param>
        protected ContentInstance(ContentInstance contentInstance)
            : base(contentInstance)
        {
        }
        #endregion Constructor: ContentInstance(ContentInstance contentInstance)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddPredicateType(PredicateTypeBase predicateTypeBase)
        /// <summary>
        /// Adds a predicate type.
        /// </summary>
        /// <param name="predicateTypeBase">The predicate type to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        protected internal Message AddPredicateType(PredicateTypeBase predicateTypeBase)
        {
            if (predicateTypeBase != null)
            {
                // If this predicate type is already available, there is no use to add it
                if (this.PredicateTypes.Contains(predicateTypeBase))
                    return Message.RelationExistsAlready;

                // Add the predicate type
                AddToArrayProperty<PredicateTypeBase>("PredicateTypes", predicateTypeBase);

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddPredicateType(PredicateTypeBase predicateTypeBase)

        #region Method: RemovePredicateType(PredicateTypeBase predicateTypeBase)
        /// <summary>
        /// Removes a predicate type.
        /// </summary>
        /// <param name="predicateTypeBase">The predicate type to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        protected internal Message RemovePredicateType(PredicateTypeBase predicateTypeBase)
        {
            if (predicateTypeBase != null)
            {
                if (this.PredicateTypes.Contains(predicateTypeBase))
                {
                    // Remove the predicate type
                    RemoveFromArrayProperty<PredicateTypeBase>("PredicateTypes", predicateTypeBase);
                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemovePredicateType(PredicateTypeBase predicateTypeBase)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the given condition satisfies the content instance.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the content instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the condition satisfies the content instance.</returns>
        public new virtual bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            // Check whether the base satisfies the condition
            if (conditionBase != null && base.Satisfies(conditionBase, iVariableInstanceHolder))
            {
                ContentConditionBase contentConditionBase = conditionBase as ContentConditionBase;
                if (contentConditionBase != null)
                {
                    // Check whether all the properties have the correct values
                    if (contentConditionBase.File == null || contentConditionBase.File.Equals(this.File))
                    {
                        // Predicate types
                        foreach (PredicateTypeBase predicateTypeBase in contentConditionBase.PredicateTypes)
                        {
                            if (!this.PredicateTypes.Contains(predicateTypeBase))
                                return false;
                        }

                        return true;
                    }
                }
                else
                    return true;
            }

            return false;
        }
        #endregion Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the given change to the content instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the content instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        protected internal new virtual bool Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (changeBase != null && base.Apply(changeBase, iVariableInstanceHolder))
            {
                ContentChangeBase contentChangeBase = changeBase as ContentChangeBase;
                if (contentChangeBase != null)
                {
                    // Apply all changes
                    if (contentChangeBase.File != null)
                        this.File = contentChangeBase.File;

                    // Add and remove the predicate types
                    foreach (PredicateTypeBase predicateTypeBase in contentChangeBase.PredicateTypesToAdd)
                        AddPredicateType(predicateTypeBase);
                    foreach (PredicateTypeBase predicateTypeBase in contentChangeBase.PredicateTypesToRemove)
                        RemovePredicateType(predicateTypeBase);
                }
                return true;
            }

            return false;
        }
        #endregion Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: ContentInstance

    #region Class: StaticContentInstance
    /// <summary>
    /// An instance of static content.
    /// </summary>
    public abstract class StaticContentInstance : ContentInstance
    {

        #region Properties and Fields

        #region Property: StaticContentBase
        /// <summary>
        /// Gets the static content base of which this is a static content instance.
        /// </summary>
        public StaticContentBase StaticContentBase
        {
            get
            {
                if (this.StaticContentValuedBase != null)
                    return this.StaticContentValuedBase.StaticContentBase;
                return null;
            }
        }
        #endregion Property: StaticContentBase

        #region Property: StaticContentValuedBase
        /// <summary>
        /// Gets the valued static content base of which this is a static content instance.
        /// </summary>
        public StaticContentValuedBase StaticContentValuedBase
        {
            get
            {
                return this.Base as StaticContentValuedBase;
            }
        }
        #endregion Property: StaticContentValuedBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: StaticContentInstance(StaticContentValuedBase staticContentValuedBase)
        /// <summary>
        /// Creates a new static content instance from the given valued static content base.
        /// </summary>
        /// <param name="staticContentValuedBase">The valued static content base to create the static content instance from.</param>
        protected StaticContentInstance(StaticContentValuedBase staticContentValuedBase)
            : base(staticContentValuedBase)
        {
        }
        #endregion Constructor: StaticContentInstance(StaticContentValuedBase staticContentValuedBase)

        #region Constructor: StaticContentInstance(StaticContentInstance staticContentInstance)
        /// <summary>
        /// Clones a static content instance.
        /// </summary>
        /// <param name="staticContentInstance">The static content instance to clone.</param>
        protected StaticContentInstance(StaticContentInstance staticContentInstance)
            : base(staticContentInstance)
        {
        }
        #endregion Constructor: StaticContentInstance(StaticContentInstance staticContentInstance)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the static content instance satisfies the given condition.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the static content instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the static content instance is satisfies the given condition.</returns>
        public override bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            // Check whether the base satisfies the condition
            if (conditionBase != null)
                return base.Satisfies(conditionBase, iVariableInstanceHolder);
            return false;
        }
        #endregion Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the change to the static content instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the static content instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        protected internal override bool Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (changeBase != null)
                return base.Apply(changeBase, iVariableInstanceHolder);
            return false;
        }
        #endregion Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: StaticContentInstance

    #region Class: DynamicContentInstance
    /// <summary>
    /// An instance of dynamic content.
    /// </summary>
    public abstract class DynamicContentInstance : ContentInstance
    {

        #region Properties and Fields

        #region Property: DynamicContentBase
        /// <summary>
        /// Gets the dynamic content base of which this is a dynamic content instance.
        /// </summary>
        public DynamicContentBase DynamicContentBase
        {
            get
            {
                if (this.DynamicContentValuedBase != null)
                    return this.DynamicContentValuedBase.DynamicContentBase;
                return null;
            }
        }
        #endregion Property: DynamicContentBase

        #region Property: DynamicContentValuedBase
        /// <summary>
        /// Gets the valued dynamic content base of which this is a dynamic content instance.
        /// </summary>
        public DynamicContentValuedBase DynamicContentValuedBase
        {
            get
            {
                return this.Base as DynamicContentValuedBase;
            }
        }
        #endregion Property: DynamicContentValuedBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: DynamicContentInstance(DynamicContentValuedBase dynamicContentValuedBase)
        /// <summary>
        /// Creates a new dynamic content instance from the given valued dynamic content base.
        /// </summary>
        /// <param name="dynamicContentValuedBase">The valued dynamic content base to create the dynamic content instance from.</param>
        protected DynamicContentInstance(DynamicContentValuedBase dynamicContentValuedBase)
            : base(dynamicContentValuedBase)
        {
        }
        #endregion Constructor: DynamicContentInstance(DynamicContentValuedBase dynamicContentValuedBase)

        #region Constructor: DynamicContentInstance(DynamicContentInstance dynamicContentInstance)
        /// <summary>
        /// Clones a dynamic content instance.
        /// </summary>
        /// <param name="dynamicContentInstance">The dynamic content instance to clone.</param>
        protected DynamicContentInstance(DynamicContentInstance dynamicContentInstance)
            : base(dynamicContentInstance)
        {
        }
        #endregion Constructor: DynamicContentInstance(DynamicContentInstance dynamicContentInstance)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the dynamic content instance satisfies the given condition.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the dynamic content instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the dynamic content instance is satisfies the given condition.</returns>
        public override bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            // Check whether the base satisfies the condition
            if (conditionBase != null)
                return base.Satisfies(conditionBase, iVariableInstanceHolder);
            return false;
        }
        #endregion Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the change to the dynamic content instance.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <param name="changeBase">The change to apply to the dynamic content instance.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        protected internal override bool Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (changeBase != null)
                return base.Apply(changeBase, iVariableInstanceHolder);
            return false;
        }
        #endregion Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: DynamicContentInstance

    #region Class: CustomContentInstance
    /// <summary>
    /// An instance of custom content.
    /// </summary>
    public class CustomContentInstance : ContentInstance
    {

        #region Properties and Fields

        #region Property: CustomContent
		/// <summary>
		/// The custom content.
		/// </summary>
		private object customContent;
		
		/// <summary>
		/// The custom content.
		/// </summary>
		public object CustomContent
		{
			get
			{
				return customContent;
			}
			set 
			{
				customContent = value;
                NotifyPropertyChanged("CustomContent");
			}
		}
		#endregion Property: CustomContent
		
        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: CustomContentInstance(object customContent)
        /// <summary>
        /// Creates a new custom content instance for the given custom content.
        /// </summary>
        /// <param name="customContent">The custom content to create the custom content instance for.</param>
        public CustomContentInstance(object customContent)
            : base(null as ContentInstance)
        {
            this.customContent = customContent;
        }
        #endregion Constructor: CustomContentInstance(object customContent)

        #region Constructor: CustomContentInstance(CustomContentInstance customContentInstance)
        /// <summary>
        /// Clones a custom content instance.
        /// </summary>
        /// <param name="customContentInstance">The custom content instance to clone.</param>
        public CustomContentInstance(CustomContentInstance customContentInstance)
            : base(customContentInstance)
        {
            if (customContentInstance != null)
                this.customContent = customContentInstance.CustomContent;
        }
        #endregion Constructor: CustomContentInstance(CustomContentInstance customContentInstance)

        #endregion Method Group: Constructors

    }
    #endregion Class: CustomContentInstance

}