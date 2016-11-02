/**************************************************************************
 * 
 * IconBase.cs
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

    #region Class: IconBase
    /// <summary>
    /// A base of an icon.
    /// </summary>
    public class IconBase : StaticContentBase
    {

        #region Properties and Fields

        #region Property: Icon
        /// <summary>
        /// Gets the icon of which this is an icon base.
        /// </summary>
        protected internal Icon Icon
        {
            get
            {
                return this.Node as Icon;
            }
        }
        #endregion Property: Icon

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

        #region Constructor: IconBase(Icon icon)
        /// <summary>
        /// Creates a new icon base from the given icon.
        /// </summary>
        /// <param name="icon">The icon to create an icon base from.</param>
        protected internal IconBase(Icon icon)
            : base(icon)
        {
            if (icon != null)
                this.zIndex = icon.ZIndex;
        }
        #endregion Constructor: IconBase(Icon icon)

        #endregion Method Group: Constructors

    }
    #endregion Class: IconBase

    #region Class: IconValuedBase
    /// <summary>
    /// A base of a valued icon.
    /// </summary>
    public class IconValuedBase : StaticContentValuedBase
    {

        #region Properties and Fields

        #region Property: IconValued
        /// <summary>
        /// Gets the valued icon of which this is a valued icon base.
        /// </summary>
        protected internal IconValued IconValued
        {
            get
            {
                return this.NodeValued as IconValued;
            }
        }
        #endregion Property: IconValued

        #region Property: IconBase
        /// <summary>
        /// Gets the icon base.
        /// </summary>
        public IconBase IconBase
        {
            get
            {
                return this.NodeBase as IconBase;
            }
        }
        #endregion Property: IconBase

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

        #region Constructor: IconValuedBase(IconValued iconValued)
        /// <summary>
        /// Create a valued icon base from the given valued icon.
        /// </summary>
        /// <param name="iconValued">The valued icon to create a valued icon base from.</param>
        protected internal IconValuedBase(IconValued iconValued)
            : base(iconValued)
        {
            if (iconValued != null)
            {
                Vec2 iconValuedScale = iconValued.Scale;
                if (iconValuedScale != null)
                    this.scale = new Vec2(iconValuedScale);
            }
        }
        #endregion Constructor: IconValuedBase(IconValued iconValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: IconValuedBase

    #region Class: IconConditionBase
    /// <summary>
    /// A condition on an icon.
    /// </summary>
    public class IconConditionBase : StaticContentConditionBase
    {

        #region Properties and Fields

        #region Property: IconCondition
        /// <summary>
        /// Gets the icon condition of which this is an icon condition base.
        /// </summary>
        protected internal IconCondition IconCondition
        {
            get
            {
                return this.Condition as IconCondition;
            }
        }
        #endregion Property: IconCondition

        #region Property: IconBase
        /// <summary>
        /// Gets the icon base of which this is an icon condition base.
        /// </summary>
        public IconBase IconBase
        {
            get
            {
                return this.NodeBase as IconBase;
            }
        }
        #endregion Property: IconBase

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

        #region Constructor: IconConditionBase(IconCondition iconCondition)
        /// <summary>
        /// Creates a base of the given icon condition.
        /// </summary>
        /// <param name="iconCondition">The icon condition to create a base of.</param>
        protected internal IconConditionBase(IconCondition iconCondition)
            : base(iconCondition)
        {
            if (iconCondition != null)
            {
                if (iconCondition.Scale != null)
                    this.scale = new Vec2(iconCondition.Scale);
                this.scaleSign = iconCondition.ScaleSign;

                this.zIndex = iconCondition.ZIndex;
                this.zIndexSign = iconCondition.ZIndexSign;
            }
        }
        #endregion Constructor: IconConditionBase(IconCondition iconCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: IconConditionBase

    #region Class: IconChangeBase
    /// <summary>
    /// A change on an icon.
    /// </summary>
    public class IconChangeBase : StaticContentChangeBase
    {

        #region Properties and Fields

        #region Property: IconChange
        /// <summary>
        /// Gets the icon change of which this is an icon change base.
        /// </summary>
        protected internal IconChange IconChange
        {
            get
            {
                return this.Change as IconChange;
            }
        }
        #endregion Property: IconChange

        #region Property: IconBase
        /// <summary>
        /// Gets the affected icon base.
        /// </summary>
        public IconBase IconBase
        {
            get
            {
                return this.NodeBase as IconBase;
            }
        }
        #endregion Property: IconBase

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

        #region Constructor: IconChangeBase(IconChange iconChange)
        /// <summary>
        /// Creates a base of the given icon change.
        /// </summary>
        /// <param name="iconChange">The icon change to create a base of.</param>
        protected internal IconChangeBase(IconChange iconChange)
            : base(iconChange)
        {
            if (iconChange != null)
            {
                if (iconChange.Scale != null)
                    this.scale = new Vec2(iconChange.Scale);
                this.scaleChange = iconChange.ScaleChange;

                this.zIndex = iconChange.ZIndex;
                this.zIndexChange = iconChange.ZIndexChange;
            }
        }
        #endregion Constructor: IconChangeBase(IconChange iconChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: IconChangeBase

}