/**************************************************************************
 * 
 * PhysicalObjectInstance.cs
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
using Common;
using Common.MathParser;
using Common.Shapes;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Components;
using SemanticsEngine.Interfaces;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Entities
{

    #region TODO Class: PhysicalObjectInstance
    /// <summary>
    /// An instance of a physical object.
    /// </summary>
    public abstract class PhysicalObjectInstance : PhysicalEntityInstance
    {

        #region Events, Properties, and Fields

        #region Events: SpaceHandler
        /// <summary>
        /// A handler for added or removed spaces.
        /// </summary>
        /// <param name="sender">The physical object instance the space was added to or removed from.</param>
        /// <param name="space">The added or removed space.</param>
        public delegate void SpaceHandler(PhysicalObjectInstance sender, SpaceInstance space);

        /// <summary>
        /// An event to indicate an added space.
        /// </summary>
        public event SpaceHandler SpaceAdded;

        /// <summary>
        /// An event to indicate a removed space.
        /// </summary>
        public event SpaceHandler SpaceRemoved;
        #endregion Events: SpaceHandler

        #region Event: RotationChanged
        /// <summary>
        /// A handler for a changed rotation.
        /// </summary>
        /// <param name="sender">The physical object instance of which the rotation was changed.</param>
        /// <param name="rotation">The new rotation.</param>
        public delegate void RotationHandler(PhysicalObjectInstance sender, Quaternion rotation);

        /// <summary>
        /// Invoked when the rotation of the instance changes.
        /// </summary>
        public event RotationHandler RotationChanged;
        #endregion Event: RotationChanged

        #region Event: ScaleChanged
        /// <summary>
        /// A handler for a changed scale.
        /// </summary>
        /// <param name="sender">The physical object instance of which the scale was changed.</param>
        /// <param name="position">The new scale.</param>
        public delegate void ScaleHandler(PhysicalObjectInstance sender, Vec3 scale);

        /// <summary>
        /// Invoked when the scale of the instance changes.
        /// </summary>
        public event ScaleHandler ScaleChanged;
        #endregion Event: ScaleChanged

        #region Property: PhysicalObjectBase
        /// <summary>
        /// Gets the physical object base of which this is a physical object instance.
        /// </summary>
        public PhysicalObjectBase PhysicalObjectBase
        {
            get
            {
                return this.NodeBase as PhysicalObjectBase;
            }
        }
        #endregion Property: PhysicalObjectBase

        #region Property: Spaces
        /// <summary>
        /// The spaces of the physical object.
        /// </summary>
        private SpaceInstance[] spaces = null;
        
        /// <summary>
        /// Gets the spaces of the physical object.
        /// </summary>
        public ReadOnlyCollection<SpaceInstance> Spaces
        {
            get
            {
                if (spaces != null)
                    return new ReadOnlyCollection<SpaceInstance>(spaces);

                return new List<SpaceInstance>(0).AsReadOnly();
            }
        }
        #endregion Property: Spaces

        #region Property: DefaultSpace
        /// <summary>
        /// Gets or sets the default space.
        /// </summary>
        public SpaceInstance DefaultSpace
        {
            get
            {
                return GetProperty<SpaceInstance>("DefaultSpace");
            }
            set
            {
                if (value == null || !value.Equals(this.DefaultSpace))
                    SetProperty("DefaultSpace", value);
            }
        }
        #endregion Property: DefaultSpace

        #region Property: SpaceItems
        /// <summary>
        /// Gets the items in the spaces of this instance.
        /// </summary>
        public ReadOnlyCollection<TangibleObjectInstance> SpaceItems
        {
            get
            {
                List<TangibleObjectInstance> items = new List<TangibleObjectInstance>();
                foreach (SpaceInstance spaceInstance in this.Spaces)
                    items.AddRange(spaceInstance.Items);
                return items.AsReadOnly();
            }
        }
        #endregion Property: SpaceItems

        #region Property: AccessibleSpaceItems
        /// <summary>
        /// Gets the items in the accessible spaces.
        /// </summary>
        public ReadOnlyCollection<TangibleObjectInstance> AccessibleSpaceItems
        {
            get
            {
                List<TangibleObjectInstance> items = new List<TangibleObjectInstance>();
                foreach (SpaceInstance spaceInstance in this.Spaces)
                {
                    if (spaceInstance.SpaceType == SpaceType.Open)
                        items.AddRange(spaceInstance.Items);
                }
                return items.AsReadOnly();
            }
        }
        #endregion Property: AccessibleSpaceItems

        #region Property: SpaceTangibleMatter
        /// <summary>
        /// Gets the tangible matter in the spaces of this instance.
        /// </summary>
        public ReadOnlyCollection<MatterInstance> SpaceTangibleMatter
        {
            get
            {
                List<MatterInstance> tangibleMatter = new List<MatterInstance>();
                foreach (SpaceInstance spaceInstance in this.Spaces)
                    tangibleMatter.AddRange(spaceInstance.TangibleMatter);
                return tangibleMatter.AsReadOnly();
            }
        }
        #endregion Property: SpaceTangibleMatter

        #region Property: AccessibleSpaceTangibleMatter
        /// <summary>
        /// Gets the tangible matter in the accessible spaces.
        /// </summary>
        public ReadOnlyCollection<MatterInstance> AccessibleSpaceTangibleMatter
        {
            get
            {
                List<MatterInstance> tangibleMatter = new List<MatterInstance>();
                foreach (SpaceInstance spaceInstance in this.Spaces)
                {
                    if (spaceInstance.SpaceType == SpaceType.Open)
                        tangibleMatter.AddRange(spaceInstance.TangibleMatter);
                }
                return tangibleMatter.AsReadOnly();
            }
        }
        #endregion Property: AccessibleSpaceTangibleMatter

        #region Property: Rotation
        /// <summary>
        /// The possible attribute instance that stores the rotation.
        /// </summary>
        private AttributeInstance rotationAttribute = null;

        /// <summary>
        /// The rotation.
        /// </summary>
        private Quaternion rotation = Quaternion.Identity;

        /// <summary>
        /// A handler for changes in the quaternion.
        /// </summary>
        private Quaternion.ValueChangedHandler rotationHandler;

        /// <summary>
        /// Indicates whether the rotation setting should be disabled.
        /// </summary>
        private bool ignoreRotationSet = false;

        /// <summary>
        /// Gets the rotation.
        /// </summary>
        public Quaternion Rotation
        {
            get
            {
                // If the rotation is stored as an attribute instance, set their current values
                if (rotationAttribute != null)
                {
                    Vec4 attributeRotation = ((VectorValueInstance)rotationAttribute.Value).Vector;
                    ignoreRotationSet = true;
                    rotation.X = attributeRotation.X;
                    rotation.Y = attributeRotation.Y;
                    rotation.Z = attributeRotation.Z;
                    ignoreRotationSet = false;
                }

                return rotation;
            }
            set
            {
                SetRotation(value);
            }
        }

        /// <summary>
        /// Notify of changes in the rotation quaternion.
        /// </summary>
        private void rotation_ValueChanged(Quaternion sender)
        {
            if (!ignoreRotationSet)
                SetRotation(rotation);
        }

        /// <summary>
        /// Notify of changes in the rotation attribute.
        /// </summary>
        private void rotationAttribute_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!ignoreRotationSet)
                SetRotation(this.Rotation);
        }
        #endregion Property: Rotation

        #region Property: Scale
        /// <summary>
        /// The possible attribute instance that stores the scale.
        /// </summary>
        private AttributeInstance scaleAttribute = null;

        /// <summary>
        /// The scale.
        /// </summary>
        private Vec3 scale = Vec3.One;

        /// <summary>
        /// A handler for changes in the scale.
        /// </summary>
        private Vec3.Vec3Handler scaleHandler;

        /// <summary>
        /// Indicates whether the scale setting should be disabled.
        /// </summary>
        private bool ignoreScaleSet = false;

        /// <summary>
        /// Gets the scale.
        /// </summary>
        public Vec3 Scale
        {
            get
            {
                // If the scale is stored as an attribute instance, set their current values
                if (scaleAttribute != null)
                {
                    Vec4 attributeScale = ((VectorValueInstance)scaleAttribute.Value).Vector;
                    ignoreScaleSet = true;
                    scale.X = attributeScale.X;
                    scale.Y = attributeScale.Y;
                    scale.Z = attributeScale.Z;
                    ignoreScaleSet = false;
                }

                return scale;
            }
            set
            {
                SetScale(value);
            }
        }

        /// <summary>
        /// Notify of changes in the scale vector.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        private void scale_ValueChanged(Vec3 vector)
        {
            if (!ignoreScaleSet)
                SetScale(scale);
        }

        /// <summary>
        /// Notify of changes in the scale attribute.
        /// </summary>
        private void scaleAttribute_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!ignoreScaleSet)
                SetScale(this.Scale);
        }
        #endregion Property: Scale

        #endregion Events, Properties, and Fields

        #region Method Group: Constructors

        #region Constructor: PhysicalObjectInstance(PhysicalObjectBase physicalObjectBase)
        /// <summary>
        /// Creates a new physical object instance from the given physical object base.
        /// </summary>
        /// <param name="physicalObjectBase">The physical object base to create the physical object instance from.</param>
        protected PhysicalObjectInstance(PhysicalObjectBase physicalObjectBase)
            : this(physicalObjectBase, SemanticsEngineSettings.DefaultCreateOptions)
        {
        }
        #endregion Constructor: PhysicalObjectInstance(PhysicalObjectBase physicalObjectBase)

        #region Constructor: PhysicalObjectInstance(PhysicalObjectBase physicalObjectBase, CreateOptions createOptions)
        /// <summary>
        /// Creates a new physical object instance from the given physical object base.
        /// </summary>
        /// <param name="physicalObjectBase">The physical object base to create the physical object instance from.</param>
        /// <param name="createOptions">The create options.</param>
        protected PhysicalObjectInstance(PhysicalObjectBase physicalObjectBase, CreateOptions createOptions)
            : base(physicalObjectBase)
        {
            SetRotationScaleFromAttribute();

            Create(createOptions);
        }
        #endregion Constructor: PhysicalObjectInstance(PhysicalObjectBase physicalObjectBase, CreateOptions createOptions)

        #region Constructor: PhysicalObjectInstance(PhysicalObjectValuedBase physicalObjectValuedBase, CreateOptions createOptions)
        /// <summary>
        /// Creates a new physical object instance from the given valued physical object base.
        /// </summary>
        /// <param name="physicalObjectValuedBase">The valued physical object base to create the physical object instance from.</param>
        /// <param name="createOptions">The create options.</param>
        protected PhysicalObjectInstance(PhysicalObjectValuedBase physicalObjectValuedBase, CreateOptions createOptions)
            : base(physicalObjectValuedBase)
        {
            SetRotationScaleFromAttribute();

            Create(createOptions);
        }
        #endregion Constructor: PhysicalObjectInstance(PhysicalObjectValuedBase physicalObjectValuedBase, CreateOptions createOptions)

        #region Constructor: PhysicalObjectInstance(PhysicalObjectInstance physicalObjectInstance)
        /// <summary>
        /// Clones a physical object instance.
        /// </summary>
        /// <param name="physicalObjectInstance">The physical object instance to clone.</param>
        protected PhysicalObjectInstance(PhysicalObjectInstance physicalObjectInstance)
            : base(physicalObjectInstance)
        {
            SetRotationScaleFromAttribute();

            if (physicalObjectInstance != null)
            {
                foreach (SpaceInstance spaceInstance in physicalObjectInstance.Spaces)
                    AddSpace(new SpaceInstance(spaceInstance));

                this.Rotation = new Quaternion(physicalObjectInstance.Rotation);
                this.Scale = new Vec3(physicalObjectInstance.Scale);
            }
        }
        #endregion Constructor: PhysicalObjectInstance(PhysicalObjectInstance physicalObjectInstance)

        #region Method: SetRotationScaleFromAttribute()
        /// <summary>
        /// Try to set the rotation and scale from an attribute.
        /// </summary>
        private void SetRotationScaleFromAttribute()
        {
            if (this.PhysicalObjectBase != null)
            {
                // Set handlers
                this.rotationHandler = new Quaternion.ValueChangedHandler(rotation_ValueChanged);
                this.rotation.ValueChanged += this.rotationHandler;
                this.scaleHandler = new Vec3.Vec3Handler(scale_ValueChanged);
                this.scale.ValueChanged += this.scaleHandler;

                // Set the possible rotation and scale attribute instances
                AttributeBase rotationAttribute = Utils.GetSpecialAttribute(SpecialAttributes.Rotation);
                if (rotationAttribute != null)
                {
                    foreach (AttributeInstance attributeInstance in this.Attributes)
                    {
                        if (attributeInstance.AttributeBase != null && rotationAttribute.Equals(attributeInstance.AttributeBase))
                        {
                            this.rotationAttribute = attributeInstance;
                            this.rotationAttribute.PropertyChanged += new PropertyChangedEventHandler(rotationAttribute_PropertyChanged);
                            break;
                        }
                    }
                }
                AttributeBase scaleAttribute = Utils.GetSpecialAttribute(SpecialAttributes.Scale);
                if (scaleAttribute != null)
                {
                    foreach (AttributeInstance attributeInstance in this.Attributes)
                    {
                        if (attributeInstance.AttributeBase != null && scaleAttribute.Equals(attributeInstance.AttributeBase))
                        {
                            this.scaleAttribute = attributeInstance;
                            this.scaleAttribute.PropertyChanged += new PropertyChangedEventHandler(scaleAttribute_PropertyChanged);
                            break;
                        }
                    }
                }
            }
        }
        #endregion Method: SetRotationScaleFromAttribute()

        #region Method: Create(CreateOptions createOptions)
        /// <summary>
        /// Create the spaces.
        /// </summary>
        /// <param name="createOptions">The create options.</param>
        private void Create(CreateOptions createOptions)
        {
            if (this.PhysicalObjectBase != null)
            {
                if (createOptions.Has(CreateOptions.Spaces) || createOptions.Has(CreateOptions.Items))
                {
                    // Create instances from the mandatory base spaces
                    foreach (SpaceValuedBase spaceValuedBase in this.PhysicalObjectBase.Spaces)
                    {
                        if (InstanceManager.IgnoreNecessity || spaceValuedBase.Necessity == Necessity.Mandatory)
                        {
                            // Add the space
                            SpaceInstance spaceInstance = InstanceManager.Current.Create(spaceValuedBase, createOptions);
                            AddSpace(spaceInstance);

                            // Set the default space
                            if (spaceValuedBase.Equals(this.PhysicalObjectBase.DefaultSpace))
                                this.DefaultSpace = spaceInstance;
                        }
                    }
                }
            }
        }
        #endregion Method: Create(CreateOptions createOptions)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddSpace(SpaceInstance spaceInstance)
        /// <summary>
        /// Adds a space instance.
        /// </summary>
        /// <param name="spaceInstance">The space instance to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddSpace(SpaceInstance spaceInstance)
        {
            if (spaceInstance != null)
            {
                // Check whether the space is already there
                if (this.Spaces.Contains(spaceInstance))
                    return Message.RelationExistsAlready;

                // Remove the space from its current physical object
                if (spaceInstance.PhysicalObject != null)
                    spaceInstance.PhysicalObject.RemoveSpace(spaceInstance);

                // Add the space
                Utils.AddToArray<SpaceInstance>(ref this.spaces, spaceInstance);
                NotifyPropertyChanged("Spaces");
                spaceInstance.PhysicalObject = this;

                // If no default space has been set, make it this one
                if (this.DefaultSpace == null)
                    this.DefaultSpace = spaceInstance;

                // Set the world if this has not been done before
                if (spaceInstance.World == null && this.World != null)
                    this.World.AddInstance(spaceInstance);

                // Invoke an event
                if (SpaceAdded != null)
                    SpaceAdded.Invoke(this, spaceInstance);

                // Notify the engine
                if (SemanticsEngine.Current != null)
                    SemanticsEngine.Current.HandleSpaceAdded(this, spaceInstance);

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddSpace(SpaceInstance spaceInstance)

        #region Method: RemoveSpace(SpaceInstance spaceInstance)
        /// <summary>
        /// Removes a space instance.
        /// </summary>
        /// <param name="spaceInstance">The space instance to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveSpace(SpaceInstance spaceInstance)
        {
            if (spaceInstance != null)
            {
                if (this.Spaces.Contains(spaceInstance))
                {
                    // Remove the space
                    Utils.RemoveFromArray<SpaceInstance>(ref this.spaces, spaceInstance);
                    NotifyPropertyChanged("Spaces");
                    spaceInstance.PhysicalObject = null;

                    // If this space is the default space, set another one
                    if (spaceInstance.Equals(this.DefaultSpace))
                    {
                        if (this.Spaces.Count > 0)
                            this.DefaultSpace = this.Spaces[0];
                        else
                            this.DefaultSpace = null;
                    }

                    // Invoke an event
                    if (SpaceRemoved != null)
                        SpaceRemoved.Invoke(this, spaceInstance);

                    // Notify the engine
                    if (SemanticsEngine.Current != null)
                        SemanticsEngine.Current.HandleSpaceRemoved(this, spaceInstance);

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveSpace(SpaceInstance spaceInstance)

        #region Method: AddRelationship(RelationshipTypeBase relationshipType, PhysicalObjectInstance source, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        /// <summary>
        /// Add a relationship of the given type between the given source and target.
        /// </summary>
        /// <param name="relationshipType">The relationship to create between source and target.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="sourceSpace">The space of the source.</param>
        /// <param name="targetSpace">The space of the target.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public static Message AddRelationship(RelationshipTypeBase relationshipType, PhysicalObjectInstance source, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        {
            if (relationshipType != null && source != null && target != null)
            {
                foreach (RelationshipBase relationshipBase in relationshipType.Relationships)
                {
                    if (AddRelationship(relationshipBase, source, target, sourceSpace, targetSpace) == Message.RelationSuccess)
                        return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: AddRelationship(RelationshipTypeBase relationshipType, PhysicalObjectInstance source, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)

        #region Method: RemoveRelationship(RelationshipTypeBase relationshipType, PhysicalObjectInstance source, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        /// <summary>
        /// Remove a relationship of the given type between the given source and target.
        /// </summary>
        /// <param name="relationshipType">The relationship to remove between source and target.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="sourceSpace">The space of the source.</param>
        /// <param name="targetSpace">The space of the target.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public static Message RemoveRelationship(RelationshipTypeBase relationshipType, PhysicalObjectInstance source, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        {
            if (relationshipType != null && source != null && target != null)
            {
                // Find the relationship instance and remove it
                foreach (RelationshipInstance relationshipInstance in source.RelationshipsAsSource)
                {
                    if (relationshipInstance.RelationshipBase != null && !relationshipInstance.RelationshipBase.IsExclusive &&
                        relationshipType.Equals(relationshipInstance.RelationshipType) &&
                        source.Equals(relationshipInstance.Source) &&
                        target.Equals(relationshipInstance.Target) &&
                        (sourceSpace == null || source.Spaces.Contains(sourceSpace)) &&
                        (targetSpace == null || target.Spaces.Contains(targetSpace)))
                    {
                        source.RemoveRelationship(relationshipInstance);
                        target.RemoveRelationship(relationshipInstance);
                        return Message.RelationSuccess;
                    }
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveRelationship(RelationshipTypeBase relationshipType, PhysicalObjectInstance source, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)

        #region Method: AddRelationship(RelationshipBase relationship, PhysicalObjectInstance source, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        /// <summary>
        /// Add a relationship of the given base between the given source and target.
        /// </summary>
        /// <param name="relationship">The relationship to create between source and target.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="sourceSpace">The space of the source.</param>
        /// <param name="targetSpace">The space of the target.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public static Message AddRelationship(RelationshipBase relationship, PhysicalObjectInstance source, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        {
            if (relationship != null && source != null && target != null)
            {
                // Check whether the relationship already exists
                foreach (RelationshipInstance relationshipInstance in source.RelationshipsAsSource)
                {
                    if (relationship.Equals(relationshipInstance.RelationshipBase) && target.Equals(relationshipInstance.Target) &&
                        ((sourceSpace == null && relationshipInstance.SourceSpace == null) || (sourceSpace != null && sourceSpace.Equals(relationshipInstance.SourceSpace))) &&
                        ((targetSpace == null && relationshipInstance.TargetSpace == null) || (targetSpace != null && targetSpace.Equals(relationshipInstance.TargetSpace))))
                        return Message.RelationExistsAlready;
                }

                // Check the cardinality
                switch (relationship.Cardinality)
                {
                    case Cardinality.ManyToMany:
                        // Nothing to check
                        break;

                    case Cardinality.ManyToOne:
                        // Check whether the source has the same relationship
                        // E.g.: each patient has one doctor
                        foreach (RelationshipInstance sourceRelationshipAsSource in source.RelationshipsAsSource)
                        {
                            if (relationship.Equals(sourceRelationshipAsSource.RelationshipBase))
                                return Message.RelationFail;
                        }

                        break;

                    case Cardinality.OneToMany:
                        // Check whether the target has the same relationship
                        // E.g.: One doctor has multiple patients
                        foreach (RelationshipInstance targetRelationshipAsTarget in target.RelationshipsAsTarget)
                        {
                            if (relationship.Equals(targetRelationshipAsTarget.RelationshipBase))
                                return Message.RelationFail;
                        }
                        break;

                    case Cardinality.OneToOne:
                        // Check whether any relationship exists
                        // E.g.: one doctor has one suit
                        foreach (RelationshipInstance sourceRelationshipAsSource in source.RelationshipsAsSource)
                        {
                            if (relationship.Equals(sourceRelationshipAsSource.RelationshipBase))
                                return Message.RelationFail;
                        }
                        foreach (RelationshipInstance sourceRelationshipAsTarget in source.RelationshipsAsTarget)
                        {
                            if (relationship.Equals(sourceRelationshipAsTarget.RelationshipBase))
                                return Message.RelationFail;
                        }
                        break;

                    default:
                        break;
                }

                // Check whether the source is valid
                if (!source.IsNodeOf(relationship.Source) || (sourceSpace != null && !source.Spaces.Contains(sourceSpace)))
                    return Message.RelationFail;

                // Check whether the target is valid
                bool targetOK = false;
                foreach (EntityBase possibleTarget in relationship.Targets)
                {
                    if (target.IsNodeOf(possibleTarget) && (targetSpace == null || target.Spaces.Contains(targetSpace)))
                    {
                        targetOK = true;
                        break;
                    }
                }

                if (targetOK)
                {
                    // Create a new relationship instance
                    RelationshipInstance relationshipInstance = new RelationshipInstance(relationship, source, target, sourceSpace, targetSpace);

                    // Check whether the requirements have been satisfied
                    if ((relationship.SourceRequirements == null || relationship.SourceRequirements.IsSatisfied(source, relationshipInstance)) &&
                        (relationship.TargetsRequirements == null || relationship.TargetsRequirements.IsSatisfied(target, relationshipInstance)) &&
                        (relationship.SpatialRequirementBetweenSourceAndTargets == null || relationship.SpatialRequirementBetweenSourceAndTargets.IsSatisfied(source, target, relationshipInstance)))
                    {
                        // Add the relationship
                        source.AddRelationship(relationshipInstance);
                        target.AddRelationship(relationshipInstance);

                        return Message.RelationSuccess;
                    }
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: AddRelationship(RelationshipBase relationship, PhysicalObjectInstance source, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)

        #region Method: RemoveRelationship(RelationshipBase relationship, PhysicalObjectInstance source, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        /// <summary>
        /// Remove a relationship of the given base between the given source and target.
        /// </summary>
        /// <param name="relationship">The relationship to remove between source and target.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="sourceSpace">The space of the source.</param>
        /// <param name="targetSpace">The space of the target.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public static Message RemoveRelationship(RelationshipBase relationship, PhysicalObjectInstance source, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        {
            if (relationship != null && source != null && target != null && !relationship.IsExclusive)
            {
                // Find the relationship instance and remove it
                foreach (RelationshipInstance relationshipInstance in source.RelationshipsAsSource)
                {
                    if (relationship.Equals(relationshipInstance.RelationshipBase) && source.Equals(relationshipInstance.Source) && target.Equals(relationshipInstance.Target) &&
                        (sourceSpace == null || source.Spaces.Contains(sourceSpace)) && (targetSpace == null || target.Spaces.Contains(targetSpace)))
                    {
                        source.RemoveRelationship(relationshipInstance);
                        target.RemoveRelationship(relationshipInstance);
                        return Message.RelationSuccess;
                    }
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveRelationship(RelationshipBase relationship, PhysicalObjectInstance source, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)

        #region Method: AddRelationshipAsSource(RelationshipTypeBase relationshipType, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        /// <summary>
        /// Add a relationship with this entity instance as source, and the given instance as target.
        /// </summary>
        /// <param name="relationshipType">The relationship type.</param>
        /// <param name="target">The target.</param>
        /// <param name="sourceSpace">The space of the source.</param>
        /// <param name="targetSpace">The space of the target.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddRelationshipAsSource(RelationshipTypeBase relationshipType, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        {
            return PhysicalObjectInstance.AddRelationship(relationshipType, this, target, sourceSpace, targetSpace);
        }
        #endregion Method: AddRelationshipAsSource(RelationshipTypeBase relationshipType, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)

        #region Method: RemoveRelationshipAsSource(RelationshipTypeBase relationshipType, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        /// <summary>
        /// Remove a relationship with this entity instance as source, and the given instance as target.
        /// </summary>
        /// <param name="relationshipType">The relationship type.</param>
        /// <param name="target">The target.</param>
        /// <param name="sourceSpace">The space of the source.</param>
        /// <param name="targetSpace">The space of the target.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveRelationshipAsSource(RelationshipTypeBase relationshipType, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        {
            return PhysicalObjectInstance.RemoveRelationship(relationshipType, this, target, sourceSpace, targetSpace);
        }
        #endregion Method: RemoveRelationshipAsSource(RelationshipTypeBase relationship, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)

        #region Method: AddRelationshipAsTarget(RelationshipTypeBase relationshipType, PhysicalObjectInstance source, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        /// <summary>
        /// Add a relationship with this entity instance as target, and the given instance as target.
        /// </summary>
        /// <param name="relationshipType">The relationship type.</param>
        /// <param name="source">The source.</param>
        /// <param name="sourceSpace">The space of the source.</param>
        /// <param name="targetSpace">The space of the target.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddRelationshipAsTarget(RelationshipTypeBase relationshipType, PhysicalObjectInstance source, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        {
            return PhysicalObjectInstance.AddRelationship(relationshipType, source, this, targetSpace, targetSpace);
        }
        #endregion Method: AddRelationshipAsTarget(RelationshipTypeBase relationshipType, PhysicalObjectInstance source, SpaceInstance sourceSpace, SpaceInstance targetSpace)

        #region Method: RemoveRelationshipAsTarget(RelationshipTypeBase relationshipType, PhysicalObjectInstance source, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        /// <summary>
        /// Remove a relationship with this entity instance as target, and the given instance as target.
        /// </summary>
        /// <param name="relationshipType">The relationship type.</param>
        /// <param name="source">The source.</param>
        /// <param name="sourceSpace">The space of the source.</param>
        /// <param name="targetSpace">The space of the target.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveRelationshipAsTarget(RelationshipTypeBase relationshipType, PhysicalObjectInstance source, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        {
            return PhysicalObjectInstance.RemoveRelationship(relationshipType, source, this, targetSpace, targetSpace);
        }
        #endregion Method: RemoveRelationshipAsTarget(RelationshipTypeBase relationshipType, PhysicalObjectInstance source, SpaceInstance sourceSpace, SpaceInstance targetSpace)

        #region Method: AddRelationshipAsSource(RelationshipBase relationship, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        /// <summary>
        /// Add a relationship with this entity instance as source, and the given instance as target.
        /// </summary>
        /// <param name="relationship">The relationship.</param>
        /// <param name="target">The target.</param>
        /// <param name="sourceSpace">The space of the source.</param>
        /// <param name="targetSpace">The space of the target.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddRelationshipAsSource(RelationshipBase relationship, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        {
            return PhysicalObjectInstance.AddRelationship(relationship, this, target, sourceSpace, targetSpace);
        }
        #endregion Method: AddRelationshipAsSource(RelationshipBase relationship, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)

        #region Method: RemoveRelationshipAsSource(RelationshipBase relationship, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        /// <summary>
        /// Remove a relationship with this entity instance as source, and the given instance as target.
        /// </summary>
        /// <param name="relationship">The relationship.</param>
        /// <param name="target">The target.</param>
        /// <param name="sourceSpace">The space of the source.</param>
        /// <param name="targetSpace">The space of the target.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveRelationshipAsSource(RelationshipBase relationship, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        {
            return PhysicalObjectInstance.RemoveRelationship(relationship, this, target, sourceSpace, targetSpace);
        }
        #endregion Method: RemoveRelationshipAsSource(RelationshipBase relationship, PhysicalObjectInstance target, SpaceInstance sourceSpace, SpaceInstance targetSpace)

        #region Method: AddRelationshipAsTarget(RelationshipBase relationship, PhysicalObjectInstance source, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        /// <summary>
        /// Add a relationship with this entity instance as target, and the given instance as target.
        /// </summary>
        /// <param name="relationship">The relationship.</param>
        /// <param name="source">The source.</param>
        /// <param name="sourceSpace">The space of the source.</param>
        /// <param name="targetSpace">The space of the target.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddRelationshipAsTarget(RelationshipBase relationship, PhysicalObjectInstance source, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        {
            return PhysicalObjectInstance.AddRelationship(relationship, source, this, targetSpace, targetSpace);
        }
        #endregion Method: AddRelationshipAsTarget(RelationshipBase relationship, PhysicalObjectInstance source, SpaceInstance sourceSpace, SpaceInstance targetSpace)

        #region Method: RemoveRelationshipAsTarget(RelationshipBase relationship, PhysicalObjectInstance source, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        /// <summary>
        /// Remove a relationship with this entity instance as target, and the given instance as target.
        /// </summary>
        /// <param name="relationship">The relationship.</param>
        /// <param name="source">The source.</param>
        /// <param name="sourceSpace">The space of the source.</param>
        /// <param name="targetSpace">The space of the target.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveRelationshipAsTarget(RelationshipBase relationship, PhysicalObjectInstance source, SpaceInstance sourceSpace, SpaceInstance targetSpace)
        {
            return PhysicalObjectInstance.RemoveRelationship(relationship, source, this, targetSpace, targetSpace);
        }
        #endregion Method: RemoveRelationshipAsTarget(RelationshipBase relationship, PhysicalObjectInstance source, SpaceInstance sourceSpace, SpaceInstance targetSpace)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasSpace(SpaceBase spaceBase)
        /// <summary>
        /// Checks if this physical object has the given space.
        /// </summary>
        /// <param name="space">The space to check.</param>
        /// <returns>Returns true when this physical object has the space.</returns>
        public bool HasSpace(SpaceBase spaceBase)
        {
            if (spaceBase != null)
            {
                foreach (SpaceInstance spaceInstance in this.Spaces)
                {
                    if (spaceBase.Equals(spaceInstance.SpaceBase))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasSpace(SpaceBase spaceBase)

        #region Method: MarkAsModified(bool modified, bool spread)
        /// <summary>
        /// Mark this instance as modified.
        /// </summary>
        /// <param name="modified">The value that indicates whether the instance has been modified.</param>
        /// <param name="spread">The value that indicates whether the marking should spread further.</param>
        internal override void MarkAsModified(bool modified, bool spread)
        {
            base.MarkAsModified(modified, spread);

            if (spread)
            {
                foreach (SpaceInstance space in this.Spaces)
                    space.MarkAsModified(modified, false);
            }
        }
        #endregion Method: MarkAsModified(bool modified, bool spread)

        #region Method: SetPosition(Vec3 newPosition)
        /// <summary>
        /// Set the new position.
        /// </summary>
        /// <param name="newPosition">The new position.</param>
        protected override void SetPosition(Vec3 newPosition)
        {
            Vec3 newPos = new Vec3(newPosition);
            Vec3 oldPosition = new Vec3(this.Position);
            Vec3 delta = newPos - oldPosition;

            // First set the position of the base
            base.SetPosition(newPosition);

            // Then move all the spaces
            foreach (SpaceInstance spaceInstance in this.Spaces)
                spaceInstance.Position += delta;
        }
        #endregion Method: SetPosition(Vec3 newPosition)

        #region TODO Method: SetRotation(Quaternion newRotation)
        /// <summary>
        /// TODO: is er andere manier om de delta rotation te kennen dan die te kopieren in dit field?
        /// </summary>
        private Quaternion previousRotationPO = new Quaternion();
                                                               
        /// <summary>
        /// Set the new rotation.
        /// </summary>
        /// <param name="newRotation">The new rotation.</param>
        protected virtual void SetRotation(Quaternion newRotation)
        {
            if (newRotation != null && !this.IsLocked)
            {
                float deltaYaw = newRotation.Yaw - previousRotationPO.Yaw;
                float deltaPitch = newRotation.Pitch - previousRotationPO.Pitch;
                float deltaRoll = newRotation.Roll - previousRotationPO.Roll;

                if (this.rotation != null)
                    this.rotation.ValueChanged -= new Quaternion.ValueChangedHandler(rotation_ValueChanged);

                // If the rotation is stored as an attribute instance, store the value there
                if (rotationAttribute != null && newRotation != null)
                {
                    Vec4 vector = ((VectorValueInstance)rotationAttribute.Value).Vector;
                    this.ignoreRotationSet = true;
                    vector.W = newRotation.W;
                    vector.X = newRotation.X;
                    vector.Y = newRotation.Y;
                    vector.Z = newRotation.Z;
                    this.rotation.W = newRotation.W;
                    this.rotation.X = newRotation.X;
                    this.rotation.Y = newRotation.Y;
                    this.rotation.Z = newRotation.Z;
                    this.ignoreRotationSet = false;
                }
                // Otherwise, just set the default rotation
                else
                    this.rotation = newRotation;

                // Send notifications
                NotifyPropertyChanged("Rotation");
                if (RotationChanged != null)
                    RotationChanged(this, this.rotation);

                // TODO Also rotate all the spaces
                foreach (SpaceInstance spaceInstance in this.Spaces)
                {
                    // TODO: do something with the position: see TangibleObjectInstance.SetRotation
                    spaceInstance.Rotation = this.rotation;
                }

                if (this.rotation != null)
                    this.rotation.ValueChanged += new Quaternion.ValueChangedHandler(rotation_ValueChanged);

                previousRotationPO = new Quaternion(newRotation);
            }
        }
        #endregion Method: SetRotation(Quaternion newRotation)

        #region Method: SetScale(Vec3 newScale)
        /// <summary>
        /// Set the new scale.
        /// </summary>
        /// <param name="newScale">The new scale.</param>
        protected virtual void SetScale(Vec3 newScale)
        {
            if (newScale != null && !this.IsLocked)
            {
                if (this.scale != null)
                    this.scale.ValueChanged -= new Vec3.Vec3Handler(scale_ValueChanged);

                Vec3 oldScale = new Vec3(this.Scale);

                // If the scale is stored as an attribute instance, store the value there
                if (this.scaleAttribute != null)
                {
                    Vec4 vector = ((VectorValueInstance)this.scaleAttribute.Value).Vector;
                    this.ignoreScaleSet = true;
                    vector.X = newScale.X;
                    vector.Y = newScale.Y;
                    vector.Z = newScale.Z;
                    this.ignoreScaleSet = false;
                }
                // Otherwise, just set the default scale
                else
                    this.scale = newScale;

                // Send notifications
                NotifyPropertyChanged("Scale");
                if (ScaleChanged != null)
                    ScaleChanged(this, this.scale);

                // Also scale all the spaces
                Vec3 delta = new Vec3(newScale.X / oldScale.X, newScale.Y / oldScale.Y, newScale.Z / oldScale.Z);
                foreach (SpaceInstance spaceInstance in this.Spaces)
                    spaceInstance.Scale = new Vec3(spaceInstance.Scale.X * delta.X, spaceInstance.Scale.Y * delta.Y, spaceInstance.Scale.Z * delta.Z);

                if (this.scale != null)
                    this.scale.ValueChanged += new Vec3.Vec3Handler(scale_ValueChanged);
            }
        }
        #endregion Method: SetScale(Vec3 newScale)

        #region Method: SetIsLocked(bool isLockedValue)
        /// <summary>
        /// Set the new value for IsLocked.
        /// </summary>
        /// <param name="isLockedValue">The new value for IsLocked.</param>
        protected override void SetIsLocked(bool isLockedValue)
        {
            // First set the base
            base.SetIsLocked(isLockedValue);

            // Then lock all spaces
            foreach (SpaceInstance space in this.Spaces)
                space.IsLocked = isLockedValue;
        }
        #endregion Method: SetIsLocked(bool isLockedValue)

        #region Method: SetBoundingBox(Box boundingBox)
        /// <summary>
        /// Set the bounding box of the physical object instance.
        /// </summary>
        /// <param name="boundingBox">The bounding box.</param>
        public void SetBoundingBox(Box boundingBox)
        {
            MathFunctionSolverWithCustomConstants functionSolver = new MathFunctionSolverWithCustomConstants(null);
            functionSolver.AddConstant(SemanticsSettings.General.BoundingBoxName, new FunctionalBox(boundingBox));
            foreach (SpaceInstance spaceInstance in this.Spaces)
            {
                if (spaceInstance.SpaceValuedBase != null && spaceInstance.SpaceValuedBase.ShapeDescription != null &&
                    spaceInstance.SpaceValuedBase.ShapeDescription.Type != BaseShapeType.Undefined)
                    spaceInstance.SetBaseShape(spaceInstance.SpaceValuedBase.ShapeDescription.Instantiate(functionSolver));
            }
        }
        #endregion Method: SetBoundingBox(Box boundingBox)

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the given condition satisfies the physical object instance.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the physical object instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the condition satisfies the physical object instance.</returns>
        public override bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (conditionBase != null)
            {
                // Check whether the base satisfies the condition
                if ((this is TangibleObjectInstance && conditionBase is SpaceConditionBase) || base.Satisfies(conditionBase, iVariableInstanceHolder))
                {
                    // Physical object condition
                    PhysicalObjectConditionBase physicalObjectConditionBase = conditionBase as PhysicalObjectConditionBase;
                    if (physicalObjectConditionBase != null)
                    {
                        // Check whether the instance has all mandatory spaces
                        if (physicalObjectConditionBase.HasAllMandatorySpaces == true)
                        {
                            foreach (SpaceValuedBase spaceValuedBase in this.PhysicalObjectBase.Spaces)
                            {
                                if (spaceValuedBase.Necessity == Necessity.Mandatory)
                                {
                                    int quantity = 0;
                                    foreach (SpaceInstance spaceInstance in this.Spaces)
                                    {
                                        if (spaceInstance.IsNodeOf(spaceValuedBase.SpaceBase))
                                            quantity++;
                                    }
                                    if (!spaceValuedBase.Quantity.IsInRange(quantity))
                                        return false;
                                }
                            }
                        }

                        // Check whether the instance has all the required spaces
                        foreach (SpaceConditionBase spaceConditionBase in physicalObjectConditionBase.Spaces)
                        {
                            if (spaceConditionBase.Quantity == null ||
                                spaceConditionBase.Quantity.BaseValue == null ||
                                spaceConditionBase.Quantity.ValueSign == null)
                            {
                                bool satisfied = false;
                                foreach (SpaceInstance spaceInstance in this.Spaces)
                                {
                                    if (spaceInstance.Satisfies(spaceConditionBase, iVariableInstanceHolder))
                                    {
                                        satisfied = true;
                                        break;
                                    }
                                }
                                if (!satisfied)
                                    return false;
                            }
                            else
                            {
                                int quantity = 0;
                                int requiredQuantity = (int)spaceConditionBase.Quantity.BaseValue;
                                foreach (SpaceInstance spaceInstance in this.Spaces)
                                {
                                    if (spaceInstance.Satisfies(spaceConditionBase, iVariableInstanceHolder))
                                        quantity++;
                                }
                                if (!Toolbox.Compare(quantity, (EqualitySignExtended)spaceConditionBase.Quantity.ValueSign, requiredQuantity))
                                    return false;
                            }
                        }

                        // Space condition
                        SpaceConditionBase spaceCondition = physicalObjectConditionBase as SpaceConditionBase;
                        if (spaceCondition != null && !ignoreSpaceCheck)
                        {
                            ignoreSpaceCheck = true;
                            if (spaceCondition.Quantity == null ||
                                spaceCondition.Quantity.BaseValue == null ||
                                spaceCondition.Quantity.ValueSign == null)
                            {
                                bool satisfied = false;
                                foreach (SpaceInstance spaceInstance in this.Spaces)
                                {
                                    if (spaceInstance.Satisfies(spaceCondition, iVariableInstanceHolder))
                                    {
                                        satisfied = true;
                                        break;
                                    }
                                }
                                if (!satisfied)
                                {
                                    ignoreSpaceCheck = false;
                                    return false;
                                }
                            }
                            else
                            {
                                int quantity = 0;
                                int requiredQuantity = (int)spaceCondition.Quantity.BaseValue;
                                foreach (SpaceInstance spaceInstance in this.Spaces)
                                {
                                    if (spaceInstance.Satisfies(spaceCondition, iVariableInstanceHolder))
                                        quantity++;
                                }
                                if (!Toolbox.Compare(quantity, (EqualitySignExtended)spaceCondition.Quantity.ValueSign, requiredQuantity))
                                {
                                    ignoreSpaceCheck = false;
                                    return false;
                                }
                            }
                            ignoreSpaceCheck = false;
                            return true;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Indicates whether space checks should be ignored, occuring when space conditions are checked.
        /// </summary>
        protected bool ignoreSpaceCheck = false;
        #endregion Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the given change to the physical object instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the physical object instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        protected internal override bool Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (changeBase != null)
            {
                if ((this is TangibleObjectInstance && changeBase is SpaceChangeBase) || base.Apply(changeBase, iVariableInstanceHolder))
                {
                    // Physical object change
                    PhysicalObjectChangeBase physicalObjectChangeBase = changeBase as PhysicalObjectChangeBase;
                    if (physicalObjectChangeBase != null)
                    {
                        // Apply the space changes
                        foreach (SpaceChangeBase spaceChangeBase in physicalObjectChangeBase.Spaces)
                        {
                            foreach (SpaceInstance spaceInstance in this.Spaces)
                                spaceInstance.Apply(spaceChangeBase, iVariableInstanceHolder);
                        }

                        // Add the spaces
                        foreach (SpaceValuedBase spaceValuedBase in physicalObjectChangeBase.SpacesToAdd)
                        {
                            for (int i = 0; i < spaceValuedBase.Quantity.BaseValue; i++)
                                AddSpace(InstanceManager.Current.Create<SpaceInstance>(spaceValuedBase));
                        }

                        // Remove the spaces
                        foreach (SpaceConditionBase spaceConditionBase in physicalObjectChangeBase.SpacesToRemove)
                        {
                            if (spaceConditionBase.Quantity == null ||
                                spaceConditionBase.Quantity.BaseValue == null ||
                                spaceConditionBase.Quantity.ValueSign == null)
                            {
                                foreach (SpaceInstance spaceInstance in this.Spaces)
                                {
                                    if (spaceInstance.Satisfies(spaceConditionBase, iVariableInstanceHolder))
                                    {
                                        RemoveSpace(spaceInstance);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                int quantity = (int)spaceConditionBase.Quantity.BaseValue;
                                if (quantity > 0)
                                {
                                    ReadOnlyCollection<SpaceInstance> spaces = this.Spaces;
                                    foreach (SpaceInstance spaceInstance in spaces)
                                    {
                                        if (spaceInstance.Satisfies(spaceConditionBase, iVariableInstanceHolder))
                                        {
                                            RemoveSpace(spaceInstance);
                                            quantity--;
                                            if (quantity <= 0)
                                                break;
                                        }
                                    }
                                }
                            }
                        }

                        // Space change
                        SpaceChangeBase spaceChange = changeBase as SpaceChangeBase;
                        if (spaceChange != null)
                        {
                            foreach (SpaceInstance spaceInstance in this.Spaces)
                                spaceInstance.Apply(spaceChange, iVariableInstanceHolder);
                            return true;
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        #endregion Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: GetSpace(string name)
        /// <summary>
        /// Get the space with the given name.
        /// </summary>
        /// <param name="name">The name of the space to get.</param>
        /// <returns>The space with the given name.</returns>
        public SpaceInstance GetSpace(string name)
        {
            if (name != null)
            {
                foreach (SpaceInstance spaceInstance in this.Spaces)
                {
                    if (name.Equals(spaceInstance.DefaultName))
                        return spaceInstance;
                }
            }
            return null;
        }
        #endregion Method: GetSpace(string name)

        #endregion Method Group: Other

    }
    #endregion Class: PhysicalObjectInstance

}
