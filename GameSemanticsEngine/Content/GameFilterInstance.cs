/**************************************************************************
 * 
 * GameFilterInstance.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using SemanticsEngine.Components;
using SemanticsEngine.Interfaces;

namespace GameSemanticsEngine.GameContent
{

    #region Class: GameFilterInstance
    /// <summary>
    /// An instance of a game filter.
    /// </summary>
    public class GameFilterInstance : StaticContentInstance
    {

        #region Properties and Fields

        #region Property: GameFilterBase
        /// <summary>
        /// Gets the game filter base of which this is a game filter instance.
        /// </summary>
        public GameFilterBase GameFilterBase
        {
            get
            {
                if (this.GameFilterValuedBase != null)
                    return this.GameFilterValuedBase.GameFilterBase;
                return null;
            }
        }
        #endregion Property: GameFilterBase

        #region Property: GameFilterValuedBase
        /// <summary>
        /// Gets the valued game filter base of which this is a game filter instance.
        /// </summary>
        public GameFilterValuedBase GameFilterValuedBase
        {
            get
            {
                return this.Base as GameFilterValuedBase;
            }
        }
        #endregion Property: GameFilterValuedBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: GameFilterInstance(GameFilterValuedBase gameFilterValuedBase)
        /// <summary>
        /// Creates a new game filter instance from the given valued game filter base.
        /// </summary>
        /// <param name="gameFilterValuedBase">The valued game filter base to create the game filter instance from.</param>
        internal GameFilterInstance(GameFilterValuedBase gameFilterValuedBase)
            : base(gameFilterValuedBase)
        {
        }
        #endregion Constructor: GameFilterInstance(GameFilterValuedBase gameFilterValuedBase)

        #region Constructor: GameFilterInstance(GameFilterInstance gameFilterInstance)
        /// <summary>
        /// Clones a game filter instance.
        /// </summary>
        /// <param name="gameFilterInstance">The game filter instance to clone.</param>
        protected internal GameFilterInstance(GameFilterInstance gameFilterInstance)
            : base(gameFilterInstance)
        {
        }
        #endregion Constructor: GameFilterInstance(GameFilterInstance gameFilterInstance)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the game filter instance satisfies the given condition.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the game filter instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the game filter instance is satisfies the given condition.</returns>
        public override bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            // Check whether the base satisfies the condition
            if (conditionBase != null)
                return base.Satisfies(conditionBase, iVariableInstanceHolder);
            return false;
        }
        #endregion Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the change to the game filter instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the game filter instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        protected internal override bool Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (changeBase != null)
                return base.Apply(changeBase, iVariableInstanceHolder);
            return false;
        }
        #endregion Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: GameFilterInstance

}