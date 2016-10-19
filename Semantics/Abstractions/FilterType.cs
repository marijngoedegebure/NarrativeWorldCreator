/**************************************************************************
 * 
 * FilterType.cs
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Semantics.Components;
using Semantics.Data;
using Semantics.Utilities;

namespace Semantics.Abstractions
{

    #region Class: FilterType
    /// <summary>
    /// A filter type with filters.
    /// </summary>
    public class FilterType : Abstraction, IComparable<FilterType>
    {

        #region Properties and Fields

        #region Property: Filters
        /// <summary>
        /// Gets the personal and inherited filters.
        /// </summary>
        public ReadOnlyCollection<Filter> Filters
        {
            get
            {
                List<Filter> filters = new List<Filter>();
                filters.AddRange(this.PersonalFilters);
                filters.AddRange(this.InheritedFilters);
                return filters.AsReadOnly();
            }
        }
        #endregion Property: Filters

        #region Property: PersonalFilters
        /// <summary>
        /// Gets the personal filters of the filter type.
        /// </summary>
        public ReadOnlyCollection<Filter> PersonalFilters
        {
            get
            {
                return Database.Current.SelectAll<Filter>(GenericTables.Filter, Columns.FilterType, this).AsReadOnly();
            }
        }
        #endregion Property: PersonalFilters

        #region Property: InheritedFilters
        /// <summary>
        /// Gets the inherited filters.
        /// </summary>
        public ReadOnlyCollection<Filter> InheritedFilters
        {
            get
            {
                List<Filter> inheritedFilters = new List<Filter>();
                foreach (Node parent in this.PersonalParents)
                    inheritedFilters.AddRange(((FilterType)parent).Filters);
                return inheritedFilters.AsReadOnly();
            }
        }
        #endregion Property: InheritedFilters

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: FilterType()
        /// <summary>
        /// Creates a new filter type.
        /// </summary>
        public FilterType()
            : base()
        {
        }
        #endregion Constructor: FilterType()

        #region Constructor: FilterType(uint id)
        /// <summary>
        /// Creates a new filter type from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the filter type from.</param>
        protected FilterType(uint id)
            : base(id)
        {
        }
        #endregion Constructor: FilterType(uint id)

        #region Constructor: FilterType(string name)
        /// <summary>
        /// Creates a new filter type with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the filter type.</param>
        public FilterType(string name)
            : base(name)
        {
        }
        #endregion Constructor: FilterType(string name)

        #region Constructor: FilterType(FilterType filterType)
        /// <summary>
        /// Clones a filter type.
        /// </summary>
        /// <param name="filterType">The filter type to clone.</param>
        public FilterType(FilterType filterType)
            : base(filterType)
        {
            if (filterType != null)
            {
                Database.Current.StartChange();

                foreach (Filter filter in filterType.PersonalFilters)
                    AddFilter(filter);

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: FilterType(FilterType filterType)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddFilter(Filter filter)
        /// <summary>
        /// Add a filter to the filter type.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        internal Message AddFilter(Filter filter)
        {
            if (filter != null)
            {
                NotifyPropertyChanged("PersonalFilters");
                NotifyPropertyChanged("Filters");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddFilter(Filter filter)

        #region Method: RemoveFilter(Filter filter)
        /// <summary>
        /// Remove a filter from the filter type.
        /// </summary>
        /// <param name="filter">The filter to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        internal Message RemoveFilter(Filter filter)
        {
            if (filter != null)
            {
                if (HasFilter(filter))
                {
                    NotifyPropertyChanged("PersonalFilters");
                    NotifyPropertyChanged("Filters");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveFilter(Filter filter)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasFilter(Filter filter)
        /// <summary>
        /// Checks whether the filter type has the given filter.
        /// </summary>
        /// <param name="filter">The filter to check.</param>
        /// <returns>Returns true when the filter type has the filter.</returns>
        public bool HasFilter(Filter filter)
        {
            if (filter != null)
            {
                foreach (Filter myFilter in this.Filters)
                {
                    if (filter.Equals(myFilter))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasFilter(Filter filter)

        #region Method: Remove()
        /// <summary>
        /// Remove the filter type.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();
            Database.Current.StartRemove();

            // Remove the filters
            foreach (Filter filter in this.PersonalFilters)
                filter.Remove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(FilterType other)
        /// <summary>
        /// Compares the filter type to the other filter type.
        /// </summary>
        /// <param name="other">The filter type to compare to this filter type.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(FilterType other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(FilterType other)

        #endregion Method Group: Other

    }
    #endregion Class: FilterType

}