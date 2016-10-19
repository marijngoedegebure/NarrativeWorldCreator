/**************************************************************************
 * 
 * Sprite.cs
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
using Common;
using GameSemantics.Data;
using GameSemantics.Utilities;
using Semantics.Utilities;

namespace GameSemantics.GameContent
{

    #region Class: Sprite
    /// <summary>
    /// A 2-dimensional sprite.
    /// </summary>
    public class Sprite : StaticContent, IComparable<Sprite>
    {

        #region Properties and Fields

        #region Property: Dimensions
        /// <summary>
        /// Gets the X- and Y-dimensions of the sprite, expressed in the special distance unit.
        /// </summary>
        /// <summary>
        /// A handler for a change in the dimensions.
        /// </summary>
        private Vec2.Vec2Handler dimensionsChanged;

        /// <summary>
        /// The dimensions.
        /// </summary>
        private Vec2 dimensions = null;

        /// <summary>
        /// Gets or sets the dimensions.
        /// </summary>
        public Vec2 Dimensions
        {
            get
            {
                if (dimensions == null)
                {
                    dimensions = GameDatabase.Current.Select<Vec2>(this.ID, GameTables.Sprite, GameColumns.Dimensions);

                    if (dimensions == null)
                        dimensions = new Vec2(GameSemanticsSettings.Values.DimensionX, GameSemanticsSettings.Values.DimensionY);

                    if (dimensionsChanged == null)
                        dimensionsChanged = new Vec2.Vec2Handler(dimensions_ValueChanged);

                    dimensions.ValueChanged += dimensionsChanged;
                }

                return dimensions;
            }
            set
            {
                if (dimensionsChanged == null)
                    dimensionsChanged = new Vec2.Vec2Handler(dimensions_ValueChanged);

                if (dimensions != null)
                    dimensions.ValueChanged -= dimensionsChanged;

                dimensions = value;
                GameDatabase.Current.Update(this.ID, GameTables.Sprite, GameColumns.Dimensions, value);
                NotifyPropertyChanged("Dimensions");

                if (dimensions != null)
                    dimensions.ValueChanged += dimensionsChanged;
            }
        }

        /// <summary>
        /// Updates the database when a value of the dimensions changes.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        private void dimensions_ValueChanged(Vec2 vector)
        {
            this.Dimensions = dimensions;
        }
        #endregion Property: Dimensions

        #region Property: ZIndex
        /// <summary>
        /// Gets or sets the Z-index of the sprite.
        /// </summary>
        public int ZIndex
        {
            get
            {
                return GameDatabase.Current.Select<int>(this.ID, GameTables.Sprite, GameColumns.ZIndex);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameTables.Sprite, GameColumns.ZIndex, value);
                NotifyPropertyChanged("ZIndex");
            }
        }
        #endregion Property: ZIndex

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: Sprite()
        /// <summary>
        /// Creates a new sprite.
        /// </summary>
        public Sprite()
            : base()
        {
        }
        #endregion Constructor: Sprite()

        #region Constructor: Sprite(uint id)
        /// <summary>
        /// Creates a new sprite from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a sprite from.</param>
        protected Sprite(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Sprite(uint id)

        #region Constructor: Sprite(string file)
        /// <summary>
        /// Creates a new sprite with the given file.
        /// </summary>
        /// <param name="file">The file to assign to the sprite.</param>
        public Sprite(string file)
            : base(file)
        {
        }
        #endregion Constructor: Sprite(string file)

        #region Constructor: Sprite(Sprite sprite)
        /// <summary>
        /// Clones a sprite.
        /// </summary>
        /// <param name="sprite">The sprite to clone.</param>
        public Sprite(Sprite sprite)
            : base(sprite)
        {
            if (sprite != null)
            {
                if (sprite.Dimensions != null)
                    this.Dimensions = new Vec2(sprite.Dimensions);
                this.ZIndex = sprite.ZIndex;
            }
        }
        #endregion Constructor: Sprite(Sprite sprite)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the sprite.
        /// </summary>
        public override void Remove()
        {
            GameDatabase.Current.StartChange();
            GameDatabase.Current.StartRemove();

            base.Remove();

            GameDatabase.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(Sprite other)
        /// <summary>
        /// Compares the sprite to the other sprite.
        /// </summary>
        /// <param name="other">The sprite to compare to this sprite.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Sprite other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(Sprite other)

        #endregion Method Group: Other

    }
    #endregion Class: Sprite

    #region Class: SpriteValued
    /// <summary>
    /// A valued version of a sprite.
    /// </summary>
    public class SpriteValued : StaticContentValued
    {

        #region Properties and Fields

        #region Property: Sprite
        /// <summary>
        /// Gets the sprite of which this is a valued sprite.
        /// </summary>
        public Sprite Sprite
        {
            get
            {
                return this.Node as Sprite;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: Sprite

        #region Property: Translation
        /// <summary>
        /// A handler for a change in the translation.
        /// </summary>
        private Vec2.Vec2Handler translationChanged;

        /// <summary>
        /// The translation.
        /// </summary>
        private Vec2 translation = null;

        /// <summary>
        /// Gets or sets the translation.
        /// </summary>
        public Vec2 Translation
        {
            get
            {
                if (translation == null)
                {
                    translation = GameDatabase.Current.Select<Vec2>(this.ID, GameValueTables.SpriteValued, GameColumns.Translation);

                    if (translation == null)
                        translation = new Vec2(GameSemanticsSettings.Values.TranslationX, GameSemanticsSettings.Values.TranslationY);

                    if (translationChanged == null)
                        translationChanged = new Vec2.Vec2Handler(translation_ValueChanged);

                    translation.ValueChanged += translationChanged;
                }

                return translation;
            }
            set
            {
                if (translationChanged == null)
                    translationChanged = new Vec2.Vec2Handler(translation_ValueChanged);

                if (translation != null)
                    translation.ValueChanged -= translationChanged;

                translation = value;
                GameDatabase.Current.Update(this.ID, GameValueTables.SpriteValued, GameColumns.Translation, translation);
                NotifyPropertyChanged("Translation");

                if (translation != null)
                    translation.ValueChanged += translationChanged;
            }
        }

        /// <summary>
        /// Updates the database when a value of the translation changes.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        private void translation_ValueChanged(Vec2 vector)
        {
            this.Translation = translation;
        }
        #endregion Property: Translation

        #region Property: Rotation
        /// <summary>
        /// Gets or sets the rotation (in degrees).
        /// </summary>
        public float Rotation
        {
            get
            {
                return GameDatabase.Current.Select<float>(this.ID, GameValueTables.SpriteValued, GameColumns.Rotation);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.SpriteValued, GameColumns.Rotation, value);
                NotifyPropertyChanged("Rotation");
            }
        }
        #endregion Property: Rotation

        #region Property: Scale
        /// <summary>
        /// A handler for a change in the scale.
        /// </summary>
        private Vec2.Vec2Handler scaleChanged;

        /// <summary>
        /// The scale.
        /// </summary>
        private Vec2 scale = null;

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        public Vec2 Scale
        {
            get
            {
                if (scale == null)
                {
                    scale = GameDatabase.Current.Select<Vec2>(this.ID, GameValueTables.SpriteValued, GameColumns.Scale);

                    if (scale == null)
                        scale = new Vec2(GameSemanticsSettings.Values.ScaleX, GameSemanticsSettings.Values.ScaleY);

                    if (scaleChanged == null)
                        scaleChanged = new Vec2.Vec2Handler(scale_ValueChanged);

                    scale.ValueChanged += scaleChanged;
                }

                return scale;
            }
            set
            {
                if (scaleChanged == null)
                    scaleChanged = new Vec2.Vec2Handler(scale_ValueChanged);

                if (scale != null)
                    scale.ValueChanged -= scaleChanged;

                scale = value;
                GameDatabase.Current.Update(this.ID, GameValueTables.SpriteValued, GameColumns.Scale, scale);
                NotifyPropertyChanged("Scale");

                if (scale != null)
                    scale.ValueChanged += scaleChanged;
            }
        }

        /// <summary>
        /// Updates the database when a value of the scale changes.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        private void scale_ValueChanged(Vec2 vector)
        {
            this.Scale = scale;
        }
        #endregion Property: Scale

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SpriteValued(uint id)
        /// <summary>
        /// Creates a new valued sprite from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the valued sprite from.</param>
        protected SpriteValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: SpriteValued(uint id)

        #region Constructor: SpriteValued(SpriteValued spriteValued)
        /// <summary>
        /// Clones a valued sprite.
        /// </summary>
        /// <param name="spriteValued">The valued sprite to clone.</param>
        public SpriteValued(SpriteValued spriteValued)
            : base(spriteValued)
        {
            if (spriteValued != null)
            {
                GameDatabase.Current.StartChange();

                if (spriteValued.Translation != null)
                    this.Translation = new Vec2(spriteValued.Translation);
                this.Rotation = spriteValued.Rotation;
                if (spriteValued.Scale != null)
                    this.Scale = new Vec2(spriteValued.Scale);

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: SpriteValued(SpriteValued spriteValued)

        #region Constructor: SpriteValued(Sprite sprite)
        /// <summary>
        /// Creates a new valued sprite from the given sprite.
        /// </summary>
        /// <param name="sprite">The sprite to create a valued sprite from.</param>
        public SpriteValued(Sprite sprite)
            : base(sprite)
        {
        }
        #endregion Constructor: SpriteValued(Sprite sprite)

        #endregion Method Group: Constructors

    }
    #endregion Class: SpriteValued

    #region Class: SpriteCondition
    /// <summary>
    /// A condition on a sprite.
    /// </summary>
    public class SpriteCondition : StaticContentCondition
    {

        #region Properties and Fields

        #region Property: Sprite
        /// <summary>
        /// Gets or sets the required sprite.
        /// </summary>
        public Sprite Sprite
        {
            get
            {
                return this.Node as Sprite;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Sprite

        #region Property: Translation
        /// <summary>
        /// A handler for a change in the translation.
        /// </summary>
        private Vec2.Vec2Handler translationChanged;

        /// <summary>
        /// The required translation.
        /// </summary>
        private Vec2 translation = null;

        /// <summary>
        /// Gets or sets the required translation.
        /// </summary>
        public Vec2 Translation
        {
            get
            {
                if (translation == null)
                {
                    translation = GameDatabase.Current.Select<Vec2>(this.ID, GameValueTables.SpriteCondition, GameColumns.Translation);

                    if (translation != null)
                    {
                        if (translationChanged == null)
                            translationChanged = new Vec2.Vec2Handler(translation_ValueChanged);

                        translation.ValueChanged += translationChanged;
                    }
                }

                return translation;
            }
            set
            {
                if (translationChanged == null)
                    translationChanged = new Vec2.Vec2Handler(translation_ValueChanged);

                if (translation != null)
                    translation.ValueChanged -= translationChanged;

                translation = value;
                GameDatabase.Current.Update(this.ID, GameValueTables.SpriteCondition, GameColumns.Translation, translation);
                NotifyPropertyChanged("Translation");

                if (translation != null)
                    translation.ValueChanged += translationChanged;
            }
        }

        /// <summary>
        /// Updates the database when a value of the translation changes.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        private void translation_ValueChanged(Vec2 vector)
        {
            GameDatabase.Current.Update(this.ID, GameValueTables.SpriteCondition, GameColumns.Translation, translation);
            NotifyPropertyChanged("Translation");
        }
        #endregion Property: Translation

        #region Property: TranslationSign
        /// <summary>
        /// Gets or sets the sign for the translation in the condition.
        /// </summary>
        public EqualitySignExtended? TranslationSign
        {
            get
            {
                return GameDatabase.Current.Select<EqualitySignExtended?>(this.ID, GameValueTables.SpriteCondition, GameColumns.TranslationSign);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.SpriteCondition, GameColumns.TranslationSign, value);
                NotifyPropertyChanged("TranslationSign");
            }
        }
        #endregion Property: TranslationSign

        #region Property: Rotation
        /// <summary>
        /// Gets or sets the required rotation.
        /// </summary>
        public float? Rotation
        {
            get
            {
                return GameDatabase.Current.Select<float?>(this.ID, GameValueTables.SpriteCondition, GameColumns.Rotation);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.SpriteCondition, GameColumns.Rotation, value);
                NotifyPropertyChanged("Rotation");
            }
        }
        #endregion Property: Rotation

        #region Property: RotationSign
        /// <summary>
        /// Gets or sets the sign for the rotation in the condition.
        /// </summary>
        public EqualitySignExtended? RotationSign
        {
            get
            {
                return GameDatabase.Current.Select<EqualitySignExtended?>(this.ID, GameValueTables.SpriteCondition, GameColumns.RotationSign);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.SpriteCondition, GameColumns.RotationSign, value);
                NotifyPropertyChanged("RotationSign");
            }
        }
        #endregion Property: RotationSign

        #region Property: Scale
        /// <summary>
        /// A handler for a change in the scale.
        /// </summary>
        private Vec2.Vec2Handler scaleChanged;

        /// <summary>
        /// The required scale.
        /// </summary>
        private Vec2 scale = null;

        /// <summary>
        /// Gets or sets the required scale.
        /// </summary>
        public Vec2 Scale
        {
            get
            {
                if (scale == null)
                {
                    scale = GameDatabase.Current.Select<Vec2>(this.ID, GameValueTables.SpriteCondition, GameColumns.Scale);

                    if (scale != null)
                    {
                        if (scaleChanged == null)
                            scaleChanged = new Vec2.Vec2Handler(scale_ValueChanged);

                        scale.ValueChanged += scaleChanged;
                    }
                }

                return scale;
            }
            set
            {
                if (scaleChanged == null)
                    scaleChanged = new Vec2.Vec2Handler(scale_ValueChanged);

                if (scale != null)
                    scale.ValueChanged -= scaleChanged;

                scale = value;
                GameDatabase.Current.Update(this.ID, GameValueTables.SpriteCondition, GameColumns.Scale, scale);
                NotifyPropertyChanged("Scale");

                if (scale != null)
                    scale.ValueChanged += scaleChanged;
            }
        }

        /// <summary>
        /// Updates the database when a value of the scale changes.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        private void scale_ValueChanged(Vec2 vector)
        {
            GameDatabase.Current.Update(this.ID, GameValueTables.SpriteCondition, GameColumns.Scale, scale);
            NotifyPropertyChanged("Scale");
        }
        #endregion Property: Scale

        #region Property: ScaleSign
        /// <summary>
        /// Gets or sets the sign for the scale in the condition.
        /// </summary>
        public EqualitySignExtended? ScaleSign
        {
            get
            {
                return GameDatabase.Current.Select<EqualitySignExtended?>(this.ID, GameValueTables.SpriteCondition, GameColumns.ScaleSign);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.SpriteCondition, GameColumns.ScaleSign, value);
                NotifyPropertyChanged("ScaleSign");
            }
        }
        #endregion Property: ScaleSign

        #region Property: ZIndex
        /// <summary>
        /// Gets or sets the required Z-index.
        /// </summary>
        public int? ZIndex
        {
            get
            {
                return GameDatabase.Current.Select<int?>(this.ID, GameValueTables.SpriteCondition, GameColumns.ZIndex);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.SpriteCondition, GameColumns.ZIndex, value);
                NotifyPropertyChanged("ZIndex");
            }
        }
        #endregion Property: ZIndex

        #region Property: ZIndexSign
        /// <summary>
        /// Gets or sets the sign for the Z-index in the condition.
        /// </summary>
        public EqualitySignExtended? ZIndexSign
        {
            get
            {
                return GameDatabase.Current.Select<EqualitySignExtended?>(this.ID, GameValueTables.SpriteCondition, GameColumns.ZIndexSign);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.SpriteCondition, GameColumns.ZIndexSign, value);
                NotifyPropertyChanged("ZIndexSign");
            }
        }
        #endregion Property: ZIndexSign

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SpriteCondition()
        /// <summary>
        /// Creates a new sprite condition.
        /// </summary>
        public SpriteCondition()
            : base()
        {
        }
        #endregion Constructor: SpriteCondition()

        #region Constructor: SpriteCondition(uint id)
        /// <summary>
        /// Creates a new sprite condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the sprite condition from.</param>
        protected SpriteCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: SpriteCondition(uint id)

        #region Constructor: SpriteCondition(SpriteCondition spriteCondition)
        /// <summary>
        /// Clones a sprite condition.
        /// </summary>
        /// <param name="spriteCondition">The sprite condition to clone.</param>
        public SpriteCondition(SpriteCondition spriteCondition)
            : base(spriteCondition)
        {
            if (spriteCondition != null)
            {
                GameDatabase.Current.StartChange();

                if (spriteCondition.Translation != null)
                    this.Translation = new Vec2(spriteCondition.Translation);
                this.TranslationSign = spriteCondition.TranslationSign;

                this.Rotation = spriteCondition.Rotation;
                this.RotationSign = spriteCondition.RotationSign;

                if (spriteCondition.Scale != null)
                    this.Scale = new Vec2(spriteCondition.Scale);
                this.ScaleSign = spriteCondition.ScaleSign;

                this.ZIndex = spriteCondition.ZIndex;
                this.ZIndexSign = spriteCondition.ZIndexSign;

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: SpriteCondition(SpriteCondition spriteCondition)

        #region Constructor: SpriteCondition(Sprite sprite)
        /// <summary>
        /// Creates a condition for the given sprite.
        /// </summary>
        /// <param name="sprite">The sprite to create a condition for.</param>
        public SpriteCondition(Sprite sprite)
            : base(sprite)
        {
        }
        #endregion Constructor: SpriteCondition(Sprite sprite)

        #endregion Method Group: Constructors

    }
    #endregion Class: SpriteCondition

    #region Class: SpriteChange
    /// <summary>
    /// A change on a sprite.
    /// </summary>
    public class SpriteChange : StaticContentChange
    {

        #region Properties and Fields

        #region Property: Sprite
        /// <summary>
        /// Gets or sets the affected sprite.
        /// </summary>
        public Sprite Sprite
        {
            get
            {
                return this.Node as Sprite;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Sprite

        #region Property: Translation
        /// <summary>
        /// A handler for a change in the translation.
        /// </summary>
        private Vec2.Vec2Handler translationChanged;

        /// <summary>
        /// The translation to change to.
        /// </summary>
        private Vec2 translation = null;

        /// <summary>
        /// Gets or sets the translation to change to.
        /// </summary>
        public Vec2 Translation
        {
            get
            {
                if (translation == null)
                {
                    translation = GameDatabase.Current.Select<Vec2>(this.ID, GameValueTables.SpriteChange, GameColumns.Translation);

                    if (translation != null)
                    {
                        if (translationChanged == null)
                            translationChanged = new Vec2.Vec2Handler(translation_ValueChanged);

                        translation.ValueChanged += translationChanged;
                    }
                }

                return translation;
            }
            set
            {
                if (translationChanged == null)
                    translationChanged = new Vec2.Vec2Handler(translation_ValueChanged);

                if (translation != null)
                    translation.ValueChanged -= translationChanged;

                translation = value;
                GameDatabase.Current.Update(this.ID, GameValueTables.SpriteChange, GameColumns.Translation, translation);
                NotifyPropertyChanged("Translation");

                if (translation != null)
                    translation.ValueChanged += translationChanged;
            }
        }

        /// <summary>
        /// Updates the database when a value of the translation changes.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        private void translation_ValueChanged(Vec2 vector)
        {
            GameDatabase.Current.Update(this.ID, GameValueTables.SpriteChange, GameColumns.Translation, translation);
            NotifyPropertyChanged("Translation");
        }
        #endregion Property: Translation

        #region Property: TranslationChange
        /// <summary>
        /// Gets or sets the type of change for the translation.
        /// </summary>
        public ValueChangeType? TranslationChange
        {
            get
            {
                return GameDatabase.Current.Select<ValueChangeType?>(this.ID, GameValueTables.SpriteChange, GameColumns.TranslationChange);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.SpriteChange, GameColumns.TranslationChange, value);
                NotifyPropertyChanged("TranslationChange");
            }
        }
        #endregion Property: TranslationChange

        #region Property: Rotation
        /// <summary>
        /// Gets or sets the rotation to change to.
        /// </summary>
        public float? Rotation
        {
            get
            {
                return GameDatabase.Current.Select<float?>(this.ID, GameValueTables.SpriteChange, GameColumns.Rotation);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.SpriteChange, GameColumns.Rotation, value);
                NotifyPropertyChanged("Rotation");
            }
        }
        #endregion Property: Rotation

        #region Property: RotationChange
        /// <summary>
        /// Gets or sets the type of change for the rotation.
        /// </summary>
        public ValueChangeType? RotationChange
        {
            get
            {
                return GameDatabase.Current.Select<ValueChangeType?>(this.ID, GameValueTables.SpriteChange, GameColumns.RotationChange);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.SpriteChange, GameColumns.RotationChange, value);
                NotifyPropertyChanged("RotationChange");
            }
        }
        #endregion Property: RotationChange

        #region Property: Scale
        /// <summary>
        /// A handler for a change in the scale.
        /// </summary>
        private Vec2.Vec2Handler scaleChanged;

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
                if (scale == null)
                {
                    scale = GameDatabase.Current.Select<Vec2>(this.ID, GameValueTables.SpriteChange, GameColumns.Scale);

                    if (scale != null)
                    {
                        if (scaleChanged == null)
                            scaleChanged = new Vec2.Vec2Handler(scale_ValueChanged);

                        scale.ValueChanged += scaleChanged;
                    }
                }

                return scale;
            }
            set
            {
                if (scaleChanged == null)
                    scaleChanged = new Vec2.Vec2Handler(scale_ValueChanged);

                if (scale != null)
                    scale.ValueChanged -= scaleChanged;

                scale = value;
                GameDatabase.Current.Update(this.ID, GameValueTables.SpriteChange, GameColumns.Scale, scale);
                NotifyPropertyChanged("Scale");

                if (scale != null)
                    scale.ValueChanged += scaleChanged;
            }
        }

        /// <summary>
        /// Updates the database when a value of the scale changes.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        private void scale_ValueChanged(Vec2 vector)
        {
            GameDatabase.Current.Update(this.ID, GameValueTables.SpriteChange, GameColumns.Scale, scale);
            NotifyPropertyChanged("Scale");
        }
        #endregion Property: Scale

        #region Property: ScaleChange
        /// <summary>
        /// Gets or sets the type of change for the scale.
        /// </summary>
        public ValueChangeType? ScaleChange
        {
            get
            {
                return GameDatabase.Current.Select<ValueChangeType?>(this.ID, GameValueTables.SpriteChange, GameColumns.ScaleChange);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.SpriteChange, GameColumns.ScaleChange, value);
                NotifyPropertyChanged("ScaleChange");
            }
        }
        #endregion Property: ScaleChange

        #region Property: ZIndex
        /// <summary>
        /// Gets or sets the Z-index to change to.
        /// </summary>
        public int? ZIndex
        {
            get
            {
                return GameDatabase.Current.Select<int?>(this.ID, GameValueTables.SpriteChange, GameColumns.ZIndex);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.SpriteChange, GameColumns.ZIndex, value);
                NotifyPropertyChanged("ZIndex");
            }
        }
        #endregion Property: ZIndex

        #region Property: ZIndexChange
        /// <summary>
        /// Gets or sets the type of change for the Z-index.
        /// </summary>
        public ValueChangeType? ZIndexChange
        {
            get
            {
                return GameDatabase.Current.Select<ValueChangeType?>(this.ID, GameValueTables.SpriteChange, GameColumns.ZIndexChange);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.SpriteChange, GameColumns.ZIndexChange, value);
                NotifyPropertyChanged("ZIndexChange");
            }
        }
        #endregion Property: ZIndexChange

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SpriteChange()
        /// <summary>
        /// Creates a sprite change.
        /// </summary>
        public SpriteChange()
            : base()
        {
        }
        #endregion Constructor: SpriteChange()

        #region Constructor: SpriteChange(uint id)
        /// <summary>
        /// Creates a new sprite change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a sprite change from.</param>
        protected SpriteChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: SpriteChange(uint id)

        #region Constructor: SpriteChange(SpriteChange spriteChange)
        /// <summary>
        /// Clones a sprite change.
        /// </summary>
        /// <param name="spriteChange">The sprite change to clone.</param>
        public SpriteChange(SpriteChange spriteChange)
            : base(spriteChange)
        {
            if (spriteChange != null)
            {
                GameDatabase.Current.StartChange();

                if (spriteChange.Translation != null)
                    this.Translation = new Vec2(spriteChange.Translation);
                this.TranslationChange = spriteChange.TranslationChange;

                this.Rotation = spriteChange.Rotation;
                this.RotationChange = spriteChange.RotationChange;

                if (spriteChange.Scale != null)
                    this.Scale = new Vec2(spriteChange.Scale);
                this.ScaleChange = spriteChange.ScaleChange;

                this.ZIndex = spriteChange.ZIndex;
                this.ZIndexChange = spriteChange.ZIndexChange;

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: SpriteChange(SpriteChange spriteChange)

        #region Constructor: SpriteChange(Sprite sprite)
        /// <summary>
        /// Creates a change for the given sprite.
        /// </summary>
        /// <param name="sprite">The sprite to create a change for.</param>
        public SpriteChange(Sprite sprite)
            : base(sprite)
        {
        }
        #endregion Constructor: SpriteChange(Sprite sprite)

        #endregion Method Group: Constructors

    }
    #endregion Class: SpriteChange

}
