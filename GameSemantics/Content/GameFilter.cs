/**************************************************************************
 * 
 * GameFilter.cs
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
using Semantics.Abstractions;

namespace GameSemantics.GameContent
{

    #region Class: GameFilter
    /// <summary>
    /// A game filter.
    /// </summary>
    public class GameFilter : StaticContent, IComparable<GameFilter>
    {

        #region Properties and Fields

        #region Property: FilterType
        /// <summary>
        /// Gets or sets the filter type this game filter is based on.
        /// </summary>
        public FilterType FilterType
        {
            get
            {
                return GameDatabase.Current.Select<FilterType>(this.ID, GameTables.GameFilter, GameColumns.FilterType);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameTables.GameFilter, GameColumns.FilterType, value);
                NotifyPropertyChanged("FilterType");
            }
        }
        #endregion Property: FilterType

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: GameFilter()
        /// <summary>
        /// Creates a new game filter.
        /// </summary>
        public GameFilter()
            : base()
        {
        }
        #endregion Constructor: GameFilter()

        #region Constructor: GameFilter(uint id)
        /// <summary>
        /// Creates a new game filter from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a game filter from.</param>
        protected GameFilter(uint id)
            : base(id)
        {
        }
        #endregion Constructor: GameFilter(uint id)

        #region Constructor: GameFilter(string file)
        /// <summary>
        /// Creates a new game filter with the given file.
        /// </summary>
        /// <param name="file">The file to assign to the game filter.</param>
        public GameFilter(string file)
            : base(file)
        {
        }
        #endregion Constructor: GameFilter(string file)

        #region Constructor: GameFilter(GameFilter gameFilter)
        /// <summary>
        /// Clones a game filter.
        /// </summary>
        /// <param name="gameFilter">The game filter to clone.</param>
        public GameFilter(GameFilter gameFilter)
            : base(gameFilter)
        {
            if (gameFilter != null)
                this.FilterType = gameFilter.FilterType;
        }
        #endregion Constructor: GameFilter(GameFilter gameFilter)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the game filter.
        /// </summary>
        public override void Remove()
        {
            GameDatabase.Current.StartChange();
            GameDatabase.Current.StartRemove();

            base.Remove();

            GameDatabase.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(GameFilter other)
        /// <summary>
        /// Compares the game filter to the other game filter.
        /// </summary>
        /// <param name="other">The game filter to compare to this game filter.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(GameFilter other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(GameFilter other)

        #endregion Method Group: Other

    }
    #endregion Class: GameFilter

    #region Class: GameFilterValued
    /// <summary>
    /// A valued version of a game filter.
    /// </summary>
    public class GameFilterValued : StaticContentValued
    {

        #region Properties and Fields

        #region Property: GameFilter
        /// <summary>
        /// Gets the game filter of which this is a valued game filter.
        /// </summary>
        public GameFilter GameFilter
        {
            get
            {
                return this.Node as GameFilter;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: GameFilter

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: GameFilterValued(uint id)
        /// <summary>
        /// Creates a new valued game filter from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the valued game filter from.</param>
        protected GameFilterValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: GameFilterValued(uint id)

        #region Constructor: GameFilterValued(GameFilterValued gameFilterValued)
        /// <summary>
        /// Clones a valued game filter.
        /// </summary>
        /// <param name="gameFilterValued">The valued game filter to clone.</param>
        public GameFilterValued(GameFilterValued gameFilterValued)
            : base(gameFilterValued)
        {
        }
        #endregion Constructor: GameFilterValued(GameFilterValued gameFilterValued)

        #region Constructor: GameFilterValued(GameFilter gameFilter)
        /// <summary>
        /// Creates a new valued game filter from the given game filter.
        /// </summary>
        /// <param name="gameFilter">The game filter to create a valued game filter from.</param>
        public GameFilterValued(GameFilter gameFilter)
            : base(gameFilter)
        {
        }
        #endregion Constructor: GameFilterValued(GameFilter gameFilter)

        #endregion Method Group: Constructors

    }
    #endregion Class: GameFilterValued

    #region Class: GameFilterCondition
    /// <summary>
    /// A condition on a game filter.
    /// </summary>
    public class GameFilterCondition : StaticContentCondition
    {

        #region Properties and Fields

        #region Property: GameFilter
        /// <summary>
        /// Gets or sets the required game filter.
        /// </summary>
        public GameFilter GameFilter
        {
            get
            {
                return this.Node as GameFilter;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: GameFilter

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: GameFilterCondition()
        /// <summary>
        /// Creates a new game filter condition.
        /// </summary>
        public GameFilterCondition()
            : base()
        {
        }
        #endregion Constructor: GameFilterCondition()

        #region Constructor: GameFilterCondition(uint id)
        /// <summary>
        /// Creates a new game filter condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the game filter condition from.</param>
        protected GameFilterCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: GameFilterCondition(uint id)

        #region Constructor: GameFilterCondition(GameFilterCondition gameFilterCondition)
        /// <summary>
        /// Clones an game filter condition.
        /// </summary>
        /// <param name="gameFilterCondition">The game filter condition to clone.</param>
        public GameFilterCondition(GameFilterCondition gameFilterCondition)
            : base(gameFilterCondition)
        {
        }
        #endregion Constructor: GameFilterCondition(GameFilterCondition gameFilterCondition)

        #region Constructor: GameFilterCondition(GameFilter gameFilter)
        /// <summary>
        /// Creates a condition for the given game filter.
        /// </summary>
        /// <param name="gameFilter">The game filter to create a condition for.</param>
        public GameFilterCondition(GameFilter gameFilter)
            : base(gameFilter)
        {
        }
        #endregion Constructor: GameFilterCondition(GameFilter gameFilter)

        #endregion Method Group: Constructors

    }
    #endregion Class: GameFilterCondition

    #region Class: GameFilterChange
    /// <summary>
    /// A change on a game filter.
    /// </summary>
    public class GameFilterChange : StaticContentChange
    {

        #region Properties and Fields

        #region Property: GameFilter
        /// <summary>
        /// Gets or sets the affected game filter.
        /// </summary>
        public GameFilter GameFilter
        {
            get
            {
                return this.Node as GameFilter;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: GameFilter

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: GameFilterChange()
        /// <summary>
        /// Creates an game filter change.
        /// </summary>
        public GameFilterChange()
            : base()
        {
        }
        #endregion Constructor: GameFilterChange()

        #region Constructor: GameFilterChange(uint id)
        /// <summary>
        /// Creates a new game filter change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create an game filter change from.</param>
        protected GameFilterChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: GameFilterChange(uint id)

        #region Constructor: GameFilterChange(GameFilterChange gameFilterChange)
        /// <summary>
        /// Clones an game filter change.
        /// </summary>
        /// <param name="gameFilterChange">The game filter change to clone.</param>
        public GameFilterChange(GameFilterChange gameFilterChange)
            : base(gameFilterChange)
        {
        }
        #endregion Constructor: GameFilterChange(GameFilterChange gameFilterChange)

        #region Constructor: GameFilterChange(GameFilter gameFilter)
        /// <summary>
        /// Creates a change for the given game filter.
        /// </summary>
        /// <param name="gameFilter">The game filter to create a change for.</param>
        public GameFilterChange(GameFilter gameFilter)
            : base(gameFilter)
        {
        }
        #endregion Constructor: GameFilterChange(GameFilter gameFilter)

        #endregion Method Group: Constructors

    }
    #endregion Class: GameFilterChange

}