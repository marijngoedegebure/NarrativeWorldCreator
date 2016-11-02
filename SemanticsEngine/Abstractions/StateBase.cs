/**************************************************************************
 * 
 * StateBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using Semantics.Abstractions;

namespace SemanticsEngine.Abstractions
{

    #region Class: StateBase
    /// <summary>
    /// A base of a state.
    /// </summary>
    public class StateBase : AbstractionBase
    {

        #region Properties and Fields

        #region Property: State
        /// <summary>
        /// Gets the state of which this is a state base.
        /// </summary>
        protected internal State State
        {
            get
            {
                return this.IdHolder as State;
            }
        }
        #endregion Property: State

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: StateBase(State state)
        /// <summary>
        /// Creates a new state base from the given state.
        /// </summary>
        /// <param name="state">The element to create a state base from.</param>
        protected internal StateBase(State state)
            : base(state)
        {
        }
        #endregion Constructor: StateBase(State state)

        #endregion Method Group: Constructors

    }
    #endregion Class: StateBase

}