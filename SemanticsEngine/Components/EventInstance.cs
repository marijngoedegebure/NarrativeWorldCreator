/**************************************************************************
 * 
 * EventInstance.cs
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
using System.Collections.ObjectModel;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Entities;
using SemanticsEngine.Interfaces;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: EventInstance
    /// <summary>
    /// An instance of an event.
    /// </summary>
    public sealed class EventInstance : ServiceInstance, IVariableInstanceHolder
    {

        #region Properties and Fields

        #region Property: EventBase
        /// <summary>
        /// Gets the event base of which this is an event instance.
        /// </summary>
        internal EventBase EventBase
        {
            get
            {
                return this.Base as EventBase;
            }
        }
        #endregion Property: EventBase

        #region Property: Actor
        /// <summary>
        /// The actor of the event.
        /// </summary>
        private EntityInstance actor = null;
        
        /// <summary>
        /// Gets the actor of the event.
        /// </summary>
        internal EntityInstance Actor
        {
            get
            {
                return actor;
            }
        }
        #endregion Property: Actor

        #region Property: Action
        /// <summary>
        /// Gets the action of the event.
        /// </summary>
        internal ActionBase Action
        {
            get
            {
                if (this.EventBase != null)
                    return this.EventBase.Action;
                return null;
            }
        }
        #endregion Property: Action

        #region Property: Target
        /// <summary>
        /// The target of the event.
        /// </summary>
        private EntityInstance target = null;

        /// <summary>
        /// Gets the target of the event.
        /// </summary>
        internal EntityInstance Target
        {
            get
            {
                return target;
            }
        }
        #endregion Property: Target

        #region Property: Artifact
        /// <summary>
        /// The artifact of the event.
        /// </summary>
        private EntityInstance artifact = null;

        /// <summary>
        /// Gets the artifact of the event.
        /// </summary>
        internal EntityInstance Artifact
        {
            get
            {
                return artifact;
            }
        }
        #endregion Property: Artifact

        #region Property: ManualInput
        /// <summary>
        /// The manual inputted variables of the event.
        /// </summary>
        private Dictionary<string, object> manualInput = null;

        /// <summary>
        /// Gets the manual inputted variables of the event.
        /// </summary>
        internal Dictionary<string, object> ManualInput
        {
            get
            {
                return manualInput;
            }
        }
        #endregion Property: ManualInput

        #region Property: Requirements
        /// <summary>
        /// Gets the requirements.
        /// </summary>
        internal EventRequirementsBase Requirements
        {
            get
            {
                if (this.EventBase != null)
                    return this.EventBase.Requirements;
                return null;
            }
        }
        #endregion Property: Requirements

        #region Property: Effects
        /// <summary>
        /// Gets the effects of the event.
        /// </summary>
        internal IEnumerable<EffectBase> Effects
        {
            get
            {
                if (this.EventBase != null)
                {
                    foreach (EffectBase effectBase in this.EventBase.Effects)
                        yield return effectBase;
                }
            }
        }
        #endregion Property: Effects

        #region Property: Changes
        /// <summary>
        /// Gets the changes of the event.
        /// </summary>
        internal IEnumerable<ChangeBase> Changes
        {
            get
            {
                if (this.EventBase != null)
                {
                    foreach (ChangeBase changeBase in this.EventBase.Changes)
                        yield return changeBase;
                }
            }
        }
        #endregion Property: Changes

        #region Property: Creations
        /// <summary>
        /// Gets the entity creations of the event.
        /// </summary>
        internal IEnumerable<EntityCreationBase> Creations
        {
            get
            {
                if (this.EventBase != null)
                {
                    foreach (EntityCreationBase entityCreationBase in this.EventBase.Creations)
                        yield return entityCreationBase;
                }
            }
        }
        #endregion Property: Creations

        #region Property: Deletions
        /// <summary>
        /// Gets the entity deletions of the event.
        /// </summary>
        internal IEnumerable<DeletionBase> Deletions
        {
            get
            {
                if (this.EventBase != null)
                {
                    foreach (DeletionBase deletionBase in this.EventBase.Deletions)
                        yield return deletionBase;
                }
            }
        }
        #endregion Property: Deletions

        #region Property: Reactions
        /// <summary>
        /// Gets the reactions of the event.
        /// </summary>
        internal IEnumerable<ReactionBase> Reactions
        {
            get
            {
                if (this.EventBase != null)
                {
                    foreach (ReactionBase reactionBase in this.EventBase.Reactions)
                        yield return reactionBase;
                }
            }
        }
        #endregion Property: Reactions

        #region Property: Transfers
        /// <summary>
        /// Gets the transfers of the event.
        /// </summary>
        internal IEnumerable<TransferBase> Transfers
        {
            get
            {
                if (this.EventBase != null)
                {
                    foreach (TransferBase transferBase in this.EventBase.Transfers)
                        yield return transferBase;
                }
            }
        }
        #endregion Property: Transfers

        #region Property: RelationshipEstablishments
        /// <summary>
        /// Gets the relationship establishments of the event.
        /// </summary>
        internal IEnumerable<RelationshipEstablishmentBase> RelationshipEstablishments
        {
            get
            {
                if (this.EventBase != null)
                {
                    foreach (RelationshipEstablishmentBase relationshipEstablishmentBase in this.EventBase.RelationshipEstablishments)
                        yield return relationshipEstablishmentBase;
                }
            }
        }
        #endregion Property: RelationshipEstablishments

        #region Property: FilterApplications
        /// <summary>
        /// Gets the filter applications of the event.
        /// </summary>
        internal IEnumerable<FilterApplicationBase> FilterApplications
        {
            get
            {
                if (this.EventBase != null)
                {
                    foreach (FilterApplicationBase filterApplicationBase in this.EventBase.FilterApplications)
                        yield return filterApplicationBase;
                }
            }
        }
        #endregion Property: FilterApplications

        #region Property: Time
        /// <summary>
        /// Gets the time of the event.
        /// </summary>
        internal TimeBase Time
        {
            get
            {
                if (this.EventBase != null)
                    return this.EventBase.Time;
                return null;
            }
        }
        #endregion Property: Time

        #region Property: LevelOfDetail
        /// <summary>
        /// Gets the value that indicates the required amount of detail before this event can be triggered.
        /// </summary>
        internal byte LevelOfDetail
        {
            get
            {
                if (this.EventBase != null)
                    return this.EventBase.LevelOfDetail;
                return SemanticsSettings.Values.LevelOfDetail;
            }
        }
        #endregion Property: LevelOfDetail

        #region Property: Behavior
        /// <summary>
        /// Gets the behavior of the event indicates when it should be executed.
        /// </summary>
        internal EventBehavior Behavior
        {
            get
            {
                if (this.EventBase != null)
                    return this.EventBase.Behavior;
                return default(EventBehavior);
            }
        }
        #endregion Property: Behavior        
        
        #region Property: ActiveEffects
        /// <summary>
        /// All active effects of the event instance.
        /// </summary>
        private EffectInstance[] activeEffects = null;

        /// <summary>
        /// Gets all active effects of the event instance.
        /// </summary>
        internal ReadOnlyCollection<EffectInstance> ActiveEffects
        {
            get
            {
                if (activeEffects == null)
                    return new List<EffectInstance>(0).AsReadOnly();

                return new ReadOnlyCollection<EffectInstance>(activeEffects);
            }
        }
        #endregion Property: ActiveEffects

        #region Field: remainingDelay
        /// <summary>
        /// The remaining delay.
        /// </summary>
        internal float remainingDelay = 0;
        #endregion Field: remainingDelay

        #region Field: remainingFrequency
        /// <summary>
        /// The remaining frequency.
        /// </summary>
        internal int remainingFrequency = 0;
        #endregion Field: remainingFrequency

        #region Field: remainingDuration
        /// <summary>
        /// The remaining duration.
        /// </summary>
        internal float remainingDuration = 0;
        #endregion Field: remainingDuration

        #region Field: remainingInterval
        /// <summary>
        /// The remaining interval.
        /// </summary>
        internal float remainingInterval = 0;
        #endregion Field: remainingInterval

        #region Field: totalInterval
        /// <summary>
        /// The total interval.
        /// </summary>
        internal float totalInterval = 0;
        #endregion Field: totalInterval

        #region Field: durationDependentOnRequirements
        /// <summary>
        /// Indicates whether the duration is dependent on the requirements.
        /// </summary>
        internal bool durationDependentOnRequirements = false;
        #endregion Field: durationDependentOnRequirements

        #region Field: infiniteFrequency
        /// <summary>
        /// Indicates whether the frequency is infinite.
        /// </summary>
        internal bool infiniteFrequency = false;
        #endregion Field: infiniteFrequency

        #region Field: variableReferenceInstanceHolder
        /// <summary>
        /// The variable instance holder.
        /// </summary>
        private VariableReferenceInstanceHolder variableReferenceInstanceHolder = new VariableReferenceInstanceHolder();
        #endregion Field: variableReferenceInstanceHolder

        #region Field: firstExecution
        /// <summary>
        /// Indicates whether the event is executed for the first time.
        /// </summary>
        internal bool firstExecution = true;
        #endregion Field: firstExecution

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: EventInstance(EntityInstance actor, EventBase eventBase, EntityInstance target, EntityInstance artifact, Dictionary<string, object> manualInput)
        /// <summary>
        /// Creates a new event instance for the given actor from the given event base.
        /// </summary>
        /// <param name="actor">The (required) actor that will perform the event.</param>
        /// <param name="eventBase">The (required) event base to create the event instance from.</param>
        /// <param name="target">The (optional) target of the event.</param>
        /// <param name="artifact">The (optional) artifact of the event.</param>
        /// <param name="manualInput">The (optional) manually inputted variables.</param>
        internal EventInstance(EntityInstance actor, EventBase eventBase, EntityInstance target, EntityInstance artifact, Dictionary<string, object> manualInput)
            : base(eventBase)
        {
            this.actor = actor;
            this.target = target;
            this.artifact = artifact;
            this.manualInput = manualInput;
        }
        #endregion Constructor: EventInstance(EntityInstance actor, EventBase eventBase, EntityInstance target, EntityInstance artifact, Dictionary<string, object> manualInput)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddActiveEffect(EffectInstance effectInstance)
        /// <summary>
        /// Adds an active effect.
        /// </summary>
        /// <param name="effectInstance">The effect to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        internal Message AddActiveEffect(EffectInstance effectInstance)
        {
            if (effectInstance != null)
            {
                // If the effect is already available in all active effects, there is no use to add it
                if (this.ActiveEffects.Contains(effectInstance))
                    return Message.RelationExistsAlready;

                // Add the effect to the active effects
                Utils.AddToArray<EffectInstance>(ref this.activeEffects, effectInstance);
                NotifyPropertyChanged("ActiveEffects");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddActiveEffect(EffectInstance effectInstance)

        #region Method: RemoveActiveEffect(EffectInstance effectInstance)
        /// <summary>
        /// Removes an active effect.
        /// </summary>
        /// <param name="effectInstance">The effect to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        internal Message RemoveActiveEffect(EffectInstance effectInstance)
        {
            if (effectInstance != null)
            {
                if (this.ActiveEffects.Contains(effectInstance))
                {
                    // Remove the effect
                    Utils.RemoveFromArray<EffectInstance>(ref this.activeEffects, effectInstance);
                    NotifyPropertyChanged("ActiveEffects");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveActiveEffect(EffectInstance effectInstance)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

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
            return this.manualInput;
        }
        #endregion Method: GetManualInput()

        #region Method: GetActor()
        /// <summary>
        /// Get the actor.
        /// </summary>
        /// <returns>The actor.</returns>
        public EntityInstance GetActor()
        {
            return this.Actor;
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
            return this.Artifact;
        }
        #endregion Method: GetArtifact()

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString()
        {
            if (this.EventBase != null)
                return this.EventBase.ToString();

            return base.ToString();
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: EventInstance

}
