/**************************************************************************
 * 
 * Animation.cs
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

namespace GameSemantics.GameContent
{

    #region Class: Animation
    /// <summary>
    /// An animation.
    /// </summary>
    public class Animation : DynamicContent, IComparable<Animation>
    {

        #region Method Group: Constructors

        #region Constructor: Animation()
        /// <summary>
        /// Creates a new animation.
        /// </summary>
        public Animation()
            : base()
        {
        }
        #endregion Constructor: Animation()

        #region Constructor: Animation(uint id)
        /// <summary>
        /// Creates a new animation from the given ID.
        /// </summary>
        /// <param name="id">The ID to create an animation from.</param>
        protected Animation(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Animation(uint id)

        #region Constructor: Animation(string file)
        /// <summary>
        /// Creates a new animation with the given file.
        /// </summary>
        /// <param name="file">The file to assign to the animation.</param>
        public Animation(string file)
            : base(file)
        {
        }
        #endregion Constructor: Animation(string file)

        #region Constructor: Animation(Animation animation)
        /// <summary>
        /// Clones a animation.
        /// </summary>
        /// <param name="animation">The animation to clone.</param>
        public Animation(Animation animation)
            : base(animation)
        {
        }
        #endregion Constructor: Animation(Animation animation)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the animation.
        /// </summary>
        public override void Remove()
        {
            GameDatabase.Current.StartChange();
            GameDatabase.Current.StartRemove();

            base.Remove();

            GameDatabase.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(Animation other)
        /// <summary>
        /// Compares the animation to the other animation.
        /// </summary>
        /// <param name="other">The animation to compare to this animation.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Animation other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(Animation other)

        #endregion Method Group: Other

    }
    #endregion Class: Animation

    #region Class: AnimationValued
    /// <summary>
    /// A valued version of an animation.
    /// </summary>
    public class AnimationValued : DynamicContentValued
    {

        #region Properties and Fields

        #region Property: Animation
        /// <summary>
        /// Gets the animation of which this is a valued animation.
        /// </summary>
        public Animation Animation
        {
            get
            {
                return this.Node as Animation;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: Animation

        #region Property: Loop
        /// <summary>
        /// Gets or sets the value that indicates whether the animation should loop.
        /// </summary>
        public bool Loop
        {
            get
            {
                return GameDatabase.Current.Select<bool>(this.ID, GameValueTables.AnimationValued, GameColumns.Loop);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.AnimationValued, GameColumns.Loop, value);
                NotifyPropertyChanged("Loop");
            }
        }
        #endregion Property: Loop

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AnimationValued(uint id)
        /// <summary>
        /// Creates new valued animation from the given ID.
        /// </summary>
        /// <param name="id">The ID to create valued animation from.</param>
        protected AnimationValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: AnimationValued(uint id)

        #region Constructor: AnimationValued(AnimationValued animationValued)
        /// <summary>
        /// Clones a valued animation.
        /// </summary>
        /// <param name="animationValued">The valued animation to clone.</param>
        public AnimationValued(AnimationValued animationValued)
            : base(animationValued)
        {
            if (animationValued != null)
            {
                GameDatabase.Current.StartChange();

                this.Loop = animationValued.Loop;

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: AnimationValued(AnimationValued animationValued)

        #region Constructor: AnimationValued(Animation animation)
        /// <summary>
        /// Creates a new valued animation from the given animation.
        /// </summary>
        /// <param name="animation">The animation to create a valued animation from.</param>
        public AnimationValued(Animation animation)
            : base(animation)
        {
        }
        #endregion Constructor: AnimationValued(Animation animation)

        #endregion Method Group: Constructors

    }
    #endregion Class: AnimationValued

    #region Class: AnimationCondition
    /// <summary>
    /// A condition on an animation.
    /// </summary>
    public class AnimationCondition : DynamicContentCondition
    {

        #region Properties and Fields

        #region Property: Animation
        /// <summary>
        /// Gets or sets the required animation.
        /// </summary>
        public Animation Animation
        {
            get
            {
                return this.Node as Animation;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Animation

        #region Property: Loop
        /// <summary>
        /// Gets or sets the value that indicates whether the animation should loop.
        /// </summary>
        public bool? Loop
        {
            get
            {
                return GameDatabase.Current.Select<bool?>(this.ID, GameValueTables.AnimationCondition, GameColumns.Loop);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.AnimationCondition, GameColumns.Loop, value);
                NotifyPropertyChanged("Loop");
            }
        }
        #endregion Property: Loop

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AnimationCondition()
        /// <summary>
        /// Creates a new animation condition.
        /// </summary>
        public AnimationCondition()
            : base()
        {
        }
        #endregion Constructor: AnimationCondition()

        #region Constructor: AnimationCondition(uint id)
        /// <summary>
        /// Creates a new animation condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the animation condition from.</param>
        protected AnimationCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: AnimationCondition(uint id)

        #region Constructor: AnimationCondition(AnimationCondition animationCondition)
        /// <summary>
        /// Clones an animation condition.
        /// </summary>
        /// <param name="animationCondition">The animation condition to clone.</param>
        public AnimationCondition(AnimationCondition animationCondition)
            : base(animationCondition)
        {
            if (animationCondition != null)
            {
                GameDatabase.Current.StartChange();

                this.Loop = animationCondition.Loop;

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: AnimationCondition(AnimationCondition animationCondition)

        #region Constructor: AnimationCondition(Animation animation)
        /// <summary>
        /// Creates a condition for the given animation.
        /// </summary>
        /// <param name="animation">The animation to create a condition for.</param>
        public AnimationCondition(Animation animation)
            : base(animation)
        {
        }
        #endregion Constructor: AnimationCondition(Animation animation)

        #endregion Method Group: Constructors

    }
    #endregion Class: AnimationCondition

    #region Class: AnimationChange
    /// <summary>
    /// A change on an animation.
    /// </summary>
    public class AnimationChange : DynamicContentChange
    {

        #region Properties and Fields

        #region Property: Animation
        /// <summary>
        /// Gets or sets the affected animation.
        /// </summary>
        public Animation Animation
        {
            get
            {
                return this.Node as Animation;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Animation

        #region Property: Loop
        /// <summary>
        /// Gets or sets the loop value to change to.
        /// </summary>
        public bool? Loop
        {
            get
            {
                return GameDatabase.Current.Select<bool?>(this.ID, GameValueTables.AnimationChange, GameColumns.Loop);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.AnimationChange, GameColumns.Loop, value);
                NotifyPropertyChanged("Loop");
            }
        }
        #endregion Property: Loop

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AnimationChange()
        /// <summary>
        /// Creates an animation change.
        /// </summary>
        public AnimationChange()
            : base()
        {
        }
        #endregion Constructor: AnimationChange()

        #region Constructor: AnimationChange(uint id)
        /// <summary>
        /// Creates a new animation change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create an animation change from.</param>
        protected AnimationChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: AnimationChange(uint id)

        #region Constructor: AnimationChange(AnimationChange animationChange)
        /// <summary>
        /// Clones an animation change.
        /// </summary>
        /// <param name="animationChange">The animation change to clone.</param>
        public AnimationChange(AnimationChange animationChange)
            : base(animationChange)
        {
            if (animationChange != null)
            {
                GameDatabase.Current.StartChange();

                this.Loop = animationChange.Loop;

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: AnimationChange(AnimationChange animationChange)

        #region Constructor: AnimationChange(Animation animation)
        /// <summary>
        /// Creates a change for the given animation.
        /// </summary>
        /// <param name="animation">The animation to create a change for.</param>
        public AnimationChange(Animation animation)
            : base(animation)
        {
        }
        #endregion Constructor: AnimationChange(Animation animation)

        #endregion Method Group: Constructors

    }
    #endregion Class: AnimationChange

}