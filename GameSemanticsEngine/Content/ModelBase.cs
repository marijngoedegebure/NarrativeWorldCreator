/**************************************************************************
 * 
 * ModelBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using Common;
using GameSemantics.GameContent;
using GameSemantics.Utilities;
using GameSemanticsEngine.Tools;
using Semantics.Utilities;

namespace GameSemanticsEngine.GameContent
{

    #region Class: ModelBase
    /// <summary>
    /// A base of a model.
    /// </summary>
    public class ModelBase : StaticContentBase
    {

        #region Properties and Fields

        #region Property: Model
        /// <summary>
        /// Gets the model of which this is a model base.
        /// </summary>
        protected internal Model Model
        {
            get
            {
                return this.Node as Model;
            }
        }
        #endregion Property: Model

        #region Property: BoundingBox
        /// <summary>
        /// The bounding box.
        /// </summary>
        private Box boundingBox = null;
        
        /// <summary>
        /// Gets the bounding box.
        /// </summary>
        public Box BoundingBox
        {
            get
            {
                return boundingBox;
            }
        }
        #endregion Property: BoundingBox

        #region Property: Arguments
        /// <summary>
        /// The arguments and their values.
        /// </summary>
        private ReadOnlyDictionary<string, string> arguments = null;
        
        /// <summary>
        /// Gets the arguments and their values.
        /// </summary>
        public ReadOnlyDictionary<string, string> Arguments
        {
            get
            {
                if (arguments == null)
                {
                    LoadArguments();
                    if (arguments == null)
                        return new ReadOnlyDictionary<string, string>();
                }
                return arguments;
            }
        }

        /// <summary>
        /// Load the arguments.
        /// </summary>
        private void LoadArguments()
        {
            if (this.Model != null)
                arguments = this.Model.Arguments;
        }
        #endregion Property: Arguments

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ModelBase(Model model)
        /// <summary>
        /// Creates a new model base from the given model.
        /// </summary>
        /// <param name="model">The model to create a model base from.</param>
        protected internal ModelBase(Model model)
            : base(model)
        {
            if (model != null)
            {
                this.boundingBox = model.BoundingBox;

                if (GameBaseManager.PreloadProperties)
                    LoadArguments();
            }
        }
        #endregion Constructor: ModelBase(Model model)

        #endregion Method Group: Constructors

    }
    #endregion Class: ModelBase

    #region Class: ModelValuedBase
    /// <summary>
    /// A base of a valued model.
    /// </summary>
    public class ModelValuedBase : StaticContentValuedBase
    {

        #region Properties and Fields

        #region Property: ModelValued
        /// <summary>
        /// Gets the valued model of which this is a valued model base.
        /// </summary>
        protected internal ModelValued ModelValued
        {
            get
            {
                return this.NodeValued as ModelValued;
            }
        }
        #endregion Property: ModelValued

        #region Property: ModelBase
        /// <summary>
        /// Gets the model base.
        /// </summary>
        public ModelBase ModelBase
        {
            get
            {
                return this.NodeBase as ModelBase;
            }
        }
        #endregion Property: ModelBase

        #region Property: Translation
        /// <summary>
        /// The translation.
        /// </summary>
        private Vec3 translation = new Vec3(GameSemanticsSettings.Values.TranslationX, GameSemanticsSettings.Values.TranslationY, GameSemanticsSettings.Values.TranslationZ);

        /// <summary>
        /// Gets the translation.
        /// </summary>
        public Vec3 Translation
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
        private Vec3 rotation = new Vec3(GameSemanticsSettings.Values.RotationX, GameSemanticsSettings.Values.RotationY, GameSemanticsSettings.Values.RotationZ);

        /// <summary>
        /// Gets the rotation (in degrees).
        /// </summary>
        public Vec3 Rotation
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
        private Vec3 scale = new Vec3(GameSemanticsSettings.Values.ScaleX, GameSemanticsSettings.Values.ScaleY, GameSemanticsSettings.Values.ScaleZ);

        /// <summary>
        /// Gets the scale.
        /// </summary>
        public Vec3 Scale
        {
            get
            {
                return scale;
            }
        }
        #endregion Property: Scale

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ModelValuedBase(ModelValued modelValued)
        /// <summary>
        /// Create a valued model base from the given valued model.
        /// </summary>
        /// <param name="modelValued">The valued model to create a valued model base from.</param>
        protected internal ModelValuedBase(ModelValued modelValued)
            : base(modelValued)
        {
            if (modelValued != null)
            {
                Vec3 modelValuedTranslation = modelValued.Translation;
                if (modelValuedTranslation != null)
                    this.translation = new Vec3(modelValuedTranslation);

                Vec3 modelValuedRotation = modelValued.Rotation;
                if (modelValuedRotation != null)
                    this.rotation = new Vec3(modelValuedRotation);

                Vec3 modelValuedScale = modelValued.Scale;
                if (modelValuedScale != null)
                    this.scale = new Vec3(modelValuedScale);
            }
        }
        #endregion Constructor: ModelValuedBase(ModelValued modelValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: ModelValuedBase

    #region Class: ModelConditionBase
    /// <summary>
    /// A condition on a model.
    /// </summary>
    public class ModelConditionBase : StaticContentConditionBase
    {

        #region Properties and Fields

        #region Property: ModelCondition
        /// <summary>
        /// Gets the model condition of which this is a model condition base.
        /// </summary>
        protected internal ModelCondition ModelCondition
        {
            get
            {
                return this.Condition as ModelCondition;
            }
        }
        #endregion Property: ModelCondition

        #region Property: ModelBase
        /// <summary>
        /// Gets the model base of which this is a model condition base.
        /// </summary>
        public ModelBase ModelBase
        {
            get
            {
                return this.NodeBase as ModelBase;
            }
        }
        #endregion Property: ModelBase

        #region Property: Translation
        /// <summary>
        /// The required translation.
        /// </summary>
        private Vec3 translation = null;

        /// <summary>
        /// Gets the required translation.
        /// </summary>
        public Vec3 Translation
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
        private Vec3 rotation = null;

        /// <summary>
        /// Gets the required rotation.
        /// </summary>
        public Vec3 Rotation
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
        private Vec3 scale = null;

        /// <summary>
        /// Gets the required scale.
        /// </summary>
        public Vec3 Scale
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

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ModelConditionBase(ModelCondition modelCondition)
        /// <summary>
        /// Creates a base of the given model condition.
        /// </summary>
        /// <param name="modelCondition">The model condition to create a base of.</param>
        protected internal ModelConditionBase(ModelCondition modelCondition)
            : base(modelCondition)
        {
            if (modelCondition != null)
            {
                if (modelCondition.Translation != null)
                    this.translation = new Vec3(modelCondition.Translation);
                this.translationSign = modelCondition.TranslationSign;

                if (modelCondition.Rotation != null)
                    this.rotation = new Vec3(modelCondition.Rotation);
                this.rotationSign = modelCondition.RotationSign;

                if (modelCondition.Scale != null)
                    this.scale = new Vec3(modelCondition.Scale);
                this.scaleSign = modelCondition.ScaleSign;
            }
        }
        #endregion Constructor: ModelConditionBase(ModelCondition modelCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: ModelConditionBase

    #region Class: ModelChangeBase
    /// <summary>
    /// A change on a model.
    /// </summary>
    public class ModelChangeBase : StaticContentChangeBase
    {

        #region Properties and Fields

        #region Property: ModelChange
        /// <summary>
        /// Gets the model change of which this is a model change base.
        /// </summary>
        protected internal ModelChange ModelChange
        {
            get
            {
                return this.Change as ModelChange;
            }
        }
        #endregion Property: ModelChange

        #region Property: ModelBase
        /// <summary>
        /// Gets the affected model base.
        /// </summary>
        public ModelBase ModelBase
        {
            get
            {
                return this.NodeBase as ModelBase;
            }
        }
        #endregion Property: ModelBase

        #region Property: Translation
        /// <summary>
        /// The translation to change to.
        /// </summary>
        private Vec3 translation = null;

        /// <summary>
        /// Gets the translation to change to.
        /// </summary>
        public Vec3 Translation
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
        private Vec3 rotation = null;

        /// <summary>
        /// Gets the rotation to change to.
        /// </summary>
        public Vec3 Rotation
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
        private Vec3 scale = null;

        /// <summary>
        /// Gets or sets the scale to change to.
        /// </summary>
        public Vec3 Scale
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

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ModelChangeBase(ModelChange modelChange)
        /// <summary>
        /// Creates a base of the given model change.
        /// </summary>
        /// <param name="modelChange">The model change to create a base of.</param>
        protected internal ModelChangeBase(ModelChange modelChange)
            : base(modelChange)
        {
            if (modelChange != null)
            {
                if (modelChange.Translation != null)
                    this.translation = new Vec3(modelChange.Translation);
                this.translationChange = modelChange.TranslationChange;

                if (modelChange.Rotation != null)
                    this.rotation = new Vec3(modelChange.Rotation);
                this.rotationChange = modelChange.RotationChange;

                if (modelChange.Scale != null)
                    this.scale = new Vec3(modelChange.Scale);
                this.scaleChange = modelChange.ScaleChange;
            }
        }
        #endregion Constructor: ModelChangeBase(ModelChange modelChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: ModelChangeBase

}