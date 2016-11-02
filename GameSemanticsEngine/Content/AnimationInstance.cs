/**************************************************************************
 * 
 * AnimationInstance.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using GameSemantics.Utilities;
using SemanticsEngine.Components;
using SemanticsEngine.Interfaces;

namespace GameSemanticsEngine.GameContent
{

    #region Class: AnimationInstance
    /// <summary>
    /// An instance of an animation.
    /// </summary>
    public class AnimationInstance : DynamicContentInstance
    {

        #region Properties and Fields

        #region Property: AnimationBase
        /// <summary>
        /// Gets the animation base of which this is an animation instance.
        /// </summary>
        public AnimationBase AnimationBase
        {
            get
            {
                if (this.AnimationValuedBase != null)
                    return this.AnimationValuedBase.AnimationBase;
                return null;
            }
        }
        #endregion Property: AnimationBase

        #region Property: AnimationValuedBase
        /// <summary>
        /// Gets the valued animation base of which this is an animation instance.
        /// </summary>
        public AnimationValuedBase AnimationValuedBase
        {
            get
            {
                return this.Base as AnimationValuedBase;
            }
        }
        #endregion Property: AnimationValuedBase

        #region Property: Loop
        /// <summary>
        /// Gets the value that indicates whether the animation should loop.
        /// </summary>
        public bool Loop
        {
            get
            {
                if (this.AnimationValuedBase != null)
                    return GetProperty<bool>("Loop", this.AnimationValuedBase.Loop);

                return GameSemanticsSettings.Values.Loop;
            }
            protected set
            {
                if (this.Loop != value)
                    SetProperty("Loop", value);
            }
        }
        #endregion Property: Loop

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AnimationInstance(AnimationValuedBase animationValuedBase)
        /// <summary>
        /// Creates a new animation instance from the given valued animation base.
        /// </summary>
        /// <param name="animationValuedBase">The valued animation base to create the animation instance from.</param>
        internal AnimationInstance(AnimationValuedBase animationValuedBase)
            : base(animationValuedBase)
        {
        }
        #endregion Constructor: AnimationInstance(AnimationValuedBase animationValuedBase)

        #region Constructor: AnimationInstance(AnimationInstance animationInstance)
        /// <summary>
        /// Clones an animation instance.
        /// </summary>
        /// <param name="animationInstance">The animation instance to clone.</param>
        protected internal AnimationInstance(AnimationInstance animationInstance)
            : base(animationInstance)
        {
            if (animationInstance != null)
                this.Loop = animationInstance.Loop;
        }
        #endregion Constructor: AnimationInstance(AnimationInstance animationInstance)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the animation instance satisfies the given condition.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the animation instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the animation instance is satisfies the given condition.</returns>
        public override bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            // Check whether the base satisfies the condition
            if (conditionBase != null && base.Satisfies(conditionBase, iVariableInstanceHolder))
            {
                AnimationConditionBase animationConditionBase = conditionBase as AnimationConditionBase;
                if (animationConditionBase != null)
                {
                    // Check whether all the properties have the correct values
                    if ((animationConditionBase.Loop == null || this.Loop == (bool)animationConditionBase.Loop))
                        return true;
                }
                else
                    return true;
            }
            return false;
        }
        #endregion Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the change to the animation instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the animation instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        protected internal override bool Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (changeBase != null && base.Apply(changeBase, iVariableInstanceHolder))
            {
                AnimationChangeBase animationChangeBase = changeBase as AnimationChangeBase;
                if (animationChangeBase != null)
                {
                    // Apply all changes
                    if (animationChangeBase.Loop != null)
                        this.Loop = (bool)animationChangeBase.Loop;
                }
                return true;
            }
            return false;
        }
        #endregion Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: AnimationInstance

}