/**************************************************************************
 * 
 * EventRequirements.cs
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
using Common;
using Semantics.Abstractions;
using Semantics.Data;
using Semantics.Utilities;

namespace Semantics.Components
{

    #region Class: EventRequirements
    /// <summary>
    /// A class that wraps all the requirements of an event.
    /// </summary>
    public class EventRequirements : IdHolder
    {

        #region Properties and Fields

        #region Property: ActorRequirements
        /// <summary>
        /// Gets the requirements for the actor.
        /// </summary>
        public Requirements ActorRequirements
        {
            get
            {
                Requirements requirements = Database.Current.Select<Requirements>(this.ID, GenericTables.EventRequirements, Columns.ActorRequirements);
                if (requirements == null)
                {
                    requirements = new Requirements();
                    this.ActorRequirements = requirements;
                }
                return requirements;
            }
            private set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.ActorRequirements, value);
                NotifyPropertyChanged("ActorRequirements");
            }
        }
        #endregion Property: ActorRequirements

        #region Property: TargetRequirements
        /// <summary>
        /// Gets the requirements for the target.
        /// </summary>
        public Requirements TargetRequirements
        {
            get
            {
                Requirements requirements = Database.Current.Select<Requirements>(this.ID, GenericTables.EventRequirements, Columns.TargetRequirements);
                if (requirements == null)
                {
                    requirements = new Requirements();
                    this.TargetRequirements = requirements;
                }
                return requirements;
            }
            private set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.TargetRequirements, value);
                NotifyPropertyChanged("TargetRequirements");
            }
        }
        #endregion Property: TargetRequirements

        #region Property: ArtifactRequirements
        /// <summary>
        /// Gets the requirements for the artifact.
        /// </summary>
        public Requirements ArtifactRequirements
        {
            get
            {
                Requirements requirements = Database.Current.Select<Requirements>(this.ID, GenericTables.EventRequirements, Columns.ArtifactRequirements);
                if (requirements == null)
                {
                    requirements = new Requirements();
                    this.ArtifactRequirements = requirements;
                }
                return requirements;
            }
            private set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.ArtifactRequirements, value);
                NotifyPropertyChanged("ArtifactRequirements");
            }
        }
        #endregion Property: ArtifactRequirements

        #region Property: ReferenceRequirements
        /// <summary>
        /// Gets the requirements per reference.
        /// </summary>
        public Dictionary<Reference, Requirements> ReferenceRequirements
        {
            get
            {
                return Database.Current.SelectAll<Reference, Requirements>(this.ID, GenericTables.EventRequirementsReferenceRequirements, Columns.Reference, Columns.Requirements);
            }
        }
        #endregion Property: ReferenceRequirements

        #region Property: SpatialRequirementBetweenActorAndTarget
        /// <summary>
        /// Gets or sets the spatial condition between the actor and target.
        /// </summary>
        public SpatialRequirement SpatialRequirementBetweenActorAndTarget
        {
            get
            {
                return Database.Current.Select<SpatialRequirement>(this.ID, GenericTables.EventRequirements, Columns.SpatialRequirementBetweenActorAndTarget);
            }
            set
            {
                SpatialRequirement spatialRequirement = this.SpatialRequirementBetweenActorAndTarget;
                if (spatialRequirement != value)
                {
                    if (spatialRequirement != null)
                        spatialRequirement.Remove();

                    Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.SpatialRequirementBetweenActorAndTarget, value);
                    NotifyPropertyChanged("SpatialRequirementBetweenActorAndTarget");
                }
            }
        }
        #endregion Property: SpatialRequirementBetweenActorAndTarget

        #region Property: SpatialRequirementBetweenActorAndArtifact
        /// <summary>
        /// Gets or sets the spatial condition between the actor and artifact.
        /// </summary>
        public SpatialRequirement SpatialRequirementBetweenActorAndArtifact
        {
            get
            {
                return Database.Current.Select<SpatialRequirement>(this.ID, GenericTables.EventRequirements, Columns.SpatialRequirementBetweenActorAndArtifact);
            }
            set
            {
                SpatialRequirement spatialRequirement = this.SpatialRequirementBetweenActorAndArtifact;
                if (spatialRequirement != value)
                {
                    if (spatialRequirement != null)
                        spatialRequirement.Remove();

                    Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.SpatialRequirementBetweenActorAndArtifact, value);
                    NotifyPropertyChanged("SpatialRequirementBetweenActorAndArtifact");
                }
            }
        }
        #endregion Property: SpatialRequirementBetweenActorAndArtifact

        #region Property: SpatialRequirementBetweenTargetAndArtifact
        /// <summary>
        /// Gets or sets the spatial condition between the target and artifact.
        /// </summary>
        public SpatialRequirement SpatialRequirementBetweenTargetAndArtifact
        {
            get
            {
                return Database.Current.Select<SpatialRequirement>(this.ID, GenericTables.EventRequirements, Columns.SpatialRequirementBetweenTargetAndArtifact);
            }
            set
            {
                SpatialRequirement spatialRequirement = this.SpatialRequirementBetweenTargetAndArtifact;
                if (spatialRequirement != value)
                {
                    if (spatialRequirement != null)
                        spatialRequirement.Remove();

                    Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.SpatialRequirementBetweenTargetAndArtifact, value);
                    NotifyPropertyChanged("SpatialRequirementBetweenTargetAndArtifact");
                }
            }
        }
        #endregion Property: SpatialRequirementBetweenTargetAndArtifact

        #region Property: AddRemoveRequirements
        /// <summary>
        /// Gets the add/remove requirements for the actor. Only valid for automatic events!
        /// </summary>
        public AddRemoveRequirements AddRemoveRequirements
        {
            get
            {
                return Database.Current.Select<AddRemoveRequirements>(this.ID, GenericTables.EventRequirements, Columns.AddRemoveRequirements);
            }
            private set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.AddRemoveRequirements, value);
                NotifyPropertyChanged("AddRemoveRequirements");
            }
        }
        #endregion Property: AddRemoveRequirements

        #region Property: ActorHasTargetAsPart
        /// <summary>
        /// Gets or sets the value that indicates whether the actor should have the target as a part.
        /// </summary>
        public bool? ActorHasTargetAsPart
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, GenericTables.EventRequirements, Columns.ActorHasTargetAsPart);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.ActorHasTargetAsPart, value);
                NotifyPropertyChanged("ActorHasTargetAsPart");
            }
        }
        #endregion Property: ActorHasTargetAsPart

        #region Property: TargetHasActorAsPart
        /// <summary>
        /// Gets or sets the value that indicates whether the target should have the actor as a part.
        /// </summary>
        public bool? TargetHasActorAsPart
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, GenericTables.EventRequirements, Columns.TargetHasActorAsPart);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.TargetHasActorAsPart, value);
                NotifyPropertyChanged("TargetHasActorAsPart");
            }
        }
        #endregion Property: TargetHasActorAsPart

        #region Property: ActorHasArtifactAsPart
        /// <summary>
        /// Gets or sets the value that indicates whether the actor should have the artifact as a part.
        /// </summary>
        public bool? ActorHasArtifactAsPart
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, GenericTables.EventRequirements, Columns.ActorHasArtifactAsPart);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.ActorHasArtifactAsPart, value);
                NotifyPropertyChanged("ActorHasArtifactAsPart");
            }
        }
        #endregion Property: ActorHasArtifactAsPart

        #region Property: ActorHasTargetAsCover
        /// <summary>
        /// Gets or sets the value that indicates whether the actor should have the target as a cover.
        /// </summary>
        public bool? ActorHasTargetAsCover
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, GenericTables.EventRequirements, Columns.ActorHasTargetAsCover);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.ActorHasTargetAsCover, value);
                NotifyPropertyChanged("ActorHasTargetAsCover");
            }
        }
        #endregion Property: ActorHasTargetAsCover

        #region Property: TargetHasActorAsCover
        /// <summary>
        /// Gets or sets the value that indicates whether the target should have the actor as a cover.
        /// </summary>
        public bool? TargetHasActorAsCover
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, GenericTables.EventRequirements, Columns.TargetHasActorAsCover);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.TargetHasActorAsCover, value);
                NotifyPropertyChanged("TargetHasActorAsCover");
            }
        }
        #endregion Property: TargetHasActorAsCover

        #region Property: ActorHasArtifactAsCover
        /// <summary>
        /// Gets or sets the value that indicates whether the actor should have the artifact as a cover.
        /// </summary>
        public bool? ActorHasArtifactAsCover
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, GenericTables.EventRequirements, Columns.ActorHasArtifactAsCover);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.ActorHasArtifactAsCover, value);
                NotifyPropertyChanged("ActorHasArtifactAsCover");
            }
        }
        #endregion Property: ActorHasArtifactAsCover

        #region Property: ActorHasTargetAsLayer
        /// <summary>
        /// Gets or sets the value that indicates whether the actor should have the target as a layer.
        /// </summary>
        public bool? ActorHasTargetAsLayer
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, GenericTables.EventRequirements, Columns.ActorHasTargetAsLayer);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.ActorHasTargetAsLayer, value);
                NotifyPropertyChanged("ActorHasTargetAsLayer");
            }
        }
        #endregion Property: ActorHasTargetAsLayer

        #region Property: TargetHasActorAsLayer
        /// <summary>
        /// Gets or sets the value that indicates whether the target should have the actor as a layer.
        /// </summary>
        public bool? TargetHasActorAsLayer
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, GenericTables.EventRequirements, Columns.TargetHasActorAsLayer);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.TargetHasActorAsLayer, value);
                NotifyPropertyChanged("TargetHasActorAsLayer");
            }
        }
        #endregion Property: TargetHasActorAsLayer

        #region Property: ActorHasArtifactAsLayer
        /// <summary>
        /// Gets or sets the value that indicates whether the actor should have the artifact as a layer.
        /// </summary>
        public bool? ActorHasArtifactAsLayer
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, GenericTables.EventRequirements, Columns.ActorHasArtifactAsLayer);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.ActorHasArtifactAsLayer, value);
                NotifyPropertyChanged("ActorHasArtifactAsLayer");
            }
        }
        #endregion Property: ActorHasArtifactAsLayer

        #region Property: ActorHasTargetAsMatter
        /// <summary>
        /// Gets or sets the value that indicates whether the actor should have the target as a matter.
        /// </summary>
        public bool? ActorHasTargetAsMatter
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, GenericTables.EventRequirements, Columns.ActorHasTargetAsMatter);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.ActorHasTargetAsMatter, value);
                NotifyPropertyChanged("ActorHasTargetAsMatter");
            }
        }
        #endregion Property: ActorHasTargetAsMatter

        #region Property: TargetHasActorAsMatter
        /// <summary>
        /// Gets or sets the value that indicates whether the target should have the actor as a matter.
        /// </summary>
        public bool? TargetHasActorAsMatter
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, GenericTables.EventRequirements, Columns.TargetHasActorAsMatter);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.TargetHasActorAsMatter, value);
                NotifyPropertyChanged("TargetHasActorAsMatter");
            }
        }
        #endregion Property: TargetHasActorAsMatter

        #region Property: ActorHasArtifactAsMatter
        /// <summary>
        /// Gets or sets the value that indicates whether the actor should have the artifact as a matter.
        /// </summary>
        public bool? ActorHasArtifactAsMatter
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, GenericTables.EventRequirements, Columns.ActorHasArtifactAsMatter);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.ActorHasArtifactAsMatter, value);
                NotifyPropertyChanged("ActorHasArtifactAsMatter");
            }
        }
        #endregion Property: ActorHasArtifactAsMatter

        #region Property: ActorHasTargetAsItem
        /// <summary>
        /// Gets or sets the value that indicates whether the actor should have the target as a space item.
        /// </summary>
        public bool? ActorHasTargetAsItem
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, GenericTables.EventRequirements, Columns.ActorHasTargetAsItem);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.ActorHasTargetAsItem, value);
                NotifyPropertyChanged("ActorHasTargetAsItem");
            }
        }
        #endregion Property: ActorHasTargetAsItem

        #region Property: TargetHasActorAsItem
        /// <summary>
        /// Gets or sets the value that indicates whether the target should have the actor as a space item.
        /// </summary>
        public bool? TargetHasActorAsItem
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, GenericTables.EventRequirements, Columns.TargetHasActorAsItem);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.TargetHasActorAsItem, value);
                NotifyPropertyChanged("TargetHasActorAsItem");
            }
        }
        #endregion Property: TargetHasActorAsItem

        #region Property: ActorHasArtifactAsItem
        /// <summary>
        /// Gets or sets the value that indicates whether the actor should have the artifact as a space item.
        /// </summary>
        public bool? ActorHasArtifactAsItem
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, GenericTables.EventRequirements, Columns.ActorHasArtifactAsItem);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.ActorHasArtifactAsItem, value);
                NotifyPropertyChanged("ActorHasArtifactAsItem");
            }
        }
        #endregion Property: ActorHasArtifactAsItem

        #region Property: ActorAndTargetConnected
        /// <summary>
        /// Gets or sets the value that indicates whether the actor should be connected to the target.
        /// </summary>
        public bool? ActorAndTargetConnected
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, GenericTables.EventRequirements, Columns.ActorAndTargetConnected);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.ActorAndTargetConnected, value);
                NotifyPropertyChanged("ActorAndTargetConnected");
            }
        }
        #endregion Property: ActorAndTargetConnected

        #region Property: ActorAndTargetCollide
        /// <summary>
        /// Gets or sets the value that indicates whether the actor and target should collide.
        /// </summary>
        public bool? ActorAndTargetCollide
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, GenericTables.EventRequirements, Columns.ActorAndTargetCollide);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.ActorAndTargetCollide, value);
                NotifyPropertyChanged("ActorAndTargetCollide");
            }
        }
        #endregion Property: ActorAndTargetCollide

        #region Property: ActorAndTargetRelationship
        /// <summary>
        /// Gets or sets the required relationship between the actor and target.
        /// </summary>
        public RelationshipType ActorAndTargetRelationship
        {
            get
            {
                return Database.Current.Select<RelationshipType>(this.ID, GenericTables.EventRequirements, Columns.ActorAndTargetRelationship);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.ActorAndTargetRelationship, value);
                NotifyPropertyChanged("ActorAndTargetRelationship");
            }
        }
        #endregion Property: ActorAndTargetRelationship

        #region Property: ActorAndArtifactRelationship
        /// <summary>
        /// Gets or sets the required relationship between the actor and artifact.
        /// </summary>
        public RelationshipType ActorAndArtifactRelationship
        {
            get
            {
                return Database.Current.Select<RelationshipType>(this.ID, GenericTables.EventRequirements, Columns.ActorAndArtifactRelationship);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.ActorAndArtifactRelationship, value);
                NotifyPropertyChanged("ActorAndArtifactRelationship");
            }
        }
        #endregion Property: ActorAndArtifactRelationship

        #region Property: TargetAndArtifactRelationship
        /// <summary>
        /// Gets or sets the required relationship between the target and artifact.
        /// </summary>
        public RelationshipType TargetAndArtifactRelationship
        {
            get
            {
                return Database.Current.Select<RelationshipType>(this.ID, GenericTables.EventRequirements, Columns.TargetAndArtifactRelationship);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.TargetAndArtifactRelationship, value);
                NotifyPropertyChanged("TargetAndArtifactRelationship");
            }
        }
        #endregion Property: TargetAndArtifactRelationship

        #region Property: ActorTargetRelationshipSource
        /// <summary>
        /// Gets or sets the source for the actor-target relationship: the actor or target; the other one will become the target of the relationship.
        /// </summary>
        public ActorTargetArtifact ActorTargetRelationshipSource
        {
            get
            {
                return Database.Current.Select<ActorTargetArtifact>(this.ID, GenericTables.EventRequirements, Columns.ActorTargetRelationshipSource);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.ActorTargetRelationshipSource, value);
                NotifyPropertyChanged("ActorTargetRelationshipSource");
            }
        }
        #endregion Property: ActorTargetRelationshipSource

        #region Property: ActorArtifactRelationshipSource
        /// <summary>
        /// Gets or sets the source for the actor-artifact relationship: the actor or artifact; the other one will become the target of the relationship.
        /// </summary>
        public ActorTargetArtifact ActorArtifactRelationshipSource
        {
            get
            {
                return Database.Current.Select<ActorTargetArtifact>(this.ID, GenericTables.EventRequirements, Columns.ActorArtifactRelationshipSource);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.ActorArtifactRelationshipSource, value);
                NotifyPropertyChanged("ActorArtifactRelationshipSource");
            }
        }
        #endregion Property: ActorArtifactRelationshipSource

        #region Property: TargetArtifactRelationshipSource
        /// <summary>
        /// Gets or sets the source for the target-artifact relationship: the target or artifact; the other one will become the target of the relationship.
        /// </summary>
        public ActorTargetArtifact TargetArtifactRelationshipSource
        {
            get
            {
                return Database.Current.Select<ActorTargetArtifact>(this.ID, GenericTables.EventRequirements, Columns.TargetArtifactRelationshipSource);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.TargetArtifactRelationshipSource, value);
                NotifyPropertyChanged("TargetArtifactRelationshipSource");
            }
        }
        #endregion Property: TargetArtifactRelationshipSource

        #region Property: ActorIsUsedAsPart
        /// <summary>
        /// Gets or sets the value that indicates whether the actor should be used as a part.
        /// </summary>
        public bool? ActorIsUsedAsPart
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, GenericTables.EventRequirements, Columns.ActorIsUsedAsPart);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.ActorIsUsedAsPart, value);
                NotifyPropertyChanged("ActorIsUsedAsPart");
            }
        }
        #endregion Property: ActorIsUsedAsPart

        #region Property: ActorIsUsedAsCover
        /// <summary>
        /// Gets or sets the value that indicates whether the actor should be used as a cover.
        /// </summary>
        public bool? ActorIsUsedAsCover
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, GenericTables.EventRequirements, Columns.ActorIsUsedAsCover);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.ActorIsUsedAsCover, value);
                NotifyPropertyChanged("ActorIsUsedAsCover");
            }
        }
        #endregion Property: ActorIsUsedAsCover

        #region Property: ActorIsUsedAsConnectionItem
        /// <summary>
        /// Gets or sets the value that indicates whether the actor should be used as a connection item.
        /// </summary>
        public bool? ActorIsUsedAsConnectionItem
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, GenericTables.EventRequirements, Columns.ActorIsUsedAsConnectionItem);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.ActorIsUsedAsConnectionItem, value);
                NotifyPropertyChanged("ActorIsUsedAsConnectionItem");
            }
        }
        #endregion Property: ActorIsUsedAsConnectionItem

        #region Property: ActorIsUsedAsLayer
        /// <summary>
        /// Gets or sets the value that indicates whether the actor should be used as a layer.
        /// </summary>
        public bool? ActorIsUsedAsLayer
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, GenericTables.EventRequirements, Columns.ActorIsUsedAsLayer);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.ActorIsUsedAsLayer, value);
                NotifyPropertyChanged("ActorIsUsedAsLayer");
            }
        }
        #endregion Property: ActorIsUsedAsLayer

        #region Property: ShouldNotBeSatisfied
        /// <summary>
        /// Gets or sets the value that indicates whether the requirements should NOT be satisfied in order to trigger the event.
        /// </summary>
        public bool ShouldNotBeSatisfied
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, GenericTables.EventRequirements, Columns.ShouldNotBeSatisfied);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.EventRequirements, Columns.ShouldNotBeSatisfied, value);
                NotifyPropertyChanged("ShouldNotBeSatisfied");
            }
        }
        #endregion Property: ShouldNotBeSatisfied

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: EventRequirements()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static EventRequirements()
        {
            // Relationship type
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.ActorAndTargetRelationship, new Tuple<Type, EntryType>(typeof(RelationshipType), EntryType.Nullable));
            Database.Current.AddTableDefinition(GenericTables.EventRequirements, typeof(EventRequirements), dict);

            // Reference
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Reference, new Tuple<Type, EntryType>(typeof(Reference), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.EventRequirementsReferenceRequirements, typeof(EventRequirements), dict);
        }
        #endregion Static Constructor: EventRequirements()

        #region Constructor: EventRequirements()
        /// <summary>
        /// Creates new event requirements.
        /// </summary>
        internal EventRequirements()
            : base()
        {
            Database.Current.StartChange();

            this.ActorRequirements = new Requirements();
            this.TargetRequirements = new Requirements();
            this.ArtifactRequirements = new Requirements();
            this.AddRemoveRequirements = new AddRemoveRequirements();

            Database.Current.StopChange();
        }
        #endregion Constructor: EventRequirements()

        #region Constructor: EventRequirements(uint id)
        /// <summary>
        /// Creates new event requirements with the given ID.
        /// </summary>
        /// <param name="id">The ID to create new event requirements from.</param>
        private EventRequirements(uint id)
            : base(id)
        {
        }
        #endregion Constructor: EventRequirements(uint id)

        #region Constructor: EventRequirements(EventRequirements eventRequirements)
        /// <summary>
        /// Clones the event requirements.
        /// </summary>
        /// <param name="eventRequirements">The event requirements to clone.</param>
        public EventRequirements(EventRequirements eventRequirements)
            : base()
        {
            if (eventRequirements != null)
            {
                Database.Current.StartChange();

                if (eventRequirements.ActorRequirements != null)
                    this.ActorRequirements = new Requirements(eventRequirements.ActorRequirements);
                if (eventRequirements.TargetRequirements != null)
                    this.TargetRequirements = new Requirements(eventRequirements.TargetRequirements);
                if (eventRequirements.ArtifactRequirements != null)
                    this.ArtifactRequirements = new Requirements(eventRequirements.ArtifactRequirements);
                
                Dictionary<Reference, Requirements> referenceRequirements = eventRequirements.ReferenceRequirements;
                foreach (Reference reference in referenceRequirements.Keys)
                    SetRequirements(reference, new Requirements(referenceRequirements[reference]));
                
                if (eventRequirements.SpatialRequirementBetweenActorAndTarget != null)
                    this.SpatialRequirementBetweenActorAndTarget = new SpatialRequirement(eventRequirements.SpatialRequirementBetweenActorAndTarget);
                if (eventRequirements.SpatialRequirementBetweenActorAndArtifact != null)
                    this.SpatialRequirementBetweenActorAndArtifact = new SpatialRequirement(eventRequirements.SpatialRequirementBetweenActorAndArtifact);
                if (eventRequirements.SpatialRequirementBetweenTargetAndArtifact != null)
                    this.SpatialRequirementBetweenTargetAndArtifact = new SpatialRequirement(eventRequirements.SpatialRequirementBetweenTargetAndArtifact);

                if (eventRequirements.AddRemoveRequirements != null)
                    this.AddRemoveRequirements = new AddRemoveRequirements(eventRequirements.AddRemoveRequirements);
                
                this.ActorAndTargetCollide = eventRequirements.ActorAndTargetCollide;
                this.ActorAndTargetConnected = eventRequirements.ActorAndTargetConnected;
                
                this.ActorHasTargetAsCover = eventRequirements.ActorHasTargetAsCover;
                this.ActorHasTargetAsItem = eventRequirements.ActorHasTargetAsItem;
                this.ActorHasTargetAsLayer = eventRequirements.ActorHasTargetAsLayer;
                this.ActorHasTargetAsMatter = eventRequirements.ActorHasTargetAsMatter;
                this.ActorHasTargetAsPart = eventRequirements.ActorHasTargetAsPart;
                this.TargetHasActorAsCover = eventRequirements.TargetHasActorAsCover;
                this.TargetHasActorAsItem = eventRequirements.TargetHasActorAsItem;
                this.TargetHasActorAsLayer = eventRequirements.TargetHasActorAsLayer;
                this.TargetHasActorAsMatter = eventRequirements.TargetHasActorAsMatter;
                this.TargetHasActorAsPart = eventRequirements.TargetHasActorAsPart;
                this.ActorHasArtifactAsCover = eventRequirements.ActorHasArtifactAsCover;
                this.ActorHasArtifactAsItem = eventRequirements.ActorHasArtifactAsItem;
                this.ActorHasArtifactAsLayer = eventRequirements.ActorHasArtifactAsLayer;
                this.ActorHasArtifactAsMatter = eventRequirements.ActorHasArtifactAsMatter;
                this.ActorHasArtifactAsPart = eventRequirements.ActorHasArtifactAsPart;
                
                this.ActorAndTargetRelationship = eventRequirements.ActorAndTargetRelationship;
                this.ActorAndArtifactRelationship = eventRequirements.ActorAndArtifactRelationship;
                this.TargetAndArtifactRelationship = eventRequirements.TargetAndArtifactRelationship;
                this.ActorTargetRelationshipSource = eventRequirements.ActorTargetRelationshipSource;
                this.ActorArtifactRelationshipSource = eventRequirements.ActorArtifactRelationshipSource;
                this.TargetArtifactRelationshipSource = eventRequirements.TargetArtifactRelationshipSource;
                
                this.ActorIsUsedAsConnectionItem = eventRequirements.ActorIsUsedAsConnectionItem;
                this.ActorIsUsedAsCover = eventRequirements.ActorIsUsedAsCover;
                this.ActorIsUsedAsLayer = eventRequirements.ActorIsUsedAsLayer;
                this.ActorIsUsedAsPart = eventRequirements.ActorIsUsedAsPart;
                
                this.ShouldNotBeSatisfied = eventRequirements.ShouldNotBeSatisfied;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: EventRequirements(EventRequirements eventRequirements)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: SetRequirements(Reference reference, Requirements requirements)
        /// <summary>
        /// Assign the given requirements to the given reference.
        /// </summary>
        /// <param name="reference">The reference to assign requirements to.</param>
        /// <param name="requirements">The requirements to assign to the reference, can be null.</param>
        /// <returns>Returns whether the relation has been succesfully established.</returns>
        public Message SetRequirements(Reference reference, Requirements requirements)
        {
            if (reference != null)
            {
                // Link the reference and the requirements by removing or updating the existing value, or by inserting it in the database
                if (this.ReferenceRequirements.ContainsKey(reference))
                {
                    if (requirements == null)
                        Database.Current.Remove(this.ID, GenericTables.EventRequirementsReferenceRequirements, Columns.Reference, reference, false);
                    else
                        Database.Current.Update(this.ID, GenericTables.EventRequirementsReferenceRequirements, Columns.Requirements, requirements);
                }
                else
                    Database.Current.Insert(this.ID, GenericTables.EventRequirementsReferenceRequirements, new string[] { Columns.Reference, Columns.Requirements }, new object[] { reference, requirements });

                NotifyPropertyChanged("EventRequirementsReferenceRequirements");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: SetRequirements(Reference reference, Requirements requirements)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the event requirements.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the requirements
            if (this.ActorRequirements != null)
                this.ActorRequirements.Remove();
            if (this.TargetRequirements != null)
                this.TargetRequirements.Remove();
            if (this.ArtifactRequirements != null)
                this.ArtifactRequirements.Remove();

            Dictionary<Reference, Requirements> referenceRequirements = this.ReferenceRequirements;
            foreach (Requirements requirements in referenceRequirements.Values)
                requirements.Remove();
            Database.Current.Remove(this.ID, GenericTables.EventRequirementsReferenceRequirements);

            // Remove the spatial requirements
            if (this.SpatialRequirementBetweenActorAndTarget != null)
                this.SpatialRequirementBetweenActorAndTarget.Remove();

            // Remove the add/remove requirements
            if (this.AddRemoveRequirements != null)
                this.AddRemoveRequirements.Remove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: EventRequirements
		
}