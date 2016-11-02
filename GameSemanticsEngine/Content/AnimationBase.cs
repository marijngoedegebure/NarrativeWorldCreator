/**************************************************************************
 * 
 * AnimationBase.cs
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
using GameSemantics.Utilities;

namespace GameSemanticsEngine.GameContent
{

    #region Class: AnimationBase
    /// <summary>
    /// A base of animation.
    /// </summary>
    public class AnimationBase : DynamicContentBase
    {

        #region Properties and Fields

        #region Property: Animation
        /// <summary>
        /// Gets the animation of which this is an animation base.
        /// </summary>
        protected internal Animation Animation
        {
            get
            {
                return this.Node as Animation;
            }
        }
        #endregion Property: Animation

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AnimationBase(Animation animation)
        /// <summary>
        /// Creates a new animation base from the given animation.
        /// </summary>
        /// <param name="animation">The animation to create a animation base from.</param>
        protected internal AnimationBase(Animation animation)
            : base(animation)
        {
        }
        #endregion Constructor: AnimationBase(Animation animation)

        #endregion Method Group: Constructors

    }
    #endregion Class: AnimationBase

    #region Class: AnimationValuedBase
    /// <summary>
    /// A base of valued animation.
    /// </summary>
    public class AnimationValuedBase : DynamicContentValuedBase
    {

        #region Properties and Fields

        #region Property: AnimationValued
        /// <summary>
        /// Gets the valued animation of which this is a valued animation base.
        /// </summary>
        protected internal AnimationValued AnimationValued
        {
            get
            {
                return this.NodeValued as AnimationValued;
            }
        }
        #endregion Property: AnimationValued

        #region Property: AnimationBase
        /// <summary>
        /// Gets the animation base.
        /// </summary>
        public AnimationBase AnimationBase
        {
            get
            {
                return this.NodeBase as AnimationBase;
            }
        }
        #endregion Property: AnimationBase

        #region Property: Loop
        /// <summary>
        /// The value that indicates whether the animation should loop.
        /// </summary>
        private bool loop = GameSemanticsSettings.Values.Loop;

        /// <summary>
        /// Gets the value that indicates whether the animation should loop.
        /// </summary>
        public bool Loop
        {
            get
            {
                return loop;
            }
        }
        #endregion Property: Loop

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AnimationValuedBase(AnimationValued animationValued)
        /// <summary>
        /// Create a valued animation base from the given valued animation.
        /// </summary>
        /// <param name="animationValued">The valued animation to create a valued animation base from.</param>
        protected internal AnimationValuedBase(AnimationValued animationValued)
            : base(animationValued)
        {
            if (animationValued != null)
                this.loop = animationValued.Loop;
        }
        #endregion Constructor: AnimationValuedBase(AnimationValued animationValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: AnimationValuedBase

    #region Class: AnimationConditionBase
    /// <summary>
    /// A condition on an animation.
    /// </summary>
    public class AnimationConditionBase : DynamicContentConditionBase
    {

        #region Properties and Fields

        #region Property: AnimationCondition
        /// <summary>
        /// Gets the animation condition of which this is an animation condition base.
        /// </summary>
        protected internal AnimationCondition AnimationCondition
        {
            get
            {
                return this.Condition as AnimationCondition;
            }
        }
        #endregion Property: AnimationCondition

        #region Property: AnimationBase
        /// <summary>
        /// Gets the animation base of which this is an animation condition base.
        /// </summary>
        public AnimationBase AnimationBase
        {
            get
            {
                return this.NodeBase as AnimationBase;
            }
        }
        #endregion Property: AnimationBase

        #region Property: Loop
        /// <summary>
        /// The value that indicates whether the animation should loop.
        /// </summary>
        private bool? loop = null;

        /// <summary>
        /// Gets the value that indicates whether the animation should loop.
        /// </summary>
        public bool? Loop
        {
            get
            {
                return loop;
            }
        }
        #endregion Property: Loop

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AnimationConditionBase(AnimationCondition animationCondition)
        /// <summary>
        /// Creates a base of the given animation condition.
        /// </summary>
        /// <param name="animationCondition">The animation condition to create a base of.</param>
        protected internal AnimationConditionBase(AnimationCondition animationCondition)
            : base(animationCondition)
        {
            if (animationCondition != null)
                this.loop = animationCondition.Loop;
        }
        #endregion Constructor: AnimationConditionBase(AnimationCondition animationCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: AnimationConditionBase

    #region Class: AnimationChangeBase
    /// <summary>
    /// A change on an animation.
    /// </summary>
    public class AnimationChangeBase : DynamicContentChangeBase
    {

        #region Properties and Fields

        #region Property: AnimationChange
        /// <summary>
        /// Gets the animation change of which this is an animation change base.
        /// </summary>
        protected internal AnimationChange AnimationChange
        {
            get
            {
                return this.Change as AnimationChange;
            }
        }
        #endregion Property: AnimationChange

        #region Property: AnimationBase
        /// <summary>
        /// Gets the affected animation base.
        /// </summary>
        public AnimationBase AnimationBase
        {
            get
            {
                return this.NodeBase as AnimationBase;
            }
        }
        #endregion Property: AnimationBase

        #region Property: Loop
        /// <summary>
        /// The loop value to change to.
        /// </summary>
        private bool? loop = null;

        /// <summary>
        /// Gets the loop value to change to.
        /// </summary>
        public bool? Loop
        {
            get
            {
                return loop;
            }
        }
        #endregion Property: Loop

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AnimationChangeBase(AnimationChange animationChange)
        /// <summary>
        /// Creates a base of the given animation change.
        /// </summary>
        /// <param name="animationChange">The animation change to create a base of.</param>
        protected internal AnimationChangeBase(AnimationChange animationChange)
            : base(animationChange)
        {
            if (animationChange != null)
                this.loop = animationChange.Loop;
        }
        #endregion Constructor: AnimationChangeBase(AnimationChange animationChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: AnimationChangeBase

}