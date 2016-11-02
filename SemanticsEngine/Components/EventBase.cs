/**************************************************************************
 * 
 * EventBase.cs
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
using Semantics.Components;
using Semantics.Entities;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Entities;
using SemanticsEngine.Interfaces;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: EventBase
    /// <summary>
    /// A base of an event.
    /// </summary>
    public sealed class EventBase : ServiceBase
    {

        #region Properties and Fields

        #region Property: Event
        /// <summary>
        /// Gets the event of which this is an event base.
        /// </summary>
        internal Event Event
        {
            get
            {
                return this.Service as Event;
            }
        }
        #endregion Property: Event

        #region Property: Actor
        /// <summary>
        /// The actor of the event.
        /// </summary>
        private EntityBase actor = null;

        /// <summary>
        /// Gets the actor of the event.
        /// </summary>
        public EntityBase Actor
        {
            get
            {
                return actor;
            }
        }
        #endregion Property: Actor

        #region Property: Action
        /// <summary>
        /// The action of the event.
        /// </summary>
        private ActionBase action = null;
        
        /// <summary>
        /// Gets the action of the event.
        /// </summary>
        public ActionBase Action
        {
            get
            {
                return action;
            }
        }
        #endregion Property: Action

        #region Property: Target
        /// <summary>
        /// The target of the event.
        /// </summary>
        private EntityBase target = null;

        /// <summary>
        /// Gets the target of the event.
        /// </summary>
        public EntityBase Target
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
        private EntityBase artifact = null;

        /// <summary>
        /// Gets the artifact of the event.
        /// </summary>
        public EntityBase Artifact
        {
            get
            {
                return artifact;
            }
        }
        #endregion Property: Artifact

        #region Property: Requirements
        /// <summary>
        /// The requirements of the event.
        /// </summary>
        private EventRequirementsBase requirements = null;

        /// <summary>
        /// Gets the requirements of the event.
        /// </summary>
        public EventRequirementsBase Requirements
        {
            get
            {
                if (requirements == null)
                    LoadRequirements();
                return requirements;
            }
        }

        /// <summary>
        /// Loads the requirements.
        /// </summary>
        private void LoadRequirements()
        {
            if (this.Event != null)
            {
                requirements = BaseManager.Current.GetBase<EventRequirementsBase>(this.Event.Requirements);
                requirements.eventBase = this;
            }
        }
        #endregion Property: Requirements

        #region Property: Effects
        /// <summary>
        /// The effects of the event.
        /// </summary>
        private EffectBase[] effects = null;

        /// <summary>
        /// Gets the effects of the event.
        /// </summary>
        public IEnumerable<EffectBase> Effects
        {
            get
            {
                if (effects == null)
                    LoadEffects();
                foreach (EffectBase effectBase in effects)
                    yield return effectBase;
            }
        }

        /// <summary>
        /// Loads the effects.
        /// </summary>
        private void LoadEffects()
        {
            List<EffectBase> effectBases = new List<EffectBase>();
            if (this.Event != null)
            {
                // Get the effects of the event
                foreach (Effect effect in this.Event.Effects)
                    effectBases.Add(BaseManager.Current.GetBase<EffectBase>(effect));

                // Get the effects of the extensions
                foreach (IEventExtension eventExtension in this.EventExtensions)
                    effectBases.AddRange(eventExtension.GetEffects());

                // Sort by priority
                effectBases.Sort(delegate(EffectBase c1, EffectBase c2) { return c2.Priority.CompareTo(c1.Priority); });
            }
            effects = effectBases.ToArray();
        }
        #endregion Property: Effects

        #region Property: Changes
        /// <summary>
        /// Gets the changes of the event.
        /// </summary>
        public IEnumerable<ChangeBase> Changes
        {
            get
            {
                foreach (EffectBase effectBase in this.Effects)
                {
                    ChangeBase changeBase = effectBase as ChangeBase;
                    if (changeBase != null)
                        yield return changeBase;
                }
            }
        }
        #endregion Property: Changes

        #region Property: Creations
        /// <summary>
        /// Gets the entity creations of the event.
        /// </summary>
        public IEnumerable<EntityCreationBase> Creations
        {
            get
            {
                foreach (EffectBase effectBase in this.Effects)
                {
                    EntityCreationBase entityCreationBase = effectBase as EntityCreationBase;
                    if (entityCreationBase != null)
                        yield return entityCreationBase;
                }
            }
        }
        #endregion Property: Creations

        #region Property: Deletions
        /// <summary>
        /// Gets the entity deletions of the event.
        /// </summary>
        public IEnumerable<DeletionBase> Deletions
        {
            get
            {
                foreach (EffectBase effectBase in this.Effects)
                {
                    DeletionBase deletionBase = effectBase as DeletionBase;
                    if (deletionBase != null)
                        yield return deletionBase;
                }
            }
        }
        #endregion Property: Deletions

        #region Property: Reactions
        /// <summary>
        /// Gets the reactions of the event.
        /// </summary>
        public IEnumerable<ReactionBase> Reactions
        {
            get
            {
                foreach (EffectBase effectBase in this.Effects)
                {
                    ReactionBase reactionBase = effectBase as ReactionBase;
                    if (reactionBase != null)
                        yield return reactionBase;
                }
            }
        }
        #endregion Property: Reactions

        #region Property: Transfers
        /// <summary>
        /// Gets the transfers of the event.
        /// </summary>
        public IEnumerable<TransferBase> Transfers
        {
            get
            {
                foreach (EffectBase effectBase in this.Effects)
                {
                    TransferBase transferBase = effectBase as TransferBase;
                    if (transferBase != null)
                        yield return transferBase;
                }
            }
        }
        #endregion Property: Transfers

        #region Property: RelationshipEstablishments
        /// <summary>
        /// Gets the relationship establishments of the event.
        /// </summary>
        public IEnumerable<RelationshipEstablishmentBase> RelationshipEstablishments
        {
            get
            {
                foreach (EffectBase effectBase in this.Effects)
                {
                    RelationshipEstablishmentBase relationshipEstablishmentBase = effectBase as RelationshipEstablishmentBase;
                    if (relationshipEstablishmentBase != null)
                        yield return relationshipEstablishmentBase;
                }
            }
        }
        #endregion Property: RelationshipEstablishments

        #region Property: FilterApplications
        /// <summary>
        /// Gets the filter applications of the event.
        /// </summary>
        public IEnumerable<FilterApplicationBase> FilterApplications
        {
            get
            {
                foreach (EffectBase effectBase in this.Effects)
                {
                    FilterApplicationBase filterApplicationBase = effectBase as FilterApplicationBase;
                    if (filterApplicationBase != null)
                        yield return filterApplicationBase;
                }
            }
        }
        #endregion Property: FilterApplications

        #region Property: Time
        /// <summary>
        /// The time of the event.
        /// </summary>
        private TimeBase time = null;

        /// <summary>
        /// Gets the time of the event.
        /// </summary>
        public TimeBase Time
        {
            get
            {
                if (time == null)
                    LoadTime();
                return time;
            }
        }

        /// <summary>
        /// Loads the time.
        /// </summary>
        private void LoadTime()
        {
            if (this.Event != null)
                time = BaseManager.Current.GetBase<TimeBase>(this.Event.Time);
        }
        #endregion Property: Time

        #region Property: LevelOfDetail
        /// <summary>
        /// Indicates the required amount of detail before this event can be triggered.
        /// </summary>
        private byte levelOfDetail = SemanticsSettings.Values.LevelOfDetail;

        /// <summary>
        /// Gets the value that indicates the required amount of detail before this event can be triggered.
        /// </summary>
        public byte LevelOfDetail
        {
            get
            {
                return levelOfDetail;
            }
        }
        #endregion Property: LevelOfDetail

        #region Property: Behavior
        /// <summary>
        /// The behavior of the event indicates when it should be executed.
        /// </summary>
        private EventBehavior behavior = default(EventBehavior);

        /// <summary>
        /// Gets the behavior of the event indicates when it should be executed.
        /// </summary>
        public EventBehavior Behavior
        {
            get
            {
                return behavior;
            }
        }
        #endregion Property: Behavior

        #region Property: NrOfSimultaneousUses
        /// <summary>
        /// Gets the value that indicates the maximum number of times this event can be executed simultaneously.
        /// </summary>
        private int nrOfSimultaneousUses = SemanticsSettings.Values.NrOfSimultaneousUses;
        
        /// <summary>
        /// Gets the value that indicates the maximum number of times this event can be executed simultaneously.
        /// </summary>
        public int NrOfSimultaneousUses
        {
            get
            {
                return nrOfSimultaneousUses;
            }
        }
        #endregion Property: NrOfSimultaneousUses

        #region Property: Variables
        /// <summary>
        /// The variables.
        /// </summary>
        private VariableBase[] variables = null;

        /// <summary>
        /// Gets the variables.
        /// </summary>
        public IEnumerable<VariableBase> Variables
        {
            get
            {
                if (variables == null)
                    LoadVariables();
                foreach (VariableBase variableBase in variables)
                    yield return variableBase;
            }
        }

        /// <summary>
        /// Loads the variables.
        /// </summary>
        private void LoadVariables()
        {
            List<VariableBase> variableBases = new List<VariableBase>();
            if (this.Event != null)
            {
                foreach (Variable variable in this.Event.Variables)
                    variableBases.Add(BaseManager.Current.GetBase<VariableBase>(variable));
            }
            variables = variableBases.ToArray();
        }
        #endregion Property: Variables

        #region Property: References
        /// <summary>
        /// The references.
        /// </summary>
        private ReferenceBase[] references = null;

        /// <summary>
        /// Gets the references.
        /// </summary>
        public IEnumerable<ReferenceBase> References
        {
            get
            {
                if (references == null)
                    LoadReferences();
                foreach (ReferenceBase referenceBase in references)
                    yield return referenceBase;
            }
        }

        /// <summary>
        /// Loads the references.
        /// </summary>
        private void LoadReferences()
        {
            List<ReferenceBase> referenceBases = new List<ReferenceBase>();
            if (this.Event != null)
            {
                foreach (Reference reference in this.Event.References)
                    referenceBases.Add(BaseManager.Current.GetBase<ReferenceBase>(reference));
            }
            references = referenceBases.ToArray();
        }
        #endregion Property: References

        #region Property: EventExtensions
        /// <summary>
        /// The event extensions.
        /// </summary>
        private IEventExtension[] eventExtensions = null;

        /// <summary>
        /// Gets the event extensions.
        /// </summary>
        public ReadOnlyCollection<IEventExtension> EventExtensions
        {
            get
            {
                if (eventExtensions != null)
                    return new List<IEventExtension>(eventExtensions).AsReadOnly();
                return new List<IEventExtension>(0).AsReadOnly();
            }
        }
        #endregion Property: EventExtensions

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: EventBase(Event even)
        /// <summary>
        /// Create an event base from the given event.
        /// </summary>
        /// <param name="event">The event to create an event base from.</param>
        internal EventBase(Event even)
            : base(even)
        {
            if (even != null)
            {
                this.actor = BaseManager.Current.GetBase<EntityBase>(even.Actor);
                this.action = BaseManager.Current.GetBase<ActionBase>(even.Action);
                this.target = BaseManager.Current.GetBase<EntityBase>(even.Target);
                this.artifact = BaseManager.Current.GetBase<EntityBase>(even.Artifact);
                this.levelOfDetail = even.LevelOfDetail;
                this.behavior = even.Behavior;
                this.nrOfSimultaneousUses = even.NrOfSimultaneousUses;

                if (BaseManager.PreloadProperties)
                {
                    LoadRequirements();
                    LoadEffects();
                    LoadTime();
                    LoadVariables();
                    LoadReferences();
                }
            }
        }
        #endregion Constructor: EventBase(Event even)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddExtension(IEventExtension eventExtension)
        /// <summary>
        /// Add an event extension.
        /// </summary>
        /// <param name="eventExtension">The event extension to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddExtension(IEventExtension eventExtension)
        {
            if (eventExtension != null)
            {
                // If the event extension is already available in all event extensions, there is no use to add it
                if (this.EventExtensions.Contains(eventExtension))
                    return Message.RelationExistsAlready;

                // Add the event extension to the event extensions
                Utils.AddToArray<IEventExtension>(ref this.eventExtensions, eventExtension);

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddExtension(IEventExtension eventExtension)

        #region Method: RemoveExtension(IEventExtension eventExtension)
        /// <summary>
        /// Removes an event extension.
        /// </summary>
        /// <param name="eventExtension">The event extension to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        internal Message RemoveExtension(IEventExtension eventExtension)
        {
            if (eventExtension != null)
            {
                if (this.EventExtensions.Contains(eventExtension))
                {
                    // Remove the event extension
                    Utils.RemoveFromArray<IEventExtension>(ref this.eventExtensions, eventExtension);

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveExtension(IEventExtension eventExtension)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString()
        {
            if (this.Action != null)
                return this.Action.ToString();

            return base.ToString();
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: EventBase

}
