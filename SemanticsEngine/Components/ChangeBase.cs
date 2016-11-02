/**************************************************************************
 * 
 * ChangeBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using Semantics.Components;

namespace SemanticsEngine.Components
{

    #region Class: ChangeBase
    /// <summary>
    /// A change.
    /// </summary>
    public abstract class ChangeBase : EffectBase
    {

        #region Properties and Fields

        #region Property: Change
        /// <summary>
        /// Gets the change this is a base of.
        /// </summary>
        protected internal Change Change
        {
            get
            {
                return this.Effect as Change;
            }
        }
        #endregion Property: Change

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ChangeBase(Change change)
        /// <summary>
        /// Creates a change base from the given change.
        /// </summary>
        /// <param name="change">The change to to create a base from.</param>
        protected ChangeBase(Change change)
            : base(change)
        {
        }
        #endregion Constructor: ChangeBase(Change change)

        #endregion Method Group: Constructors

    }
    #endregion Class: ChangeBase

}