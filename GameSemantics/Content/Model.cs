/**************************************************************************
 * 
 * GameModel.cs
 * 
 * Jassin Kessing & Tim Tutenel
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using System.Collections.Generic;
using Common;
using GameSemantics.Data;
using GameSemantics.Utilities;
using Semantics.Utilities;

namespace GameSemantics.GameContent
{

    #region Class: Model
    /// <summary>
    /// A 3-dimensional model.
    /// </summary>
    public class Model : StaticContent, IComparable<Model>
    {

        #region Properties and Fields

        #region Property: BoundingBox
        /// <summary>
        /// A handler for a change in the bounding box.
        /// </summary>
        private Box.BoxHandler boundingBoxChanged;

        /// <summary>
        /// The bounding box.
        /// </summary>
        private Box boundingBox = null;
        
        /// <summary>
        /// Gets or sets the bounding box.
        /// </summary>
        public Box BoundingBox
        {
            get
            {
                if (boundingBox == null)
                {
                    boundingBox = GameDatabase.Current.Select<Box>(this.ID, GameTables.Model, GameColumns.BoundingBox);

                    if (boundingBox != null && boundingBoxChanged == null)
                    {
                        boundingBoxChanged = new Box.BoxHandler(boundingBox_ValueChanged);
                        boundingBox.BoxChanged += boundingBoxChanged;
                    }
                }

                return boundingBox;
            }
            set
            {
                if (boundingBoxChanged == null)
                    boundingBoxChanged = new Box.BoxHandler(boundingBox_ValueChanged);

                if (boundingBox != null)
                    boundingBox.BoxChanged -= boundingBoxChanged;

                boundingBox = value;
                GameDatabase.Current.Update(this.ID, GameTables.Model, GameColumns.BoundingBox, boundingBox);
                NotifyPropertyChanged("BoundingBox");

                if (boundingBox != null)
                    boundingBox.BoxChanged += boundingBoxChanged;
            }
        }

        private void boundingBox_ValueChanged(Box box)
        {
            GameDatabase.Current.Update(this.ID, GameTables.Model, GameColumns.BoundingBox, boundingBox);
            NotifyPropertyChanged("BoundingBox");
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
                return new ReadOnlyDictionary<string, string>(GameDatabase.Current.SelectAll<string, string>(this.ID, GameTables.ModelArgument, GameColumns.Argument, GameColumns.Value));
            }
        }
        #endregion Property: Arguments

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: Model()
        /// <summary>
        /// Creates a new model.
        /// </summary>
        public Model()
            : base()
        {
        }
        #endregion Constructor: Model()

        #region Constructor: Model(uint id)
        /// <summary>
        /// Creates a new model from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a model from.</param>
        protected Model(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Model(uint id)

        #region Constructor: Model(string file)
        /// <summary>
        /// Creates a new model with the given file.
        /// </summary>
        /// <param name="file">The file to assign to the model.</param>
        public Model(string file)
            : base(file)
        {
        }
        #endregion Constructor: Model(string file)

        #region Constructor: Model(Model model)
        /// <summary>
        /// Clones a model.
        /// </summary>
        /// <param name="model">The model to clone.</param>
        public Model(Model model)
            : base(model)
        {
            if (model != null)
            {
                GameDatabase.Current.StartChange();

                if (model.BoundingBox != null)
                    this.BoundingBox = new Box(model.BoundingBox);
                foreach (KeyValuePair<string, string> pair in model.Arguments)
                    AddArgument(pair.Key, pair.Value);

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: Model(Model model)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddArgument(string argument, string value)
        /// <summary>
        /// Add an argument and its value.
        /// </summary>
        /// <param name="argument">The argument.</param>
        /// <param name="value">The value.</param>
        public Message AddArgument(string argument, string value)
        {
            if (argument != null && value != null)
            {
                if (this.Arguments.ContainsKey(argument))
                    GameDatabase.Current.Update(this.ID, GameTables.ModelArgument, GameColumns.Value, value);
                else
                    GameDatabase.Current.Insert(this.ID, GameTables.ModelArgument, new string[] { GameColumns.Argument, GameColumns.Value }, new object[] { argument, value });

                NotifyPropertyChanged("Arguments");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddArgument(string argument, string value)

        #region Method: RemoveArgument(string argument)
        /// <summary>
        /// Removes the argument.
        /// </summary>
        /// <param name="argument">The argument to remove.</param>
        public Message RemoveArgument(string argument)
        {
            if (argument != null && this.Arguments.ContainsKey(argument))
            {
                GameDatabase.Current.Remove(this.ID, GameTables.ModelArgument, GameColumns.Argument, argument);

                NotifyPropertyChanged("Arguments");
                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveArgument(string argument)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the model.
        /// </summary>
        public override void Remove()
        {
            GameDatabase.Current.StartChange();
            GameDatabase.Current.StartRemove();

            base.Remove();

            GameDatabase.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(Model other)
        /// <summary>
        /// Compares the model to the other model.
        /// </summary>
        /// <param name="other">The model to compare to this model.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Model other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(Model other)

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override String ToString()
        {
            if (this.File != null)
            {
                string returnString = BasicFunctionality.GetFile(this.File);

                string str = string.Empty;
                foreach (KeyValuePair<String, String> pair in this.Arguments)
                {
                    if (String.IsNullOrEmpty(str))
                        str += pair.Key + "=" + pair.Value;
                    else
                        str += ", " + pair.Key + "=" + pair.Value;
                }
                return String.IsNullOrEmpty(str) ? returnString : returnString + " (" + str + ")";
            }
            return base.ToString();
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: Model

    #region Class: ModelValued
    /// <summary>
    /// A valued version of a model.
    /// </summary>
    public class ModelValued : StaticContentValued
    {

        #region Properties and Fields

        #region Property: Model
        /// <summary>
        /// Gets the model of which this is a valued model.
        /// </summary>
        public Model Model
        {
            get
            {
                return this.Node as Model;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: Model

        #region Property: Translation
        /// <summary>
        /// A handler for a change in the translation.
        /// </summary>
        private Vec3.Vec3Handler translationChanged;

        /// <summary>
        /// The translation.
        /// </summary>
        private Vec3 translation = null;

        /// <summary>
        /// Gets or sets the translation.
        /// </summary>
        public Vec3 Translation
        {
            get
            {
                if (translation == null)
                {
                    translation = GameDatabase.Current.Select<Vec3>(this.ID, GameValueTables.ModelValued, GameColumns.Translation);

                    if (translation == null)
                        translation = new Vec3(GameSemanticsSettings.Values.TranslationX, GameSemanticsSettings.Values.TranslationY, GameSemanticsSettings.Values.TranslationZ);

                    if (translationChanged == null)
                        translationChanged = new Vec3.Vec3Handler(translation_ValueChanged);

                    translation.ValueChanged += translationChanged;
                }

                return translation;
            }
            set
            {
                if (translationChanged == null)
                    translationChanged = new Vec3.Vec3Handler(translation_ValueChanged);

                if (translation != null)
                    translation.ValueChanged -= translationChanged;

                translation = value;
                GameDatabase.Current.Update(this.ID, GameValueTables.ModelValued, GameColumns.Translation, translation);
                NotifyPropertyChanged("Translation");

                if (translation != null)
                    translation.ValueChanged += translationChanged;
            }
        }

        /// <summary>
        /// Updates the database when a value of the translation changes.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        private void translation_ValueChanged(Vec3 vector)
        {
            this.Translation = translation;
        }
        #endregion Property: Translation

        #region Property: Rotation
        /// <summary>
        /// A handler for a change in the rotation.
        /// </summary>
        private Vec3.Vec3Handler rotationChanged;

        /// <summary>
        /// The rotation.
        /// </summary>
        private Vec3 rotation = null;

        /// <summary>
        /// Gets or sets the rotation (in degrees).
        /// </summary>
        public Vec3 Rotation
        {
            get
            {
                if (rotation == null)
                {
                    rotation = GameDatabase.Current.Select<Vec3>(this.ID, GameValueTables.ModelValued, GameColumns.Rotation);

                    if (rotation == null)
                        rotation = new Vec3(GameSemanticsSettings.Values.RotationX, GameSemanticsSettings.Values.RotationY, GameSemanticsSettings.Values.RotationZ);

                    if (rotationChanged == null)
                        rotationChanged = new Vec3.Vec3Handler(rotation_ValueChanged);

                    rotation.ValueChanged += rotationChanged;
                }

                return rotation;
            }
            set
            {
                if (rotationChanged == null)
                    rotationChanged = new Vec3.Vec3Handler(rotation_ValueChanged);

                if (rotation != null)
                    rotation.ValueChanged -= rotationChanged;

                rotation = value;
                GameDatabase.Current.Update(this.ID, GameValueTables.ModelValued, GameColumns.Rotation, rotation);
                NotifyPropertyChanged("Rotation");

                if (rotation != null)
                    rotation.ValueChanged += rotationChanged;
            }
        }

        /// <summary>
        /// Updates the database when a value of the rotation changes.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        private void rotation_ValueChanged(Vec3 vector)
        {
            this.Rotation = rotation;
        }
        #endregion Property: Rotation

        #region Property: Scale
        /// <summary>
        /// A handler for a change in the scale.
        /// </summary>
        private Vec3.Vec3Handler scaleChanged;

        /// <summary>
        /// The scale.
        /// </summary>
        private Vec3 scale = null;

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        public Vec3 Scale
        {
            get
            {
                if (scale == null)
                {
                    scale = GameDatabase.Current.Select<Vec3>(this.ID, GameValueTables.ModelValued, GameColumns.Scale);

                    if (scale == null)
                        scale = new Vec3(GameSemanticsSettings.Values.ScaleX, GameSemanticsSettings.Values.ScaleY, GameSemanticsSettings.Values.ScaleZ);

                    if (scaleChanged == null)
                        scaleChanged = new Vec3.Vec3Handler(scale_ValueChanged);

                    scale.ValueChanged += scaleChanged;
                }

                return scale;
            }
            set
            {
                if (scaleChanged == null)
                    scaleChanged = new Vec3.Vec3Handler(scale_ValueChanged);

                if (scale != null)
                    scale.ValueChanged -= scaleChanged;

                scale = value;
                GameDatabase.Current.Update(this.ID, GameValueTables.ModelValued, GameColumns.Scale, scale);
                NotifyPropertyChanged("Scale");

                if (scale != null)
                    scale.ValueChanged += scaleChanged;
            }
        }

        /// <summary>
        /// Updates the database when a value of the scale changes.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        private void scale_ValueChanged(Vec3 vector)
        {
            this.Scale = scale;
        }
        #endregion Property: Scale

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ModelValued(uint id)
        /// <summary>
        /// Creates a new valued model from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the valued model from.</param>
        protected ModelValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ModelValued(uint id)

        #region Constructor: ModelValued(ModelValued modelValued)
        /// <summary>
        /// Clones a valued model.
        /// </summary>
        /// <param name="modelValued">The valued model to clone.</param>
        public ModelValued(ModelValued modelValued)
            : base(modelValued)
        {
            if (modelValued != null)
            {
                GameDatabase.Current.StartChange();

                if (modelValued.Translation != null)
                    this.Translation = new Vec3(modelValued.Translation);
                if (modelValued.Rotation != null)
                    this.Rotation = new Vec3(modelValued.Rotation);
                if (modelValued.Scale != null)
                    this.Scale = new Vec3(modelValued.Scale);

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: ModelValued(ModelValued modelValued)

        #region Constructor: ModelValued(Model model)
        /// <summary>
        /// Creates a new valued model from the given model.
        /// </summary>
        /// <param name="model">The model to create a valued model from.</param>
        public ModelValued(Model model)
            : base(model)
        {
        }
        #endregion Constructor: ModelValued(Model model)

        #endregion Method Group: Constructors

    }
    #endregion Class: ModelValued

    #region Class: ModelCondition
    /// <summary>
    /// A condition on a model.
    /// </summary>
    public class ModelCondition : StaticContentCondition
    {

        #region Properties and Fields

        #region Property: Model
        /// <summary>
        /// Gets or sets the required model.
        /// </summary>
        public Model Model
        {
            get
            {
                return this.Node as Model;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Model

        #region Property: Translation
        /// <summary>
        /// A handler for a change in the translation.
        /// </summary>
        private Vec3.Vec3Handler translationChanged;

        /// <summary>
        /// The required translation.
        /// </summary>
        private Vec3 translation = null;

        /// <summary>
        /// Gets or sets the required translation.
        /// </summary>
        public Vec3 Translation
        {
            get
            {
                if (translation == null)
                {
                    translation = GameDatabase.Current.Select<Vec3>(this.ID, GameValueTables.ModelCondition, GameColumns.Translation);

                    if (translation != null)
                    {
                        if (translationChanged == null)
                            translationChanged = new Vec3.Vec3Handler(translation_ValueChanged);

                        translation.ValueChanged += translationChanged;
                    }
                }

                return translation;
            }
            set
            {
                if (translationChanged == null)
                    translationChanged = new Vec3.Vec3Handler(translation_ValueChanged);

                if (translation != null)
                    translation.ValueChanged -= translationChanged;

                translation = value;
                GameDatabase.Current.Update(this.ID, GameValueTables.ModelCondition, GameColumns.Translation, translation);
                NotifyPropertyChanged("Translation");

                if (translation != null)
                    translation.ValueChanged += translationChanged;
            }
        }

        /// <summary>
        /// Updates the database when a value of the translation changes.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        private void translation_ValueChanged(Vec3 vector)
        {
            GameDatabase.Current.Update(this.ID, GameValueTables.ModelCondition, GameColumns.Translation, translation);
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
                return GameDatabase.Current.Select<EqualitySignExtended?>(this.ID, GameValueTables.ModelCondition, GameColumns.TranslationSign);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.ModelCondition, GameColumns.TranslationSign, value);
                NotifyPropertyChanged("TranslationSign");
            }
        }
        #endregion Property: TranslationSign

        #region Property: Rotation
        /// <summary>
        /// A handler for a change in the rotation.
        /// </summary>
        private Vec3.Vec3Handler rotationChanged;

        /// <summary>
        /// The required rotation.
        /// </summary>
        private Vec3 rotation = null;

        /// <summary>
        /// Gets or sets the required rotation.
        /// </summary>
        public Vec3 Rotation
        {
            get
            {
                if (rotation == null)
                {
                    rotation = GameDatabase.Current.Select<Vec3>(this.ID, GameValueTables.ModelCondition, GameColumns.Rotation);

                    if (rotation != null)
                    {
                        if (rotationChanged == null)
                            rotationChanged = new Vec3.Vec3Handler(rotation_ValueChanged);

                        rotation.ValueChanged += rotationChanged;
                    }
                }

                return rotation;
            }
            set
            {
                if (rotationChanged == null)
                    rotationChanged = new Vec3.Vec3Handler(rotation_ValueChanged);

                if (rotation != null)
                    rotation.ValueChanged -= rotationChanged;

                rotation = value;
                GameDatabase.Current.Update(this.ID, GameValueTables.ModelCondition, GameColumns.Rotation, rotation);
                NotifyPropertyChanged("Rotation");

                if (rotation != null)
                    rotation.ValueChanged += rotationChanged;
            }
        }

        /// <summary>
        /// Updates the database when a value of the rotation changes.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        private void rotation_ValueChanged(Vec3 vector)
        {
            GameDatabase.Current.Update(this.ID, GameValueTables.ModelCondition, GameColumns.Rotation, rotation);
            NotifyPropertyChanged("Rotation");
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
                return GameDatabase.Current.Select<EqualitySignExtended?>(this.ID, GameValueTables.ModelCondition, GameColumns.RotationSign);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.ModelCondition, GameColumns.RotationSign, value);
                NotifyPropertyChanged("RotationSign");
            }
        }
        #endregion Property: RotationSign

        #region Property: Scale
        /// <summary>
        /// A handler for a change in the scale.
        /// </summary>
        private Vec3.Vec3Handler scaleChanged;

        /// <summary>
        /// The required scale.
        /// </summary>
        private Vec3 scale = null;

        /// <summary>
        /// Gets or sets the required scale.
        /// </summary>
        public Vec3 Scale
        {
            get
            {
                if (scale == null)
                {
                    scale = GameDatabase.Current.Select<Vec3>(this.ID, GameValueTables.ModelCondition, GameColumns.Scale);

                    if (scale != null)
                    {
                        if (scaleChanged == null)
                            scaleChanged = new Vec3.Vec3Handler(scale_ValueChanged);

                        scale.ValueChanged += scaleChanged;
                    }
                }

                return scale;
            }
            set
            {
                if (scaleChanged == null)
                    scaleChanged = new Vec3.Vec3Handler(scale_ValueChanged);

                if (scale != null)
                    scale.ValueChanged -= scaleChanged;

                scale = value;
                GameDatabase.Current.Update(this.ID, GameValueTables.ModelCondition, GameColumns.Scale, scale);
                NotifyPropertyChanged("Scale");

                if (scale != null)
                    scale.ValueChanged += scaleChanged;
            }
        }

        /// <summary>
        /// Updates the database when a value of the scale changes.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        private void scale_ValueChanged(Vec3 vector)
        {
            GameDatabase.Current.Update(this.ID, GameValueTables.ModelCondition, GameColumns.Scale, scale);
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
                return GameDatabase.Current.Select<EqualitySignExtended?>(this.ID, GameValueTables.ModelCondition, GameColumns.ScaleSign);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.ModelCondition, GameColumns.ScaleSign, value);
                NotifyPropertyChanged("ScaleSign");
            }
        }
        #endregion Property: ScaleSign

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ModelCondition()
        /// <summary>
        /// Creates a new model condition.
        /// </summary>
        public ModelCondition()
            : base()
        {
        }
        #endregion Constructor: ModelCondition()

        #region Constructor: ModelCondition(uint id)
        /// <summary>
        /// Creates a new model condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the model condition from.</param>
        protected ModelCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ModelCondition(uint id)

        #region Constructor: ModelCondition(ModelCondition modelCondition)
        /// <summary>
        /// Clones a model condition.
        /// </summary>
        /// <param name="modelCondition">The model condition to clone.</param>
        public ModelCondition(ModelCondition modelCondition)
            : base(modelCondition)
        {
            if (modelCondition != null)
            {
                GameDatabase.Current.StartChange();

                if (modelCondition.Translation != null)
                    this.Translation = new Vec3(modelCondition.Translation);
                this.TranslationSign = modelCondition.TranslationSign;

                if (modelCondition.Rotation != null)
                    this.Rotation = new Vec3(modelCondition.Rotation);
                this.RotationSign = modelCondition.RotationSign;

                if (modelCondition.Scale != null)
                    this.Scale = new Vec3(modelCondition.Scale);
                this.ScaleSign = modelCondition.ScaleSign;

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: ModelCondition(ModelCondition modelCondition)

        #region Constructor: ModelCondition(Model model)
        /// <summary>
        /// Creates a condition for the given model.
        /// </summary>
        /// <param name="model">The model to create a condition for.</param>
        public ModelCondition(Model model)
            : base(model)
        {
        }
        #endregion Constructor: ModelCondition(Model model)

        #endregion Method Group: Constructors

    }
    #endregion Class: ModelCondition

    #region Class: ModelChange
    /// <summary>
    /// A change on a model.
    /// </summary>
    public class ModelChange : StaticContentChange
    {

        #region Properties and Fields

        #region Property: Model
        /// <summary>
        /// Gets or sets the affected model.
        /// </summary>
        public Model Model
        {
            get
            {
                return this.Node as Model;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Model

        #region Property: Translation
        /// <summary>
        /// A handler for a change in the translation.
        /// </summary>
        private Vec3.Vec3Handler translationChanged;

        /// <summary>
        /// The translation to change to.
        /// </summary>
        private Vec3 translation = null;

        /// <summary>
        /// Gets or sets the translation to change to.
        /// </summary>
        public Vec3 Translation
        {
            get
            {
                if (translation == null)
                {
                    translation = GameDatabase.Current.Select<Vec3>(this.ID, GameValueTables.ModelChange, GameColumns.Translation);

                    if (translation != null)
                    {
                        if (translationChanged == null)
                            translationChanged = new Vec3.Vec3Handler(translation_ValueChanged);

                        translation.ValueChanged += translationChanged;
                    }
                }

                return translation;
            }
            set
            {
                if (translationChanged == null)
                    translationChanged = new Vec3.Vec3Handler(translation_ValueChanged);

                if (translation != null)
                    translation.ValueChanged -= translationChanged;

                translation = value;
                GameDatabase.Current.Update(this.ID, GameValueTables.ModelChange, GameColumns.Translation, translation);
                NotifyPropertyChanged("Translation");

                if (translation != null)
                    translation.ValueChanged += translationChanged;
            }
        }

        /// <summary>
        /// Updates the database when a value of the translation changes.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        private void translation_ValueChanged(Vec3 vector)
        {
            GameDatabase.Current.Update(this.ID, GameValueTables.ModelChange, GameColumns.Translation, translation);
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
                return GameDatabase.Current.Select<ValueChangeType?>(this.ID, GameValueTables.ModelChange, GameColumns.TranslationChange);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.ModelChange, GameColumns.TranslationChange, value);
                NotifyPropertyChanged("TranslationChange");
            }
        }
        #endregion Property: TranslationChange

        #region Property: Rotation
        /// <summary>
        /// A handler for a change in the rotation.
        /// </summary>
        private Vec3.Vec3Handler rotationChanged;

        /// <summary>
        /// The rotation to change to.
        /// </summary>
        private Vec3 rotation = null;

        /// <summary>
        /// Gets or sets the rotation to change to.
        /// </summary>
        public Vec3 Rotation
        {
            get
            {
                if (rotation == null)
                {
                    rotation = GameDatabase.Current.Select<Vec3>(this.ID, GameValueTables.ModelChange, GameColumns.Rotation);

                    if (rotation != null)
                    {
                        if (rotationChanged == null)
                            rotationChanged = new Vec3.Vec3Handler(rotation_ValueChanged);

                        rotation.ValueChanged += rotationChanged;
                    }
                }

                return rotation;
            }
            set
            {
                if (rotationChanged == null)
                    rotationChanged = new Vec3.Vec3Handler(rotation_ValueChanged);

                if (rotation != null)
                    rotation.ValueChanged -= rotationChanged;

                rotation = value;
                GameDatabase.Current.Update(this.ID, GameValueTables.ModelChange, GameColumns.Rotation, rotation);
                NotifyPropertyChanged("Rotation");

                if (rotation != null)
                    rotation.ValueChanged += rotationChanged;
            }
        }

        /// <summary>
        /// Updates the database when a value of the rotation changes.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        private void rotation_ValueChanged(Vec3 vector)
        {
            GameDatabase.Current.Update(this.ID, GameValueTables.ModelChange, GameColumns.Rotation, rotation);
            NotifyPropertyChanged("Rotation");
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
                return GameDatabase.Current.Select<ValueChangeType?>(this.ID, GameValueTables.ModelChange, GameColumns.RotationChange);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.ModelChange, GameColumns.RotationChange, value);
                NotifyPropertyChanged("RotationChange");
            }
        }
        #endregion Property: RotationChange

        #region Property: Scale
        /// <summary>
        /// A handler for a change in the scale.
        /// </summary>
        private Vec3.Vec3Handler scaleChanged;

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
                if (scale == null)
                {
                    scale = GameDatabase.Current.Select<Vec3>(this.ID, GameValueTables.ModelChange, GameColumns.Scale);

                    if (scale != null)
                    {
                        if (scaleChanged == null)
                            scaleChanged = new Vec3.Vec3Handler(scale_ValueChanged);

                        scale.ValueChanged += scaleChanged;
                    }
                }

                return scale;
            }
            set
            {
                if (scaleChanged == null)
                    scaleChanged = new Vec3.Vec3Handler(scale_ValueChanged);

                if (scale != null)
                    scale.ValueChanged -= scaleChanged;

                scale = value;
                GameDatabase.Current.Update(this.ID, GameValueTables.ModelChange, GameColumns.Scale, scale);
                NotifyPropertyChanged("Scale");

                if (scale != null)
                    scale.ValueChanged += scaleChanged;
            }
        }

        /// <summary>
        /// Updates the database when a value of the scale changes.
        /// </summary>
        /// <param name="vector">The changed vector.</param>
        private void scale_ValueChanged(Vec3 vector)
        {
            GameDatabase.Current.Update(this.ID, GameValueTables.ModelChange, GameColumns.Scale, scale);
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
                return GameDatabase.Current.Select<ValueChangeType?>(this.ID, GameValueTables.ModelChange, GameColumns.ScaleChange);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.ModelChange, GameColumns.ScaleChange, value);
                NotifyPropertyChanged("ScaleChange");
            }
        }
        #endregion Property: ScaleChange

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ModelChange()
        /// <summary>
        /// Creates a model change.
        /// </summary>
        public ModelChange()
            : base()
        {
        }
        #endregion Constructor: ModelChange()

        #region Constructor: ModelChange(uint id)
        /// <summary>
        /// Creates a new model change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a model change from.</param>
        protected ModelChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ModelChange(uint id)

        #region Constructor: ModelChange(ModelChange modelChange)
        /// <summary>
        /// Clones a model change.
        /// </summary>
        /// <param name="modelChange">The model change to clone.</param>
        public ModelChange(ModelChange modelChange)
            : base(modelChange)
        {
            if (modelChange != null)
            {
                GameDatabase.Current.StartChange();

                if (modelChange.Translation != null)
                    this.Translation = new Vec3(modelChange.Translation);
                this.TranslationChange = modelChange.TranslationChange;

                if (modelChange.Rotation != null)
                    this.Rotation = new Vec3(modelChange.Rotation);
                this.RotationChange = modelChange.RotationChange;

                if (modelChange.Scale != null)
                    this.Scale = new Vec3(modelChange.Scale);
                this.ScaleChange = modelChange.ScaleChange;

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: ModelChange(ModelChange modelChange)

        #region Constructor: ModelChange(Model model)
        /// <summary>
        /// Creates a change for the given model.
        /// </summary>
        /// <param name="model">The model to create a change for.</param>
        public ModelChange(Model model)
            : base(model)
        {
        }
        #endregion Constructor: ModelChange(Model model)

        #endregion Method Group: Constructors

    }
    #endregion Class: ModelChange

}
