/**************************************************************************
 * 
 * RangeBase.cs
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
using Semantics.Abstractions;
using Semantics.Components;
using Semantics.Entities;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Entities;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: RangeBase
    /// <summary>
    /// An base of a range.
    /// </summary>
    public sealed class RangeBase : Base
    {

        #region Properties and Fields

        #region Property: Range
        /// <summary>
        /// Gets the range this is a base of.
        /// </summary>
        internal Range Range
        {
            get
            {
                return this.IdHolder as Range;
            }
        }
        #endregion Property: Range

        #region Property: RangeType
        /// <summary>
        /// The type of the range.
        /// </summary>
        private RangeType rangeType = default(RangeType);

        /// <summary>
        /// Gets the type of the range.
        /// </summary>
        public RangeType RangeType
        {
            get
            {
                return rangeType;
            }
        }
        #endregion Property: RangeType

        #region Property: Radius
        /// <summary>
        /// The radius that defines the range. Only used for RangeType 'RadiusOnly' or 'EntitiesInRadius'.
        /// </summary>
        private NumericalValueRangeBase radius = null;

        /// <summary>
        /// Gets the radius that defines the range. Only used for RangeType 'RadiusOnly' or 'EntitiesInRadius'.
        /// </summary>
        public NumericalValueRangeBase Radius
        {
            get
            {
                if (radius == null)
                {
                    LoadRadius();
                    if (radius == null)
                        radius = new NumericalValueRangeBase(SemanticsSettings.Values.Range);
                    if (radius.Unit == null)
                    {
                        UnitCategoryBase distanceUnitCategoryBase = Utils.GetSpecialUnitCategory(SpecialUnitCategories.Distance);
                        if (distanceUnitCategoryBase != null)
                            radius.Unit = distanceUnitCategoryBase.BaseUnit;
                    }
                }
                return radius;
            }
        }

        /// <summary>
        /// Loads the radius.
        /// </summary>
        private void LoadRadius()
        {
            if (this.Range != null)
                radius = BaseManager.Current.GetBase<NumericalValueRangeBase>(this.Range.Radius);
        }
        #endregion Property: Radius

        #region Property: RadiusOrigin
        /// <summary>
        /// The origin of the radius. Only used for RangeType 'RadiusOnly' or 'EntitiesInRadius'.
        /// </summary>
        private ActorTargetArtifact radiusOrigin = default(ActorTargetArtifact);

        /// <summary>
        /// Gets the origin of the radius. Only used for RangeType 'RadiusOnly' or 'EntitiesInRadius'.
        /// </summary>
        public ActorTargetArtifact RadiusOrigin
        {
            get
            {
                return radiusOrigin;
            }
        }
        #endregion Property: RadiusOrigin

        #region Property: MaximumNumberOfTargets
        /// <summary>
        /// The maximum number of targets in the range.
        /// </summary>
        private uint? maximumNumberOfTargets = null;

        /// <summary>
        /// Gets the maximum number of targets in the range.
        /// </summary>
        public uint? MaximumNumberOfTargets
        {
            get
            {
                return maximumNumberOfTargets;
            }
        }
        #endregion Property: MaximumNumberOfTargets

        #region Property: ExcludeActor
        /// <summary>
        /// Indicates whether the actor is excluded from the range. Only used for RangeType 'RadiusOnly' or 'EntitiesInRadius'.
        /// </summary>
        private bool excludeActor = false;

        /// <summary>
        /// Gets the value that indicates whether the actor is excluded from the range. Only used for RangeType 'RadiusOnly' or 'EntitiesInRadius'.
        /// </summary>
        public bool ExcludeActor
        {
            get
            {
                return excludeActor;
            }
        }
        #endregion Property: ExcludeActor

        #region Property: IncludeActor
        /// <summary>
        /// Indicates whether the actor is included in the range.
        /// </summary>
        private bool includeActor = false;

        /// <summary>
        /// Gets the value that indicates whether the actor is included in the range.
        /// </summary>
        public bool IncludeActor
        {
            get
            {
                return includeActor;
            }
        }
        #endregion Property: IncludeActor

        #region Property: IncludeActorConnections
        /// <summary>
        /// Indicates whether the connections of the actor are included in the range.
        /// </summary>
        private bool includeActorConnections = false;

        /// <summary>
        /// Gets the value that indicates whether the connections of the actor are included in the range.
        /// </summary>
        public bool IncludeActorConnections
        {
            get
            {
                return includeActorConnections;
            }
        }
        #endregion Property: IncludeActorConnections

        #region Property: IncludeActorParts
        /// <summary>
        /// Indicates whether the parts of the actor are included in the range.
        /// </summary>
        private bool includeActorParts = false;

        /// <summary>
        /// Gets the value that indicates whether the parts of the actor are included in the range.
        /// </summary>
        public bool IncludeActorParts
        {
            get
            {
                return includeActorParts;
            }
        }
        #endregion Property: IncludeActorParts

        #region Property: IncludeActorWhole
        /// <summary>
        /// Indicates whether the whole of the actor is included in the range.
        /// </summary>
        private bool includeActorWhole = false;

        /// <summary>
        /// Gets the value that indicates whether the whole of the actor is included in the range.
        /// </summary>
        public bool IncludeActorWhole
        {
            get
            {
                return includeActorWhole;
            }
        }
        #endregion Property: IncludeActorWhole

        #region Property: IncludeActorSpaceItems
        /// <summary>
        /// Indicates whether the items of all spaces of the actor are included in the range.
        /// </summary>
        private bool includeActorSpaceItems = false;

        /// <summary>
        /// Gets the value that indicates whether the items of all spaces of the actor are included in the range.
        /// </summary>
        public bool IncludeActorSpaceItems
        {
            get
            {
                return includeActorSpaceItems;
            }
        }
        #endregion Property: IncludeActorSpaceItems

        #region Property: IncludeActorSpaceTangibleMatter
        /// <summary>
        /// Indicates whether the tangible matter of all spaces of the actor is included in the range.
        /// </summary>
        private bool includeActorSpaceTangibleMatter = false;

        /// <summary>
        /// Gets the value that indicates whether the tangible matter of all spaces of the actor is included in the range.
        /// </summary>
        public bool IncludeActorSpaceTangibleMatter
        {
            get
            {
                return includeActorSpaceTangibleMatter;
            }
        }
        #endregion Property: IncludeActorSpaceTangibleMatter

        #region Property: IncludeActorMatter
        /// <summary>
        /// Indicates whether the matter of the actor is included in the range.
        /// </summary>
        private bool includeActorMatter = false;

        /// <summary>
        /// Gets the value that indicates whether the matter of the actor is included in the range.
        /// </summary>
        public bool IncludeActorMatter
        {
            get
            {
                return includeActorMatter;
            }
        }
        #endregion Property: IncludeActorMatter

        #region Property: IncludeActorCovers
        /// <summary>
        /// Indicates whether the covers of the actor are included in the range.
        /// </summary>
        private bool includeActorCovers = false;

        /// <summary>
        /// Gets the value that indicates whether the covers of the actor are included in the range.
        /// </summary>
        public bool IncludeActorCovers
        {
            get
            {
                return includeActorCovers;
            }
        }
        #endregion Property: IncludeActorCovers

        #region Property: IncludeActorCoveredObject
        /// <summary>
        /// The value that indicates whether the covered object of the actor is included in the range.
        /// </summary>
        private bool includeActorCoveredObject = false;
        
        /// <summary>
        /// Gets the value that indicates whether the covered object of the actor is included in the range.
        /// </summary>
        public bool IncludeActorCoveredObject
        {
            get
            {
                return includeActorCoveredObject;
            }
        }
        #endregion Property: IncludeActorCoveredObject

        #region Property: IncludeSourceOfRelationshipWhereActorIsTarget
        /// <summary>
        /// The relationship type of which the source should be included in the range, in case the actor is the target of the relationship.
        /// </summary>
        private RelationshipTypeBase includeSourceOfRelationshipWhereActorIsTarget = null;
        
        /// <summary>
        /// Gets the relationship type of which the source should be included in the range, in case the actor is the target of the relationship.
        /// </summary>
        public RelationshipTypeBase IncludeSourceOfRelationshipWhereActorIsTarget
        {
            get
            {
                return includeSourceOfRelationshipWhereActorIsTarget;
            }
        }
        #endregion Property: IncludeSourceOfRelationshipWhereActorIsTarget

        #region Property: IncludeTargetOfRelationshipWhereActorIsSource
        /// <summary>
        /// The relationship type of which the target should be included in the range, in case the actor is the source of the relationship.
        /// </summary>
        private RelationshipTypeBase includeTargetOfRelationshipWhereActorIsSource = null;
        
        /// <summary>
        /// Gets the relationship type of which the target should be included in the range, in case the actor is the source of the relationship.
        /// </summary>
        public RelationshipTypeBase IncludeTargetOfRelationshipWhereActorIsSource
        {
            get
            {
                return includeTargetOfRelationshipWhereActorIsSource;
            }
        }
        #endregion Property: IncludeTargetOfRelationshipWhereActorIsSource

        #region Property: IncludeTarget
        /// <summary>
        /// Indicates whether the target is included in the range.
        /// </summary>
        private bool includeTarget = false;

        /// <summary>
        /// Gets the value that indicates whether the target is included in the range.
        /// </summary>
        public bool IncludeTarget
        {
            get
            {
                return includeTarget;
            }
        }
        #endregion Property: IncludeTarget

        #region Property: IncludeTargetConnections
        /// <summary>
        /// Indicates whether the connections of the target are included in the range.
        /// </summary>
        private bool includeTargetConnections = false;

        /// <summary>
        /// Gets the value that indicates whether the connections of the target are included in the range.
        /// </summary>
        public bool IncludeTargetConnections
        {
            get
            {
                return includeTargetConnections;
            }
        }
        #endregion Property: IncludeTargetConnections

        #region Property: IncludeTargetParts
        /// <summary>
        /// Indicates whether the parts of the target are included in the range.
        /// </summary>
        private bool includeTargetParts = false;

        /// <summary>
        /// Gets the value that indicates whether the parts of the target are included in the range.
        /// </summary>
        public bool IncludeTargetParts
        {
            get
            {
                return includeTargetParts;
            }
        }
        #endregion Property: IncludeTargetParts

        #region Property: IncludeTargetWhole
        /// <summary>
        /// Indicates whether the whole of the target is included in the range.
        /// </summary>
        private bool includeTargetWhole = false;

        /// <summary>
        /// Gets the value that indicates whether the whole of the target is included in the range.
        /// </summary>
        public bool IncludeTargetWhole
        {
            get
            {
                return includeTargetWhole;
            }
        }
        #endregion Property: IncludeTargetWhole

        #region Property: IncludeTargetSpaceItems
        /// <summary>
        /// Indicates whether the items of all spaces of the target are included in the range.
        /// </summary>
        private bool includeTargetSpaceItems = false;

        /// <summary>
        /// Gets the value that indicates whether the items of all spaces of the target are included in the range.
        /// </summary>
        public bool IncludeTargetSpaceItems
        {
            get
            {
                return includeTargetSpaceItems;
            }
        }
        #endregion Property: IncludeTargetSpaceItems

        #region Property: IncludeTargetSpaceTangibleMatter
        /// <summary>
        /// Indicates whether the tangible matter of all spaces of the target is included in the range.
        /// </summary>
        private bool includeTargetSpaceTangibleMatter = false;

        /// <summary>
        /// Gets the value that indicates whether the tangible matter of all spaces of the target is included in the range.
        /// </summary>
        public bool IncludeTargetSpaceTangibleMatter
        {
            get
            {
                return includeTargetSpaceTangibleMatter;
            }
        }
        #endregion Property: IncludeTargetSpaceTangibleMatter

        #region Property: IncludeTargetMatter
        /// <summary>
        /// Indicates whether the matter of the target is included in the range.
        /// </summary>
        private bool includeTargetMatter = false;

        /// <summary>
        /// Gets the value that indicates whether the matter of the target is included in the range.
        /// </summary>
        public bool IncludeTargetMatter
        {
            get
            {
                return includeTargetMatter;
            }
        }
        #endregion Property: IncludeTargetMatter

        #region Property: IncludeTargetCovers
        /// <summary>
        /// Indicates whether the covers of the target are included in the range.
        /// </summary>
        private bool includeTargetCovers = false;

        /// <summary>
        /// Gets the value that indicates whether the covers of the target are included in the range.
        /// </summary>
        public bool IncludeTargetCovers
        {
            get
            {
                return includeTargetCovers;
            }
        }
        #endregion Property: IncludeTargetCovers

        #region Property: IncludeTargetCoveredObject
        /// <summary>
        /// The value that indicates whether the covered object of the target is included in the range.
        /// </summary>
        private bool includeTargetCoveredObject = false;
        
        /// <summary>
        /// Gets the value that indicates whether the covered object of the target is included in the range.
        /// </summary>
        public bool IncludeTargetCoveredObject
        {
            get
            {
                return includeTargetCoveredObject;
            }
        }
        #endregion Property: IncludeTargetCoveredObject

        #region Property: IncludeSourceOfRelationshipWhereTargetIsTarget
        /// <summary>
        /// The relationship type of which the source should be included in the range, in case the target is the target of the relationship.
        /// </summary>
        private RelationshipType includeSourceOfRelationshipWhereTargetIsTarget = null;
        
        /// <summary>
        /// Gets the relationship type of which the source should be included in the range, in case the target is the target of the relationship.
        /// </summary>
        public RelationshipType IncludeSourceOfRelationshipWhereTargetIsTarget
        {
            get
            {
                return includeSourceOfRelationshipWhereTargetIsTarget;
            }
        }
        #endregion Property: IncludeSourceOfRelationshipWhereTargetIsTarget

        #region Property: IncludeTargetOfRelationshipWhereTargetIsSource
        /// <summary>
        /// The relationship type of which the target should be included in the range, in case the target is the source of the relationship.
        /// </summary>
        private RelationshipType includeTargetOfRelationshipWhereTargetIsSource = null;
        
        /// <summary>
        /// Gets the relationship type of which the target should be included in the range, in case the target is the source of the relationship.
        /// </summary>
        public RelationshipType IncludeTargetOfRelationshipWhereTargetIsSource
        {
            get
            {
                return includeTargetOfRelationshipWhereTargetIsSource;
            }
        }
        #endregion Property: IncludeTargetOfRelationshipWhereTargetIsSource

        #region Property: IncludeArtifact
        /// <summary>
        /// Indicates whether the artifact is included in the range.
        /// </summary>
        private bool includeArtifact = false;

        /// <summary>
        /// Gets the value that indicates whether the artifact is included in the range.
        /// </summary>
        public bool IncludeArtifact
        {
            get
            {
                return includeArtifact;
            }
        }
        #endregion Property: IncludeArtifact

        #region Property: IncludeArtifactConnections
        /// <summary>
        /// Indicates whether the connections of the artifact are included in the range.
        /// </summary>
        private bool includeArtifactConnections = false;

        /// <summary>
        /// Gets the value that indicates whether the connections of the artifact are included in the range.
        /// </summary>
        public bool IncludeArtifactConnections
        {
            get
            {
                return includeArtifactConnections;
            }
        }
        #endregion Property: IncludeArtifactConnections

        #region Property: IncludeArtifactParts
        /// <summary>
        /// Indicates whether the parts of the artifact are included in the range.
        /// </summary>
        private bool includeArtifactParts = false;

        /// <summary>
        /// Gets the value that indicates whether the parts of the artifact are included in the range.
        /// </summary>
        public bool IncludeArtifactParts
        {
            get
            {
                return includeArtifactParts;
            }
        }
        #endregion Property: IncludeArtifactParts

        #region Property: IncludeArtifactWhole
        /// <summary>
        /// Indicates whether the whole of the artifact is included in the range.
        /// </summary>
        private bool includeArtifactWhole = false;

        /// <summary>
        /// Gets the value that indicates whether the whole of the artifact is included in the range.
        /// </summary>
        public bool IncludeArtifactWhole
        {
            get
            {
                return includeArtifactWhole;
            }
        }
        #endregion Property: IncludeArtifactWhole

        #region Property: IncludeArtifactSpaceItems
        /// <summary>
        /// Indicates whether the items of all spaces of the artifact are included in the range.
        /// </summary>
        private bool includeArtifactSpaceItems = false;

        /// <summary>
        /// Gets the value that indicates whether the items of all spaces of the artifact are included in the range.
        /// </summary>
        public bool IncludeArtifactSpaceItems
        {
            get
            {
                return includeArtifactSpaceItems;
            }
        }
        #endregion Property: IncludeArtifactSpaceItems

        #region Property: IncludeArtifactSpaceTangibleMatter
        /// <summary>
        /// Indicates whether the tangible matter of all spaces of the artifact is included in the range.
        /// </summary>
        private bool includeArtifactSpaceTangibleMatter = false;

        /// <summary>
        /// Gets the value that indicates whether the tangible matter of all spaces of the artifact is included in the range.
        /// </summary>
        public bool IncludeArtifactSpaceTangibleMatter
        {
            get
            {
                return includeArtifactSpaceTangibleMatter;
            }
        }
        #endregion Property: IncludeArtifactSpaceTangibleMatter

        #region Property: IncludeArtifactMatter
        /// <summary>
        /// Indicates whether the matter of the artifact is included in the range.
        /// </summary>
        private bool includeArtifactMatter = false;

        /// <summary>
        /// Gets the value that indicates whether the matter of the artifact is included in the range.
        /// </summary>
        public bool IncludeArtifactMatter
        {
            get
            {
                return includeArtifactMatter;
            }
        }
        #endregion Property: IncludeArtifactMatter

        #region Property: IncludeArtifactCovers
        /// <summary>
        /// Indicates whether the covers of the artifact are included in the range.
        /// </summary>
        private bool includeArtifactCovers = false;

        /// <summary>
        /// Gets the value that indicates whether the covers of the artifact are included in the range.
        /// </summary>
        public bool IncludeArtifactCovers
        {
            get
            {
                return includeArtifactCovers;
            }
        }
        #endregion Property: IncludeArtifactCovers

        #region Property: IncludeArtifactCoveredObject
        /// <summary>
        /// The value that indicates whether the covered object of the artifact is included in the range.
        /// </summary>
        private bool includeArtifactCoveredObject = false;

        /// <summary>
        /// Gets the value that indicates whether the covered object of the artifact is included in the range.
        /// </summary>
        public bool IncludeArtifactCoveredObject
        {
            get
            {
                return includeArtifactCoveredObject;
            }
        }
        #endregion Property: IncludeArtifactCoveredObject

        #region Property: IncludeSourceOfRelationshipWhereArtifactIsTarget
        /// <summary>
        /// The relationship type of which the source should be included in the range, in case the artifact is the target of the relationship.
        /// </summary>
        private RelationshipTypeBase includeSourceOfRelationshipWhereArtifactIsTarget = null;

        /// <summary>
        /// Gets the relationship type of which the source should be included in the range, in case the artifact is the target of the relationship.
        /// </summary>
        public RelationshipTypeBase IncludeSourceOfRelationshipWhereArtifactIsTarget
        {
            get
            {
                return includeSourceOfRelationshipWhereArtifactIsTarget;
            }
        }
        #endregion Property: IncludeSourceOfRelationshipWhereArtifactIsTarget

        #region Property: IncludeTargetOfRelationshipWhereArtifactIsSource
        /// <summary>
        /// The relationship type of which the target should be included in the range, in case the artifact is the source of the relationship.
        /// </summary>
        private RelationshipTypeBase includeTargetOfRelationshipWhereArtifactIsSource = null;

        /// <summary>
        /// Gets the relationship type of which the target should be included in the range, in case the artifact is the source of the relationship.
        /// </summary>
        public RelationshipTypeBase IncludeTargetOfRelationshipWhereArtifactIsSource
        {
            get
            {
                return includeTargetOfRelationshipWhereArtifactIsSource;
            }
        }
        #endregion Property: IncludeTargetOfRelationshipWhereArtifactIsSource

        #region Property: SpecificTargets
        /// <summary>
        /// The specific targets that are included in the range.
        /// </summary>
        private EntityConditionBase[] specificTargets = null;

        /// <summary>
        /// Gets the specific targets that are included in the range.
        /// </summary>
        public IEnumerable<EntityConditionBase> SpecificTargets
        {
            get
            {
                if (specificTargets == null)
                    LoadSpecificTargets();

                foreach (EntityConditionBase specificTarget in specificTargets)
                    yield return specificTarget;
            }
        }

        /// <summary>
        /// Loads the specific targets.
        /// </summary>
        private void LoadSpecificTargets()
        {
            if (this.Range != null)
            {
                List<EntityConditionBase> entityConditionBases = new List<EntityConditionBase>();
                foreach (EntityCondition entityCondition in this.Range.SpecificTargets)
                    entityConditionBases.Add(BaseManager.Current.GetBase<EntityConditionBase>(entityCondition));
                specificTargets = entityConditionBases.ToArray();
            }
        }
        #endregion Property: SpecificTargets

        #region Property: IncludeSpecificTargetsConnections
        /// <summary>
        /// Indicates whether the connections of the specific targets are included in the range.
        /// </summary>
        private bool includeSpecificTargetsConnections = false;

        /// <summary>
        /// Gets the value that indicates whether the connections of the specific targets are included in the range.
        /// </summary>
        public bool IncludeSpecificTargetsConnections
        {
            get
            {
                return includeSpecificTargetsConnections;
            }
        }
        #endregion Property: IncludeSpecificTargetsConnections

        #region Property: IncludeSpecificTargetsParts
        /// <summary>
        /// Indicates whether the parts of the specific targets are included in the range.
        /// </summary>
        private bool includeSpecificTargetsParts = false;

        /// <summary>
        /// Gets the value that indicates whether the parts of the specific targets are included in the range.
        /// </summary>
        public bool IncludeSpecificTargetsParts
        {
            get
            {
                return includeSpecificTargetsParts;
            }
        }
        #endregion Property: IncludeSpecificTargetsParts

        #region Property: IncludeSpecificTargetsWhole
        /// <summary>
        /// Indicates whether the whole of each specific targets is included in the range.
        /// </summary>
        private bool includeSpecificTargetsWhole = false;

        /// <summary>
        /// Gets the value that indicates whether the whole of each specific targets is included in the range.
        /// </summary>
        public bool IncludeSpecificTargetsWhole
        {
            get
            {
                return includeSpecificTargetsWhole;
            }
        }
        #endregion Property: IncludeSpecificTargetsWhole

        #region Property: IncludeSpecificTargetsSpaceItems
        /// <summary>
        /// Indicates whether the items in the spaces of the specific targets are included in the range.
        /// </summary>
        private bool includeSpecificTargetsSpaceItems = false;

        /// <summary>
        /// Gets the value that indicates whether the items in the spaces of the specific targets are included in the range.
        /// </summary>
        public bool IncludeSpecificTargetsSpaceItems
        {
            get
            {
                return includeSpecificTargetsSpaceItems;
            }
        }
        #endregion Property: IncludeSpecificTargetsSpaceItems

        #region Property: IncludeSpecificTargetsSpaceTangibleMatter
        /// <summary>
        /// Indicates whether the tangible matter of all spaces of the specific targets is included in the range.
        /// </summary>
        private bool includeSpecificTargetsSpaceTangibleMatter = false;

        /// <summary>
        /// Gets the value that indicates whether the tangible matter of all spaces of the specific targets is included in the range.
        /// </summary>
        public bool IncludeSpecificTargetsSpaceTangibleMatter
        {
            get
            {
                return includeSpecificTargetsSpaceTangibleMatter;
            }
        }
        #endregion Property: IncludeSpecificTargetsSpaceTangibleMatter

        #region Property: IncludeSpecificTargetsMatter
        /// <summary>
        /// Indicates whether the matter of the specific targets is included in the range.
        /// </summary>
        private bool includeSpecificTargetsMatter = false;

        /// <summary>
        /// Gets the value that indicates whether the matter of the specific targets is included in the range.
        /// </summary>
        public bool IncludeSpecificTargetsMatter
        {
            get
            {
                return includeSpecificTargetsMatter;
            }
        }
        #endregion Property: IncludeSpecificTargetsMatter

        #region Property: IncludeSpecificTargetsCovers
        /// <summary>
        /// Indicates whether the covers of the specific targets are included in the range.
        /// </summary>
        private bool includeSpecificTargetsCovers = false;

        /// <summary>
        /// Gets the value that indicates whether the covers of the specific targets are included in the range.
        /// </summary>
        public bool IncludeSpecificTargetsCovers
        {
            get
            {
                return includeSpecificTargetsCovers;
            }
        }
        #endregion Property: IncludeSpecificTargetsCovers

        #region Property: IncludeSpecificTargetsCoveredObject
        /// <summary>
        /// The value that indicates whether the covered object of each specific target is included in the range.
        /// </summary>
        private bool includeSpecificTargetsCoveredObject = false;
        
        /// <summary>
        /// Gets the value that indicates whether the covered object of each specific target is included in the range.
        /// </summary>
        public bool IncludeSpecificTargetsCoveredObject
        {
            get
            {
                return includeSpecificTargetsCoveredObject;
            }
        }
        #endregion Property: IncludeSpecificTargetsCoveredObject

        #region Property: IncludeSourceOfRelationshipWhereSpecificTargetIsTarget
        /// <summary>
        /// The relationship type of which the source should be included in the range, in case the specific target is the target of the relationship.
        /// </summary>
        private RelationshipType includeSourceOfRelationshipWhereSpecificTargetIsTarget = null;
        
        /// <summary>
        /// Gets the relationship type of which the source should be included in the range, in case the specific target is the target of the relationship.
        /// </summary>
        public RelationshipType IncludeSourceOfRelationshipWhereSpecificTargetIsTarget
        {
            get
            {
                return includeSourceOfRelationshipWhereSpecificTargetIsTarget;
            }
        }
        #endregion Property: IncludeSourceOfRelationshipWhereSpecificTargetIsTarget

        #region Property: IncludeTargetOfRelationshipWhereSpecificTargetIsSource
        /// <summary>
        /// The relationship type of which the target should be included in the range, in case the specific target is the source of the relationship.
        /// </summary>
        private RelationshipType includeTargetOfRelationshipWhereSpecificTargetIsSource = null;
        
        /// <summary>
        /// Gets the relationship type of which the target should be included in the range, in case the specific target is the source of the relationship.
        /// </summary>
        public RelationshipType IncludeTargetOfRelationshipWhereSpecificTargetIsSource
        {
            get
            {
                return includeTargetOfRelationshipWhereSpecificTargetIsSource;
            }
        }
        #endregion Property: IncludeTargetOfRelationshipWhereSpecificTargetIsSource

        #region Property: IncludeItemsInSpaceOfActor
        /// <summary>
        /// The value that indicates whether the items in the space of the actor are included in the range.
        /// </summary>
        private bool includeItemsInSpaceOfActor = false;
        
        /// <summary>
        /// Gets the value that indicates whether the items in the space of the actor are included in the range.
        /// </summary>
        public bool IncludeItemsInSpaceOfActor
        {
            get
            {
                return includeItemsInSpaceOfActor;
            }
        }
        #endregion Property: IncludeItemsInSpaceOfArtifacts

        #region Property: IncludeItemsInSpaceOfTarget
        /// <summary>
        /// The value that indicates whether the items in the space of the target are included in the range.
        /// </summary>
        private bool includeItemsInSpaceOfTarget = false;
        
        /// <summary>
        /// Gets the value that indicates whether the items in the space of the target are included in the range.
        /// </summary>
        public bool IncludeItemsInSpaceOfTarget
        {
            get
            {
                return includeItemsInSpaceOfTarget;
            }
        }
        #endregion Property: IncludeItemsInSpaceOfTarget

        #region Property: IncludeItemsInSpaceOfArtifact
        /// <summary>
        /// The value that indicates whether the items in the space of the artifact are included in the range.
        /// </summary>
        private bool includeItemsInSpaceOfArtifact = false;

        /// <summary>
        /// Gets the value that indicates whether the items in the space of the artifact are included in the range.
        /// </summary>
        public bool IncludeItemsInSpaceOfArtifact
        {
            get
            {
                return includeItemsInSpaceOfArtifact;
            }
        }
        #endregion Property: IncludeItemsInSpaceOfArtifacts

        #region Property: IncludeItemsInSpaceOfSpecificTargets
        /// <summary>
        /// The value that indicates whether the items in the spaces of the specific targets are included in the range.
        /// </summary>
        private bool includeItemsInSpaceOfSpecificTargets = false;
        
        /// <summary>
        /// Gets the value that indicates whether the items in the spaces of the specific targets are included in the range.
        /// </summary>
        public bool IncludeItemsInSpaceOfSpecificTargets
        {
            get
            {
                return includeItemsInSpaceOfSpecificTargets;
            }
        }
        #endregion Property: IncludeItemsInSpaceOfSpecificTargets

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: RangeBase(Range range)
        /// <summary>
        /// Creates a new range base from the given range.
        /// </summary>
        /// <param name="range">The range to create a base from.</param>
        internal RangeBase(Range range)
            : base(range)
        {
            if (range != null)
            {
                this.rangeType = range.RangeType;
                this.radiusOrigin = range.RadiusOrigin;
                this.maximumNumberOfTargets = range.MaximumNumberOfTargets;
                this.excludeActor = range.ExcludeActor;
                this.includeActor = range.IncludeActor;
                this.includeActorConnections = range.IncludeActorConnections;
                this.includeActorParts = range.IncludeActorParts;
                this.includeActorWhole = range.IncludeActorWhole;
                this.includeActorSpaceItems = range.IncludeActorSpaceItems;
                this.includeActorSpaceTangibleMatter = range.IncludeActorSpaceTangibleMatter;
                this.includeActorMatter = range.IncludeActorMatter;
                this.includeActorCovers = range.IncludeActorCovers;
                this.includeActorCoveredObject = range.IncludeActorCoveredObject;
                this.includeSourceOfRelationshipWhereActorIsTarget = BaseManager.Current.GetBase<RelationshipTypeBase>(range.IncludeSourceOfRelationshipWhereActorIsTarget);
                this.includeTargetOfRelationshipWhereActorIsSource = BaseManager.Current.GetBase<RelationshipTypeBase>(range.IncludeTargetOfRelationshipWhereActorIsSource);
                this.includeTarget = range.IncludeTarget;
                this.includeTargetConnections = range.IncludeTargetConnections;
                this.includeTargetParts = range.IncludeTargetParts;
                this.includeTargetWhole = range.IncludeTargetWhole;
                this.includeTargetSpaceItems = range.IncludeTargetSpaceItems;
                this.includeTargetSpaceTangibleMatter = range.IncludeTargetSpaceTangibleMatter;
                this.includeTargetMatter = range.IncludeTargetMatter;
                this.includeTargetCovers = range.IncludeTargetCovers;
                this.includeTargetCoveredObject = range.IncludeTargetCoveredObject;
                this.includeSourceOfRelationshipWhereTargetIsTarget = range.IncludeSourceOfRelationshipWhereTargetIsTarget;
                this.includeTargetOfRelationshipWhereTargetIsSource = range.IncludeTargetOfRelationshipWhereTargetIsSource;
                this.includeArtifact = range.IncludeArtifact;
                this.includeArtifactConnections = range.IncludeArtifactConnections;
                this.includeArtifactParts = range.IncludeArtifactParts;
                this.includeArtifactWhole = range.IncludeArtifactWhole;
                this.includeArtifactSpaceItems = range.IncludeArtifactSpaceItems;
                this.includeArtifactSpaceTangibleMatter = range.IncludeArtifactSpaceTangibleMatter;
                this.includeArtifactMatter = range.IncludeArtifactMatter;
                this.includeArtifactCovers = range.IncludeArtifactCovers;
                this.includeArtifactCoveredObject = range.IncludeArtifactCoveredObject;
                this.includeSourceOfRelationshipWhereArtifactIsTarget = BaseManager.Current.GetBase<RelationshipTypeBase>(range.IncludeSourceOfRelationshipWhereArtifactIsTarget);
                this.includeTargetOfRelationshipWhereArtifactIsSource = BaseManager.Current.GetBase<RelationshipTypeBase>(range.IncludeTargetOfRelationshipWhereArtifactIsSource);
                this.includeSpecificTargetsConnections = range.IncludeSpecificTargetsConnections;
                this.includeSpecificTargetsParts = range.IncludeSpecificTargetsParts;
                this.includeSpecificTargetsWhole = range.IncludeSpecificTargetsWhole;
                this.includeSpecificTargetsSpaceItems = range.IncludeSpecificTargetsSpaceItems;
                this.includeSpecificTargetsSpaceTangibleMatter = range.IncludeSpecificTargetsSpaceTangibleMatter;
                this.includeSpecificTargetsMatter = range.IncludeSpecificTargetsMatter;
                this.includeSpecificTargetsCovers = range.IncludeSpecificTargetsCovers;
                this.includeSpecificTargetsCoveredObject = range.IncludeSpecificTargetsCoveredObject;
                this.includeSourceOfRelationshipWhereSpecificTargetIsTarget = range.IncludeSourceOfRelationshipWhereSpecificTargetIsTarget;
                this.includeTargetOfRelationshipWhereSpecificTargetIsSource = range.IncludeTargetOfRelationshipWhereSpecificTargetIsSource;
                this.includeItemsInSpaceOfActor = range.IncludeItemsInSpaceOfActor;
                this.includeItemsInSpaceOfTarget = range.IncludeItemsInSpaceOfTarget;
                this.includeItemsInSpaceOfArtifact = range.IncludeItemsInSpaceOfArtifact;
                this.includeItemsInSpaceOfSpecificTargets = range.IncludeItemsInSpaceOfSpecificTargets;

                if (BaseManager.PreloadProperties)
                {
                    LoadRadius();
                    LoadSpecificTargets();
                }
            }
        }
        #endregion Constructor: RangeBase(Range range)

        #endregion Method Group: Constructors

    }
    #endregion Class: RangeBase

}