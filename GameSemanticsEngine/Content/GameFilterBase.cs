/**************************************************************************
 * 
 * GameFilterBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using GameSemantics.GameContent;
using GameSemanticsEngine.Tools;
using SemanticsEngine.Abstractions;

namespace GameSemanticsEngine.GameContent
{

    #region Class: GameFilterBase
    /// <summary>
    /// A base of a game filter.
    /// </summary>
    public class GameFilterBase : StaticContentBase
    {

        #region Properties and Fields

        #region Property: GameFilter
        /// <summary>
        /// Gets the game filter of which this is a game filter base.
        /// </summary>
        protected internal GameFilter GameFilter
        {
            get
            {
                return this.Node as GameFilter;
            }
        }
        #endregion Property: GameFilter

        #region Property: FilterType
        /// <summary>
        /// The filter type on which this game filter is based on.
        /// </summary>
        private FilterTypeBase filterType = null;
        
        /// <summary>
        /// Gets the filter type on which this game filter is based on.
        /// </summary>
        public FilterTypeBase FilterType
        {
            get
            {
                return filterType;
            }
        }
        #endregion Property: FilterType

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: GameFilterBase(GameFilter gameFilter)
        /// <summary>
        /// Creates a new game filter base from the given game filter.
        /// </summary>
        /// <param name="gameFilter">The game filter to create a game filter base from.</param>
        protected internal GameFilterBase(GameFilter gameFilter)
            : base(gameFilter)
        {
            if (gameFilter != null)
                this.filterType = GameBaseManager.Current.GetBase<FilterTypeBase>(gameFilter.FilterType);
        }
        #endregion Constructor: GameFilterBase(GameFilter gameFilter)

        #endregion Method Group: Constructors

    }
    #endregion Class: GameFilterBase

    #region Class: GameFilterValuedBase
    /// <summary>
    /// A base of a valued game filter.
    /// </summary>
    public class GameFilterValuedBase : StaticContentValuedBase
    {

        #region Properties and Fields

        #region Property: GameFilterValued
        /// <summary>
        /// Gets the valued game filter of which this is a valued game filter base.
        /// </summary>
        protected internal GameFilterValued GameFilterValued
        {
            get
            {
                return this.NodeValued as GameFilterValued;
            }
        }
        #endregion Property: GameFilterValued

        #region Property: GameFilterBase
        /// <summary>
        /// Gets the game filter base.
        /// </summary>
        public GameFilterBase GameFilterBase
        {
            get
            {
                return this.NodeBase as GameFilterBase;
            }
        }
        #endregion Property: GameFilterBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: GameFilterValuedBase(GameFilterValued gameFilterValued)
        /// <summary>
        /// Create a valued game filter base from the given valued game filter.
        /// </summary>
        /// <param name="gameFilterValued">The valued game filter to create a valued game filter base from.</param>
        protected internal GameFilterValuedBase(GameFilterValued gameFilterValued)
            : base(gameFilterValued)
        {
        }
        #endregion Constructor: GameFilterValuedBase(GameFilterValued gameFilterValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: GameFilterValuedBase

    #region Class: GameFilterConditionBase
    /// <summary>
    /// A condition on a game filter.
    /// </summary>
    public class GameFilterConditionBase : StaticContentConditionBase
    {

        #region Properties and Fields

        #region Property: GameFilterCondition
        /// <summary>
        /// Gets the game filter condition of which this is a game filter condition base.
        /// </summary>
        protected internal GameFilterCondition GameFilterCondition
        {
            get
            {
                return this.Condition as GameFilterCondition;
            }
        }
        #endregion Property: GameFilterCondition

        #region Property: GameFilterBase
        /// <summary>
        /// Gets the game filter base of which this is a game filter condition base.
        /// </summary>
        public GameFilterBase GameFilterBase
        {
            get
            {
                return this.NodeBase as GameFilterBase;
            }
        }
        #endregion Property: GameFilterBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: GameFilterConditionBase(GameFilterCondition gameFilterCondition)
        /// <summary>
        /// Creates a base of the given game filter condition.
        /// </summary>
        /// <param name="gameFilterCondition">The game filter condition to create a base of.</param>
        protected internal GameFilterConditionBase(GameFilterCondition gameFilterCondition)
            : base(gameFilterCondition)
        {
        }
        #endregion Constructor: GameFilterConditionBase(GameFilterCondition gameFilterCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: GameFilterConditionBase

    #region Class: GameFilterChangeBase
    /// <summary>
    /// A change on game filter.
    /// </summary>
    public class GameFilterChangeBase : StaticContentChangeBase
    {

        #region Properties and Fields

        #region Property: GameFilterChange
        /// <summary>
        /// Gets the game filter change of which this is a game filter change base.
        /// </summary>
        protected internal GameFilterChange GameFilterChange
        {
            get
            {
                return this.Change as GameFilterChange;
            }
        }
        #endregion Property: GameFilterChange

        #region Property: GameFilterBase
        /// <summary>
        /// Gets the affected game filter base.
        /// </summary>
        public GameFilterBase GameFilterBase
        {
            get
            {
                return this.NodeBase as GameFilterBase;
            }
        }
        #endregion Property: GameFilterBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: GameFilterChangeBase(GameFilterChange gameFilterChange)
        /// <summary>
        /// Creates a base of the given game filter change.
        /// </summary>
        /// <param name="gameFilterChange">The game filter change to create a base of.</param>
        protected internal GameFilterChangeBase(GameFilterChange gameFilterChange)
            : base(gameFilterChange)
        {
        }
        #endregion Constructor: GameFilterChangeBase(GameFilterChange gameFilterChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: GameFilterChangeBase

}