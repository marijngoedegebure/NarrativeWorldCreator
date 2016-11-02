/**************************************************************************
 * 
 * GameMaterialBase.cs
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

namespace GameSemanticsEngine.GameContent
{

    #region Class: GameMaterialBase
    /// <summary>
    /// A base of a game material.
    /// </summary>
    public class GameMaterialBase : StaticContentBase
    {

        #region Properties and Fields

        #region Property: GameMaterial
        /// <summary>
        /// Gets the game material of which this is a game material base.
        /// </summary>
        protected internal GameMaterial GameMaterial
        {
            get
            {
                return this.Node as GameMaterial;
            }
        }
        #endregion Property: GameMaterial

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: GameMaterialBase(GameMaterial gameMaterial)
        /// <summary>
        /// Creates a new game material base from the given game material.
        /// </summary>
        /// <param name="gameMaterial">The game material to create a game material base from.</param>
        protected internal GameMaterialBase(GameMaterial gameMaterial)
            : base(gameMaterial)
        {
        }
        #endregion Constructor: GameMaterialBase(GameMaterial gameMaterial)

        #endregion Method Group: Constructors

    }
    #endregion Class: GameMaterialBase

    #region Class: GameMaterialValuedBase
    /// <summary>
    /// A base of a valued game material.
    /// </summary>
    public class GameMaterialValuedBase : StaticContentValuedBase
    {

        #region Properties and Fields

        #region Property: GameMaterialValued
        /// <summary>
        /// Gets the valued game material of which this is a valued game material base.
        /// </summary>
        protected internal GameMaterialValued GameMaterialValued
        {
            get
            {
                return this.NodeValued as GameMaterialValued;
            }
        }
        #endregion Property: GameMaterialValued

        #region Property: GameMaterialBase
        /// <summary>
        /// Gets the game material base.
        /// </summary>
        public GameMaterialBase GameMaterialBase
        {
            get
            {
                return this.NodeBase as GameMaterialBase;
            }
        }
        #endregion Property: GameMaterialBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: GameMaterialValuedBase(GameMaterialValued gameMaterialValued)
        /// <summary>
        /// Create a valued game material base from the given valued game material.
        /// </summary>
        /// <param name="gameMaterialValued">The valued game material to create a valued game material base from.</param>
        protected internal GameMaterialValuedBase(GameMaterialValued gameMaterialValued)
            : base(gameMaterialValued)
        {
        }
        #endregion Constructor: GameMaterialValuedBase(GameMaterialValued gameMaterialValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: GameMaterialValuedBase

    #region Class: GameMaterialConditionBase
    /// <summary>
    /// A condition on a game material.
    /// </summary>
    public class GameMaterialConditionBase : StaticContentConditionBase
    {

        #region Properties and Fields

        #region Property: GameMaterialCondition
        /// <summary>
        /// Gets the game material condition of which this is a game material condition base.
        /// </summary>
        protected internal GameMaterialCondition GameMaterialCondition
        {
            get
            {
                return this.Condition as GameMaterialCondition;
            }
        }
        #endregion Property: GameMaterialCondition

        #region Property: GameMaterialBase
        /// <summary>
        /// Gets the game material base of which this is a game material condition base.
        /// </summary>
        public GameMaterialBase GameMaterialBase
        {
            get
            {
                return this.NodeBase as GameMaterialBase;
            }
        }
        #endregion Property: GameMaterialBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: GameMaterialConditionBase(GameMaterialCondition gameMaterialCondition)
        /// <summary>
        /// Creates a base of the given game material condition.
        /// </summary>
        /// <param name="gameMaterialCondition">The game material condition to create a base of.</param>
        protected internal GameMaterialConditionBase(GameMaterialCondition gameMaterialCondition)
            : base(gameMaterialCondition)
        {
        }
        #endregion Constructor: GameMaterialConditionBase(GameMaterialCondition gameMaterialCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: GameMaterialConditionBase

    #region Class: GameMaterialChangeBase
    /// <summary>
    /// A change on game material.
    /// </summary>
    public class GameMaterialChangeBase : StaticContentChangeBase
    {

        #region Properties and Fields

        #region Property: GameMaterialChange
        /// <summary>
        /// Gets the game material change of which this is a game material change base.
        /// </summary>
        protected internal GameMaterialChange GameMaterialChange
        {
            get
            {
                return this.Change as GameMaterialChange;
            }
        }
        #endregion Property: GameMaterialChange

        #region Property: GameMaterialBase
        /// <summary>
        /// Gets the affected game material base.
        /// </summary>
        public GameMaterialBase GameMaterialBase
        {
            get
            {
                return this.NodeBase as GameMaterialBase;
            }
        }
        #endregion Property: GameMaterialBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: GameMaterialChangeBase(GameMaterialChange gameMaterialChange)
        /// <summary>
        /// Creates a base of the given game material change.
        /// </summary>
        /// <param name="gameMaterialChange">The game material change to create a base of.</param>
        protected internal GameMaterialChangeBase(GameMaterialChange gameMaterialChange)
            : base(gameMaterialChange)
        {
        }
        #endregion Constructor: GameMaterialChangeBase(GameMaterialChange gameMaterialChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: GameMaterialChangeBase

}