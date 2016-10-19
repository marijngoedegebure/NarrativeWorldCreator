/**************************************************************************
 * 
 * Icon.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2010-2011
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

    #region Class: Icon
    /// <summary>
    /// An icon.
    /// </summary>
    public class Icon : StaticContent, IComparable<Icon>
    {

        #region Properties and Fields

        #region Property: ZIndex
        /// <summary>
        /// Gets or sets the Z-index of the icon.
        /// </summary>
        public int ZIndex
        {
            get
            {
                return GameDatabase.Current.Select<int>(this.ID, GameTables.Icon, GameColumns.ZIndex);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameTables.Icon, GameColumns.ZIndex, value);
                NotifyPropertyChanged("ZIndex");
            }
        }
        #endregion Property: ZIndex

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: Icon()
        /// <summary>
        /// Creates a new icon.
        /// </summary>
        public Icon()
            : base()
        {
        }
        #endregion Constructor: Icon()

        #region Constructor: Icon(uint id)
        /// <summary>
        /// Creates a new icon from the given ID.
        /// </summary>
        /// <param name="id">The ID to create an icon from.</param>
        protected Icon(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Icon(uint id)

        #region Constructor: Icon(string file)
        /// <summary>
        /// Creates a new icon with the given file.
        /// </summary>
        /// <param name="file">The file to assign to the icon.</param>
        public Icon(string file)
            : base(file)
        {
        }
        #endregion Constructor: Icon(string file)

        #region Constructor: Icon(Icon icon)
        /// <summary>
        /// Clones an icon.
        /// </summary>
        /// <param name="icon">The icon to clone.</param>
        public Icon(Icon icon)
            : base(icon)
        {
            if (icon != null)
                this.ZIndex = icon.ZIndex;
        }
        #endregion Constructor: Icon(Icon icon)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the icon.
        /// </summary>
        public override void Remove()
        {
            GameDatabase.Current.StartChange();
            GameDatabase.Current.StartRemove();

            base.Remove();

            GameDatabase.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(Icon other)
        /// <summary>
        /// Compares the icon to the other icon.
        /// </summary>
        /// <param name="other">The icon to compare to this icon.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Icon other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(Icon other)

        #endregion Method Group: Other

    }
    #endregion Class: Icon

    #region Class: IconValued
    /// <summary>
    /// A valued version of an icon.
    /// </summary>
    public class IconValued : StaticContentValued
    {

        #region Properties and Fields

        #region Property: Icon
        /// <summary>
        /// Gets the icon of which this is a valued icon.
        /// </summary>
        public Icon Icon
        {
            get
            {
                return this.Node as Icon;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: Icon

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
                    scale = GameDatabase.Current.Select<Vec2>(this.ID, GameValueTables.IconValued, GameColumns.Scale);

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
                GameDatabase.Current.Update(this.ID, GameValueTables.IconValued, GameColumns.Scale, scale);
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

        #region Constructor: IconValued(uint id)
        /// <summary>
        /// Creates a new valued icon from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the valued icon from.</param>
        protected IconValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: IconValued(uint id)

        #region Constructor: IconValued(IconValued iconValued)
        /// <summary>
        /// Clones a valued icon.
        /// </summary>
        /// <param name="iconValued">The valued icon to clone.</param>
        public IconValued(IconValued iconValued)
            : base(iconValued)
        {
            if (iconValued != null)
            {
                GameDatabase.Current.StartChange();

                this.Scale = new Vec2(iconValued.Scale);

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: IconValued(IconValued iconValued)

        #region Constructor: IconValued(Icon icon)
        /// <summary>
        /// Creates a new valued icon from the given icon.
        /// </summary>
        /// <param name="icon">The icon to create a valued icon from.</param>
        public IconValued(Icon icon)
            : base(icon)
        {
        }
        #endregion Constructor: IconValued(Icon icon)

        #endregion Method Group: Constructors

    }
    #endregion Class: IconValued

    #region Class: IconCondition
    /// <summary>
    /// A condition on an icon.
    /// </summary>
    public class IconCondition : StaticContentCondition
    {

        #region Properties and Fields

        #region Property: Icon
        /// <summary>
        /// Gets or sets the required icon.
        /// </summary>
        public Icon Icon
        {
            get
            {
                return this.Node as Icon;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Icon

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
                    scale = GameDatabase.Current.Select<Vec2>(this.ID, GameValueTables.IconCondition, GameColumns.Scale);

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
                GameDatabase.Current.Update(this.ID, GameValueTables.IconCondition, GameColumns.Scale, scale);
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
            GameDatabase.Current.Update(this.ID, GameValueTables.IconCondition, GameColumns.Scale, scale);
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
                return GameDatabase.Current.Select<EqualitySignExtended?>(this.ID, GameValueTables.IconCondition, GameColumns.ScaleSign);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.IconCondition, GameColumns.ScaleSign, value);
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
                return GameDatabase.Current.Select<int?>(this.ID, GameValueTables.IconCondition, GameColumns.ZIndex);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.IconCondition, GameColumns.ZIndex, value);
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
                return GameDatabase.Current.Select<EqualitySignExtended?>(this.ID, GameValueTables.IconCondition, GameColumns.ZIndexSign);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.IconCondition, GameColumns.ZIndexSign, value);
                NotifyPropertyChanged("ZIndexSign");
            }
        }
        #endregion Property: ZIndexSign

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: IconCondition()
        /// <summary>
        /// Creates a new icon condition.
        /// </summary>
        public IconCondition()
            : base()
        {
        }
        #endregion Constructor: IconCondition()

        #region Constructor: IconCondition(uint id)
        /// <summary>
        /// Creates a new icon condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the icon condition from.</param>
        protected IconCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: IconCondition(uint id)

        #region Constructor: IconCondition(IconCondition iconCondition)
        /// <summary>
        /// Clones an icon condition.
        /// </summary>
        /// <param name="iconCondition">The icon condition to clone.</param>
        public IconCondition(IconCondition iconCondition)
            : base(iconCondition)
        {
            if (iconCondition != null)
            {
                GameDatabase.Current.StartChange();

                if (iconCondition.Scale != null)
                    this.Scale = new Vec2(iconCondition.Scale);
                this.ScaleSign = iconCondition.ScaleSign;

                this.ZIndex = iconCondition.ZIndex;
                this.ZIndexSign = iconCondition.ZIndexSign;

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: IconCondition(IconCondition iconCondition)

        #region Constructor: IconCondition(Icon icon)
        /// <summary>
        /// Creates a condition for the given icon.
        /// </summary>
        /// <param name="icon">The icon to create a condition for.</param>
        public IconCondition(Icon icon)
            : base(icon)
        {
        }
        #endregion Constructor: IconCondition(Icon icon)

        #endregion Method Group: Constructors

    }
    #endregion Class: IconCondition

    #region Class: IconChange
    /// <summary>
    /// A change on an icon.
    /// </summary>
    public class IconChange : StaticContentChange
    {

        #region Properties and Fields

        #region Property: Icon
        /// <summary>
        /// Gets or sets the affected icon.
        /// </summary>
        public Icon Icon
        {
            get
            {
                return this.Node as Icon;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Icon

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
                    scale = GameDatabase.Current.Select<Vec2>(this.ID, GameValueTables.IconChange, GameColumns.Scale);

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
                GameDatabase.Current.Update(this.ID, GameValueTables.IconChange, GameColumns.Scale, scale);
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
            GameDatabase.Current.Update(this.ID, GameValueTables.IconChange, GameColumns.Scale, scale);
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
                return GameDatabase.Current.Select<ValueChangeType?>(this.ID, GameValueTables.IconChange, GameColumns.ScaleChange);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.IconChange, GameColumns.ScaleChange, value);
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
                return GameDatabase.Current.Select<int?>(this.ID, GameValueTables.IconChange, GameColumns.ZIndex);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.IconChange, GameColumns.ZIndex, value);
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
                return GameDatabase.Current.Select<ValueChangeType?>(this.ID, GameValueTables.IconChange, GameColumns.ZIndexChange);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.IconChange, GameColumns.ZIndexChange, value);
                NotifyPropertyChanged("ZIndexChange");
            }
        }
        #endregion Property: ZIndexChange

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: IconChange()
        /// <summary>
        /// Creates an icon change.
        /// </summary>
        public IconChange()
            : base()
        {
        }
        #endregion Constructor: IconChange()

        #region Constructor: IconChange(uint id)
        /// <summary>
        /// Creates a new icon change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create an icon change from.</param>
        protected IconChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: IconChange(uint id)

        #region Constructor: IconChange(IconChange iconChange)
        /// <summary>
        /// Clones an icon change.
        /// </summary>
        /// <param name="iconChange">The icon change to clone.</param>
        public IconChange(IconChange iconChange)
            : base(iconChange)
        {
            if (iconChange != null)
            {
                GameDatabase.Current.StartChange();

                if (iconChange.Scale != null)
                    this.Scale = new Vec2(iconChange.Scale);
                this.ScaleChange = iconChange.ScaleChange;

                this.ZIndex = iconChange.ZIndex;
                this.ZIndexChange = iconChange.ZIndexChange;

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: IconChange(IconChange iconChange)

        #region Constructor: IconChange(Icon icon)
        /// <summary>
        /// Creates a change for the given icon.
        /// </summary>
        /// <param name="icon">The icon to create a change for.</param>
        public IconChange(Icon icon)
            : base(icon)
        {
        }
        #endregion Constructor: IconChange(Icon icon)

        #endregion Method Group: Constructors

    }
    #endregion Class: IconChange

}