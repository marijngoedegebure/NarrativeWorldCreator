/**************************************************************************
 * 
 * FilterApplicationBase.cs
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
using SemanticsEngine.Abstractions;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: FilterApplicationBase
    /// <summary>
    /// A base of a filter application.
    /// </summary>
    public sealed class FilterApplicationBase : EffectBase
    {

        #region Properties and Fields

        #region Property: FilterApplication
        /// <summary>
        /// Gets the filter application of which this is a filter application base.
        /// </summary>
        internal FilterApplication FilterApplication
        {
            get
            {
                return this.Effect as FilterApplication;
            }
        }
        #endregion Property: FilterApplication

        #region Property: FilterType
        /// <summary>
        /// The type of the filter that should be applied.
        /// </summary>
        private FilterTypeBase filterType = null;
        
        /// <summary>
        /// Gets the type of the filter that should be applied.
        /// </summary>
        public FilterTypeBase FilterType
        {
            get
            {
                return FilterType;
            }
        }
        #endregion Property: Filter

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: FilterApplicationBase(FilterApplication filterApplication)
        /// <summary>
        /// Creates a base of the given filter application.
        /// </summary>
        /// <param name="filterApplication">The filter application to create a base of.</param>
        internal FilterApplicationBase(FilterApplication filterApplication)
            : base(filterApplication)
        {
            if (filterApplication != null)
                this.filterType = BaseManager.Current.GetBase<FilterTypeBase>(filterApplication.FilterType);
        }
        #endregion Constructor: FilterApplicationBase(FilterApplication filterApplication)

        #endregion Method Group: Constructors

    }
    #endregion Class: FilterApplicationBase

}