/**************************************************************************
 * 
 * StateGroupBase.cs
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
using Semantics.Components;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: StateGroupBase
    /// <summary>
    /// A base of a state group.
    /// </summary>
    public sealed class StateGroupBase : Base
    {

        #region Properties and Fields

        #region Property: StateGroup
        /// <summary>
        /// Gets the state group of which this is a base.
        /// </summary>
        internal StateGroup StateGroup
        {
            get
            {
                return this.IdHolder as StateGroup;
            }
        }
        #endregion Property: StateGroup

        #region Property: States
        /// <summary>
        /// The states in this state group.
        /// </summary>
        private StateBase[] states = null;

        /// <summary>
        /// Gets the states in this state group.
        /// </summary>
        public ReadOnlyCollection<StateBase> States
        {
            get
            {
                if (states == null)
                {
                    LoadStates();
                    if (this.StateGroup == null)
                        return new List<StateBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<StateBase>(states);
            }
        }

        /// <summary>
        /// Loads the states.
        /// </summary>
        private void LoadStates()
        {
            if (this.StateGroup != null)
            {
                List<StateBase> stateGroupStates = new List<StateBase>();
                foreach (State stateGroupState in this.StateGroup.States)
                    stateGroupStates.Add(BaseManager.Current.GetBase<StateBase>(stateGroupState));
                states = stateGroupStates.ToArray();
            }
        }
        #endregion Property: States

        #region Property: State
        /// <summary>
        /// The selected state of the group.
        /// </summary>
        private StateBase state = null;

        /// <summary>
        /// Gets the selected state of the group.
        /// </summary>
        public StateBase State
        {
            get
            {
                if (state == null)
                    LoadState();
                return state;
            }
        }

        /// <summary>
        /// Loads the state.
        /// </summary>
        private void LoadState()
        {
            if (this.StateGroup != null)
                state = BaseManager.Current.GetBase<StateBase>(this.StateGroup.DefaultState);
        }
        #endregion Property: State

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: StateGroupBase(StateGroup stateGroup)
        /// <summary>
        /// Creates a new state group base from the given state group.
        /// </summary>
        /// <param name="stateGroup">The state group to create a base from.</param>
        internal StateGroupBase(StateGroup stateGroup)
            : base(stateGroup)
        {
            if (stateGroup != null)
            {
                if (BaseManager.PreloadProperties)
                {
                    LoadState();
                    LoadStates();
                }
            }
        }
        #endregion Constructor: StateGroupBase(StateGroup stateGroup)

        #endregion Method Group: Constructors

    }
    #endregion Class: StateGroupBase

    #region Class: StateGroupConditionBase
    /// <summary>
    /// A condition on a state group.
    /// </summary>
    public sealed class StateGroupConditionBase : ConditionBase
    {

        #region Properties and Fields

        #region Property: StateGroupCondition
        /// <summary>
        /// Gets the state group condition of which this is a state group condition base.
        /// </summary>
        internal StateGroupCondition StateGroupCondition
        {
            get
            {
                return this.Condition as StateGroupCondition;
            }
        }
        #endregion Property: StateGroupCondition

        #region Property: States
        /// <summary>
        /// The required states.
        /// </summary>
        private StateBase[] states = null;

        /// <summary>
        /// Gets the required states.
        /// </summary>
        public ReadOnlyCollection<StateBase> States
        {
            get
            {
                if (states == null)
                {
                    LoadStates();
                    if (states == null)
                        return new List<StateBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<StateBase>(states);
            }
        }

        /// <summary>
        /// Loads the states.
        /// </summary>
        private void LoadStates()
        {
            if (this.StateGroupCondition != null)
            {
                List<StateBase> stateGroupConditionStates = new List<StateBase>();
                foreach (State stateGroupConditionState in this.StateGroupCondition.States)
                    stateGroupConditionStates.Add(BaseManager.Current.GetBase<StateBase>(stateGroupConditionState));
                states = stateGroupConditionStates.ToArray();
            }
        }
        #endregion Property: States

        #region Property: State
        /// <summary>
        /// The required state.
        /// </summary>
        private StateBase state = null;

        /// <summary>
        /// Gets the required state.
        /// </summary>
        public StateBase State
        {
            get
            {
                return state;
            }
        }
        #endregion Property: State

        #region Property: StateSign
        /// <summary>
        /// The equality sign of the state in the condition.
        /// </summary>
        private EqualitySign? stateSign = null;

        /// <summary>
        /// Gets the equality sign of the state in the condition.
        /// </summary>
        public EqualitySign? StateSign
        {
            get
            {
                return stateSign;
            }
        }
        #endregion Property: StateSign

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: StateGroupConditionBase(StateGroupCondition stateGroupCondition)
        /// <summary>
        /// Creates a base of the given state group condition.
        /// </summary>
        /// <param name="stateGroupCondition">The state group condition to create a base of.</param>
        internal StateGroupConditionBase(StateGroupCondition stateGroupCondition)
            : base(stateGroupCondition)
        {
            if (stateGroupCondition != null)
            {
                this.state = BaseManager.Current.GetBase<StateBase>(stateGroupCondition.State);
                this.stateSign = stateGroupCondition.StateSign;

                if (BaseManager.PreloadProperties)
                    LoadStates();
            }
        }
        #endregion Constructor: StateGroupConditionBase(StateGroupCondition stateGroupCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: StateGroupConditionBase

    #region Class: StateGroupChangeBase
    /// <summary>
    /// A change on a state group.
    /// </summary>
    public sealed class StateGroupChangeBase : ChangeBase
    {

        #region Properties and Fields

        #region Property: StateGroupChange
        /// <summary>
        /// Gets the state group change of which this is a state group change base.
        /// </summary>
        internal StateGroupChange StateGroupChange
        {
            get
            {
                return this.Change as StateGroupChange;
            }
        }
        #endregion Property: StateGroupChange

        #region Property: State
        /// <summary>
        /// The state to change to.
        /// </summary>
        private StateBase state = null;

        /// <summary>
        /// Gets the state to change to.
        /// </summary>
        public StateBase State
        {
            get
            {
                return state;
            }
        }
        #endregion Property: State

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: StateGroupChangeBase(StateGroupChange stateGroupChange)
        /// <summary>
        /// Creates a base of the given state group change.
        /// </summary>
        /// <param name="stateChange">The state group change to create a base of.</param>
        internal StateGroupChangeBase(StateGroupChange stateGroupChange)
            : base(stateGroupChange)
        {
            if (stateGroupChange != null)
                this.state = BaseManager.Current.GetBase<StateBase>(stateGroupChange.State);
        }
        #endregion Constructor: StateGroupChangeBase(StateGroupChange stateGroupChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: StateGroupChangeBase

}