/**************************************************************************
 * 
 * ReferenceBase.cs
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
using Common;
using Semantics.Components;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Entities;
using SemanticsEngine.Interfaces;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: ReferenceBase
    /// <summary>
    /// A base for a reference.
    /// </summary>
    public abstract class ReferenceBase : Base
    {

        #region Properties and Fields

        #region Property: Reference
        /// <summary>
        /// Gets the reference this is a base of.
        /// </summary>
        internal Reference Reference
        {
            get
            {
                return this.IdHolder as Reference;
            }
        }
        #endregion Property: Reference

        #region Property: Name
        /// <summary>
        /// The name of the reference.
        /// </summary>
        private String name = String.Empty;
        
        /// <summary>
        /// Gets the name of the reference.
        /// </summary>
        public String Name
        {
            get
            {
                return name;
            }
        }
        #endregion Property: Name

        #region Property: Subject
        /// <summary>
        /// The subject.
        /// </summary>
        private ActorTargetArtifactReference subject = default(ActorTargetArtifactReference);
        
        /// <summary>
        /// Gets the subject.
        /// </summary>
        public ActorTargetArtifactReference Subject
        {
            get
            {
                return subject;
            }
        }
        #endregion Property: Subject

        #region Property: SubjectReference
        /// <summary>
        /// The reference, in case the Subject has been set to 'Reference'.
        /// </summary>
        private ReferenceBase subjectReference = null;
        
        /// <summary>
        /// Gets the reference, in case the Subject has been set to 'Reference'.
        /// </summary>
        public ReferenceBase SubjectReference
        {
            get
            {
                return subjectReference;
            }
        }
        #endregion Property: SubjectReference

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ReferenceBase(Reference reference)
        /// <summary>
        /// Create a reference base from the given reference.
        /// </summary>
        /// <param name="reference">The reference to create a reference base from.</param>
        protected ReferenceBase(Reference reference)
            : base(reference)
        {
            if (reference != null)
            {
                this.name = reference.Name;
                this.subject = reference.Subject;
                this.subjectReference = BaseManager.Current.GetBase<ReferenceBase>(reference.SubjectReference);
            }
        }
        #endregion Constructor: ReferenceBase(Reference reference)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Abstract Method: GetEntities(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get the referenced entity instances.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>The referenced entity instances.</returns>
        public abstract ReadOnlyCollection<EntityInstance> GetEntities(IVariableInstanceHolder iVariableInstanceHolder);
        #endregion Abstract Method: GetEntities(IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: GetSubjects(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get the entity instances that are the subject.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>The entity instances that are the subject.</returns>
        protected List<EntityInstance> GetSubjects(IVariableInstanceHolder iVariableInstanceHolder)
        {
            List<EntityInstance> subjects = new List<EntityInstance>();

            if (iVariableInstanceHolder != null)
            {
                switch (this.Subject)
                {
                    // Get the actor
                    case ActorTargetArtifactReference.Actor:
                        subjects.Add(iVariableInstanceHolder.GetActor());
                        break;

                    // Get the target
                    case ActorTargetArtifactReference.Target:
                        subjects.Add(iVariableInstanceHolder.GetTarget());
                        break;

                    // Get the artifact
                    case ActorTargetArtifactReference.Artifact:
                        subjects.Add(iVariableInstanceHolder.GetArtifact());
                        break;

                    // Get all referenced entities
                    case ActorTargetArtifactReference.Reference:
                        if (this.SubjectReference != null)
                        {
                            foreach (EntityInstance entityInstance in this.SubjectReference.GetEntities(iVariableInstanceHolder))
                                subjects.Add(entityInstance);
                        }
                        break;

                    default:
                        break;
                }
            }

            return subjects;
        }
        #endregion Method: GetSubjects(IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation of the reference, which is its name.
        /// </summary>
        /// <returns>The string representation of the reference, which is its name.</returns>
        public override string ToString()
        {
            return this.Name;
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: ReferenceBase

    #region Class: SpatialReferenceBase
    /// <summary>
    /// A reference to entities in a range of the subject.
    /// </summary>
    public sealed class SpatialReferenceBase : ReferenceBase
    {

        #region Properties and Fields

        #region Property: SpatialRequirement
        /// <summary>
        /// The spatial requirement.
        /// </summary>
        private SpatialRequirementBase spatialRequirement = null;
        
        /// <summary>
        /// Gets the spatial requirement.
        /// </summary>
        public SpatialRequirementBase SpatialRequirement
        {
            get
            {
                return spatialRequirement;
            }
        }
        #endregion Property: SpatialRequirement

        #region Property: SelectionType
        /// <summary>
        /// The selection type.
        /// </summary>
        private SelectionType selectionType = default(SelectionType);
        
        /// <summary>
        /// Gets the selection type.
        /// </summary>
        public SelectionType SelectionType
        {
            get
            {
                return selectionType;
            }
        }
        #endregion Property: SelectionType

        #region Property: SelectionAmount
        /// <summary>
        /// The amount of entities to select in the range; will not be used for SelectionType 'All'.
        /// </summary>
        private float selectionAmount = SemanticsSettings.Values.SelectionAmount;
        
        /// <summary>
        /// Gets the amount of entities to select in the range; will not be used for SelectionType 'All'.
        /// </summary>
        public float SelectionAmount
        {
            get
            {
                return selectionAmount;
            }
        }
        #endregion Property: SelectionAmount

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SpatialReferenceBase(SpatialReference spatialReference)
        /// <summary>
        /// Create a spatial reference base from the given spatial reference.
        /// </summary>
        /// <param name="spatialReference">The spatial reference to create a spatial reference base from.</param>
        internal SpatialReferenceBase(SpatialReference spatialReference)
            : base(spatialReference)
        {
            if (spatialReference != null)
            {
                this.spatialRequirement = BaseManager.Current.GetBase<SpatialRequirementBase>(spatialReference.SpatialRequirement);
                this.selectionType = spatialReference.SelectionType;
                this.selectionAmount = spatialReference.SelectionAmount;
            }
        }
        #endregion Constructor: SpatialReferenceBase(SpatialReference spatialReference)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetEntities(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get the referenced entity instances.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>The referenced entity instances.</returns>
        public override ReadOnlyCollection<EntityInstance> GetEntities(IVariableInstanceHolder iVariableInstanceHolder)
        {
            List<EntityInstance> entities = new List<EntityInstance>();

            List<Tuple<EntityInstance, float>> entitiesInRange = new List<Tuple<EntityInstance, float>>();
            if (this.SpatialRequirement != null)
            {
                foreach (EntityInstance subject in GetSubjects(iVariableInstanceHolder))
                {
                    PhysicalObjectInstance physicalSubject = subject as PhysicalObjectInstance;
                    if (physicalSubject != null && physicalSubject.World != null)
                    {
                        foreach (EntityInstance entityInstance in physicalSubject.World.Instances)
                        {
                            PhysicalObjectInstance physicalEntityInstance = entityInstance as PhysicalObjectInstance;
                            if (physicalEntityInstance != null)
                            {
                                // Check whether the distance between the subject and the instance around it satisfies the spatial requirement
                                float distanceSquared = physicalSubject.GetDistance(physicalEntityInstance, true, true);
                                if (Toolbox.Compare(distanceSquared, this.SpatialRequirement.ValueSign, this.SpatialRequirement.BaseValue * this.SpatialRequirement.BaseValue, this.SpatialRequirement.BaseValue2 * this.SpatialRequirement.BaseValue2))
                                    entitiesInRange.Add(new Tuple<EntityInstance, float>(physicalEntityInstance, distanceSquared));
                            }
                        }
                    }
                }
            }

            // Select the correct instances
            if (this.SelectionType == SelectionType.All || entitiesInRange.Count <= this.SelectionAmount)
            {
                // Get them all
                foreach (Tuple<EntityInstance, float> tuple in entitiesInRange)
                    entities.Add(tuple.Item1);
            }
            else if (this.SelectionType == SelectionType.Nearest || this.SelectionType == SelectionType.Farthest)
            {
                // Sort the instances by distance
                List<Tuple<EntityInstance, float>> sortedByDistance = new List<Tuple<EntityInstance, float>>(entitiesInRange.Count);
                foreach (Tuple<EntityInstance, float> tuple in entitiesInRange)
                {
                    if (sortedByDistance.Count == 0)
                        sortedByDistance.Add(tuple);
                    else
                    {
                        int i;
                        for (i = 0; i < sortedByDistance.Count; i++)
                        {
                            if (tuple.Item2 > sortedByDistance[i].Item2)
                                break;
                        }
                        sortedByDistance.Insert(i, tuple);
                    }
                }

                // Get the nearest or farthest ones
                if (this.SelectionType == SelectionType.Nearest)
                {
                    for (int i = 0; i < this.SelectionAmount; i++)
                        entities.Add(sortedByDistance[i].Item1);
                }
                else if (this.SelectionType == SelectionType.Farthest)
                {
                    for (int i = (int)this.SelectionAmount; i > 0; i--)
                        entities.Add(sortedByDistance[i].Item1);
                }
            }
            else if (this.SelectionType == SelectionType.Random)
            {
                // Get random ones
                int remainingAmount = (int)this.SelectionAmount;
                while (remainingAmount > 0)
                {
                    int index = RandomNumber.Next(entitiesInRange.Count);
                    entities.Add(entitiesInRange[index].Item1);
                    entitiesInRange.RemoveAt(index);
                    remainingAmount--;
                }
            }

            return entities.AsReadOnly();
        }
        #endregion Method: GetEntities(IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: SpatialReferenceBase

    #region Class: SpaceReferenceBase
    /// <summary>
    /// A reference to a space of a source.
    /// </summary>
    public sealed class SpaceReferenceBase : ReferenceBase
    {

        #region Properties and Fields

        #region Property: Space
        /// <summary>
        /// The space.
        /// </summary>
        private SpaceBase space = null;
        
        /// <summary>
        /// Gets the space.
        /// </summary>
        public SpaceBase Space
        {
            get
            {
                return space;
            }
        }
        #endregion Property: Space

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SpaceReferenceBase(SpaceReference spaceReference)
        /// <summary>
        /// Create a space reference base from the given space reference.
        /// </summary>
        /// <param name="spaceReference">The space reference to create a space reference base from.</param>
        internal SpaceReferenceBase(SpaceReference spaceReference)
            : base(spaceReference)
        {
            if (spaceReference != null)
                this.space = BaseManager.Current.GetBase<SpaceBase>(spaceReference.Space);
        }
        #endregion Constructor: SpaceReferenceBase(SpaceReference spaceReference)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetEntities(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get the referenced entity instances.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>The referenced entity instances.</returns>
        public override ReadOnlyCollection<EntityInstance> GetEntities(IVariableInstanceHolder iVariableInstanceHolder)
        {
            List<EntityInstance> entities = new List<EntityInstance>();

            if (this.Space != null)
            {
                foreach (EntityInstance subject in GetSubjects(iVariableInstanceHolder))
                {
                    PhysicalObjectInstance physicalObjectInstance = subject as PhysicalObjectInstance;
                    if (physicalObjectInstance != null)
                    {
                        // Get all the matching spaces of the subject
                        foreach (SpaceInstance spaceInstance in physicalObjectInstance.Spaces)
                        {
                            if (spaceInstance.IsNodeOf(this.Space))
                                entities.Add(spaceInstance);
                        }
                    }
                }
            }

            return entities.AsReadOnly();
        }
        #endregion Method: GetEntities(IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: SpaceReferenceBase

    #region Class: RelationshipReferenceBase
    /// <summary>
    /// A reference to a relationship.
    /// </summary>
    public sealed class RelationshipReferenceBase : ReferenceBase
    {

        #region Properties and Fields

        #region Property: RelationshipType
        /// <summary>
        /// The relationship type.
        /// </summary>
        private RelationshipTypeBase relationshipType = null;
        
        /// <summary>
        /// Gets the relationship type.
        /// </summary>
        public RelationshipTypeBase RelationshipType
        {
            get
            {
                return relationshipType;
            }
        }
        #endregion Property: RelationshipType

        #region Property: SubjectRole
        /// <summary>
        /// The role of the subject in the relationship: source or target; if set to 'null', both are valid.
        /// </summary>
        private SourceTarget? subjectRole = null;
        
        /// <summary>
        /// Gets the role of the subject in the relationship: source or target; if set to 'null', both are valid.
        /// </summary>
        public SourceTarget? SubjectRole
        {
            get
            {
                return subjectRole;
            }
        }
        #endregion Property: SubjectRole

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: RelationshipReferenceBase(RelationshipReference relationshipReference)
        /// <summary>
        /// Create a relationship reference base from the given relationship reference.
        /// </summary>
        /// <param name="relationshipReference">The relationship reference to create a relationship reference base from.</param>
        internal RelationshipReferenceBase(RelationshipReference relationshipReference)
            : base(relationshipReference)
        {
            if (relationshipReference != null)
            {
                this.relationshipType = BaseManager.Current.GetBase<RelationshipTypeBase>(relationshipReference.RelationshipType);
                this.subjectRole = relationshipReference.SubjectRole;
            }
        }
        #endregion Constructor: RelationshipReferenceBase(RelationshipReference relationshipReference)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetEntities(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get the referenced entity instances.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>The referenced entity instances.</returns>
        public override ReadOnlyCollection<EntityInstance> GetEntities(IVariableInstanceHolder iVariableInstanceHolder)
        {
            List<EntityInstance> entities = new List<EntityInstance>();

            if (this.RelationshipType != null)
            {
                // Retrieve all targets of the relationships where the subjects are the source
                if (this.SubjectRole == null || this.SubjectRole == SourceTarget.Source)
                {
                    foreach (EntityInstance subject in GetSubjects(iVariableInstanceHolder))
                    {
                        foreach (RelationshipInstance relationshipInstance in subject.RelationshipsAsSource)
                        {
                            if (relationshipInstance.RelationshipType.IsNodeOf(this.RelationshipType) && relationshipInstance.Target != null)
                                entities.Add(relationshipInstance.Target);
                        }
                    }
                }

                // Retrieve all sources of the relationships where the subjects are the target
                if (this.SubjectRole == null || this.SubjectRole == SourceTarget.Target)
                {
                    foreach (EntityInstance subject in GetSubjects(iVariableInstanceHolder))
                    {
                        foreach (RelationshipInstance relationshipInstance in subject.RelationshipsAsTarget)
                        {
                            if (relationshipInstance.RelationshipType.IsNodeOf(this.RelationshipType) && relationshipInstance.Target != null)
                                entities.Add(relationshipInstance.Source);
                        }
                    }
                }
            }

            return entities.AsReadOnly();
        }
        #endregion Method: GetEntities(IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: RelationshipReferenceBase

    #region Class: PartReferenceBase
    /// <summary>
    /// A reference to a part.
    /// </summary>
    public sealed class PartReferenceBase : ReferenceBase
    {

        #region Properties and Fields

        #region Property: Part
        /// <summary>
        /// The part.
        /// </summary>
        private TangibleObjectBase part = null;
        
        /// <summary>
        /// Gets the part.
        /// </summary>
        public TangibleObjectBase Part
        {
            get
            {
                return part;
            }
        }
        #endregion Property: Part

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: PartReferenceBase(PartReference partReference)
        /// <summary>
        /// Create a part reference base from the given part reference.
        /// </summary>
        /// <param name="partReference">The part reference to create a part reference base from.</param>
        internal PartReferenceBase(PartReference partReference)
            : base(partReference)
        {
            if (partReference != null)
                this.part = BaseManager.Current.GetBase<TangibleObjectBase>(partReference.Part);
        }
        #endregion Constructor: PartReferenceBase(PartReference partReference)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetEntities(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get the referenced entity instances.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>The referenced entity instances.</returns>
        public override ReadOnlyCollection<EntityInstance> GetEntities(IVariableInstanceHolder iVariableInstanceHolder)
        {
            List<EntityInstance> entities = new List<EntityInstance>();

            if (this.Part != null)
            {
                foreach (EntityInstance subject in GetSubjects(iVariableInstanceHolder))
                {
                    TangibleObjectInstance tangibleObjectInstance = subject as TangibleObjectInstance;
                    if (tangibleObjectInstance != null)
                    {
                        // Get all the matching parts of the tangible object
                        foreach (TangibleObjectInstance partInstance in tangibleObjectInstance.Parts)
                        {
                            if (partInstance.IsNodeOf(this.Part))
                                entities.Add(partInstance);
                        }
                    }
                }
            }

            return entities.AsReadOnly();
        }
        #endregion Method: GetEntities(IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: PartReferenceBase

    #region Class: CoverReferenceBase
    /// <summary>
    /// A reference to a cover.
    /// </summary>
    public sealed class CoverReferenceBase : ReferenceBase
    {

        #region Properties and Fields

        #region Property: Cover
        /// <summary>
        /// The cover.
        /// </summary>
        private TangibleObjectBase cover = null;
        
        /// <summary>
        /// Gets the cover.
        /// </summary>
        public TangibleObjectBase Cover
        {
            get
            {
                return cover;
            }
        }
        #endregion Property: Cover

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: CoverReferenceBase(CoverReference coverReference)
        /// <summary>
        /// Create a cover reference base from the given cover reference.
        /// </summary>
        /// <param name="coverReference">The cover reference to create a cover reference base from.</param>
        internal CoverReferenceBase(CoverReference coverReference)
            : base(coverReference)
        {
            if (coverReference != null)
                this.cover = BaseManager.Current.GetBase<TangibleObjectBase>(coverReference.Cover);
        }
        #endregion Constructor: CoverReferenceBase(CoverReference coverReference)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetEntities(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get the referenced entity instances.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>The referenced entity instances.</returns>
        public override ReadOnlyCollection<EntityInstance> GetEntities(IVariableInstanceHolder iVariableInstanceHolder)
        {
            List<EntityInstance> entities = new List<EntityInstance>();

            if (this.Cover != null)
            {
                foreach (EntityInstance subject in GetSubjects(iVariableInstanceHolder))
                {
                    TangibleObjectInstance tangibleObjectInstance = subject as TangibleObjectInstance;
                    if (tangibleObjectInstance != null)
                    {
                        // Get all the matching covers of the tangible object
                        foreach (TangibleObjectInstance coverInstance in tangibleObjectInstance.Covers)
                        {
                            if (coverInstance.IsNodeOf(this.Cover))
                                entities.Add(coverInstance);
                        }
                    }
                }
            }

            return entities.AsReadOnly();
        }
        #endregion Method: GetEntities(IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: CoverReferenceBase

    #region Class: ConnectionReferenceBase
    /// <summary>
    /// A reference to a connection item.
    /// </summary>
    public sealed class ConnectionReferenceBase : ReferenceBase
    {

        #region Properties and Fields

        #region Property: ConnectionItem
        /// <summary>
        /// The connection item.
        /// </summary>
        private TangibleObjectBase connectionItem = null;
        
        /// <summary>
        /// Gets the connection item.
        /// </summary>
        public TangibleObjectBase ConnectionItem
        {
            get
            {
                return connectionItem;
            }
        }
        #endregion Property: ConnectionItem

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ConnectionReferenceBase(ConnectionReference connectionReference)
        /// <summary>
        /// Create a connection reference base from the given connection reference.
        /// </summary>
        /// <param name="connectionReference">The connection reference to create a connection reference base from.</param>
        internal ConnectionReferenceBase(ConnectionReference connectionReference)
            : base(connectionReference)
        {
            if (connectionReference != null)
                this.connectionItem = BaseManager.Current.GetBase<TangibleObjectBase>(connectionReference.ConnectionItem);
        }
        #endregion Constructor: ConnectionReferenceBase(ConnectionReference connectionReference)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetEntities(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get the referenced entity instances.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>The referenced entity instances.</returns>
        public override ReadOnlyCollection<EntityInstance> GetEntities(IVariableInstanceHolder iVariableInstanceHolder)
        {
            List<EntityInstance> entities = new List<EntityInstance>();

            if (this.ConnectionItem != null)
            {
                foreach (EntityInstance subject in GetSubjects(iVariableInstanceHolder))
                {
                    TangibleObjectInstance tangibleObjectInstance = subject as TangibleObjectInstance;
                    if (tangibleObjectInstance != null)
                    {
                        // Get all the matching connection items of the tangible object
                        foreach (TangibleObjectInstance connectionInstance in tangibleObjectInstance.Connections)
                        {
                            if (connectionInstance.IsNodeOf(this.ConnectionItem))
                                entities.Add(connectionInstance);
                        }
                    }
                }
            }

            return entities.AsReadOnly();
        }
        #endregion Method: GetEntities(IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: ConnectionReferenceBase

    #region Class: ItemReferenceBase
    /// <summary>
    /// A reference to an item.
    /// </summary>
    public sealed class ItemReferenceBase : ReferenceBase
    {

        #region Properties and Fields

        #region Property: Item
        /// <summary>
        /// The item.
        /// </summary>
        private TangibleObjectBase item = null;
        
        /// <summary>
        /// Gets the item.
        /// </summary>
        public TangibleObjectBase Item
        {
            get
            {
                return item;
            }
        }
        #endregion Property: Item

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ItemReferenceBase(ItemReference itemReference)
        /// <summary>
        /// Create an item reference base from the given item reference.
        /// </summary>
        /// <param name="itemReference">The item reference to create an item reference base from.</param>
        internal ItemReferenceBase(ItemReference itemReference)
            : base(itemReference)
        {
            if (itemReference != null)
                this.item = BaseManager.Current.GetBase<TangibleObjectBase>(itemReference.Item);
        }
        #endregion Constructor: ItemReferenceBase(ItemReference itemReference)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetEntities(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get the referenced entity instances.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>The referenced entity instances.</returns>
        public override ReadOnlyCollection<EntityInstance> GetEntities(IVariableInstanceHolder iVariableInstanceHolder)
        {
            List<EntityInstance> entities = new List<EntityInstance>();

            if (this.Item != null)
            {
                foreach (EntityInstance subject in GetSubjects(iVariableInstanceHolder))
                {
                    SpaceInstance spaceInstance = subject as SpaceInstance;
                    if (spaceInstance != null)
                    {
                        // Get all the matching items of the space
                        foreach (TangibleObjectInstance itemInstance in spaceInstance.Items)
                        {
                            if (itemInstance.IsNodeOf(this.Item))
                                entities.Add(itemInstance);
                        }
                    }
                }
            }

            return entities.AsReadOnly();
        }
        #endregion Method: GetEntities(IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: ItemReferenceBase

    #region Class: MatterReferenceBase
    /// <summary>
    /// A reference to a matter.
    /// </summary>
    public sealed class MatterReferenceBase : ReferenceBase
    {

        #region Properties and Fields

        #region Property: Matter
        /// <summary>
        /// The matter.
        /// </summary>
        private MatterBase matter = null;
        
        /// <summary>
        /// Gets the matter.
        /// </summary>
        public MatterBase Matter
        {
            get
            {
                return matter;
            }
        }
        #endregion Property: Matter

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: MatterReferenceBase(MatterReference matterReference)
        /// <summary>
        /// Create a matter reference base from the given matter reference.
        /// </summary>
        /// <param name="matterReference">The matter reference to create a matter reference base from.</param>
        internal MatterReferenceBase(MatterReference matterReference)
            : base(matterReference)
        {
            if (matterReference != null)
                this.matter = BaseManager.Current.GetBase<MatterBase>(matterReference.Matter);
        }
        #endregion Constructor: MatterReferenceBase(MatterReference matterReference)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetEntities(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get the referenced entity instances.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>The referenced entity instances.</returns>
        public override ReadOnlyCollection<EntityInstance> GetEntities(IVariableInstanceHolder iVariableInstanceHolder)
        {
            List<EntityInstance> entities = new List<EntityInstance>();

            if (this.Matter != null)
            {
                foreach (EntityInstance subject in GetSubjects(iVariableInstanceHolder))
                {
                    TangibleObjectInstance tangibleObjectInstance = subject as TangibleObjectInstance;
                    if (tangibleObjectInstance != null)
                    {
                        // Get the matching matter of the tangible object
                        foreach (MatterInstance matterInstance in tangibleObjectInstance.Matter)
                        {
                            if (matterInstance.IsNodeOf(this.Matter))
                            {
                                entities.Add(tangibleObjectInstance);
                                break;
                            }
                        }
                    }
                }
            }

            return entities.AsReadOnly();
        }
        #endregion Method: GetEntities(IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: MatterReferenceBase

    #region Class: LayerReferenceBase
    /// <summary>
    /// A reference to a layer.
    /// </summary>
    public sealed class LayerReferenceBase : ReferenceBase
    {

        #region Properties and Fields

        #region Property: Layer
        /// <summary>
        /// The layer.
        /// </summary>
        private MatterBase layer = null;
        
        /// <summary>
        /// Gets the layer.
        /// </summary>
        public MatterBase Layer
        {
            get
            {
                return layer;
            }
        }
        #endregion Property: Layer

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: LayerReferenceBase(LayerReference layerReference)
        /// <summary>
        /// Create a layer reference base from the given layer reference.
        /// </summary>
        /// <param name="layerReference">The layer reference to create a layer reference base from.</param>
        internal LayerReferenceBase(LayerReference layerReference)
            : base(layerReference)
        {
            if (layerReference != null)
                this.layer = BaseManager.Current.GetBase<MatterBase>(layerReference.Layer);
        }
        #endregion Constructor: LayerReferenceBase(LayerReference layerReference)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetEntities(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get the referenced entity instances.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>The referenced entity instances.</returns>
        public override ReadOnlyCollection<EntityInstance> GetEntities(IVariableInstanceHolder iVariableInstanceHolder)
        {
            List<EntityInstance> entities = new List<EntityInstance>();

            if (this.Layer != null)
            {
                foreach (EntityInstance subject in GetSubjects(iVariableInstanceHolder))
                {
                    TangibleObjectInstance tangibleObjectInstance = subject as TangibleObjectInstance;
                    if (tangibleObjectInstance != null)
                    {
                        // Get all the matching layers of the tangible object
                        foreach (MatterInstance layerInstance in tangibleObjectInstance.Layers)
                        {
                            if (layerInstance.IsNodeOf(this.Layer))
                                entities.Add(layerInstance);
                        }
                    }
                }
            }

            return entities.AsReadOnly();
        }
        #endregion Method: GetEntities(IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: LayerReferenceBase

    #region Class: SetReferenceBase
    /// <summary>
    /// A reference to a set of two other references.
    /// </summary>
    public sealed class SetReferenceBase : ReferenceBase
    {

        #region Properties and Fields

        #region Property: Reference1
        /// <summary>
        /// The first reference.
        /// </summary>
        private ReferenceBase reference1 = null;

        /// <summary>
        /// Gets the first reference.
        /// </summary>
        public ReferenceBase Reference1
        {
            get
            {
                return reference1;
            }
        }
        #endregion Property: Reference1

        #region Property: BinaryOperator
        /// <summary>
        /// The binary operator.
        /// </summary>
        private BinaryOperator binaryOperator = default(BinaryOperator);

        /// <summary>
        /// Gets the binary operator.
        /// </summary>
        public BinaryOperator BinaryOperator
        {
            get
            {
                return binaryOperator;
            }
        }
        #endregion Property: BinaryOperator

        #region Property: Reference2
        /// <summary>
        /// The second reference.
        /// </summary>
        private ReferenceBase reference2 = null;

        /// <summary>
        /// Gets the second reference.
        /// </summary>
        public ReferenceBase Reference2
        {
            get
            {
                return reference2;
            }
        }
        #endregion Property: Reference2

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SetReferenceBase(SetReference setReference)
        /// <summary>
        /// Create a set reference base from the given set reference.
        /// </summary>
        /// <param name="setReference">The set reference to create a set reference base from.</param>
        internal SetReferenceBase(SetReference setReference)
            : base(setReference)
        {
            if (setReference != null)
            {
                this.reference1 = BaseManager.Current.GetBase<ReferenceBase>(setReference.Reference1);
                this.binaryOperator = setReference.BinaryOperator;
                this.reference2 = BaseManager.Current.GetBase<ReferenceBase>(setReference.Reference2);
            }
        }
        #endregion Constructor: SetReferenceBase(SetReference setReference)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetEntities(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get the referenced entity instances.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>The referenced entity instances.</returns>
        public override ReadOnlyCollection<EntityInstance> GetEntities(IVariableInstanceHolder iVariableInstanceHolder)
        {
            List<EntityInstance> entities = new List<EntityInstance>();

            if (this.Reference1 != null && this.Reference2 != null)
            {
                ReadOnlyCollection<EntityInstance> entities1 = this.Reference1.GetEntities(iVariableInstanceHolder);
                ReadOnlyCollection<EntityInstance> entities2 = this.Reference2.GetEntities(iVariableInstanceHolder);

                switch (this.BinaryOperator)
                {
                    case BinaryOperator.Union:
                        // Add all entity instances
                        entities.AddRange(entities1);
                        foreach (EntityInstance entityInstance in entities2)
                        {
                            if (!entities.Contains(entityInstance))
                                entities.Add(entityInstance);
                        }
                        break;

                    case BinaryOperator.Intersection:
                        // Only add entity instances that are in both lists
                        foreach (EntityInstance entityInstance in entities1)
                        {
                            if (entities2.Contains(entityInstance))
                                entities.Add(entityInstance);
                        }
                        break;

                    case BinaryOperator.Complement:
                        // Only add entity instances from the first list that are not in the second
                        foreach (EntityInstance entityInstance in entities1)
                        {
                            if (!entities2.Contains(entityInstance))
                                entities.Add(entityInstance);
                        }
                        break;

                    case BinaryOperator.SymmetricDifference:
                        // Only add entity instances that are either in the first or the second list
                        foreach (EntityInstance entityInstance in entities1)
                        {
                            if (!entities2.Contains(entityInstance))
                                entities.Add(entityInstance);
                        }
                        foreach (EntityInstance entityInstance in entities2)
                        {
                            if (!entities1.Contains(entityInstance))
                                entities.Add(entityInstance);
                        }
                        break;

                    default:
                        break;
                }
            }

            return entities.AsReadOnly();
        }
        #endregion Method: GetEntities(IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: SetReferenceBase

    #region Class: OwnerReferenceBase
    /// <summary>
    /// A reference to the owner of the subject, like the whole of a part, the mixture of a substance, or the space of an item.
    /// </summary>
    public sealed class OwnerReferenceBase : ReferenceBase
    {

        #region Properties and Fields

        #region Property: OwnerType
        /// <summary>
        /// The type of the owner.
        /// </summary>
        private OwnerType ownerType = default(OwnerType);
        
        /// <summary>
        /// Gets the type of the owner.
        /// </summary>
        public OwnerType OwnerType
        {
            get
            {
                return ownerType;
            }
        }
        #endregion Property: OwnerType

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: OwnerReferenceBase(OwnerReference ownerReference)
        /// <summary>
        /// Create an owner reference base from the given owner reference.
        /// </summary>
        /// <param name="ownerReference">The owner reference to create an owner reference base from.</param>
        internal OwnerReferenceBase(OwnerReference ownerReference)
            : base(ownerReference)
        {
            if (ownerReference != null)
                this.ownerType = ownerReference.OwnerType;
        }
        #endregion Constructor: OwnerReferenceBase(OwnerReference ownerReference)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetEntities(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Get the referenced entity instances.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>The referenced entity instances.</returns>
        public override ReadOnlyCollection<EntityInstance> GetEntities(IVariableInstanceHolder iVariableInstanceHolder)
        {
            List<EntityInstance> entities = new List<EntityInstance>();

            foreach (EntityInstance subject in GetSubjects(iVariableInstanceHolder))
            {
                // Get the correct owner
                switch (this.OwnerType)
                {
                    case OwnerType.SpaceOfItem:
                        TangibleObjectInstance item = subject as TangibleObjectInstance;
                        if (item != null && item.Space != null)
                            entities.Add(item.Space);
                        break;
                    case OwnerType.WholeOfPart:
                        TangibleObjectInstance part = subject as TangibleObjectInstance;
                        if (part != null && part.Whole != null)
                            entities.Add(part.Whole);
                        break;
                    case OwnerType.PhysicalObjectOfSpace:
                        SpaceInstance space = subject as SpaceInstance;
                        if (space != null && space.PhysicalObject != null)
                            entities.Add(space.PhysicalObject);
                        break;
                    case OwnerType.CompoundOfSubstance:
                        SubstanceInstance substance = subject as SubstanceInstance;
                        if (substance != null && substance.Compound != null)
                            entities.Add(substance.Compound);
                        break;
                    case OwnerType.MixtureOfSubstance:
                        SubstanceInstance substance2 = subject as SubstanceInstance;
                        if (substance2 != null && substance2.Mixture != null)
                            entities.Add(substance2.Mixture);
                        break;
                    case OwnerType.CoveredObjectOfCover:
                        TangibleObjectInstance cover = subject as TangibleObjectInstance;
                        if (cover != null && cover.CoveredObject != null)
                            entities.Add(cover.CoveredObject);
                        break;
                    case OwnerType.ApplicantOfLayer:
                        MatterInstance layer = subject as MatterInstance;
                        if (layer != null && layer.Applicant != null)
                            entities.Add(layer.Applicant);
                        break;
                    case OwnerType.TangibleObjectOfMatter:
                        MatterInstance matter = subject as MatterInstance;
                        if (matter != null && matter.TangibleObject != null)
                            entities.Add(matter.TangibleObject);
                        break;
                    case OwnerType.SpaceOfMatter:
                        MatterInstance tangibleMatter = subject as MatterInstance;
                        if (tangibleMatter != null && tangibleMatter.Space != null)
                            entities.Add(tangibleMatter.Space);
                        break;
                    default:
                        break;
                }
            }

            return entities.AsReadOnly();
        }
        #endregion Method: GetEntities(IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: OwnerReferenceBase

}