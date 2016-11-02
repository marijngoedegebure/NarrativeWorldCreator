/**************************************************************************
 * 
 * ConditionBase.cs
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

    #region Class: ConditionBase
    /// <summary>
    /// A condition on targets in a range.
    /// </summary>
    public abstract class ConditionBase : Base
    {

        #region Properties and Fields

        #region Property: Condition
        /// <summary>
        /// Gets the condition this is a base of.
        /// </summary>
        protected internal Condition Condition
        {
            get
            {
                return this.IdHolder as Condition;
            }
        }
        #endregion Property: Condition

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ConditionBase(Condition condition)
        /// <summary>
        /// Creates a base of the given condition.
        /// </summary>
        /// <param name="condition">The condition to create a base of.</param>
        protected ConditionBase(Condition condition)
            : base(condition)
        {
        }
        #endregion Constructor: ConditionBase(Condition condition)

        #endregion Method Group: Constructors

    }
    #endregion Class: ConditionBase

}