/**************************************************************************
 * 
 * StateGroup.cs
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
using Semantics.Abstractions;
using Semantics.Data;
using Semantics.Utilities;

namespace Semantics.Components
{

    #region Class: StateGroup
    /// <summary>
    /// A state group.
    /// </summary>
    public sealed class StateGroup : IdHolder
    {

        #region Properties and Fields

        #region Property: States
        /// <summary>
        /// Gets the states in this state group.
        /// </summary>
        public ReadOnlyCollection<State> States
        {
            get
            {
                return Database.Current.SelectAll<State>(this.ID, GenericTables.StateGroupState, Columns.State).AsReadOnly();
            }
        }
        #endregion Property: States

        #region Property: DefaultState
        /// <summary>
        /// Gets or sets the default state of the state group. Setting only works when the state group has the state.
        /// </summary>
        public State DefaultState
        {
            get
            {
                return Database.Current.Select<State>(this.ID, GenericTables.StateGroup, Columns.DefaultState);
            }
            set
            {
                if (HasState((State)value))
                {
                    Database.Current.Update(this.ID, GenericTables.StateGroup, Columns.DefaultState, value);
                    NotifyPropertyChanged("DefaultState");
                }
            }
        }
        #endregion Property: DefaultState

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: StateGroup()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static StateGroup()
        {
            // States
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.State, new Tuple<Type, EntryType>(typeof(State), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.StateGroupState, typeof(StateGroup), dict);

            // Default state
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.DefaultState, new Tuple<Type, EntryType>(typeof(State), EntryType.Nullable));
            Database.Current.AddTableDefinition(GenericTables.StateGroup, typeof(StateGroup), dict);
        }
        #endregion Static Constructor: StateGroup()

        #region Constructor: StateGroup()
        /// <summary>
        /// Creates a new state group.
        /// </summary>
        public StateGroup()
            : base()
        {
        }
        #endregion Constructor: StateGroup()

        #region Constructor: StateGroup(uint id)
        /// <summary>
        /// Creates a new state group from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a state group from.</param>
        private StateGroup(uint id)
            : base(id)
        {
        }
        #endregion Constructor: StateGroup(uint id)

        #region Constructor: StateGroup(StateGroup stateGroup)
        /// <summary>
        /// Clones a state group.
        /// </summary>
        /// <param name="stateGroup">The state group to clone.</param>
        public StateGroup(StateGroup stateGroup)
            : base()
        {
            if (stateGroup != null)
            {
                Database.Current.StartChange();

                foreach (State state in stateGroup.States)
                    AddState(state);
                this.DefaultState = stateGroup.DefaultState;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: StateGroup(StateGroup stateGroup)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddState(State state)
        /// <summary>
        /// Adds a state to the state group.
        /// </summary>
        /// <param name="state">The state to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddState(State state)
        {
            if (state != null)
            {
                // If the state is already there, there's no use to add it again
                if (HasState(state))
                    return Message.RelationExistsAlready;

                // Add the state
                Database.Current.Insert(this.ID, GenericTables.StateGroupState, Columns.State, state);
                NotifyPropertyChanged("States");
                
                // If no default state has been set, assign this one to it
                if (this.DefaultState == null)
                    this.DefaultState = state;

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddState(State state)

        #region Method: RemoveState(State state)
        /// <summary>
        /// Removes a state from the state group.
        /// </summary>
        /// <param name="state">The state to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveState(State state)
        {
            if (state != null)
            {
                if (HasState(state))
                {
                    // Remove the state
                    Database.Current.Remove(this.ID, GenericTables.StateGroupState, Columns.State, state);
                    NotifyPropertyChanged("States");

                    // If this was the default state, pick another one
                    if (state.Equals(this.DefaultState) && this.States.Count > 0)
                        this.DefaultState = this.States[0];

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveState(State state)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasState(State state)
        /// <summary>
        /// Checks if this state group has the given state.
        /// </summary>
        /// <param name="state">The state to check.</param>
        /// <returns>Returns true when this state group has the state.</returns>
        public bool HasState(State state)
        {
            if (state != null)
            {
                foreach (State myState in this.States)
                {
                    if (state.Equals(myState))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasState(State state)

        #region Method: Remove()
        /// <summary>
        /// Remove the state group.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the states
            Database.Current.Remove(this.ID, GenericTables.StateGroupState);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: StateGroup

    #region Class: StateGroupCondition
    /// <summary>
    /// A condition on a state group.
    /// </summary>
    public sealed class StateGroupCondition : Condition
    {

        #region Properties and Fields

        #region Property: States
        /// <summary>
        /// Gets the required states.
        /// </summary>
        public ReadOnlyCollection<State> States
        {
            get
            {
                return Database.Current.SelectAll<State>(this.ID, ValueTables.StateGroupConditionState, Columns.State).AsReadOnly();
            }
        }
        #endregion Property: States

        #region Property: State
        /// <summary>
        /// Gets or sets the required state.
        /// </summary>
        public State State
        {
            get
            {
                return Database.Current.Select<State>(this.ID, ValueTables.StateGroupCondition, Columns.State);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.StateGroupCondition, Columns.State, value);
                NotifyPropertyChanged("State");
            }
        }
        #endregion Property: State

        #region Property: StateSign
        /// <summary>
        /// Gets or sets the equality sign of the state in the condition.
        /// </summary>
        public EqualitySign? StateSign
        {
            get
            {
                return Database.Current.Select<EqualitySign?>(this.ID, ValueTables.StateGroupCondition, Columns.StateSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.StateGroupCondition, Columns.StateSign, value);
                NotifyPropertyChanged("StateSign");
            }
        }
        #endregion Property: StateSign

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: StateGroupCondition()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static StateGroupCondition()
        {
            // States
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.State, new Tuple<Type, EntryType>(typeof(State), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.StateGroupConditionState, typeof(StateGroupCondition), dict);

            // State
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.State, new Tuple<Type, EntryType>(typeof(State), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.StateGroupCondition, typeof(StateGroupCondition), dict);
        }
        #endregion Static Constructor: StateGroupCondition()

        #region Constructor: StateGroupCondition()
        /// <summary>
        /// Creates a new state group condition.
        /// </summary>
        public StateGroupCondition()
            : base()
        {
            Database.Current.StartChange();

            this.StateSign = EqualitySign.Equal;

            Database.Current.StopChange();
        }
        #endregion Constructor: StateGroupCondition()

        #region Constructor: StateGroupCondition(uint id)
        /// <summary>
        /// Creates a new state group condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a state group condition from.</param>
        internal StateGroupCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: StateGroupCondition(uint id)

        #region Constructor: StateGroupCondition(StateGroupCondition stateGroupCondition)
        /// <summary>
        /// Clones a state group condition.
        /// </summary>
        /// <param name="stateGroupCondition">The state group condition to clone.</param>
        public StateGroupCondition(StateGroupCondition stateGroupCondition)
            : base(stateGroupCondition)
        {
            if (stateGroupCondition != null)
            {
                Database.Current.StartChange();

                foreach (State state in stateGroupCondition.States)
                    AddState(state);
                this.State = stateGroupCondition.State;
                this.StateSign = stateGroupCondition.StateSign;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: StateGroupCondition(StateGroupCondition stateGroupCondition)

        #region Constructor: StateGroupCondition(State state, EqualitySign stateSign)
        /// <summary>
        /// Creates a new state group condition with the given required state.
        /// </summary>
        /// <param name="state">The required state.</param>
        /// <param name="stateSign">The sign for the state.</param>
        public StateGroupCondition(State state, EqualitySign stateSign)
            : base()
        {
            Database.Current.StartChange();

            this.State = state;
            this.StateSign = stateSign;

            Database.Current.StopChange();
        }
        #endregion Constructor: StateGroupCondition(State state, EqualitySign stateSign)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddState(State state)
        /// <summary>
        /// Adds a state to the state group condition.
        /// </summary>
        /// <param name="state">The state to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddState(State state)
        {
            if (state != null)
            {
                // If the state is already there, there's no use to add it again
                if (HasState(state))
                    return Message.RelationExistsAlready;

                // Add the state
                Database.Current.Insert(this.ID, ValueTables.StateGroupConditionState, Columns.State, state);
                NotifyPropertyChanged("States");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddState(State state)

        #region Method: RemoveState(State state)
        /// <summary>
        /// Removes a state from the state group condition.
        /// </summary>
        /// <param name="state">The state to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveState(State state)
        {
            if (state != null)
            {
                if (HasState(state))
                {
                    // Remove the state
                    Database.Current.Remove(this.ID, ValueTables.StateGroupConditionState, Columns.State, state);
                    NotifyPropertyChanged("States");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveState(State state)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasState(State state)
        /// <summary>
        /// Checks if this state group condition has the given state.
        /// </summary>
        /// <param name="state">The state to check.</param>
        /// <returns>Returns true when this state group condition has the state.</returns>
        public bool HasState(State state)
        {
            if (state != null)
            {
                foreach (State myState in this.States)
                {
                    if (state.Equals(myState))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasState(State state)

        #region Method: Remove()
        /// <summary>
        /// Remove the state group condition.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the states
            Database.Current.Remove(this.ID, ValueTables.StateGroupConditionState);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: StateGroupCondition

    #region Class: StateGroupChange
    /// <summary>
    /// A change on a state group.
    /// </summary>
    public sealed class StateGroupChange : Change
    {

        #region Properties and Fields

        #region Property: State
        /// <summary>
        /// Gets or sets the state to change to.
        /// </summary>
        public State State
        {
            get
            {
                return Database.Current.Select<State>(this.ID, ValueTables.StateGroupChange, Columns.State);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.StateGroupChange, Columns.State, value);
                NotifyPropertyChanged("State");
            }
        }
        #endregion Property: State

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: StateGroupChange()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static StateGroupChange()
        {
            // State
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.State, new Tuple<Type, EntryType>(typeof(State), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.StateGroupChange, typeof(StateGroupChange), dict);
        }
        #endregion Static Constructor: StateGroupChange()

        #region Constructor: StateGroupChange()
        /// <summary>
        /// Creates a new state group change.
        /// </summary>
        public StateGroupChange()
            : base()
        {
        }
        #endregion Constructor: StateGroupChange()

        #region Constructor: StateGroupChange(uint id)
        /// <summary>
        /// Creates a new state group change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a state group change from.</param>
        internal StateGroupChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: StateGroupChange(uint id)

        #region Constructor: StateGroupChange(StateGroupChange stateGroupChange)
        /// <summary>
        /// Clones a state group change.
        /// </summary>
        /// <param name="stateChange">The state group change to clone.</param>
        public StateGroupChange(StateGroupChange stateGroupChange)
            : base(stateGroupChange)
        {
            if (stateGroupChange != null)
            {
                Database.Current.StartChange();

                this.State = stateGroupChange.State;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: StateGroupChange(StateGroupChange stateGroupChange)

        #region Constructor: StateGroupChange(State state)
        /// <summary>
        /// Creates a change for the given state group.
        /// </summary>
        /// <param name="state">The state to change to.</param>
        public StateGroupChange(State state)
            : base()
        {
            Database.Current.StartChange();

            this.State = state;

            Database.Current.StopChange();
        }
        #endregion Constructor: StateGroupChange(State state)

        #endregion Method Group: Constructors

    }
    #endregion Class: StateGroupChange

}