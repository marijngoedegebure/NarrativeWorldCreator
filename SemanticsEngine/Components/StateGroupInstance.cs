/**************************************************************************
 * 
 * StateGroupInstance.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Semantics.Abstractions;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;

namespace SemanticsEngine.Components
{

    #region Class: StateGroupInstance
    /// <summary>
    /// An instance of a state group.
    /// </summary>
    public sealed class StateGroupInstance : Instance
    {

        #region Properties and Fields

        #region Property: StateGroupBase
        /// <summary>
        /// Gets the state group base of which this is a state group instance.
        /// </summary>
        public StateGroupBase StateGroupBase
        {
            get
            {
                return this.Base as StateGroupBase;
            }
        }
        #endregion Property: StateGroupBase
		
        #region Property: States
        /// <summary>
        /// Gets the states in this state group.
        /// </summary>
        public ReadOnlyCollection<StateBase> States
        {
            get
            {
                if (this.StateGroupBase != null)
                    return this.StateGroupBase.States;
                return new List<StateBase>(0).AsReadOnly();
            }
        }
        #endregion Property: States

        #region Property: State
        /// <summary>
        /// Gets or sets the selected state of the group.
        /// </summary>
        public StateBase State
        {
            get
            {
                if (this.StateGroupBase != null)
                    return GetProperty<StateBase>("State", this.StateGroupBase.State);
                
                return null;
            }
            set
            {
                if (this.State != value && this.States.Contains((StateBase)value))
                    SetProperty("State", value);
            }
        }
        #endregion Property: State

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: StateGroupInstance(StateGroupBase stateGroupBase)
        /// <summary>
        /// Creates a new state group instance from the given state group base.
        /// </summary>
        /// <param name="stateGroupBase">The state group base to create the state group instance from.</param>
        internal StateGroupInstance(StateGroupBase stateGroupBase)
            : base(stateGroupBase)
        {
        }
        #endregion Constructor: StateGroupInstance(StateGroupBase stateGroupBase)

        #region Constructor: StateGroupInstance(StateGroupInstance stateGroupInstance)
        /// <summary>
        /// Clones a state group instance.
        /// </summary>
        /// <param name="stateGroupInstance">The state group instance to clone.</param>
        internal StateGroupInstance(StateGroupInstance stateGroupInstance)
            : base(stateGroupInstance)
        {
            if (stateGroupInstance != null)
                this.State = stateGroupInstance.State;
        }
        #endregion Constructor: StateGroupInstance(StateGroupInstance stateGroupInstance)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Satisfies(State state)
        /// <summary>
        /// Check whether the given state satisfies the state group instance.
        /// </summary>
        /// <param name="state">The state to compare to the state group instance.</param>
        /// <returns>Returns whether the state satisfies the state group instance.</returns>
        internal bool Satisfies(State state)
        {
            if (state != null)
            {
                // Check whether the state is satisfied
                return Toolbox.Compare(this.State, EqualitySign.Equal, state);
            }
            return false;
        }
        #endregion Method: Satisfies(State state)

        #region Method: Satisfies(StateGroupConditionBase stateGroupConditionBase)
        /// <summary>
        /// Check whether the given condition satisfies the state group instance.
        /// </summary>
        /// <param name="stateGroupConditionBase">The state group condition to compare to the state group instance.</param>
        /// <returns>Returns whether the condition satisfies the state group instance.</returns>
        public bool Satisfies(StateGroupConditionBase stateGroupConditionBase)
        {
            if (stateGroupConditionBase != null)
            {
                // Check whether the state is satisfied
                if (stateGroupConditionBase.StateSign == null || stateGroupConditionBase.State == null || Toolbox.Compare(this.State, (EqualitySign)stateGroupConditionBase.StateSign, stateGroupConditionBase.State))
                {
                    // Check whether the required states are there
                    foreach (StateBase stateBase in stateGroupConditionBase.States)
                    {
                        if (!this.States.Contains(stateBase))
                            return false;
                    }
                    return true;
                }
            }
            return false;
        }
        #endregion Method: Satisfies(StateGroupConditionBase stateGroupConditionBase)

        #region Method: Apply(StateGroupChangeBase stateGroupChangeBase)
        /// <summary>
        /// Apply the given change to the state group instance.
        /// </summary>
        /// <param name="stateGroupChangeBase">The change to apply to the state group instance.</param>
        internal bool Apply(StateGroupChangeBase stateGroupChangeBase)
        {
            if (stateGroupChangeBase != null)
            {
                // Change the state
                if (stateGroupChangeBase.State != null && this.States.Contains(stateGroupChangeBase.State))
                {
                    SetProperty("State", stateGroupChangeBase.State);
                    return true;
                }
            }
            return false;
        }
        #endregion Method: Apply(StateGroupChangeBase stateGroupChangeBase)

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            string returnString = string.Empty;

            if (this.State != null)
                returnString += this.State.DefaultName + " ";
            returnString += "{";
            ReadOnlyCollection<StateBase> states = this.States;
            for (int i = 0; i < states.Count; i++)
            {
                if (i != 0)
                    returnString += ", ";
                returnString += states[i].DefaultName;
            }
            returnString += "}";

            return returnString;
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: StateGroupInstance

}