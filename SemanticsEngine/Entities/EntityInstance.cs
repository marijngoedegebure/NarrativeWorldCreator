/**************************************************************************
 * 
 * EntityInstance.cs
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
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Components;
using SemanticsEngine.Interfaces;
using SemanticsEngine.Tools;
using SemanticsEngine.Worlds;

namespace SemanticsEngine.Entities
{

    #region Class: EntityInstance
    /// <summary>
    /// An instance of an entity.
    /// </summary>
    public abstract class EntityInstance : NodeInstance, IVariableInstanceHolder
    {

        #region Events, Properties, and Fields

        #region Events: AbstractEntityHandler
        /// <summary>
        /// A handler for added or removed abstract entities.
        /// </summary>
        /// <param name="sender">The entity instance the abstract entity was added to or removed from.</param>
        /// <param name="abstractEntity">The added or removed abstract entity.</param>
        public delegate void AbstractEntityHandler(EntityInstance sender, AbstractEntityInstance abstractEntity);

        /// <summary>
        /// An event to indicate an added abstract entity.
        /// </summary>
        public event AbstractEntityHandler AbstractEntityAdded;

        /// <summary>
        /// An event to indicate a removed abstract entity.
        /// </summary>
        public event AbstractEntityHandler AbstractEntityRemoved;
        #endregion Events: AbstractEntityHandler

        #region Events: ActionHandler
        /// <summary>
        /// A handler for executed actions.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="action">The action.</param>
        /// <param name="target">The target.</param>
        public delegate void ActionHandler(EntityInstance actor, ActionBase action, EntityInstance target);

        /// <summary>
        /// Invoked when this entity instance starts an action.
        /// </summary>
        public event ActionHandler StartedAction;

        /// <summary>
        /// Invoked when this entity instance executes an action.
        /// </summary>
        public event ActionHandler ExecutedAction;

        /// <summary>
        /// Invoked when this entity instance stops an action.
        /// </summary>
        public event ActionHandler StoppedAction;

        /// <summary>
        /// Invoked when an action is executed on this entity instance.
        /// </summary>
        public event ActionHandler AffectedByAction;
        #endregion Events: ActionHandler

        #region Events: ContextTypeHandler
        /// <summary>
        /// A handler for added or removed context types.
        /// </summary>
        /// <param name="entityInstance">The entity instance.</param>
        /// <param name="contextTypeBase">The context type.</param>
        public delegate void ContextTypeHandler(EntityInstance entityInstance, ContextTypeBase contextTypeBase);

        /// <summary>
        /// Invoked when this entity instance gets a new context type.
        /// </summary>
        public event ContextTypeHandler ContextTypeAdded;

        /// <summary>
        /// Invoked when this entity instance loses a context type.
        /// </summary>
        public event ContextTypeHandler ContextTypeRemoved;
        #endregion Events: ContextTypeHandler

        #region Event: FilterHandler
        /// <summary>
        /// A handler for applied filters.
        /// </summary>
        /// <param name="entityInstance">The entity instance..</param>
        /// <param name="filter">The filter that has been applied.</param>
        public delegate void FilterHandler(EntityInstance entityInstance, FilterTypeBase filter);

        /// <summary>
        /// Invoked when a filter is applied to this entity instance.
        /// </summary>
        public event FilterHandler FilterApplied;
        #endregion Event: FilterHandler

        #region Property: EntityBase
        /// <summary>
        /// Gets the entity base of which this is an entity instance.
        /// </summary>
        public EntityBase EntityBase
        {
            get
            {
                return this.NodeBase as EntityBase;
            }
        }
        #endregion Property: EntityBase

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
                    if (this.EntityBase != null)
                    {
                        // Create instances from the base valued attributes
                        foreach (AttributeValuedBase attributeValuedBase in this.EntityBase.Attributes)
                        {
                            if (attributeValuedBase.AttributeBase != null && attributeValuedBase.AttributeBase.AttributeType != AttributeType.Constant)
                            {
                                AttributeInstance attributeInstance = InstanceManager.Current.Create<AttributeInstance>(attributeValuedBase);
                                attributeInstance.Owner = this;
                                attributeInstances.Add(attributeInstance);
                            }
                        }
                    }
                    mutableAttributes = attributeInstances.ToArray();
                }

                foreach (AttributeInstance attributeInstance in mutableAttributes)
                    yield return attributeInstance;

                if (this.EntityBase != null)
                {
                    foreach (AttributeInstance attributeInstance in this.EntityBase.ConstantAttributes)
                        yield return attributeInstance;
                }
            }
        }
        #endregion Property: Attributes

        #region Property: StateGroups
        /// <summary>
        /// The state groups.
        /// </summary>
        private StateGroupInstance[] stateGroups = null;
        
        /// <summary>
        /// Gets the state groups.
        /// </summary>
        public IEnumerable<StateGroupInstance> StateGroups
        {
            get
            {
                if (stateGroups == null)
                {
                    List<StateGroupInstance> stateGroupInstances = new List<StateGroupInstance>();
                    if (this.EntityBase != null)
                    {
                        // Create instances from the base state groups
                        foreach (StateGroupBase stateGroupBase in this.EntityBase.StateGroups)
                            stateGroupInstances.Add(InstanceManager.Current.Create<StateGroupInstance>(stateGroupBase));
                    }
                    stateGroups = stateGroupInstances.ToArray();
                }

                foreach (StateGroupInstance stateGroupInstance in stateGroups)
                    yield return stateGroupInstance;
            }
        }
        #endregion Property: StateGroups

        #region Property: States
        /// <summary>
        /// Gets the current states of the entity.
        /// </summary>
        public ReadOnlyCollection<StateBase> States
        {
            get
            {
                List<StateBase> states = new List<StateBase>();
                foreach (StateGroupInstance stateGroupInstance in this.StateGroups)
                {
                    if (stateGroupInstance.State != null)
                        states.Add(stateGroupInstance.State);
                }
                return states.AsReadOnly();
            }
        }
        #endregion Property: States

        #region Property: PredicatesAsActor
        /// <summary>
        /// All predicates where this entity is the actor.
        /// </summary>
        private PredicateInstance[] predicatesAsActor = null;

        /// <summary>
        /// Gets all predicates where this entity is the actor.
        /// </summary>
        public ReadOnlyCollection<PredicateInstance> PredicatesAsActor
        {
            get
            {
                if (predicatesAsActor == null)
                    return new List<PredicateInstance>(0).AsReadOnly();
                return new ReadOnlyCollection<PredicateInstance>(predicatesAsActor);
            }
        }
        #endregion Property: PredicatesAsActor

        #region Property: PredicatesAsTarget
        /// <summary>
        /// All predicates where this entity is the target.
        /// </summary>
        private PredicateInstance[] predicatesAsTarget = null;

        /// <summary>
        /// Gets all predicates where this entity is the target.
        /// </summary>
        public ReadOnlyCollection<PredicateInstance> PredicatesAsTarget
        {
            get
            {
                if (predicatesAsTarget == null)
                    return new List<PredicateInstance>(0).AsReadOnly();
                return new ReadOnlyCollection<PredicateInstance>(predicatesAsTarget);
            }
        }
        #endregion Property: PredicatesAsTarget

        #region Property: RelationshipsAsSource
        /// <summary>
        /// All relationships where this entity is the source.
        /// </summary>
        private RelationshipInstance[] relationshipsAsSource = null;

        /// <summary>
        /// Gets all relationships where this entity is the source.
        /// </summary>
        public ReadOnlyCollection<RelationshipInstance> RelationshipsAsSource
        {
            get
            {
                if (relationshipsAsSource == null)
                    return new List<RelationshipInstance>(0).AsReadOnly();
                return new ReadOnlyCollection<RelationshipInstance>(relationshipsAsSource);
            }
        }
        #endregion Property: RelationshipsAsSource

        #region Property: RelationshipsAsTarget
        /// <summary>
        /// All relationships where this entity is the target.
        /// </summary>
        private RelationshipInstance[] relationshipsAsTarget = null;

        /// <summary>
        /// Gets all relationships where this entity is the target.
        /// </summary>
        public ReadOnlyCollection<RelationshipInstance> RelationshipsAsTarget
        {
            get
            {
                if (relationshipsAsTarget == null)
                    return new List<RelationshipInstance>(0).AsReadOnly();
                return new ReadOnlyCollection<RelationshipInstance>(relationshipsAsTarget);
            }
        }
        #endregion Property: RelationshipsAsTarget

        #region Property: CombinedRelationships
        /// <summary>
        /// Gets the combined relationships.
        /// </summary>
        internal ReadOnlyCollection<CombinedRelationshipBase> CombinedRelationships
        {
            get
            {
                if (this.EntityBase != null)
                    return this.EntityBase.CombinedRelationships;

                return new List<CombinedRelationshipBase>(0).AsReadOnly();
            }
        }
        #endregion Property: CombinedRelationships

        #region Property: AbstractEntities
        /// <summary>
        /// The abstract entities of the entity instance.
        /// </summary>
        private AbstractEntityInstance[] abstractEntities = null;

        /// <summary>
        /// Gets the abstract entities of the entity instance.
        /// </summary>
        public ReadOnlyCollection<AbstractEntityInstance> AbstractEntities
        {
            get
            {
                if (abstractEntities != null)
                    return new ReadOnlyCollection<AbstractEntityInstance>(abstractEntities);

                return new List<AbstractEntityInstance>(0).AsReadOnly();
            }
        }
        #endregion Property: AbstractEntities

        #region Property: Actions
        /// <summary>
        /// Gets all the actions that the entity instance can perform.
        /// </summary>
        public ReadOnlyCollection<ActionBase> Actions
        {
            get
            {
                if (this.EntityBase != null)
                    return this.EntityBase.Actions;
                return new List<ActionBase>(0).AsReadOnly();
            }
        }
        #endregion Property: Actions

        #region Property: ManualEvents
        /// <summary>
        /// Gets all the events that the entity instance can execute on request.
        /// </summary>
        internal ReadOnlyCollection<EventBase> ManualEvents
        {
            get
            {
                if (this.EntityBase != null)
                    return this.EntityBase.ManualEvents;

                return new List<EventBase>(0).AsReadOnly();
            }
        }
        #endregion Property: ManualEvents

        #region Property: AutomaticEvents
        /// <summary>
        /// Gets all the automatic events of the entity instance.
        /// </summary>
        internal ReadOnlyCollection<EventBase> AutomaticEvents
        {
            get
            {
                if (this.EntityBase != null)
                    return this.EntityBase.AutomaticEvents;

                return new List<EventBase>(0).AsReadOnly();
            }
        }
        #endregion Property: AutomaticEvents

        #region Property: ActiveEvents
        /// <summary>
        /// All active events of the entity instance.
        /// </summary>
        private EventInstance[] activeEvents = null;

        /// <summary>
        /// Gets all active events of the entity instance.
        /// </summary>
        internal ReadOnlyCollection<EventInstance> ActiveEvents
        {
            get
            {
                if (activeEvents == null)
                    return new List<EventInstance>(0).AsReadOnly();

                return new ReadOnlyCollection<EventInstance>(activeEvents);
            }
        }
        #endregion Property: ActiveEvents

        #region Property: PossibleContextTypes
        /// <summary>
        /// All the possible context types.
        /// </summary>
        public ReadOnlyCollection<ContextTypeBase> PossibleContextTypes
        {
            get
            {
                if (this.EntityBase != null)
                    return this.EntityBase.PossibleContextTypes;
                return new List<ContextTypeBase>(0).AsReadOnly();
            }
        }
        #endregion Property: PossibleContextTypes
		
        #region Property: ContextTypes
        /// <summary>
        /// All current context types of the entity instance.
        /// </summary>
        private ContextTypeBase[] contextTypes = null;

        /// <summary>
        /// Gets all current context types of the entity instance.
        /// </summary>
        public ReadOnlyCollection<ContextTypeBase> ContextTypes
        {
            get
            {
                if (contextTypes == null)
                    return new List<ContextTypeBase>(0).AsReadOnly();

                return new ReadOnlyCollection<ContextTypeBase>(contextTypes);
            }
        }
        #endregion Property: ContextTypes

        #region Property: Worldviews
        /// <summary>
        /// The worldviews of the entity.
        /// </summary>
        private Worldview[] worldviews = null;

        /// <summary>
        /// Gets the worldviews of the entity.
        /// </summary>
        public ReadOnlyCollection<Worldview> Worldviews
        {
            get
            {
                if (worldviews != null)
                    return new ReadOnlyCollection<Worldview>(worldviews);

                return new List<Worldview>(0).AsReadOnly();
            }
        }
        #endregion Property: Worldviews

        #region Property: LevelOfDetail
        /// <summary>
        /// The level of detail for the instance, possibly preventing some events to occur.
        /// </summary>
        private byte levelOfDetail = byte.MaxValue;

        /// <summary>
        /// Gets the level of detail for the instance, possibly preventing some events to occur.
        /// </summary>
        public byte LevelOfDetail
        {
            get
            {
                return levelOfDetail;
            }
            set
            {
                if (levelOfDetail != value)
                {
                    levelOfDetail = value;
                    NotifyPropertyChanged("LevelOfDetail");
                }
            }
        }
        #endregion Property: LevelOfDetail

        #region Property: World
        /// <summary>
        /// The semantic world this entity instance is in.
        /// </summary>
        private SemanticWorld world = null;

        /// <summary>
        /// Gets the semantic world this entity instance is in.
        /// </summary>
        public SemanticWorld World
        {
            get
            {
                return world;
            }
            internal set
            {
                world = value;
            }
        }
        #endregion Property: World

        #region Property: IsModified
        /// <summary>
        /// Indicates whether this entity instance is modified and needs to be updated by the engine.
        /// </summary>
        private bool isModified = false;

        /// <summary>
        /// Gets or sets the value that indicates whether this entity instance is modified and needs to be updated by the engine.
        /// </summary>
        internal bool IsModified
        {
            get
            {
                return isModified;
            }
            set
            {
                MarkAsModified(value, true);
            }
        }
        #endregion Property: IsModified

        #region Field: addedOrRemoved
        /// <summary>
        /// Indicates whether an addition or removal has been handled.
        /// </summary>
        internal bool addedOrRemoved = false;
        #endregion Field: addedOrRemoved

        #region Field: variableReferenceInstanceHolder
        /// <summary>
        /// The variable instance holder.
        /// </summary>
        private VariableReferenceInstanceHolder variableReferenceInstanceHolder = new VariableReferenceInstanceHolder();
        #endregion Field: variableReferenceInstanceHolder
        
        #endregion Events, Properties, and Fields

        #region Method Group: Constructors

        #region Constructor: EntityInstance(EntityBase entityBase)
        /// <summary>
        /// Creates a new entity instance from the given entity base.
        /// </summary>
        /// <param name="entityBase">The entity base to create the entity instance from.</param>
        protected EntityInstance(EntityBase entityBase)
            : base(entityBase)
        {
            if (entityBase != null)
            {
                // Create instances from the mandatory base abstract entities
                foreach (AbstractEntityValuedBase abstractEntityValuedBase in this.EntityBase.AbstractEntities)
                {
                    if ((InstanceManager.IgnoreNecessity || abstractEntityValuedBase.Necessity == Necessity.Mandatory) && abstractEntityValuedBase.Quantity != null)
                    {
                        for (int i = 0; i < abstractEntityValuedBase.Quantity.GetRandomInteger(this); i++)
                            AddAbstractEntity(InstanceManager.Current.Create<AbstractEntityInstance>(abstractEntityValuedBase.AbstractEntityBase));
                    }
                }
            }
        }
        #endregion Constructor: EntityInstance(EntityBase entityBase)

        #region Constructor: EntityInstance(EntityValuedBase entityValuedBase)
        /// <summary>
        /// Creates a new entity instance from the given valued entity base.
        /// </summary>
        /// <param name="entityValuedBase">The valued entity base to create the entity instance from.</param>
        protected EntityInstance(EntityValuedBase entityValuedBase)
            : base(entityValuedBase)
        {
            if (entityValuedBase != null)
            {
                // Create instances from the mandatory base abstract entities
                foreach (AbstractEntityValuedBase abstractEntityValuedBase in this.EntityBase.AbstractEntities)
                {
                    if ((InstanceManager.IgnoreNecessity || abstractEntityValuedBase.Necessity == Necessity.Mandatory) && abstractEntityValuedBase.Quantity != null)
                    {
                        for (int i = 0; i < abstractEntityValuedBase.Quantity.GetRandomInteger(this); i++)
                            AddAbstractEntity(InstanceManager.Current.Create<AbstractEntityInstance>(abstractEntityValuedBase.AbstractEntityBase));
                    }
                }
            }
        }
        #endregion Constructor: EntityInstance(EntityValuedBase entityValuedBase)

        #region Constructor: EntityInstance(EntityInstance entityInstance)
        /// <summary>
        /// Clones an entity instance.
        /// </summary>
        /// <param name="entityInstance">The entity instance to clone.</param>
        protected EntityInstance(EntityInstance entityInstance)
            : base(entityInstance)
        {
            if (entityInstance != null)
            {
                foreach (AttributeInstance attributeInstance in entityInstance.Attributes)
                    AddAttribute(new AttributeInstance(attributeInstance));
                foreach (StateGroupInstance stateGroupInstance in entityInstance.StateGroups)
                    AddStateGroup(new StateGroupInstance(stateGroupInstance));
                foreach (AbstractEntityInstance abstractEntityInstance in entityInstance.AbstractEntities)
                    AddAbstractEntity(new AbstractEntityInstance(abstractEntityInstance));
                foreach (Worldview worldview in entityInstance.Worldviews)
                    AddWorldview(new Worldview(worldview));
                this.LevelOfDetail = entityInstance.LevelOfDetail;
                this.variableReferenceInstanceHolder = new VariableReferenceInstanceHolder(entityInstance.variableReferenceInstanceHolder);
            }
        }
        #endregion Constructor: EntityInstance(EntityInstance entityInstance)

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
                attributeInstance.Owner = this;
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

        #region Method: AddStateGroup(StateGroupInstance stateGroupInstance)
        /// <summary>
        /// Adds a state group instance.
        /// </summary>
        /// <param name="stateGroupInstance">The state group instance to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        protected internal Message AddStateGroup(StateGroupInstance stateGroupInstance)
        {
            if (stateGroupInstance != null)
            {
                // If the state group is already available in all state groups, there is no use to add it
                if (HasStateGroup(stateGroupInstance.StateGroupBase))
                    return Message.RelationExistsAlready;

                // Add the state group instance
                Utils.AddToArray<StateGroupInstance>(ref this.stateGroups, stateGroupInstance);
                NotifyPropertyChanged("StateGroups");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddStateGroup(StateGroupInstance stateGroupInstance)

        #region Method: RemoveStateGroup(StateGroupInstance stateGroupInstance)
        /// <summary>
        /// Removes a state group instance.
        /// </summary>
        /// <param name="stateGroupInstance">The state group instance to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        protected internal Message RemoveStateGroup(StateGroupInstance stateGroupInstance)
        {
            if (stateGroupInstance != null)
            {
                if (HasStateGroup(stateGroupInstance.StateGroupBase))
                {
                    // Remove the state group instance
                    Utils.RemoveFromArray<StateGroupInstance>(ref this.stateGroups, stateGroupInstance);
                    NotifyPropertyChanged("StateGroups");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveStateGroup(StateGroupInstance stateGroupInstance)

        #region Method: AddActiveEvent(EventInstance eventInstance)
        /// <summary>
        /// Adds an active event.
        /// </summary>
        /// <param name="eventInstance">The event to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        internal Message AddActiveEvent(EventInstance eventInstance)
        {
            if (eventInstance != null)
            {
                // If the event is already available in all active events, there is no use to add it
                if (this.ActiveEvents.Contains(eventInstance))
                    return Message.RelationExistsAlready;

                // Add the event to the active events
                Utils.AddToArray<EventInstance>(ref this.activeEvents, eventInstance);
                NotifyPropertyChanged("ActiveEvents");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddActiveEvent(EventInstance eventInstance)

        #region Method: RemoveActiveEvent(EventInstance eventInstance)
        /// <summary>
        /// Removes an active event.
        /// </summary>
        /// <param name="eventInstance">The event to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        internal Message RemoveActiveEvent(EventInstance eventInstance)
        {
            if (eventInstance != null)
            {
                if (this.ActiveEvents.Contains(eventInstance))
                {
                    // Remove the event
                    Utils.RemoveFromArray<EventInstance>(ref this.activeEvents, eventInstance);
                    NotifyPropertyChanged("ActiveEvents");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveActiveEvent(EventInstance eventInstance)

        #region Method: AddContextType(ContextTypeBase contextTypeBase)
        /// <summary>
        /// Adds a context type.
        /// </summary>
        /// <param name="contextTypeBase">The context type to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddContextType(ContextTypeBase contextTypeBase)
        {
            if (contextTypeBase != null)
            {
                // If the context type is already available in all context types, there is no use to add it
                if (this.ContextTypes.Contains(contextTypeBase))
                    return Message.RelationExistsAlready;

                // Add the context type
                Utils.AddToArray<ContextTypeBase>(ref this.contextTypes, contextTypeBase);
                NotifyPropertyChanged("ContextTypes");

                // Throw an event
                if (ContextTypeAdded != null)
                    ContextTypeAdded(this, contextTypeBase);

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddContextType(ContextTypeBase contextTypeBase)

        #region Method: RemoveContextType(ContextTypeBase contextTypeBase)
        /// <summary>
        /// Removes a context type.
        /// </summary>
        /// <param name="contextTypeBase">The context type to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveContextType(ContextTypeBase contextTypeBase)
        {
            if (contextTypeBase != null)
            {
                if (this.ContextTypes.Contains(contextTypeBase))
                {
                    // Remove the context type
                    Utils.RemoveFromArray<ContextTypeBase>(ref this.contextTypes, contextTypeBase);
                    NotifyPropertyChanged("ContextTypes");

                    // Throw an event
                    if (ContextTypeRemoved != null)
                        ContextTypeRemoved(this, contextTypeBase);

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveContextType(ContextTypeBase contextTypeBase)

        #region Method: AddPredicate(PredicateInstance predicateInstance)
        /// <summary>
        /// Adds a predicate.
        /// </summary>
        /// <param name="predicateInstance">The predicate to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        internal Message AddPredicate(PredicateInstance predicateInstance)
        {
            if (predicateInstance != null)
            {
                if (this.Equals(predicateInstance.Actor))
                {
                    // If the predicate is already available in all predicates, there is no use to add it
                    if (this.PredicatesAsActor.Contains(predicateInstance))
                        return Message.RelationExistsAlready;

                    // Add the predicate
                    Utils.AddToArray<PredicateInstance>(ref this.predicatesAsActor, predicateInstance);
                    NotifyPropertyChanged("PredicatesAsActor");
                    return Message.RelationSuccess;
                }
                else if (this.Equals(predicateInstance.Target))
                {
                    // If the predicate is already available in all predicates, there is no use to add it
                    if (this.PredicatesAsTarget.Contains(predicateInstance))
                        return Message.RelationExistsAlready;

                    // Add the predicate
                    Utils.AddToArray<PredicateInstance>(ref this.predicatesAsTarget, predicateInstance);
                    NotifyPropertyChanged("PredicatesAsTarget");
                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: AddPredicate(PredicateInstance predicateInstance)

        #region Method: RemovePredicate(PredicateInstance predicateInstance)
        /// <summary>
        /// Removes a predicate.
        /// </summary>
        /// <param name="predicateInstance">The predicate to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        internal Message RemovePredicate(PredicateInstance predicateInstance)
        {
            if (predicateInstance != null)
            {
                // Remove the predicate
                if (this.Equals(predicateInstance.Actor))
                {
                    if (this.PredicatesAsActor.Contains(predicateInstance))
                    {
                        Utils.RemoveFromArray<PredicateInstance>(ref this.predicatesAsActor, predicateInstance);
                        NotifyPropertyChanged("PredicatesAsActor");
                        return Message.RelationSuccess;
                    }
                }
                else if (this.Equals(predicateInstance.Target))
                {
                    if (this.PredicatesAsTarget.Contains(predicateInstance))
                    {
                        Utils.RemoveFromArray<PredicateInstance>(ref this.predicatesAsTarget, predicateInstance);
                        NotifyPropertyChanged("PredicatesAsTarget");
                        return Message.RelationSuccess;
                    }
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemovePredicate(PredicateInstance predicateInstance)

        #region Method: AddAbstractEntity(AbstractEntityInstance abstractEntityInstance)
        /// <summary>
        /// Adds an abstract entity instance.
        /// </summary>
        /// <param name="abstractEntityInstance">The abstract entity instance to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        protected internal Message AddAbstractEntity(AbstractEntityInstance abstractEntityInstance)
        {
            if (abstractEntityInstance != null)
            {
                // If the abstract entity instance is already available in all abstract entities, there is no use to add it
                if (this.AbstractEntities.Contains(abstractEntityInstance))
                    return Message.RelationExistsAlready;

                // Add the abstract entity instance
                Utils.AddToArray<AbstractEntityInstance>(ref this.abstractEntities, abstractEntityInstance);
                NotifyPropertyChanged("AbstractEntities");

                // Set the world if this has not been done before
                if (abstractEntityInstance.World == null && this.World != null)
                    this.World.AddInstance(abstractEntityInstance);

                // Invoke an event
                if (AbstractEntityAdded != null)
                    AbstractEntityAdded.Invoke(this, abstractEntityInstance);

                // Notify the engine
                if (SemanticsEngine.Current != null)
                    SemanticsEngine.Current.HandleAbstractEntityAdded(this, abstractEntityInstance);

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddAbstractEntity(AbstractEntityInstance abstractEntityInstance)

        #region Method: RemoveAbstractEntity(AbstractEntityInstance abstractEntityInstance)
        /// <summary>
        /// Removes an abstract entity instance.
        /// </summary>
        /// <param name="abstractEntityInstance">The abstract entity instance to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        protected internal Message RemoveAbstractEntity(AbstractEntityInstance abstractEntityInstance)
        {
            if (abstractEntityInstance != null)
            {
                if (this.AbstractEntities.Contains(abstractEntityInstance))
                {
                    // Remove the abstract entity instance
                    Utils.RemoveFromArray<AbstractEntityInstance>(ref this.abstractEntities, abstractEntityInstance);
                    NotifyPropertyChanged("AbstractEntities");

                    // Invoke an event
                    if (AbstractEntityRemoved != null)
                        AbstractEntityRemoved.Invoke(this, abstractEntityInstance);

                    // Notify the engine
                    if (SemanticsEngine.Current != null)
                        SemanticsEngine.Current.HandleAbstractEntityRemoved(this, abstractEntityInstance);

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveAbstractEntity(AbstractEntityInstance abstractEntityInstance)

        #region Method: AddWorldview(WorldView worldview)
        /// <summary>
        /// Adds a worldview.
        /// </summary>
        /// <param name="worldview">The worldview to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddWorldview(Worldview worldview)
        {
            if (worldview != null)
            {
                // If the worldview is already available in all world views, there is no use to add it
                if (this.Worldviews.Contains(worldview))
                    return Message.RelationExistsAlready;

                // Add the worldview to the worldviews
                Utils.AddToArray<Worldview>(ref this.worldviews, worldview);
                NotifyPropertyChanged("Worldviews");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddWorldview(WorldView worldview)

        #region Method: RemoveWorldview(WorldView worldview)
        /// <summary>
        /// Removes a worldview.
        /// </summary>
        /// <param name="worldview">The worldview to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveWorldview(Worldview worldview)
        {
            if (worldview != null)
            {
                if (this.Worldviews.Contains(worldview))
                {
                    // Remove the worldview
                    Utils.RemoveFromArray<Worldview>(ref this.worldviews, worldview);
                    NotifyPropertyChanged("Worldviews");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveWorldview(WorldView worldview)

        #endregion Method Group: Add/Remove

        #region Method Group: Relationships

        #region Method: AddRelationship(RelationshipTypeBase relationshipType, EntityInstance source, EntityInstance target)
        /// <summary>
        /// Add a relationship of the given type between the given source and target.
        /// </summary>
        /// <param name="relationshipType">The relationship to create between source and target.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public static Message AddRelationship(RelationshipTypeBase relationshipType, EntityInstance source, EntityInstance target)
        {
            if (relationshipType != null && source != null && target != null)
            {
                foreach (RelationshipBase relationshipBase in relationshipType.Relationships)
                {
                    if (AddRelationship(relationshipBase, source, target) == Message.RelationSuccess)
                        return Message.RelationSuccess;
                }
            }
            return Message.RemovalFail;
        }
        #endregion Method: AddRelationship(RelationshipTypeBase relationshipType, EntityInstance source, EntityInstance target)

        #region Method: RemoveRelationship(RelationshipTypeBase relationshipType, EntityInstance source, EntityInstance target)
        /// <summary>
        /// Remove a relationship of the given type between the given source and target.
        /// </summary>
        /// <param name="relationshipType">The relationship to remove between source and target.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public static Message RemoveRelationship(RelationshipTypeBase relationshipType, EntityInstance source, EntityInstance target)
        {
            if (relationshipType != null && source != null && target != null)
            {
                // Find the relationship instance and remove it
                foreach (RelationshipInstance relationshipInstance in source.RelationshipsAsSource)
                {
                    if (relationshipInstance.RelationshipBase != null && !relationshipInstance.RelationshipBase.IsExclusive &&
                        relationshipType.Equals(relationshipInstance.RelationshipType) &&
                        source.Equals(relationshipInstance.Source) &&
                        target.Equals(relationshipInstance.Target))
                    {
                        source.RemoveRelationship(relationshipInstance);
                        target.RemoveRelationship(relationshipInstance);
                        return Message.RelationSuccess;
                    }
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveRelationship(RelationshipTypeBase relationshipType, EntityInstance source, EntityInstance target)

        #region Method: AddRelationship(RelationshipBase relationship, EntityInstance source, EntityInstance target)
        /// <summary>
        /// Add a relationship of the given base between the given source and target.
        /// </summary>
        /// <param name="relationship">The relationship to create between source and target.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public static Message AddRelationship(RelationshipBase relationship, EntityInstance source, EntityInstance target)
        {
            if (relationship != null && source != null && target != null)
            {
                // Check whether the relationship already exists
                foreach (RelationshipInstance relationshipInstance in source.RelationshipsAsSource)
                {
                    // Check whether the relationship and targets are the same
                    if (relationship.Equals(relationshipInstance.RelationshipBase) && target.Equals(relationshipInstance.Target))
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
                if (!source.IsNodeOf(relationship.Source))
                    return Message.RelationFail;

                // Check whether the target is valid
                bool targetOK = false;
                foreach (EntityBase possibleTarget in relationship.Targets)
                {
                    if (target.IsNodeOf(possibleTarget))
                    {
                        targetOK = true;
                        break;
                    }
                }

                if (targetOK)
                {
                    // Create a new relationship instance
                    RelationshipInstance relationshipInstance = new RelationshipInstance(relationship, source, target);

                    // Check whether the requirements have been satisfied
                    if ((relationship.SourceRequirements == null || relationship.SourceRequirements.IsSatisfied(source, relationshipInstance)) &&
                        (relationship.TargetsRequirements == null || relationship.TargetsRequirements.IsSatisfied(target, relationshipInstance)) &&
                        (relationship.SpatialRequirementBetweenSourceAndTargets == null || relationship.SpatialRequirementBetweenSourceAndTargets.IsSatisfied(source as PhysicalEntityInstance, target as PhysicalEntityInstance, relationshipInstance)))
                    {
                        // Add the relationship instance
                        source.AddRelationship(relationshipInstance);
                        target.AddRelationship(relationshipInstance);

                        return Message.RelationSuccess;
                    }
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: AddRelationship(RelationshipBase relationship, EntityInstance source, EntityInstance target)

        #region Method: RemoveRelationship(RelationshipBase relationship, EntityInstance source, EntityInstance target)
        /// <summary>
        /// Remove a relationship of the given base between the given source and target.
        /// </summary>
        /// <param name="relationship">The relationship to remove between source and target.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public static Message RemoveRelationship(RelationshipBase relationship, EntityInstance source, EntityInstance target)
        {
            if (relationship != null && source != null && target != null && !relationship.IsExclusive)
            {
                // Find the relationship instance and remove it
                foreach (RelationshipInstance relationshipInstance in source.RelationshipsAsSource)
                {
                    if (relationship.Equals(relationshipInstance.RelationshipBase) &&
                        source.Equals(relationshipInstance.Source) &&
                        target.Equals(relationshipInstance.Target))
                    {
                        source.RemoveRelationship(relationshipInstance);
                        target.RemoveRelationship(relationshipInstance);
                        return Message.RelationSuccess;
                    }
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveRelationship(RelationshipBase relationship, EntityInstance source, EntityInstance target)

        #region Method: AddRelationship(RelationshipInstance relationshipInstance)
        /// <summary>
        /// Adds the given relationship.
        /// </summary>
        /// <param name="relationshipInstance">The relationship to add.</param>
        /// <returns>The relationship to add.</returns>
        protected Message AddRelationship(RelationshipInstance relationshipInstance)
        {
            if (relationshipInstance != null)
            {
                if (this.Equals(relationshipInstance.Source))
                {
                    Utils.AddToArray<RelationshipInstance>(ref this.relationshipsAsSource, relationshipInstance);
                    NotifyPropertyChanged("RelationshipsAsSource");

                    // Notify the engine
                    SemanticsEngine.Current.HandleRelationshipAdded(this, relationshipInstance);

                    return Message.RelationSuccess;
                }
                else if (this.Equals(relationshipInstance.Target))
                {
                    Utils.AddToArray<RelationshipInstance>(ref this.relationshipsAsTarget, relationshipInstance);
                    NotifyPropertyChanged("RelationshipsAsTarget");

                    // Notify the engine
                    SemanticsEngine.Current.HandleRelationshipAdded(this, relationshipInstance);

                    return Message.RelationSuccess;
                }
            }
            return Message.RemovalFail;
        }
        #endregion Method: AddRelationship(RelationshipInstance relationshipInstance)

        #region Method: RemoveRelationship(RelationshipInstance relationshipInstance)
        /// <summary>
        /// Removes the given relationship.
        /// </summary>
        /// <param name="relationshipInstance">The relationship to remove.</param>
        /// <returns>The relationship to remove.</returns>
        protected Message RemoveRelationship(RelationshipInstance relationshipInstance)
        {
            if (relationshipInstance != null)
            {
                if (this.Equals(relationshipInstance.Source))
                {
                    Utils.RemoveFromArray<RelationshipInstance>(ref this.relationshipsAsSource, relationshipInstance);
                    NotifyPropertyChanged("RelationshipsAsSource");
                    return Message.RelationSuccess;
                }
                else if (this.Equals(relationshipInstance.Target))
                {
                    Utils.RemoveFromArray<RelationshipInstance>(ref this.relationshipsAsTarget, relationshipInstance);
                    NotifyPropertyChanged("RelationshipsAsTarget");
                    return Message.RelationSuccess;
                }
            }
            return Message.RemovalFail;
        }
        #endregion Method: RemoveRelationship(RelationshipInstance relationshipInstance)

        #region Method: AddRelationshipAsSource(RelationshipTypeBase relationshipType, EntityInstance target)
        /// <summary>
        /// Add a relationship with this entity instance as source, and the given instance as target.
        /// </summary>
        /// <param name="relationshipType">The relationship type.</param>
        /// <param name="target">The target.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddRelationshipAsSource(RelationshipTypeBase relationshipType, EntityInstance target)
        {
            return EntityInstance.AddRelationship(relationshipType, this, target);
        }
        #endregion Method: AddRelationshipAsSource(RelationshipTypeBase relationshipType, EntityInstance target)

        #region Method: RemoveRelationshipAsSource(RelationshipTypeBase relationshipType, EntityInstance target)
        /// <summary>
        /// Remove a relationship with this entity instance as source, and the given instance as target.
        /// </summary>
        /// <param name="relationshipType">The relationship type.</param>
        /// <param name="target">The target.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveRelationshipAsSource(RelationshipTypeBase relationshipType, EntityInstance target)
        {
            return EntityInstance.RemoveRelationship(relationshipType, this, target);
        }
        #endregion Method: RemoveRelationshipAsSource(RelationshipTypeBase relationshipType, EntityInstance target)

        #region Method: AddRelationshipAsTarget(RelationshipTypeBase relationshipType, EntityInstance source)
        /// <summary>
        /// Add a relationship with this entity instance as target, and the given instance as source.
        /// </summary>
        /// <param name="relationshipType">The relationship type.</param>
        /// <param name="source">The source.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddRelationshipAsTarget(RelationshipTypeBase relationshipType, EntityInstance source)
        {
            return EntityInstance.AddRelationship(relationshipType, source, this);
        }
        #endregion Method: AddRelationshipAsTarget(RelationshipTypeBase relationshipType, EntityInstance source)

        #region Method: RemoveRelationshipAsTarget(RelationshipTypeBase relationshipType, EntityInstance source)
        /// <summary>
        /// Remove a relationship with this entity instance as target, and the given instance as source.
        /// </summary>
        /// <param name="relationshipType">The relationship type.</param>
        /// <param name="source">The source.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveRelationshipAsTarget(RelationshipTypeBase relationshipType, EntityInstance source)
        {
            return EntityInstance.RemoveRelationship(relationshipType, source, this);
        }
        #endregion Method: RemoveRelationshipAsTarget(RelationshipTypeBase relationshipType, EntityInstance source)

        #region Method: AddRelationshipAsSource(RelationshipBase relationship, EntityInstance target)
        /// <summary>
        /// Add a relationship with this entity instance as source, and the given instance as target.
        /// </summary>
        /// <param name="relationship">The relationship.</param>
        /// <param name="target">The target.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddRelationshipAsSource(RelationshipBase relationship, EntityInstance target)
        {
            return EntityInstance.AddRelationship(relationship, this, target);
        }
        #endregion Method: AddRelationshipAsSource(RelationshipBase relationship, EntityInstance target)

        #region Method: RemoveRelationshipAsSource(RelationshipBase relationship, EntityInstance target)
        /// <summary>
        /// Remove a relationship with this entity instance as source, and the given instance as target.
        /// </summary>
        /// <param name="relationship">The relationship.</param>
        /// <param name="target">The target.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveRelationshipAsSource(RelationshipBase relationship, EntityInstance target)
        {
            return EntityInstance.RemoveRelationship(relationship, this, target);
        }
        #endregion Method: RemoveRelationshipAsSource(RelationshipBase relationship, EntityInstance target)

        #region Method: AddRelationshipAsTarget(RelationshipBase relationship, EntityInstance source)
        /// <summary>
        /// Add a relationship with this entity instance as target, and the given instance as source.
        /// </summary>
        /// <param name="relationship">The relationship.</param>
        /// <param name="source">The source.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddRelationshipAsTarget(RelationshipBase relationship, EntityInstance source)
        {
            return EntityInstance.AddRelationship(relationship, source, this);
        }
        #endregion Method: AddRelationshipAsTarget(RelationshipBase relationship, EntityInstance source)

        #region Method: RemoveRelationshipAsTarget(RelationshipBase relationship, EntityInstance source)
        /// <summary>
        /// Remove a relationship with this entity instance as target, and the given instance as source.
        /// </summary>
        /// <param name="relationship">The relationship.</param>
        /// <param name="source">The source.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveRelationshipAsTarget(RelationshipBase relationship, EntityInstance source)
        {
            return EntityInstance.RemoveRelationship(relationship, source, this);
        }
        #endregion Method: RemoveRelationshipAsTarget(RelationshipBase relationship, EntityInstance source)

        #region Method: AddRelationshipAsSource(string relationshipName, EntityInstance target)
        /// <summary>
        /// Add a relationship with this entity instance as source, and the given instance as target.
        /// </summary>
        /// <param name="relationshipName">The name of the relationship.</param>
        /// <param name="target">The target.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddRelationshipAsSource(string relationshipName, EntityInstance target)
        {
            if (this.EntityBase != null)
            {
                foreach (RelationshipBase relationship in this.EntityBase.RelationshipsAsSource)
                {
                    if (relationship.RelationshipType != null && relationship.RelationshipType.Names.Contains(relationshipName))
                        return EntityInstance.AddRelationship(relationship, this, target);
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: AddRelationshipAsSource(string relationshipName, EntityInstance target)

        #region Method: AddRelationshipAsTarget(string relationshipName, EntityInstance source)
        /// <summary>
        /// Add a relationship with this entity instance as target, and the given instance as source.
        /// </summary>
        /// <param name="relationshipName">The name of the relationship.</param>
        /// <param name="source">The source.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddRelationshipAsTarget(string relationshipName, EntityInstance source)
        {
            if (this.EntityBase != null)
            {
                foreach (RelationshipBase relationship in this.EntityBase.RelationshipsAsTarget)
                {
                    if (relationship.RelationshipType != null && relationship.RelationshipType.Names.Contains(relationshipName))
                        return EntityInstance.AddRelationship(relationship, source, this);
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: AddRelationshipAsTarget(string relationshipName, EntityInstance source)

        #endregion Method Group: Relationships

        #region Method Group: Other

        #region Method: HasAttribute(AttributeBase attributeBase)
        /// <summary>
        /// Checks if this entity instance has the given attribute.
        /// </summary>
        /// <param name="attributeBase">The attribute to check.</param>
        /// <returns>Returns true when the entity instance has the attribute.</returns>
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

        #region Method: HasStateGroup(StateGroupBase stateGroupBase)
        /// <summary>
        /// Checks if this entity has the given state group.
        /// </summary>
        /// <param name="stateGroupBase">The state group to check.</param>
        /// <returns>Returns true when the entity has the state group.</returns>
        public bool HasStateGroup(StateGroupBase stateGroupBase)
        {
            if (stateGroupBase != null)
            {
                foreach (StateGroupInstance stateGroupInstance in this.StateGroups)
                {
                    if (stateGroupBase.Equals(stateGroupInstance.StateGroupBase))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasStateGroup(StateGroupBase stateGroupBase)

        #region Method: HasAbstractEntity(AbstractEntityBase abstractEntityBase)
        /// <summary>
        /// Checks if this entity instance has the given abstract entity.
        /// </summary>
        /// <param name="abstractEntityBase">The abstract entity to check.</param>
        /// <returns>Returns true when the entity instance has the abstract entity.</returns>
        public bool HasAbstractEntity(AbstractEntityBase abstractEntityBase)
        {
            if (abstractEntityBase != null)
            {
                foreach (AbstractEntityInstance abstractEntityInstance in this.AbstractEntities)
                {
                    if (abstractEntityBase.Equals(abstractEntityInstance.AbstractEntityBase))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasAbstractEntity(AbstractEntityBase abstractEntityBase)

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the given condition satisfies the entity instance.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the entity instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the condition satisfies the entity instance.</returns>
        public override bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (conditionBase != null)
            {
                // Check whether the base satisfies the condition
                if (base.Satisfies(conditionBase, iVariableInstanceHolder))
                {
                    // Entity condition
                    EntityConditionBase entityConditionBase = conditionBase as EntityConditionBase;
                    if (entityConditionBase != null)
                    {
                        // Check whether all attributes are there
                        foreach (AttributeConditionBase attributeConditionBase in entityConditionBase.Attributes)
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

                        // Check whether the states are satisfied
                        foreach (StateGroupConditionBase stateGroupConditionBase in entityConditionBase.StateGroups)
                        {
                            bool satisfied = false;
                            foreach (StateGroupInstance stateGroupInstance in this.StateGroups)
                            {
                                if (stateGroupInstance.Satisfies(stateGroupConditionBase))
                                {
                                    satisfied = true;
                                    break;
                                }
                            }
                            if (!satisfied)
                                return false;
                        }

                        // Check whether the relationships are satisfied
                        foreach (RelationshipConditionBase relationshipConditionBase in entityConditionBase.Relationships)
                        {
                            bool satisfied = false;
                            foreach (RelationshipInstance relationshipInstance in this.RelationshipsAsSource)
                            {
                                if (relationshipInstance.Satisfies(relationshipConditionBase, iVariableInstanceHolder))
                                {
                                    satisfied = true;
                                    break;
                                }
                            }
                            foreach (RelationshipInstance relationshipInstance in this.RelationshipsAsTarget)
                            {
                                if (relationshipInstance.Satisfies(relationshipConditionBase, iVariableInstanceHolder))
                                {
                                    satisfied = true;
                                    break;
                                }
                            }
                            if (!satisfied)
                                return false;
                        }

                        // Check whether the instance can perform all the actions
                        ReadOnlyCollection<ActionBase> myActions = this.Actions;
                        foreach (ActionBase actionBase in entityConditionBase.Actions)
                        {
                            if (!myActions.Contains(actionBase))
                                return false;
                        }

                        return true;
                    }
                }
                else
                {
                    // Attribute condition
                    AttributeConditionBase attributeCondition = conditionBase as AttributeConditionBase;
                    if (attributeCondition != null)
                    {
                        foreach (AttributeInstance attributeInstance in this.Attributes)
                        {
                            if (attributeInstance.Satisfies(attributeCondition, iVariableInstanceHolder))
                                return true;
                        }
                        return false;
                    }

                    // State group condition
                    StateGroupConditionBase stateGroupCondition = conditionBase as StateGroupConditionBase;
                    if (stateGroupCondition != null)
                    {
                        foreach (StateGroupInstance stateGroupInstance in this.StateGroups)
                        {
                            if (stateGroupInstance.Satisfies(stateGroupCondition))
                                return true;
                        }
                        return false;
                    }

                    // Relationship conditions
                    RelationshipConditionBase relationshipCondition = conditionBase as RelationshipConditionBase;
                    if (relationshipCondition != null)
                    {
                        foreach (RelationshipInstance relationshipInstance in this.RelationshipsAsSource)
                        {
                            if (relationshipInstance.Satisfies(relationshipCondition, iVariableInstanceHolder))
                                return true;
                        }
                        foreach (RelationshipInstance relationshipInstance in this.RelationshipsAsTarget)
                        {
                            if (relationshipInstance.Satisfies(relationshipCondition, iVariableInstanceHolder))
                                return true;
                        }
                        return false;
                    }
                }
            }
            return false;
        }
        #endregion Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the given change to the entity instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the entity instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        protected internal override bool Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (changeBase != null)
            {
                if (base.Apply(changeBase, iVariableInstanceHolder))
                {
                    // Entity change
                    EntityChangeBase entityChangeBase = changeBase as EntityChangeBase;
                    if (entityChangeBase != null)
                    {
                        // Change the attributes
                        foreach (AttributeChangeBase attributeChangeBase in entityChangeBase.Attributes)
                        {
                            foreach (AttributeInstance attributeInstance in this.Attributes)
                                attributeInstance.Apply(attributeChangeBase, iVariableInstanceHolder);
                        }

                        // Add and remove the attributes
                        foreach (AttributeValuedBase attributeValuedBase in entityChangeBase.AttributesToAdd)
                            AddAttribute(InstanceManager.Current.Create<AttributeInstance>(attributeValuedBase));
                        foreach (AttributeConditionBase attributeConditionBase in entityChangeBase.AttributesToRemove)
                        {
                            foreach (AttributeInstance attributeInstance in this.Attributes)
                            {
                                if (attributeInstance.Satisfies(attributeConditionBase, iVariableInstanceHolder))
                                {
                                    RemoveAttribute(attributeInstance);
                                    break;
                                }
                            }
                        }

                        // Change the state groups
                        foreach (StateGroupChangeBase stateGroupChangeBase in entityChangeBase.StateGroups)
                        {
                            foreach (StateGroupInstance stateGroupInstance in this.StateGroups)
                                stateGroupInstance.Apply(stateGroupChangeBase);
                        }

                        // Change the relationships
                        foreach (RelationshipChangeBase relationshipChangeBase in entityChangeBase.Relationships)
                        {
                            if (relationshipChangeBase != null)
                            {
                                foreach (RelationshipInstance relationshipInstance in this.RelationshipsAsSource)
                                {
                                    if (relationshipInstance.Apply(relationshipChangeBase, iVariableInstanceHolder))
                                        return true;
                                }
                                foreach (RelationshipInstance relationshipInstance in this.RelationshipsAsTarget)
                                {
                                    if (relationshipInstance.Apply(relationshipChangeBase, iVariableInstanceHolder))
                                        return true;
                                }
                                return false;
                            }
                        }

                        return true;
                    }
                }
                else
                {
                    // Attribute change
                    AttributeChangeBase attributeChange = changeBase as AttributeChangeBase;
                    if (attributeChange != null)
                    {
                        foreach (AttributeInstance attributeInstance in this.Attributes)
                            attributeInstance.Apply(attributeChange, iVariableInstanceHolder);
                        return true;
                    }

                    // State group change
                    StateGroupChangeBase stateGroupChangeBase = changeBase as StateGroupChangeBase;
                    if (stateGroupChangeBase != null)
                    {
                        foreach (StateGroupInstance stateGroupInstance in this.StateGroups)
                            stateGroupInstance.Apply(stateGroupChangeBase);
                        return true;
                    }

                    // Relationship change
                    RelationshipChangeBase relationshipChange = changeBase as RelationshipChangeBase;
                    if (relationshipChange != null)
                    {
                        foreach (RelationshipInstance relationshipInstance in this.RelationshipsAsSource)
                        {
                            if (relationshipInstance.Apply(relationshipChange, iVariableInstanceHolder))
                                return true;
                        }
                        foreach (RelationshipInstance relationshipInstance in this.RelationshipsAsTarget)
                        {
                            if (relationshipInstance.Apply(relationshipChange, iVariableInstanceHolder))
                                return true;
                        }
                        return false;
                    }
                }
            }
            return false;
        }
        #endregion Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: MarkAsModified(bool modified, bool spread)
        /// <summary>
        /// Indicate whether the instance has been modified.
        /// </summary>
        /// <param name="modified">The value that indicates whether the instance has been modified.</param>
        /// <param name="spread">The value that indicates whether the marking should spread further.</param>
        internal virtual void MarkAsModified(bool modified, bool spread)
        {
            if (modified != this.isModified)
                this.isModified = modified;
        }
        #endregion Method: MarkAsModified(bool modified, bool spread)

        #region Method: GetAction(string name)
        /// <summary>
        /// Get the action with the given name.
        /// </summary>
        /// <param name="name">The name of the action to get.</param>
        /// <returns>The action with the given name.</returns>
        public ActionBase GetAction(string name)
        {
            if (name != null)
            {
                foreach (ActionBase actionBase in this.Actions)
                {
                    if (actionBase.HasName(name))
                        return actionBase;
                }
            }
            return null;
        }
        #endregion Method: GetAction(string name)

        #region Method: GetAttribute(string name)
        /// <summary>
        /// Get the attribute with the given name.
        /// </summary>
        /// <param name="name">The name of the attribute to get.</param>
        /// <returns>The attribute with the given name.</returns>
        public AttributeInstance GetAttribute(string name)
        {
            if (name != null)
            {
                foreach (AttributeInstance attributeInstance in this.Attributes)
                {
                    if (attributeInstance.HasName(name))
                        return attributeInstance;
                }
            }
            return null;
        }
        #endregion Method: GetAttribute(string name)

        #region Method: GetValueOfAttribute(string attribute)
        /// <summary>
        /// Return the value of the attribute with the given name.
        /// </summary>
        /// <param name="attribute">The name of the attribute.</param>
        /// <returns>The value of the attribute with the given name.</returns>
        public ValueInstance GetValueOfAttribute(string attribute)
        {
            if (attribute != null)
            {
                foreach (AttributeInstance attributeInstance in this.Attributes)
                {
                    if (attributeInstance.HasName(attribute))
                        return attributeInstance.Value;
                }
            }

            return null;
        }
        #endregion Method: GetValueOfAttribute(string attribute)

        #region Method: GetValueOfAttribute(AttributeBase attributeBase)
        /// <summary>
        /// Return the value of the attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <returns>The value of the attribute, or null when it does not exist.</returns>
        public ValueInstance GetValueOfAttribute(AttributeBase attributeBase)
        {
            if (attributeBase != null)
            {
                foreach (AttributeInstance attributeInstance in this.Attributes)
                {
                    if (attributeBase.Equals(attributeInstance.AttributeBase))
                        return attributeInstance.Value;
                }
            }

            return null;
        }
        #endregion Method: GetValueOfAttribute(AttributeBase attributeBase)

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

        #region Method: GetVariable(string name)
        /// <summary>
        /// Get the variable instance of the variable with the given name.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        /// <returns>The variable instance of the variable with the name.</returns>
        public VariableInstance GetVariable(string name)
        {
            if (name != null && this.EntityBase != null)
            {
                foreach (VariableBase variableBase in this.EntityBase.Variables)
                {
                    if (name.Equals(variableBase.Name))
                        return GetVariable(variableBase);
                }
            }
            return null;
        }
        #endregion Method: GetVariable(string name)

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
            return this;
        }
        #endregion Method: GetActor()

        #region Method: GetTarget()
        /// <summary>
        /// Get the target.
        /// </summary>
        /// <returns>The target.</returns>
        public EntityInstance GetTarget()
        {
            return this;
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

        #region Method: InvokeStartedAction(EventInstance eventInstance)
        /// <summary>
        /// Invoke an event that the entity instance has started the given event instance.
        /// </summary>
        /// <param name="eventInstance">The started event instance.</param>
        internal void InvokeStartedAction(EventInstance eventInstance)
        {
            if (StartedAction != null)
                StartedAction(eventInstance.Actor, eventInstance.Action, eventInstance.Target);
        }
        #endregion Method: InvokeStartedAction(EventInstance eventInstance)

        #region Method: InvokeExecutedAction(EventInstance eventInstance)
        /// <summary>
        /// Invoke an event that the entity instance has executed the given event instance.
        /// </summary>
        /// <param name="eventInstance">The executed event instance.</param>
        internal void InvokeExecutedAction(EventInstance eventInstance)
        {
            if (ExecutedAction != null)
                ExecutedAction(eventInstance.Actor, eventInstance.Action, eventInstance.Target);
        }
        #endregion Method: InvokeExecutedAction(EventInstance eventInstance)

        #region Method: InvokeStoppedAction(EventInstance eventInstance)
        /// <summary>
        /// Invoke an event that the entity instance has stopped the given event instance.
        /// </summary>
        /// <param name="eventInstance">The stopped event instance.</param>
        internal void InvokeStoppedAction(EventInstance eventInstance)
        {
            if (StoppedAction != null)
                StoppedAction(eventInstance.Actor, eventInstance.Action, eventInstance.Target);
        }
        #endregion Method: InvokeStoppedAction(EventInstance eventInstance)

        #region Method: InvokeAffectedByAction(EventInstance eventInstance)
        /// <summary>
        /// Invoke an event that the given event instance has affected this entity instance.
        /// </summary>
        /// <param name="eventInstance">The executed event instance.</param>
        internal void InvokeAffectedByAction(EventInstance eventInstance)
        {
            if (AffectedByAction != null)
                AffectedByAction(eventInstance.Actor, eventInstance.Action, eventInstance.Target);
        }
        #endregion Method: InvokeAffectedByAction(EventInstance eventInstance)

        #region Method: InvokeFilterApplication(FilterTypeBase filterTypeBase)
        /// <summary>
        /// Invoke an event that the given filter has been applied to this entity instance.
        /// </summary>
        /// <param name="filterTypeBase">The applied filter.</param>
        internal void InvokeFilterApplication(FilterTypeBase filterTypeBase)
        {
            if (FilterApplied != null)
                FilterApplied(this, filterTypeBase);
        }
        #endregion Method: InvokeFilterApplication(FilterTypeBase filterTypeBase)

        #region Method: BelongsTo(GroupBase groupBase)
        /// <summary>
        /// Checks whether this entity instance belongs to the given group.
        /// </summary>
        /// <param name="groupBase">The group to check.</param>
        /// <returns>Returns whether this entity instance belongs to the group.</returns>
        public bool BelongsTo(GroupBase groupBase)
        {
            if (groupBase != null)
                return groupBase.BelongsToGroup(this);
            return false;
        }
        #endregion Method: BelongsTo(GroupBase groupBase)

        #region Method: BelongsTo(FamilyBase familyBase)
        /// <summary>
        /// Checks whether this entity instance belongs to the given family.
        /// </summary>
        /// <param name="familyBase">The family to check.</param>
        /// <returns>Returns whether this entity instance belongs to the family.</returns>
        public bool BelongsTo(FamilyBase familyBase)
        {
            if (familyBase != null)
                return familyBase.BelongsToFamily(this);
            return false;
        }
        #endregion Method: BelongsTo(FamilyBase familyBase)

        #endregion Method Group: Other

    }
    #endregion Class: EntityInstance

}