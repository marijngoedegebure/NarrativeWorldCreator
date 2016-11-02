/**************************************************************************
 * 
 * FilterBase.cs
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
using Semantics.Components;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Entities;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: FilterBase
    /// <summary>
    /// A base of a filter.
    /// </summary>
    public sealed class FilterBase : Base
    {

        #region Properties and Fields

        #region Property: Filter
        /// <summary>
        /// Gets the filter of which this is a base.
        /// </summary>
        internal Filter Filter
        {
            get
            {
                return this.IdHolder as Filter;
            }
        }
        #endregion Property: Filter

        #region Property: FilterType
        /// <summary>
        /// The filter type of the filter.
        /// </summary>
        private FilterTypeBase filterType = null;

        /// <summary>
        /// Gets the filter type of the filter.
        /// </summary>
        public FilterTypeBase FilterType
        {
            get
            {
                return filterType;
            }
        }
        #endregion Property: FilterType

        #region Property: Subject
        /// <summary>
        /// The subject of the filter.
        /// </summary>
        private EntityBase subject = null;

        /// <summary>
        /// Gets the subject of the filter.
        /// </summary>
        public EntityBase Subject
        {
            get
            {
                return subject;
            }
        }
        #endregion Property: Subject

        #region Property: Effects
        /// <summary>
        /// The effects in the filter.
        /// </summary>
        private EffectBase[] effects = null;

        /// <summary>
        /// Gets the effects in the filter.
        /// </summary>
        public ReadOnlyCollection<EffectBase> Effects
        {
            get
            {
                if (effects == null)
                {
                    LoadEffects();
                    if (effects == null)
                        return new List<EffectBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<EffectBase>(effects);
            }
        }

        /// <summary>
        /// Loads the effects.
        /// </summary>
        private void LoadEffects()
        {
            if (this.Filter != null)
            {
                List<EffectBase> filterEffects = new List<EffectBase>();
                foreach (Effect effect in this.Filter.Effects)
                    filterEffects.Add(BaseManager.Current.GetBase<EffectBase>(effect));
                effects = filterEffects.ToArray();
            }
        }
        #endregion Property: Effects

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: FilterBase(Filter filter)
        /// <summary>
        /// Creates a new filter base from the given filter.
        /// </summary>
        /// <param name="filter">The filter to create a base from.</param>
        internal FilterBase(Filter filter)
            : base(filter)
        {
            if (filter != null)
            {
                this.filterType = BaseManager.Current.GetBase<FilterTypeBase>(filter.FilterType);
                this.subject = BaseManager.Current.GetBase<EntityBase>(filter.Subject);

                if (BaseManager.PreloadProperties)
                    LoadEffects();
            }
        }
        #endregion Constructor: FilterBase(Filter filter)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString()
        {
            if (this.FilterType != null)
                return this.FilterType.ToString();

            return base.ToString();
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: FilterBase

}
