/**************************************************************************
 * 
 * EventRequirementsBase.cs
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
using Semantics.Components;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Entities;
using SemanticsEngine.Interfaces;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: EventRequirementsBase
    /// <summary>
    /// A base for the event requirements.
    /// </summary>
    public class EventRequirementsBase : Base
    {

        #region Properties and Fields

        #region Property: EventRequirements
        /// <summary>
        /// Gets the event requirements of which this is an event requirements base.
        /// </summary>
        internal EventRequirements EventRequirements
        {
            get
            {
                return this.IdHolder as EventRequirements;
            }
        }
        #endregion Property: EventRequirements

        #region Property: ActorRequirements
        /// <summary>
        /// The requirements of the actor.
        /// </summary>
        private RequirementsBase actorRequirements = null;

        /// <summary>
        /// Gets the requirements of the actor.
        /// </summary>
        public RequirementsBase ActorRequirements
        {
            get
            {
                if (actorRequirements == null)
                    LoadActorRequirements();
                return actorRequirements;
            }
        }

        /// <summary>
        /// Loads the requirements of the actor.
        /// </summary>
        private void LoadActorRequirements()
        {
            if (this.EventRequirements != null)
                actorRequirements = BaseManager.Current.GetBase<RequirementsBase>(this.EventRequirements.ActorRequirements);
        }
        #endregion Property: ActorRequirements

        #region Property: TargetRequirements
        /// <summary>
        /// The requirements of the target.
        /// </summary>
        private RequirementsBase targetRequirements = null;

        /// <summary>
        /// Gets the requirements of the target.
        /// </summary>
        public RequirementsBase TargetRequirements
        {
            get
            {
                if (targetRequirements == null)
                    LoadTargetRequirements();
                return targetRequirements;
            }
        }

        /// <summary>
        /// Loads the requirements of the target.
        /// </summary>
        private void LoadTargetRequirements()
        {
            if (this.EventRequirements != null)
                targetRequirements = BaseManager.Current.GetBase<RequirementsBase>(this.EventRequirements.TargetRequirements);
        }
        #endregion Property: TargetRequirements

        #region Property: ArtifactRequirements
        /// <summary>
        /// The requirements of the artifact.
        /// </summary>
        private RequirementsBase artifactRequirements = null;

        /// <summary>
        /// Gets the requirements of the artifact.
        /// </summary>
        public RequirementsBase ArtifactRequirements
        {
            get
            {
                if (artifactRequirements == null)
                    LoadArtifactRequirements();
                return artifactRequirements;
            }
        }

        /// <summary>
        /// Loads the requirements of the artifact.
        /// </summary>
        private void LoadArtifactRequirements()
        {
            if (this.EventRequirements != null)
                artifactRequirements = BaseManager.Current.GetBase<RequirementsBase>(this.EventRequirements.ArtifactRequirements);
        }
        #endregion Property: ArtifactRequirements

        #region Property: ReferenceRequirements
        /// <summary>
        /// The dictionary that defines the requirements for each reference.
        /// </summary>
        private Dictionary<ReferenceBase, RequirementsBase> referenceRequirements = null;

        /// <summary>
        /// Gets the dictionary that defines the requirements for each reference.
        /// </summary>
        public Dictionary<ReferenceBase, RequirementsBase> ReferenceRequirements
        {
            get
            {
                if (referenceRequirements == null)
                {
                    LoadReferenceRequirements();
                    if (referenceRequirements == null)
                        return new Dictionary<ReferenceBase, RequirementsBase>();
                }
                return referenceRequirements;
            }
        }

        /// <summary>
        /// Loads the dictionary that defines the requirements for each reference.
        /// </summary>
        private void LoadReferenceRequirements()
        {
            if (this.EventRequirements != null)
            {
                referenceRequirements = new Dictionary<ReferenceBase, RequirementsBase>();
                foreach (KeyValuePair<Reference, Requirements> referenceRequirement in this.EventRequirements.ReferenceRequirements)
                {
                    ReferenceBase referenceBase = BaseManager.Current.GetBase<ReferenceBase>(referenceRequirement.Key);
                    RequirementsBase requirementsBase = BaseManager.Current.GetBase<RequirementsBase>(referenceRequirement.Value);
                    referenceRequirements.Add(referenceBase, requirementsBase);
                }
            }
        }
        #endregion Property: ReferenceRequirements

        #region Property: SpatialRequirementBetweenActorAndTarget
        /// <summary>
        /// The spatial requirement between the actor and target.
        /// </summary>
        private SpatialRequirementBase spatialRequirementBetweenActorAndTarget = null;

        /// <summary>
        /// Gets the spatial requirement between the actor and target.
        /// </summary>
        public SpatialRequirementBase SpatialRequirementBetweenActorAndTarget
        {
            get
            {
                return spatialRequirementBetweenActorAndTarget;
            }
        }
        #endregion Property: SpatialRequirementBetweenActorAndTarget

        #region Property: SpatialRequirementBetweenActorAndArtifact
        /// <summary>
        /// The spatial requirement between the actor and artifact.
        /// </summary>
        private SpatialRequirementBase spatialRequirementBetweenActorAndArtifact = null;

        /// <summary>
        /// Gets the spatial requirement between the actor and artifact.
        /// </summary>
        public SpatialRequirementBase SpatialRequirementBetweenActorAndArtifact
        {
            get
            {
                return spatialRequirementBetweenActorAndArtifact;
            }
        }
        #endregion Property: SpatialRequirementBetweenActorAndArtifact

        #region Property: SpatialRequirementBetweenTargetAndArtifact
        /// <summary>
        /// The spatial requirement between the target and artifact.
        /// </summary>
        private SpatialRequirementBase spatialRequirementBetweenTargetAndArtifact = null;

        /// <summary>
        /// Gets the spatial requirement between the target and artifact.
        /// </summary>
        public SpatialRequirementBase SpatialRequirementBetweenTargetAndArtifact
        {
            get
            {
                return spatialRequirementBetweenTargetAndArtifact;
            }
        }
        #endregion Property: SpatialRequirementBetweenTargetAndArtifact

        #region Property: AddRemoveRequirements
        /// <summary>
        /// The add/remove requirements for the actor. Only valid for automatic events!
        /// </summary>
        private AddRemoveRequirementsBase addRemoveRequirements = null;
        
        /// <summary>
        /// Gets the add/remove requirements for the actor. Only valid for automatic events!
        /// </summary>
        public AddRemoveRequirementsBase AddRemoveRequirements
        {
            get
            {
                if (addRemoveRequirements == null)
                    LoadAddRemoveRequirements();
                return addRemoveRequirements;
            }
        }

        /// <summary>
        /// Loads the add/remove requirements for the actor.
        /// </summary>
        private void LoadAddRemoveRequirements()
        {
            if (this.EventRequirements != null)
                addRemoveRequirements = BaseManager.Current.GetBase<AddRemoveRequirementsBase>(this.EventRequirements.AddRemoveRequirements);
        }
        #endregion Property: AddRemoveRequirements

        #region Property: ActorHasTargetAsPart
        /// <summary>
        /// The value that indicates whether the actor should have the target as a part.
        /// </summary>
        private bool? actorHasTargetAsPart = null;
        
        /// <summary>
        /// Gets the value that indicates whether the actor should have the target as a part.
        /// </summary>
        public bool? ActorHasTargetAsPart
        {
            get
            {
                return actorHasTargetAsPart;
            }
        }
        #endregion Property: ActorHasTargetAsPart

        #region Property: TargetHasActorAsPart
        /// <summary>
        /// The value that indicates whether the target should have the actor as a part.
        /// </summary>
        private bool? targetHasActorAsPart = null;

        /// <summary>
        /// Gets the value that indicates whether the target should have the actor as a part.
        /// </summary>
        public bool? TargetHasActorAsPart
        {
            get
            {
                return targetHasActorAsPart;
            }
        }
        #endregion Property: TargetHasActorAsPart

        #region Property: ActorHasArtifactAsPart
        /// <summary>
        /// The value that indicates whether the actor should have the artifact as a part.
        /// </summary>
        private bool? actorHasArtifactAsPart = null;

        /// <summary>
        /// Gets the value that indicates whether the actor should have the artifact as a part.
        /// </summary>
        public bool? ActorHasArtifactAsPart
        {
            get
            {
                return actorHasArtifactAsPart;
            }
        }
        #endregion Property: ActorHasArtifactAsPart

        #region Property: ActorHasTargetAsCover
        /// <summary>
        /// The value that indicates whether the actor should have the target as a cover.
        /// </summary>
        private bool? actorHasTargetAsCover = null;
        
        /// <summary>
        /// Gets the value that indicates whether the actor should have the target as a cover.
        /// </summary>
        public bool? ActorHasTargetAsCover
        {
            get
            {
                return actorHasTargetAsCover;
            }
        }
        #endregion Property: ActorHasTargetAsCover

        #region Property: TargetHasActorAsCover
        /// <summary>
        /// The value that indicates whether the target should have the actor as a cover.
        /// </summary>
        private bool? targetHasActorAsCover = null;
        
        /// <summary>
        /// Gets the value that indicates whether the target should have the actor as a cover.
        /// </summary>
        public bool? TargetHasActorAsCover
        {
            get
            {
                return targetHasActorAsCover;
            }
        }
        #endregion Property: TargetHasActorAsCover

        #region Property: ActorHasArtifactAsCover
        /// <summary>
        /// The value that indicates whether the actor should have the artifact as a cover.
        /// </summary>
        private bool? actorHasArtifactAsCover = null;

        /// <summary>
        /// Gets the value that indicates whether the actor should have the artifact as a cover.
        /// </summary>
        public bool? ActorHasArtifactAsCover
        {
            get
            {
                return actorHasArtifactAsCover;
            }
        }
        #endregion Property: ActorHasArtifactAsCover

        #region Property: ActorHasTargetAsLayer
        /// <summary>
        /// The value that indicates whether the actor should have the target as a layer.
        /// </summary>
        private bool? actorHasTargetAsLayer = null;
        
        /// <summary>
        /// Gets the value that indicates whether the actor should have the target as a layer.
        /// </summary>
        public bool? ActorHasTargetAsLayer
        {
            get
            {
                return actorHasTargetAsLayer;
            }
        }
        #endregion Property: ActorHasTargetAsLayer

        #region Property: TargetHasActorAsLayer
        /// <summary>
        /// The value that indicates whether the target should have the actor as a layer.
        /// </summary>
        private bool? targetHasActorAsLayer = null;
        
        /// <summary>
        /// Gets the value that indicates whether the target should have the actor as a layer.
        /// </summary>
        public bool? TargetHasActorAsLayer
        {
            get
            {
                return targetHasActorAsLayer;
            }
        }
        #endregion Property: TargetHasActorAsLayer

        #region Property: ActorHasArtifactAsLayer
        /// <summary>
        /// The value that indicates whether the actor should have the artifact as a layer.
        /// </summary>
        private bool? actorHasArtifactAsLayer = null;

        /// <summary>
        /// Gets the value that indicates whether the actor should have the artifact as a layer.
        /// </summary>
        public bool? ActorHasArtifactAsLayer
        {
            get
            {
                return actorHasArtifactAsLayer;
            }
        }
        #endregion Property: ActorHasArtifactAsLayer

        #region Property: ActorHasTargetAsMatter
        /// <summary>
        /// The value that indicates whether the actor should have the target as a matter.
        /// </summary>
        private bool? actorHasTargetAsMatter = null;
        
        /// <summary>
        /// Gets the value that indicates whether the actor should have the target as a matter.
        /// </summary>
        public bool? ActorHasTargetAsMatter
        {
            get
            {
                return actorHasTargetAsMatter;
            }
        }
        #endregion Property: ActorHasTargetAsMatter

        #region Property: TargetHasActorAsMatter
        /// <summary>
        /// The value that indicates whether the target should have the actor as a matter.
        /// </summary>
        private bool? targetHasActorAsMatter = null;
        
        /// <summary>
        /// Gets the value that indicates whether the target should have the actor as a matter.
        /// </summary>
        public bool? TargetHasActorAsMatter
        {
            get
            {
                return targetHasActorAsMatter;
            }
        }
        #endregion Property: TargetHasActorAsMatter

        #region Property: ActorHasArtifactAsMatter
        /// <summary>
        /// The value that indicates whether the actor should have the artifact as a matter.
        /// </summary>
        private bool? actorHasArtifactAsMatter = null;

        /// <summary>
        /// Gets the value that indicates whether the actor should have the artifact as a matter.
        /// </summary>
        public bool? ActorHasArtifactAsMatter
        {
            get
            {
                return actorHasArtifactAsMatter;
            }
        }
        #endregion Property: ActorHasArtifactAsMatter

        #region Property: ActorHasTargetAsItem
        /// <summary>
        /// The value that indicates whether the actor should have the target as a space item.
        /// </summary>
        private bool? actorHasTargetAsItem = null;
        
        /// <summary>
        /// Gets the value that indicates whether the actor should have the target as a space item.
        /// </summary>
        public bool? ActorHasTargetAsItem
        {
            get
            {
                return actorHasTargetAsItem;
            }
        }
        #endregion Property: ActorHasTargetAsItem

        #region Property: TargetHasActorAsItem
        /// <summary>
        /// The value that indicates whether the target should have the actor as a space item.
        /// </summary>
        private bool? targetHasActorAsItem = null;
        
        /// <summary>
        /// Gets the value that indicates whether the target should have the actor as a space item.
        /// </summary>
        public bool? TargetHasActorAsItem
        {
            get
            {
                return targetHasActorAsItem;
            }
        }
        #endregion Property: TargetHasActorAsItem

        #region Property: ActorHasArtifactAsItem
        /// <summary>
        /// The value that indicates whether the actor should have the artifact as a space item.
        /// </summary>
        private bool? actorHasArtifactAsItem = null;

        /// <summary>
        /// Gets the value that indicates whether the actor should have the artifact as a space item.
        /// </summary>
        public bool? ActorHasArtifactAsItem
        {
            get
            {
                return actorHasArtifactAsItem;
            }
        }
        #endregion Property: ActorHasArtifactAsItem

        #region Property: ActorAndTargetConnected
        /// <summary>
        /// The value that indicates whether the actor should be connected to the target.
        /// </summary>
        private bool? actorAndTargetConnected = null;
        
        /// <summary>
        /// Gets the value that indicates whether the actor should be connected to the target.
        /// </summary>
        public bool? ActorAndTargetConnected
        {
            get
            {
                return actorAndTargetConnected;
            }
        }
        #endregion Property: ActorAndTargetConnected

        #region Property: ActorAndTargetCollide
        /// <summary>
        /// The value that indicates whether the actor and target should collide.
        /// </summary>
        private bool? actorAndTargetCollide = null;

        /// <summary>
        /// Gets the value that indicates whether the actor and target should collide.
        /// </summary>
        public bool? ActorAndTargetCollide
        {
            get
            {
                return actorAndTargetCollide;
            }
        }
        #endregion Property: ActorAndTargetCollide

        #region Property: ActorAndTargetRelationship
        /// <summary>
        /// The required relationship between the actor and target.
        /// </summary>
        private RelationshipTypeBase actorAndTargetRelationship = null;

        /// <summary>
        /// Gets the required relationship between the actor and target.
        /// </summary>
        public RelationshipTypeBase ActorAndTargetRelationship
        {
            get
            {
                return actorAndTargetRelationship;
            }
        }
        #endregion Property: ActorAndTargetRelationship

        #region Property: ActorAndArtifactRelationship
        /// <summary>
        /// The required relationship between the actor and artifact.
        /// </summary>
        private RelationshipTypeBase actorAndArtifactRelationship = null;

        /// <summary>
        /// Gets the required relationship between the actor and artifact.
        /// </summary>
        public RelationshipTypeBase ActorAndArtifactRelationship
        {
            get
            {
                return actorAndArtifactRelationship;
            }
        }
        #endregion Property: ActorAndArtifactRelationship

        #region Property: TargetAndArtifactRelationship
        /// <summary>
        /// The required relationship between the target and artifact.
        /// </summary>
        private RelationshipTypeBase targetAndArtifactRelationship = null;

        /// <summary>
        /// Gets the required relationship between the target and artifact.
        /// </summary>
        public RelationshipTypeBase TargetAndArtifactRelationship
        {
            get
            {
                return targetAndArtifactRelationship;
            }
        }
        #endregion Property: TargetAndArtifactRelationship

        #region Property: ActorTargetRelationshipSource
        /// <summary>
        /// The source for the actor-target relationship: the actor or target; the other one will become the target of the relationship.
        /// </summary>
        private ActorTargetArtifact actorTargetRelationshipSource = default(ActorTargetArtifact);
        
        /// <summary>
        /// Gets the source for the actor-target relationship: the actor or target; the other one will become the target of the relationship.
        /// </summary>
        public ActorTargetArtifact ActorTargetRelationshipSource
        {
            get
            {
                return actorTargetRelationshipSource;
            }
        }
        #endregion Property: ActorTargetRelationshipSource

        #region Property: ActorArtifactRelationshipSource
        /// <summary>
        /// The source for the actor-artifact relationship: the actor or artifact; the other one will become the target of the relationship.
        /// </summary>
        private ActorTargetArtifact actorArtifactRelationshipSource = default(ActorTargetArtifact);

        /// <summary>
        /// Gets the source for the actor-artifact relationship: the actor or artifact; the other one will become the target of the relationship.
        /// </summary>
        public ActorTargetArtifact ActorArtifactRelationshipSource
        {
            get
            {
                return actorArtifactRelationshipSource;
            }
        }
        #endregion Property: ActorArtifactRelationshipSource

        #region Property: TargetArtifactRelationshipSource
        /// <summary>
        /// The source for the target-artifact relationship: the target or artifact; the other one will become the target of the relationship.
        /// </summary>
        private ActorTargetArtifact targetArtifactRelationshipSource = default(ActorTargetArtifact);

        /// <summary>
        /// Gets the source for the target-artifact relationship: the target or artifact; the other one will become the target of the relationship.
        /// </summary>
        public ActorTargetArtifact TargetArtifactRelationshipSource
        {
            get
            {
                return targetArtifactRelationshipSource;
            }
        }
        #endregion Property: TargetArtifactRelationshipSource

        #region Property: ActorIsUsedAsPart
        /// <summary>
        /// The value that indicates whether the actor should be used as a part.
        /// </summary>
        private bool? actorIsUsedAsPart = null;
        
        /// <summary>
        /// Gets the value that indicates whether the actor should be used as a part.
        /// </summary>
        public bool? ActorIsUsedAsPart
        {
            get
            {
                return actorIsUsedAsPart;
            }
        }
        #endregion Property: ActorIsUsedAsPart

        #region Property: ActorIsUsedAsCover
        /// <summary>
        /// The value that indicates whether the actor should be used as a cover.
        /// </summary>
        private bool? actorIsUsedAsCover = null;
        
        /// <summary>
        /// Gets the value that indicates whether the actor should be used as a cover.
        /// </summary>
        public bool? ActorIsUsedAsCover
        {
            get
            {
                return actorIsUsedAsCover;
            }
        }
        #endregion Property: ActorIsUsedAsCover

        #region Property: ActorIsUsedAsConnectionItem
        /// <summary>
        /// The value that indicates whether the actor should be used as a connection item.
        /// </summary>
        private bool? actorIsUsedAsConnectionItem = null;
        
        /// <summary>
        /// Gets the value that indicates whether the actor should be used as a connection item.
        /// </summary>
        public bool? ActorIsUsedAsConnectionItem
        {
            get
            {
                return actorIsUsedAsConnectionItem;
            }
        }
        #endregion Property: ActorIsUsedAsConnectionItem

        #region Property: ActorIsUsedAsLayer
        /// <summary>
        /// The value that indicates whether the actor should be used as a layer.
        /// </summary>
        private bool? actorIsUsedAsLayer = null;
        
        /// <summary>
        /// Gets the value that indicates whether the actor should be used as a layer.
        /// </summary>
        public bool? ActorIsUsedAsLayer
        {
            get
            {
                return actorIsUsedAsLayer;
            }
        }
        #endregion Property: ActorIsUsedAsLayer

        #region Property: ShouldNotBeSatisfied
        /// <summary>
        /// The value that indicates whether the requirements should NOT be satisfied in order to trigger the event.
        /// </summary>
        private bool shouldNotBeSatisfied = false;

        /// <summary>
        /// Gets the value that indicates whether the requirements should NOT be satisfied in order to trigger the event.
        /// </summary>
        public bool ShouldNotBeSatisfied
        {
            get
            {
                return shouldNotBeSatisfied;
            }
        }
        #endregion Property: ShouldNotBeSatisfied

        #region Field: eventBase
        /// <summary>
        /// The event base of which these are requirements.
        /// </summary>
        internal EventBase eventBase = null;
        #endregion Field: eventBase
		
        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: EventRequirementsBase(EventRequirements eventRequirements)
        /// <summary>
        /// Create an event requirements base from the given event requirements.
        /// </summary>
        /// <param name="eventRequirements">The event requirements to create an event requirements base from.</param>
        internal EventRequirementsBase(EventRequirements eventRequirements)
            : base(eventRequirements)
        {
            if (eventRequirements != null)
            {
                this.spatialRequirementBetweenActorAndTarget = BaseManager.Current.GetBase<SpatialRequirementBase>(eventRequirements.SpatialRequirementBetweenActorAndTarget);
                this.spatialRequirementBetweenActorAndArtifact = BaseManager.Current.GetBase<SpatialRequirementBase>(eventRequirements.SpatialRequirementBetweenActorAndArtifact);
                this.spatialRequirementBetweenTargetAndArtifact = BaseManager.Current.GetBase<SpatialRequirementBase>(eventRequirements.SpatialRequirementBetweenTargetAndArtifact);
                
                this.actorAndTargetCollide = eventRequirements.ActorAndTargetCollide;
                this.actorAndTargetConnected = eventRequirements.ActorAndTargetConnected;
                
                this.actorHasTargetAsCover = eventRequirements.ActorHasTargetAsCover;
                this.actorHasTargetAsItem = eventRequirements.ActorHasTargetAsItem;
                this.actorHasTargetAsLayer = eventRequirements.ActorHasTargetAsLayer;
                this.actorHasTargetAsMatter = eventRequirements.ActorHasTargetAsMatter;
                this.actorHasTargetAsPart = eventRequirements.ActorHasTargetAsPart;
                this.targetHasActorAsCover = eventRequirements.TargetHasActorAsCover;
                this.targetHasActorAsItem = eventRequirements.TargetHasActorAsItem;
                this.targetHasActorAsLayer = eventRequirements.TargetHasActorAsLayer;
                this.targetHasActorAsMatter = eventRequirements.TargetHasActorAsMatter;
                this.targetHasActorAsPart = eventRequirements.TargetHasActorAsPart;
                this.actorHasArtifactAsCover = eventRequirements.ActorHasArtifactAsCover;
                this.actorHasArtifactAsItem = eventRequirements.ActorHasArtifactAsItem;
                this.actorHasArtifactAsLayer = eventRequirements.ActorHasArtifactAsLayer;
                this.actorHasArtifactAsMatter = eventRequirements.ActorHasArtifactAsMatter;
                this.actorHasArtifactAsPart = eventRequirements.ActorHasArtifactAsPart;
                
                this.actorAndTargetRelationship = BaseManager.Current.GetBase<RelationshipTypeBase>(eventRequirements.ActorAndTargetRelationship);
                this.actorAndArtifactRelationship = BaseManager.Current.GetBase<RelationshipTypeBase>(eventRequirements.ActorAndArtifactRelationship);
                this.targetAndArtifactRelationship = BaseManager.Current.GetBase<RelationshipTypeBase>(eventRequirements.TargetAndArtifactRelationship);
                this.actorTargetRelationshipSource = eventRequirements.ActorTargetRelationshipSource;
                this.actorArtifactRelationshipSource = eventRequirements.ActorArtifactRelationshipSource;
                this.targetArtifactRelationshipSource = eventRequirements.TargetArtifactRelationshipSource;
                
                this.actorIsUsedAsConnectionItem = eventRequirements.ActorIsUsedAsConnectionItem;
                this.actorIsUsedAsCover = eventRequirements.ActorIsUsedAsCover;
                this.actorIsUsedAsLayer = eventRequirements.ActorIsUsedAsLayer;
                this.actorIsUsedAsPart = eventRequirements.ActorIsUsedAsPart;
                
                this.shouldNotBeSatisfied = eventRequirements.ShouldNotBeSatisfied;

                if (BaseManager.PreloadProperties)
                {
                    LoadActorRequirements();
                    LoadTargetRequirements();
                    LoadArtifactRequirements();
                    LoadReferenceRequirements();
                    LoadAddRemoveRequirements();
                }
            }
        }
        #endregion Constructor: EventRequirementsBase(EventRequirements eventRequirements)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: IsSatisfied(IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Checks whether the event requirements are satisfied.
        /// </summary>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the event requirements are satisfied.</returns>
        public bool IsSatisfied(IVariableInstanceHolder iVariableInstanceHolder)
        {
            EntityInstance actor = iVariableInstanceHolder.GetActor();
            EntityInstance target = iVariableInstanceHolder.GetTarget();
            EntityInstance artifact = iVariableInstanceHolder.GetArtifact();

            // Check whether the requirements have been satisfied
            if (this.ActorRequirements != null && !this.ActorRequirements.IsSatisfied(actor, iVariableInstanceHolder))
                return Return(false);
            if (this.TargetRequirements != null && !this.TargetRequirements.IsSatisfied(target, iVariableInstanceHolder))
                return Return(false);
            if (this.ArtifactRequirements != null && !this.ArtifactRequirements.IsSatisfied(artifact, iVariableInstanceHolder))
                return Return(false);
            foreach (KeyValuePair<ReferenceBase, RequirementsBase> pair in this.ReferenceRequirements)
            {
                foreach (EntityInstance entityInstance in pair.Key.GetEntities(iVariableInstanceHolder))
                {
                    if (!pair.Value.IsSatisfied(entityInstance, iVariableInstanceHolder))
                        return Return(false);
                }
            }

            // Check whether the actor and target conditions of the extensions have been satisfied
            if (eventBase != null)
            {
                foreach (IEventExtension eventExtension in eventBase.EventExtensions)
                {
                    if (actor != null)
                    {
                        foreach (ConditionBase conditionBase in eventExtension.GetActorConditions())
                        {
                            if (!actor.Satisfies(conditionBase, iVariableInstanceHolder))
                                return Return(false);
                        }
                    }
                    if (target != null)
                    {
                        foreach (ConditionBase conditionBase in eventExtension.GetTargetConditions())
                        {
                            if (!target.Satisfies(conditionBase, iVariableInstanceHolder))
                                return Return(false);
                        }
                    }
                    if (artifact != null)
                    {
                        foreach (ConditionBase conditionBase in eventExtension.GetArtifactConditions())
                        {
                            if (!artifact.Satisfies(conditionBase, iVariableInstanceHolder))
                                return Return(false);
                        }
                    }
                }
            }

            // Check whether the collision requirements have been satisfied
            if (this.ActorAndTargetCollide == true)
            {
                PhysicalEntityInstance physicalEntityActor = actor as PhysicalEntityInstance;
                PhysicalEntityInstance physicalEntityTarget = target as PhysicalEntityInstance;
                if (physicalEntityActor != null && physicalEntityTarget != null)
                {
                    if (!PhysicsManager.Current.Collide(physicalEntityActor, physicalEntityTarget))
                        return Return(false);
                }
                else
                    return Return(false);
            }

            // Check whether the right relationship are there
            if (this.ActorAndTargetRelationship != null)
            {
                if (actor == null && target == null)
                    return Return(false);

                EntityInstance relationshipSource = null;
                EntityInstance relationshipTarget = null;
                switch (this.ActorTargetRelationshipSource)
                {
                    case ActorTargetArtifact.Actor:
                        relationshipSource = actor;
                        relationshipTarget = target;
                        break;
                    case ActorTargetArtifact.Target:
                        relationshipSource = target;
                        relationshipTarget = actor;
                        break;
                    default:
                        break;
                }
                if (relationshipSource == null && relationshipTarget == null)
                    return Return(false);

                bool satisfied = false;
                foreach (RelationshipInstance relationshipInstance in relationshipSource.RelationshipsAsSource)
                {
                    if (this.ActorAndTargetRelationship.Equals(relationshipInstance.RelationshipType) && relationshipTarget.Equals(relationshipInstance.Target))
                    {
                        satisfied = true;
                        break;
                    }
                }
                if (!satisfied)
                    return Return(false);
            }
            if (this.ActorAndArtifactRelationship != null)
            {
                if (actor == null && artifact == null)
                    return Return(false);

                EntityInstance relationshipSource = null;
                EntityInstance relationshipTarget = null;
                switch (this.ActorArtifactRelationshipSource)
                {
                    case ActorTargetArtifact.Actor:
                        relationshipSource = actor;
                        relationshipTarget = artifact;
                        break;
                    case ActorTargetArtifact.Artifact:
                        relationshipSource = artifact;
                        relationshipTarget = actor;
                        break;
                    default:
                        break;
                }
                if (relationshipSource == null && relationshipTarget == null)
                    return Return(false);

                bool satisfied = false;
                foreach (RelationshipInstance relationshipInstance in relationshipSource.RelationshipsAsSource)
                {
                    if (this.ActorAndArtifactRelationship.Equals(relationshipInstance.RelationshipType) && relationshipTarget.Equals(relationshipInstance.Target))
                    {
                        satisfied = true;
                        break;
                    }
                }
                if (!satisfied)
                    return Return(false);
            }
            if (this.TargetAndArtifactRelationship != null)
            {
                if (target == null && artifact == null)
                    return Return(false);

                EntityInstance relationshipSource = null;
                EntityInstance relationshipTarget = null;
                switch (this.TargetArtifactRelationshipSource)
                {
                    case ActorTargetArtifact.Target:
                        relationshipSource = target;
                        relationshipTarget = artifact;
                        break;
                    case ActorTargetArtifact.Artifact:
                        relationshipSource = artifact;
                        relationshipTarget = target;
                        break;
                    default:
                        break;
                }
                if (relationshipSource == null && relationshipTarget == null)
                    return Return(false);

                bool satisfied = false;
                foreach (RelationshipInstance relationshipInstance in relationshipSource.RelationshipsAsSource)
                {
                    if (this.TargetAndArtifactRelationship.Equals(relationshipInstance.RelationshipType) && relationshipTarget.Equals(relationshipInstance.Target))
                    {
                        satisfied = true;
                        break;
                    }
                }
                if (!satisfied)
                    return Return(false);
            }

            // Check whether the spatial requirements have been satisfied
            PhysicalObjectInstance physicalActor = actor as PhysicalObjectInstance;
            PhysicalObjectInstance physicalTarget = target as PhysicalObjectInstance;
            PhysicalObjectInstance physicalArtifact = artifact as PhysicalObjectInstance;
            if (this.SpatialRequirementBetweenActorAndTarget != null && !this.SpatialRequirementBetweenActorAndTarget.IsSatisfied(physicalActor, physicalTarget, iVariableInstanceHolder))
                return Return(false);
            if (this.SpatialRequirementBetweenActorAndArtifact != null && !this.SpatialRequirementBetweenActorAndArtifact.IsSatisfied(physicalActor, physicalArtifact, iVariableInstanceHolder))
                return Return(false);
            if (this.SpatialRequirementBetweenTargetAndArtifact != null && !this.SpatialRequirementBetweenTargetAndArtifact.IsSatisfied(physicalTarget, physicalArtifact, iVariableInstanceHolder))
                return Return(false);

            // Check whether the add/remove requirements have been satisfied
            if (this.AddRemoveRequirements != null && !this.AddRemoveRequirements.IsSatisfied(actor, iVariableInstanceHolder))
                return Return(false);

            // Check whether the remaining requirements have been satisfied
            TangibleObjectInstance tangibleActor = actor as TangibleObjectInstance;
            TangibleObjectInstance tangibleTarget = target as TangibleObjectInstance;
            TangibleObjectInstance tangibleArtifact = artifact as TangibleObjectInstance;
            if (tangibleActor != null)
            {
                if (this.ActorAndTargetConnected == true)
                {
                    if (!tangibleActor.Connections.Contains(tangibleTarget))
                        return Return(false);
                }
                if (this.ActorHasTargetAsCover == true)
                {
                    if (!tangibleActor.Covers.Contains(tangibleTarget))
                        return Return(false);
                }
                if (this.ActorHasTargetAsItem == true)
                {
                    if (!tangibleActor.SpaceItems.Contains(tangibleTarget))
                        return Return(false);
                }
                if (this.ActorHasTargetAsPart == true)
                {
                    if (!tangibleActor.Parts.Contains(tangibleTarget))
                        return Return(false);
                }
                if (this.ActorHasTargetAsLayer == true)
                {
                    MatterInstance layerTarget = target as MatterInstance;
                    if (!tangibleActor.Layers.Contains(layerTarget))
                        return Return(false);
                }
                if (this.ActorHasTargetAsMatter == true)
                {
                    MatterInstance matterTarget = target as MatterInstance;
                    if (!tangibleActor.Matter.Contains(matterTarget))
                        return Return(false);
                }

                if (this.ActorHasArtifactAsCover == true)
                {
                    if (!tangibleActor.Covers.Contains(tangibleArtifact))
                        return Return(false);
                }
                if (this.ActorHasArtifactAsItem == true)
                {
                    if (!tangibleActor.SpaceItems.Contains(tangibleArtifact))
                        return Return(false);
                }
                if (this.ActorHasArtifactAsPart == true)
                {
                    if (!tangibleActor.Parts.Contains(tangibleArtifact))
                        return Return(false);
                }
                if (this.ActorHasArtifactAsLayer == true)
                {
                    MatterInstance layerArtifact = artifact as MatterInstance;
                    if (!tangibleActor.Layers.Contains(layerArtifact))
                        return Return(false);
                }
                if (this.ActorHasArtifactAsMatter == true)
                {
                    MatterInstance matterArtifact = artifact as MatterInstance;
                    if (!tangibleActor.Matter.Contains(matterArtifact))
                        return Return(false);
                }

                if (this.ActorIsUsedAsConnectionItem == true)
                {
                    if (tangibleActor.Connections.Count < 1)
                        return Return(false);
                }
                if (this.ActorIsUsedAsCover == true)
                {
                    if (tangibleActor.CoveredObject == null)
                        return Return(false);
                }
                if (this.ActorIsUsedAsPart == true)
                {
                    if (tangibleActor.Whole == null)
                        return Return(false);
                }
            }
            if (tangibleTarget != null)
            {
                if (this.TargetHasActorAsCover == true)
                {
                    if (!tangibleTarget.Covers.Contains(tangibleActor))
                        return Return(false);
                }
                if (this.TargetHasActorAsItem == true)
                {
                    if (!tangibleTarget.SpaceItems.Contains(tangibleActor))
                        return Return(false);
                }
                if (this.TargetHasActorAsPart == true)
                {
                    if (!tangibleTarget.Parts.Contains(tangibleActor))
                        return Return(false);
                }
                if (this.TargetHasActorAsLayer == true)
                {
                    MatterInstance layerActor = actor as MatterInstance;
                    if (!tangibleTarget.Layers.Contains(layerActor))
                        return Return(false);
                }
                if (this.TargetHasActorAsMatter == true)
                {
                    MatterInstance matterActor = actor as MatterInstance;
                    if (!tangibleTarget.Matter.Contains(matterActor))
                        return Return(false);
                }
            }

            MatterInstance layerInstanceActor = actor as MatterInstance;
            if (layerInstanceActor != null)
            {
                if (this.ActorIsUsedAsLayer == true)
                {
                    if (layerInstanceActor.Applicant == null)
                        return Return(false);
                }
            }

            return Return(true);
        }
        #endregion Method: IsSatisfied(IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Return(bool returnValue)
        /// <summary>
        /// Returns the boolean value, and adjust it if the requirements should NOT be satisfied.
        /// </summary>
        private bool Return(bool returnValue)
        {
            if (this.ShouldNotBeSatisfied)
                return !returnValue;
            return returnValue;
        }
        #endregion Method: Return(bool returnValue)
		
        #endregion Method Group: Other

    }
    #endregion Class: EventRequirementsBase
		
}