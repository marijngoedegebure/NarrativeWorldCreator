/**************************************************************************
 * 
 * View.cs
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
using GameSemantics.Data;
using Semantics.Components;

namespace GameSemantics.Components
{

    #region Class: View
    /// <summary>
    /// A view.
    /// </summary>
    public class View : Node, IComparable<View>
    {

        #region Method Group: Constructors

        #region Constructor: View()
        /// <summary>
        /// Creates a new view.
        /// </summary>
        public View()
            : base()
        {
        }
        #endregion Constructor: View()

        #region Constructor: View(uint id)
        /// <summary>
        /// Creates a new view from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the view from.</param>
        protected View(uint id)
            : base(id)
        {
        }
        #endregion Constructor: View(uint id)

        #region Constructor: View(string name)
        /// <summary>
        /// Creates a new view with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the view.</param>
        public View(string name)
            : base(name)
        {
        }
        #endregion Constructor: View(string name)

        #region Constructor: View(View view)
        /// <summary>
        /// Clones a view.
        /// </summary>
        /// <param name="view">The view to clone.</param>
        public View(View view)
            : base(view)
        {
        }
        #endregion Constructor: View(View view)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the view.
        /// </summary>
        public override void Remove()
        {
            GameDatabase.Current.StartChange();
            GameDatabase.Current.StartRemove();

            base.Remove();

            GameDatabase.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(View other)
        /// <summary>
        /// Compares the view to the other view.
        /// </summary>
        /// <param name="other">The view to compare to this view.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(View other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(View other)

        #endregion Method Group: Other

    }
    #endregion Class: View

}