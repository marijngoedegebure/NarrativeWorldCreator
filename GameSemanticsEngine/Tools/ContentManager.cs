/**************************************************************************
 * 
 * ContentManager.cs
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
using System.Collections.Generic;
using GameSemanticsEngine.Components;
using GameSemanticsEngine.GameContent;
using GameSemanticsEngine.Interfaces;
using SemanticsEngine.Components;
using SemanticsEngine.Entities;
using SemanticsEngine.Worlds;

namespace GameSemanticsEngine.Tools
{

    #region Class: ContentManager
    /// <summary>
    /// A manager for content.
    /// </summary>
    public static class ContentManager
    {

        #region Properties and Fields

        #region Field: contentPerInstance
        /// <summary>
        /// A dictionary containing the content wrapper for a node instance.
        /// </summary>
        private static Dictionary<NodeInstance, ContentWrapper> contentPerInstance = new Dictionary<NodeInstance, ContentWrapper>();
        #endregion Field: contentPerInstance

        #region Field: contentHandlers
        /// <summary>
        /// The content handlers.
        /// </summary>
        private static List<IContentHandler> contentHandlers = new List<IContentHandler>();
        #endregion Field: contentHandlers

        #region Field: contentHandlersPerWorld
        /// <summary>
        /// The content handlers per world.
        /// </summary>
        private static Dictionary<SemanticWorld, List<IContentHandler>> contentHandlersPerWorld = new Dictionary<SemanticWorld, List<IContentHandler>>();
        #endregion Field: contentHandlersPerWorld

        #endregion Properties and Fields

        #region Method Group: Content wrappers

        #region Method: AddContentWrapper(NodeInstance nodeInstance, ContentWrapper contentWrapper)
        /// <summary>
        /// Add a content wrapper for the given node instance.
        /// </summary>
        /// <param name="nodeInstance">The node instance to add.</param>
        /// <param name="contentWrapper">The content wrapper to assign to the node instance.</param>
        public static void AddContentWrapper(NodeInstance nodeInstance, ContentWrapper contentWrapper)
        {
            if (contentPerInstance.ContainsKey(nodeInstance))
                contentPerInstance[nodeInstance] = contentWrapper;
            else
                contentPerInstance.Add(nodeInstance, contentWrapper);
        }
        #endregion Method: AddContentWrapper(NodeInstance nodeInstance, ContentWrapper contentWrapper)

        #region Method: GetContentWrapper(NodeInstance nodeInstance)
        /// <summary>
        /// Get the content wrapper for the given node instance.
        /// </summary>
        /// <param name="nodeInstance">The node instance to get the content wrapper of.</param>
        /// <returns>The content wrapper of the node instance.</returns>
        public static ContentWrapper GetContentWrapper(NodeInstance nodeInstance)
        {
            ContentWrapper contentWrapper = null;
            contentPerInstance.TryGetValue(nodeInstance, out contentWrapper);
            return contentWrapper;
        }
        #endregion Method: GetContentWrapper(NodeInstance nodeInstance)

        #region Method: TryGetContentWrapper(NodeInstance nodeInstance, out ContentWrapper contentWrapper)
        /// <summary>
        /// Try to get the content wrapper for the given node instance.
        /// </summary>
        /// <param name="nodeInstance">The node instance to get the content wrapper of.</param>
        /// <param name="contentWrapper">The content wrapper of the node instance.</param>
        /// <returns>Returns whether the retrieval has been successful.</returns>
        public static bool TryGetContentWrapper(NodeInstance nodeInstance, out ContentWrapper contentWrapper)
        {
            return contentPerInstance.TryGetValue(nodeInstance, out contentWrapper);
        }
        #endregion Method: TryGetContentWrapper(NodeInstance nodeInstance, out ContentWrapper contentWrapper)

        #endregion Method Group: Content wrappers

        #region Method Group: Interfaces

        #region Method: Register(IContentHandler iContentHandler)
        /// <summary>
        /// Register the given content handler.
        /// </summary>
        /// <param name="iContentHandler">The content handler to register.</param>
        public static void Register(IContentHandler iContentHandler)
        {
            if (!contentHandlers.Contains(iContentHandler))
                contentHandlers.Add(iContentHandler);
        }
        #endregion Method: Register(IContentHandler iContentHandler)

        #region Method: Unregister(IContentHandler iContentHandler)
        /// <summary>
        /// Unregister the given content handler.
        /// </summary>
        /// <param name="iContentHandler">The content handler to unregister.</param>
        public static void Unregister(IContentHandler iContentHandler)
        {
            if (contentHandlers.Contains(iContentHandler))
                contentHandlers.Remove(iContentHandler);
        }
        #endregion Method: Unregister(IContentHandler iContentHandler)

        #region Method: Register(IContentHandler iContentHandler, SemanticWorld semanticWorld)
        /// <summary>
        /// Register the given content handler for the given semantic world.
        /// </summary>
        /// <param name="iContentHandler">The content handler to register.</param>
        /// <param name="semanticWorld">The semantic world to register the content handler for.</param>
        public static void Register(IContentHandler iContentHandler, SemanticWorld semanticWorld)
        {
            if (contentHandlersPerWorld.ContainsKey(semanticWorld))
                contentHandlersPerWorld[semanticWorld].Add(iContentHandler);
            else
            {
                List<IContentHandler> list = new List<IContentHandler>();
                list.Add(iContentHandler);
                contentHandlersPerWorld.Add(semanticWorld, list);
            }
        }
        #endregion Method: Register(IContentHandler iContentHandler, SemanticWorld semanticWorld)

        #region Method: Unregister(IContentHandler iContentHandler, SemanticWorld semanticWorld)
        /// <summary>
        /// Unregister the given content handler for the given semantic world.
        /// </summary>
        /// <param name="iContentHandler">The content handler to unregister.</param>
        /// <param name="semanticWorld">The semantic world to unregister the content handler for.</param>
        public static void Unregister(IContentHandler iContentHandler, SemanticWorld semanticWorld)
        {
            if (contentHandlersPerWorld.ContainsKey(semanticWorld))
            {
                if (contentHandlersPerWorld[semanticWorld].Contains(iContentHandler))
                {
                    contentHandlersPerWorld[semanticWorld].Remove(iContentHandler);
                    if (contentHandlersPerWorld[semanticWorld].Count == 0)
                        contentHandlersPerWorld.Remove(semanticWorld);
                }
            }
        }
        #endregion Method: Unregister(IContentHandler iContentHandler, SemanticWorld semanticWorld)

        #region Method: GetContentHandlers(NodeInstance nodeInstance)
        /// <summary>
        /// Get the content handlers for the given node instance.
        /// </summary>
        /// <param name="nodeInstance">The node instance to get the content handlers of.</param>
        /// <returns>Thye content handlers of the node instance.</returns>
        private static List<T> GetContentHandlers<T>(NodeInstance nodeInstance)
            where T : IContentHandler
        {
            List<T> handlers = new List<T>();

            Type type = typeof(T);
            foreach (IContentHandler contentHandler in contentHandlers)
            {
                if (type.IsAssignableFrom(contentHandler.GetType()))
                    handlers.Add((T)contentHandler);
            }
            EntityInstance entityInstance = nodeInstance as EntityInstance;
            if (entityInstance != null && entityInstance.World != null)
            {
                List<IContentHandler> handlersPerWorld = null;
                if (contentHandlersPerWorld.TryGetValue(entityInstance.World, out handlersPerWorld))
                {
                    foreach (IContentHandler contentHandler in handlersPerWorld)
                    {
                        if (type.IsAssignableFrom(contentHandler.GetType()))
                            handlers.Add((T)contentHandler);
                    }
                }
            }

            return handlers;
        }
        #endregion Method: GetContentHandlers(NodeInstance nodeInstance)
		
        #endregion Method Group: Interfaces

        #region Method Group: Handlers

        #region Method: InvokeGameMaterialShown(NodeInstance nodeInstance, ViewBase viewBase, GameMaterialInstance gameMaterialInstance)
        /// <summary>
        /// Invoke an event that a game material of a node instance should be shown for a particular view.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the game material was changed.</param>
        /// <param name="viewBase">The view for which the game materal was changed.</param>
        /// <param name="gameMaterialInstance">The new game material instance.</param>
        internal static void InvokeGameMaterialShown(NodeInstance nodeInstance, ViewBase viewBase, GameMaterialInstance gameMaterialInstance)
        {
            foreach (IGameMaterialHandler handler in GetContentHandlers<IGameMaterialHandler>(nodeInstance))
                handler.GameMaterialShown(nodeInstance, viewBase, gameMaterialInstance);
        }
        #endregion Method: InvokeGameMaterialShown(NodeInstance nodeInstance, ViewBase viewBase, GameMaterialInstance gameMaterialInstance)

        #region Method: InvokeGameMaterialHidden(NodeInstance nodeInstance, ViewBase viewBase, GameMaterialInstance gameMaterialInstance)
        /// <summary>
        /// Invoke an event that a game material of a node instance should be hidden for a particular view.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the game material was changed.</param>
        /// <param name="viewBase">The view for which the game materal was changed.</param>
        /// <param name="gameMaterialInstance">The game material instance.</param>
        internal static void InvokeGameMaterialHidden(NodeInstance nodeInstance, ViewBase viewBase, GameMaterialInstance gameMaterialInstance)
        {
            foreach (IGameMaterialHandler handler in GetContentHandlers<IGameMaterialHandler>(nodeInstance))
                handler.GameMaterialHidden(nodeInstance, viewBase, gameMaterialInstance);
        }
        #endregion Method: InvokeGameMaterialHidden(NodeInstance nodeInstance, ViewBase viewBase, GameMaterialInstance gameMaterialInstance)

        #region Method: InvokeIconShown(NodeInstance nodeInstance, ViewBase viewBase, IconInstance iconInstance)
        /// <summary>
        /// Invoke an event that an icon of a node instance should be shown for a particular view.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the icon was changed.</param>
        /// <param name="viewBase">The view for which the icon was changed.</param>
        /// <param name="iconInstance">The icon instance.</param>
        internal static void InvokeIconShown(NodeInstance nodeInstance, ViewBase viewBase, IconInstance iconInstance)
        {
            foreach (IIconHandler handler in GetContentHandlers<IIconHandler>(nodeInstance))
                handler.IconShown(nodeInstance, viewBase, iconInstance);
        }
        #endregion Method: InvokeIconShown(NodeInstance nodeInstance, ViewBase viewBase, IconInstance iconInstance)

        #region Method: InvokeIconHidden(NodeInstance nodeInstance, ViewBase viewBase, IconInstance iconInstance)
        /// <summary>
        /// Invoke an event that an icon of a node instance should be hidden for a particular view.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the icon was changed.</param>
        /// <param name="viewBase">The view for which the icon was changed.</param>
        /// <param name="iconInstance">The icon instance.</param>
        internal static void InvokeIconHidden(NodeInstance nodeInstance, ViewBase viewBase, IconInstance iconInstance)
        {
            foreach (IIconHandler handler in GetContentHandlers<IIconHandler>(nodeInstance))
                handler.IconHidden(nodeInstance, viewBase, iconInstance);
        }
        #endregion Method: InvokeIconHidden(NodeInstance nodeInstance, ViewBase viewBase, IconInstance iconInstance)

        #region Method: InvokeModelShown(NodeInstance nodeInstance, ViewBase viewBase, ModelInstance modelInstance)
        /// <summary>
        /// Invoke an event that a model of a node instance should be shown for a particular view.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the model was changed.</param>
        /// <param name="viewBase">The view for which the model was changed.</param>
        /// <param name="modelInstance">The model instance.</param>
        internal static void InvokeModelShown(NodeInstance nodeInstance, ViewBase viewBase, ModelInstance modelInstance)
        {
            foreach (IModelHandler handler in GetContentHandlers<IModelHandler>(nodeInstance))
                handler.ModelShown(nodeInstance, viewBase, modelInstance);
        }
        #endregion Method: InvokeModelShown(NodeInstance nodeInstance, ViewBase viewBase, ModelInstance modelInstance)

        #region Method: InvokeModelHidden(NodeInstance nodeInstance, ViewBase viewBase, ModelInstance modelInstance)
        /// <summary>
        /// Invoke an event that a model of a node instance should be hidden for a particular view.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the model was changed.</param>
        /// <param name="viewBase">The view for which the model was changed.</param>
        /// <param name="modelInstance">The model instance.</param>
        internal static void InvokeModelHidden(NodeInstance nodeInstance, ViewBase viewBase, ModelInstance modelInstance)
        {
            foreach (IModelHandler handler in GetContentHandlers<IModelHandler>(nodeInstance))
                handler.ModelHidden(nodeInstance, viewBase, modelInstance);
        }
        #endregion Method: InvokeModelHidden(NodeInstance nodeInstance, ViewBase viewBase, ModelInstance modelInstance)

        #region Method: InvokeParticlePropertiesShown(NodeInstance nodeInstance, ViewBase viewBase, ParticlePropertiesInstance particlePropertiesInstance)
        /// <summary>
        /// Invoke an event that particle properties of a node instance are added for a particular view.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the particle properties were changed.</param>
        /// <param name="viewBase">The view for which the particle properties were changed.</param>
        /// <param name="particlePropertiesInstance">The particle properties instance.</param>
        internal static void InvokeParticlePropertiesShown(NodeInstance nodeInstance, ViewBase viewBase, ParticlePropertiesInstance particlePropertiesInstance)
        {
            foreach (IParticlePropertiesHandler handler in GetContentHandlers<IParticlePropertiesHandler>(nodeInstance))
                handler.ParticlePropertiesShown(nodeInstance, viewBase, particlePropertiesInstance);
        }
        #endregion Method: InvokeParticlePropertiesShown(NodeInstance nodeInstance, ViewBase viewBase, ParticlePropertiesInstance particlePropertiesInstance)

        #region Method: InvokeParticlePropertiesHidden(NodeInstance nodeInstance, ViewBase viewBase, ParticlePropertiesInstance particlePropertiesInstance)
        /// <summary>
        /// Invoke an event that particle properties of a node instance are removed for a particular view.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the particle properties were changed.</param>
        /// <param name="viewBase">The view for which the particle properties were changed.</param>
        /// <param name="particlePropertiesInstance">The particle properties instance.</param>
        internal static void InvokeParticlePropertiesHidden(NodeInstance nodeInstance, ViewBase viewBase, ParticlePropertiesInstance particlePropertiesInstance)
        {
            foreach (IParticlePropertiesHandler handler in GetContentHandlers<IParticlePropertiesHandler>(nodeInstance))
                handler.ParticlePropertiesHidden(nodeInstance, viewBase, particlePropertiesInstance);
        }
        #endregion Method: InvokeParticlePropertiesHidden(NodeInstance nodeInstance, ViewBase viewBase, ParticlePropertiesInstance particlePropertiesInstance)

        #region Method: InvokeSpriteShown(NodeInstance nodeInstance, ViewBase viewBase, SpriteInstance spriteInstance)
        /// <summary>
        /// Invoke an event that a sprite of a node instance should be shown for a particular view.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the sprite was changed.</param>
        /// <param name="viewBase">The view for which the sprite was changed.</param>
        /// <param name="spriteInstance">The sprite instance.</param>
        internal static void InvokeSpriteShown(NodeInstance nodeInstance, ViewBase viewBase, SpriteInstance spriteInstance)
        {
            foreach (ISpriteHandler handler in GetContentHandlers<ISpriteHandler>(nodeInstance))
                handler.SpriteShown(nodeInstance, viewBase, spriteInstance);
        }
        #endregion Method: InvokeSpriteShown(NodeInstance nodeInstance, ViewBase viewBase, SpriteInstance spriteInstance)

        #region Method: InvokeSpriteHidden(NodeInstance nodeInstance, ViewBase viewBase, SpriteInstance spriteInstance)
        /// <summary>
        /// Invoke an event that a sprite of a node instance should be hidden for a particular view.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the sprite was changed.</param>
        /// <param name="viewBase">The view for which the sprite was changed.</param>
        /// <param name="spriteInstance">The sprite instance.</param>
        internal static void InvokeSpriteHidden(NodeInstance nodeInstance, ViewBase viewBase, SpriteInstance spriteInstance)
        {
            foreach (ISpriteHandler handler in GetContentHandlers<ISpriteHandler>(nodeInstance))
                handler.SpriteHidden(nodeInstance, viewBase, spriteInstance);
        }
        #endregion Method: InvokeSpriteHidden(NodeInstance nodeInstance, ViewBase viewBase, SpriteInstance spriteInstance)

        #region Method: InvokeCustomContentEnabled(NodeInstance nodeInstance, ViewBase viewBase, object customContent)
        /// <summary>
        /// Invoke an event that custom content of a node instance should be enabled for a particular view.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the custom content was changed.</param>
        /// <param name="viewBase">The view for which the custom content was changed.</param>
        /// <param name="customContent">The custom content.</param>
        internal static void InvokeCustomContentEnabled(NodeInstance nodeInstance, ViewBase viewBase, object customContent)
        {
            foreach (ICustomContentHandler handler in GetContentHandlers<ICustomContentHandler>(nodeInstance))
                handler.CustomContentEnabled(nodeInstance, viewBase, customContent);
        }
        #endregion Method: InvokeCustomContentEnabled(NodeInstance nodeInstance, ViewBase viewBase, object customContent)

        #region Method: InvokeCustomContentDisabled(NodeInstance nodeInstance, ViewBase viewBase, object customContent)
        /// <summary>
        /// Invoke an event that custom content of a node instance should be disabled for a particular view.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the custom content was changed.</param>
        /// <param name="viewBase">The view for which the custom content was changed.</param>
        /// <param name="customContent">The custom content.</param>
        internal static void InvokeCustomContentDisabled(NodeInstance nodeInstance, ViewBase viewBase, object customContent)
        {
            foreach (ICustomContentHandler handler in GetContentHandlers<ICustomContentHandler>(nodeInstance))
                handler.CustomContentDisabled(nodeInstance, viewBase, customContent);
        }
        #endregion Method: InvokeCustomContentDisabled(NodeInstance nodeInstance, ViewBase viewBase, object customContent)

        #region Method: InvokeAnimationStarted(NodeInstance nodeInstance, AnimationInstance animationInstance)
        /// <summary>
        /// Invoke an event that an animation of a node instance is started.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the animation was changed.</param>
        /// <param name="animationInstance">The animation instance.</param>
        internal static void InvokeAnimationStarted(NodeInstance nodeInstance, AnimationInstance animationInstance)
        {
            foreach (IAnimationHandler handler in GetContentHandlers<IAnimationHandler>(nodeInstance))
                handler.AnimationStarted(nodeInstance, animationInstance);
        }
        #endregion Method: InvokeAnimationStarted(NodeInstance nodeInstance, AnimationInstance animationInstance)

        #region Method: InvokeAnimationStopped(NodeInstance nodeInstance, AnimationInstance animationInstance)
        /// <summary>
        /// Invoke an event that an animation of a node instance is stopped.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the animation was changed.</param>
        /// <param name="animationInstance">The animation instance.</param>
        internal static void InvokeAnimationStopped(NodeInstance nodeInstance, AnimationInstance animationInstance)
        {
            foreach (IAnimationHandler handler in GetContentHandlers<IAnimationHandler>(nodeInstance))
                handler.AnimationStopped(nodeInstance, animationInstance);
        }
        #endregion Method: InvokeAnimationStopped(NodeInstance nodeInstance, AnimationInstance iconInstance)

        #region Method: InvokeAudioStarted(NodeInstance nodeInstance, AudioInstance audioInstance)
        /// <summary>
        /// Invoke an event that audio of a node instance is started.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the audio was changed.</param>
        /// <param name="audioInstance">The audio instance.</param>
        internal static void InvokeAudioStarted(NodeInstance nodeInstance, AudioInstance audioInstance)
        {
            foreach (IAudioHandler handler in GetContentHandlers<IAudioHandler>(nodeInstance))
                handler.AudioStarted(nodeInstance, audioInstance);
        }
        #endregion Method: InvokeAudioStarted(NodeInstance nodeInstance, AudioInstance audioInstance)

        #region Method: InvokeAudioStopped(NodeInstance nodeInstance, AudioInstance audioInstance)
        /// <summary>
        /// Invoke an event that audio of a node instance is stopped.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the audio was changed.</param>
        /// <param name="audioInstance">The audio instance.</param>
        internal static void InvokeAudioStopped(NodeInstance nodeInstance, AudioInstance audioInstance)
        {
            foreach (IAudioHandler handler in GetContentHandlers<IAudioHandler>(nodeInstance))
                handler.AudioStopped(nodeInstance, audioInstance);
        }
        #endregion Method: InvokeAudioStopped(NodeInstance nodeInstance, AudioInstance iconInstance)

        #region Method: InvokeCinematicStarted(NodeInstance nodeInstance, CinematicInstance cinematicInstance)
        /// <summary>
        /// Invoke an event that a cinematic of a node instance is started.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the cinematic was changed.</param>
        /// <param name="cinematicInstance">The cinematic instance.</param>
        internal static void InvokeCinematicStarted(NodeInstance nodeInstance, CinematicInstance cinematicInstance)
        {
            foreach (ICinematicHandler handler in GetContentHandlers<ICinematicHandler>(nodeInstance))
                handler.CinematicStarted(nodeInstance, cinematicInstance);
        }
        #endregion Method: InvokeCinematicStarted(NodeInstance nodeInstance, CinematicInstance cinematicInstance)

        #region Method: InvokeCinematicStopped(NodeInstance nodeInstance, CinematicInstance cinematicInstance)
        /// <summary>
        /// Invoke an event that a cinematic of a node instance is stopped.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the cinematic was changed.</param>
        /// <param name="cinematicInstance">The cinematic instance.</param>
        internal static void InvokeCinematicStopped(NodeInstance nodeInstance, CinematicInstance cinematicInstance)
        {
            foreach (ICinematicHandler handler in GetContentHandlers<ICinematicHandler>(nodeInstance))
                handler.CinematicStopped(nodeInstance, cinematicInstance);
        }
        #endregion Method: InvokeCinematicStopped(NodeInstance nodeInstance, CinematicInstance iconInstance)

        #region Method: InvokeParticleEmitterStarted(NodeInstance nodeInstance, ParticleEmitterInstance particleEmitterInstance)
        /// <summary>
        /// Invoke an event that a particle emitter of a node instance is started.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the particle emitter was changed.</param>
        /// <param name="particleEmitterInstance">The particle emitter instance.</param>
        internal static void InvokeParticleEmitterStarted(NodeInstance nodeInstance, ParticleEmitterInstance particleEmitterInstance)
        {
            foreach (IParticleEmitterHandler handler in GetContentHandlers<IParticleEmitterHandler>(nodeInstance))
                handler.ParticleEmitterStarted(nodeInstance, particleEmitterInstance);
        }
        #endregion Method: InvokeParticleEmitterStarted(NodeInstance nodeInstance, ParticleEmitterInstance particleEmitterInstance)

        #region Method: InvokeParticleEmitterStopped(NodeInstance nodeInstance, ParticleEmitterInstance particleEmitterInstance)
        /// <summary>
        /// Invoke an event that a particle emitter of a node instance is stopped.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the particle emitter was changed.</param>
        /// <param name="particleEmitterInstance">The particle emitter instance.</param>
        internal static void InvokeParticleEmitterStopped(NodeInstance nodeInstance, ParticleEmitterInstance particleEmitterInstance)
        {
            foreach (IParticleEmitterHandler handler in GetContentHandlers<IParticleEmitterHandler>(nodeInstance))
                handler.ParticleEmitterStopped(nodeInstance, particleEmitterInstance);
        }
        #endregion Method: InvokeParticleEmitterStopped(NodeInstance nodeInstance, ParticleEmitterInstance iconInstance)

        #region Method: InvokeScriptStarted(NodeInstance nodeInstance, ScriptInstance scriptInstance)
        /// <summary>
        /// Invoke an event that a script of a node instance is started.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the script was changed.</param>
        /// <param name="scriptInstance">The script instance.</param>
        internal static void InvokeScriptStarted(NodeInstance nodeInstance, ScriptInstance scriptInstance)
        {
            foreach (IScriptHandler handler in GetContentHandlers<IScriptHandler>(nodeInstance))
                handler.ScriptStarted(nodeInstance, scriptInstance);
        }
        #endregion Method: InvokeScriptStarted(NodeInstance nodeInstance, ScriptInstance scriptInstance)

        #region Method: InvokeScriptStopped(NodeInstance nodeInstance, ScriptInstance scriptInstance)
        /// <summary>
        /// Invoke an event that a script of a node instance is stopped.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the script was changed.</param>
        /// <param name="scriptInstance">The script instance.</param>
        internal static void InvokeScriptStopped(NodeInstance nodeInstance, ScriptInstance scriptInstance)
        {
            foreach (IScriptHandler handler in GetContentHandlers<IScriptHandler>(nodeInstance))
                handler.ScriptStopped(nodeInstance, scriptInstance);
        }
        #endregion Method: InvokeScriptStopped(NodeInstance nodeInstance, ScriptInstance iconInstance)

        #endregion Method Group: Handlers

    }
    #endregion Class: ContentManager
		
}