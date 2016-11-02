/**************************************************************************
 * 
 * IconInstance.cs
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

    #region Class: IconInstance
    /// <summary>
    /// An instance of an icon.
    /// </summary>
    public class IconInstance : StaticContentInstance
    {

        #region Properties and Fields

        #region Property: IconBase
        /// <summary>
        /// Gets the icon base of which this is an icon instance.
        /// </summary>
        public IconBase IconBase
        {
            get
            {
                if (this.IconValuedBase != null)
                    return this.IconValuedBase.IconBase;
                return null;
            }
        }
        #endregion Property: IconBase

        #region Property: IconValuedBase
        /// <summary>
        /// Gets the valued icon base of which this is an icon instance.
        /// </summary>
        public IconValuedBase IconValuedBase
        {
            get
            {
                return this.Base as IconValuedBase;
            }
        }
        #endregion Property: IconValuedBase

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
                if (this.IconValuedBase != null)
                    return GetProperty<Vec2>("Scale", this.IconValuedBase.Scale);

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
                if (this.IconBase != null)
                    return GetProperty<int>("ZIndex", this.IconBase.ZIndex);

                return GameSemanticsSettings.Values.ZIndex;
            }
            protected set
            {
                if (this.ZIndex != value)
                    SetProperty("ZIndex", value);
            }
        }
        #endregion Property: ZIndex

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: IconInstance(IconValuedBase iconValuedBase)
        /// <summary>
        /// Creates a new icon instance from the given valued icon base.
        /// </summary>
        /// <param name="iconValuedBase">The valued icon base to create the icon instance from.</param>
        internal IconInstance(IconValuedBase iconValuedBase)
            : base(iconValuedBase)
        {
        }
        #endregion Constructor: IconInstance(IconValuedBase iconValuedBase)

        #region Constructor: IconInstance(IconInstance iconInstance)
        /// <summary>
        /// Clones an icon instance.
        /// </summary>
        /// <param name="iconInstance">The icon instance to clone.</param>
        protected internal IconInstance(IconInstance iconInstance)
            : base(iconInstance)
        {
            if (iconInstance != null)
            {
                if (iconInstance.Scale != null)
                    this.Scale = new Vec2(iconInstance.Scale);
                else
                    this.Scale = null;

                this.ZIndex = iconInstance.ZIndex;
            }
        }
        #endregion Constructor: IconInstance(IconInstance iconInstance)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the icon instance satisfies the given condition.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the icon instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the icon instance is satisfies the given condition.</returns>
        public override bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            // Check whether the base satisfies the condition
            if (conditionBase != null && base.Satisfies(conditionBase, iVariableInstanceHolder))
            {
                IconConditionBase iconConditionBase = conditionBase as IconConditionBase;
                if (iconConditionBase != null)
                {
                    // Check whether all the properties have the correct values
                    if (iconConditionBase.ScaleSign == null || iconConditionBase.Scale == null || Toolbox.Compare(this.Scale, (EqualitySignExtended)iconConditionBase.ScaleSign, (Vec2)iconConditionBase.Scale) &&
                       (iconConditionBase.ZIndexSign == null || iconConditionBase.ZIndex == null || Toolbox.Compare(this.ZIndex, (EqualitySignExtended)iconConditionBase.ZIndexSign, (float)iconConditionBase.ZIndex)))
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
        /// Apply the change to the icon instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the icon instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        protected internal override bool Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (changeBase != null && base.Apply(changeBase, iVariableInstanceHolder))
            {
                IconChangeBase iconChangeBase = changeBase as IconChangeBase;
                if (iconChangeBase != null)
                {
                    // Apply all changes
                    if (iconChangeBase.ScaleChange != null && iconChangeBase.Scale != null)
                        this.Scale = Toolbox.CalcValue(this.Scale, (ValueChangeType)iconChangeBase.ScaleChange, (Vec2)iconChangeBase.Scale);
                    if (iconChangeBase.ZIndex != null && iconChangeBase.ZIndexChange != null)
                        this.ZIndex = Toolbox.CalcValue(this.ZIndex, (ValueChangeType)iconChangeBase.ZIndexChange, (int)iconChangeBase.ZIndex);
                }
                return true;
            }
            return false;
        }
        #endregion Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: IconInstance

}