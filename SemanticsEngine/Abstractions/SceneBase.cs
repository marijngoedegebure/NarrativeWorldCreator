/**************************************************************************
 * 
 * SceneBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Semantics.Abstractions;
using Semantics.Entities;
using Semantics.Utilities;
using SemanticsEngine.Components;
using SemanticsEngine.Entities;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Abstractions
{

    #region Class: SceneBase
    /// <summary>
    /// A base of a scene.
    /// </summary>
    public class SceneBase : AbstractionBase
    {

        #region Properties and Fields

        #region Property: Scene
        /// <summary>
        /// Gets the scene of which this is a scene base.
        /// </summary>
        protected internal Scene Scene
        {
            get
            {
                return this.IdHolder as Scene;
            }
        }
        #endregion Property: Scene

        #region Property: PhysicalObjects
        /// <summary>
        /// All physical objects that belong to the scene.
        /// </summary>
        private PhysicalObjectValuedBase[] physicalObjects = null;

        /// <summary>
        /// Gets all physical objects that belong to the scene.
        /// </summary>
        public ReadOnlyCollection<PhysicalObjectValuedBase> PhysicalObjects
        {
            get
            {
                if (physicalObjects == null)
                {
                    LoadPhysicalObjects();
                    if (physicalObjects == null)
                        return new List<PhysicalObjectValuedBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<PhysicalObjectValuedBase>(physicalObjects);
            }
        }

        /// <summary>
        /// Loads the physical objects.
        /// </summary>
        private void LoadPhysicalObjects()
        {
            if (this.Scene != null)
            {
                List<PhysicalObjectValuedBase> physicalObjectValuedBases = new List<PhysicalObjectValuedBase>();
                foreach (PhysicalObjectValued physicalObjectValued in this.Scene.PhysicalObjects)
                    physicalObjectValuedBases.Add(BaseManager.Current.GetBase<PhysicalObjectValuedBase>(physicalObjectValued));
                physicalObjects = physicalObjectValuedBases.ToArray();
            }
        }
        #endregion Property: PhysicalObjects

        #region Property: Scenes
        /// <summary>
        /// All scenes that belong to the scene.
        /// </summary>
        private SceneValuedBase[] scenes = null;
        
        /// <summary>
        /// Gets all scenes that belong to the scene.
        /// </summary>
        public ReadOnlyCollection<SceneValuedBase> Scenes
        {
            get
            {
                if (scenes == null)
                {
                    LoadScenes();
                    if (scenes == null)
                        return new List<SceneValuedBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<SceneValuedBase>(scenes);
            }
        }

        /// <summary>
        /// Loads the scenes.
        /// </summary>
        private void LoadScenes()
        {
            if (this.Scene != null)
            {
                List<SceneValuedBase> sceneBases = new List<SceneValuedBase>();
                foreach (SceneValued sceneValued in this.Scene.Scenes)
                    sceneBases.Add(BaseManager.Current.GetBase<SceneValuedBase>(sceneValued));
                scenes = sceneBases.ToArray();
            }
        }
        #endregion Property: Scenes

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SceneBase(Scene scene)
        /// <summary>
        /// Create a scene base from the given scene.
        /// </summary>
        /// <param name="scene">The scene to create a scene base from.</param>
        protected internal SceneBase(Scene scene)
            : base(scene)
        {
            if (scene != null)
            {
                if (BaseManager.PreloadProperties)
                {
                    LoadPhysicalObjects();
                    LoadScenes();
                }
            }
        }
        #endregion Constructor: SceneBase(Scene scene)

        #region Constructor: SceneBase(SceneValued sceneValued)
        /// <summary>
        /// Create a scene base from the given valued scene.
        /// </summary>
        /// <param name="sceneValued">The valued scene to create a scene base from.</param>
        protected internal SceneBase(SceneValued sceneValued)
            : base(sceneValued)
        {
            if (sceneValued != null)
            {
                if (BaseManager.PreloadProperties)
                {
                    LoadPhysicalObjects();
                    LoadScenes();
                }
            }
        }
        #endregion Constructor: SceneBase(SceneValued sceneValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: SceneBase

    #region Class: SceneValuedBase
    /// <summary>
    /// A base of a valued scene.
    /// </summary>
    public class SceneValuedBase : AbstractionValuedBase
    {

        #region Properties and Fields

        #region Property: SceneValued
        /// <summary>
        /// Gets the valued scene of which this is a valued scene base.
        /// </summary>
        protected internal SceneValued SceneValued
        {
            get
            {
                return this.NodeValued as SceneValued;
            }
        }
        #endregion Property: SceneValued

        #region Property: SceneBase
        /// <summary>
        /// Gets the scene of which this is a valued scene base.
        /// </summary>
        public SceneBase SceneBase
        {
            get
            {
                return this.NodeBase as SceneBase;
            }
        }
        #endregion Property: SceneBase

        #region Property: Quantity
        /// <summary>
        /// The quantity.
        /// </summary>
        private NumericalValueRangeBase quantity = null;

        /// <summary>
        /// Gets the quantity.
        /// </summary>
        public NumericalValueRangeBase Quantity
        {
            get
            {
                if (quantity == null)
                {
                    LoadQuantity();
                    if (quantity == null)
                        quantity = new NumericalValueRangeBase(SemanticsSettings.Values.Quantity);
                }
                return quantity;
            }
        }

        /// <summary>
        /// Loads the quantity.
        /// </summary>
        private void LoadQuantity()
        {
            if (this.SceneValued != null)
                quantity = BaseManager.Current.GetBase<NumericalValueRangeBase>(this.SceneValued.Quantity);
        }
        #endregion Property: Quantity

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SceneValuedBase(SceneValued sceneValued)
        /// <summary>
        /// Create a valued scene base from the given valued scene.
        /// </summary>
        /// <param name="sceneValued">The valued scene to create a valued scene base from.</param>
        protected internal SceneValuedBase(SceneValued sceneValued)
            : base(sceneValued)
        {
            if (sceneValued != null)
            {
                if (BaseManager.PreloadProperties)
                    LoadQuantity();
            }
        }
        #endregion Constructor: SceneValuedBase(SceneValued sceneValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: SceneValuedBase

}
