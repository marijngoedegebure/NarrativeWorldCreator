/**************************************************************************
 * 
 * SpriteInstance.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using Common;
using GameSemantics.Utilities;
using Semantics.Utilities;
using SemanticsEngine.Components;
using SemanticsEngine.Interfaces;

namespace GameSemanticsEngine.GameContent
{

    #region Class: SpriteInstance
    /// <summary>
    /// An instance of a sprite.
    /// </summary>
    public class SpriteInstance : StaticContentInstance
    {

        #region Properties and Fields

        #region Property: SpriteBase
        /// <summary>
        /// Gets the sprite base of which this is a sprite instance.
        /// </summary>
        public SpriteBase SpriteBase
        {
            get
            {
                if (this.SpriteValuedBase != null)
                    return this.SpriteValuedBase.SpriteBase;
                return null;
            }
        }
        #endregion Property: SpriteBase

        #region Property: SpriteValuedBase
        /// <summary>
        /// Gets the valued sprite base of which this is a sprite instance.
        /// </summary>
        public SpriteValuedBase SpriteValuedBase
        {
            get
            {
                return this.Base as SpriteValuedBase;
            }
        }
        #endregion Property: SpriteValuedBase

        #region Property: Dimensions
        /// <summary>
        /// A handler for a changed dimensions.
        /// </summary>
        Vec2.Vec2Handler dimensionsChanged = null;

        /// <summary>
        /// Gets the dimensions.
        /// </summary>
        public Vec2 Dimensions
        {
            get
            {
                if (this.SpriteBase != null)
                    return GetProperty<Vec2>("Dimensions", this.SpriteBase.Dimensions);

                return new Vec2(GameSemanticsSettings.Values.DimensionX, GameSemanticsSettings.Values.DimensionY);
            }
            protected set
            {
                Vec2 dimensions = this.Dimensions;
                if (dimensions != value)
                {
                    if (dimensionsChanged == null)
                        dimensionsChanged = new Vec2.Vec2Handler(dimensions_ValueChanged);

                    if (dimensions != null)
                        dimensions.ValueChanged -= dimensionsChanged;

                    SetProperty("Dimensions", value);

                    if (value != null)
                        value.ValueChanged += dimensionsChanged;
                }
            }
        }

        /// <summary>
        /// Notify any changes of the dimensions.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        private void dimensions_ValueChanged(Vec2 vector)
        {
            NotifyPropertyChanged("Dimensions");
        }
        #endregion Property: Dimensions

        #region Property: Translation
        /// <summary>
        /// A handler for a changed translation.
        /// </summary>
        Vec2.Vec2Handler translationChanged = null;
        
        /// <summary>
        /// Gets the translation.
        /// </summary>
        public Vec2 Translation
        {
            get
            {
                if (this.SpriteValuedBase != null)
                    return GetProperty<Vec2>("Translation", this.SpriteValuedBase.Translation);

                return new Vec2(GameSemanticsSettings.Values.TranslationX, GameSemanticsSettings.Values.TranslationY);
            }
            set
            {
                Vec2 translation = this.Translation;
                if (translation != value)
                {
                    if (translationChanged == null)
                        translationChanged = new Vec2.Vec2Handler(translation_ValueChanged);

                    if (translation != null)
                        translation.ValueChanged -= translationChanged;

                    SetProperty("Translation", value);

                    if (value != null)
                        value.ValueChanged += translationChanged;
                }
            }
        }

        /// <summary>
        /// Notify any changes of the translation.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        private void translation_ValueChanged(Vec2 vector)
        {
            NotifyPropertyChanged("Translation");
        }
        #endregion Property: Translation

        #region Property: Rotation
        /// <summary>
        /// Gets the rotation (in degrees).
        /// </summary>
        public float Rotation
        {
            get
            {
                if (this.SpriteValuedBase != null)
                    return GetProperty<float>("Rotation", this.SpriteValuedBase.Rotation);

                return GameSemanticsSettings.Values.Rotation;
            }
            set
            {
                if (this.Rotation != value)
                    SetProperty("Rotation", value);
            }
        }
        #endregion Property: Rotation

        #region Property: Scale
        /// <summary>
        /// A handler for a changed scale.
        /// </summary>
        Vec2.Vec2Handler scaleChanged = null;
        
        /// <summary>
        /// Gets the scale.
        /// </summary>
        public Vec2 Scale
        {
            get
            {
                if (this.SpriteValuedBase != null)
                    return GetProperty<Vec2>("Scale", this.SpriteValuedBase.Scale);

                return new Vec2(GameSemanticsSettings.Values.ScaleX, GameSemanticsSettings.Values.ScaleY);
            }
            set
            {
                Vec2 scale = this.Scale;
                if (scale != value)
                {
                    if (scaleChanged == null)
                        scaleChanged = new Vec2.Vec2Handler(scale_ValueChanged);

                    if (scale != null)
                        scale.ValueChanged -= scaleChanged;

                    SetProperty("Scale", value);

                    if (value != null)
                        value.ValueChanged += scaleChanged;
                }
            }
        }

        /// <summary>
        /// Notify any changes of the scale.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        private void scale_ValueChanged(Vec2 vector)
        {
            NotifyPropertyChanged("Scale");
        }
        #endregion Property: Scale

        #region Property: ZIndex
        /// <summary>
        /// Gets the Z-index.
        /// </summary>
        public int ZIndex
        {
            get
            {
                if (this.SpriteBase != null)
                    return GetProperty<int>("ZIndex", this.SpriteBase.ZIndex);

                return GameSemanticsSettings.Values.ZIndex;
            }
            set
            {
                if (this.ZIndex != value)
                    SetProperty("ZIndex", value);
            }
        }
        #endregion Property: ZIndex

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SpriteInstance(SpriteValuedBase spriteValuedBase)
        /// <summary>
        /// Creates a new sprite instance from the given valued sprite base.
        /// </summary>
        /// <param name="spriteValuedBase">The valued sprite base to create the sprite instance from.</param>
        internal SpriteInstance(SpriteValuedBase spriteValuedBase)
            : base(spriteValuedBase)
        {
        }
        #endregion Constructor: SpriteInstance(SpriteValuedBase spriteValuedBase)

        #region Constructor: SpriteInstance(SpriteInstance spriteInstance)
        /// <summary>
        /// Clones a sprite instance.
        /// </summary>
        /// <param name="spriteInstance">The sprite instance to clone.</param>
        protected internal SpriteInstance(SpriteInstance spriteInstance)
            : base(spriteInstance)
        {
            if (spriteInstance != null)
            {
                if (spriteInstance.Dimensions != null)
                    this.Dimensions = new Vec2(spriteInstance.Dimensions);
                else
                    this.Dimensions = null;
                if (spriteInstance.Translation != null)
                    this.Translation = new Vec2(spriteInstance.Translation);
                else
                    this.Translation = null;
                this.Rotation = spriteInstance.Rotation;
                if (spriteInstance.Scale != null)
                    this.Scale = new Vec2(spriteInstance.Scale);
                else
                    this.Scale = null;
                this.ZIndex = spriteInstance.ZIndex;
            }
        }
        #endregion Constructor: SpriteInstance(SpriteInstance spriteInstance)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the sprite instance satisfies the given condition.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the sprite instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the sprite instance is satisfies the given condition.</returns>
        public override bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            // Check whether the base satisfies the condition
            if (conditionBase != null && base.Satisfies(conditionBase, iVariableInstanceHolder))
            {
                SpriteConditionBase spriteConditionBase = conditionBase as SpriteConditionBase;
                if (spriteConditionBase != null)
                {
                    // Check whether all the properties have the correct values
                    if ((spriteConditionBase.TranslationSign == null || spriteConditionBase.Translation == null || Toolbox.Compare(this.Translation, (EqualitySignExtended)spriteConditionBase.TranslationSign, (Vec2)spriteConditionBase.Translation)) &&
                        (spriteConditionBase.RotationSign == null || spriteConditionBase.Rotation == null || Toolbox.Compare(this.Rotation, (EqualitySignExtended)spriteConditionBase.RotationSign, (float)spriteConditionBase.Rotation)) &&
                        (spriteConditionBase.ScaleSign == null || spriteConditionBase.Scale == null || Toolbox.Compare(this.Scale, (EqualitySignExtended)spriteConditionBase.ScaleSign, (Vec2)spriteConditionBase.Scale)) &&
                        (spriteConditionBase.ZIndexSign == null || spriteConditionBase.ZIndex == null || Toolbox.Compare(this.ZIndex, (EqualitySignExtended)spriteConditionBase.ZIndexSign, (float)spriteConditionBase.ZIndex)))
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
        /// Apply the change to the sprite instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the sprite instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        protected internal override bool Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (changeBase != null && base.Apply(changeBase, iVariableInstanceHolder))
            {
                SpriteChangeBase spriteChangeBase = changeBase as SpriteChangeBase;
                if (spriteChangeBase != null)
                {
                    // Apply all changes
                    if (spriteChangeBase.TranslationChange != null && spriteChangeBase.Translation != null)
                        this.Translation = Toolbox.CalcValue(this.Translation, (ValueChangeType)spriteChangeBase.TranslationChange, (Vec2)spriteChangeBase.Translation);
                    if (spriteChangeBase.Rotation != null && spriteChangeBase.RotationChange != null)
                        this.Rotation = Toolbox.CalcValue(this.Rotation, (ValueChangeType)spriteChangeBase.RotationChange, (float)spriteChangeBase.Rotation);
                    if (spriteChangeBase.ScaleChange != null && spriteChangeBase.Scale != null)
                        this.Scale = Toolbox.CalcValue(this.Scale, (ValueChangeType)spriteChangeBase.ScaleChange, (Vec2)spriteChangeBase.Scale);
                    if (spriteChangeBase.ZIndex != null && spriteChangeBase.ZIndexChange != null)
                        this.ZIndex = Toolbox.CalcValue(this.ZIndex, (ValueChangeType)spriteChangeBase.ZIndexChange, (int)spriteChangeBase.ZIndex);
                }
                return true;
            }
            return false;
        }
        #endregion Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: SpriteInstance

}