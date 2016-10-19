/**************************************************************************
 * 
 * Action.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Common;
using Semantics.Components;
using Semantics.Data;
using Semantics.Utilities;

namespace Semantics.Abstractions
{

    #region Class: Action
    /// <summary>
    /// An action with events.
    /// </summary>
    public class Action : Abstraction, IComparable<Action>
    {

        #region Properties and Fields

        #region Property: Events
        /// <summary>
        /// Gets the personal and inherited events.
        /// </summary>
        public ReadOnlyCollection<Event> Events
        {
            get
            {
                List<Event> events = new List<Event>();
                events.AddRange(this.PersonalEvents);
                events.AddRange(this.InheritedEvents);
                return events.AsReadOnly();
            }
        }
        #endregion Property: Events

        #region Property: PersonalEvents
        /// <summary>
        /// Gets all personal events of the action.
        /// </summary>
        public ReadOnlyCollection<Event> PersonalEvents
        {
            get
            {
                return Database.Current.SelectAll<Event>(GenericTables.Event, Columns.Action, this).AsReadOnly();
            }
        }
        #endregion Property: PersonalEvents

        #region Property: InheritedEvents
        /// <summary>
        /// Gets the inherited events.
        /// </summary>
        public ReadOnlyCollection<Event> InheritedEvents
        {
            get
            {
                List<Event> inheritedEvents = new List<Event>();
                foreach (Node parent in this.PersonalParents)
                    inheritedEvents.AddRange(((Action)parent).Events);
                return inheritedEvents.AsReadOnly();
            }
        }
        #endregion Property: InheritedEvents

        #region Property: AutomaticEvents
        /// <summary>
        /// Gets the automatic events of the action.
        /// </summary>
        public ReadOnlyCollection<Event> AutomaticEvents
        {
            get
            {
                return Database.Current.SelectAll<Event>(GenericTables.Event, new string[] {Columns.Action, Columns.Behavior }, new object[] { this, EventBehavior.Automatic }).AsReadOnly();
            }
        }
        #endregion Property: AutomaticEvents

        #region Property: ManualEvents
        /// <summary>
        /// Gets the manual events of the action.
        /// </summary>
        public ReadOnlyCollection<Event> ManualEvents
        {
            get
            {
                return Database.Current.SelectAll<Event>(GenericTables.Event, new string[] { Columns.Action, Columns.Behavior }, new object[] { this, EventBehavior.Manual }).AsReadOnly();
            }
        }
        #endregion Property: ManualEvents

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: Action()
        /// <summary>
        /// Creates an action.
        /// </summary>
        public Action()
            : base()
        {
        }
        #endregion Constructor: Action()

        #region Constructor: Action(uint id)
        /// <summary>
        /// Creates a new action from the given ID.
        /// </summary>
        /// <param name="id">The ID to create an action from.</param>
        protected Action(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Action(uint id)

        #region Constructor: Action(string name)
        /// <summary>
        /// Creates a new action with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the action.</param>
        public Action(string name)
            : base(name)
        {
        }
        #endregion Constructor: Action(string name)

        #region Constructor: Action(Action action)
        /// <summary>
        /// Clones an action.
        /// </summary>
        /// <param name="action">The action to clone.</param>
        public Action(Action action)
            : base(action)
        {
            if (action != null)
            {
                Database.Current.StartChange();

                foreach (Event e in action.PersonalEvents)
                    AddEvent(new Event(e));

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Action(Action action)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddEvent(Event e)
        /// <summary>
        /// Add an event to the action.
        /// </summary>
        /// <param name="e">The event to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        internal Message AddEvent(Event e)
        {
            if (e != null)
            {
                NotifyPropertyChanged("ManualEvents");
                NotifyPropertyChanged("AutomaticEvents");
                NotifyPropertyChanged("PersonalEvents");
                NotifyPropertyChanged("Events");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddEvent(Event e)

        #region Method: RemoveEvent(Event e)
        /// <summary>
        /// Remove an event from the action.
        /// </summary>
        /// <param name="e">The event to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        internal Message RemoveEvent(Event e)
        {
            if (e != null)
            {
                NotifyPropertyChanged("ManualEvents");
                NotifyPropertyChanged("AutomaticEvents");
                NotifyPropertyChanged("PersonalEvents");
                NotifyPropertyChanged("Events");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveEvent(Event e)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasEvent(Event even)
        /// <summary>
        /// Checks whether the action has the given event.
        /// </summary>
        /// <param name="even">The event to check.</param>
        /// <returns>Returns true when the action has the event.</returns>
        public bool HasEvent(Event even)
        {
            if (even != null)
            {
                foreach (Event myEvent in this.Events)
                {
                    if (even.Equals(myEvent))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasEvent(Event even)

        #region Method: Remove()
        /// <summary>
        /// Remove the action.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();
            Database.Current.StartRemove();

            // Remove the events
            foreach (Event e in this.PersonalEvents)
                e.Remove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(Action other)
        /// <summary>
        /// Compares the action to the other action.
        /// </summary>
        /// <param name="other">The action to compare to this action.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Action other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(Action other)

        #endregion Method Group: Other

    }
    #endregion Class: Action

}