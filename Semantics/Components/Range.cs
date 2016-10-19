/**************************************************************************
 * 
 * Range.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Common;
using Semantics.Abstractions;
using Semantics.Data;
using Semantics.Entities;
using Semantics.Utilities;

namespace Semantics.Components
{

    #region Class: Range
    /// <summary>
    /// A range.
    /// </summary>
    public sealed class Range : IdHolder
    {

        #region Properties and Fields

        #region Property: RangeType
        /// <summary>
        /// Gets or sets the type of the range.
        /// </summary>
        public RangeType RangeType
        {
            get
            {
                return Database.Current.Select<RangeType>(this.ID, ValueTables.Range, Columns.RangeType);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.RangeType, value);
                NotifyPropertyChanged("RangeType");
            }
        }
        #endregion Property: RangeType

        #region Property: Radius
        /// <summary>
        /// Gets or sets the radius that defines the range. Only used for RangeType 'RadiusOnly' or 'EntitiesInRadius'.
        /// </summary>
        public NumericalValueRange Radius
        {
            get
            {
                return Database.Current.Select<NumericalValueRange>(this.ID, ValueTables.Range, Columns.Radius);
            }
            private set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.Radius, value);
                NotifyPropertyChanged("Radius");
            }
        }
        #endregion Property: Radius

        #region Property: RadiusOrigin
        /// <summary>
        /// Gets or sets the origin of the radius. Only used for RangeType 'RadiusOnly' or 'EntitiesInRadius'.
        /// </summary>
        public ActorTargetArtifact RadiusOrigin
        {
            get
            {
                return Database.Current.Select<ActorTargetArtifact>(this.ID, ValueTables.Range, Columns.RadiusOrigin);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.RadiusOrigin, value);
                NotifyPropertyChanged("RadiusOrigin");
            }
        }
        #endregion Property: RadiusOrigin

        #region Property: MaximumNumberOfTargets
        /// <summary>
        /// Gets or sets the maximum number of targets in the range.
        /// </summary>
        public uint? MaximumNumberOfTargets
        {
            get
            {
                return Database.Current.Select<uint?>(this.ID, ValueTables.Range, Columns.MaximumNumberOfTargets);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.MaximumNumberOfTargets, value);
                NotifyPropertyChanged("MaximumNumberOfTargets");
            }
        }
        #endregion Property: MaximumNumberOfTargets

        #region Property: ExcludeActor
        /// <summary>
        /// Gets or sets the value that indicates whether the actor is excluded from the range. Only used for RangeType 'RadiusOnly' or 'EntitiesInRadius'.
        /// </summary>
        public bool ExcludeActor
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.ExcludeActor);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.ExcludeActor, value);
                NotifyPropertyChanged("ExcludeActor");
            }
        }
        #endregion Property: ExcludeActor

        #region Property: IncludeActor
        /// <summary>
        /// Gets or sets the value that indicates whether the actor is included in the range.
        /// </summary>
        public bool IncludeActor
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeActor);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeActor, value);
                NotifyPropertyChanged("IncludeActor");
            }
        }
        #endregion Property: IncludeActor

        #region Property: IncludeActorConnections
        /// <summary>
        /// Gets or sets the value that indicates whether the connections of the actor are included in the range.
        /// </summary>
        public bool IncludeActorConnections
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeActorConnections);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeActorConnections, value);
                NotifyPropertyChanged("IncludeActorConnections");
            }
        }
        #endregion Property: IncludeActorConnections

        #region Property: IncludeActorParts
        /// <summary>
        /// Gets or sets the value that indicates whether the parts of the actor are included in the range.
        /// </summary>
        public bool IncludeActorParts
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeActorParts);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeActorParts, value);
                NotifyPropertyChanged("IncludeActorParts");
            }
        }
        #endregion Property: IncludeActorParts

        #region Property: IncludeActorWhole
        /// <summary>
        /// Gets or sets the value that indicates whether the whole of the actor is included in the range.
        /// </summary>
        public bool IncludeActorWhole
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeActorWhole);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeActorWhole, value);
                NotifyPropertyChanged("IncludeActorWhole");
            }
        }
        #endregion Property: IncludeActorWhole

        #region Property: IncludeActorSpaceItems
        /// <summary>
        /// Gets or sets the value that indicates whether the items of all spaces of the actor are included in the range.
        /// </summary>
        public bool IncludeActorSpaceItems
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeActorSpaceItems);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeActorSpaceItems, value);
                NotifyPropertyChanged("IncludeActorSpaceItems");
            }
        }
        #endregion Property: IncludeActorSpaceItems

        #region Property: IncludeActorSpaceTangibleMatter
        /// <summary>
        /// Gets or sets the value that indicates whether the tangible matter of all spaces of the actor is included in the range.
        /// </summary>
        public bool IncludeActorSpaceTangibleMatter
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeActorSpaceTangibleMatter);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeActorSpaceTangibleMatter, value);
                NotifyPropertyChanged("IncludeActorSpaceTangibleMatter");
            }
        }
        #endregion Property: IncludeActorSpaceTangibleMatter

        #region Property: IncludeActorMatter
        /// <summary>
        /// Gets or sets the value that indicates whether the matter of the actor is included in the range.
        /// </summary>
        public bool IncludeActorMatter
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeActorMatter);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeActorMatter, value);
                NotifyPropertyChanged("IncludeActorMatter");
            }
        }
        #endregion Property: IncludeActorMatter

        #region Property: IncludeActorCovers
        /// <summary>
        /// Gets or sets the value that indicates whether the covers of the actor are included in the range.
        /// </summary>
        public bool IncludeActorCovers
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeActorCovers);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeActorCovers, value);
                NotifyPropertyChanged("IncludeActorCovers");
            }
        }
        #endregion Property: IncludeActorCovers

        #region Property: IncludeActorCoveredObject
        /// <summary>
        /// Gets or sets the value that indicates whether the covered object of the actor is included in the range.
        /// </summary>
        public bool IncludeActorCoveredObject
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeActorCoveredObject);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeActorCoveredObject, value);
                NotifyPropertyChanged("IncludeActorCoveredObject");
            }
        }
        #endregion Property: IncludeActorCoveredObject

        #region Property: IncludeSourceOfRelationshipWhereActorIsTarget
        /// <summary>
        /// Gets or sets the relationship type of which the source should be included in the range, in case the actor is the target of the relationship.
        /// </summary>
        public RelationshipType IncludeSourceOfRelationshipWhereActorIsTarget
        {
            get
            {
                return Database.Current.Select<RelationshipType>(this.ID, ValueTables.Range, Columns.IncludeSourceOfRelationshipWhereActorIsTarget);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeSourceOfRelationshipWhereActorIsTarget, value);
                NotifyPropertyChanged("IncludeSourceOfRelationshipWhereActorIsTarget");
            }
        }
        #endregion Property: IncludeSourceOfRelationshipWhereActorIsTarget

        #region Property: IncludeTargetOfRelationshipWhereActorIsSource
        /// <summary>
        /// Gets or sets the relationship type of which the target should be included in the range, in case the actor is the source of the relationship.
        /// </summary>
        public RelationshipType IncludeTargetOfRelationshipWhereActorIsSource
        {
            get
            {
                return Database.Current.Select<RelationshipType>(this.ID, ValueTables.Range, Columns.IncludeTargetOfRelationshipWhereActorIsSource);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeTargetOfRelationshipWhereActorIsSource, value);
                NotifyPropertyChanged("IncludeTargetOfRelationshipWhereActorIsSource");
            }
        }
        #endregion Property: IncludeTargetOfRelationshipWhereActorIsSource

        #region Property: IncludeTarget
        /// <summary>
        /// Gets or sets the value that indicates whether the target is included in the range.
        /// </summary>
        public bool IncludeTarget
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeTarget);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeTarget, value);
                NotifyPropertyChanged("IncludeTarget");
            }
        }
        #endregion Property: IncludeTarget

        #region Property: IncludeTargetConnections
        /// <summary>
        /// Gets or sets the value that indicates whether the connections of the target are included in the range.
        /// </summary>
        public bool IncludeTargetConnections
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeTargetConnections);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeTargetConnections, value);
                NotifyPropertyChanged("IncludeTargetConnections");
            }
        }
        #endregion Property: IncludeTargetConnections

        #region Property: IncludeTargetParts
        /// <summary>
        /// Gets or sets the value that indicates whether the parts of the target are included in the range.
        /// </summary>
        public bool IncludeTargetParts
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeTargetParts);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeTargetParts, value);
                NotifyPropertyChanged("IncludeTargetParts");
            }
        }
        #endregion Property: IncludeTargetParts

        #region Property: IncludeTargetWhole
        /// <summary>
        /// Gets or sets the value that indicates whether the whole of the target is included in the range.
        /// </summary>
        public bool IncludeTargetWhole
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeTargetWhole);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeTargetWhole, value);
                NotifyPropertyChanged("IncludeTargetWhole");
            }
        }
        #endregion Property: IncludeTargetWhole

        #region Property: IncludeTargetSpaceItems
        /// <summary>
        /// Gets or sets the value that indicates whether the items of all spaces of the target are included in the range.
        /// </summary>
        public bool IncludeTargetSpaceItems
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeTargetSpaceItems);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeTargetSpaceItems, value);
                NotifyPropertyChanged("IncludeTargetSpaceItems");
            }
        }
        #endregion Property: IncludeTargetSpaceItems

        #region Property: IncludeTargetSpaceTangibleMatter
        /// <summary>
        /// Gets or sets the value that indicates whether the tangible matter of all spaces of the target is included in the range.
        /// </summary>
        public bool IncludeTargetSpaceTangibleMatter
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeTargetSpaceTangibleMatter);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeTargetSpaceTangibleMatter, value);
                NotifyPropertyChanged("IncludeTargetSpaceTangibleMatter");
            }
        }
        #endregion Property: IncludeTargetSpaceTangibleMatter

        #region Property: IncludeTargetMatter
        /// <summary>
        /// Gets or sets the value that indicates whether the matter of the target is included in the range.
        /// </summary>
        public bool IncludeTargetMatter
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeTargetMatter);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeTargetMatter, value);
                NotifyPropertyChanged("IncludeTargetMatter");
            }
        }
        #endregion Property: IncludeTargetMatter

        #region Property: IncludeTargetCovers
        /// <summary>
        /// Gets or sets the value that indicates whether the covers of the target are included in the range.
        /// </summary>
        public bool IncludeTargetCovers
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeTargetCovers);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeTargetCovers, value);
                NotifyPropertyChanged("IncludeTargetCovers");
            }
        }
        #endregion Property: IncludeTargetCovers

        #region Property: IncludeTargetCoveredObject
        /// <summary>
        /// Gets or sets the value that indicates whether the covered object of the target is included in the range.
        /// </summary>
        public bool IncludeTargetCoveredObject
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeTargetCoveredObject);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeTargetCoveredObject, value);
                NotifyPropertyChanged("IncludeTargetCoveredObject");
            }
        }
        #endregion Property: IncludeTargetCoveredObject

        #region Property: IncludeSourceOfRelationshipWhereTargetIsTarget
        /// <summary>
        /// Gets or sets the relationship type of which the source should be included in the range, in case the target is the target of the relationship.
        /// </summary>
        public RelationshipType IncludeSourceOfRelationshipWhereTargetIsTarget
        {
            get
            {
                return Database.Current.Select<RelationshipType>(this.ID, ValueTables.Range, Columns.IncludeSourceOfRelationshipWhereTargetIsTarget);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeSourceOfRelationshipWhereTargetIsTarget, value);
                NotifyPropertyChanged("IncludeSourceOfRelationshipWhereTargetIsTarget");
            }
        }
        #endregion Property: IncludeSourceOfRelationshipWhereTargetIsTarget

        #region Property: IncludeTargetOfRelationshipWhereTargetIsSource
        /// <summary>
        /// Gets or sets the relationship type of which the target should be included in the range, in case the target is the source of the relationship.
        /// </summary>
        public RelationshipType IncludeTargetOfRelationshipWhereTargetIsSource
        {
            get
            {
                return Database.Current.Select<RelationshipType>(this.ID, ValueTables.Range, Columns.IncludeTargetOfRelationshipWhereTargetIsSource);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeTargetOfRelationshipWhereTargetIsSource, value);
                NotifyPropertyChanged("IncludeTargetOfRelationshipWhereTargetIsSource");
            }
        }
        #endregion Property: IncludeTargetOfRelationshipWhereTargetIsSource

        #region Property: IncludeArtifact
        /// <summary>
        /// Gets or sets the value that indicates whether the artifact is included in the range.
        /// </summary>
        public bool IncludeArtifact
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeArtifact);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeArtifact, value);
                NotifyPropertyChanged("IncludeArtifact");
            }
        }
        #endregion Property: IncludeArtifact

        #region Property: IncludeArtifactConnections
        /// <summary>
        /// Gets or sets the value that indicates whether the connections of the artifact are included in the range.
        /// </summary>
        public bool IncludeArtifactConnections
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeArtifactConnections);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeArtifactConnections, value);
                NotifyPropertyChanged("IncludeArtifactConnections");
            }
        }
        #endregion Property: IncludeArtifactConnections

        #region Property: IncludeArtifactParts
        /// <summary>
        /// Gets or sets the value that indicates whether the parts of the artifact are included in the range.
        /// </summary>
        public bool IncludeArtifactParts
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeArtifactParts);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeArtifactParts, value);
                NotifyPropertyChanged("IncludeArtifactParts");
            }
        }
        #endregion Property: IncludeArtifactParts

        #region Property: IncludeArtifactWhole
        /// <summary>
        /// Gets or sets the value that indicates whether the whole of the artifact is included in the range.
        /// </summary>
        public bool IncludeArtifactWhole
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeArtifactWhole);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeArtifactWhole, value);
                NotifyPropertyChanged("IncludeArtifactWhole");
            }
        }
        #endregion Property: IncludeArtifactWhole

        #region Property: IncludeArtifactSpaceItems
        /// <summary>
        /// Gets or sets the value that indicates whether the items of all spaces of the artifact are included in the range.
        /// </summary>
        public bool IncludeArtifactSpaceItems
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeArtifactSpaceItems);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeArtifactSpaceItems, value);
                NotifyPropertyChanged("IncludeArtifactSpaceItems");
            }
        }
        #endregion Property: IncludeArtifactSpaceItems

        #region Property: IncludeArtifactSpaceTangibleMatter
        /// <summary>
        /// Gets or sets the value that indicates whether the tangible matter of all spaces of the artifact is included in the range.
        /// </summary>
        public bool IncludeArtifactSpaceTangibleMatter
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeArtifactSpaceTangibleMatter);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeArtifactSpaceTangibleMatter, value);
                NotifyPropertyChanged("IncludeArtifactSpaceTangibleMatter");
            }
        }
        #endregion Property: IncludeArtifactSpaceTangibleMatter

        #region Property: IncludeArtifactMatter
        /// <summary>
        /// Gets or sets the value that indicates whether the matter of the artifact is included in the range.
        /// </summary>
        public bool IncludeArtifactMatter
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeArtifactMatter);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeArtifactMatter, value);
                NotifyPropertyChanged("IncludeArtifactMatter");
            }
        }
        #endregion Property: IncludeArtifactMatter

        #region Property: IncludeArtifactCovers
        /// <summary>
        /// Gets or sets the value that indicates whether the covers of the artifact are included in the range.
        /// </summary>
        public bool IncludeArtifactCovers
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeArtifactCovers);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeArtifactCovers, value);
                NotifyPropertyChanged("IncludeArtifactCovers");
            }
        }
        #endregion Property: IncludeArtifactCovers

        #region Property: IncludeArtifactCoveredObject
        /// <summary>
        /// Gets or sets the value that indicates whether the covered object of the artifact is included in the range.
        /// </summary>
        public bool IncludeArtifactCoveredObject
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeArtifactCoveredObject);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeArtifactCoveredObject, value);
                NotifyPropertyChanged("IncludeArtifactCoveredObject");
            }
        }
        #endregion Property: IncludeArtifactCoveredObject

        #region Property: IncludeSourceOfRelationshipWhereArtifactIsTarget
        /// <summary>
        /// Gets or sets the relationship type of which the source should be included in the range, in case the artifact is the target of the relationship.
        /// </summary>
        public RelationshipType IncludeSourceOfRelationshipWhereArtifactIsTarget
        {
            get
            {
                return Database.Current.Select<RelationshipType>(this.ID, ValueTables.Range, Columns.IncludeSourceOfRelationshipWhereArtifactIsTarget);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeSourceOfRelationshipWhereArtifactIsTarget, value);
                NotifyPropertyChanged("IncludeSourceOfRelationshipWhereArtifactIsTarget");
            }
        }
        #endregion Property: IncludeSourceOfRelationshipWhereArtifactIsTarget

        #region Property: IncludeTargetOfRelationshipWhereArtifactIsSource
        /// <summary>
        /// Gets or sets the relationship type of which the target should be included in the range, in case the artifact is the source of the relationship.
        /// </summary>
        public RelationshipType IncludeTargetOfRelationshipWhereArtifactIsSource
        {
            get
            {
                return Database.Current.Select<RelationshipType>(this.ID, ValueTables.Range, Columns.IncludeTargetOfRelationshipWhereArtifactIsSource);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeTargetOfRelationshipWhereArtifactIsSource, value);
                NotifyPropertyChanged("IncludeTargetOfRelationshipWhereArtifactIsSource");
            }
        }
        #endregion Property: IncludeTargetOfRelationshipWhereArtifactIsSource

        #region Property: SpecificTargets
        /// <summary>
        /// Gets the specific targets that are included in the range.
        /// </summary>
        public ReadOnlyCollection<EntityCondition> SpecificTargets
        {
            get
            {
                return Database.Current.SelectAll<EntityCondition>(this.ID, ValueTables.RangeSpecificTarget, Columns.SpecificTarget).AsReadOnly();
            }
        }
        #endregion Property: SpecificTargets

        #region Property: IncludeSpecificTargetsConnections
        /// <summary>
        /// Gets or sets the value that indicates whether the connections of the specific targets are included in the range.
        /// </summary>
        public bool IncludeSpecificTargetsConnections
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeSpecificTargetsConnections);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeSpecificTargetsConnections, value);
                NotifyPropertyChanged("IncludeSpecificTargetsConnections");
            }
        }
        #endregion Property: IncludeSpecificTargetsConnections

        #region Property: IncludeSpecificTargetsParts
        /// <summary>
        /// Gets or sets the value that indicates whether the parts of the specific targets are included in the range.
        /// </summary>
        public bool IncludeSpecificTargetsParts
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeSpecificTargetsParts);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeSpecificTargetsParts, value);
                NotifyPropertyChanged("IncludeSpecificTargetsParts");
            }
        }
        #endregion Property: IncludeSpecificTargetsParts

        #region Property: IncludeSpecificTargetsWhole
        /// <summary>
        /// Gets or sets the value that indicates whether the wholes of the specific targets are included in the range.
        /// </summary>
        public bool IncludeSpecificTargetsWhole
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeSpecificTargetsWhole);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeSpecificTargetsWhole, value);
                NotifyPropertyChanged("IncludeSpecificTargetsWhole");
            }
        }
        #endregion Property: IncludeSpecificTargetsWhole

        #region Property: IncludeSpecificTargetsSpaceItems
        /// <summary>
        /// Gets or sets the value that indicates whether the items in the spaces of the specific targets are included in the range.
        /// </summary>
        public bool IncludeSpecificTargetsSpaceItems
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeSpecificTargetsSpaceItems);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeSpecificTargetsSpaceItems, value);
                NotifyPropertyChanged("IncludeSpecificTargetsSpaceItems");
            }
        }
        #endregion Property: IncludeSpecificTargetsSpaceItems

        #region Property: IncludeSpecificTargetsSpaceTangibleMatter
        /// <summary>
        /// Gets or sets the value that indicates whether the tangible matter of all spaces of the specific targets is included in the range.
        /// </summary>
        public bool IncludeSpecificTargetsSpaceTangibleMatter
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeSpecificTargetsSpaceTangibleMatter);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeSpecificTargetsSpaceTangibleMatter, value);
                NotifyPropertyChanged("IncludeSpecificTargetsSpaceTangibleMatter");
            }
        }
        #endregion Property: IncludeSpecificTargetsSpaceTangibleMatter

        #region Property: IncludeSpecificTargetsMatter
        /// <summary>
        /// Gets or sets the value that indicates whether the matter of the specific targets is included in the range.
        /// </summary>
        public bool IncludeSpecificTargetsMatter
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeSpecificTargetsMatter);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeSpecificTargetsMatter, value);
                NotifyPropertyChanged("IncludeSpecificTargetsMatter");
            }
        }
        #endregion Property: IncludeSpecificTargetsMatter

        #region Property: IncludeSpecificTargetsCovers
        /// <summary>
        /// Gets or sets the value that indicates whether the covers of the specific targets are included in the range.
        /// </summary>
        public bool IncludeSpecificTargetsCovers
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeSpecificTargetsCovers);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeSpecificTargetsCovers, value);
                NotifyPropertyChanged("IncludeSpecificTargetsCovers");
            }
        }
        #endregion Property: IncludeSpecificTargetsCovers

        #region Property: IncludeSpecificTargetsCoveredObject
        /// <summary>
        /// Gets or sets the value that indicates whether the covered object of each specific target is included in the range.
        /// </summary>
        public bool IncludeSpecificTargetsCoveredObject
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeSpecificTargetsCoveredObject);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeSpecificTargetsCoveredObject, value);
                NotifyPropertyChanged("IncludeSpecificTargetsCoveredObject");
            }
        }
        #endregion Property: IncludeSpecificTargetsCoveredObject

        #region Property: IncludeSourceOfRelationshipWhereSpecificTargetIsTarget
        /// <summary>
        /// Gets or sets the relationship type of which the source should be included in the range, in case the specific target is the target of the relationship.
        /// </summary>
        public RelationshipType IncludeSourceOfRelationshipWhereSpecificTargetIsTarget
        {
            get
            {
                return Database.Current.Select<RelationshipType>(this.ID, ValueTables.Range, Columns.IncludeSourceOfRelationshipWhereSpecificTargetIsTarget);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeSourceOfRelationshipWhereSpecificTargetIsTarget, value);
                NotifyPropertyChanged("IncludeSourceOfRelationshipWhereSpecificTargetIsTarget");
            }
        }
        #endregion Property: IncludeSourceOfRelationshipWhereSpecificTargetIsTarget

        #region Property: IncludeTargetOfRelationshipWhereSpecificTargetIsSource
        /// <summary>
        /// Gets or sets the relationship type of which the target should be included in the range, in case the specific target is the source of the relationship.
        /// </summary>
        public RelationshipType IncludeTargetOfRelationshipWhereSpecificTargetIsSource
        {
            get
            {
                return Database.Current.Select<RelationshipType>(this.ID, ValueTables.Range, Columns.IncludeTargetOfRelationshipWhereSpecificTargetIsSource);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeTargetOfRelationshipWhereSpecificTargetIsSource, value);
                NotifyPropertyChanged("IncludeTargetOfRelationshipWhereSpecificTargetIsSource");
            }
        }
        #endregion Property: IncludeTargetOfRelationshipWhereSpecificTargetIsSource

        #region Property: IncludeItemsInSpaceOfActor
        /// <summary>
        /// Gets or sets the value that indicates whether the items in the space of the actor are included in the range.
        /// </summary>
        public bool IncludeItemsInSpaceOfActor
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeItemsInSpaceOfActor);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeItemsInSpaceOfActor, value);
                NotifyPropertyChanged("IncludeItemsInSpaceOfActor");
            }
        }
        #endregion Property: IncludeItemsInSpaceOfArtifacts

        #region Property: IncludeItemsInSpaceOfTarget
        /// <summary>
        /// Gets or sets the value that indicates whether the items in the space of the target are included in the range.
        /// </summary>
        public bool IncludeItemsInSpaceOfTarget
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeItemsInSpaceOfTarget);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeItemsInSpaceOfTarget, value);
                NotifyPropertyChanged("IncludeItemsInSpaceOfTarget");
            }
        }
        #endregion Property: IncludeItemsInSpaceOfTarget

        #region Property: IncludeItemsInSpaceOfArtifact
        /// <summary>
        /// Gets or sets the value that indicates whether the items in the space of the artifact are included in the range.
        /// </summary>
        public bool IncludeItemsInSpaceOfArtifact
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeItemsInSpaceOfArtifact);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeItemsInSpaceOfArtifact, value);
                NotifyPropertyChanged("IncludeItemsInSpaceOfArtifact");
            }
        }
        #endregion Property: IncludeItemsInSpaceOfArtifacts

        #region Property: IncludeItemsInSpaceOfSpecificTargets
        /// <summary>
        /// Gets or sets the value that indicates whether the items in the spaces of the specific targets are included in the range.
        /// </summary>
        public bool IncludeItemsInSpaceOfSpecificTargets
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Range, Columns.IncludeItemsInSpaceOfSpecificTargets);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Range, Columns.IncludeItemsInSpaceOfSpecificTargets, value);
                NotifyPropertyChanged("IncludeItemsInSpaceOfSpecificTargets");
            }
        }
        #endregion Property: IncludeItemsInSpaceOfSpecificTargets

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: Range()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static Range()
        {
            // Specific targets
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.SpecificTarget, new Tuple<Type, EntryType>(typeof(EntityCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.RangeSpecificTarget, typeof(Range), dict);

            // Relationship types
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.IncludeSourceOfRelationshipWhereActorIsTarget, new Tuple<Type, EntryType>(typeof(RelationshipType), EntryType.Unique));
            dict.Add(Columns.IncludeTargetOfRelationshipWhereActorIsSource, new Tuple<Type, EntryType>(typeof(RelationshipType), EntryType.Unique));
            dict.Add(Columns.IncludeSourceOfRelationshipWhereTargetIsTarget, new Tuple<Type, EntryType>(typeof(RelationshipType), EntryType.Unique));
            dict.Add(Columns.IncludeTargetOfRelationshipWhereTargetIsSource, new Tuple<Type, EntryType>(typeof(RelationshipType), EntryType.Unique));
            dict.Add(Columns.IncludeSourceOfRelationshipWhereArtifactIsTarget, new Tuple<Type, EntryType>(typeof(RelationshipType), EntryType.Unique));
            dict.Add(Columns.IncludeTargetOfRelationshipWhereArtifactIsSource, new Tuple<Type, EntryType>(typeof(RelationshipType), EntryType.Unique));
            dict.Add(Columns.IncludeSourceOfRelationshipWhereSpecificTargetIsTarget, new Tuple<Type, EntryType>(typeof(RelationshipType), EntryType.Unique));
            dict.Add(Columns.IncludeTargetOfRelationshipWhereSpecificTargetIsSource, new Tuple<Type, EntryType>(typeof(RelationshipType), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.Range, typeof(Range), dict);
        }
        #endregion Static Constructor: Range()

        #region Constructor: Range()
        /// <summary>
        /// Creates a new range.
        /// </summary>
        public Range()
            : base()
        {
            Database.Current.StartChange();

            // Create the radius and set the unit category to the special distance unit category
            this.Radius = new NumericalValueRange(SemanticsSettings.Values.Range, EqualitySignExtendedDual.LowerOrEqual, SemanticsManager.GetSpecialUnitCategory(SpecialUnitCategories.Distance));
            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);

            Database.Current.StopChange();
        }
        #endregion Constructor: Range()

        #region Constructor: Range(uint id)
        /// <summary>
        /// Creates a new range from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a range from.</param>
        internal Range(uint id)
            : base(id)
        {
            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: Range(uint id)

        #region Constructor: Range(Range range)
        /// <summary>
        /// Clones a range.
        /// </summary>
        /// <param name="range">The range to clone.</param>
        public Range(Range range)
            : base()
        {
            if (range != null)
            {
                Database.Current.StartChange();

                this.RangeType = range.RangeType;
                if (range.Radius != null)
                    this.Radius = new NumericalValueRange(range.Radius);
                this.RadiusOrigin = range.RadiusOrigin;
                this.MaximumNumberOfTargets = range.MaximumNumberOfTargets;
                this.ExcludeActor = range.ExcludeActor;
                this.IncludeActor = range.IncludeActor;
                this.IncludeActorConnections = range.IncludeActorConnections;
                this.IncludeActorParts = range.IncludeActorParts;
                this.IncludeActorWhole = range.IncludeActorWhole;
                this.IncludeActorSpaceItems = range.IncludeActorSpaceItems;
                this.IncludeActorSpaceTangibleMatter = range.IncludeActorSpaceTangibleMatter;
                this.IncludeActorMatter = range.IncludeActorMatter;
                this.IncludeActorCovers = range.IncludeActorCovers;
                this.IncludeActorCoveredObject = range.IncludeActorCoveredObject;
                this.IncludeSourceOfRelationshipWhereActorIsTarget = range.IncludeSourceOfRelationshipWhereActorIsTarget;
                this.IncludeTargetOfRelationshipWhereActorIsSource = range.IncludeTargetOfRelationshipWhereActorIsSource;
                this.IncludeTarget = range.IncludeTarget;
                this.IncludeTargetConnections = range.IncludeTargetConnections;
                this.IncludeTargetParts = range.IncludeTargetParts;
                this.IncludeTargetWhole = range.IncludeTargetWhole;
                this.IncludeTargetSpaceItems = range.IncludeTargetSpaceItems;
                this.IncludeTargetSpaceTangibleMatter = range.IncludeTargetSpaceTangibleMatter;
                this.IncludeTargetMatter = range.IncludeTargetMatter;
                this.IncludeTargetCovers = range.IncludeTargetCovers;
                this.IncludeTargetCoveredObject = range.IncludeTargetCoveredObject;
                this.IncludeSourceOfRelationshipWhereTargetIsTarget = range.IncludeSourceOfRelationshipWhereTargetIsTarget;
                this.IncludeTargetOfRelationshipWhereTargetIsSource = range.IncludeTargetOfRelationshipWhereTargetIsSource;
                this.IncludeArtifact = range.IncludeArtifact;
                this.IncludeArtifactConnections = range.IncludeArtifactConnections;
                this.IncludeArtifactParts = range.IncludeArtifactParts;
                this.IncludeArtifactWhole = range.IncludeArtifactWhole;
                this.IncludeArtifactSpaceItems = range.IncludeArtifactSpaceItems;
                this.IncludeArtifactSpaceTangibleMatter = range.IncludeArtifactSpaceTangibleMatter;
                this.IncludeArtifactMatter = range.IncludeArtifactMatter;
                this.IncludeArtifactCovers = range.IncludeArtifactCovers;
                this.IncludeArtifactCoveredObject = range.IncludeArtifactCoveredObject;
                this.IncludeSourceOfRelationshipWhereArtifactIsTarget = range.IncludeSourceOfRelationshipWhereArtifactIsTarget;
                this.IncludeTargetOfRelationshipWhereArtifactIsSource = range.IncludeTargetOfRelationshipWhereArtifactIsSource;
                foreach (EntityCondition specificTarget in range.SpecificTargets)
                    AddSpecificTarget(specificTarget.Clone());
                this.IncludeSpecificTargetsConnections = range.IncludeSpecificTargetsConnections;
                this.IncludeSpecificTargetsParts = range.IncludeSpecificTargetsParts;
                this.IncludeSpecificTargetsWhole = range.IncludeSpecificTargetsWhole;
                this.IncludeSpecificTargetsSpaceItems = range.IncludeSpecificTargetsSpaceItems;
                this.IncludeSpecificTargetsSpaceTangibleMatter = range.IncludeSpecificTargetsSpaceTangibleMatter;
                this.IncludeSpecificTargetsMatter = range.IncludeSpecificTargetsMatter;
                this.IncludeSpecificTargetsCovers = range.IncludeSpecificTargetsCovers;
                this.IncludeSpecificTargetsCoveredObject = range.IncludeSpecificTargetsCoveredObject;
                this.IncludeSourceOfRelationshipWhereSpecificTargetIsTarget = range.IncludeSourceOfRelationshipWhereSpecificTargetIsTarget;
                this.IncludeTargetOfRelationshipWhereSpecificTargetIsSource = range.IncludeTargetOfRelationshipWhereSpecificTargetIsSource;
                this.IncludeItemsInSpaceOfActor = range.IncludeItemsInSpaceOfActor;
                this.IncludeItemsInSpaceOfArtifact = range.IncludeItemsInSpaceOfArtifact;
                this.IncludeItemsInSpaceOfTarget = range.IncludeItemsInSpaceOfTarget;
                this.IncludeItemsInSpaceOfSpecificTargets = range.IncludeItemsInSpaceOfSpecificTargets;

                Database.Current.StopChange();
            }

            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: Range(Range range)

        #region Method: SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)
        /// <summary>
        /// Change the special distance unit category.
        /// </summary>
        /// <param name="specialUnitCategory">The special unit category.</param>
        /// <param name="unitCategory">The unit category.</param>
        private void SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)
        {
            if (specialUnitCategory == SpecialUnitCategories.Distance && this.Radius != null)
                this.Radius.UnitCategory = unitCategory;
        }
        #endregion Method: SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)
        
        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddSpecificTarget(EntityCondition entityCondition)
        /// <summary>
        /// Add a specific target.
        /// </summary>
        /// <param name="entityCondition">The specific target.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddSpecificTarget(EntityCondition entityCondition)
        {
            if (entityCondition != null)
            {
                // If the specific target is already there, there's no use to add it again
                if (HasSpecificTarget(entityCondition.Entity))
                    return Message.RelationExistsAlready;

                // Add the specific target
                Database.Current.Insert(this.ID, ValueTables.RangeSpecificTarget, Columns.SpecificTarget, entityCondition);
                NotifyPropertyChanged("SpecificTargets");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddSpecificTarget(EntityCondition entityCondition)

        #region Method: RemoveSpecificTarget(EntityCondition entityCondition)
        /// <summary>
        /// Removes a specific target.
        /// </summary>
        /// <param name="entityCondition">The specific target.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveSpecificTarget(EntityCondition entityCondition)
        {
            if (entityCondition != null)
            {
                if (HasSpecificTarget(entityCondition.Entity))
                {
                    // Remove the specific target
                    Database.Current.Remove(this.ID, ValueTables.RangeSpecificTarget, Columns.SpecificTarget, entityCondition);
                    NotifyPropertyChanged("SpecificTargets");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveSpecificTarget(EntityCondition entityCondition)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasSpecificTarget(Entity entity)
        /// <summary>
        /// Checks if this range has the given entity as a specific target.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>Returns true when this range has the entity as a specific target.</returns>
        public bool HasSpecificTarget(Entity entity)
        {
            if (entity != null)
            {
                foreach (EntityCondition specificTarget in this.SpecificTargets)
                {
                    if (entity.Equals(specificTarget.Entity))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasSpecificTarget(Entity entity)

        #region Method: Remove()
        /// <summary>
        /// Remove the range.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the radius
            if (this.Radius != null)
                this.Radius.Remove();

            // Remove the specific targets
            Database.Current.Remove(this.ID, ValueTables.RangeSpecificTarget);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: Range

}