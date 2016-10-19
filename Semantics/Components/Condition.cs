/**************************************************************************
 * 
 * Condition.cs
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

    #region Class: Condition
    /// <summary>
    /// A condition.
    /// </summary>
    public abstract class Condition : IdHolder
    {

        #region Method Group: Constructors

        #region Constructor: Condition()
        /// <summary>
        /// Creates a new condition.
        /// </summary>
        protected Condition()
            : base()
        {
        }
        #endregion Constructor: Condition()

        #region Constructor: Condition(uint id)
        /// <summary>
        /// Creates a new condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a condition from.</param>
        protected Condition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Condition(uint id)

        #region Constructor: Condition(Condition condition)
        /// <summary>
        /// Clones a condition.
        /// </summary>
        /// <param name="condition">The condition to clone.</param>
        protected Condition(Condition condition)
            : base()
        {
        }
        #endregion Constructor: Condition(Condition condition)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Clone()
        /// <summary>
        /// Clones the condition.
        /// </summary>
        /// <returns>A clone of the condition.</returns>
        public Condition Clone()
        {
            try
            {
                Type type = this.GetType();
                return type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new Type[] { type }, null).Invoke(new object[] { this }) as Condition;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: Condition

}