/**************************************************************************
 * 
 * SemanticsEngine.cs
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
using Common;
using Semantics.Components;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Components;
using SemanticsEngine.Entities;
using SemanticsEngine.Interfaces;
using SemanticsEngine.Tools;
using SemanticsEngine.Worlds;
using ValueType = Semantics.Utilities.ValueType;

namespace SemanticsEngine
{

    #region Class: SemanticsEngine
    /// <summary>
    /// The Semantics Engine handles all events of entity instances.
    /// </summary>
    public class SemanticsEngine : PropertyChangedComponent
    {

        #region Events, Properties, and Fields

        #region Event: EntityInstanceHandler
        /// <summary>
        /// A handler for the entity instance events.
        /// </summary>
        /// <param name="sender">The entity instance.</param>
        public delegate void EntityInstanceHandler(EntityInstance sender);

        /// <summary>
        /// An event for an added entity instance.
        /// </summary>
        public event EntityInstanceHandler EntityInstanceAdded;

        /// <summary>
        /// An event for a removed entity instance.
        /// </summary>
        public event EntityInstanceHandler EntityInstanceRemoved;
        #endregion Event: EntityInstanceHandler

        #region Property: Current
        /// <summary>
        /// The current semantics engine.
        /// </summary>
        private static SemanticsEngine current = null;

        /// <summary>
        /// Gets the current semantics engine.
        /// </summary>
        public static SemanticsEngine Current
        {
            get
            {
                return current;
            }
            protected set
            {
                current = value;
            }
        }
        #endregion Property: Current

        #region Property: Worlds
        /// <summary>
        /// All the semantic worlds that are updated by the engine.
        /// </summary>
        private List<SemanticWorld> worlds = new List<SemanticWorld>();

        /// <summary>
        /// Gets all the semantic worlds that are updated by the engine.
        /// </summary>
        public ReadOnlyCollection<SemanticWorld> Worlds
        {
            get
            {
                return worlds.AsReadOnly();
            }
        }
        #endregion Property: Worlds

        #region Property: IsUpdating
        /// <summary>
        /// Indicates whether the engine is updating.
        /// </summary>
        private bool isUpdating = false;

        /// <summary>
        /// Gets the value that indicates whether the engine is updating.
        /// </summary>
        public bool IsUpdating
        {
            get
            {
                return isUpdating;
            }
        }
        #endregion Property: IsUpdating

        #region Property: LastUpdateTime
        /// <summary>
        /// The last used elapsed time for an update.
        /// </summary>
        private float lastUpdateTime = 0;

        /// <summary>
        /// Gets the last used elapsed time for an update.
        /// </summary>
        public float LastUpdateTime
        {
            get
            {
                return lastUpdateTime;
            }
            private set
            {
                lastUpdateTime = value;
                NotifyPropertyChanged("LastUpdateTime");
            }
        }
        #endregion Property: LastUpdateTime

        #region Property: TotalElapsedTime
        /// <summary>
        /// The total elapsed time of the engine.
        /// </summary>
        private float totalElapsedTime = 0;

        /// <summary>
        /// Gets the total elapsed time of the engine.
        /// </summary>
        public float TotalElapsedTime
        {
            get
            {
                return totalElapsedTime;
            }
            private set
            {
                totalElapsedTime = value;
                NotifyPropertyChanged("TotalElapsedTime");
            }
        }
        #endregion Property: TotalElapsedTime

        #region Property: NrOfUpdates
        /// <summary>
        /// The number of updates on the engine.
        /// </summary>
        private int nrOfUpdates = 0;

        /// <summary>
        /// Gets the number of updates on the engine.
        /// </summary>
        public int NrOfUpdates
        {
            get
            {
                return nrOfUpdates;
            }
            private set
            {
                nrOfUpdates = value;
                NotifyPropertyChanged("NrOfUpdates");
            }
        }
        #endregion Property: NrOfUpdates
        
        #region Field: delayedEvents
        /// <summary>
        /// A list with all delayed events.
        /// </summary>
        private List<EventInstance> delayedEvents = new List<EventInstance>();
        #endregion Field: delayedEvents

        #region Field: continuousEvents
        /// <summary>
        /// A list with all continuous events.
        /// </summary>
        private List<EventInstance> continuousEvents = new List<EventInstance>();
        #endregion Field: continuousEvents

        #region Field: discreteEvents
        /// <summary>
        /// A list with all discrete events.
        /// </summary>
        private List<EventInstance> discreteEvents = new List<EventInstance>();
        #endregion Field: discreteEvents

        #region Field: delayedEffects
        /// <summary>
        /// A list with all delayed effects.
        /// </summary>
        private List<EffectInstance> delayedEffects = new List<EffectInstance>();
        #endregion Field: delayedEffects

        #region Field: continuousEffects
        /// <summary>
        /// A list with all continuous effects.
        /// </summary>
        private List<EffectInstance> continuousEffects = new List<EffectInstance>();
        #endregion Field: continuousEffects

        #region Field: discreteEffects
        /// <summary>
        /// A list with all discrete effects.
        /// </summary>
        private List<EffectInstance> discreteEffects = new List<EffectInstance>();
        #endregion Field: discreteEffects

        #region Field: eventsToStop
        /// <summary>
        /// All the events that should be stopped.
        /// </summary>
        private List<EventInstance> eventsToStop = new List<EventInstance>();
        #endregion Field: eventsToStop

        #region Field: effectsToStop
        /// <summary>
        /// All the effects that should be stopped.
        /// </summary>
        private List<EffectInstance> effectsToStop = new List<EffectInstance>();
        #endregion Field: effectsToStop

        #region Field: affectedInstances
        /// <summary>
        /// A dictionary that keeps track of all affected instances of one update loop.
        /// </summary>
        private List<EntityInstance> affectedInstances = new List<EntityInstance>();
        #endregion Field: affectedInstances

        #region Field group: additions/removals

        private Dictionary<EntityInstance, List<AbstractEntityInstance>> addedAbstractEntities = new Dictionary<EntityInstance, List<AbstractEntityInstance>>();
        private Dictionary<EntityInstance, List<AbstractEntityInstance>> removedAbstractEntities = new Dictionary<EntityInstance, List<AbstractEntityInstance>>();
        private Dictionary<TangibleObjectInstance, List<TangibleObjectInstance>> addedConnectionItems = new Dictionary<TangibleObjectInstance, List<TangibleObjectInstance>>();
        private Dictionary<TangibleObjectInstance, List<TangibleObjectInstance>> removedConnectionItems = new Dictionary<TangibleObjectInstance, List<TangibleObjectInstance>>();
        private Dictionary<TangibleObjectInstance, List<TangibleObjectInstance>> addedCovers = new Dictionary<TangibleObjectInstance, List<TangibleObjectInstance>>();
        private Dictionary<TangibleObjectInstance, List<TangibleObjectInstance>> removedCovers = new Dictionary<TangibleObjectInstance, List<TangibleObjectInstance>>();
        private Dictionary<MatterInstance, List<ElementInstance>> addedElements = new Dictionary<MatterInstance, List<ElementInstance>>();
        private Dictionary<MatterInstance, List<ElementInstance>> removedElements = new Dictionary<MatterInstance, List<ElementInstance>>();
        private Dictionary<SpaceInstance, List<TangibleObjectInstance>> addedItems = new Dictionary<SpaceInstance, List<TangibleObjectInstance>>();
        private Dictionary<SpaceInstance, List<TangibleObjectInstance>> removedItems = new Dictionary<SpaceInstance, List<TangibleObjectInstance>>();
        private Dictionary<SpaceInstance, List<MatterInstance>> addedTangibleMatter = new Dictionary<SpaceInstance, List<MatterInstance>>();
        private Dictionary<SpaceInstance, List<MatterInstance>> removedTangibleMatter = new Dictionary<SpaceInstance, List<MatterInstance>>();
        private Dictionary<TangibleObjectInstance, List<MatterInstance>> addedLayers = new Dictionary<TangibleObjectInstance, List<MatterInstance>>();
        private Dictionary<TangibleObjectInstance, List<MatterInstance>> removedLayers = new Dictionary<TangibleObjectInstance, List<MatterInstance>>();
        private Dictionary<TangibleObjectInstance, List<MatterInstance>> addedMatter = new Dictionary<TangibleObjectInstance, List<MatterInstance>>();
        private Dictionary<TangibleObjectInstance, List<MatterInstance>> removedMatter = new Dictionary<TangibleObjectInstance, List<MatterInstance>>();
        private Dictionary<TangibleObjectInstance, List<TangibleObjectInstance>> addedParts = new Dictionary<TangibleObjectInstance, List<TangibleObjectInstance>>();
        private Dictionary<TangibleObjectInstance, List<TangibleObjectInstance>> removedParts = new Dictionary<TangibleObjectInstance, List<TangibleObjectInstance>>();
        private Dictionary<EntityInstance, List<RelationshipInstance>> addedRelationships = new Dictionary<EntityInstance, List<RelationshipInstance>>();
        private Dictionary<EntityInstance, List<RelationshipInstance>> removedRelationships = new Dictionary<EntityInstance, List<RelationshipInstance>>();
        private Dictionary<PhysicalObjectInstance, List<SpaceInstance>> addedSpaces = new Dictionary<PhysicalObjectInstance, List<SpaceInstance>>();
        private Dictionary<PhysicalObjectInstance, List<SpaceInstance>> removedSpaces = new Dictionary<PhysicalObjectInstance, List<SpaceInstance>>();
        private Dictionary<CompoundInstance, List<SubstanceInstance>> addedSubstances = new Dictionary<CompoundInstance, List<SubstanceInstance>>();
        private Dictionary<CompoundInstance, List<SubstanceInstance>> removedSubstances = new Dictionary<CompoundInstance, List<SubstanceInstance>>();
        private Dictionary<MixtureInstance, List<SubstanceInstance>> addedSubstances2 = new Dictionary<MixtureInstance, List<SubstanceInstance>>();
        private Dictionary<MixtureInstance, List<SubstanceInstance>> removedSubstances2 = new Dictionary<MixtureInstance, List<SubstanceInstance>>();

        #endregion Field group: additions/removals

        #region Field: randomVariableResults
        /// <summary>
        /// A dictionary containing the results of random variables per event instance for one update.
        /// </summary>
        private static Dictionary<EventInstance, Dictionary<RandomVariableBase, bool>> randomVariableResults = new Dictionary<EventInstance, Dictionary<RandomVariableBase, bool>>();
        #endregion Field: randomVariableResults

        #region Field: positionHandler
        /// <summary>
        /// A handler for changed positions of physical entity instances.
        /// </summary>
        private PhysicalEntityInstance.PositionHandler positionHandler = null;
        #endregion Field: positionHandler

        #region Field: reverseStopBehavior
        /// <summary>
        /// Indicates whether the stop behavior should be reversed; this is a hack, as something goes wrong with reactions.
        /// </summary>
        public bool reverseStopBehavior = false;
        #endregion Field: reverseStopBehavior
        
        #region Property: InputHandler
        /// <summary>
        /// The input handler.
        /// </summary>
        private static IInputHandler inputHandler = null;

        /// <summary>
        /// Gets or set the input handler.
        /// </summary>
        public static IInputHandler InputHandler
        {
            get
            {
                return inputHandler;
            }
            set
            {
                inputHandler = value;
            }
        }
        #endregion Property: InputHandler

        #endregion Events, Properties, and Fields
        
        #region Method Group: Constructors

        #region Constructor: SemanticsEngine()
        /// <summary>
        /// Creates a new semantics engine.
        /// </summary>
        protected SemanticsEngine()
        {
            this.positionHandler = new PhysicalEntityInstance.PositionHandler(physicalEntityInstance_PositionChanged);
        }
        #endregion Constructor: SemanticsEngine()

        #region Method: Initialize()
        /// <summary>
        /// Initialize the semantics engine.
        /// </summary>
        public static void Initialize()
        {
            if (Current == null)
            {
                // Make this semantics engine the current semantics engine
                Current = new SemanticsEngine();
            }
        }
        #endregion Method: Initialize()

        #region Method: Uninitialize()
        /// <summary>
        /// Uninitialize the semantics engine.
        /// </summary>
        public static void Uninitialize()
        {
            Current = null;

            BaseManager.Current.Clear();

            Utils.Clear();
        }
        #endregion Method: Uninitialize()

        #endregion Method Group: Constructors

        #region Method Group: Worlds and instances

        #region Method: AddWorld(SemanticWorld world)
        /// <summary>
        /// Add a new world to the Semantics Engine.
        /// </summary>
        /// <param name="world">The world to add to the Semantics Engine.</param>
        /// <returns>Returns whether the world has been added successfully.</returns>
        public virtual bool AddWorld(SemanticWorld world)
        {
            if (world != null)
            {
                if (!this.worlds.Contains(world))
                {
                    // Add the world
                    this.worlds.Add(world);

                    // Handle all added entity instances
                    foreach (EntityInstance entityInstance in world.Instances)
                        HandleAddedEntityInstance(entityInstance);

                    return true;
                }
            }
            return false;
        }
        #endregion Method: AddWorld(SemanticWorld world)

        #region Method: RemoveWorld(SemanticWorld world)
        /// <summary>
        /// Removes a world from the Semantics Engine.
        /// </summary>
        /// <param name="instance">The world to remove from the Semantics Engine.</param>
        /// <returns>Returns whether the world has been removed successfully.</returns>
        public virtual bool RemoveWorld(SemanticWorld world)
        {
            if (world != null)
            {
                if (this.worlds.Contains(world))
                {
                    // Remove the world
                    this.worlds.Remove(world);

                    // Handle all removed entity instances
                    foreach (EntityInstance entityInstance in world.Instances)
                        HandleRemovedEntityInstance(entityInstance);

                    return true;
                }
            }
            return false;
        }
        #endregion Method: RemoveWorld(SemanticWorld world)

        #region Method: HandleAddedEntityInstance(EntityInstance entityInstance)
        /// <summary>
        /// Handle the addition of the given entity instance.
        /// </summary>
        /// <param name="entityInstance">The added entity instance.</param>
        internal void HandleAddedEntityInstance(EntityInstance entityInstance)
        {
            if (entityInstance != null)
            {
                // Check whether we should keep track of collision requirements of this instance
                PhysicalEntityInstance physicalEntityInstance = entityInstance as PhysicalEntityInstance;
                if (physicalEntityInstance != null)
                {
                    foreach (EventBase automaticEvent in physicalEntityInstance.AutomaticEvents)
                    {
                        if (automaticEvent.Requirements.ActorAndTargetCollide != null)
                        {
                            physicalEntityInstance.PositionChanged += this.positionHandler;
                            break;
                        }
                    }
                }

                // Make a notification
                if (EntityInstanceAdded != null)
                    EntityInstanceAdded(entityInstance);

                // Set the context types and predicates
                UpdateContextTypes(entityInstance);
                UpdatePredicates(entityInstance);
            }
        }
        #endregion Method: HandleAddedEntityInstance(EntityInstance entityInstance)

        #region Method: HandleRemovedEntityInstance(EntityInstance entityInstance)
        /// <summary>
        /// Handle the addition of the given entity instance.
        /// </summary>
        /// <param name="entityInstance">The added entity instance.</param>
        internal void HandleRemovedEntityInstance(EntityInstance entityInstance)
        {
            if (entityInstance != null)
            {
                // Remove the possibly added position handler
                PhysicalEntityInstance physicalEntityInstance = entityInstance as PhysicalEntityInstance;
                if (physicalEntityInstance != null)
                    physicalEntityInstance.PositionChanged -= this.positionHandler;

                // Make sure that all events of the entity instance are stopped
                foreach (EventInstance eventInstance in entityInstance.ActiveEvents)
                    this.eventsToStop.Add(eventInstance);

                // Make a notification
                if (EntityInstanceRemoved != null)
                    EntityInstanceRemoved(entityInstance);
            }
        }
        #endregion Method: HandleRemovedEntityInstance(EntityInstance entityInstance)

        #endregion Method Group: Worlds and instances

        #region Method Group: Update

        #region Method: Update(float elapsedTime)
        /// <summary>
        /// Update the Semantics Engine.
        /// </summary>
        /// <param name="elapsedTime">The elapsed time in the base unit of the special time unit category.</param>
        public void Update(float elapsedTime)
        {
            // Set the last update time
            this.lastUpdateTime = elapsedTime;

            this.isUpdating = true;

            // Update all modified instances in the semantic worlds
            foreach (SemanticWorld world in this.Worlds)
            {
                List<EntityInstance> modifiedInstances = new List<EntityInstance>(world.ModifiedInstances);
                foreach (EntityInstance instance in modifiedInstances)
                    UpdateInstance(instance, elapsedTime);
            }

            // Clear the additions and removals of the previous update
            ClearAdditionsAndRemovals();

			// Stop all events and effects that have to be stopped
            if (!this.reverseStopBehavior)
                StopEventsAndEffects();

            // Update all events
            UpdateEvents(elapsedTime);

            // Stop all events and effects that have to be stopped
            if (!this.reverseStopBehavior)
                StopEventsAndEffects();
            
            // Update all effects
            UpdateEffects(elapsedTime);

            // Stop all events and effects that have to be stopped
            if (this.reverseStopBehavior)
                StopEventsAndEffects();

            // Update the context types and predicates of affected instances
            foreach (EntityInstance entityInstance in this.affectedInstances)
            {
                UpdateContextTypes(entityInstance);
                UpdatePredicates(entityInstance);
            }
            this.affectedInstances.Clear();

            // Clear the random variable results
            randomVariableResults.Clear();

            // Increase properties
            this.TotalElapsedTime += elapsedTime;
            this.NrOfUpdates++;

            this.isUpdating = false;
        }
        #endregion Method: Update(float elapsedTime)

        #region Method: UpdateInstance(EntityInstance entityInstance, float elapsedTime)
        /// <summary>
        /// Update the entity instance.
        /// </summary>
        /// <param name="entityInstance">The entity instance to update.</param>
        /// <param name="elapsedTime">The elapsed time in the base unit of the special time unit category.</param>
        private void UpdateInstance(EntityInstance entityInstance, float elapsedTime)
        {
            if (entityInstance != null)
            {
                // Check whether there's any event that has been triggered
                foreach (EventBase eventBase in entityInstance.AutomaticEvents)
                    HandleEvent(entityInstance, eventBase, null, null, null);

                // The instance does not have to be updated anymore
                entityInstance.IsModified = false;
            }
        }
        #endregion Method: UpdateInstance(EntityInstance entityInstance, float elapsedTime)

        #region Method: UpdateEvents(float elapsedTime)
        /// <summary>
        /// Update all the events.
        /// </summary>
        /// <param name="elapsedTime">The elapsed time in the base unit of the special time unit category.</param>
        private void UpdateEvents(float elapsedTime)
        {
            #region Delayed events

            // Start all delayed events
            List<EventInstance> delayedEventsToRemove = new List<EventInstance>();
            foreach (EventInstance delayedEvent in this.delayedEvents)
            {
                // Reduce the delay time with the elapsed time
                delayedEvent.remainingDelay -= elapsedTime;

                // If the delay is over, start it, and make sure it is removed from the list
                if (delayedEvent.remainingDelay <= 0)
                {
                    StartEvent(delayedEvent);
                    delayedEventsToRemove.Add(delayedEvent);
                }
            }
            foreach (EventInstance delayedEvent in delayedEventsToRemove)
                this.delayedEvents.Remove(delayedEvent);

            #endregion Delayed events

            #region Continuous events

            // Execute all continuous events
            foreach (EventInstance continuousEvent in this.continuousEvents)
            {
                bool remove = false;

                // Check whether there is still time left
                bool durationInfinite = continuousEvent.remainingDuration == float.PositiveInfinity;
                if (!durationInfinite)
                    continuousEvent.remainingDuration -= elapsedTime;
                if (continuousEvent.remainingDuration > 0)
                {
                    // Check whether the remaining time is also dependent on the satisfaction of the requirements
                    bool cont = true;
                    if (continuousEvent.durationDependentOnRequirements && continuousEvent.Requirements != null)
                        cont = continuousEvent.Requirements.IsSatisfied(continuousEvent);

                    if (cont)
                    {
                        // Reduce the remaining interval time
                        continuousEvent.remainingInterval -= elapsedTime;

                        // If the interval is over, execute the event the correct number of times, and reset the remaining interval
                        while (continuousEvent.remainingInterval <= 0)
                        {
                            ExecuteEvent(continuousEvent);
                            continuousEvent.remainingInterval += continuousEvent.totalInterval;
                        }
                    }
                    else
                        remove = true;
                }
                else
                    remove = true;

                // Indicate that it should be stopped
                if (remove)
                    this.eventsToStop.Add(continuousEvent);
            }
            #endregion Continuous events

            #region Discrete events

            // Execute all discrete events
            foreach (EventInstance discreteEvent in this.discreteEvents)
            {
                // Reduce the remaining interval time
                discreteEvent.remainingInterval -= elapsedTime;

                // If the interval is over, execute the event the correct number of times, and reset the remaining interval
                while (discreteEvent.remainingInterval <= 0 && (discreteEvent.infiniteFrequency || discreteEvent.remainingFrequency > 0))
                {
                    ExecuteEvent(discreteEvent);
                    discreteEvent.remainingInterval += discreteEvent.totalInterval;

                    // Decrease the frequency
                    if (!discreteEvent.infiniteFrequency)
                        discreteEvent.remainingFrequency--;
                }

                // Check whether we should continue executing this event
                if (!discreteEvent.infiniteFrequency && discreteEvent.remainingFrequency <= 0)
                    this.eventsToStop.Add(discreteEvent);
            }
            #endregion Discrete events
        }
        #endregion Method: UpdateEvents(float elapsedTime)

        #region Method: UpdateEffects(float elapsedTime)
        /// <summary>
        /// Update all the effects.
        /// </summary>
        /// <param name="elapsedTime">The elapsed time in the base unit of the special time unit category.</param>
        private void UpdateEffects(float elapsedTime)
        {
            #region Delayed effects

            // Handle all delayed effects
            List<EffectInstance> delayedEffectsToRemove = new List<EffectInstance>();
            foreach (EffectInstance delayedEffect in this.delayedEffects)
            {
                // Reduce the delay time with the elapsed time
                delayedEffect.remainingDelay -= elapsedTime;

                // If the delay is over, start it, and make sure it is removed from the list
                if (delayedEffect.remainingDelay <= 0)
                {
                    StartEffect(delayedEffect);
                    delayedEffectsToRemove.Add(delayedEffect);
                }
            }
            foreach (EffectInstance delayedEffect in delayedEffectsToRemove)
                this.delayedEffects.Remove(delayedEffect);

            #endregion Delayed effects

            #region Continuous effects

            // Handle all continuous effects
            foreach (EffectInstance continuousEffect in this.continuousEffects)
            {
                bool remove = false;

                // Check whether there is still time left
                bool durationInfinite = continuousEffect.remainingDuration == float.PositiveInfinity;
                if (!durationInfinite)
                    continuousEffect.remainingDuration -= elapsedTime;
                if (continuousEffect.remainingDuration > 0)
                {
                    // Check whether the remaining time is also dependent on the satisfaction of the requirements
                    bool cont = true;
                    if (continuousEffect.durationDependentOnRequirements && continuousEffect.EventInstance != null && continuousEffect.EventInstance.Requirements != null)
                        cont = continuousEffect.EventInstance.Requirements.IsSatisfied(continuousEffect.EventInstance);

                    if (cont)
                    {
                        // Reduce the remaining interval time
                        continuousEffect.remainingInterval -= elapsedTime;

                        // If the interval is over, handle the effect the correct number of times, and reset the remaining interval
                        while (continuousEffect.remainingInterval <= 0)
                        {
                            ExecuteEffect(continuousEffect);
                            continuousEffect.remainingInterval += continuousEffect.totalInterval;
                        }
                    }
                    else
                        remove = true;
                }
                else
                    remove = true;

                // Indicate that it should be stopped
                if (remove)
                    this.effectsToStop.Add(continuousEffect);
            }

            #endregion Continuous effects

            #region Discrete effects

            // Execute all discrete effects
            foreach (EffectInstance discreteEffect in this.discreteEffects)
            {
                // Reduce the remaining interval time
                discreteEffect.remainingInterval -= elapsedTime;

                // If the interval is over, handle the effect the correct number of times, and reset the remaining interval
                while (discreteEffect.remainingInterval <= 0 && (discreteEffect.infiniteFrequency || discreteEffect.remainingFrequency > 0))
                {
                    ExecuteEffect(discreteEffect);
                    discreteEffect.remainingInterval += discreteEffect.totalInterval;

                    // Decrease the frequency
                    if (!discreteEffect.infiniteFrequency)
                        discreteEffect.remainingFrequency--;
                }

                // Check whether we should continue executing this effect
                if (!discreteEffect.infiniteFrequency && discreteEffect.remainingFrequency <= 0)
                    this.effectsToStop.Add(discreteEffect);
            }
            #endregion Discrete effects
        }
        #endregion Method: UpdateEffects(float elapsedTime)

        #region Method: UpdateContextTypes(EntityInstance entityInstance)
        /// <summary>
        /// Update the context types of the given entity instance.
        /// </summary>
        /// <param name="entityInstance">The entity instance to update the context types of.</param>
        private void UpdateContextTypes(EntityInstance entityInstance)
        {
            // Check whether the possible context types have been satisfied
            foreach (ContextTypeBase contextTypeBase in entityInstance.PossibleContextTypes)
            {
                if (contextTypeBase.IsSatisfied(entityInstance, entityInstance))
                    entityInstance.AddContextType(contextTypeBase);
                else
                    entityInstance.RemoveContextType(contextTypeBase);
            }
        }
        #endregion Method: UpdateContextTypes(EntityInstance entityInstance)

        #region Method: UpdatePredicates(EntityInstance entityInstance)
        /// <summary>
        /// Update the predicates of the given entity instance.
        /// </summary>
        /// <param name="entityInstance">The entity instance to update the predicates of.</param>
        private void UpdatePredicates(EntityInstance entityInstance)
        {
            if (entityInstance.EntityBase != null && entityInstance.World != null)
            {
                foreach (PredicateBase predicateBase in entityInstance.EntityBase.PredicatesAsActor)
                {
                    if (predicateBase.Target != null)
                    {
                        // Check all possible targets in the world
                        foreach (EntityInstance worldInstance in entityInstance.World.Instances)
                        {
                            if (worldInstance.IsNodeOf(predicateBase.Target))
                            {
                                // Check for satisfaction
                                if (predicateBase.IsSatisfied(entityInstance, worldInstance, null))
                                {
                                    // Check whether the predicate already exists, and if not, add it
                                    bool exists = false;
                                    foreach (PredicateInstance predicateInstance in entityInstance.PredicatesAsActor)
                                    {
                                        if (predicateBase.Equals(predicateInstance.PredicateBase) && worldInstance.Equals(predicateInstance.Target))
                                        {
                                            exists = true;
                                            break;
                                        }
                                    }
                                    if (!exists)
                                    {
                                        PredicateInstance predicateInstance = new PredicateInstance(predicateBase, entityInstance, worldInstance);
                                        entityInstance.AddPredicate(predicateInstance);
                                        worldInstance.AddPredicate(predicateInstance);
                                    }
                                }
                                else
                                {
                                    // Check whether the predicate already exists, and if so, remove it
                                    foreach (PredicateInstance predicateInstance in entityInstance.PredicatesAsActor)
                                    {
                                        if (predicateBase.Equals(predicateInstance.PredicateBase) && worldInstance.Equals(predicateInstance.Target))
                                        {
                                            entityInstance.RemovePredicate(predicateInstance);
                                            worldInstance.RemovePredicate(predicateInstance);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion Method: UpdatePredicates(EntityInstance entityInstance)

        #region Method: ClearAdditionsAndRemovals()
        /// <summary>
        /// Clear all additions and removals.
        /// </summary>
        private void ClearAdditionsAndRemovals()
        {
            foreach (EntityInstance entityInstance in this.addedAbstractEntities.Keys)
                entityInstance.addedOrRemoved = false;
            this.addedAbstractEntities.Clear();

            foreach (EntityInstance entityInstance in this.addedConnectionItems.Keys)
                entityInstance.addedOrRemoved = false;
            this.addedConnectionItems.Clear();

            foreach (EntityInstance entityInstance in this.addedCovers.Keys)
                entityInstance.addedOrRemoved = false;
            this.addedCovers.Clear();

            foreach (EntityInstance entityInstance in this.addedElements.Keys)
                entityInstance.addedOrRemoved = false;
            this.addedElements.Clear();

            foreach (EntityInstance entityInstance in this.addedItems.Keys)
                entityInstance.addedOrRemoved = false;
            this.addedItems.Clear();

            foreach (EntityInstance entityInstance in this.addedLayers.Keys)
                entityInstance.addedOrRemoved = false;
            this.addedLayers.Clear();

            foreach (EntityInstance entityInstance in this.addedMatter.Keys)
                entityInstance.addedOrRemoved = false;
            this.addedMatter.Clear();

            foreach (EntityInstance entityInstance in this.addedParts.Keys)
                entityInstance.addedOrRemoved = false;
            this.addedParts.Clear();

            foreach (EntityInstance entityInstance in this.addedRelationships.Keys)
                entityInstance.addedOrRemoved = false;
            this.addedRelationships.Clear();

            foreach (EntityInstance entityInstance in this.addedSpaces.Keys)
                entityInstance.addedOrRemoved = false;
            this.addedSpaces.Clear();

            foreach (EntityInstance entityInstance in this.addedSubstances.Keys)
                entityInstance.addedOrRemoved = false;
            this.addedSubstances.Clear();

            foreach (EntityInstance entityInstance in this.addedSubstances2.Keys)
                entityInstance.addedOrRemoved = false;
            this.addedSubstances2.Clear();

            foreach (EntityInstance entityInstance in this.addedTangibleMatter.Keys)
                entityInstance.addedOrRemoved = false;
            this.addedTangibleMatter.Clear();

            foreach (EntityInstance entityInstance in this.removedAbstractEntities.Keys)
                entityInstance.addedOrRemoved = false;
            this.removedAbstractEntities.Clear();

            foreach (EntityInstance entityInstance in this.removedConnectionItems.Keys)
                entityInstance.addedOrRemoved = false;
            this.removedConnectionItems.Clear();

            foreach (EntityInstance entityInstance in this.removedCovers.Keys)
                entityInstance.addedOrRemoved = false;
            this.removedCovers.Clear();

            foreach (EntityInstance entityInstance in this.removedElements.Keys)
                entityInstance.addedOrRemoved = false;
            this.removedElements.Clear();
            
            foreach (EntityInstance entityInstance in this.removedItems.Keys)
                entityInstance.addedOrRemoved = false;
            this.removedItems.Clear();

            foreach (EntityInstance entityInstance in this.removedMatter.Keys)
                entityInstance.addedOrRemoved = false;
            this.removedMatter.Clear();

            foreach (EntityInstance entityInstance in this.removedLayers.Keys)
                entityInstance.addedOrRemoved = false;
            this.removedLayers.Clear();

            foreach (EntityInstance entityInstance in this.removedParts.Keys)
                entityInstance.addedOrRemoved = false;
            this.removedParts.Clear();

            foreach (EntityInstance entityInstance in this.removedRelationships.Keys)
                entityInstance.addedOrRemoved = false;
            this.removedRelationships.Clear();

            foreach (EntityInstance entityInstance in this.removedSpaces.Keys)
                entityInstance.addedOrRemoved = false;
            this.removedSpaces.Clear();

            foreach (EntityInstance entityInstance in this.removedSubstances.Keys)
                entityInstance.addedOrRemoved = false;
            this.removedSubstances.Clear();

            foreach (EntityInstance entityInstance in this.removedSubstances2.Keys)
                entityInstance.addedOrRemoved = false;
            this.removedSubstances2.Clear();

            foreach (EntityInstance entityInstance in this.removedTangibleMatter.Keys)
                entityInstance.addedOrRemoved = false;
            this.removedTangibleMatter.Clear();
        }
        #endregion Method: ClearAdditionsAndRemovals()

        #region Method: StopEventsAndEffects()
        /// <summary>
        /// Stop the events and effects that have to be stopped.
        /// </summary>
        private void StopEventsAndEffects()
        {
            foreach (EventInstance eventToStop in this.eventsToStop)
                StopEvent(eventToStop);
            foreach (EffectInstance effectToStop in this.effectsToStop)
                StopEffect(effectToStop);
            this.eventsToStop.Clear();
            this.effectsToStop.Clear();
        }
        #endregion Method: StopEventsAndEffects()
		
        #endregion Method Group: Update

        #region Method Group: Action/Event handling

        #region Method: CanPerform(EntityInstance actor, ActionBase action)
        /// <summary>
        /// Checks whether the given actor can perform the given action.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="action">The action.</param>
        /// <returns>Returns whether the actor can perform the action.</returns>
        public bool CanPerform(EntityInstance actor, ActionBase action)
        {
            if (actor != null && action != null)
                return actor.Actions.Contains(action);
            return false;
        }
        #endregion Method: CanPerform(EntityInstance actor, ActionBase action)

        #region Method: CanPerform(EntityInstance actor, ActionBase action, EntityInstance target)
        /// <summary>
        /// Checks whether the given actor can perform the given action on the given target.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="action">The action.</param>
        /// <param name="target">The target.</param>
        /// <returns>Returns whether the actor can perform the action on the target.</returns>
        public bool CanPerform(EntityInstance actor, ActionBase action, EntityInstance target)
        {
            if (actor != null && action != null && target != null)
            {
                foreach (EventBase eventBase in actor.ManualEvents)
                {
                    // Check whether the actions match
                    if (action.Equals(eventBase.Action))
                    {
                        // Check whether the targets match
                        if (target.IsNodeOf(eventBase.Target))
                            return true;
                    }
                }
            }
            return false;
        }
        #endregion Method: CanPerform(EntityInstance actor, ActionBase action, EntityInstance target)
		
        #region Method: HandleAction(EntityInstance actor, ActionBase action)
        /// <summary>
        /// Try to let the actor perform the given action.
        /// </summary>
        /// <param name="actor">The actor to perform the action.</param>
        /// <param name="action">The action to perform.</param>
        /// <returns>Returns whether the action will be performed by the actor.</returns>
        public bool HandleAction(EntityInstance actor, ActionBase action)
        {
            return HandleAction(actor, action, null as Dictionary<string, object>);
        }

        /// <summary>
        /// Try to let the actor perform the given action.
        /// </summary>
        /// <param name="actor">The actor to perform the action.</param>
        /// <param name="action">The action to perform.</param>
        /// <param name="manualVariables">The manual variables.</param>
        /// <returns>Returns whether the action will be performed by the actor.</returns>
        public bool HandleAction(EntityInstance actor, ActionBase action, Dictionary<string, object> manualVariables)
        {
            bool actionHandled = false;
            if (actor != null && action != null)
            {
                // Get the corresponding events of the actor
                foreach (EventBase eventBase in actor.ManualEvents)
                {
                    // Check whether the actions match
                    if (action.Equals(eventBase.Action))
                    {
                        // Handle the event
                        if (HandleEvent(actor, eventBase, null, null, manualVariables))
                            actionHandled = true;
                    }
                }
            }
            return actionHandled;
        }
        #endregion Method: HandleAction(EntityInstance actor, ActionBase action)

        #region Method: HandleAction(EntityInstance actor, ActionBase action, EntityInstance target)
        /// <summary>
        /// Try to let the actor perform the given action on the given target.
        /// </summary>
        /// <param name="actor">The actor to perform the action.</param>
        /// <param name="action">The action to perform.</param>
        /// <param name="target">The target of the action.</param>
        /// <returns>Returns whether the action will be performed by the actor on the target.</returns>
        public bool HandleAction(EntityInstance actor, ActionBase action, EntityInstance target)
        {
            return HandleAction(actor, action, target, null as Dictionary<string, object>);
        }

        /// <summary>
        /// Try to let the actor perform the given action on the given target.
        /// </summary>
        /// <param name="actor">The actor to perform the action.</param>
        /// <param name="action">The action to perform.</param>
        /// <param name="target">The target of the action.</param>
        /// <param name="manualVariables">The manual variables.</param>
        /// <returns>Returns whether the action will be performed by the actor on the target.</returns>
        public bool HandleAction(EntityInstance actor, ActionBase action, EntityInstance target, Dictionary<string, object> manualVariables)
        {
            if (target == null)
                return HandleAction(actor, action);

            if (actor != null && action != null)
            {
                // Get the corresponding event of the actor
                foreach (EventBase eventBase in actor.ManualEvents)
                {
                    // Check whether the actions match
                    if (action.Equals(eventBase.Action))
                    {
                        // Check whether the targets match
                        if (target.IsNodeOf(eventBase.Target))
                        {
                            // Handle the event
                            if (HandleEvent(actor, eventBase, target, null, manualVariables))
                                return true;
                        }
                    }
                }
            }
            return false;
        }
        #endregion Method: HandleAction(EntityInstance actor, ActionBase action, EntityInstance target)

        #region Method: HandleAction(EntityInstance actor, ActionBase action, EntityInstance target, EntityInstance artifact)
        /// <summary>
        /// Try to let the actor perform the given action on the given target with the given artifact.
        /// </summary>
        /// <param name="actor">The actor to perform the action.</param>
        /// <param name="action">The action to perform.</param>
        /// <param name="target">The target of the action.</param>
        /// <param name="artifact">The artifact of the action.</param>
        /// <returns>Returns whether the action will be performed by the actor on the target.</returns>
        public bool HandleAction(EntityInstance actor, ActionBase action, EntityInstance target, EntityInstance artifact)
        {
            return HandleAction(actor, action, target, artifact, null);
        }

        /// <summary>
        /// Try to let the actor perform the given action on the given target.
        /// </summary>
        /// <param name="actor">The actor to perform the action.</param>
        /// <param name="action">The action to perform.</param>
        /// <param name="target">The target of the action.</param>
        /// <param name="artifact">The artifact of the action.</param>
        /// <param name="manualVariables">The manual variables.</param>
        /// <returns>Returns whether the action will be performed by the actor on the target.</returns>
        public bool HandleAction(EntityInstance actor, ActionBase action, EntityInstance target, EntityInstance artifact, Dictionary<string, object> manualVariables)
        {
            if (target == null)
                return HandleAction(actor, action);

            if (actor != null && action != null)
            {
                bool noArtifactRequired = false;

                // Get the corresponding event of the actor
                foreach (EventBase eventBase in actor.ManualEvents)
                {
                    // Check whether the action matches
                    if (action.Equals(eventBase.Action))
                    {
                        // Check whether the target matches
                        if (target.IsNodeOf(eventBase.Target))
                        {
                            // Check whether the event is also possible without an artifact
                            if (eventBase.Artifact == null)
                                noArtifactRequired = true;

                            // Check whether the artifact matches
                            if (eventBase.Artifact == null || (artifact != null && artifact.IsNodeOf(eventBase.Artifact)))
                            {
                                // Handle the event
                                if (HandleEvent(actor, eventBase, target, artifact, manualVariables))
                                    return true;
                            }
                        }
                    }
                }

                if (noArtifactRequired)
                    return HandleAction(actor, action, target, manualVariables);
            }
            return false;
        }
        #endregion Method: HandleAction(EntityInstance actor, ActionBase action, EntityInstance target, EntityInstance artifact)

        #region Method: HandleEvent(EntityInstance actor, EventBase eventBase, EntityInstance target, EntityInstance artifact, Dictionary<string, object> manualVariables)
        /// <summary>
        /// Handle the given requested event that the given actor wants to execute on the target: check whether the requirements have been satisfied, and if so, start it (after its possible delay).
        /// </summary>
        /// <param name="actor">The (required) actor of the event.</param>
        /// <param name="eventBase">The (required) event to check for satisfaction and start.</param>
        /// <param name="target">The (optional) target of the event.</param>
        /// <param name="artifact">The (optional) artifact of the event.</param>
        /// <param name="manualVariables">The (optional) manual variables.</param>
        /// <returns>Returns whether the event has been handled successfully, and will be executed.</returns>
        private bool HandleEvent(EntityInstance actor, EventBase eventBase, EntityInstance target, EntityInstance artifact, Dictionary<string, object> manualVariables)
        {
            if (actor != null && eventBase != null)
            {
                if (IsAllowedToPerformAction(actor, target))
                {
                    // Check whether the actor is not already executing the event
                    if (eventBase.NrOfSimultaneousUses > 0 && !IsActiveEvent(actor, eventBase))
                    {
                        // Check whether the event has the correct level of detail
                        if (eventBase.LevelOfDetail <= actor.LevelOfDetail)
                        {
                            // Create a new event instance
                            EventInstance eventInstance = new EventInstance(actor, eventBase, target, artifact, manualVariables);

                            // Check whether the requirements have been satisfied
                            if (eventBase.Requirements.IsSatisfied(eventInstance))
                            {
                                // Mark the event as an active event of the actor
                                actor.AddActiveEvent(eventInstance);

                                // Check whether the event can be executed right now, or whether there's a delay we have to wait for
                                float delay = GetDelay(eventInstance.Time, eventInstance);
                                eventInstance.remainingDelay = delay;
                                if (delay <= 0)
                                {
                                    // If there's no delay, start it immediately
                                    StartEvent(eventInstance);
                                }
                                else
                                {
                                    // In case of a delay, store it
                                    this.delayedEvents.Add(eventInstance);
                                }

                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        #endregion Method: HandleEvent(EntityInstance actor, EventBase eventBase, EntityInstance target, EntityInstance artifact, Dictionary<string, object> manualVariables)

        #region Method: IsActiveEvent(EntityInstance actor, EventBase eventBase)
        /// <summary>
        /// Check whether the given event is active for the given actor.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="eventBase">The event.</param>
        /// <param name="target">The (optional) target of the event.</param>
        /// <returns>Returns whether the event is active for the actor.</returns>
        private bool IsActiveEvent(EntityInstance actor, EventBase eventBase)
        {
            if (actor != null && eventBase != null)
            {
                int nrOfSimultaneousUses = eventBase.NrOfSimultaneousUses;
                foreach (EventInstance eventInstance in actor.ActiveEvents)
                {
                    if (eventBase.Equals(eventInstance.EventBase))
                    {
                        nrOfSimultaneousUses--;
                        if (nrOfSimultaneousUses <= 0)
                            return true;
                    }
                }
            }
            return false;
        }
        #endregion Method: IsActiveEvent(EntityInstance actor, EventBase eventBase)

        #region Method: StartEvent(EventInstance eventInstance)
        /// <summary>
        /// Start the given event.
        /// </summary>
        /// <param name="eventInstance">The event to start.</param>
        private void StartEvent(EventInstance eventInstance)
        {
            if (eventInstance != null)
            {
                TimeBase time = eventInstance.Time;
                EntityInstance actor = eventInstance.Actor;

                // Check the interval
                float interval = GetInterval(time, eventInstance);
                eventInstance.remainingInterval = interval;
                eventInstance.totalInterval = interval;

                // Check the duration
                bool dependentOnRequirements = false;
                float duration = GetDuration(time, eventInstance, out dependentOnRequirements);
                eventInstance.remainingDuration = duration;
                eventInstance.durationDependentOnRequirements = dependentOnRequirements;
                
                // Check whether the event is continuous or discrete to know whether it should be executed over time, or only once
                switch (time.TimeType)
                {
                    case TimeType.Continuous:
                        // Add the event to the list with continuous events
                        this.continuousEvents.Add(eventInstance);
                        break;

                    case TimeType.Discrete:
                        // Add the event to the list with discrete events, while also keeping track of the frequency
                        eventInstance.remainingFrequency = GetFrequency(time, eventInstance);
                        eventInstance.infiniteFrequency = time.FrequencyType == FrequencyType.Infinite;
                        this.discreteEvents.Add(eventInstance);
                        break;

                    default:
                        break;
                }

                // Invoke an event for the actor
                if (actor != null)
                    actor.InvokeExecutedAction(eventInstance);
            }
        }
        #endregion Method: StartEvent(EventInstance eventInstance)

        #region Method: ExecuteEvent(EventInstance eventInstance)
        /// <summary>
        /// Execute the given event.
        /// </summary>
        /// <param name="eventInstance">The event to execute.</param>
        private void ExecuteEvent(EventInstance eventInstance)
        {
            if (eventInstance != null && eventInstance.Actor != null)
            {
                // Check whether the requirements are still satisfied
                if (!eventInstance.firstExecution)
                {
                    if (!eventInstance.EventBase.Requirements.IsSatisfied(eventInstance))
                    {
                        this.eventsToStop.Add(eventInstance);
                        return;
                    }
                }
                else
                    eventInstance.firstExecution = false;

                // Invoke an event for the actor
                eventInstance.Actor.InvokeExecutedAction(eventInstance);

                // Handle all effects that should be started when the event starts
                foreach (EffectBase effectBase in eventInstance.Effects)
                {
                    if (effectBase.StartTrigger == EventState.Starts)
                        HandleEffect(effectBase, eventInstance);
                }
            }
        }
        #endregion Method: ExecuteEvent(EventInstance eventInstance)

        #region Method: StopAction(EntityInstance actor, ActionBase action)
        /// <summary>
        /// Let the given actor stop the given action.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="action">The action that should stop.</param>
        /// <returns>Returns whether the action of the actor has been stopped successfully.</returns>
        public bool StopAction(EntityInstance actor, ActionBase action)
        {
            return StopAction(actor, action, null);
        }
        #endregion Method: StopAction(EntityInstance actor, ActionBase action)

        #region Method: StopAction(EntityInstance actor, ActionBase action, EntityInstance target)
        /// <summary>
        /// Let the given actor stop the given action on the given target.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="action">The action that should stop.</param>
        /// <param name="target">The target.</param>
        /// <returns>Returns whether the action of the actor on the target has been stopped successfully.</returns>
        public bool StopAction(EntityInstance actor, ActionBase action, EntityInstance target)
        {
            if (actor != null && action != null)
            {
                // Find all active event instances of the action
                List<EventInstance> events = new List<EventInstance>();
                foreach (EventInstance activeEvent in actor.ActiveEvents)
                {
                    if (action.Equals(activeEvent.Action) && (target == null || target.Equals(activeEvent.Target)))
                        events.Add(activeEvent);
                }

                if (events.Count > 0)
                {
                    foreach (EventInstance activeEvent in events)
                        this.eventsToStop.Add(activeEvent);
                    return true;
                }
            }
            return false;
        }
        #endregion Method: StopAction(EntityInstance actor, ActionBase action, EntityInstance target)

        #region Method: StopEvent(EventInstance eventInstance)
        /// <summary>
        /// Stop and remove the given event instance.
        /// </summary>
        /// <param name="eventInstance">The event instance to stop.</param>
        /// <returns>Returns whether the event has been stopped successfully.</returns>
        private bool StopEvent(EventInstance eventInstance)
        {
            if (eventInstance != null)
            {
                // Remove the event from the actor
                if (eventInstance.Actor != null)
                    eventInstance.Actor.RemoveActiveEvent(eventInstance);

                // Remove the event from the delayed, continuous, or discrete events, if it is there
                this.delayedEvents.Remove(eventInstance);
                this.continuousEvents.Remove(eventInstance);
                this.discreteEvents.Remove(eventInstance);

                // Get all effects of this event, because they have to be removed as well
                foreach (EffectInstance delayedEffect in this.delayedEffects)
                {
                    if (eventInstance.Equals(delayedEffect.EventInstance) && !delayedEffect.EffectBase.OverrideTime)
                        this.effectsToStop.Add(delayedEffect);
                }
                foreach (EffectInstance continuousEffect in this.continuousEffects)
                {
                    if (eventInstance.Equals(continuousEffect.EventInstance) && !continuousEffect.EffectBase.OverrideTime)
                        this.effectsToStop.Add(continuousEffect);
                }
                foreach (EffectInstance discreteEffect in this.discreteEffects)
                {
                    // Discrete effects may be executed only one more time
                    if (eventInstance.Equals(discreteEffect.EventInstance) && discreteEffect.remainingFrequency > 1)
                        discreteEffect.remainingFrequency = 1;
                }

                // Invoke an event for the actor
                if (eventInstance.Actor != null)
                    eventInstance.Actor.InvokeStoppedAction(eventInstance);

                // Handle all effects that should be started when the event stops
                foreach (EffectBase effectBase in eventInstance.Effects)
                {
                    if (effectBase.StartTrigger == EventState.Stops)
                        HandleEffect(effectBase, eventInstance);
                }

                return true;
            }
            return false;
        }
        #endregion Method: StopEvent(EventInstance eventInstance)

        #region Method: physicalEntityInstance_PositionChanged(PhysicalEntityInstance sender, Vec3 position)
        /// <summary>
        /// When a physical entity instance is moved, check whether any collision requirements have been satisfied.
        /// </summary>
        /// <param name="sender">The changed physical entity instance.</param>
        /// <param name="position">The new position.</param>
        private void physicalEntityInstance_PositionChanged(PhysicalEntityInstance sender, Vec3 position)
        {
            if (sender != null)
            {
                // Get the space of the tangible object or matter
                SpaceInstance spaceInstance = null;
                if (sender is TangibleObjectInstance)
                    spaceInstance = ((TangibleObjectInstance)sender).Space;
                else if (sender is MatterInstance)
                    spaceInstance = ((MatterInstance)sender).Space;

                if (spaceInstance != null)
                {
                    // Check for collision with all items and tangible matter in the space
                    List<PhysicalEntityInstance> collisions = new List<PhysicalEntityInstance>();
                    foreach (TangibleObjectInstance item in spaceInstance.Items)
                    {
                        if (!sender.Equals(item) && PhysicsManager.Current.Collide(sender, item))
                            collisions.Add(item);
                    }
                    foreach (MatterInstance matter in spaceInstance.TangibleMatter)
                    {
                        if (!sender.Equals(matter) && PhysicsManager.Current.Collide(sender, matter))
                            collisions.Add(matter);
                    }

                    if (collisions.Count > 0)
                    {
                        // Get the correct events and handle them
                        foreach (EventBase automaticEvent in sender.AutomaticEvents)
                        {
                            if (automaticEvent.Requirements.ActorAndTargetCollide != null)
                            {
                                foreach (PhysicalEntityInstance target in collisions)
                                {
                                    if (target.IsNodeOf(automaticEvent.Target))
                                        HandleEvent(sender, automaticEvent, target, null, null);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion Method: physicalEntityInstance_PositionChanged(PhysicalEntityInstance sender, Vec3 position)

        #endregion Method Group: Action/Event handling

        #region Method Group: Effect handling

        #region Method: HandleEffect(EffectBase effectBase, EventInstance eventInstance)
        /// <summary>
        /// Handle the given effect instance.
        /// </summary>
        /// <param name="effectBase">The effect instance to handle.</param>
        /// <param name="eventInstance">The event instance of the efffect instance.</param>
        /// <returns>Returns whether the effect has been handled successfully, and will be executed.</returns>
        private bool HandleEffect(EffectBase effectBase, EventInstance eventInstance)
        {
            if (effectBase != null && eventInstance != null)
            {
                // Check whether the event is not already executing the effect
                if (!IsActiveEffect(eventInstance, effectBase))
                {
                    // Create a new effect instance
                    EffectInstance effectInstance = new EffectInstance(effectBase, eventInstance);

                    // Mark it as an active effect of the event
                    eventInstance.AddActiveEffect(effectInstance);

                    // If the time of the effect is overridden, check whether the effect can be executed right now,
                    // or whether there's a delay we have to wait for
                    if (effectBase.OverrideTime)
                    {
                        float delay = GetDelay(effectBase.Time, eventInstance);
                        effectInstance.remainingDelay = delay;
                        if (delay <= 0)
                        {
                            // If there's no delay, start it immediately
                            StartEffect(effectInstance);
                        }
                        else
                        {
                            // In case of a delay, store it
                            this.delayedEffects.Add(effectInstance);
                        }
                    }
                    // If not, immediately start the effect
                    else
                        StartEffect(effectInstance);

                    return true;
                }
            }
            return false;
        }
        #endregion Method: HandleEffect(EffectBase effectBase, EventInstance eventInstance)

        #region Method: IsActiveEffect(EventInstance eventInstance, EffectBase effectBase)
        /// <summary>
        /// Check whether the given effect is active for the given event.
        /// </summary>
        /// <param name="eventInstance">The event.</param>
        /// <param name="effectBase">The effect.</param>
        /// <returns>Returns whether the effect is active for the event.</returns>
        private bool IsActiveEffect(EventInstance eventInstance, EffectBase effectBase)
        {
            if (eventInstance != null && effectBase != null)
            {
                foreach (EffectInstance effectInstance in eventInstance.ActiveEffects)
                {
                    if (effectBase.Equals(effectInstance.EffectBase))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: IsActiveEffect(EventInstance eventInstance, EffectBase effectBase)

        #region Method: StartEffect(EffectInstance effectInstance)
        /// <summary>
        /// Start the given effect.
        /// </summary>
        /// <param name="effectInstance">The effect to start.</param>
        private void StartEffect(EffectInstance effectInstance)
        {
            if (effectInstance != null && effectInstance.EffectBase != null && effectInstance.EventInstance != null)
            {
                // Get the (possibly overridden) time
                TimeBase time = null;
                if (effectInstance.EffectBase.OverrideTime)
                    time = effectInstance.EffectBase.Time;
                else
                    time = effectInstance.EventInstance.Time;

                // Check the interval
                float interval = GetInterval(time, effectInstance.EventInstance);
                effectInstance.totalInterval = interval;

                // Check the duration
                bool dependentOnRequirements = false;
                float duration = GetDuration(time, effectInstance.EventInstance, out dependentOnRequirements);
                effectInstance.remainingDuration = duration;
                effectInstance.durationDependentOnRequirements = dependentOnRequirements;

                // Check whether the effect is continuous or discrete to know whether it should be executed over time, or only once
                switch (time.TimeType)
                {
                    case TimeType.Continuous:
                        // Add the effect to the list with continuous effects
                        this.continuousEffects.Add(effectInstance);
                        break;

                    case TimeType.Discrete:
                        // Add the effect to the list with discrete effects, while also keeping track of the frequency
                        effectInstance.remainingFrequency = GetFrequency(time, effectInstance.EventInstance);
                        effectInstance.infiniteFrequency = time.FrequencyType == FrequencyType.Infinite;
                        this.discreteEffects.Add(effectInstance);
                        break;

                    default:
                        break;
                }
            }
        }
        #endregion Method: StartEffect(EffectInstance effectInstance)

        #region Method: ExecuteEffect(EffectInstance effectInstance)
        /// <summary>
        /// Execute the given effect.
        /// </summary>
        /// <param name="effectInstance">The effect to execute.</param>
        private void ExecuteEffect(EffectInstance effectInstance)
        {
            if (effectInstance != null)
            {
                EffectBase effectBase = effectInstance.EffectBase;

                // Check whether the requirements are still satisfied
                if (!effectInstance.firstExecution)
                {
                    if (!effectInstance.EventInstance.EventBase.Requirements.IsSatisfied(effectInstance.EventInstance))
                    {
                        this.effectsToStop.Add(effectInstance);
                        return;
                    }
                }
                else
                    effectInstance.firstExecution = false;

                ChangeBase changeBase = effectBase as ChangeBase;
                if (changeBase != null)
                {
                    ExecuteChange(changeBase, effectInstance.EventInstance);
                    return;
                }

                EntityCreationBase entityCreationBase = effectBase as EntityCreationBase;
                if (entityCreationBase != null)
                {
                    ExecuteCreation(entityCreationBase, effectInstance.EventInstance);
                    return;
                }

                DeletionBase deletionBase = effectBase as DeletionBase;
                if (deletionBase != null)
                {
                    ExecuteDeletion(deletionBase, effectInstance.EventInstance);
                    return;
                }

                ReactionBase reactionBase = effectBase as ReactionBase;
                if (reactionBase != null)
                {
                    ExecuteReaction(reactionBase, effectInstance.EventInstance);
                    return;
                }

                TransferBase transferBase = effectBase as TransferBase;
                if (transferBase != null)
                {
                    ExecuteTransfer(transferBase, effectInstance.EventInstance);
                    return;
                }

                RelationshipEstablishmentBase relationshipEstablishmentBase = effectBase as RelationshipEstablishmentBase;
                if (relationshipEstablishmentBase != null)
                {
                    ExecuteRelationshipEstablishment(relationshipEstablishmentBase, effectInstance.EventInstance);
                    return;
                }

                FilterApplicationBase filterApplicationBase = effectBase as FilterApplicationBase;
                if (filterApplicationBase != null)
                {
                    ExecuteFilterApplication(filterApplicationBase, effectInstance.EventInstance);
                    return;
                }

                TransformationBase transformationBase = effectBase as TransformationBase;
                if (transformationBase != null)
                {
                    ExecuteTransformation(transformationBase, effectInstance.EventInstance);
                    return;
                }
            }
        }
        #endregion Method: ExecuteEffect(EffectInstance effectInstance)

        #region Method: ExecuteChange(ChangeBase changeBase, EventInstance eventInstance)
        /// <summary>
        /// Execute the change instance of the event instance.
        /// </summary>
        /// <param name="changeBase">The change instance to execute.</param>
        /// <param name="eventInstance">The event instance belonging to the change instance.</param>
        private void ExecuteChange(ChangeBase changeBase, EventInstance eventInstance)
        {
            if (changeBase != null && eventInstance != null && eventInstance.Actor != null)
            {
                if (Gamble(changeBase.Chance, eventInstance))
                {
                    // Retrieve all targets
                    foreach (EntityInstance target in GetTargets(changeBase.Range, eventInstance.Actor, eventInstance.Target, eventInstance.Artifact))
                    {
                        // Change the target and mark it as affected
                        if (ApplyChange(changeBase, eventInstance, target))
                            MarkAsAffected(target, eventInstance);
                    }
                }
            }
        }
        #endregion Method: ExecuteChange(ChangeBase changeBase, EventInstance eventInstance)

        #region Method: ApplyChange(ChangeBase changeBase, EventInstance eventInstance, EntityInstance target)
        /// <summary>
        /// Apply a change to a target.
        /// </summary>
        /// <param name="changeBase">The change instance to execute.</param>
        /// <param name="eventInstance">The event instance belonging to the change instance.</param>
        /// <param name="target">The target.</param>
        /// <returns>Returns whether the application has been successful.</returns>
        protected virtual bool ApplyChange(ChangeBase changeBase, EventInstance eventInstance, EntityInstance target)
        {
            if (target != null)
                return target.Apply(changeBase, eventInstance);
            return false;
        }
        #endregion Method: ApplyChange(ChangeBase changeBase, EventInstance eventInstance, EntityInstance target)

        #region Method: ExecuteCreation(EntityCreationBase entityCreationBase, EventInstance eventInstance)
        /// <summary>
        /// Execute the given creation instance of the event instance.
        /// </summary>
        /// <param name="entityCreationBase">The entity creation instance to execute.</param>
        /// <param name="eventInstance">The event instance belonging to the creation base.</param>
        private void ExecuteCreation(EntityCreationBase entityCreationBase, EventInstance eventInstance)
        {
            EntityInstance actor = eventInstance.Actor;
            EntityInstance target = eventInstance.Target;
            EntityInstance artifact = eventInstance.Artifact;
            if (entityCreationBase != null && eventInstance != null && actor != null && actor.World != null)
            {
                if (Gamble(entityCreationBase.Chance, eventInstance))
                {
                    // Create the instances
                    EntityValuedBase entityValuedBase = entityCreationBase.EntityValuedBase;
                    if (entityValuedBase != null)
                    {
                        List<EntityInstance> createdInstances = new List<EntityInstance>();

                        // Matter
                        MatterValuedBase matterValuedBase = entityValuedBase as MatterValuedBase;
                        if (matterValuedBase != null)
                            createdInstances.Add(InstanceManager.Current.Create<MatterInstance>(matterValuedBase));
                        else
                        {
                            // Space
                            SpaceValuedBase spaceValuedBase = entityValuedBase as SpaceValuedBase;
                            int quantity = (int)entityValuedBase.Quantity.GetValue(eventInstance);
                            if (spaceValuedBase != null)
                            {
                                for (int i = 0; i < quantity; i++)
                                    createdInstances.Add(InstanceManager.Current.Create<SpaceInstance>(spaceValuedBase));
                            }
                            else
                            {
                                // Other entities
                                for (int i = 0; i < quantity; i++)
                                {
                                    bool createInstance = true;
                                    if (entityValuedBase.EntityBase is TangibleObjectBase)
                                    {
                                        // Possibly create a tangible object instance by retrieving matter from a source
                                        PhysicalObjectInstance matterSource = null;
                                        if (entityCreationBase.MatterSource != null)
                                        {
                                            // Get the source of the matter
                                            if (entityCreationBase.MatterSource == ActorTargetArtifactReference.Actor)
                                                matterSource = actor as PhysicalObjectInstance;
                                            else if (entityCreationBase.MatterSource == ActorTargetArtifactReference.Target)
                                                matterSource = target as PhysicalObjectInstance;
                                            else if (entityCreationBase.MatterSource == ActorTargetArtifactReference.Artifact)
                                                matterSource = artifact as PhysicalObjectInstance;
                                            else if (entityCreationBase.MatterSource == ActorTargetArtifactReference.Reference && entityCreationBase.MatterSourceReference != null)
                                            {
                                                ReadOnlyCollection<EntityInstance> referencedEntities = entityCreationBase.MatterSourceReference.GetEntities(eventInstance);
                                                foreach (EntityInstance referencedEntity in referencedEntities)
                                                {
                                                    if (referencedEntity is PhysicalObjectInstance)
                                                    {
                                                        matterSource = (PhysicalObjectInstance)referencedEntity;
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                        // Possibly create a tangible object instance by retrieving parts from a source
                                        PhysicalObjectInstance partSource = null;
                                        if (entityCreationBase.PartSource != null)
                                        {
                                            // Get the source of the parts
                                            if (entityCreationBase.PartSource == ActorTargetArtifactReference.Actor)
                                                partSource = actor as PhysicalObjectInstance;
                                            else if (entityCreationBase.PartSource == ActorTargetArtifactReference.Target)
                                                partSource = target as PhysicalObjectInstance;
                                            else if (entityCreationBase.PartSource == ActorTargetArtifactReference.Artifact)
                                                partSource = artifact as PhysicalObjectInstance;
                                            else if (entityCreationBase.PartSource == ActorTargetArtifactReference.Reference && entityCreationBase.PartSourceReference != null)
                                            {
                                                ReadOnlyCollection<EntityInstance> referencedEntities = entityCreationBase.PartSourceReference.GetEntities(eventInstance);
                                                foreach (EntityInstance referencedEntity in referencedEntities)
                                                {
                                                    if (referencedEntity is PhysicalObjectInstance)
                                                    {
                                                        partSource = (PhysicalObjectInstance)referencedEntity;
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                        // Create the instance
                                        if (matterSource != null || partSource != null)
                                        {
                                            TangibleObjectInstance createdInstance = InstanceManager.Current.Create((TangibleObjectBase)entityValuedBase.EntityBase, matterSource, false, partSource, false);
                                            if (createdInstance != null)
                                                createdInstances.Add(createdInstance);
                                            createInstance = false;
                                        }
                                    }

                                    // Create the instance
                                    if (createInstance)
                                        createdInstances.Add(InstanceManager.Current.Create<EntityInstance>(entityValuedBase.EntityBase));
                                }
                            }
                        }

                        TangibleObjectInstance tangibleActor = actor as TangibleObjectInstance;
                        TangibleObjectInstance tangibleTarget = target as TangibleObjectInstance;

                        foreach (EntityInstance entityInstance in createdInstances)
                        {
                            // Add entity instances to the same world as the actor
                            PhysicalEntityInstance physicalEntityInstance  = entityInstance as PhysicalEntityInstance;
                            if (physicalEntityInstance != null)
                            {
                                // Get the possible position in the world
                                Vec3 worldPosition = Vec3.Zero;
                                if (entityCreationBase.Destination == Destination.World && entityCreationBase.Position != null)
                                {
                                    Vec4 pos = entityCreationBase.Position.GetValue(eventInstance);
                                    worldPosition = new Vec3(pos.X, pos.Y, pos.Z);
                                }
                                actor.World.AddInstance(physicalEntityInstance, worldPosition);
                            }
                            else
                                actor.World.AddInstance(entityInstance);

                            // Possibly add the instance to the actor or target in a special way
                            switch (entityCreationBase.Destination)
                            {
                                case Destination.ActorItems:
                                    TangibleObjectInstance itemOfActor = entityInstance as TangibleObjectInstance;
                                    if (itemOfActor != null && tangibleActor != null && tangibleActor.DefaultSpace != null)
                                        tangibleActor.DefaultSpace.AddItem(itemOfActor);
                                    break;
                                case Destination.ActorParts:
                                    TangibleObjectInstance partOfActor = entityInstance as TangibleObjectInstance;
                                    if (partOfActor != null && tangibleActor != null)
                                        tangibleActor.AddPart(partOfActor);
                                    break;
                                case Destination.ActorCovers:
                                    TangibleObjectInstance coverOfActor = entityInstance as TangibleObjectInstance;
                                    if (coverOfActor != null && tangibleActor != null)
                                        tangibleActor.AddCover(coverOfActor);
                                    break;
                                case Destination.ActorLayers:
                                    MatterInstance layerOfActor = entityInstance as MatterInstance;
                                    if (layerOfActor != null && tangibleActor != null)
                                        tangibleActor.AddLayer(layerOfActor);
                                    break;
                                case Destination.ActorElements:
                                    MatterInstance matterActor = actor as MatterInstance;
                                    ElementInstance elementOfActor = entityInstance as ElementInstance;
                                    if (elementOfActor != null && matterActor != null)
                                        matterActor.AddElement(elementOfActor);
                                    break;
                                case Destination.ActorSpaces:
                                    PhysicalObjectInstance physicalActor = actor as PhysicalObjectInstance;
                                    SpaceInstance spaceOfActor = entityInstance as SpaceInstance;
                                    if (spaceOfActor != null && physicalActor != null)
                                        physicalActor.AddSpace(spaceOfActor);
                                    break;
                                case Destination.ActorSubstances:
                                    SubstanceInstance substanceOfActor = entityInstance as SubstanceInstance;
                                    if (substanceOfActor != null)
                                    {
                                        MixtureInstance mixtureActor = actor as MixtureInstance;
                                        if (mixtureActor != null)
                                            mixtureActor.AddSubstance(substanceOfActor);
                                        CompoundInstance compoundActor = actor as CompoundInstance;
                                        if (compoundActor != null)
                                            compoundActor.AddSubstance(substanceOfActor);
                                    }
                                    break;
                                case Destination.TargetItems:
                                    TangibleObjectInstance itemOfTarget = entityInstance as TangibleObjectInstance;
                                    if (itemOfTarget != null && tangibleTarget != null && tangibleTarget.DefaultSpace != null)
                                        tangibleTarget.DefaultSpace.AddItem(itemOfTarget);
                                    break;
                                case Destination.TargetParts:
                                    TangibleObjectInstance partOfTarget = entityInstance as TangibleObjectInstance;
                                    if (partOfTarget != null && tangibleTarget != null)
                                        tangibleTarget.AddPart(partOfTarget);
                                    break;
                                case Destination.TargetCovers:
                                    TangibleObjectInstance coverOfTarget = entityInstance as TangibleObjectInstance;
                                    if (coverOfTarget != null && tangibleTarget != null)
                                        tangibleTarget.AddCover(coverOfTarget);
                                    break;
                                case Destination.TargetLayers:
                                    MatterInstance layerOfTarget = entityInstance as MatterInstance;
                                    if (layerOfTarget != null && tangibleTarget != null)
                                        tangibleTarget.AddLayer(layerOfTarget);
                                    break;
                                case Destination.TargetElements:
                                    MatterInstance matterTarget = target as MatterInstance;
                                    ElementInstance elementOfTarget = entityInstance as ElementInstance;
                                    if (elementOfTarget != null && matterTarget != null)
                                        matterTarget.AddElement(elementOfTarget);
                                    break;
                                case Destination.TargetSpaces:
                                    PhysicalObjectInstance physicalTarget = target as PhysicalObjectInstance;
                                    SpaceInstance spaceOfTarget = entityInstance as SpaceInstance;
                                    if (spaceOfTarget != null && physicalTarget != null)
                                        physicalTarget.AddSpace(spaceOfTarget);
                                    break;
                                case Destination.TargetSubstances:
                                    SubstanceInstance substanceOfTarget = entityInstance as SubstanceInstance;
                                    if (substanceOfTarget != null)
                                    {
                                        MixtureInstance mixtureTarget = target as MixtureInstance;
                                        if (mixtureTarget != null)
                                            mixtureTarget.AddSubstance(substanceOfTarget);
                                        CompoundInstance compoundTarget = target as CompoundInstance;
                                        if (compoundTarget != null)
                                            compoundTarget.AddSubstance(substanceOfTarget);
                                    }
                                    break;
                                default:
                                    break;
                            }

                            // Set the rotation
                            PhysicalObjectInstance physicalObject = entityInstance as PhysicalObjectInstance;
                            if (physicalObject != null)
                            {
                                Vec4 rotation = entityCreationBase.Rotation.GetValue(eventInstance);
                                physicalObject.Rotation = Quaternion.FromEulerAngles(rotation.X, rotation.Y, rotation.Z);
                            }

                            // Possibly establish a relationship
                            if (entityCreationBase.RelationshipType != null)
                            {
                                switch (entityCreationBase.RelationshipSourceTarget)
                                {
                                    case CreationRelationshipSourceTarget.RelateEntityAsSourceAndActorAsTarget:
                                        EntityInstance.AddRelationship(entityCreationBase.RelationshipType, entityInstance, actor);
                                        break;
                                    case CreationRelationshipSourceTarget.RelateEntityAsSourceAndTargetAsTarget:
                                        if (eventInstance.Target != null)
                                            EntityInstance.AddRelationship(entityCreationBase.RelationshipType, entityInstance, eventInstance.Target);
                                        break;
                                    case CreationRelationshipSourceTarget.RelateEntityAsTargetAndActorAsSource:
                                        EntityInstance.AddRelationship(entityCreationBase.RelationshipType, actor, entityInstance);
                                        break;
                                    case CreationRelationshipSourceTarget.RelateEntityAsTargetAndTargetAsSource:
                                        if (eventInstance.Target != null)
                                            EntityInstance.AddRelationship(entityCreationBase.RelationshipType, eventInstance.Target, entityInstance);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion Method: ExecuteCreation(EntityCreationBase entityCreationBase, EventInstance eventInstance)

        #region Method: ExecuteDeletion(DeletionBase deletionBase, EventInstance eventInstance)
        /// <summary>
        /// Execute the deletion of the event instance.
        /// </summary>
        /// <param name="deletionBase">The deletion to execute.</param>
        /// <param name="eventInstance">The event instance belonging to the deletion base.</param>
        private void ExecuteDeletion(DeletionBase deletionBase, EventInstance eventInstance)
        {
            if (deletionBase != null && eventInstance != null && eventInstance.Actor != null)
            {
                if (Gamble(deletionBase.Chance, eventInstance))
                {
                    NumericalValueInstance quantity = InstanceManager.Current.Create<NumericalValueInstance>(deletionBase.Quantity);

                    // Retrieve all targets and possibly remove them
                    foreach (EntityInstance target in GetTargets(deletionBase.Range, eventInstance.Actor, eventInstance.Target, eventInstance.Artifact))
                    {
                        // Only continue when there is any quantity left
                        if (deletionBase.DeletionType == DeletionType.DeleteAll || quantity.Value > 0)
                        {
                            if (target.World != null)
                            {
                                // Delete all, or take the quantity into account
                                switch (deletionBase.DeletionType)
                                {
                                    case DeletionType.DeleteAll:
                                        // Remove the instance from the world
                                        target.World.RemoveInstance(target, false);
                                        break;

                                    case DeletionType.DefinedAtQuantity:
                                        // Reduce the quantity of the matter
                                        MatterInstance matterInstance = target as MatterInstance;
                                        if (matterInstance != null)
                                        {
                                            NumericalValueInstance matterQuantity = new NumericalValueInstance(matterInstance.Quantity);
                                            if (matterQuantity >= quantity)
                                            {
                                                // Reduce the right amount of the matter
                                                matterInstance.Quantity -= quantity;
                                                quantity -= matterQuantity;
                                            }
                                            else
                                            {
                                                // Remove the entire matter
                                                matterInstance.Quantity -= matterInstance.Quantity;
                                                quantity -= matterInstance.Quantity;
                                            }
                                        }
                                        else
                                        {
                                            // Remove physical objects and reduce the quantity with 1
                                            PhysicalObjectInstance physicalObjectInstance = target as PhysicalObjectInstance;
                                            {
                                                physicalObjectInstance.World.RemoveInstance(physicalObjectInstance, false);
                                                quantity.Value--;
                                            }
                                        }
                                        break;

                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion Method: ExecuteDeletion(DeletionBase deletionBase, EventInstance eventInstance)

        #region Method: ExecuteReaction(ReactionBase reactionBase, EventInstance eventInstance)
        /// <summary>
        /// Execute the given reaction.
        /// </summary>
        /// <param name="reactionBase">The reaction to execute.</param>
        /// <param name="eventInstance">The event instance belonging to the reaction base.</param>
        private void ExecuteReaction(ReactionBase reactionBase, EventInstance eventInstance)
        {
            if (reactionBase != null && eventInstance != null)
            {
                // Get the actor of the reaction, which can be the actor or target of the original event
                EntityInstance actor = null;
                switch (reactionBase.Actor)
                {
                    case ActorTarget.Actor:
                        actor = eventInstance.Actor;
                        break;
                    case ActorTarget.Target:
                        actor = eventInstance.Target;
                        break;
                    default:
                        break;
                }

                if (actor != null && Gamble(reactionBase.Chance, eventInstance))
                {
                    // Check whether a target has been defined
                    if (reactionBase.Target == null)
                    {
                        // Get all valid targets in the range and handle the action on them
                        foreach (EntityInstance target in GetTargets(reactionBase.Range, actor, eventInstance.Target, eventInstance.Artifact))
                            HandleAction(actor, reactionBase.Action, target);
                    }
                    else if (reactionBase.Target == ActorTarget.Actor)
                    {
                        // Try to handle the action on the original actor
                        HandleAction(actor, reactionBase.Action, eventInstance.Actor);
                    }
                    else if (reactionBase.Target == ActorTarget.Target)
                    {
                        // Try to handle the action on the original target
                        HandleAction(actor, reactionBase.Action, eventInstance.Target);
                    }
                }
            }
        }
        #endregion Method: ExecuteReaction(ReactionBase reactionBase, EventInstance eventInstance)

        #region Method: ExecuteTransfer(TransferBase transferBase, EventInstance eventInstance)
        /// <summary>
        /// Execute the given transfer.
        /// </summary>
        /// <param name="transferBase">The transfer to execute.</param>
        /// <param name="eventInstance">The event instance belonging to the transfer base.</param>
        private void ExecuteTransfer(TransferBase transferBase, EventInstance eventInstance)
        {
            if (transferBase != null && eventInstance != null)
            {
                PhysicalObjectInstance physicalActor = eventInstance.Actor as PhysicalObjectInstance;
                if (physicalActor != null)
                {
                    if (Gamble(transferBase.Chance, eventInstance))
                    {
                        // Transfer items
                        if (transferBase.TransferType == TransferType.Items && physicalActor.Spaces.Count > 0)
                        {
                            // Retrieve all targets
                            List<EntityInstance> targets = new List<EntityInstance>();
                            if (eventInstance.Target != null)
                                targets.Add(eventInstance.Target);
                            else
                                targets.AddRange(GetTargets(transferBase.Range, eventInstance.Actor, eventInstance.Target, eventInstance.Artifact));
                            foreach (EntityInstance target in targets)
                            {
                                PhysicalObjectInstance physicalTarget = target as PhysicalObjectInstance;
                                if (physicalTarget != null && physicalTarget.Spaces.Count > 0)
                                {
                                    // Check the direction
                                    PhysicalObjectInstance source = null;
                                    PhysicalObjectInstance destination = null;
                                    switch (transferBase.Direction)
                                    {
                                        case TransferDirection.ActorToTarget:
                                            source = physicalActor;
                                            destination = physicalTarget;
                                            break;
                                        case TransferDirection.TargetToActor:
                                            source = physicalTarget;
                                            destination = physicalActor;
                                            break;
                                        case TransferDirection.ActorToActor:
                                            source = physicalActor;
                                            destination = physicalActor;
                                            break;
                                        case TransferDirection.TargetToTarget:
                                            source = physicalTarget;
                                            destination = physicalTarget;
                                            break;
                                        default:
                                            break;
                                    }

                                    if (source != null && destination != null)
                                    {
                                        bool success = false;

                                        // Search for the items to transfer
                                        foreach (TangibleObjectValuedBase itemToTransfer in transferBase.Items)
                                        {
                                            if (itemToTransfer.TangibleObjectBase != null)
                                            {
                                                // Check the remaining quantity
                                                int remainingQuantity = (int)itemToTransfer.Quantity.Value;
                                                if (remainingQuantity > 0)
                                                {
                                                    // Look in the spaces of the source
                                                    foreach (SpaceInstance spaceInstance in source.Spaces)
                                                    {
                                                        // Check whether this space is valid
                                                        if (transferBase.SourceSpace == null || spaceInstance.IsNodeOf(transferBase.SourceSpace))
                                                        {
                                                            // Look for matching items
                                                            List<TangibleObjectInstance> items = new List<TangibleObjectInstance>(spaceInstance.Items);
                                                            foreach (TangibleObjectInstance item in items)
                                                            {
                                                                if (item.IsNodeOf(itemToTransfer.TangibleObjectBase))
                                                                {
                                                                    // Remove the item from the space of the source
                                                                    spaceInstance.RemoveItem(item);

                                                                    // Add the item to the right space of the destination
                                                                    if (transferBase.DestinationSpace == null)
                                                                    {
                                                                        if (destination.DefaultSpace != null)
                                                                        {
                                                                            if (destination.DefaultSpace.AddItem(item) == Message.RelationSuccess)
                                                                                success = true;
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        foreach (SpaceInstance destinationSpace in destination.Spaces)
                                                                        {
                                                                            if (destinationSpace.IsNodeOf(transferBase.DestinationSpace))
                                                                            {
                                                                                if (destinationSpace.AddItem(item) == Message.RelationSuccess)
                                                                                    success = true;
                                                                                break;
                                                                            }
                                                                        }
                                                                    }

                                                                    // Reduce the remaining quantity
                                                                    remainingQuantity--;
                                                                    if (remainingQuantity <= 0)
                                                                        break;
                                                                }
                                                            }
                                                            if (remainingQuantity <= 0)
                                                                break;
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        // Mark the source and destination as affected
                                        if (success)
                                        {
                                            MarkAsAffected(source, eventInstance);
                                            MarkAsAffected(destination, eventInstance);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Get the source and destination
                            TangibleObjectInstance source = null;
                            PhysicalObjectInstance destination = null;
                            if (transferBase.TransferType == TransferType.ActorToTarget)
                            {
                                source = eventInstance.Actor as TangibleObjectInstance;
                                destination = eventInstance.Target as PhysicalObjectInstance;
                            }
                            else
                            {
                                source = eventInstance.Target as TangibleObjectInstance;
                                destination = eventInstance.Actor as PhysicalObjectInstance;
                            }
                            if (source != null && destination != null && destination.Spaces.Count > 0)
                            {
                                bool success = false;

                                if (transferBase.DestinationSpace == null)
                                {
                                    if (destination.DefaultSpace != null)
                                    {
                                        if (destination.DefaultSpace.AddItem(source) == Message.RelationSuccess)
                                            success = true;
                                    }
                                }
                                else
                                {
                                    foreach (SpaceInstance destinationSpace in destination.Spaces)
                                    {
                                        if (destinationSpace.IsNodeOf(transferBase.DestinationSpace))
                                        {
                                            if (destinationSpace.AddItem(source) == Message.RelationSuccess)
                                                success = true;
                                            break;
                                        }
                                    }
                                }

                                // Mark the source and destination as affected
                                if (success)
                                {
                                    MarkAsAffected(source, eventInstance);
                                    MarkAsAffected(destination, eventInstance);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion Method: ExecuteTransfer(TransferBase transferBase, EventInstance eventInstance)

        #region Method: ExecuteRelationshipEstablishment(RelationshipEstablishmentBase relationshipEstablishmentBase, EventInstance eventInstance)
        /// <summary>
        /// Execute the given relationship establishment.
        /// </summary>
        /// <param name="relationshipEstablishmentBase">The relationship establishment to execute.</param>
        /// <param name="eventInstance">The event instance belonging to the relationship establishment base.</param>
        private void ExecuteRelationshipEstablishment(RelationshipEstablishmentBase relationshipEstablishmentBase, EventInstance eventInstance)
        {
            if (relationshipEstablishmentBase != null && eventInstance != null)
            {
                if (Gamble(relationshipEstablishmentBase.Chance, eventInstance))
                {
                    EntityInstance source = null;
                    List<EntityInstance> targets = new List<EntityInstance>();

                    // Get the source and targets of the relationship
                    switch (relationshipEstablishmentBase.Source)
                    {
                        case ActorTarget.Actor:
                            source = eventInstance.Actor;
                            break;
                        case ActorTarget.Target:
                            source = eventInstance.Target;
                            break;
                        default:
                            break;
                    }
                    switch (relationshipEstablishmentBase.Target)
                    {
                        case ActorTargetArtifactReference.Actor:
                            targets.Add(eventInstance.Actor);
                            break;
                        case ActorTargetArtifactReference.Target:
                            targets.Add(eventInstance.Target);
                            break;
                        case ActorTargetArtifactReference.Reference:
                            if (relationshipEstablishmentBase.TargetReference != null)
                                targets.AddRange(relationshipEstablishmentBase.TargetReference.GetEntities(eventInstance));
                            break;
                        default:
                            break;
                    }

                    if (source != null)
                    {
                        foreach (EntityInstance target in targets)
                        {
                            // Create a relationship between the source and target, and mark them as affected
                            EntityInstance.AddRelationship(relationshipEstablishmentBase.RelationshipType, source, target);
                            MarkAsAffected(source, eventInstance);
                            MarkAsAffected(target, eventInstance);
                        }
                    }
                }
            }
        }
        #endregion Method: ExecuteRelationshipEstablishment(RelationshipEstablishmentBase relationshipEstablishmentBase, EventInstance eventInstance)

        #region Method: ExecuteFilterApplication(FilterApplicationBase filterApplicationBase, EventInstance eventInstance)
        /// <summary>
        /// Execute the given filter application.
        /// </summary>
        /// <param name="filterApplicationBase">The filter application to execute.</param>
        /// <param name="eventInstance">The event instance belonging to the filter application base.</param>
        private void ExecuteFilterApplication(FilterApplicationBase filterApplicationBase, EventInstance eventInstance)
        {
            if (filterApplicationBase != null && filterApplicationBase.FilterType != null && eventInstance != null && eventInstance.Actor != null)
            {
                if (Gamble(filterApplicationBase.Chance, eventInstance))
                {
                    // Retrieve all targets
                    foreach (EntityInstance target in GetTargets(filterApplicationBase.Range, eventInstance.Actor, eventInstance.Target, eventInstance.Artifact))
                    {
                        // Apply each filter when the target corresponds with the subject of the filter
                        foreach (FilterBase filterBase in filterApplicationBase.FilterType.Filters)
                        {
                            if (filterBase.Subject != null && target.IsNodeOf(filterBase.Subject))
                            {
                                // Handle all effects
                                foreach (EffectBase effectBase in filterBase.Effects)
                                    HandleEffect(effectBase, eventInstance);
                            }
                        }

                        // Notify of the filter application
                        target.InvokeFilterApplication(filterApplicationBase.FilterType);
                    }
                }
            }
        }
        #endregion Method: ExecuteFilterApplication(FilterApplicationBase filterApplicationBase, EventInstance eventInstance)

        #region Method: ExecuteTransformation(TransformationBase transformationBase, EventInstance eventInstance)
        /// <summary>
        /// Execute the given transformation.
        /// </summary>
        /// <param name="transformationBase">The transformation to execute.</param>
        /// <param name="eventInstance">The event instance belonging to the transformation base.</param>
        private void ExecuteTransformation(TransformationBase transformationBase, EventInstance eventInstance)
        {
            if (transformationBase != null && eventInstance != null && eventInstance.Actor != null)
            {
                if (Gamble(transformationBase.Chance, eventInstance))
                {
                    foreach (EntityInstance target in GetTargets(transformationBase.Range, eventInstance.Actor, eventInstance.Target, eventInstance.Artifact))
                    {
                        // Keep track of instances the target belongs to
                        SpaceInstance spaceOfTarget = null;
                        PhysicalObjectInstance physicalObjectOfTarget = null;
                        TangibleObjectInstance coveredObjectOfTarget = null;
                        TangibleObjectInstance wholeOfTarget = null;
                        CompoundInstance compoundOfTarget = null;
                        MixtureInstance mixtureOfTarget = null;
                        Vec3 position = null;
                        Quaternion rotation = null;

                        // Store the attributes and relationships
                        List<AttributeInstance> attributes = new List<AttributeInstance>(target.Attributes);
                        List<RelationshipInstance> relationshipsAsSource = new List<RelationshipInstance>(target.RelationshipsAsSource);
                        List<RelationshipInstance> relationshipsAsTarget = new List<RelationshipInstance>(target.RelationshipsAsTarget);

                        PhysicalObjectInstance physicalTarget = target as PhysicalObjectInstance;
                        if (physicalTarget != null)
                        {
                            // Store the position and rotation
                            position = physicalTarget.Position;
                            rotation = physicalTarget.Rotation;

                            TangibleObjectInstance tangibleTarget = target as TangibleObjectInstance;
                            if (tangibleTarget != null)
                            {
                                // Store the space, covered object, and whole
                                spaceOfTarget = tangibleTarget.Space;
                                coveredObjectOfTarget = tangibleTarget.CoveredObject;
                                wholeOfTarget = tangibleTarget.Whole;

                                // Retrieve the matter of tangible objects, remove it, and create tangible matter for it
                                List<MatterInstance> matter = new List<MatterInstance>(tangibleTarget.Matter);
                                foreach (MatterInstance matterInstance in matter)
                                {
                                    tangibleTarget.RemoveMatter(matterInstance);
                                    if (spaceOfTarget != null)
                                        spaceOfTarget.AddTangibleMatter(matterInstance);
                                }
                            }

                            SpaceInstance spaceTarget = target as SpaceInstance;
                            if (spaceTarget != null)
                            {
                                // Store the physical object of the space
                                physicalObjectOfTarget = spaceTarget.PhysicalObject;
                            }
                        }

                        SubstanceInstance substanceTarget = target as SubstanceInstance;
                        if (substanceTarget != null)
                        {
                            // Store the compound and mixture
                            compoundOfTarget = substanceTarget.Compound;
                            mixtureOfTarget = substanceTarget.Mixture;
                        }

                        // Remove the target from the world
                        SemanticWorld world = target.World;
                        world.RemoveInstance(target);

                        // Create the transformed entity instances
                        foreach (EntityValuedBase entityValuedBase in transformationBase.Entities)
                        {
                            List<EntityInstance> transformedInstances = new List<EntityInstance>();

                            if (entityValuedBase is MatterValuedBase)
                            {
                                // In case of matter, create one instance with the correct quantity
                                MatterInstance matterInstance = InstanceManager.Current.Create<MatterInstance>(entityValuedBase.EntityBase);
                                if (entityValuedBase.Quantity != null)
                                {
                                    matterInstance.Quantity.Value = entityValuedBase.Quantity.GetRandomValue(eventInstance);
                                    matterInstance.Quantity.Prefix = entityValuedBase.Quantity.Prefix;
                                    matterInstance.Quantity.Unit = entityValuedBase.Quantity.Unit;
                                }
                                transformedInstances.Add(matterInstance);
                            }
                            else
                            {
                                // In other cases, create the correct amount of instances
                                if (entityValuedBase.Quantity != null)
                                {
                                    for (int i = 0; i < entityValuedBase.Quantity.GetRandomInteger(eventInstance); i++)
                                        transformedInstances.Add(InstanceManager.Current.Create<EntityInstance>(entityValuedBase.EntityBase));
                                }
                            }

                            foreach (EntityInstance entityInstance in transformedInstances)
                            {
                                // Add the instance to the world
                                world.AddInstance(entityInstance);

                                // Restore the values of corresponding attributes
                                foreach (AttributeInstance originalAttribute in attributes)
                                {
                                    foreach (AttributeInstance newAttribute in entityInstance.Attributes)
                                    {
                                        if (newAttribute.AttributeBase.Equals(originalAttribute.AttributeBase))
                                        {
                                            newAttribute.Value.ReplaceBy(originalAttribute.Value);
                                            break;
                                        }
                                    }
                                }

                                // Restore the relationships and their attributes
                                foreach (RelationshipInstance originalRelationship in relationshipsAsSource)
                                    entityInstance.AddRelationshipAsSource(originalRelationship.RelationshipType, originalRelationship.Target);
                                foreach (RelationshipInstance originalRelationship in relationshipsAsSource)
                                {
                                    foreach (RelationshipInstance newRelationship in entityInstance.RelationshipsAsSource)
                                    {
                                        if (originalRelationship.RelationshipType.Equals(newRelationship.RelationshipType))
                                        {
                                            foreach (AttributeInstance originalAttribute in originalRelationship.Attributes)
                                            {
                                                foreach (AttributeInstance newAttribute in newRelationship.Attributes)
                                                {
                                                    if (newAttribute.AttributeBase.Equals(originalAttribute.AttributeBase))
                                                    {
                                                        newAttribute.Value.ReplaceBy(originalAttribute.Value);
                                                        break;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                                foreach (RelationshipInstance originalRelationship in relationshipsAsTarget)
                                    entityInstance.AddRelationshipAsTarget(originalRelationship.RelationshipType, originalRelationship.Source);
                                foreach (RelationshipInstance originalRelationship in relationshipsAsTarget)
                                {
                                    foreach (RelationshipInstance newRelationship in entityInstance.RelationshipsAsTarget)
                                    {
                                        if (originalRelationship.RelationshipType.Equals(newRelationship.RelationshipType))
                                        {
                                            foreach (AttributeInstance originalAttribute in originalRelationship.Attributes)
                                            {
                                                foreach (AttributeInstance newAttribute in newRelationship.Attributes)
                                                {
                                                    if (newAttribute.AttributeBase.Equals(originalAttribute.AttributeBase))
                                                    {
                                                        newAttribute.Value.ReplaceBy(originalAttribute.Value);
                                                        break;
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }

                                PhysicalObjectInstance physicalObjectInstance = entityInstance as PhysicalObjectInstance;
                                if (physicalObjectInstance != null)
                                {
                                    TangibleObjectInstance tangibleObjectInstance = physicalObjectInstance as TangibleObjectInstance;
                                    if (tangibleObjectInstance != null)
                                    {
                                        // Restore the space, covered object, and whole the untransformed instance belonged to
                                        if (spaceOfTarget != null)
                                            spaceOfTarget.AddItem(tangibleObjectInstance);
                                        if (coveredObjectOfTarget != null)
                                            coveredObjectOfTarget.AddCover(tangibleObjectInstance);
                                        if (wholeOfTarget != null)
                                            wholeOfTarget.AddPart(tangibleObjectInstance);
                                    }

                                    SpaceInstance spaceInstance = physicalObjectInstance as SpaceInstance;
                                    if (spaceInstance != null)
                                    {
                                        // Restore the physical object the untransformed instance belonged to
                                        if (physicalObjectOfTarget != null)
                                            physicalObjectOfTarget.AddSpace(spaceInstance);
                                    }

                                    // Restore the position and rotation of physical object instances
                                    if (position != null)
                                        physicalObjectInstance.Position = position;
                                    if (rotation != null)
                                        physicalObjectInstance.Rotation = rotation;
                                }

                                SubstanceInstance substanceInstance = entityInstance as SubstanceInstance;
                                if (substanceInstance != null)
                                {
                                    // Restore the compound and mixture the untransformed instance belonged to
                                    if (compoundOfTarget != null)
                                        compoundOfTarget.AddSubstance(substanceInstance);
                                    if (mixtureOfTarget != null)
                                        mixtureOfTarget.AddSubstance(substanceInstance);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion Method: ExecuteTransformation(TransformationBase transformationBase, EventInstance eventInstance)

        #region Method: StopEffect(EffectInstance effectInstance)
        /// <summary>
        /// Stop and remove the given effect instance.
        /// </summary>
        /// <param name="effectInstance">The effect instance to stop.</param>
        /// <returns>Returns whether the effect has been stopped successfully.</returns>
        private bool StopEffect(EffectInstance effectInstance)
        {
            if (effectInstance != null)
            {
                // Remove the effect from the event
                if (effectInstance.EventInstance != null)
                    effectInstance.EventInstance.RemoveActiveEffect(effectInstance);

                // Remove the effect from the delayed, continuous, or discrete effects, if it is there
                this.delayedEffects.Remove(effectInstance);
                this.continuousEffects.Remove(effectInstance);
                this.discreteEffects.Remove(effectInstance);

                return true;
            }
            return false;
        }
        #endregion Method: StopEffect(EffectInstance effectInstance)

        #region Method: MarkAsAffected(EntityInstance entityInstance, EventInstance eventInstance)
        /// <summary>
        /// Mark the given entity instance affected by the given event instance.
        /// </summary>
        /// <param name="entityInstance">The affected entity instance.</param>
        /// <param name="eventInstance">The event instance.</param>
        private void MarkAsAffected(EntityInstance entityInstance, EventInstance eventInstance)
        {
            if (entityInstance != null)
            {
                // Invoke an event to indicate that this event instance has been executed on the target
                entityInstance.InvokeAffectedByAction(eventInstance);
                entityInstance.IsModified = true;

                // Add the instance to the list with affected instances for context type checking
                if (!this.affectedInstances.Contains(entityInstance))
                    this.affectedInstances.Add(entityInstance);
            }
        }
        #endregion Method: MarkAsAffected(EntityInstance entityInstance, EventInstance eventInstance)

        #endregion Method Group: Effect handling

        #region Method Group: Helpers

        #region Method: GetDelay(TimeBase time, EventInstance eventInstance)
        /// <summary>
        /// Get the delay of the given time.
        /// </summary>
        /// <param name="time">The time to get the delay from.</param>
        /// <param name="eventInstance">The event instance.</param>
        /// <returns>The delay of the time.</returns>
        private static float GetDelay(TimeBase time, EventInstance eventInstance)
        {
            float delay = 0;

            if (time != null)
                delay = time.Delay.GetValue(eventInstance);

            return delay;
        }
        #endregion Method: GetDelay(TimeBase time, EventInstance eventInstance)

        #region Method: GetInterval(TimeBase time, EventInstance eventInstance)
        /// <summary>
        /// Get the interval of the given time.
        /// </summary>
        /// <param name="time">The time to get the interval from.</param>
        /// <param name="eventInstance">The event instance.</param>
        /// <returns>The interval of the time.</returns>
        private static float GetInterval(TimeBase time, EventInstance eventInstance)
        {
            float interval = 0;

            if (time != null)
                interval = time.Interval.GetValue(eventInstance);

            return interval;
        }
        #endregion Method: GetInterval(TimeBase time, EventInstance eventInstance)

        #region Method: GetDuration(TimeBase time, EventInstance eventInstance, out bool dependentOnRequirements)
        /// <summary>
        /// Get the duration of the given time.
        /// Also sets whether the duration is dependent on the requirements.
        /// </summary>
        /// <param name="time">The time to get the duration from.</param>
        /// <param name="eventInstance">The event instance.</param>
        /// <param name="dependentOnRequirements">Indicates whether the duration is dependent on the requirements.</param>
        /// <returns>The duration of the time.</returns>
        private static float GetDuration(TimeBase time, EventInstance eventInstance, out bool dependentOnRequirements)
        {
            float duration = 0;
            dependentOnRequirements = false;

            if (time != null)
            {
                switch (time.DurationType)
                {
                    case DurationType.Fixed:
                        // Get the fixed value
                        duration = time.Duration.GetValue(eventInstance);
                        break;

                    case DurationType.FixedDependentOnRequirements:
                        // Get the fixed value
                        duration = time.Duration.GetValue(eventInstance);
                        dependentOnRequirements = true;
                        break;

                    case DurationType.Infinite:
                        // Go on forever
                        duration = float.PositiveInfinity;
                        break;

                    case DurationType.InfiniteDependentOnRequirements:
                        // Go on forever
                        duration = float.PositiveInfinity;
                        dependentOnRequirements = true;
                        break;

                    default:
                        break;
                }
            }

            return duration;
        }
        #endregion Method: GetDuration(TimeBase time, EventInstance eventInstance, out bool dependentOnRequirements)

        #region Method: GetFrequency(TimeBase time, EventInstance eventInstance)
        /// <summary>
        /// Get the frequency of the given time.
        /// </summary>
        /// <param name="time">The time to get the frequency from.</param>
        /// <param name="eventInstance">The event instance.</param>
        /// <returns>The frequency of the time.</returns>
        private static int GetFrequency(TimeBase time, EventInstance eventInstance)
        {
            int frequency = 0;

            if (time != null)
            {
                switch (time.FrequencyType)
                {
                    case FrequencyType.Fixed:
                        // Get the fixed value
                        if (time.FrequencyVariable != null)
                            frequency = (int)new NumericalVariableInstance(time.FrequencyVariable, eventInstance).Value;
                        else
                            frequency = time.Frequency;
                        break;

                    case FrequencyType.Infinite:
                        frequency = int.MaxValue;
                        break;

                    default:
                        break;
                }
            }

            return frequency;
        }
        #endregion Method: GetFrequency(TimeBase time, EventInstance eventInstance)

        #region Method: Gamble(ChanceBase chanceBase, EventInstance eventInstance)
        /// <summary>
        /// Checks if the given chance is higher than a random number between 0 and 1.
        /// </summary>
        /// <param name="chanceBase">The chance to check.</param>
        /// <param name="eventInstance">The event instance.</param>
        /// <returns>Returns whether the given chance is higher than a random number between 0 and 1.</returns>
        private static bool Gamble(ChanceBase chanceBase, EventInstance eventInstance)
        {
            if (chanceBase != null)
            {
                // Get the chance value
                float val = 0;
                if (chanceBase.IsRandom)
                    val = RandomNumber.RandomF();
                else
                {
                    if (chanceBase.Variable != null)
                    {
                        if (chanceBase.Variable is NumericalVariableBase)
                            val = new NumericalVariableInstance((NumericalVariableBase)chanceBase.Variable, eventInstance).Value;
                        else if (chanceBase.Variable is TermVariableBase)
                            val = new TermVariableInstance((TermVariableBase)chanceBase.Variable, eventInstance).Value;
                        else if (chanceBase.Variable is RandomVariableBase)
                        {
                            // If it is a random variable, get the value that was calculated earlier in an update, or calculate and store its result
                            Dictionary<RandomVariableBase, bool> dictionary = null;
                            if (!randomVariableResults.TryGetValue(eventInstance, out dictionary))
                            {
                                dictionary = new Dictionary<RandomVariableBase, bool>();
                                randomVariableResults.Add(eventInstance, dictionary);
                            }
                            RandomVariableBase randomVariableBase = (RandomVariableBase)chanceBase.Variable;
                            bool success = false;
                            if (!dictionary.TryGetValue(randomVariableBase, out success))
                            {
                                success =  new RandomVariableInstance(randomVariableBase, eventInstance).Value >= RandomNumber.RandomF();
                                dictionary.Add(randomVariableBase, success);
                            }
                            return success;
                        }
                    }
                    else
                        val = chanceBase.Value;
                }

                return val >= RandomNumber.RandomF();
            }

            return false;
        }
        #endregion Method: Gamble(ChanceBase chanceBase, EventInstance eventInstance)

        #region Method: GetTargets(RangeBase range, EntityInstance actor, EntityInstance target, EntityInstance artifact)
        /// <summary>
        /// Get the targets in the given range.
        /// </summary>
        /// <param name="range">The range in which the targets should be found.</param>
        /// <param name="actor">The (optional) actor.</param>
        /// <param name="target">The (optional) target.</param>
        /// <param name="artifact">The (optional) artifact.</param>
        /// <returns>The targets in the range.</returns>
        private List<EntityInstance> GetTargets(RangeBase range, EntityInstance actor, EntityInstance target, EntityInstance artifact)
        {
            List<EntityInstance> targets = new List<EntityInstance>();

            // Search for particular entity instances
            if (range.RangeType == RangeType.EntitiesOnly || (range.RangeType == RangeType.EntitiesInRadius && range.Radius != null && actor is PhysicalObjectInstance))
            {
                #region Actor

                // Actor
                if (range.IncludeActor)
                    targets.Add(actor);

                // Connections of actor
                if (range.IncludeActorConnections)
                {
                    TangibleObjectInstance tangibleObjectInstance = actor as TangibleObjectInstance;
                    if (tangibleObjectInstance != null)
                    {
                        foreach (TangibleObjectInstance connectionItem in tangibleObjectInstance.Connections)
                            targets.Add(connectionItem);
                    }
                }

                // Parts of actor
                if (range.IncludeActorParts)
                {
                    TangibleObjectInstance tangibleObjectInstance = actor as TangibleObjectInstance;
                    if (tangibleObjectInstance != null)
                    {
                        foreach (TangibleObjectInstance part in tangibleObjectInstance.Parts)
                            targets.Add(part);
                    }
                }

                // Whole of actor
                if (range.IncludeActorWhole)
                {
                    TangibleObjectInstance tangibleObjectInstance = actor as TangibleObjectInstance;
                    if (tangibleObjectInstance != null && tangibleObjectInstance.Whole != null)
                        targets.Add(tangibleObjectInstance.Whole);
                }

                // Space items of actor
                if (range.IncludeActorSpaceItems)
                {
                    PhysicalObjectInstance physicalObjectInstance = actor as PhysicalObjectInstance;
                    if (physicalObjectInstance != null)
                    {
                        foreach (PhysicalObjectInstance spaceItem in physicalObjectInstance.SpaceItems)
                            targets.Add(spaceItem);
                    }
                }

                // Space tangible matter of actor
                if (range.IncludeActorSpaceTangibleMatter)
                {
                    TangibleObjectInstance tangibleObjectInstance = actor as TangibleObjectInstance;
                    if (tangibleObjectInstance != null)
                    {
                        foreach (SpaceInstance spaceInstance in tangibleObjectInstance.Spaces)
                        {
                            foreach (MatterInstance matterInstance in spaceInstance.TangibleMatter)
                                targets.Add(matterInstance);
                        }
                    }
                }

                // Matter of actor
                if (range.IncludeActorMatter)
                {
                    TangibleObjectInstance tangibleObjectInstance = actor as TangibleObjectInstance;
                    if (tangibleObjectInstance != null)
                    {
                        foreach (MatterInstance matterInstance in tangibleObjectInstance.Matter)
                            targets.Add(matterInstance);
                    }
                }

                // Covers of actor
                if (range.IncludeActorCovers)
                {
                    TangibleObjectInstance tangibleObjectInstance = actor as TangibleObjectInstance;
                    if (tangibleObjectInstance != null)
                    {
                        foreach (TangibleObjectInstance cover in tangibleObjectInstance.Covers)
                            targets.Add(cover);
                    }
                }

                // Covered object of actor
                if (range.IncludeActorCoveredObject)
                {
                    TangibleObjectInstance tangibleObjectInstance = actor as TangibleObjectInstance;
                    if (tangibleObjectInstance != null && tangibleObjectInstance.CoveredObject != null)
                    {
                        // Add the covered object
                        targets.Add(tangibleObjectInstance.CoveredObject);

                        // Also add all covers in between the actor/cover and the covered object
                        foreach (TangibleObjectInstance cover in tangibleObjectInstance.CoveredObject.Covers)
                        {
                            if (!tangibleObjectInstance.Equals(cover) && cover.CoverIndex < tangibleObjectInstance.CoverIndex)
                                targets.Add(cover);
                        }
                    }
                }

                // Source of relationship where actor is target
                if (range.IncludeSourceOfRelationshipWhereActorIsTarget != null)
                {
                    foreach (RelationshipInstance relationshipInstance in actor.RelationshipsAsTarget)
                    {
                        if (relationshipInstance.RelationshipType != null &&
                            relationshipInstance.RelationshipType.IsNodeOf(range.IncludeSourceOfRelationshipWhereActorIsTarget) &&
                            relationshipInstance.Source != null)
                            targets.Add(relationshipInstance.Source);
                    }
                }

                // Target of relationship where actor is source
                if (range.IncludeTargetOfRelationshipWhereActorIsSource != null)
                {
                    foreach (RelationshipInstance relationshipInstance in actor.RelationshipsAsSource)
                    {
                        if (relationshipInstance.RelationshipType != null &&
                            relationshipInstance.RelationshipType.IsNodeOf(range.IncludeTargetOfRelationshipWhereActorIsSource) &&
                            relationshipInstance.Target != null)
                            targets.Add(relationshipInstance.Target);
                    }
                }

                // Items in the space of the actor
                if (range.IncludeItemsInSpaceOfActor)
                {
                    TangibleObjectInstance tangibleObjectInstance = actor as TangibleObjectInstance;
                    if (tangibleObjectInstance != null && tangibleObjectInstance.Space != null)
                    {
                        foreach (TangibleObjectInstance item in tangibleObjectInstance.Space.Items)
                        {
                            if (!tangibleObjectInstance.Equals(item))
                                targets.Add(item);
                        }
                    }
                }

                #endregion Actor

                #region Target

                if (target != null)
                {
                    // Target
                    if (range.IncludeTarget)
                        targets.Add(target);

                    // Connections of target
                    if (range.IncludeTargetConnections)
                    {
                        TangibleObjectInstance tangibleObjectInstance = target as TangibleObjectInstance;
                        if (tangibleObjectInstance != null)
                        {
                            foreach (TangibleObjectInstance connectionItem in tangibleObjectInstance.Connections)
                                targets.Add(connectionItem);
                        }
                    }

                    // Parts of target
                    if (range.IncludeTargetParts)
                    {
                        TangibleObjectInstance tangibleObjectInstance = target as TangibleObjectInstance;
                        if (tangibleObjectInstance != null)
                        {
                            foreach (TangibleObjectInstance part in tangibleObjectInstance.Parts)
                                targets.Add(part);
                        }
                    }

                    // Whole of target
                    if (range.IncludeTargetWhole)
                    {
                        TangibleObjectInstance tangibleObjectInstance = target as TangibleObjectInstance;
                        if (tangibleObjectInstance != null && tangibleObjectInstance.Whole != null)
                            targets.Add(tangibleObjectInstance.Whole);
                    }

                    // Space items of target
                    if (range.IncludeTargetSpaceItems)
                    {
                        PhysicalObjectInstance physicalObjectInstance = target as PhysicalObjectInstance;
                        if (physicalObjectInstance != null)
                        {
                            foreach (PhysicalObjectInstance spaceItem in physicalObjectInstance.SpaceItems)
                                targets.Add(spaceItem);
                        }
                    }

                    // Space tangible matter of actor
                    if (range.IncludeTargetSpaceTangibleMatter)
                    {
                        TangibleObjectInstance tangibleObjectInstance = target as TangibleObjectInstance;
                        if (tangibleObjectInstance != null)
                        {
                            foreach (SpaceInstance spaceInstance in tangibleObjectInstance.Spaces)
                            {
                                foreach (MatterInstance matterInstance in spaceInstance.TangibleMatter)
                                    targets.Add(matterInstance);
                            }
                        }
                    }

                    // Matter of actor
                    if (range.IncludeTargetMatter)
                    {
                        TangibleObjectInstance tangibleObjectInstance = target as TangibleObjectInstance;
                        if (tangibleObjectInstance != null)
                        {
                            foreach (MatterInstance matterInstance in tangibleObjectInstance.Matter)
                                targets.Add(matterInstance);
                        }
                    }

                    // Covers of target
                    if (range.IncludeTargetCovers)
                    {
                        TangibleObjectInstance tangibleObjectInstance = target as TangibleObjectInstance;
                        if (tangibleObjectInstance != null)
                        {
                            foreach (TangibleObjectInstance cover in tangibleObjectInstance.Covers)
                                targets.Add(cover);
                        }
                    }

                    // Covered object of target
                    if (range.IncludeTargetCoveredObject)
                    {
                        TangibleObjectInstance tangibleObjectInstance = target as TangibleObjectInstance;
                        if (tangibleObjectInstance != null && tangibleObjectInstance.CoveredObject != null)
                        {
                            // Add the covered object
                            targets.Add(tangibleObjectInstance.CoveredObject);

                            // Also add all covers in between the target/cover and the covered object
                            foreach (TangibleObjectInstance cover in tangibleObjectInstance.CoveredObject.Covers)
                            {
                                if (!tangibleObjectInstance.Equals(cover) && cover.CoverIndex < tangibleObjectInstance.CoverIndex)
                                    targets.Add(cover);
                            }
                        }
                    }

                    // Source of relationship where target is target
                    if (range.IncludeSourceOfRelationshipWhereTargetIsTarget != null)
                    {
                        foreach (RelationshipInstance relationshipInstance in target.RelationshipsAsTarget)
                        {
                            if (relationshipInstance.RelationshipType != null &&
                                relationshipInstance.RelationshipType.IsNodeOf(range.IncludeSourceOfRelationshipWhereTargetIsTarget) &&
                                relationshipInstance.Source != null)
                                targets.Add(relationshipInstance.Source);
                        }
                    }

                    // Target of relationship where target is source
                    if (range.IncludeTargetOfRelationshipWhereTargetIsSource != null)
                    {
                        foreach (RelationshipInstance relationshipInstance in target.RelationshipsAsSource)
                        {
                            if (relationshipInstance.RelationshipType != null &&
                                relationshipInstance.RelationshipType.IsNodeOf(range.IncludeTargetOfRelationshipWhereTargetIsSource) && 
                                relationshipInstance.Target != null)
                                targets.Add(relationshipInstance.Target);
                        }
                    }

                    // Items in the space of the target
                    if (range.IncludeItemsInSpaceOfTarget)
                    {
                        TangibleObjectInstance tangibleObjectInstance = target as TangibleObjectInstance;
                        if (tangibleObjectInstance != null && tangibleObjectInstance.Space != null)
                        {
                            foreach (TangibleObjectInstance item in tangibleObjectInstance.Space.Items)
                            {
                                if (!tangibleObjectInstance.Equals(item))
                                    targets.Add(item);
                            }
                        }
                    }
                }
                #endregion Target

                #region Artifact

                if (artifact != null)
                {

                    // Artifact
                    if (range.IncludeArtifact)
                        targets.Add(artifact);

                    // Connections of artifact
                    if (range.IncludeArtifactConnections)
                    {
                        TangibleObjectInstance tangibleObjectInstance = artifact as TangibleObjectInstance;
                        if (tangibleObjectInstance != null)
                        {
                            foreach (TangibleObjectInstance connectionItem in tangibleObjectInstance.Connections)
                                targets.Add(connectionItem);
                        }
                    }

                    // Parts of artifact
                    if (range.IncludeArtifactParts)
                    {
                        TangibleObjectInstance tangibleObjectInstance = artifact as TangibleObjectInstance;
                        if (tangibleObjectInstance != null)
                        {
                            foreach (TangibleObjectInstance part in tangibleObjectInstance.Parts)
                                targets.Add(part);
                        }
                    }

                    // Whole of artifact
                    if (range.IncludeArtifactWhole)
                    {
                        TangibleObjectInstance tangibleObjectInstance = artifact as TangibleObjectInstance;
                        if (tangibleObjectInstance != null && tangibleObjectInstance.Whole != null)
                            targets.Add(tangibleObjectInstance.Whole);
                    }

                    // Space items of artifact
                    if (range.IncludeArtifactSpaceItems)
                    {
                        PhysicalObjectInstance physicalObjectInstance = artifact as PhysicalObjectInstance;
                        if (physicalObjectInstance != null)
                        {
                            foreach (PhysicalObjectInstance spaceItem in physicalObjectInstance.SpaceItems)
                                targets.Add(spaceItem);
                        }
                    }

                    // Space tangible matter of artifact
                    if (range.IncludeArtifactSpaceTangibleMatter)
                    {
                        TangibleObjectInstance tangibleObjectInstance = artifact as TangibleObjectInstance;
                        if (tangibleObjectInstance != null)
                        {
                            foreach (SpaceInstance spaceInstance in tangibleObjectInstance.Spaces)
                            {
                                foreach (MatterInstance matterInstance in spaceInstance.TangibleMatter)
                                    targets.Add(matterInstance);
                            }
                        }
                    }

                    // Matter of artifact
                    if (range.IncludeArtifactMatter)
                    {
                        TangibleObjectInstance tangibleObjectInstance = artifact as TangibleObjectInstance;
                        if (tangibleObjectInstance != null)
                        {
                            foreach (MatterInstance matterInstance in tangibleObjectInstance.Matter)
                                targets.Add(matterInstance);
                        }
                    }

                    // Covers of artifact
                    if (range.IncludeArtifactCovers)
                    {
                        TangibleObjectInstance tangibleObjectInstance = artifact as TangibleObjectInstance;
                        if (tangibleObjectInstance != null)
                        {
                            foreach (TangibleObjectInstance cover in tangibleObjectInstance.Covers)
                                targets.Add(cover);
                        }
                    }

                    // Covered object of artifact
                    if (range.IncludeArtifactCoveredObject)
                    {
                        TangibleObjectInstance tangibleObjectInstance = artifact as TangibleObjectInstance;
                        if (tangibleObjectInstance != null && tangibleObjectInstance.CoveredObject != null)
                        {
                            // Add the covered object
                            targets.Add(tangibleObjectInstance.CoveredObject);

                            // Also add all covers in between the artifact/cover and the covered object
                            foreach (TangibleObjectInstance cover in tangibleObjectInstance.CoveredObject.Covers)
                            {
                                if (!tangibleObjectInstance.Equals(cover) && cover.CoverIndex < tangibleObjectInstance.CoverIndex)
                                    targets.Add(cover);
                            }
                        }
                    }

                    // Source of relationship where artifact is target
                    if (range.IncludeSourceOfRelationshipWhereArtifactIsTarget != null)
                    {
                        foreach (RelationshipInstance relationshipInstance in artifact.RelationshipsAsTarget)
                        {
                            if (relationshipInstance.RelationshipType != null &&
                                relationshipInstance.RelationshipType.IsNodeOf(range.IncludeSourceOfRelationshipWhereArtifactIsTarget) &&
                                relationshipInstance.Source != null)
                                targets.Add(relationshipInstance.Source);
                        }
                    }

                    // Target of relationship where artifact is source
                    if (range.IncludeTargetOfRelationshipWhereArtifactIsSource != null)
                    {
                        foreach (RelationshipInstance relationshipInstance in artifact.RelationshipsAsSource)
                        {
                            if (relationshipInstance.RelationshipType != null &&
                                relationshipInstance.RelationshipType.IsNodeOf(range.IncludeTargetOfRelationshipWhereArtifactIsSource) &&
                                relationshipInstance.Target != null)
                                targets.Add(relationshipInstance.Target);
                        }
                    }

                    // Items in the space of the artifact
                    if (range.IncludeItemsInSpaceOfArtifact)
                    {
                        TangibleObjectInstance tangibleObjectInstance = artifact as TangibleObjectInstance;
                        if (tangibleObjectInstance != null && tangibleObjectInstance.Space != null)
                        {
                            foreach (TangibleObjectInstance item in tangibleObjectInstance.Space.Items)
                            {
                                if (!tangibleObjectInstance.Equals(item))
                                    targets.Add(item);
                            }
                        }
                    }
                }

                #endregion Artifact

                #region Specific targets

                // Specific targets
                foreach (EntityConditionBase entityConditionBase in range.SpecificTargets)
                {
                    // Get all instances of the specific entity
                    List<EntityInstance> specificTargets = new List<EntityInstance>();
                    foreach (EntityInstance entityInstance in actor.World.Instances)
                    {
                        if (entityInstance.Satisfies(entityConditionBase, entityInstance))
                            specificTargets.Add(entityInstance);
                    }
                    targets.AddRange(specificTargets);

                    // Connections of specific targets
                    if (range.IncludeSpecificTargetsConnections)
                    {
                        foreach (EntityInstance specificTarget in specificTargets)
                        {
                            TangibleObjectInstance tangibleObjectInstance = specificTarget as TangibleObjectInstance;
                            if (tangibleObjectInstance != null)
                            {
                                foreach (TangibleObjectInstance connectionItem in tangibleObjectInstance.Connections)
                                    targets.Add(connectionItem);
                            }
                        }
                    }

                    // Parts of specific targets
                    if (range.IncludeSpecificTargetsParts)
                    {
                        foreach (EntityInstance specificTarget in specificTargets)
                        {
                            TangibleObjectInstance tangibleObjectInstance = specificTarget as TangibleObjectInstance;
                            if (tangibleObjectInstance != null)
                            {
                                foreach (TangibleObjectInstance part in tangibleObjectInstance.Parts)
                                    targets.Add(part);
                            }
                        }
                    }

                    // Whole of specific targets
                    if (range.IncludeSpecificTargetsWhole)
                    {
                        foreach (EntityInstance specificTarget in specificTargets)
                        {
                            TangibleObjectInstance tangibleObjectInstance = specificTarget as TangibleObjectInstance;
                            if (tangibleObjectInstance != null && tangibleObjectInstance.Whole != null)
                                targets.Add(tangibleObjectInstance.Whole);
                        }
                    }

                    // Space items of specific targets
                    if (range.IncludeSpecificTargetsSpaceItems)
                    {
                        foreach (EntityInstance specificTarget in specificTargets)
                        {
                            PhysicalObjectInstance physicalObjectInstance = specificTarget as PhysicalObjectInstance;
                            if (physicalObjectInstance != null)
                            {
                                foreach (PhysicalObjectInstance spaceItem in physicalObjectInstance.SpaceItems)
                                    targets.Add(spaceItem);
                            }
                        }
                    }

                    // Space tangible matter of actor
                    if (range.IncludeSpecificTargetsSpaceTangibleMatter)
                    {
                        foreach (EntityInstance specificTarget in specificTargets)
                        {
                            TangibleObjectInstance tangibleObjectInstance = specificTarget as TangibleObjectInstance;
                            if (tangibleObjectInstance != null)
                            {
                                foreach (SpaceInstance spaceInstance in tangibleObjectInstance.Spaces)
                                {
                                    foreach (MatterInstance matterInstance in spaceInstance.TangibleMatter)
                                        targets.Add(matterInstance);
                                }
                            }
                        }
                    }

                    // Matter of actor
                    if (range.IncludeSpecificTargetsMatter)
                    {
                        foreach (EntityInstance specificTarget in specificTargets)
                        {
                            TangibleObjectInstance tangibleObjectInstance = specificTarget as TangibleObjectInstance;
                            if (tangibleObjectInstance != null)
                            {
                                foreach (MatterInstance matterInstance in tangibleObjectInstance.Matter)
                                    targets.Add(matterInstance);
                            }
                        }
                    }

                    // Covers of specific targets
                    if (range.IncludeSpecificTargetsCovers)
                    {
                        foreach (EntityInstance specificTarget in specificTargets)
                        {
                            TangibleObjectInstance tangibleObjectInstance = specificTarget as TangibleObjectInstance;
                            if (tangibleObjectInstance != null)
                            {
                                foreach (TangibleObjectInstance cover in tangibleObjectInstance.Covers)
                                    targets.Add(cover);
                            }
                        }
                    }

                    // Covered object of specific targets
                    if (range.IncludeSpecificTargetsCoveredObject)
                    {
                        foreach (EntityInstance specificTarget in specificTargets)
                        {
                            TangibleObjectInstance tangibleObjectInstance = specificTarget as TangibleObjectInstance;
                            if (tangibleObjectInstance != null && tangibleObjectInstance.CoveredObject != null)
                            {
                                // Add the covered object
                                targets.Add(tangibleObjectInstance.CoveredObject);

                                // Also add all covers in between the specific target/cover and the covered object
                                foreach (TangibleObjectInstance cover in tangibleObjectInstance.CoveredObject.Covers)
                                {
                                    if (!tangibleObjectInstance.Equals(cover) && cover.CoverIndex < tangibleObjectInstance.CoverIndex)
                                        targets.Add(cover);
                                }
                            }
                        }
                    }

                    // Source of relationship where specific target is target
                    if (range.IncludeSourceOfRelationshipWhereSpecificTargetIsTarget != null)
                    {
                        foreach (EntityInstance specificTarget in specificTargets)
                        {
                            foreach (RelationshipInstance relationshipInstance in specificTarget.RelationshipsAsTarget)
                            {
                                if (relationshipInstance.RelationshipType != null &&
                                    relationshipInstance.RelationshipType.IsNodeOf(range.IncludeSourceOfRelationshipWhereSpecificTargetIsTarget) &&
                                    relationshipInstance.Source != null)
                                    targets.Add(relationshipInstance.Source);
                            }
                        }
                    }

                    // Target of relationship where specific target is source
                    if (range.IncludeTargetOfRelationshipWhereSpecificTargetIsSource != null)
                    {
                        foreach (EntityInstance specificTarget in specificTargets)
                        {
                            foreach (RelationshipInstance relationshipInstance in specificTarget.RelationshipsAsSource)
                            {
                                if (relationshipInstance.RelationshipType != null &&
                                    relationshipInstance.RelationshipType.IsNodeOf(range.IncludeTargetOfRelationshipWhereSpecificTargetIsSource) &&
                                    relationshipInstance.Target != null)
                                    targets.Add(relationshipInstance.Target);
                            }
                        }
                    }

                    // Items in the space of the specific targets
                    if (range.IncludeItemsInSpaceOfActor)
                    {
                        foreach (EntityInstance specificTarget in specificTargets)
                        {
                            TangibleObjectInstance tangibleObjectInstance = specificTarget as TangibleObjectInstance;
                            if (tangibleObjectInstance != null && tangibleObjectInstance.Space != null)
                            {
                                foreach (TangibleObjectInstance item in tangibleObjectInstance.Space.Items)
                                {
                                    if (!tangibleObjectInstance.Equals(item))
                                        targets.Add(item);
                                }
                            }
                        }
                    }
                }

                #endregion Specific targets

                // Check whether we should only consider the targets in the specified radius
                if (range.RangeType == RangeType.EntitiesInRadius && range.Radius != null)
                {
                    PhysicalEntityInstance radiusOrigin = null;
                    switch (range.RadiusOrigin)
                    {
                        case ActorTargetArtifact.Actor:
                            radiusOrigin = actor as PhysicalEntityInstance;
                            break;
                        case ActorTargetArtifact.Target:
                            radiusOrigin = target as PhysicalEntityInstance;
                            break;
                        case ActorTargetArtifact.Artifact:
                            radiusOrigin = artifact as PhysicalEntityInstance;
                            break;
                        default:
                            break;
                    }
                    if (radiusOrigin != null)
                        targets = SortByDistance(range.Radius, radiusOrigin, targets.AsReadOnly());

                    // Possibly exclude the actor
                    if (range.ExcludeActor)
                        targets.Remove(actor);
                }
            }

            // Search for all physical object instances within the radius
            else if (range.RangeType == RangeType.RadiusOnly)
            {
                PhysicalEntityInstance radiusOrigin = null;
                switch (range.RadiusOrigin)
                {
                    case ActorTargetArtifact.Actor:
                        radiusOrigin = actor as PhysicalEntityInstance;
                        break;
                    case ActorTargetArtifact.Target:
                        radiusOrigin = target as PhysicalEntityInstance;
                        break;
                    case ActorTargetArtifact.Artifact:
                        radiusOrigin = artifact as PhysicalEntityInstance;
                        break;
                    default:
                        break;
                }
                if (radiusOrigin != null && radiusOrigin.World != null)
                    targets = SortByDistance(range.Radius, radiusOrigin, radiusOrigin.World.Instances);

                // Possibly exclude the actor
                if (range.ExcludeActor)
                    targets.Remove(actor);
            }

            // Remove targets if there are too many
            if (range.MaximumNumberOfTargets != null && targets.Count > range.MaximumNumberOfTargets)
                targets.RemoveRange((int)range.MaximumNumberOfTargets, targets.Count);

            // Remove targets that do not belong to any world (which may have been removed earlier)
            for (int i = 0; i < targets.Count; i++)
            {
                EntityInstance entityInstance = targets[i];
                if (entityInstance.World == null)
                {
                    targets.Remove(entityInstance);
                    i--;
                }
            }

            return targets;
        }
        #endregion Method: GetTargets(RangeBase range, EntityInstance actor, EntityInstance target, EntityInstance artifact)

        #region Method: SortByDistance(NumericalValueRangeBase radius, PhysicalEntityInstance actor, ReadOnlyCollection<EntityInstance> allInstances)
        /// <summary>
        /// Fill a list with instances with all instances that are within the radius of the actor.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="actor">The physical actor.</param>
        /// <param name="allInstances">All instances.</param>
        /// <returns>All instances within the rnage.</returns>
        private List<EntityInstance> SortByDistance(NumericalValueRangeBase range, PhysicalEntityInstance actor, ReadOnlyCollection<EntityInstance> allInstances)
        {
            List<EntityInstance> instances = new List<EntityInstance>();

            float radius = range.BaseValue;
            float radiusSquared = radius * radius;
            float radius2 = range.BaseValue2;
            float radius2Squared = radius2 * radius2;
            List<float> distances = new List<float>();
            foreach (EntityInstance entityInstance in allInstances)
            {
                PhysicalEntityInstance physicalEntityInstance = entityInstance as PhysicalEntityInstance;
                if (physicalEntityInstance != null)
                {
                    // Compare the distance to check whether it is in the radius
                    float distanceSquared = actor.GetDistance(physicalEntityInstance, true, true);
                    if (Toolbox.Compare(distanceSquared, range.ValueSign, radiusSquared, radius2Squared))
                    {
                        // Sort by distance
                        if (distances.Count == 0 || distances[distances.Count - 1] <= distanceSquared)
                        {
                            instances.Add(physicalEntityInstance);
                            distances.Add(distanceSquared);
                        }
                        else
                        {
                            for (int i = 0; i < distances.Count; i++)
                            {
                                if (distances[i] > distanceSquared)
                                {
                                    instances.Insert(i, physicalEntityInstance);
                                    distances.Insert(i, distanceSquared);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            return instances;
        }
        #endregion Method: SortByDistance(NumericalValueRangeBase radius, PhysicalEntityInstance actor, ReadOnlyCollection<EntityInstance> allInstances)

        #endregion Method Group: Helpers

        #region Method Group: Add/Remove handling

        #region Method: HandleAbstractEntityAdded(EntityInstance entityInstance, AbstractEntityInstance abstractEntity)
        /// <summary>
        /// Handle the addition of an abstract entity.
        /// </summary>
        /// <param name="entityInstance">The instance the abstract entity was added to.</param>
        /// <param name="abstractEntity">The added abstract entity.</param>
        internal void HandleAbstractEntityAdded(EntityInstance entityInstance, AbstractEntityInstance abstractEntity)
        {
            if (entityInstance != null && abstractEntity != null)
            {
                List<AbstractEntityInstance> abstractEntities;
                if (this.addedAbstractEntities.TryGetValue(entityInstance, out abstractEntities))
                    abstractEntities.Add(abstractEntity);
                else
                {
                    abstractEntities = new List<AbstractEntityInstance>(1);
                    abstractEntities.Add(abstractEntity);
                    this.addedAbstractEntities.Add(entityInstance, abstractEntities);
                }
                entityInstance.IsModified = true;
                entityInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the added abstract entities of the entity instance.
        /// </summary>
        /// <param name="entityInstance">The entity instance to get the added abstract entities of.</param>
        /// <returns>The added abstract entities of the entity instance.</returns>
        internal List<AbstractEntityInstance> GetAddedAbstractEntities(EntityInstance entityInstance)
        {
            List<AbstractEntityInstance> abstractEntities = null;
            if (entityInstance != null)
                this.addedAbstractEntities.TryGetValue(entityInstance, out abstractEntities);
            if (abstractEntities == null)
                abstractEntities = new List<AbstractEntityInstance>();
            return abstractEntities;
        }
        #endregion Method: HandleAbstractEntityAdded(EntityInstance entityInstance, AbstractEntityInstance abstractEntity)

        #region Method: HandleAbstractEntityRemoved(EntityInstance entityInstance, AbstractEntityInstance abstractEntity)
        /// <summary>
        /// Handle the removal of an abstract entity.
        /// </summary>
        /// <param name="entityInstance">The instance the abstract entity was removed from.</param>
        /// <param name="abstractEntityInstance">The removed abstract entity.</param>
        internal void HandleAbstractEntityRemoved(EntityInstance entityInstance, AbstractEntityInstance abstractEntity)
        {
            if (entityInstance != null && abstractEntity != null)
            {
                List<AbstractEntityInstance> abstractEntities;
                if (this.removedAbstractEntities.TryGetValue(entityInstance, out abstractEntities))
                    abstractEntities.Add(abstractEntity);
                else
                {
                    abstractEntities = new List<AbstractEntityInstance>(1);
                    abstractEntities.Add(abstractEntity);
                    this.removedAbstractEntities.Add(entityInstance, abstractEntities);
                }
                entityInstance.IsModified = true;
                entityInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the removed abstract entities of the entity instance.
        /// </summary>
        /// <param name="entityInstance">The entity instance to get the removed abstract entities of.</param>
        /// <returns>The removed abstract entities of the entity instance.</returns>
        internal List<AbstractEntityInstance> GetRemovedAbstractEntities(EntityInstance entityInstance)
        {
            List<AbstractEntityInstance> abstractEntities = null;
            if (entityInstance != null)
                this.removedAbstractEntities.TryGetValue(entityInstance, out abstractEntities);
            if (abstractEntities == null)
                abstractEntities = new List<AbstractEntityInstance>();
            return abstractEntities;
        }
        #endregion Method: HandleAbstractEntityRemoved(EntityInstance entityInstance, AbstractEntityInstance abstractEntity)

        #region Method: HandleConnectionItemAdded(TangibleObjectInstance tangibleObjectInstance, TangibleObjectInstance connectionItem)
        /// <summary>
        /// Handle the addition of a connection item.
        /// </summary>
        /// <param name="tangibleObjectInstance">The instance the connection item was added to.</param>
        /// <param name="connectionItem">The added connection item.</param>
        internal void HandleConnectionItemAdded(TangibleObjectInstance tangibleObjectInstance, TangibleObjectInstance connectionItem)
        {
            if (tangibleObjectInstance != null && connectionItem != null)
            {
                List<TangibleObjectInstance> connectionItems;
                if (this.addedConnectionItems.TryGetValue(tangibleObjectInstance, out connectionItems))
                    connectionItems.Add(connectionItem);
                else
                {
                    connectionItems = new List<TangibleObjectInstance>(1);
                    connectionItems.Add(connectionItem);
                    this.addedConnectionItems.Add(tangibleObjectInstance, connectionItems);
                }
                tangibleObjectInstance.IsModified = true;
                tangibleObjectInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the added connection items of the tangible object instance.
        /// </summary>
        /// <param name="tangibleObjectInstance">The tangible object instance to get the added connection items of.</param>
        /// <returns>The added connection items of the tangible object instance.</returns>
        internal List<TangibleObjectInstance> GetAddedConnectionItems(TangibleObjectInstance tangibleObjectInstance)
        {
            List<TangibleObjectInstance> connectionItems = null;
            if (tangibleObjectInstance != null)
                this.addedConnectionItems.TryGetValue(tangibleObjectInstance, out connectionItems);
            if (connectionItems == null)
                connectionItems = new List<TangibleObjectInstance>();
            return connectionItems;
        }
        #endregion Method: HandleConnectionItemAdded(TangibleObjectInstance tangibleObjectInstance, TangibleObjectInstance connectionItem)

        #region Method: HandleConnectionItemRemoved(TangibleObjectInstance tangibleObjectInstance, TangibleObjectInstance connectionItem)
        /// <summary>
        /// Handle the removal of a connection item.
        /// </summary>
        /// <param name="tangibleObjectInstance">The instance the connection item was removed from.</param>
        /// <param name="connectionItem">The removed connection item.</param>
        internal void HandleConnectionItemRemoved(TangibleObjectInstance tangibleObjectInstance, TangibleObjectInstance connectionItem)
        {
            if (tangibleObjectInstance != null && connectionItem != null)
            {
                List<TangibleObjectInstance> connectionItems;
                if (this.removedConnectionItems.TryGetValue(tangibleObjectInstance, out connectionItems))
                    connectionItems.Add(connectionItem);
                else
                {
                    connectionItems = new List<TangibleObjectInstance>(1);
                    connectionItems.Add(connectionItem);
                    this.removedConnectionItems.Add(tangibleObjectInstance, connectionItems);
                }
                tangibleObjectInstance.IsModified = true;
                tangibleObjectInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the removed connection items of the tangible object instance.
        /// </summary>
        /// <param name="tangibleObjectInstance">The tangible object instance to get the removed connection items of.</param>
        /// <returns>The removed connection items of the tangible object instance.</returns>
        internal List<TangibleObjectInstance> GetRemovedConnectionItems(TangibleObjectInstance tangibleObjectInstance)
        {
            List<TangibleObjectInstance> connectionItems = null;
            if (tangibleObjectInstance != null)
                this.removedConnectionItems.TryGetValue(tangibleObjectInstance, out connectionItems);
            if (connectionItems == null)
                connectionItems = new List<TangibleObjectInstance>();
            return connectionItems;
        }
        #endregion Method: HandleConnectionItemRemoved(TangibleObjectInstance tangibleObjectInstance, TangibleObjectInstance connectionItem)

        #region Method: HandleCoverAdded(TangibleObjectInstance tangibleObjectInstance, TangibleObjectInstance cover)
        /// <summary>
        /// Handle the addition of a cover.
        /// </summary>
        /// <param name="tangibleObjectInstance">The instance the cover was added to.</param>
        /// <param name="cover">The added cover.</param>
        internal void HandleCoverAdded(TangibleObjectInstance tangibleObjectInstance, TangibleObjectInstance cover)
        {
            if (tangibleObjectInstance != null && cover != null)
            {
                List<TangibleObjectInstance> covers;
                if (this.addedCovers.TryGetValue(tangibleObjectInstance, out covers))
                    covers.Add(cover);
                else
                {
                    covers = new List<TangibleObjectInstance>(1);
                    covers.Add(cover);
                    this.addedCovers.Add(tangibleObjectInstance, covers);
                }
                tangibleObjectInstance.IsModified = true;
                tangibleObjectInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the added covers of the tangible object instance.
        /// </summary>
        /// <param name="tangibleObjectInstance">The tangible object instance to get the added covers of.</param>
        /// <returns>The added covers of the tangible object instance.</returns>
        internal List<TangibleObjectInstance> GetAddedCovers(TangibleObjectInstance tangibleObjectInstance)
        {
            List<TangibleObjectInstance> covers = null;
            if (tangibleObjectInstance != null)
                this.addedCovers.TryGetValue(tangibleObjectInstance, out covers);
            if (covers == null)
                covers = new List<TangibleObjectInstance>();
            return covers;
        }
        #endregion Method: HandleCoverAdded(TangibleObjectInstance tangibleObjectInstance, TangibleObjectInstance cover)

        #region Method: HandleCoverRemoved(TangibleObjectInstance tangibleObjectInstance, TangibleObjectInstance cover)
        /// <summary>
        /// Handle the removal of a cover.
        /// </summary>
        /// <param name="tangibleObjectInstance">The instance the cover was removed from.</param>
        /// <param name="cover">The removed cover.</param>
        internal void HandleCoverRemoved(TangibleObjectInstance tangibleObjectInstance, TangibleObjectInstance cover)
        {
            if (tangibleObjectInstance != null && cover != null)
            {
                List<TangibleObjectInstance> covers;
                if (this.removedCovers.TryGetValue(tangibleObjectInstance, out covers))
                    covers.Add(cover);
                else
                {
                    covers = new List<TangibleObjectInstance>(1);
                    covers.Add(cover);
                    this.removedCovers.Add(tangibleObjectInstance, covers);
                }
                tangibleObjectInstance.IsModified = true;
                tangibleObjectInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the removed covers of the tangible object instance.
        /// </summary>
        /// <param name="tangibleObjectInstance">The tangible object instance to get the removed covers of.</param>
        /// <returns>The removed covers of the tangible object instance.</returns>
        internal List<TangibleObjectInstance> GetRemovedCovers(TangibleObjectInstance tangibleObjectInstance)
        {
            List<TangibleObjectInstance> covers = null;
            if (tangibleObjectInstance != null)
                this.removedCovers.TryGetValue(tangibleObjectInstance, out covers);
            if (covers == null)
                covers = new List<TangibleObjectInstance>();
            return covers;
        }
        #endregion Method: HandleCoverRemoved(TangibleObjectInstance tangibleObjectInstance, TangibleObjectInstance cover)

        #region Method: HandleElementAdded(MatterInstance matterInstance, ElementInstance element)
        /// <summary>
        /// Handle the addition of an element.
        /// </summary>
        /// <param name="matterInstance">The instance the element was added to.</param>
        /// <param name="element">The added element.</param>
        internal void HandleElementAdded(MatterInstance matterInstance, ElementInstance element)
        {
            if (matterInstance != null && element != null)
            {
                List<ElementInstance> elements;
                if (this.addedElements.TryGetValue(matterInstance, out elements))
                    elements.Add(element);
                else
                {
                    elements = new List<ElementInstance>(1);
                    elements.Add(element);
                    this.addedElements.Add(matterInstance, elements);
                }
                matterInstance.IsModified = true;
                matterInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the added elements of the matter instance.
        /// </summary>
        /// <param name="matterInstance">The matter instance to get the added elements of.</param>
        /// <returns>The added elements of the matter instance.</returns>
        internal List<ElementInstance> GetAddedElements(MatterInstance matterInstance)
        {
            List<ElementInstance> elements = null;
            if (matterInstance != null)
                this.addedElements.TryGetValue(matterInstance, out elements);
            if (elements == null)
                elements = new List<ElementInstance>();
            return elements;
        }
        #endregion Method: HandleElementAdded(MatterInstance matterInstance, ElementInstance element)

        #region Method: HandleElementRemoved(MatterInstance matterInstance, ElementInstance element)
        /// <summary>
        /// Handle the removal of an element.
        /// </summary>
        /// <param name="matterInstance">The instance the element was removed from.</param>
        /// <param name="elementInstance">The removed element.</param>
        internal void HandleElementRemoved(MatterInstance matterInstance, ElementInstance element)
        {
            if (matterInstance != null && element != null)
            {
                List<ElementInstance> elements;
                if (this.removedElements.TryGetValue(matterInstance, out elements))
                    elements.Add(element);
                else
                {
                    elements = new List<ElementInstance>(1);
                    elements.Add(element);
                    this.removedElements.Add(matterInstance, elements);
                }
                matterInstance.IsModified = true;
                matterInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the removed elements of the matter instance.
        /// </summary>
        /// <param name="matterInstance">The matter instance to get the removed elements of.</param>
        /// <returns>The removed elements of the matter instance.</returns>
        internal List<ElementInstance> GetRemovedElements(MatterInstance matterInstance)
        {
            List<ElementInstance> elements = null;
            if (matterInstance != null)
                this.removedElements.TryGetValue(matterInstance, out elements);
            if (elements == null)
                elements = new List<ElementInstance>();
            return elements;
        }
        #endregion Method: HandleElementRemoved(MatterInstance matterInstance, ElementInstance element)

        #region Method: HandleItemAdded(SpaceInstance spaceInstance, TangibleObjectInstance item)
        /// <summary>
        /// Handle the addition of an item.
        /// </summary>
        /// <param name="spaceInstance">The instance the item was added to.</param>
        /// <param name="item">The added item.</param>
        internal void HandleItemAdded(SpaceInstance spaceInstance, TangibleObjectInstance item)
        {
            if (spaceInstance != null && item != null)
            {
                List<TangibleObjectInstance> items;
                if (this.addedItems.TryGetValue(spaceInstance, out items))
                    items.Add(item);
                else
                {
                    items = new List<TangibleObjectInstance>(1);
                    items.Add(item);
                    this.addedItems.Add(spaceInstance, items);
                }
                spaceInstance.IsModified = true;
                spaceInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the added items of the space instance.
        /// </summary>
        /// <param name="spaceInstance">The space instance to get the added items of.</param>
        /// <returns>The added items of the space instance.</returns>
        internal List<TangibleObjectInstance> GetAddedItems(SpaceInstance spaceInstance)
        {
            List<TangibleObjectInstance> items = null;
            if (spaceInstance != null)
                this.addedItems.TryGetValue(spaceInstance, out items);
            if (items == null)
                items = new List<TangibleObjectInstance>();
            return items;
        }
        #endregion Method: HandleItemAdded(SpaceInstance spaceInstance, TangibleObjectInstance item)

        #region Method: HandleItemRemoved(SpaceInstance spaceInstance, TangibleObjectInstance item)
        /// <summary>
        /// Handle the removal of an item.
        /// </summary>
        /// <param name="spaceInstance">The instance the item was removed from.</param>
        /// <param name="item">The removed item.</param>
        internal void HandleItemRemoved(SpaceInstance spaceInstance, TangibleObjectInstance item)
        {
            if (spaceInstance != null && item != null)
            {
                List<TangibleObjectInstance> items;
                if (this.removedItems.TryGetValue(spaceInstance, out items))
                    items.Add(item);
                else
                {
                    items = new List<TangibleObjectInstance>(1);
                    items.Add(item);
                    this.removedItems.Add(spaceInstance, items);
                }
                spaceInstance.IsModified = true;
                spaceInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the removed items of the space instance.
        /// </summary>
        /// <param name="spaceInstance">The space instance to get the removed items of.</param>
        /// <returns>The removed items of the space instance.</returns>
        internal List<TangibleObjectInstance> GetRemovedItems(SpaceInstance spaceInstance)
        {
            List<TangibleObjectInstance> items = null;
            if (spaceInstance != null)
                this.removedItems.TryGetValue(spaceInstance, out items);
            if (items == null)
                items = new List<TangibleObjectInstance>();
            return items;
        }
        #endregion Method: HandleItemRemoved(SpaceInstance spaceInstance, TangibleObjectInstance item)

        #region Method: HandleTangibleMatterAdded(SpaceInstance spaceInstance, MatterInstance tangibleMatter)
        /// <summary>
        /// Handle the addition of tangible matter.
        /// </summary>
        /// <param name="spaceInstance">The instance the tangible matter was added to.</param>
        /// <param name="tangibleMatter">The added tangible matter.</param>
        internal void HandleTangibleMatterAdded(SpaceInstance spaceInstance, MatterInstance tangibleMatter)
        {
            if (spaceInstance != null && tangibleMatter != null)
            {
                List<MatterInstance> tangibleMatterInstances;
                if (this.addedTangibleMatter.TryGetValue(spaceInstance, out tangibleMatterInstances))
                    tangibleMatterInstances.Add(tangibleMatter);
                else
                {
                    tangibleMatterInstances = new List<MatterInstance>(1);
                    tangibleMatterInstances.Add(tangibleMatter);
                    this.addedTangibleMatter.Add(spaceInstance, tangibleMatterInstances);
                }
                spaceInstance.IsModified = true;
                spaceInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the added tangible matter of the space instance.
        /// </summary>
        /// <param name="spaceInstance">The space instance to get the added tangible matter of.</param>
        /// <returns>The added tangible matter of the space instance.</returns>
        internal List<MatterInstance> GetAddedTangibleMatter(SpaceInstance spaceInstance)
        {
            List<MatterInstance> tangibleMatterInstances = null;
            if (spaceInstance != null)
                this.addedTangibleMatter.TryGetValue(spaceInstance, out tangibleMatterInstances);
            if (tangibleMatterInstances == null)
                tangibleMatterInstances = new List<MatterInstance>();
            return tangibleMatterInstances;
        }
        #endregion Method: HandleTangibleMatterAdded(SpaceInstance spaceInstance, MatterInstance tangibleMatter)

        #region Method: HandleTangibleMatterRemoved(SpaceInstance spaceInstance, MatterInstance tangibleMatter)
        /// <summary>
        /// Handle the removal of tangible matter.
        /// </summary>
        /// <param name="spaceInstance">The instance the tangible matter was removed from.</param>
        /// <param name="tangibleMatter">The removed tangible matter.</param>
        internal void HandleTangibleMatterRemoved(SpaceInstance spaceInstance, MatterInstance tangibleMatter)
        {
            if (spaceInstance != null && tangibleMatter != null)
            {
                List<MatterInstance> tangibleMatterInstances;
                if (this.removedTangibleMatter.TryGetValue(spaceInstance, out tangibleMatterInstances))
                    tangibleMatterInstances.Add(tangibleMatter);
                else
                {
                    tangibleMatterInstances = new List<MatterInstance>(1);
                    tangibleMatterInstances.Add(tangibleMatter);
                    this.removedTangibleMatter.Add(spaceInstance, tangibleMatterInstances);
                }
                spaceInstance.IsModified = true;
                spaceInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the removed tangible matter of the space instance.
        /// </summary>
        /// <param name="spaceInstance">The space instance to get the removed tangible matter of.</param>
        /// <returns>The removed tangible matter of the space instance.</returns>
        internal List<MatterInstance> GetRemovedTangibleMatter(SpaceInstance spaceInstance)
        {
            List<MatterInstance> tangibleMatterInstances = null;
            if (spaceInstance != null)
                this.removedTangibleMatter.TryGetValue(spaceInstance, out tangibleMatterInstances);
            if (tangibleMatterInstances == null)
                tangibleMatterInstances = new List<MatterInstance>();
            return tangibleMatterInstances;
        }
        #endregion Method: HandleTangibleMatterRemoved(SpaceInstance spaceInstance, MatterInstance tangibleMatter)

        #region Method: HandleLayerAdded(TangibleObjectInstance tangibleObjectInstance, MatterInstance layer)
        /// <summary>
        /// Handle the addition of a layer.
        /// </summary>
        /// <param name="tangibleObjectInstance">The instance the layer was added to.</param>
        /// <param name="layer">The added layer.</param>
        internal void HandleLayerAdded(TangibleObjectInstance tangibleObjectInstance, MatterInstance layer)
        {
            if (tangibleObjectInstance != null && layer != null)
            {
                List<MatterInstance> layers;
                if (this.addedLayers.TryGetValue(tangibleObjectInstance, out layers))
                    layers.Add(layer);
                else
                {
                    layers = new List<MatterInstance>(1);
                    layers.Add(layer);
                    this.addedLayers.Add(tangibleObjectInstance, layers);
                }
                tangibleObjectInstance.IsModified = true;
                tangibleObjectInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the added layers of the tangible object instance.
        /// </summary>
        /// <param name="tangibleObjectInstance">The tangible object instance to get the added layers of.</param>
        /// <returns>The added layers of the tangible object instance.</returns>
        internal List<MatterInstance> GetAddedLayers(TangibleObjectInstance tangibleObjectInstance)
        {
            List<MatterInstance> layers = null;
            if (tangibleObjectInstance != null)
                this.addedLayers.TryGetValue(tangibleObjectInstance, out layers);
            if (layers == null)
                layers = new List<MatterInstance>();
            return layers;
        }
        #endregion Method: HandleLayerAdded(TangibleObjectInstance tangibleObjectInstance, MatterInstance layer)

        #region Method: HandleLayerRemoved(TangibleObjectInstance tangibleObjectInstance, MatterInstance layer)
        /// <summary>
        /// Handle the removal of a layer.
        /// </summary>
        /// <param name="tangibleObjectInstance">The instance the layer was removed from.</param>
        /// <param name="layer">The removed layer.</param>
        internal void HandleLayerRemoved(TangibleObjectInstance tangibleObjectInstance, MatterInstance layer)
        {
            if (tangibleObjectInstance != null && layer != null)
            {
                List<MatterInstance> layers;
                if (this.removedLayers.TryGetValue(tangibleObjectInstance, out layers))
                    layers.Add(layer);
                else
                {
                    layers = new List<MatterInstance>(1);
                    layers.Add(layer);
                    this.removedLayers.Add(tangibleObjectInstance, layers);
                }
                tangibleObjectInstance.IsModified = true;
                tangibleObjectInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the removed layers of the tangible object instance.
        /// </summary>
        /// <param name="tangibleObjectInstance">The tangible object instance to get the removed layers of.</param>
        /// <returns>The removed layers of the tangible object instance.</returns>
        internal List<MatterInstance> GetRemovedLayers(TangibleObjectInstance tangibleObjectInstance)
        {
            List<MatterInstance> layers = null;
            if (tangibleObjectInstance != null)
                this.removedLayers.TryGetValue(tangibleObjectInstance, out layers);
            if (layers == null)
                layers = new List<MatterInstance>();
            return layers;
        }
        #endregion Method: HandleLayerRemoved(TangibleObjectInstance tangibleObjectInstance, MatterInstance layer)

        #region Method: HandleMatterAdded(TangibleObjectInstance tangibleObjectInstance, MatterInstance matter)
        /// <summary>
        /// Handle the addition of matter.
        /// </summary>
        /// <param name="tangibleObjectInstance">The instance the matter was added to.</param>
        /// <param name="matter">The added matter.</param>
        internal void HandleMatterAdded(TangibleObjectInstance tangibleObjectInstance, MatterInstance matter)
        {
            if (tangibleObjectInstance != null && matter != null)
            {
                List<MatterInstance> allMatter;
                if (this.addedMatter.TryGetValue(tangibleObjectInstance, out allMatter))
                    allMatter.Add(matter);
                else
                {
                    allMatter = new List<MatterInstance>(1);
                    allMatter.Add(matter);
                    this.addedMatter.Add(tangibleObjectInstance, allMatter);
                }
                tangibleObjectInstance.IsModified = true;
                tangibleObjectInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the added matter of the tangible object instance.
        /// </summary>
        /// <param name="tangibleObjectInstance">The tangible object instance to get the added matter of.</param>
        /// <returns>The added matter of the tangible object instance.</returns>
        internal List<MatterInstance> GetAddedMatter(TangibleObjectInstance tangibleObjectInstance)
        {
            List<MatterInstance> allMatter = null;
            if (tangibleObjectInstance != null)
                this.addedMatter.TryGetValue(tangibleObjectInstance, out allMatter);
            if (allMatter == null)
                allMatter = new List<MatterInstance>();
            return allMatter;
        }
        #endregion Method: HandleMatterAdded(TangibleObjectInstance tangibleObjectInstance, MatterInstance matter)

        #region Method: HandleMatterRemoved(TangibleObjectInstance tangibleObjectInstance, MatterInstance matter)
        /// <summary>
        /// Handle the removal of matter.
        /// </summary>
        /// <param name="tangibleObjectInstance">The instance the matter was removed from.</param>
        /// <param name="matter">The removed matter.</param>
        internal void HandleMatterRemoved(TangibleObjectInstance tangibleObjectInstance, MatterInstance matter)
        {
            if (tangibleObjectInstance != null && matter != null)
            {
                List<MatterInstance> allMatter;
                if (this.removedMatter.TryGetValue(tangibleObjectInstance, out allMatter))
                    allMatter.Add(matter);
                else
                {
                    allMatter = new List<MatterInstance>(1);
                    allMatter.Add(matter);
                    this.removedMatter.Add(tangibleObjectInstance, allMatter);
                }
                tangibleObjectInstance.IsModified = true;
                tangibleObjectInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the removed matter of the tangible object instance.
        /// </summary>
        /// <param name="tangibleObjectInstance">The tangible object instance to get the removed matter of.</param>
        /// <returns>The removed matter of the tangible object instance.</returns>
        internal List<MatterInstance> GetRemovedMatter(TangibleObjectInstance tangibleObjectInstance)
        {
            List<MatterInstance> allMatter = null;
            if (tangibleObjectInstance != null)
                this.removedMatter.TryGetValue(tangibleObjectInstance, out allMatter);
            if (allMatter == null)
                allMatter = new List<MatterInstance>();
            return allMatter;
        }
        #endregion Method: HandleMatterRemoved(TangibleObjectInstance tangibleObjectInstance, MatterInstance matter)

        #region Method: HandlePartAdded(TangibleObjectInstance tangibleObjectInstance, TangibleObjectInstance part)
        /// <summary>
        /// Handle the addition of a part.
        /// </summary>
        /// <param name="tangibleObjectInstance">The instance the part was added to.</param>
        /// <param name="part">The added part.</param>
        internal void HandlePartAdded(TangibleObjectInstance tangibleObjectInstance, TangibleObjectInstance part)
        {
            if (tangibleObjectInstance != null && part != null)
            {
                List<TangibleObjectInstance> parts;
                if (this.addedParts.TryGetValue(tangibleObjectInstance, out parts))
                    parts.Add(part);
                else
                {
                    parts = new List<TangibleObjectInstance>(1);
                    parts.Add(part);
                    this.addedParts.Add(tangibleObjectInstance, parts);
                }
                tangibleObjectInstance.IsModified = true;
                tangibleObjectInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the added parts of the tangible object instance.
        /// </summary>
        /// <param name="tangibleObjectInstance">The tangible object instance to get the added parts of.</param>
        /// <returns>The added parts of the tangible object instance.</returns>
        internal List<TangibleObjectInstance> GetAddedParts(TangibleObjectInstance tangibleObjectInstance)
        {
            List<TangibleObjectInstance> parts = null;
            if (tangibleObjectInstance != null)
                this.addedParts.TryGetValue(tangibleObjectInstance, out parts);
            if (parts == null)
                parts = new List<TangibleObjectInstance>();
            return parts;
        }
        #endregion Method: HandlePartAdded(TangibleObjectInstance tangibleObjectInstance, TangibleObjectInstance part)

        #region Method: HandlePartRemoved(TangibleObjectInstance tangibleObjectInstance, TangibleObjectInstance part)
        /// <summary>
        /// Handle the removal of a part.
        /// </summary>
        /// <param name="tangibleObjectInstance">The instance the part was removed from.</param>
        /// <param name="part">The removed part.</param>
        internal void HandlePartRemoved(TangibleObjectInstance tangibleObjectInstance, TangibleObjectInstance part)
        {
            if (tangibleObjectInstance != null && part != null)
            {
                List<TangibleObjectInstance> parts;
                if (this.removedParts.TryGetValue(tangibleObjectInstance, out parts))
                    parts.Add(part);
                else
                {
                    parts = new List<TangibleObjectInstance>(1);
                    parts.Add(part);
                    this.removedParts.Add(tangibleObjectInstance, parts);
                }
                tangibleObjectInstance.IsModified = true;
                tangibleObjectInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the removed parts of the tangible object instance.
        /// </summary>
        /// <param name="tangibleObjectInstance">The tangible object instance to get the removed parts of.</param>
        /// <returns>The removed parts of the tangible object instance.</returns>
        internal List<TangibleObjectInstance> GetRemovedParts(TangibleObjectInstance tangibleObjectInstance)
        {
            List<TangibleObjectInstance> parts = null;
            if (tangibleObjectInstance != null)
                this.removedParts.TryGetValue(tangibleObjectInstance, out parts);
            if (parts == null)
                parts = new List<TangibleObjectInstance>();
            return parts;
        }
        #endregion Method: HandlePartRemoved(TangibleObjectInstance tangibleObjectInstance, TangibleObjectInstance part)
        
        #region Method: HandleRelationshipAdded(EntityInstance entityInstance, RelationshipInstance relationship)
        /// <summary>
        /// Handle the addition of a relationship.
        /// </summary>
        /// <param name="entityInstance">The instance the relationship was added to.</param>
        /// <param name="relationship">The added relationship.</param>
        internal void HandleRelationshipAdded(EntityInstance entityInstance, RelationshipInstance relationship)
        {
            if (entityInstance != null && relationship != null)
            {
                List<RelationshipInstance> relationships;
                if (this.addedRelationships.TryGetValue(entityInstance, out relationships))
                    relationships.Add(relationship);
                else
                {
                    relationships = new List<RelationshipInstance>(1);
                    relationships.Add(relationship);
                    this.addedRelationships.Add(entityInstance, relationships);
                }
                entityInstance.IsModified = true;
                entityInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the added relationships of the entity instance.
        /// </summary>
        /// <param name="entityInstance">The entity instance to get the added relationships of.</param>
        /// <returns>The added relationships of the entity instance.</returns>
        internal List<RelationshipInstance> GetAddedRelationships(EntityInstance entityInstance)
        {
            List<RelationshipInstance> relationships = null;
            if (entityInstance != null)
                this.addedRelationships.TryGetValue(entityInstance, out relationships);
            if (relationships == null)
                relationships = new List<RelationshipInstance>();
            return relationships;
        }
        #endregion Method: HandleRelationshipAdded(EntityInstance entityInstance, RelationshipInstance relationship)

        #region Method: HandleRelationshipRemoved(EntityInstance entityInstance, RelationshipInstance relationship)
        /// <summary>
        /// Handle the removal of a relationship.
        /// </summary>
        /// <param name="entityInstance">The instance the relationship was removed from.</param>
        /// <param name="relationship">The removed relationship.</param>
        internal void HandleRelationshipRemoved(EntityInstance entityInstance, RelationshipInstance relationship)
        {
            if (entityInstance != null && relationship != null)
            {
                List<RelationshipInstance> relationships;
                if (this.removedRelationships.TryGetValue(entityInstance, out relationships))
                    relationships.Add(relationship);
                else
                {
                    relationships = new List<RelationshipInstance>(1);
                    relationships.Add(relationship);
                    this.removedRelationships.Add(entityInstance, relationships);
                }
                entityInstance.IsModified = true;
                entityInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the removed relationships of the entity instance.
        /// </summary>
        /// <param name="entityInstance">The entity instance to get the removed relationships of.</param>
        /// <returns>The removed relationships of the entity instance.</returns>
        internal List<RelationshipInstance> GetRemovedRelationships(EntityInstance entityInstance)
        {
            List<RelationshipInstance> relationships = null;
            if (entityInstance != null)
                this.removedRelationships.TryGetValue(entityInstance, out relationships);
            if (relationships == null)
                relationships = new List<RelationshipInstance>();
            return relationships;
        }
        #endregion Method: HandleRelationshipRemoved(EntityInstance entityInstance, RelationshipInstance relationship)

        #region Method: HandleSpaceAdded(PhysicalObjectInstance physicalObjectInstance, SpaceInstance space)
        /// <summary>
        /// Handle the addition of a space.
        /// </summary>
        /// <param name="physicalObjectInstance">The instance the space was added to.</param>
        /// <param name="space">The added space.</param>
        internal void HandleSpaceAdded(PhysicalObjectInstance physicalObjectInstance, SpaceInstance space)
        {
            if (physicalObjectInstance != null && space != null)
            {
                List<SpaceInstance> spaces;
                if (this.addedSpaces.TryGetValue(physicalObjectInstance, out spaces))
                    spaces.Add(space);
                else
                {
                    spaces = new List<SpaceInstance>(1);
                    spaces.Add(space);
                    this.addedSpaces.Add(physicalObjectInstance, spaces);
                }
                physicalObjectInstance.IsModified = true;
                physicalObjectInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the added spaces of the physical object instance.
        /// </summary>
        /// <param name="physicalObjectInstance">The physical object instance to get the added spaces of.</param>
        /// <returns>The added spaces of the physical object instance.</returns>
        internal List<SpaceInstance> GetAddedSpaces(PhysicalObjectInstance physicalObjectInstance)
        {
            List<SpaceInstance> spaces = null;
            if (physicalObjectInstance != null)
                this.addedSpaces.TryGetValue(physicalObjectInstance, out spaces);
            if (spaces == null)
                spaces = new List<SpaceInstance>();
            return spaces;
        }
        #endregion Method: HandleSpaceAdded(PhysicalObjectInstance physicalObjectInstance, SpaceInstance space)

        #region Method: HandleSpaceRemoved(PhysicalObjectInstance physicalObjectInstance, SpaceInstance space)
        /// <summary>
        /// Handle the removal of a space.
        /// </summary>
        /// <param name="physicalObjectInstance">The instance the space was removed from.</param>
        /// <param name="space">The removed space.</param>
        internal void HandleSpaceRemoved(PhysicalObjectInstance physicalObjectInstance, SpaceInstance space)
        {
            if (physicalObjectInstance != null && space != null)
            {
                List<SpaceInstance> spaces;
                if (this.removedSpaces.TryGetValue(physicalObjectInstance, out spaces))
                    spaces.Add(space);
                else
                {
                    spaces = new List<SpaceInstance>(1);
                    spaces.Add(space);
                    this.removedSpaces.Add(physicalObjectInstance, spaces);
                }
                physicalObjectInstance.IsModified = true;
                physicalObjectInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the removed spaces of the physical object instance.
        /// </summary>
        /// <param name="physicalObjectInstance">The physical object instance to get the removed spaces of.</param>
        /// <returns>The removed spaces of the physical object instance.</returns>
        internal List<SpaceInstance> GetRemovedSpaces(PhysicalObjectInstance physicalObjectInstance)
        {
            List<SpaceInstance> spaces = null;
            if (physicalObjectInstance != null)
                this.removedSpaces.TryGetValue(physicalObjectInstance, out spaces);
            if (spaces == null)
                spaces = new List<SpaceInstance>();
            return spaces;
        }
        #endregion Method: HandleSpaceRemoved(PhysicalObjectInstance physicalObjectInstance, SpaceInstance space)

        #region Method: HandleSubstanceAdded(CompoundInstance compoundInstance, SubstanceInstance substance)
        /// <summary>
        /// Handle the addition of a substance.
        /// </summary>
        /// <param name="compoundInstance">The instance the substance was added to.</param>
        /// <param name="substance">The added substance.</param>
        internal void HandleSubstanceAdded(CompoundInstance compoundInstance, SubstanceInstance substance)
        {
            if (compoundInstance != null && substance != null)
            {
                List<SubstanceInstance> substances;
                if (this.addedSubstances.TryGetValue(compoundInstance, out substances))
                    substances.Add(substance);
                else
                {
                    substances = new List<SubstanceInstance>(1);
                    substances.Add(substance);
                    this.addedSubstances.Add(compoundInstance, substances);
                }
                compoundInstance.IsModified = true;
                compoundInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the added substances of the compound instance.
        /// </summary>
        /// <param name="compoundInstance">The compound instance to get the added substances of.</param>
        /// <returns>The added substances of the compound instance.</returns>
        internal List<SubstanceInstance> GetAddedSubstances(CompoundInstance compoundInstance)
        {
            List<SubstanceInstance> substances = null;
            if (compoundInstance != null)
                this.addedSubstances.TryGetValue(compoundInstance, out substances);
            if (substances == null)
                substances = new List<SubstanceInstance>();
            return substances;
        }
        #endregion Method: HandleSubstanceAdded(CompoundInstance compoundInstance, SubstanceInstance substance)

        #region Method: HandleSubstanceRemoved(CompoundInstance compoundInstance, SubstanceInstance substance)
        /// <summary>
        /// Handle the removal of a substance.
        /// </summary>
        /// <param name="compoundInstance">The instance the substance was removed from.</param>
        /// <param name="substance">The removed substance.</param>
        internal void HandleSubstanceRemoved(CompoundInstance compoundInstance, SubstanceInstance substance)
        {
            if (compoundInstance != null && substance != null)
            {
                List<SubstanceInstance> substances;
                if (this.removedSubstances.TryGetValue(compoundInstance, out substances))
                    substances.Add(substance);
                else
                {
                    substances = new List<SubstanceInstance>(1);
                    substances.Add(substance);
                    this.removedSubstances.Add(compoundInstance, substances);
                }
                compoundInstance.IsModified = true;
                compoundInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the removed substances of the compound instance.
        /// </summary>
        /// <param name="compoundInstance">The compound instance to get the removed substances of.</param>
        /// <returns>The removed substances of the compound instance.</returns>
        internal List<SubstanceInstance> GetRemovedSubstances(CompoundInstance compoundInstance)
        {
            List<SubstanceInstance> substances = null;
            if (compoundInstance != null)
                this.removedSubstances.TryGetValue(compoundInstance, out substances);
            if (substances == null)
                substances = new List<SubstanceInstance>();
            return substances;
        }
        #endregion Method: HandleSubstanceRemoved(CompoundInstance compoundInstance, SubstanceInstance substance)

        #region Method: HandleSubstanceAdded(MixtureInstance mixtureInstance, SubstanceInstance substance)
        /// <summary>
        /// Handle the addition of a substance.
        /// </summary>
        /// <param name="mixtureInstance">The instance the substance was added to.</param>
        /// <param name="substance">The added substance.</param>
        internal void HandleSubstanceAdded(MixtureInstance mixtureInstance, SubstanceInstance substance)
        {
            if (mixtureInstance != null && substance != null)
            {
                List<SubstanceInstance> substances;
                if (this.addedSubstances2.TryGetValue(mixtureInstance, out substances))
                    substances.Add(substance);
                else
                {
                    substances = new List<SubstanceInstance>(1);
                    substances.Add(substance);
                    this.addedSubstances2.Add(mixtureInstance, substances);
                }
                mixtureInstance.IsModified = true;
                mixtureInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the added substances of the mixture instance.
        /// </summary>
        /// <param name="mixtureInstance">The mixture instance to get the added substances of.</param>
        /// <returns>The added substances of the mixture instance.</returns>
        internal List<SubstanceInstance> GetAddedSubstances(MixtureInstance mixtureInstance)
        {
            List<SubstanceInstance> substances = null;
            if (mixtureInstance != null)
                this.addedSubstances2.TryGetValue(mixtureInstance, out substances);
            if (substances == null)
                substances = new List<SubstanceInstance>();
            return substances;
        }
        #endregion Method: HandleSubstanceAdded(MixtureInstance mixtureInstance, SubstanceInstance substance)

        #region Method: HandleSubstanceRemoved(MixtureInstance mixtureInstance, SubstanceInstance substance)
        /// <summary>
        /// Handle the removal of a substance.
        /// </summary>
        /// <param name="mixtureInstance">The instance the substance was removed from.</param>
        /// <param name="substance">The removed substance.</param>
        internal void HandleSubstanceRemoved(MixtureInstance mixtureInstance, SubstanceInstance substance)
        {
            if (mixtureInstance != null && substance != null)
            {
                List<SubstanceInstance> substances;
                if (this.removedSubstances2.TryGetValue(mixtureInstance, out substances))
                    substances.Add(substance);
                else
                {
                    substances = new List<SubstanceInstance>(1);
                    substances.Add(substance);
                    this.removedSubstances2.Add(mixtureInstance, substances);
                }
                mixtureInstance.IsModified = true;
                mixtureInstance.addedOrRemoved = true;
            }
        }

        /// <summary>
        /// Get the removed substances of the mixture instance.
        /// </summary>
        /// <param name="mixtureInstance">The mixture instance to get the removed substances of.</param>
        /// <returns>The removed substances of the mixture instance.</returns>
        internal List<SubstanceInstance> GetRemovedSubstances(MixtureInstance mixtureInstance)
        {
            List<SubstanceInstance> substances = null;
            if (mixtureInstance != null)
                this.removedSubstances2.TryGetValue(mixtureInstance, out substances);
            if (substances == null)
                substances = new List<SubstanceInstance>();
            return substances;
        }
        #endregion Method: HandleSubstanceRemoved(MixtureInstance mixtureInstance, SubstanceInstance substance)

        #endregion Method Group: Add/Remove handling

        #region Method Group: Other

        #region Method: GetRequiredInput(EntityInstance actor, ActionBase action, EntityInstance target)
        /// <summary>
        /// Gets the input that is required in order for the actor to be able to execute the action, possibly on a target.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="action">The action that the actor should perform.</param>
        /// <param name="target">The (optional) target.</param>
        /// <returns>A dictionary with the names of required variables as keys, and the value type of the required input as keys.</returns>
        public Dictionary<string, ValueType> GetRequiredInput(EntityInstance actor, ActionBase action, EntityInstance target)
        {
            Dictionary<string, ValueType> dictionary = new Dictionary<string, ValueType>();

            if (actor != null && action != null)
            {
                // Get the correct event by checking whether the actor, action, and target match
                foreach (EventBase eventBase in actor.ManualEvents)
                {
                    if (action.Equals(eventBase.Action))
                    {
                        if (target == null || target.IsNodeOf(eventBase.Target))
                        {
                            // Get the variables that require manual input
                            foreach (VariableBase variable in eventBase.Variables)
                            {
                                if (variable.VariableType == VariableType.RequiresManualInput)
                                {
                                    if (variable is NumericalVariableBase)
                                        dictionary.Add(variable.Name, ValueType.Numerical);
                                    else if (variable is VectorVariableBase)
                                        dictionary.Add(variable.Name, ValueType.Vector);
                                    else if (variable is BoolVariableBase)
                                        dictionary.Add(variable.Name, ValueType.Boolean);
                                    else if (variable is StringVariableBase)
                                        dictionary.Add(variable.Name, ValueType.String);
                                }
                            }
                        }
                    }
                }
            }

            return dictionary;
        }
        #endregion Method: GetRequiredInput(EntityInstance actor, ActionBase action, EntityInstance target)

        #region Method: GetManualInput(string variableName, ValueType valueType)
        /// <summary>
        /// Request manual input for the given variable name and value type.
        /// </summary>
        /// <param name="variableName">The name of the variable.</param>
        /// <param name="valueType">The required value type.</param>
        /// <returns>Returns the manual input for the variable name of the given value type.</returns>
        internal object GetManualInput(string variableName, ValueType valueType)
        {
            if (variableName != null && InputHandler != null)
                return InputHandler.GetManualInput(variableName, valueType);
            return null;
        }
        #endregion Method: GetManualInput(string variableName, ValueType valueType)

        #region Method: IsAllowedToPerformAction(EntityInstance actor, EntityInstance target)
        /// <summary>
        /// Check whether the actor is allowed to perform an action on the target.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="target">The target.</param>
        /// <returns>Indicates whether the actor is allowed to perform an action on the target.</returns>
        internal bool IsAllowedToPerformAction(EntityInstance actor, EntityInstance target)
        {
            // Actor and target should be in the same world
            if (actor != null && target != null && actor.World != null && !actor.World.Equals(target.World))
                return false;

            // Tangible actors are not allowed to perform actions on items in closed spaces that are not theirs,
            // unless they are in the same space
            TangibleObjectInstance tangibleActor = actor as TangibleObjectInstance;
            TangibleObjectInstance tangibleTarget = target as TangibleObjectInstance;
            if (tangibleActor != null && tangibleTarget != null)
            {
                if (tangibleTarget.Space != null && tangibleTarget.Space.SpaceType == SpaceType.Closed &&
                    !tangibleTarget.Space.Equals(tangibleActor.Space))
                {
                    PhysicalObjectInstance owner = tangibleTarget.Space.PhysicalObject;
                    while (owner != null && !owner.Equals(tangibleActor))
                    {
                        if (owner is TangibleObjectInstance)
                            owner = ((TangibleObjectInstance)owner).Space;
                        if (owner is SpaceInstance)
                            owner = ((SpaceInstance)owner).PhysicalObject;
                    }
                    if (owner == null)
                        return false;
                }
            }
            return true;
        }
        #endregion Method: IsAllowedToPerformAction(EntityInstance actor, EntityInstance target)
		
        #endregion Method Group: Other

    }
    #endregion Class: SemanticsEngine

}