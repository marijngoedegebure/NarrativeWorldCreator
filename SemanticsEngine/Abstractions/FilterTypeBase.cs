/**************************************************************************
 * 
 * FilterTypeBase.cs
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
using SemanticsEngine.Components;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Abstractions
{

    #region Class: FilterTypeBase
    /// <summary>
    /// A base of a filter type.
    /// </summary>
    public class FilterTypeBase : AbstractionBase
    {

        #region Properties and Fields

        #region Property: FilterType
        /// <summary>
        /// Gets the filter type of which this is a filter type base.
        /// </summary>
        protected internal FilterType FilterType
        {
            get
            {
                return this.IdHolder as FilterType;
            }
        }
        #endregion Property: FilterType

        #region Property: Filters
        /// <summary>
        /// The filters of the filter type.
        /// </summary>
        private FilterBase[] filters = null;
        
        /// <summary>
        /// Gets the filters of the filter type.
        /// </summary>
        public ReadOnlyCollection<FilterBase> Filters
        {
            get
            {
                if (filters == null)
                {
                    LoadFilters();
                    if (filters == null)
                        return new List<FilterBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<FilterBase>(filters);
            }
        }

        /// <summary>
        /// Loads the filters.
        /// </summary>
        private void LoadFilters()
        {
            if (this.FilterType != null)
            {
                List<FilterBase> filterBases = new List<FilterBase>();
                foreach (Filter filter in this.FilterType.PersonalFilters)
                    filterBases.Add(BaseManager.Current.GetBase<FilterBase>(filter));
                filters = filterBases.ToArray();
            }
        }
        #endregion Property: Filters

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: FilterTypeBase(FilterType filterType)
        /// <summary>
        /// Create a filter type base from the given filter type.
        /// </summary>
        /// <param name="filterType">The filter type to create a filter type base from.</param>
        protected internal FilterTypeBase(FilterType filterType)
            : base(filterType)
        {
            if (filterType != null)
            {
                if (BaseManager.PreloadProperties)
                    LoadFilters();
            }
        }
        #endregion Constructor: FilterTypeBase(FilterType filterType)

        #endregion Method Group: Constructors

    }
    #endregion Class: FilterTypeBase

}
