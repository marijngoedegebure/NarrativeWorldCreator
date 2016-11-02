/**************************************************************************
 * 
 * GameInstanceManager.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using GameSemantics.Components;
using GameSemanticsEngine.Components;
using GameSemanticsEngine.GameContent;
using SemanticsEngine.Entities;
using SemanticsEngine.Tools;

namespace GameSemanticsEngine.Tools
{

    #region TODO Class: GameInstanceManager
    /// <summary>
    /// The instance manager allows the creation of instances from ID holders or bases.
    /// </summary>
    public class GameInstanceManager : InstanceManager
    {

        #region Property: Current
        /// <summary>
        /// Gets the current instance manager.
        /// </summary>
        public static new GameInstanceManager Current
        {
            get
            {
                return InstanceManager.Current as GameInstanceManager;
            }
            protected set
            {
                InstanceManager.Current = value;
            }
        }
        #endregion Property: Current

        #region Static Constructor: GameInstanceManager()
        /// <summary>
        /// Add pairs of non-abstract bases and their corresponding instance classes.
        /// </summary>
        static GameInstanceManager()
        {
            // Content
            AddBaseInstancePair(typeof(AnimationValuedBase), typeof(AnimationInstance));
            AddBaseInstancePair(typeof(AudioValuedBase), typeof(AudioInstance));
            AddBaseInstancePair(typeof(CinematicValuedBase), typeof(CinematicInstance));
            AddBaseInstancePair(typeof(GameFilterValuedBase), typeof(GameFilterInstance));
            AddBaseInstancePair(typeof(GameMaterialValuedBase), typeof(GameMaterialInstance));
            AddBaseInstancePair(typeof(IconValuedBase), typeof(IconInstance));
            AddBaseInstancePair(typeof(ModelValuedBase), typeof(ModelInstance));
            AddBaseInstancePair(typeof(ParticleEmitterValuedBase), typeof(ParticleEmitterInstance));
            AddBaseInstancePair(typeof(ParticlePropertiesValuedBase), typeof(ParticlePropertiesInstance));
            AddBaseInstancePair(typeof(ScriptValuedBase), typeof(ScriptInstance));
            AddBaseInstancePair(typeof(SpriteValuedBase), typeof(SpriteInstance));
        }
        #endregion Static Constructor: GameInstanceManager()

        #region Method: Initialize()
        /// <summary>
        /// Initialize the game instance manager.
        /// </summary>
        public static void Initialize()
        {
            // Make the game instance manager the current instance manager
            Current = new GameInstanceManager();
        }
        #endregion Method: Initialize()

        #region Method: Create(GameObject gameObject)
        /// <summary>
        /// Create an instance of the given game object.
        /// </summary>
        /// <param name="gameObject">The game object to create an instance of.</param>
        /// <returns>The created instance.</returns>
        public TangibleObjectInstance Create(GameObject gameObject)
        {
            return Create(gameObject, SemanticsEngineSettings.DefaultCreateOptions);
        }
        #endregion Method: Create(GameObject gameObject)

        #region Method: Create(GameObject gameObject, CreateOptions createOptions)
        /// <summary>
        /// Create an instance of the given game object.
        /// </summary>
        /// <param name="gameObject">The game object to create an instance of.</param>
        /// <param name="createOptions">The create options.</param>
        /// <returns>The created instance.</returns>
        public TangibleObjectInstance Create(GameObject gameObject, CreateOptions createOptions)
        {
            if (gameObject != null)
            {
                // Get the base of the game object, and create an instance of it
                GameObjectBase gameObjectBase = GameBaseManager.Current.GetBase<GameObjectBase>(gameObject);
                if (gameObjectBase != null)
                    return Create(gameObjectBase, createOptions);
            }

            return null;
        }
        #endregion Method: Create(GameObject gameObject, CreateOptions createOptions)

        #region Method: Create(GameObjectBase gameObjectBase)
        /// <summary>
        /// Create an instance of the given game object.
        /// </summary>
        /// <param name="gameObjectBase">The game object to create an instance of.</param>
        /// <returns>The created instance.</returns>
        public TangibleObjectInstance Create(GameObjectBase gameObjectBase)
        {
            return Create(gameObjectBase, SemanticsEngineSettings.DefaultCreateOptions);
        }
        #endregion Method: Create(GameObjectBase gameObjectBase)

        #region Method: Create(GameObjectBase gameObjectBase, CreateOptions createOptions)
        /// <summary>
        /// Create an instance of the given game object.
        /// </summary>
        /// <param name="gameObjectBase">The game object to create an instance of.</param>
        /// <param name="createOptions">The create options.</param>
        /// <returns>The created instance.</returns>
        public TangibleObjectInstance Create(GameObjectBase gameObjectBase, CreateOptions createOptions)
        {
            return GameInstanceManager.Current.Create(gameObjectBase, createOptions, false);
        }
        #endregion Method: Create(GameObjectBase gameObjectBase, CreateOptions createOptions)

        #region TODO Method: Create(GameObjectBase gameObjectBase, CreateOptions createOptions, bool skipContentCreation)
        /// <summary>
        /// Create an instance of the given game object.
        /// </summary>
        /// <param name="gameObjectBase">The game object to create an instance of.</param>
        /// <param name="createOptions">The create options.</param>
        /// <param name="skipContentCreation">Indicates whether content creation should be skipped.</param>
        /// <returns>The created instance.</returns>
        private TangibleObjectInstance Create(GameObjectBase gameObjectBase, CreateOptions createOptions, bool skipContentCreation)
        {
            if (gameObjectBase != null)
            {
                // Create an instance of the tangible object of the game object
                createOptions = createOptions.Remove(CreateOptions.Matter);
                TangibleObjectInstance tangibleObjectInstance = GameInstanceManager.Current.Create(gameObjectBase.TangibleObject, createOptions);
                if (tangibleObjectInstance != null)
                {
                    if (gameObjectBase.Matter.Count > 0)
                    {
                        // Create and add the matter defined for the game object
                        foreach (MatterValuedBase matterValuedBase in gameObjectBase.Matter)
                            tangibleObjectInstance.AddMatter(GameInstanceManager.Current.Create<MatterInstance>(matterValuedBase));
                    }
                    else
                    {
                        // If no matter has been defined for the game object, create the matter of the tangible object
                        foreach (MatterValuedBase matterValuedBase in gameObjectBase.TangibleObject.Matter)
                            tangibleObjectInstance.AddMatter(GameInstanceManager.Current.Create<MatterInstance>(matterValuedBase));
                    }

                    // Create the defined game objects of the parts
                    foreach (TangibleObjectInstance part in tangibleObjectInstance.Parts)
                    {
                        if (part.TangibleObjectBase != null)
                        {
                            foreach (KeyValuePair<PartBase, GameObjectBase> partGameObject in gameObjectBase.PartGameObjects)
                            {
                                if (part.TangibleObjectBase.Equals(partGameObject.Key.TangibleObjectBase))
                                {
                                    // Create the part instance and manually create its content from the set game object
                                    TangibleObjectInstance partInstance = GameInstanceManager.Current.Create(partGameObject.Value, createOptions, true);
                                    GameSemanticsEngine.Current.CreateContent(partInstance, partGameObject.Value);
                                    GameSemanticsEngine.Current.contentToSkip.Add(partInstance);
                                    break;
                                }
                            }
                        }
                    }

                    // Create the defined game objects of the covers
                    foreach (TangibleObjectInstance cover in tangibleObjectInstance.Covers)
                    {
                        if (cover.TangibleObjectBase != null)
                        {
                            foreach (KeyValuePair<CoverBase, GameObjectBase> coverGameObject in gameObjectBase.CoverGameObjects)
                            {
                                if (cover.TangibleObjectBase.Equals(coverGameObject.Key.TangibleObjectBase))
                                {
                                    // Create the cover instance and manually create its content from the set game object
                                    TangibleObjectInstance coverInstance = GameInstanceManager.Current.Create(coverGameObject.Value, createOptions, true);
                                    GameSemanticsEngine.Current.CreateContent(coverInstance, coverGameObject.Value);
                                    GameSemanticsEngine.Current.contentToSkip.Add(coverInstance);
                                    break;
                                }
                            }
                        }
                    }

                    //// Create the defined game nodes of the layers
                    //foreach (LayerInstance layer in tangibleObjectInstance.Layers)
                    //{
                    //    if (layer.LayerBase != null)
                    //    {
                    //        foreach (KeyValuePair<LayerBase, GameNodeBase> layerGameNode in gameObjectBase.LayerGameNodes)
                    //        {
                    //            if (layer.LayerBase.Equals(layerGameNode.Key))
                    //            {
                    //                // Add the layer instance and manually create its content from the set game node
                    //                EntityInstance layerInstance = world.AddInstance(layerGameNode.Value, true);
                    //                GameSemanticsEngine.CurrentSemanticsEngine.CreateContent(layerInstance, layerGameNode.Value);
                    //                break;
                    //            }
                    //        }
                    //    }
                    //}

                    // Create content
                    if (!skipContentCreation)
                    {
                        GameSemanticsEngine.Current.CreateContent(tangibleObjectInstance, gameObjectBase);
                        GameSemanticsEngine.Current.contentToSkip.Add(tangibleObjectInstance);
                    }

                    return tangibleObjectInstance;
                }
            }

            return null;
        }
        #endregion Method: Create(GameObjectBase gameObjectBase, CreateOptions createOptions, bool skipContentCreation)

        #region Method: GetRequiredMatter(TangibleObjectBase tangibleObjectBase)
        /// <summary>
        /// Get the matter required to create an instance of the given tangible object.
        /// </summary>
        /// <param name="tangibleObjectBase">The tangible object base to get the required matter of.</param>
        /// <returns>The required matter to create an instance of the tangible object.</returns>
        protected override ReadOnlyCollection<MatterValuedBase> GetRequiredMatter(TangibleObjectBase tangibleObjectBase)
        {
            if (tangibleObjectBase != null)
            {
                // Get the best game object of the tangible object and get the defined matter
                GameObjectBase gameObjectBase = GameSemanticsEngine.Current.GetAbstractGameNodeBase(tangibleObjectBase) as GameObjectBase;
                if (gameObjectBase != null)
                {
                    if (gameObjectBase.Matter.Count > 0)
                        return gameObjectBase.Matter;
                }
            }
            
            // If not, return the matter of the base
            return base.GetRequiredMatter(tangibleObjectBase);
        }
        #endregion Method: GetRequiredMatter(TangibleObjectBase tangibleObjectBase)

    }
    #endregion Class: GameInstanceManager

}