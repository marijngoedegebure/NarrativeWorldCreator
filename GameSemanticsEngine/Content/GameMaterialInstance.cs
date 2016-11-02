/**************************************************************************
 * 
 * GameMaterialInstance.cs
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

    #region Class: GameMaterialInstance
    /// <summary>
    /// An instance of a game material.
    /// </summary>
    public class GameMaterialInstance : StaticContentInstance
    {

        #region Properties and Fields

        #region Property: GameMaterialBase
        /// <summary>
        /// Gets the game material base of which this is a game material instance.
        /// </summary>
        public GameMaterialBase GameMaterialBase
        {
            get
            {
                if (this.GameMaterialValuedBase != null)
                    return this.GameMaterialValuedBase.GameMaterialBase;
                return null;
            }
        }
        #endregion Property: GameMaterialBase

        #region Property: GameMaterialValuedBase
        /// <summary>
        /// Gets the valued game material base of which this is a game material instance.
        /// </summary>
        public GameMaterialValuedBase GameMaterialValuedBase
        {
            get
            {
                return this.Base as GameMaterialValuedBase;
            }
        }
        #endregion Property: GameMaterialValuedBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: GameMaterialInstance(GameMaterialValuedBase gameMaterialValuedBase)
        /// <summary>
        /// Creates a new game material instance from the given valued game material base.
        /// </summary>
        /// <param name="gameMaterialValuedBase">The valued game material base to create the game material instance from.</param>
        internal GameMaterialInstance(GameMaterialValuedBase gameMaterialValuedBase)
            : base(gameMaterialValuedBase)
        {
        }
        #endregion Constructor: GameMaterialInstance(GameMaterialValuedBase gameMaterialValuedBase)

        #region Constructor: GameMaterialInstance(GameMaterialInstance gameMaterialInstance)
        /// <summary>
        /// Clones a game material instance.
        /// </summary>
        /// <param name="gameMaterialInstance">The game material instance to clone.</param>
        protected internal GameMaterialInstance(GameMaterialInstance gameMaterialInstance)
            : base(gameMaterialInstance)
        {
        }
        #endregion Constructor: GameMaterialInstance(GameMaterialInstance gameMaterialInstance)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the game material instance satisfies the given condition.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the game material instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the game material instance is satisfies the given condition.</returns>
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
        /// Apply the change to the game material instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the game material instance.</param>
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
    #endregion Class: GameMaterialInstance

}