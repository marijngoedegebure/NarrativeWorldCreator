/**************************************************************************
 * 
 * State.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using Semantics.Data;

namespace Semantics.Abstractions
{

    #region Class: State
    /// <summary>
    /// A state.
    /// </summary>
    public class State : Abstraction, IComparable<State>
    {

        #region Method Group: Constructors

        #region Constructor: State()
        /// <summary>
        /// Creates a new state.
        /// </summary>
        public State()
            : base()
        {
        }
        #endregion Constructor: State()

        #region Constructor: State(uint id)
        /// <summary>
        /// Creates a new state from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a state from.</param>
        protected State(uint id)
            : base(id)
        {
        }
        #endregion Constructor: State(uint id)

        #region Constructor: State(string name)
        /// <summary>
        /// Creates a new state with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the state.</param>
        public State(string name)
            : base(name)
        {
        }
        #endregion Constructor: State(string name)

        #region Constructor: State(State state)
        /// <summary>
        /// Copy a state.
        /// </summary>
        /// <param name="state">The state to clone.</param>
        public State(State state)
            : base(state)
        {
        }
        #endregion Constructor: State(State state)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the state.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();
            Database.Current.StartRemove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(State other)
        /// <summary>
        /// Compares the state to the other state.
        /// </summary>
        /// <param name="other">The state to compare to this state.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(State other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(State other)

        #endregion Method Group: Other

    }
    #endregion Class: State

}