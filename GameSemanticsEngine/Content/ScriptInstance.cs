/**************************************************************************
 * 
 * ScriptInstance.cs
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

    #region Class: ScriptInstance
    /// <summary>
    /// An instance of a script.
    /// </summary>
    public class ScriptInstance : DynamicContentInstance
    {

        #region Properties and Fields

        #region Property: ScriptBase
        /// <summary>
        /// Gets the script base of which this is a script instance.
        /// </summary>
        public ScriptBase ScriptBase
        {
            get
            {
                if (this.ScriptValuedBase != null)
                    return this.ScriptValuedBase.ScriptBase;
                return null;
            }
        }
        #endregion Property: ScriptBase

        #region Property: ScriptValuedBase
        /// <summary>
        /// Gets the valued script base of which this is a script instance.
        /// </summary>
        public ScriptValuedBase ScriptValuedBase
        {
            get
            {
                return this.Base as ScriptValuedBase;
            }
        }
        #endregion Property: ScriptValuedBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ScriptInstance(ScriptValuedBase scriptValuedBase)
        /// <summary>
        /// Creates a new script instance from the given valued script base.
        /// </summary>
        /// <param name="scriptValuedBase">The valued script base to create the script instance from.</param>
        internal ScriptInstance(ScriptValuedBase scriptValuedBase)
            : base(scriptValuedBase)
        {
        }
        #endregion Constructor: ScriptInstance(ScriptValuedBase scriptValuedBase)

        #region Constructor: ScriptInstance(ScriptInstance scriptInstance)
        /// <summary>
        /// Clones a script instance.
        /// </summary>
        /// <param name="scriptInstance">The script instance to clone.</param>
        protected internal ScriptInstance(ScriptInstance scriptInstance)
            : base(scriptInstance)
        {
        }
        #endregion Constructor: ScriptInstance(ScriptInstance scriptInstance)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the script instance satisfies the given condition.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the script instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the script instance is satisfies the given condition.</returns>
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
        /// Apply the change to the script instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the script instance.</param>
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
    #endregion Class: ScriptInstance

}