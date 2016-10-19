/**************************************************************************
 * 
 * FilterApplication.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using System.Collections.Generic;
using Common;
using Semantics.Abstractions;
using Semantics.Data;

namespace Semantics.Components
{

    #region Class: FilterApplication
    /// <summary>
    /// A filter application effect.
    /// </summary>
    public sealed class FilterApplication : Effect
    {

        #region Properties and Fields

        #region Property: FilterType
        /// <summary>
        /// Gets or sets the type of the filter that should be applied.
        /// </summary>
        public FilterType FilterType
        {
            get
            {
                return Database.Current.Select<FilterType>(this.ID, ValueTables.FilterApplication, Columns.FilterType);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.FilterApplication, Columns.FilterType, value);
                NotifyPropertyChanged("FilterType");
            }
        }
        #endregion Property: FilterType

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: FilterApplication()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static FilterApplication()
        {
            // Filter type
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.FilterType, new Tuple<Type, EntryType>(typeof(FilterType), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.FilterApplication, typeof(FilterApplication), dict);
        }
        #endregion Static Constructor: FilterApplication()

        #region Constructor: FilterApplication()
        /// <summary>
        /// Creates a new filter application.
        /// </summary>
        public FilterApplication()
        {
        }
        #endregion Constructor: FilterApplication()

        #region Constructor: FilterApplication(uint id)
        /// <summary>
        /// Creates a new filter application from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a filter application from.</param>
        private FilterApplication(uint id)
            : base(id)
        {
        }
        #endregion Constructor: FilterApplication(uint id)

        #region Constructor: FilterApplication(FilterApplication filterApplication)
        /// <summary>
        /// Clones a filter application.
        /// </summary>
        /// <param name="filterApplication">The filter application to clone.</param>
        public FilterApplication(FilterApplication filterApplication)
            : base(filterApplication)
        {
            if (filterApplication != null)
            {
                Database.Current.StartChange();

                this.FilterType = filterApplication.FilterType;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: FilterApplication(FilterApplication filterApplication)

        #region Constructor: FilterApplication(FilterType filterType)
        /// <summary>
        /// Creates a new filter application with the given filter type.
        /// </summary>
        /// <param name="filterType">The filter type to create a filter application for.</param>
        public FilterApplication(FilterType filterType)
            : this()
        {
            Database.Current.StartChange();

            this.FilterType = filterType;

            Database.Current.StopChange();
        }
        #endregion Constructor: FilterApplication(FilterType filterType)

        #endregion Method Group: Constructors

    }
    #endregion Class: FilterApplication

}