/**************************************************************************
 * 
 * SpriteBase.cs
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
using GameSemantics.GameContent;
using GameSemantics.Utilities;
using Semantics.Utilities;

namespace GameSemanticsEngine.GameContent
{

    #region Class: SpriteBase
    /// <summary>
    /// A base of a sprite.
    /// </summary>
    public class SpriteBase : StaticContentBase
    {

        #region Properties and Fields

        #region Property: Sprite
        /// <summary>
        /// Gets the sprite of which this is a sprite base.
        /// </summary>
        protected internal Sprite Sprite
        {
            get
            {
                return this.Node as Sprite;
            }
        }
        #endregion Property: Sprite

        #region Property: Dimensions
        /// <summary>
        /// The dimensions.
        /// </summary>
        private Vec2 dimensions = new Vec2(GameSemanticsSettings.Values.DimensionX, GameSemanticsSettings.Values.DimensionY);

        /// <summary>
        /// Gets the dimensions.
        /// </summary>
        public Vec2 Dimensions
        {
            get
            {
                return dimensions;
            }
        }
        #endregion Property: Dimensions

        #region Property: ZIndex
        /// <summary>
        /// The Z-index.
        /// </summary>
        private int zIndex = GameSemanticsSettings.Values.ZIndex;

        /// <summary>
        /// Gets the Z-index.
        /// </summary>
        public int ZIndex
        {
            get
            {
                return zIndex;
            }
        }
        #endregion Property: ZIndex

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SpriteBase(Sprite sprite)
        /// <summary>
        /// Creates a new sprite base from the given sprite.
        /// </summary>
        /// <param name="sprite">The sprite to create a sprite base from.</param>
        protected internal SpriteBase(Sprite sprite)
            : base(sprite)
        {
            if (sprite != null)
            {
                Vec2 spriteDimensions = sprite.Dimensions;
                if (spriteDimensions != null)
                    this.dimensions = new Vec2(spriteDimensions);
                this.zIndex = sprite.ZIndex;
            }
        }
        #endregion Constructor: SpriteBase(Sprite sprite)

        #endregion Method Group: Constructors

    }
    #endregion Class: SpriteBase

    #region Class: SpriteValuedBase
    /// <summary>
    /// A base of a valued sprite.
    /// </summary>
    public class SpriteValuedBase : StaticContentValuedBase
    {

        #region Properties and Fields

        #region Property: SpriteValued
        /// <summary>
        /// Gets the valued sprite of which this is a valued sprite base.
        /// </summary>
        protected internal SpriteValued SpriteValued
        {
            get
            {
                return this.NodeValued as SpriteValued;
            }
        }
        #endregion Property: SpriteValued

        #region Property: SpriteBase
        /// <summary>
        /// Gets the sprite base.
        /// </summary>
        public SpriteBase SpriteBase
        {
            get
            {
                return this.NodeBase as SpriteBase;
            }
        }
        #endregion Property: SpriteBase

        #region Property: Translation
        /// <summary>
        /// The translation.
        /// </summary>
        private Vec2 translation = new Vec2(GameSemanticsSettings.Values.TranslationX, GameSemanticsSettings.Values.TranslationY);

        /// <summary>
        /// Gets the translation.
        /// </summary>
        public Vec2 Translation
        {
            get
            {
                return translation;
            }
        }
        #endregion Property: Translation

        #region Property: Rotation
        /// <summary>
        /// The rotation (in degrees).
        /// </summary>
        private float rotation = GameSemanticsSettings.Values.Rotation;

        /// <summary>
        /// Gets the rotation (in degrees).
        /// </summary>
        public float Rotation
        {
            get
            {
                return rotation;
            }
        }
        #endregion Property: Rotation

        #region Property: Scale
        /// <summary>
        /// The scale.
        /// </summary>
        private Vec2 scale = new Vec2(GameSemanticsSettings.Values.ScaleX, GameSemanticsSettings.Values.ScaleY);

        /// <summary>
        /// Gets the scale.
        /// </summary>
        public Vec2 Scale
        {
            get
            {
                return scale;
            }
        }
        #endregion Property: Scale

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SpriteValuedBase(SpriteValued spriteValued)
        /// <summary>
        /// Create a valued sprite base from the given valued sprite.
        /// </summary>
        /// <param name="spriteValued">The valued sprite to create a valued sprite base from.</param>
        protected internal SpriteValuedBase(SpriteValued spriteValued)
            : base(spriteValued)
        {
            if (spriteValued != null)
            {
                Vec2 spriteValuedTranslation = spriteValued.Translation;
                if (spriteValuedTranslation != null)
                    this.translation = new Vec2(spriteValuedTranslation);

                this.rotation = spriteValued.Rotation;

                Vec2 spriteValuedScale = spriteValued.Scale;
                if (spriteValuedScale != null)
                    this.scale = new Vec2(spriteValuedScale);
            }
        }
        #endregion Constructor: SpriteValuedBase(SpriteValued spriteValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: SpriteValuedBase

    #region Class: SpriteConditionBase
    /// <summary>
    /// A condition on a sprite.
    /// </summary>
    public class SpriteConditionBase : StaticContentConditionBase
    {

        #region Properties and Fields

        #region Property: SpriteCondition
        /// <summary>
        /// Gets the sprite condition of which this is a sprite condition base.
        /// </summary>
        protected internal SpriteCondition SpriteCondition
        {
            get
            {
                return this.Condition as SpriteCondition;
            }
        }
        #endregion Property: SpriteCondition

        #region Property: SpriteBase
        /// <summary>
        /// Gets the sprite base of which this is a sprite condition base.
        /// </summary>
        public SpriteBase SpriteBase
        {
            get
            {
                return this.NodeBase as SpriteBase;
            }
        }
        #endregion Property: SpriteBase

        #region Property: Translation
        /// <summary>
        /// The required translation.
        /// </summary>
        private Vec2 translation = null;

        /// <summary>
        /// Gets the required translation.
        /// </summary>
        public Vec2 Translation
        {
            get
            {
                return translation;
            }
        }
        #endregion Property: Translation

        #region Property: TranslationSign
        /// <summary>
        /// The sign for the translation in the condition.
        /// </summary>
        private EqualitySignExtended? translationSign = null;
        
        /// <summary>
        /// Gets the sign for the translation in the condition.
        /// </summary>
        public EqualitySignExtended? TranslationSign
        {
            get
            {
                return translationSign;
            }
        }
        #endregion Property: TranslationSign

        #region Property: Rotation
        /// <summary>
        /// The required rotation.
        /// </summary>
        private float? rotation = null;
        
        /// <summary>
        /// Gets the required rotation.
        /// </summary>
        public float? Rotation
        {
            get
            {
                return rotation;
            }
        }
        #endregion Property: Rotation

        #region Property: RotationSign
        /// <summary>
        /// The sign for the rotation in the condition.
        /// </summary>
        private EqualitySignExtended? rotationSign = null;
        
        /// <summary>
        /// Gets the sign for the rotation in the condition.
        /// </summary>
        public EqualitySignExtended? RotationSign
        {
            get
            {
                return rotationSign;
            }
        }
        #endregion Property: RotationSign

        #region Property: Scale
        /// <summary>
        /// The required scale.
        /// </summary>
        private Vec2 scale = null;

        /// <summary>
        /// Gets the required scale.
        /// </summary>
        public Vec2 Scale
        {
            get
            {
                return scale;
            }
        }
        #endregion Property: Scale

        #region Property: ScaleSign
        /// <summary>
        /// The sign for the scale in the condition.
        /// </summary>
        private EqualitySignExtended? scaleSign = null;

        /// <summary>
        /// Gets the sign for the scale in the condition.
        /// </summary>
        public EqualitySignExtended? ScaleSign
        {
            get
            {
                return scaleSign;
            }
        }
        #endregion Property: ScaleSign

        #region Property: ZIndex
        /// <summary>
        /// The required Z-index.
        /// </summary>
        private int? zIndex = null;

        /// <summary>
        /// Gets the required Z-index.
        /// </summary>
        public int? ZIndex
        {
            get
            {
                return zIndex;
            }
        }
        #endregion Property: ZIndex

        #region Property: ZIndexSign
        /// <summary>
        /// The sign for the Z-index in the condition.
        /// </summary>
        private EqualitySignExtended? zIndexSign = null;

        /// <summary>
        /// Gets the sign for the Z-index in the condition.
        /// </summary>
        public EqualitySignExtended? ZIndexSign
        {
            get
            {
                return zIndexSign;
            }
        }
        #endregion Property: ZIndexSign

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SpriteConditionBase(SpriteCondition spriteCondition)
        /// <summary>
        /// Creates a base of the given sprite condition.
        /// </summary>
        /// <param name="spriteCondition">The sprite condition to create a base of.</param>
        protected internal SpriteConditionBase(SpriteCondition spriteCondition)
            : base(spriteCondition)
        {
            if (spriteCondition != null)
            {
                if (spriteCondition.Translation != null)
                    this.translation = new Vec2(spriteCondition.Translation);
                this.translationSign = spriteCondition.TranslationSign;

                this.rotation = spriteCondition.Rotation;
                this.rotationSign = spriteCondition.RotationSign;

                if (spriteCondition.Scale != null)
                    this.scale = new Vec2(spriteCondition.Scale);
                this.scaleSign = spriteCondition.ScaleSign;

                this.zIndex = spriteCondition.ZIndex;
                this.zIndexSign = spriteCondition.ZIndexSign;
            }
        }
        #endregion Constructor: SpriteConditionBase(SpriteCondition spriteCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: SpriteConditionBase

    #region Class: SpriteChangeBase
    /// <summary>
    /// A change on a sprite.
    /// </summary>
    public class SpriteChangeBase : StaticContentChangeBase
    {

        #region Properties and Fields

        #region Property: SpriteChange
        /// <summary>
        /// Gets the sprite change of which this is a sprite change base.
        /// </summary>
        protected internal SpriteChange SpriteChange
        {
            get
            {
                return this.Change as SpriteChange;
            }
        }
        #endregion Property: SpriteChange

        #region Property: SpriteBase
        /// <summary>
        /// Gets the affected sprite base.
        /// </summary>
        public SpriteBase SpriteBase
        {
            get
            {
                return this.NodeBase as SpriteBase;
            }
        }
        #endregion Property: SpriteBase

        #region Property: Translation
        /// <summary>
        /// The translation to change to.
        /// </summary>
        private Vec2 translation = null;

        /// <summary>
        /// Gets the translation to change to.
        /// </summary>
        public Vec2 Translation
        {
            get
            {
                return translation;
            }
        }
        #endregion Property: Translation

        #region Property: TranslationChange
        /// <summary>
        /// The type of change for the translation.
        /// </summary>
        private ValueChangeType? translationChange = null;
        
        /// <summary>
        /// Gets the type of change for the translation.
        /// </summary>
        public ValueChangeType? TranslationChange
        {
            get
            {
                return translationChange;
            }
        }
        #endregion Property: TranslationChange

        #region Property: Rotation
        /// <summary>
        /// The rotation to change to.
        /// </summary>
        private float? rotation = null;
        
        /// <summary>
        /// Gets the rotation to change to.
        /// </summary>
        public float? Rotation
        {
            get
            {
                return rotation;
            }
        }
        #endregion Property: Rotation

        #region Property: RotationChange
        /// <summary>
        /// The type of change for the rotation.
        /// </summary>
        private ValueChangeType? rotationChange = null;
        
        /// <summary>
        /// Gets the type of change for the rotation.
        /// </summary>
        public ValueChangeType? RotationChange
        {
            get
            {
                return rotationChange;
            }
        }
        #endregion Property: RotationChange

        #region Property: Scale
        /// <summary>
        /// The scale to change to.
        /// </summary>
        private Vec2 scale = null;

        /// <summary>
        /// Gets or sets the scale to change to.
        /// </summary>
        public Vec2 Scale
        {
            get
            {
                return scale;
            }
        }
        #endregion Property: Scale

        #region Property: ScaleChange
        /// <summary>
        /// The type of change for the scale.
        /// </summary>
        private ValueChangeType? scaleChange = null;

        /// <summary>
        /// Gets the type of change for the scale.
        /// </summary>
        public ValueChangeType? ScaleChange
        {
            get
            {
                return scaleChange;
            }
        }
        #endregion Property: ScaleChange

        #region Property: ZIndex
        /// <summary>
        /// The Z-index to change to.
        /// </summary>
        private int? zIndex = null;

        /// <summary>
        /// Gets the Z-index to change to.
        /// </summary>
        public int? ZIndex
        {
            get
            {
                return zIndex;
            }
        }
        #endregion Property: ZIndex

        #region Property: ZIndexChange
        /// <summary>
        /// The type of change for the Z-index.
        /// </summary>
        private ValueChangeType? zIndexChange = null;

        /// <summary>
        /// Gets the type of change for the Z-index.
        /// </summary>
        public ValueChangeType? ZIndexChange
        {
            get
            {
                return zIndexChange;
            }
        }
        #endregion Property: ZIndexChange

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SpriteChangeBase(SpriteChange spriteChange)
        /// <summary>
        /// Creates a base of the given sprite change.
        /// </summary>
        /// <param name="spriteChange">The sprite change to create a base of.</param>
        protected internal SpriteChangeBase(SpriteChange spriteChange)
            : base(spriteChange)
        {
            if (spriteChange != null)
            {
                if (spriteChange.Translation != null)
                    this.translation = new Vec2(spriteChange.Translation);
                this.translationChange = spriteChange.TranslationChange;

                this.rotation = spriteChange.Rotation;
                this.rotationChange = spriteChange.RotationChange;

                if (spriteChange.Scale != null)
                    this.scale = new Vec2(spriteChange.Scale);
                this.scaleChange = spriteChange.ScaleChange;

                this.zIndex = spriteChange.ZIndex;
                this.zIndexChange = spriteChange.ZIndexChange;
            }
        }
        #endregion Constructor: SpriteChangeBase(SpriteChange spriteChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: SpriteChangeBase

}