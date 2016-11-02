/**************************************************************************
 * 
 * CinematicInstance.cs
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

    #region Class: CinematicInstance
    /// <summary>
    /// An instance of a cinematic.
    /// </summary>
    public class CinematicInstance : DynamicContentInstance
    {

        #region Properties and Fields

        #region Property: CinematicBase
        /// <summary>
        /// Gets the cinematic base of which this is a cinematic instance.
        /// </summary>
        public CinematicBase CinematicBase
        {
            get
            {
                if (this.CinematicValuedBase != null)
                    return this.CinematicValuedBase.CinematicBase;
                return null;
            }
        }
        #endregion Property: CinematicBase

        #region Property: CinematicValuedBase
        /// <summary>
        /// Gets the valued cinematic base of which this is a cinematic instance.
        /// </summary>
        public CinematicValuedBase CinematicValuedBase
        {
            get
            {
                return this.Base as CinematicValuedBase;
            }
        }
        #endregion Property: CinematicValuedBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: CinematicInstance(CinematicValuedBase cinematicValuedBase)
        /// <summary>
        /// Creates a new cinematic instance from the given valued cinematic base.
        /// </summary>
        /// <param name="cinematicValuedBase">The valued cinematic base to create the cinematic instance from.</param>
        internal CinematicInstance(CinematicValuedBase cinematicValuedBase)
            : base(cinematicValuedBase)
        {
        }
        #endregion Constructor: CinematicInstance(CinematicValuedBase cinematicValuedBase)

        #region Constructor: CinematicInstance(CinematicInstance cinematicInstance)
        /// <summary>
        /// Clones a cinematic instance.
        /// </summary>
        /// <param name="cinematicInstance">The cinematic instance to clone.</param>
        protected internal CinematicInstance(CinematicInstance cinematicInstance)
            : base(cinematicInstance)
        {
        }
        #endregion Constructor: CinematicInstance(CinematicInstance cinematicInstance)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the cinematic instance satisfies the given condition.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the cinematic instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the cinematic instance is satisfies the given condition.</returns>
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
        /// Apply the change to the cinematic instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the cinematic instance.</param>
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
    #endregion Class: CinematicInstance

}