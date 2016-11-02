/**************************************************************************
 * 
 * SemanticWorldExtensions.cs
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
using Common;
using GameSemantics.Components;
using GameSemanticsEngine.Components;
using GameSemanticsEngine.Tools;
using SemanticsEngine.Entities;
using SemanticsEngine.Tools;
using SemanticsEngine.Worlds;

namespace GameSemanticsEngine.Worlds
{

    #region TODO Class: SemanticWorldExtensions
    /// <summary>
    /// Extensions for the SemanticWorld class.
    /// </summary>
    public static class SemanticWorldExtensions
    {

        #region Method: AddInstance(this SemanticWorld world, GameObject gameObject)
        /// <summary>
        /// Add the game object to the world.
        /// </summary>
        /// <param name="gameObject">The game object to create and add an instance of.</param>
        /// <returns>The tangible object instance of the game object that was added to the world.</returns>
        public static TangibleObjectInstance AddInstance(this SemanticWorld world, GameObject gameObject)
        {
            if (world != null)
                return world.AddInstance(gameObject, Vec3.Zero, Quaternion.Identity);
            return null;
        }
        #endregion Method: AddInstance(this SemanticWorld world, GameObject gameObject)

        #region Method: AddInstance(this SemanticWorld world, GameObject gameObject, Vec3 position)
        /// <summary>
        /// Add the game object to the world on the given position.
        /// </summary>
        /// <param name="gameObject">The game object to create and add an instance of.</param>
        /// <param name="position">The position in the world the instance should get.</param>
        /// <returns>The tangible object instance of the game object that was added to the world.</returns>
        public static TangibleObjectInstance AddInstance(this SemanticWorld world, GameObject gameObject, Vec3 position)
        {
            if (world != null)
                return world.AddInstance(gameObject, position, Quaternion.Identity);
            return null;
        }
        #endregion Method: AddInstance(this SemanticWorld world, GameObject gameObject, Vec3 position)

        #region Method: AddInstance(this SemanticWorld world, GameObject gameObject, Vec3 position, Quaternion rotation)
        /// <summary>
        /// Add the game object to the world on the given position and with the given rotation.
        /// </summary>
        /// <param name="gameObject">The game object to create and add an instance of.</param>
        /// <param name="position">The position in the world the instance should get.</param>
        /// <param name="rotation">The rotation the instance should get.</param>
        /// <returns>The tangible object instance of the game object that was added to the world.</returns>
        public static TangibleObjectInstance AddInstance(this SemanticWorld world, GameObject gameObject, Vec3 position, Quaternion rotation)
        {
            if (world != null)
                return world.AddInstance(GameBaseManager.Current.GetBase<GameObjectBase>(gameObject), position, rotation);
            return null;
        }
        #endregion Method: AddInstance(this SemanticWorld world, GameObject gameObject, Vec3 position, Quaternion rotation)

        #region Method: AddInstance(this SemanticWorld world, GameObjectBase gameObjectBase, Vec3 position, Quaternion rotation)
        /// <summary>
        /// Add the game object to the world on the given position and with the given rotation.
        /// </summary>
        /// <param name="gameObjectBase">The game object to create and add an instance of.</param>
        /// <param name="position">The position in the world the instance should get.</param>
        /// <param name="rotation">The rotation the instance should get.</param>
        /// <returns>The tangible object instance of the game object that was added to the world.</returns>
        public static TangibleObjectInstance AddInstance(this SemanticWorld world, GameObjectBase gameObjectBase, Vec3 position, Quaternion rotation)
        {
            if (world != null)
                return world.AddInstance(gameObjectBase, position, rotation, false);
            return null;
        }
        #endregion Method: AddInstance(this SemanticWorld world, GameObjectBase gameObjectBase, Vec3 position, Quaternion rotation)

        #region TODO Method: AddInstance(this SemanticWorld world, GameObjectBase gameObjectBase, Vec3 position, Quaternion rotation, bool skipContentCreation)
        /// <summary>
        /// Add the game object to the world on the given position and with the given rotation.
        /// </summary>
        /// <param name="gameObjectBase">The game object to create and add an instance of.</param>
        /// <param name="position">The position in the world the instance should get.</param>
        /// <param name="rotation">The rotation the instance should get.</param>
        /// <param name="skipContentCreation">Indicates whether content creation should be skipped.</param>
        /// <returns>The tangible object instance of the game object that was added to the world.</returns>
        private static TangibleObjectInstance AddInstance(this SemanticWorld world, GameObjectBase gameObjectBase, Vec3 position, Quaternion rotation, bool skipContentCreation)
        {
            if (world != null && gameObjectBase != null)
            {
                // Create an instance of the tangible object of the game object, without the matter
                CreateOptions createOptions = SemanticsEngineSettings.DefaultCreateOptions;
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
                                    // TODO: Quantities! what if random?

                                    // Add the part instance and manually create its content from the set game object
                                    TangibleObjectInstance partInstance = world.AddInstance(partGameObject.Value, part.Position, part.Rotation, true);
                                    GameSemanticsEngine.Current.CreateContent(partInstance, partGameObject.Value);
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
                                    // TODO: Quantities! what if random?

                                    // Add the cover instance and manually create its content from the set game object
                                    TangibleObjectInstance coverInstance = world.AddInstance(coverGameObject.Value, cover.Position, cover.Rotation, true);
                                    GameSemanticsEngine.Current.CreateContent(coverInstance, coverGameObject.Value);
                                    break;
                                }
                            }
                        }
                    }

                    //// Create the defined game nodes of the layers
                    //foreach (MatterInstance layer in tangibleObjectInstance.Layers)
                    //{
                    //    if (layer.MatterBase != null)
                    //    {
                    //        foreach (KeyValuePair<LayerBase, GameNodeBase> layerGameNode in gameObjectBase.LayerGameNodes)
                    //        {
                    //            if (layer.MatterBase.Equals(layerGameNode.Key.MatterBase))
                    //            {
                    //                // Add the layer instance and manually create its content from the set game node
                    //                EntityInstance layerInstance = world.AddInstance(layerGameNode.Value, true);
                    //                GameSemanticsEngine.Current.CreateContent(layerInstance, layerGameNode.Value);
                    //                break;
                    //            }
                    //        }
                    //    }
                    //}

                    // Add the instance to the world
                    if (!skipContentCreation)
                    {
                        GameSemanticsEngine.Current.contentToSkip.Add(tangibleObjectInstance);
                        world.AddInstance(tangibleObjectInstance, position, rotation);
                        GameSemanticsEngine.Current.contentToSkip.Remove(tangibleObjectInstance);

						// Create content based on this game object
						GameSemanticsEngine.Current.CreateContent(tangibleObjectInstance, gameObjectBase);
                    }

                    return tangibleObjectInstance;
                }
            }

            return null;
        }
        #endregion Method: AddInstance(this SemanticWorld world, GameObjectBase gameObjectBase, Vec3 position, Quaternion rotation, bool skipContentCreation)

    }
    #endregion Class: SemanticWorldExtensions

}