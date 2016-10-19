/**************************************************************************
 * 
 * Filter.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2010-2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Common;
using Semantics.Abstractions;
using Semantics.Data;
using Semantics.Entities;
using Semantics.Utilities;

namespace Semantics.Components
{

    #region Class: Filter
    /// <summary>
    /// A filter of a particular filter type, having effects for a subject.
    /// </summary>
    public sealed class Filter : IdHolder
    {

        #region Properties and Fields

        #region Property: FilterType
        /// <summary>
        /// Gets the filter type of the filter.
        /// </summary>
        public FilterType FilterType
        {
            get
            {
                return Database.Current.Select<FilterType>(this.ID, GenericTables.Filter, Columns.FilterType);
            }
            private set
            {
                Database.Current.Update(this.ID, GenericTables.Filter, Columns.FilterType, value);
                NotifyPropertyChanged("FilterType");
            }
        }
        #endregion Property: FilterType

        #region Property: Subject
        /// <summary>
        /// Gets or sets the subject of the filter.
        /// </summary>
        public Entity Subject
        {
            get
            {
                return Database.Current.Select<Entity>(this.ID, GenericTables.Filter, Columns.Subject);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Filter, Columns.Subject, value);
                NotifyPropertyChanged("Subject");
            }
        }
        #endregion Property: Subject

        #region Property: Effects
        /// <summary>
        /// Gets the effects in the filter.
        /// </summary>
        public ReadOnlyCollection<Effect> Effects
        {
            get
            {
                return Database.Current.SelectAll<Effect>(this.ID, GenericTables.FilterEffect, Columns.Effect).AsReadOnly();
            }
        }
        #endregion Property: Effects

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: Filter()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static Filter()
        {
            // Filter type and subject
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.FilterType, new Tuple<Type, EntryType>(typeof(FilterType), EntryType.Nullable));
            dict.Add(Columns.Subject, new Tuple<Type, EntryType>(typeof(Entity), EntryType.Nullable));
            Database.Current.AddTableDefinition(GenericTables.Filter, typeof(Filter), dict);

            // Effects
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Effect, new Tuple<Type, EntryType>(typeof(Effect), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.FilterEffect, typeof(Filter), dict);
        }
        #endregion Static Constructor: Filter()

        #region Constructor: Filter(uint id)
        /// <summary>
        /// Creates a new filter from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a filter from.</param>
        private Filter(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Filter(uint id)

        #region Constructor: Filter(Filter filter)
        /// <summary>
        /// Clones a filter.
        /// </summary>
        /// <param name="filter">The filter to clone.</param>
        public Filter(Filter filter)
            : base()
        {
            if (filter != null)
            {
                Database.Current.StartChange();

                this.Subject = filter.Subject;
                foreach (Effect effect in filter.Effects)
                    AddEffect(effect.Clone());

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Filter(Filter filter)

        #region Constructor: Filter(FilterType filterType)
        /// <summary>
        /// Creates a filter from the given filter type.
        /// </summary>
        /// <param name="filterType">The filter type to create a filter from.</param>
        public Filter(FilterType filterType)
            : base()
        {
            if (filterType != null)
            {
                Database.Current.StartChange();

                this.FilterType = filterType;
                filterType.AddFilter(this);

                Database.Current.StopChange();
            }
            else
                Remove();
        }
        #endregion Constructor: Filter(FilterType filterType)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddEffect(Effect effect)
        /// <summary>
        /// Adds the given effect.
        /// </summary>
        /// <param name="effect">The effect to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddEffect(Effect effect)
        {
            if (effect != null)
            {
                // If the effect is already available in all effects, there is no use to add it
                if (HasEffect(effect))
                    return Message.RelationExistsAlready;

                // Add the effect
                Database.Current.Insert(this.ID, GenericTables.FilterEffect, Columns.Effect, effect);
                NotifyPropertyChanged("Effects");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddEffect(Effect effect)

        #region Method: RemoveEffect(Effect effect)
        /// <summary>
        /// Removes a effect.
        /// </summary>
        /// <param name="effect">The effect to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveEffect(Effect effect)
        {
            if (effect != null)
            {
                if (HasEffect(effect))
                {
                    // Remove the effect
                    Database.Current.Remove(this.ID, GenericTables.FilterEffect, Columns.Effect, effect);
                    NotifyPropertyChanged("Effects");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveEffect(Effect effect)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasEffect(Effect effect)
        /// <summary>
        /// Checks if this filter has the given effect.
        /// </summary>
        /// <param name="effect">The effect to check.</param>
        /// <returns>Returns true when this filter has the effect.</returns>
        public bool HasEffect(Effect effect)
        {
            if (effect != null)
            {
                foreach (Effect myEffect in this.Effects)
                {
                    if (effect.Equals(myEffect))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasEffect(Effect effect)

        #region Method: Remove()
        /// <summary>
        /// Remove the filter.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the filter from the filter type
            if (this.FilterType != null)
                this.FilterType.RemoveFilter(this);

            // Remove the effects
            foreach (Effect effect in this.Effects)
                effect.Remove();
            Database.Current.Remove(this.ID, GenericTables.FilterEffect);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(Filter other)
        /// <summary>
        /// Compares the filter to the other filter.
        /// </summary>
        /// <param name="other">The filter to compare to this filter.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Filter other)
        {
            if (this == other)
                return 0;
            if (other != null)
                return this.ID.CompareTo(other.ID);
            return 0;
        }
        #endregion Method: CompareTo(Filter other)

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
    #endregion Class: Filter

}
