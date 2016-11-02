/**************************************************************************
 * 
 * ViewBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using GameSemantics.Components;
using SemanticsEngine.Components;

namespace GameSemanticsEngine.Components
{

    #region Class: ViewBase
    /// <summary>
    /// A base of a view.
    /// </summary>
    public class ViewBase : NodeBase
    {

        #region Properties and Fields

        #region Property: View
        /// <summary>
        /// Gets the view of which this is a view base.
        /// </summary>
        protected internal View View
        {
            get
            {
                return this.Node as View;
            }
        }
        #endregion Property: View

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ViewBase(View view)
        /// <summary>
        /// Creates a new view base from the given view.
        /// </summary>
        /// <param name="view">The view to create a view base from.</param>
        protected internal ViewBase(View view)
            : base(view)
        {
        }
        #endregion Constructor: ViewBase(View view)

        #endregion Method Group: Constructors

    }
    #endregion Class: ViewBase

    #region Class: NullViewBase
    /// <summary>
    /// A view base that represents null.
    /// </summary>
    internal class NullViewBase : ViewBase
    {

        #region Method Group: Constructors

        #region Constructor: NullViewBase()
        /// <summary>
        /// Creates a new null view base.
        /// </summary>
        protected internal NullViewBase()
            : base(null)
        {
        }
        #endregion Constructor: NullViewBase()

        #endregion Method Group: Constructors

    }
    #endregion Class: NullViewBase

}