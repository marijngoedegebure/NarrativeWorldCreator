/**************************************************************************
 * 
 * RelationshipInstance.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.Collections.Generic;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Entities;
using SemanticsEngine.Interfaces;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: RelationshipInstance
    /// <summary>
    /// An instance of a specific relationship.
    /// </summary>
    public sealed class RelationshipInstance : Instance, IVariableInstanceHolder
    {

        #region Properties and Fields

        #region Property: RelationshipBase
        /// <summary>
        /// Gets the relationship base of which this is a relationship instance.
        /// </summary>
        public RelationshipBase RelationshipBase
        {
            get
            {
                return this.Base as RelationshipBase;
            }
        }
        #endregion Property: RelationshipBase

        #region Property: Source
        /// <summary>
        /// Gets the source of the relationship.
        /// </summary>
        public EntityInstance Source
        {
            get
            {
                return GetProperty<EntityInstance>("Source", null);
            }
            private set
            {
                if (this.Source != value)
                    SetProperty("Source", value);
            }
        }
        #endregion Property: Source

        #region Property: SourceSpace
        /// <summary>
        /// Gets the space of the source that is used in the relationship; only valid for physical objects.
        /// </summary>
        public SpaceInstance SourceSpace
        {
            get
            {
                return GetProperty<SpaceInstance>("SourceSpace", null);
            }
            private set
            {
                if (this.SourceSpace != value)
                    SetProperty("SourceSpace", value);
            }
        }
        #endregion Property: SourceSpace

        #region Property: RelationshipType
        /// <summary>
        /// Gets the relationship type of the relationship.
        /// </summary>
        public RelationshipTypeBase RelationshipType
        {
            get
            {
                if (this.RelationshipBase != null)
                    return this.RelationshipBase.RelationshipType;
                return null;
            }
        }
        #endregion Property: RelationshipType

        #region Property: Target
        /// <summary>
        /// Gets the target of the relationship.
        /// </summary>
        public EntityInstance Target
        {
            get
            {
                return GetProperty<EntityInstance>("Target", null);
            }
            private set
            {
                if (this.Target != value)
                    SetProperty("Target", value);
            }
        }
        #endregion Property: Target

        #region Property: TargetSpace
        /// <summary>
        /// Gets the space of the target that is used in the relationship; only valid for physical objects.
        /// </summary>
        public SpaceInstance TargetSpace
        {
            get
            {
                return GetProperty<SpaceInstance>("TargetSpace", null);
            }
            private set
            {
                if (this.TargetSpace != value)
                    SetProperty("TargetSpace", value);
            }
        }
        #endregion Property: TargetSpace

        #region Property: Attributes
        /// <summary>
        /// The attributes.
        /// </summary>
        private AttributeInstance[] mutableAttributes = null;

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        public IEnumerable<AttributeInstance> Attributes
        {
            get
            {
                if (mutableAttributes == null)
                {
                    List<AttributeInstance> attributeInstances = new List<AttributeInstance>();
                    if (this.RelationshipBase != null)
                    {
                        // Create instances from the base valued attributes
                        foreach (AttributeValuedBase attributeValuedBase in this.RelationshipBase.Attributes)
                        {
                            if (attributeValuedBase.AttributeBase.AttributeType != AttributeType.Constant)
                                attributeInstances.Add(InstanceManager.Current.Create<AttributeInstance>(attributeValuedBase));
                        }
                    }
                    mutableAttributes = attributeInstances.ToArray();
                }

                foreach (AttributeInstance attributeInstance in mutableAttributes)
                    yield return attributeInstance;

                if (this.RelationshipBase != null)
                {
                    foreach (AttributeInstance attributeInstance in this.RelationshipBase.ConstantAttributes)
                        yield return attributeInstance;
                }
            }
        }
        #endregion Property: Attributes

        #region Field: variableReferenceInstanceHolder
        /// <summary>
        /// The variable instance holder.
        /// </summary>
        private VariableReferenceInstanceHolder variableReferenceInstanceHolder = new VariableReferenceInstanceHolder();
        #endregion Field: variableReferenceInstanceHolder

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: RelationshipInstance(RelationshipBase relationshipBase)
        /// <summary>
        /// Creates a new relationship instance from the given relationship base.
        /// </summary>
        /// <param name="relationshipBase">The relationship base to create the relationship instance from.</param>
        internal RelationshipInstance(RelationshipBase relationshipBase)
            : base(relationshipBase)
        {
        }
        #endregion Constructor: RelationshipInstance(RelationshipBase relationshipBase)

        #region Constructor: RelationshipInstance(RelationshipBase relationshipBase, EntityInstance source, EntityInstance target)
        /// <summary>
        /// Creates a new relationship instance from the given relationship base for the given source and target.
        /// </summary>
        /// <param name="relationshipBase">The relationship base to create the relationship instance from.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        internal RelationshipInstance(RelationshipBase relationshipBase, EntityInstance source, EntityInstance target)
            : base(relationshipBase)
        {
            this.Source = source;
            this.Target = target;
        }
        #endregion Constructor: RelationshipInstance(RelationshipBase relationshipBase, EntityInstance source, EntityInstance target)

        #region Constructor: RelationshipInstance(RelationshipBase relationshipBase, PhysicalObjectInstance source, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        /// <summary>
        /// Creates a new relationship instance from the given relationship base for the given physical source and target, and their spaces.
        /// </summary>
        /// <param name="relationshipBase">The relationship base to create the relationship instance from.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="sourceSpace">The space of the source.</param>
        /// <param name="targetSpace">The space of the target.</param>
        internal RelationshipInstance(RelationshipBase relationshipBase, PhysicalObjectInstance source, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)
            : this(relationshipBase)
        {
            this.SourceSpace = sourceSpace;
            this.TargetSpace = targetSpace;
        }
        #endregion Constructor: RelationshipInstance(RelationshipBase relationshipBase, PhysicalObjectInstance source, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)

        #region Constructor: RelationshipInstance(RelationshipInstance relationshipInstance)
        /// <summary>
        /// Clones a relationship instance.
        /// </summary>
        /// <param name="relationshipInstance">The relationship instance to clone.</param>
        internal RelationshipInstance(RelationshipInstance relationshipInstance)
            : base(relationshipInstance)
        {
            if (relationshipInstance != null)
            {
                this.Source = relationshipInstance.Source;
                this.SourceSpace = relationshipInstance.SourceSpace;
                this.Target = relationshipInstance.Target;
                this.TargetSpace = relationshipInstance.TargetSpace;
                foreach (AttributeInstance attributeInstance in relationshipInstance.Attributes)
                    AddAttribute(new AttributeInstance(attributeInstance));
                this.variableReferenceInstanceHolder = new VariableReferenceInstanceHolder(relationshipInstance.variableReferenceInstanceHolder);
            }
        }
        #endregion Constructor: RelationshipInstance(RelationshipInstance relationshipInstance)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddAttribute(AttributeInstance attributeInstance)
        /// <summary>
        /// Adds an attribute instance.
        /// </summary>
        /// <param name="attributeInstance">The attribute instance to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        protected internal Message AddAttribute(AttributeInstance attributeInstance)
        {
            if (attributeInstance != null && attributeInstance.AttributeValuedBase != null)
            {
                // If the attribute instance is already available in all attributes, there is no use to add it
                if (HasAttribute(attributeInstance.AttributeValuedBase.AttributeBase))
                    return Message.RelationExistsAlready;

                // Add the attribute instance to the attributes
                Utils.AddToArray<AttributeInstance>(ref this.mutableAttributes, attributeInstance);
                NotifyPropertyChanged("Attributes");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddAttribute(AttributeInstance attributeInstance)

        #region Method: RemoveAttribute(AttributeInstance attributeInstance)
        /// <summary>
        /// Removes an attribute instance.
        /// </summary>
        /// <param name="attributeInstance">The attribute instance to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        protected internal Message RemoveAttribute(AttributeInstance attributeInstance)
        {
            if (attributeInstance != null && attributeInstance.AttributeValuedBase != null)
            {
                if (HasAttribute(attributeInstance.AttributeValuedBase.AttributeBase))
                {
                    // Remove the attribute instance
                    Utils.RemoveFromArray<AttributeInstance>(ref this.mutableAttributes, attributeInstance);
                    NotifyPropertyChanged("Attributes");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveAttribute(AttributeInstance attributeInstance)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasAttribute(AttributeBase attributeBase)
        /// <summary>
        /// Checks if this relationship instance has the given attribute.
        /// </summary>
        /// <param name="attributeBase">The attribute to check.</param>
        /// <returns>Returns true when the relationship instance has the attribute.</returns>
        public bool HasAttribute(AttributeBase attributeBase)
        {
            if (attributeBase != null)
            {
                foreach (AttributeInstance attributeInstance in this.Attributes)
                {
                    if (attributeInstance.AttributeValuedBase != null && attributeBase.Equals(attributeInstance.AttributeValuedBase.AttributeBase))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasAttribute(AttributeBase attributeBase)

        #region Method: Satisfies(RelationshipConditionBase relationshipConditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the given condition satisfies the relationship instance.
        /// </summary>
        /// <param name="relationshipConditionBase">The condition to compare to the relationship instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the condition satisfies the relationship instance.</returns>
        public bool Satisfies(RelationshipConditionBase relationshipConditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (relationshipConditionBase != null)
            {
                if ((relationshipConditionBase.RelationshipType == null || relationshipConditionBase.RelationshipType.Equals(this.RelationshipType)) &&
                    (relationshipConditionBase.Source == null || (this.Source != null && this.Source.EntityBase != null && this.Source.EntityBase.IsNodeOf(relationshipConditionBase.Source))) &&
                    (relationshipConditionBase.Target == null || (this.Target != null && this.Target.EntityBase != null && this.Target.EntityBase.IsNodeOf(relationshipConditionBase.Target))) &&
                    (relationshipConditionBase.SourceSpace == null || (this.SourceSpace != null && this.SourceSpace.SpaceBase != null && this.SourceSpace.SpaceBase.IsNodeOf(relationshipConditionBase.SourceSpace))) &&
                    (relationshipConditionBase.TargetSpace == null || (this.TargetSpace != null && this.TargetSpace.SpaceBase != null && this.TargetSpace.SpaceBase.IsNodeOf(relationshipConditionBase.TargetSpace))))
                {
                    // Check whether all attributes are there
                    foreach (AttributeConditionBase attributeConditionBase in relationshipConditionBase.Attributes)
                    {
                        bool satisfied = false;
                        foreach (AttributeInstance attributeInstance in this.Attributes)
                        {
                            if (attributeInstance.Satisfies(attributeConditionBase, iVariableInstanceHolder))
                            {
                                satisfied = true;
                                break;
                            }
                        }
                        if (!satisfied)
                            return false;
                    }

                    return true;
                }
            }
            return false;
        }
        #endregion Method: Satisfies(RelationshipConditionBase relationshipConditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the given change to the relationship instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the relationship instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        internal bool Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (changeBase != null)
            {
                // Relationship change
                RelationshipChangeBase relationshipChangeBase = changeBase as RelationshipChangeBase;
                if (relationshipChangeBase != null)
                {
                    // Check the relationship type
                    if (relationshipChangeBase.RelationshipType == null || relationshipChangeBase.RelationshipType.Equals(this.RelationshipType))
                    {
                        // Change the attributes
                        foreach (AttributeChangeBase attributeChangeBase in relationshipChangeBase.Attributes)
                        {
                            foreach (AttributeInstance attributeInstance in this.Attributes)
                                attributeInstance.Apply(attributeChangeBase, iVariableInstanceHolder);
                        }
                    }

                    return true;
                }

                // Attribute change
                AttributeChangeBase attributeChange = changeBase as AttributeChangeBase;
                if (attributeChange != null)
                {
                    foreach (AttributeInstance attributeInstance in this.Attributes)
                        attributeInstance.Apply(attributeChange, iVariableInstanceHolder);
                    return true;
                }
            }
            return false;
        }
        #endregion Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: GetVariable(VariableBase variableBase)
        /// <summary>
        /// Get the variable instance for the given variable base.
        /// </summary>
        /// <param name="variableBase">The variable base to get the variable instance of.</param>
        /// <returns>The variable instance of the variable base.</returns>
        public VariableInstance GetVariable(VariableBase variableBase)
        {
            return this.variableReferenceInstanceHolder.GetVariable(variableBase, this);
        }
        #endregion Method: GetVariable(VariableBase variableBase)

        #region Method: GetManualInput()
        /// <summary>
        /// Get the manual input.
        /// </summary>
        /// <returns>The manual input.</returns>
        public Dictionary<string, object> GetManualInput()
        {
            return null;
        }
        #endregion Method: GetManualInput()

        #region Method: GetActor()
        /// <summary>
        /// Get the actor.
        /// </summary>
        /// <returns>The actor.</returns>
        public EntityInstance GetActor()
        {
            return this.Source;
        }
        #endregion Method: GetActor()

        #region Method: GetTarget()
        /// <summary>
        /// Get the target.
        /// </summary>
        /// <returns>The target.</returns>
        public EntityInstance GetTarget()
        {
            return this.Target;
        }
        #endregion Method: GetTarget()

        #region Method: GetArtifact()
        /// <summary>
        /// Get the artifact.
        /// </summary>
        /// <returns>The artifact.</returns>
        public EntityInstance GetArtifact()
        {
            return null;
        }
        #endregion Method: GetArtifact()
        
        #endregion Method Group: Other

    }
    #endregion Class: RelationshipInstance

}