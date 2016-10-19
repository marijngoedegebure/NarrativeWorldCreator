/**************************************************************************
 * 
 * Change.cs
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
using System.Reflection;

namespace Semantics.Components
{

    #region Class: Change
    /// <summary>
    /// A change.
    /// </summary>
    public abstract class Change : Effect
    {

        #region Method Group: Constructors

        #region Constructor: Change()
        /// <summary>
        /// Creates a new change.
        /// </summary>
        protected Change()
            : base()
        {
        }
        #endregion Constructor: Change()

        #region Constructor: Change(uint id)
        /// <summary>
        /// Creates a new change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a change from.</param>
        protected Change(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Change(uint id)

        #region Constructor: Change(Change change)
        /// <summary>
        /// Clones a change.
        /// </summary>
        /// <param name="change">The change to clone.</param>
        protected Change(Change change)
            : base(change)
        {
        }
        #endregion Constructor: Change(Change change)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the change.
        /// </summary>
        /// <returns>A clone of the change.</returns>
        public new Change Clone()
        {
            return base.Clone() as Change;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: Change

}