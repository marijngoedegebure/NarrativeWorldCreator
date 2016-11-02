/**************************************************************************
 * 
 * ModelInstance.cs
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

    #region Class: ModelInstance
    /// <summary>
    /// An instance of a model.
    /// </summary>
    public class ModelInstance : StaticContentInstance
    {

        #region Properties and Fields

        #region Property: ModelBase
        /// <summary>
        /// Gets the model base of which this is a model instance.
        /// </summary>
        public ModelBase ModelBase
        {
            get
            {
                if (this.ModelValuedBase != null)
                    return this.ModelValuedBase.ModelBase;
                return null;
            }
        }
        #endregion Property: ModelBase

        #region Property: ModelValuedBase
        /// <summary>
        /// Gets the valued model base of which this is a model instance.
        /// </summary>
        public ModelValuedBase ModelValuedBase
        {
            get
            {
                return this.Base as ModelValuedBase;
            }
        }
        #endregion Property: ModelValuedBase

        #region Property: Translation
        /// <summary>
        /// A handler for a changed translation.
        /// </summary>
        Vec3.Vec3Handler translationChanged = null;
        
        /// <summary>
        /// Gets the translation.
        /// </summary>
        public Vec3 Translation
        {
            get
            {
                if (this.ModelValuedBase != null)
                    return GetProperty<Vec3>("Translation", this.ModelValuedBase.Translation);

                return new Vec3(GameSemanticsSettings.Values.TranslationX, GameSemanticsSettings.Values.TranslationY, GameSemanticsSettings.Values.TranslationZ);
            }
            set
            {
                Vec3 translation = this.Translation;
                if (translation != value)
                {
                    if (translationChanged == null)
                        translationChanged = new Vec3.Vec3Handler(translation_ValueChanged);

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
        private void translation_ValueChanged(Vec3 vector)
        {
            NotifyPropertyChanged("Translation");
        }
        #endregion Property: Translation

        #region Property: Rotation
        /// <summary>
        /// A handler for a changed rotation.
        /// </summary>
        Vec3.Vec3Handler rotationChanged = null;
        
        /// <summary>
        /// Gets the rotation (in degrees).
        /// </summary>
        public Vec3 Rotation
        {
            get
            {
                if (this.ModelValuedBase != null)
                    return GetProperty<Vec3>("Rotation", this.ModelValuedBase.Rotation);

                return new Vec3(GameSemanticsSettings.Values.RotationX, GameSemanticsSettings.Values.RotationY, GameSemanticsSettings.Values.RotationZ);
            }
            set
            {
                Vec3 rotation = this.Rotation;
                if (rotation != value)
                {
                    if (rotationChanged == null)
                        rotationChanged = new Vec3.Vec3Handler(rotation_ValueChanged);

                    if (rotation != null)
                        rotation.ValueChanged -= rotationChanged;

                    SetProperty("Rotation", value);

                    if (value != null)
                        value.ValueChanged += rotationChanged;
                }
            }
        }

        /// <summary>
        /// Notify any changes of the rotation.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        private void rotation_ValueChanged(Vec3 vector)
        {
            NotifyPropertyChanged("Rotation");
        }
        #endregion Property: Rotation

        #region Property: Scale
        /// <summary>
        /// A handler for a changed scale.
        /// </summary>
        Vec3.Vec3Handler scaleChanged = null;
        
        /// <summary>
        /// Gets the scale.
        /// </summary>
        public Vec3 Scale
        {
            get
            {
                if (this.ModelValuedBase != null)
                    return GetProperty<Vec3>("Scale", this.ModelValuedBase.Scale);

                return new Vec3(GameSemanticsSettings.Values.ScaleX, GameSemanticsSettings.Values.ScaleY, GameSemanticsSettings.Values.ScaleZ);
            }
            set
            {
                Vec3 scale = this.Scale;
                if (scale != value)
                {
                    if (scaleChanged == null)
                        scaleChanged = new Vec3.Vec3Handler(scale_ValueChanged);

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
        private void scale_ValueChanged(Vec3 vector)
        {
            NotifyPropertyChanged("Scale");
        }
        #endregion Property: Scale

        #region Property: BoundingBox
        /// <summary>
        /// Gets the bounding box.
        /// </summary>
        public Box BoundingBox
        {
            get
            {
                if (this.ModelBase != null)
                    return this.ModelBase.BoundingBox;
                return null;
            }
        }
        #endregion Property: BoundingBox

        #region Property: Arguments
        /// <summary>
        /// Gets the arguments and their values.
        /// </summary>
        public ReadOnlyDictionary<string, string> Arguments
        {
            get
            {
                if (this.ModelBase != null)
                    return this.ModelBase.Arguments;
                return new ReadOnlyDictionary<string, string>();
            }
        }
        #endregion Property: Arguments

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ModelInstance(ModelValuedBase modelValuedBase)
        /// <summary>
        /// Creates a new model instance from the given valued model base.
        /// </summary>
        /// <param name="modelValuedBase">The valued model base to create the model instance from.</param>
        internal ModelInstance(ModelValuedBase modelValuedBase)
            : base(modelValuedBase)
        {
        }
        #endregion Constructor: ModelInstance(ModelValuedBase modelValuedBase)

        #region Constructor: ModelInstance(ModelInstance modelInstance)
        /// <summary>
        /// Clones a model instance.
        /// </summary>
        /// <param name="modelInstance">The model instance to clone.</param>
        protected internal ModelInstance(ModelInstance modelInstance)
            : base(modelInstance)
        {
            if (modelInstance != null)
            {
                if (modelInstance.Translation != null)
                    this.Translation = new Vec3(modelInstance.Translation);
                else
                    this.Translation = null;
                if (modelInstance.Rotation != null)
                    this.Rotation = new Vec3(modelInstance.Rotation);
                else
                    this.Rotation = null;
                if (modelInstance.Scale != null)
                    this.Scale = new Vec3(modelInstance.Scale);
                else
                    this.Scale = null;
            }
        }
        #endregion Constructor: ModelInstance(ModelInstance modelInstance)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the model instance satisfies the given condition.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the model instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the model instance is satisfies the given condition.</returns>
        public override bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            // Check whether the base satisfies the condition
            if (conditionBase != null && base.Satisfies(conditionBase, iVariableInstanceHolder))
            {
                ModelConditionBase modelConditionBase = conditionBase as ModelConditionBase;
                if (modelConditionBase != null)
                {
                    // Check whether all the properties have the correct values
                    if ((modelConditionBase.TranslationSign == null || modelConditionBase.Translation == null || Toolbox.Compare(this.Translation, (EqualitySignExtended)modelConditionBase.TranslationSign, (Vec3)modelConditionBase.Translation)) &&
                        (modelConditionBase.RotationSign == null || modelConditionBase.Rotation == null || Toolbox.Compare(this.Rotation, (EqualitySignExtended)modelConditionBase.RotationSign, (Vec3)modelConditionBase.Rotation)) &&
                        (modelConditionBase.ScaleSign == null || modelConditionBase.Scale == null || Toolbox.Compare(this.Scale, (EqualitySignExtended)modelConditionBase.ScaleSign, (Vec3)modelConditionBase.Scale)))
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
        /// Apply the change to the model instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the model instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        protected internal override bool Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (changeBase != null && base.Apply(changeBase, iVariableInstanceHolder))
            {
                ModelChangeBase modelChangeBase = changeBase as ModelChangeBase;
                if (modelChangeBase != null)
                {
                    // Apply all changes
                    if (modelChangeBase.TranslationChange != null && modelChangeBase.Translation != null)
                        this.Translation = Toolbox.CalcValue(this.Translation, (ValueChangeType)modelChangeBase.TranslationChange, (Vec3)modelChangeBase.Translation);
                    if (modelChangeBase.RotationChange != null && modelChangeBase.Rotation != null)
                        this.Rotation = Toolbox.CalcValue(this.Rotation, (ValueChangeType)modelChangeBase.RotationChange, (Vec3)modelChangeBase.Rotation);
                    if (modelChangeBase.ScaleChange != null && modelChangeBase.Scale != null)
                        this.Scale = Toolbox.CalcValue(this.Scale, (ValueChangeType)modelChangeBase.ScaleChange, (Vec3)modelChangeBase.Scale);
                }
                return true;
            }
            return false;
        }
        #endregion Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: ModelInstance

}