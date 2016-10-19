/**************************************************************************
 * 
 * Event.cs
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
using Semantics.Data;
using Semantics.Entities;
using Semantics.Interfaces;
using Semantics.Utilities;
using Action = Semantics.Abstractions.Action;

namespace Semantics.Components
{

    #region Class: Event
    /// <summary>
    /// An event is an action performed by an actor, possibly on a target with an artifact.
    /// </summary>
    public sealed class Event : Service, IVariableReferenceHolder
    {

        #region Properties and Fields

        #region Property: Actor
        /// <summary>
        /// Gets or sets the actor of the event.
        /// </summary>
        public Entity Actor
        {
            get
            {
                return Database.Current.Select<Entity>(this.ID, GenericTables.Event, Columns.Actor);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Event, Columns.Actor, value);
                NotifyPropertyChanged("Actor");
            }
        }
        #endregion Property: Actor

        #region Property: Action
        /// <summary>
        /// Gets the action of the event.
        /// </summary>
        public Action Action
        {
            get
            {
                return Database.Current.Select<Action>(this.ID, GenericTables.Event, Columns.Action);
            }
            private set
            {
                Database.Current.Update(this.ID, GenericTables.Event, Columns.Action, value);
                NotifyPropertyChanged("Action");
            }
        }
        #endregion Property: Action

        #region Property: Target
        /// <summary>
        /// Gets or sets the target of the event.
        /// </summary>
        public Entity Target
        {
            get
            {
                return Database.Current.Select<Entity>(this.ID, GenericTables.Event, Columns.Target);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Event, Columns.Target, value);
                NotifyPropertyChanged("Target");
            }
        }
        #endregion Property: Target

        #region Property: Artifact
        /// <summary>
        /// Gets or sets the artifact of the event.
        /// </summary>
        public Entity Artifact
        {
            get
            {
                return Database.Current.Select<Entity>(this.ID, GenericTables.Event, Columns.Artifact);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Event, Columns.Artifact, value);
                NotifyPropertyChanged("Artifact");
            }
        }
        #endregion Property: Artifact

        #region Property: Requirements
        /// <summary>
        /// Gets or sets the requirements of the event.
        /// </summary>
        public EventRequirements Requirements
        {
            get
            {
                return Database.Current.Select<EventRequirements>(this.ID, GenericTables.Event, Columns.Requirements);
            }
            private set
            {
                Database.Current.Update(this.ID, GenericTables.Event, Columns.Requirements, value);
                NotifyPropertyChanged("Requirements");
            }
        }
        #endregion Property: Requirements

        #region Property: Effects
        /// <summary>
        /// Gets the effects of the event.
        /// </summary>
        public ReadOnlyCollection<Effect> Effects
        {
            get
            {
                return Database.Current.SelectAll<Effect>(this.ID, GenericTables.EventEffect, Columns.Effect).AsReadOnly();
            }
        }
        #endregion Property: Effects

        #region Property: Time
        /// <summary>
        /// Gets the time of the event.
        /// </summary>
        public Time Time
        {
            get
            {
                return Database.Current.Select<Time>(this.ID, GenericTables.Event, Columns.Time);
            }
            private set
            {
                Database.Current.Update(this.ID, GenericTables.Event, Columns.Time, value);
                NotifyPropertyChanged("Time");
            }
        }
        #endregion Property: Time

        #region Property: LevelOfDetail
        /// <summary>
        /// Gets or sets the value that indicates the required amount of detail before this event can be triggered.
        /// </summary>
        public byte LevelOfDetail
        {
            get
            {
                return Database.Current.Select<byte>(this.ID, GenericTables.Event, Columns.LevelOfDetail);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Event, Columns.LevelOfDetail, value);
                NotifyPropertyChanged("LevelOfDetail");
            }
        }
        #endregion Property: LevelOfDetail

        #region Property: Behavior
        /// <summary>
        /// Gets or sets the value that indicates when the event should be executed.
        /// </summary>
        public EventBehavior Behavior
        {
            get
            {
                return Database.Current.Select<EventBehavior>(this.ID, GenericTables.Event, Columns.Behavior);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Event, Columns.Behavior, value);
                NotifyPropertyChanged("Behavior");
            }
        }
        #endregion Property: Behavior		

        #region Property: NrOfSimultaneousUses
        /// <summary>
        /// Gets or sets the value that indicates the maximum number of times this event can be executed simultaneously.
        /// </summary>
        public int NrOfSimultaneousUses
        {
            get
            {
                return Database.Current.Select<int>(this.ID, GenericTables.Event, Columns.NrOfSimultaneousUses);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Event, Columns.NrOfSimultaneousUses, value);
                NotifyPropertyChanged("NrOfSimultaneousUses");
            }
        }
        #endregion Property: NrOfSimultaneousUses

        #region Property: Variables
        /// <summary>
        /// Gets the variables.
        /// </summary>
        public ReadOnlyCollection<Variable> Variables
        {
            get
            {
                return Database.Current.SelectAll<Variable>(this.ID, GenericTables.EventVariable, Columns.Variable).AsReadOnly();
            }
        }
        #endregion Property: Variables

        #region Property: References
        /// <summary>
        /// Gets the references.
        /// </summary>
        public ReadOnlyCollection<Reference> References
        {
            get
            {
                return Database.Current.SelectAll<Reference>(this.ID, GenericTables.EventReference, Columns.Reference).AsReadOnly();
            }
        }
        #endregion Property: References

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: Event()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static Event()
        {
            // Actor, action, target, artifact
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Actor, new Tuple<Type, EntryType>(typeof(Entity), EntryType.Nullable));
            dict.Add(Columns.Action, new Tuple<Type, EntryType>(typeof(Action), EntryType.Nullable));
            dict.Add(Columns.Target, new Tuple<Type, EntryType>(typeof(Entity), EntryType.Nullable));
            dict.Add(Columns.Artifact, new Tuple<Type, EntryType>(typeof(Entity), EntryType.Nullable));
            Database.Current.AddTableDefinition(GenericTables.Event, typeof(Event), dict);

            // Variables
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Variable, new Tuple<Type, EntryType>(typeof(Variable), EntryType.Intermediate));
            Database.Current.AddTableDefinition(GenericTables.EventVariable, typeof(Event), dict);

            // References
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Reference, new Tuple<Type, EntryType>(typeof(Reference), EntryType.Intermediate));
            Database.Current.AddTableDefinition(GenericTables.EventReference, typeof(Event), dict);
        }
        #endregion Static Constructor: Event()

        #region Constructor: Event(uint id)
        /// <summary>
        /// Creates a new event with the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new event from.</param>
        private Event(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Event(uint id)

        #region Constructor: Event(Event even)
        /// <summary>
        /// Clones the event.
        /// </summary>
        /// <param name="even">The event to clone.</param>
        public Event(Event even)
            : base()
        {
            if (even != null)
            {
                Database.Current.StartChange();

                this.Action = even.Action;
                this.Actor = even.Actor;
                this.Target = even.Target;
                this.Artifact = even.Artifact;
                this.Requirements = new EventRequirements(even.Requirements);
                foreach (Effect effect in even.Effects)
                    AddEffect(effect.Clone());
                if (even.Time != null)
                    this.Time = new Time(even.Time);
                this.LevelOfDetail = even.LevelOfDetail;
                this.Behavior = even.Behavior;
                this.NrOfSimultaneousUses = even.NrOfSimultaneousUses;
                foreach (Reference reference in even.References)
                    AddReference(reference.Clone());
                foreach (Variable variable in even.Variables)
                    AddVariable(variable.Clone());

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Event(Event even)

        #region Constructor: Event(Action action)
        /// <summary>
        /// Creates an event from the given action.
        /// </summary>
        /// <param name="action">The action to create an event from.</param>
        public Event(Action action)
            : this(action, EventBehavior.Automatic)
        {
        }
        #endregion Constructor: Event(Action action)

        #region Constructor: Event(Action action, EventBehavior behavior)
        /// <summary>
        /// Creates an event from the given action.
        /// </summary>
        /// <param name="action">The action to create an event from.</param>
        /// <param name="behavior">The behavior.</param>
        public Event(Action action, EventBehavior behavior)
            : base()
        {
            if (action != null)
            {
                Database.Current.StartChange();

                this.Requirements = new EventRequirements();
                this.Time = new Time();
                Database.Current.QueryBegin();
                this.LevelOfDetail = SemanticsSettings.Values.LevelOfDetail;
                this.NrOfSimultaneousUses = SemanticsSettings.Values.NrOfSimultaneousUses;
                this.Behavior = behavior;
                this.Action = action;
                Database.Current.QueryCommit();
                action.AddEvent(this);

                Database.Current.StopChange();
            }
            else
                Remove();
        }
        #endregion Constructor: Event(Action action, EventBehavior behavior)

        #region Constructor: Event(Action action, Entity actor)
        /// <summary>
        /// Creates an event from the given action for the given actor.
        /// </summary>
        /// <param name="action">The action to create an event from.</param>
        /// <param name="actor">The actor that should execute the event.</param>
        public Event(Action action, Entity actor)
            : this(action, actor, EventBehavior.Automatic)
        {
        }
        #endregion Constructor: Event(Action action, Entity actor)

        #region Constructor: Event(Action action, Entity actor, EventBehavior behavior)
        /// <summary>
        /// Creates an event from the given action for the given actor.
        /// </summary>
        /// <param name="action">The action to create an event from.</param>
        /// <param name="actor">The actor that should execute the event.</param>
        /// <param name="behavior">The behavior.</param>
        public Event(Action action, Entity actor, EventBehavior behavior)
            : this(action, behavior)
        {
            if (actor != null)
            {
                Database.Current.StartChange();

                this.Actor = actor;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Event(Action action, Entity actor, EventBehavior behavior)

        #region Constructor: Event(Action action, Entity actor, Entity target)
        /// <summary>
        /// Creates an event from the given action for the given actor on the given target.
        /// </summary>
        /// <param name="action">The action to create an event from.</param>
        /// <param name="actor">The actor that should execute the event.</param>
        /// <param name="target">The target on which the event is executed.</param>
        public Event(Action action, Entity actor, Entity target)
            : this(action, actor, EventBehavior.Automatic)
        {
            if (target != null)
            {
                Database.Current.StartChange();

                this.Target = target;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Event(Action action, Entity actor, Entity target)

        #region Constructor: Event(Action action, Entity actor, Entity target, EventBehavior behavior)
        /// <summary>
        /// Creates an event from the given action for the given actor on the given target.
        /// </summary>
        /// <param name="action">The action to create an event from.</param>
        /// <param name="actor">The actor that should execute the event.</param>
        /// <param name="target">The target on which the event is executed.</param>
        /// <param name="behavior">The behavior.</param>
        public Event(Action action, Entity actor, Entity target, EventBehavior behavior)
            : this(action, actor, behavior)
        {
            if (target != null)
            {
                Database.Current.StartChange();

                this.Target = target;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Event(Action action, Entity actor, Entity target, EventBehavior behavior)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddEffect(Effect effect)
        /// <summary>
        /// Adds the given effect.
        /// </summary>
        /// <param name="effect">The effect to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddEffect(Effect effect)
        {
            if (effect != null)
            {
                // If the effect is already available in all effects, there is no use to add it
                if (HasEffect(effect))
                    return Message.RelationExistsAlready;

                // Add the effect
                Database.Current.Insert(this.ID, GenericTables.EventEffect, Columns.Effect, effect);
                NotifyPropertyChanged("Effects");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddEffect(Effect effect)

        #region Method: RemoveEffect(Effect effect)
        /// <summary>
        /// Removes an effect.
        /// </summary>
        /// <param name="effect">The effect to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveEffect(Effect effect)
        {
            if (effect != null)
            {
                if (HasEffect(effect))
                {
                    // Remove the effect
                    Database.Current.Remove(this.ID, GenericTables.EventEffect, Columns.Effect, effect);
                    NotifyPropertyChanged("Effects");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveEffect(Effect effect)

        #region Method: AddVariable(Variable variable)
        /// <summary>
        /// Adds the given variable.
        /// </summary>
        /// <param name="variable">The variable to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddVariable(Variable variable)
        {
            if (variable != null)
            {
                // If the variable is already available in all variables, there is no use to add it
                if (HasVariable(variable))
                    return Message.RelationExistsAlready;

                // Add the variable
                Database.Current.Insert(this.ID, GenericTables.EventVariable, Columns.Variable, variable);
                NotifyPropertyChanged("Variables");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddVariable(Variable variable)

        #region Method: RemoveVariable(Variable variable)
        /// <summary>
        /// Removes an variable.
        /// </summary>
        /// <param name="variable">The variable to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveVariable(Variable variable)
        {
            if (variable != null)
            {
                if (HasVariable(variable))
                {
                    // Remove the variable
                    Database.Current.Remove(this.ID, GenericTables.EventVariable, Columns.Variable, variable);
                    NotifyPropertyChanged("Variables");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveVariable(Variable variable)

        #region Method: AddReference(Reference reference)
        /// <summary>
        /// Adds the given reference.
        /// </summary>
        /// <param name="reference">The reference to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddReference(Reference reference)
        {
            if (reference != null)
            {
                // If the reference is already available in all references, there is no use to add it
                if (HasReference(reference))
                    return Message.RelationExistsAlready;

                // Add the reference
                Database.Current.Insert(this.ID, GenericTables.EventReference, Columns.Reference, reference);
                NotifyPropertyChanged("References");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddReference(Reference reference)

        #region Method: RemoveReference(Reference reference)
        /// <summary>
        /// Removes an reference.
        /// </summary>
        /// <param name="reference">The reference to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveReference(Reference reference)
        {
            if (reference != null)
            {
                if (HasReference(reference))
                {
                    // Remove the reference
                    Database.Current.Remove(this.ID, GenericTables.EventReference, Columns.Reference, reference);
                    NotifyPropertyChanged("References");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveReference(Reference reference)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasEffect(Effect effect)
        /// <summary>
        /// Checks if this event has the given effect.
        /// </summary>
        /// <param name="effect">The effect to check.</param>
        /// <returns>Returns true when this event has the effect.</returns>
        public bool HasEffect(Effect effect)
        {
            if (effect != null)
            {
                foreach (Effect myEffect in this.Effects)
                {
                    if (effect.Equals(myEffect))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasEffect(Effect effect)

        #region Method: HasVariable(Variable variable)
        /// <summary>
        /// Checks if this event has the given variable.
        /// </summary>
        /// <param name="variable">The variable to check.</param>
        /// <returns>Returns true when this event has the variable.</returns>
        public bool HasVariable(Variable variable)
        {
            if (variable != null)
            {
                foreach (Variable myVariable in this.Variables)
                {
                    if (variable.Equals(myVariable))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasVariable(Variable variable)

        #region Method: HasReference(Reference reference)
        /// <summary>
        /// Checks if this event has the given reference.
        /// </summary>
        /// <param name="reference">The reference to check.</param>
        /// <returns>Returns true when this event has the reference.</returns>
        public bool HasReference(Reference reference)
        {
            if (reference != null)
            {
                foreach (Reference myReference in this.References)
                {
                    if (reference.Equals(myReference))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasReference(Reference reference)

        #region Method: Remove()
        /// <summary>
        /// Remove the event.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the requirements
            if (this.Requirements != null)
                this.Requirements.Remove();

            // Remove the effects
            foreach (Effect effect in this.Effects)
                effect.Remove();
            Database.Current.Remove(this.ID, GenericTables.EventEffect);

            // Remove the time
            if (this.Time != null)
                this.Time.Remove();

            // Remove the variables
            foreach (Variable variable in this.Variables)
                variable.Remove();
            Database.Current.Remove(this.ID, GenericTables.EventVariable);

            // Remove the references
            foreach (Reference reference in this.References)
                reference.Remove();
            Database.Current.Remove(this.ID, GenericTables.EventReference);

            Action action = this.Action;

            base.Remove();

            // Remove the event from the action
            if (action != null)
                action.RemoveEvent(this);

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

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
    #endregion Class: Event

}
