/**************************************************************************
 * 
 * IContentHandler.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using GameSemanticsEngine.Components;
using GameSemanticsEngine.GameContent;
using SemanticsEngine.Components;

namespace GameSemanticsEngine.Interfaces
{

    #region Interface: IContentHandler
    /// <summary>
    /// A handler for content.
    /// </summary>
    public interface IContentHandler
    {
    }
    #endregion Interface: IContentHandler

    #region Interface: IGameMaterialHandler
    /// <summary>
    /// A handler for game materials.
    /// </summary>
    public interface IGameMaterialHandler : IContentHandler
    {

        /// <summary>
        /// Indicates that a game material of a node instance should be shown for a particular view.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the game material was changed.</param>
        /// <param name="viewBase">The view for which the game materal was changed.</param>
        /// <param name="gameMaterialInstance">The game material instance.</param>
        void GameMaterialShown(NodeInstance nodeInstance, ViewBase viewBase, GameMaterialInstance gameMaterialInstance);

        /// <summary>
        /// Indicates that a game material of a node instance should be hidden for a particular view.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the game material was changed.</param>
        /// <param name="viewBase">The view for which the game materal was changed.</param>
        /// <param name="gameMaterialInstance">The game material instance.</param>
        void GameMaterialHidden(NodeInstance nodeInstance, ViewBase viewBase, GameMaterialInstance gameMaterialInstance);

    }
    #endregion Interface: IGameMaterialHandler

    #region Interface: IIconHandler
    /// <summary>
    /// A handler for icons.
    /// </summary>
    public interface IIconHandler : IContentHandler
    {

        /// <summary>
        /// Indicates that an icon of a node instance should be shown for a particular view.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the icon was changed.</param>
        /// <param name="viewBase">The view for which the icon was changed.</param>
        /// <param name="iconInstance">The icon instance.</param>
        void IconShown(NodeInstance nodeInstance, ViewBase viewBase, IconInstance iconInstance);

        /// <summary>
        /// Indicates that an icon of a node instance should be hidden for a particular view.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the icon was changed.</param>
        /// <param name="viewBase">The view for which the icon was changed.</param>
        /// <param name="iconInstance">The icon instance.</param>
        void IconHidden(NodeInstance nodeInstance, ViewBase viewBase, IconInstance iconInstance);

    }
    #endregion Interface: IIconHandler

    #region Interface: IModelHandler
    /// <summary>
    /// A handler for models.
    /// </summary>
    public interface IModelHandler : IContentHandler
    {

        /// <summary>
        /// Indicates that a model of a node instance should be shown for a particular view.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the model was changed.</param>
        /// <param name="viewBase">The view for which the model was changed.</param>
        /// <param name="modelInstance">The model instance.</param>
        void ModelShown(NodeInstance nodeInstance, ViewBase viewBase, ModelInstance modelInstance);

        /// <summary>
        /// Indicates that a model of a node instance should be hidden for a particular view.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the model was changed.</param>
        /// <param name="viewBase">The view for which the model was changed.</param>
        /// <param name="modelInstance">The model instance.</param>
        void ModelHidden(NodeInstance nodeInstance, ViewBase viewBase, ModelInstance modelInstance);

    }
    #endregion Interface: IModelHandler

    #region Interface: IParticlePropertiesHandler
    /// <summary>
    /// A handler for particle properties.
    /// </summary>
    public interface IParticlePropertiesHandler : IContentHandler
    {

        /// <summary>
        /// Indicates that particle properties of a node instance should be shown for a particular view.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the particle properties were changed.</param>
        /// <param name="viewBase">The view for which the particle properties were changed.</param>
        /// <param name="particlePropertiesInstance">The particle properties instance.</param>
        void ParticlePropertiesShown(NodeInstance nodeInstance, ViewBase viewBase, ParticlePropertiesInstance particlePropertiesInstance);

        /// <summary>
        /// Indicates that particle properties of a node instance should be hidden for a particular view.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the particle properties were changed.</param>
        /// <param name="viewBase">The view for which the particle properties were changed.</param>
        /// <param name="particlePropertiesInstance">The particle properties instance.</param>
        void ParticlePropertiesHidden(NodeInstance nodeInstance, ViewBase viewBase, ParticlePropertiesInstance particlePropertiesInstance);

    }
    #endregion Interface: IParticlePropertiesHandler

    #region Interface: ISpriteHandler
    /// <summary>
    /// A handler for sprites.
    /// </summary>
    public interface ISpriteHandler : IContentHandler
    {

        /// <summary>
        /// Indicates that a sprite of a node instance should be shown for a particular view.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the sprite was changed.</param>
        /// <param name="viewBase">The view for which the sprite was changed.</param>
        /// <param name="spriteInstance">The sprite instance.</param>
        void SpriteShown(NodeInstance nodeInstance, ViewBase viewBase, SpriteInstance spriteInstance);

        /// <summary>
        /// Indicates that a sprite of a node instance should be hidden for a particular view.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the sprite was changed.</param>
        /// <param name="viewBase">The view for which the sprite was changed.</param>
        /// <param name="spriteInstance">The sprite instance.</param>
        void SpriteHidden(NodeInstance nodeInstance, ViewBase viewBase, SpriteInstance spriteInstance);

    }
    #endregion Interface: ISpriteHandler

    #region Interface: ICustomContentHandler
    /// <summary>
    /// A handler for custom content.
    /// </summary>
    public interface ICustomContentHandler : IContentHandler
    {

        /// <summary>
        /// Indicates that custom content of a node instance should be enabled for a particular view.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the custom content was changed.</param>
        /// <param name="viewBase">The view for which the custom content was changed.</param>
        /// <param name="customContent">The custom content.</param>
        void CustomContentEnabled(NodeInstance nodeInstance, ViewBase viewBase, object customContent);

        /// <summary>
        /// Indicates that custom content of a node instance should be disabled for a particular view.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the custom content was changed.</param>
        /// <param name="viewBase">The view for which the custom content was changed.</param>
        /// <param name="customContent">The custom content.</param>
        void CustomContentDisabled(NodeInstance nodeInstance, ViewBase viewBase, object customContent);

    }
    #endregion Interface: ICustomContentHandler

    #region Interface: IAnimationHandler
    /// <summary>
    /// A handler for animations.
    /// </summary>
    public interface IAnimationHandler : IContentHandler
    {

        /// <summary>
        /// Indicates that an animation of a node instance is started.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the animation was changed.</param>
        /// <param name="animationInstance">The animation instance.</param>
        void AnimationStarted(NodeInstance nodeInstance, AnimationInstance animationInstance);

        /// <summary>
        /// Indicates that an animation of a node instance is stopped.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the animation was changed.</param>
        /// <param name="animationInstance">The animation instance.</param>
        void AnimationStopped(NodeInstance nodeInstance, AnimationInstance animationInstance);

    }
    #endregion Interface: IAnimationHandler

    #region Interface: IAudioHandler
    /// <summary>
    /// A handler for audio.
    /// </summary>
    public interface IAudioHandler : IContentHandler
    {

        /// <summary>
        /// Indicates that audio of a node instance is started.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the audio was changed.</param>
        /// <param name="audioInstance">The audio instance.</param>
        void AudioStarted(NodeInstance nodeInstance, AudioInstance audioInstance);

        /// <summary>
        /// Indicates that audio of a node instance is stopped.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the audio was changed.</param>
        /// <param name="audioInstance">The audio instance.</param>
        void AudioStopped(NodeInstance nodeInstance, AudioInstance audioInstance);

    }
    #endregion Interface: IAudioHandler

    #region Interface: ICinematicHandler
    /// <summary>
    /// A handler for cinematics.
    /// </summary>
    public interface ICinematicHandler : IContentHandler
    {

        /// <summary>
        /// Indicates that a cinematic of a node instance is started.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the cinematic was changed.</param>
        /// <param name="cinematicInstance">The cinematic instance.</param>
        void CinematicStarted(NodeInstance nodeInstance, CinematicInstance cinematicInstance);

        /// <summary>
        /// Indicates that a cinematic of a node instance is stopped.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the cinematic was changed.</param>
        /// <param name="cinematicInstance">The cinematic instance.</param>
        void CinematicStopped(NodeInstance nodeInstance, CinematicInstance cinematicInstance);

    }
    #endregion Interface: ICinematicHandler

    #region Interface: IParticleEmitterHandler
    /// <summary>
    /// A handler for particle emitters.
    /// </summary>
    public interface IParticleEmitterHandler : IContentHandler
    {

        /// <summary>
        /// Indicates that a particle emitter of a node instance is started.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the particle emitter was changed.</param>
        /// <param name="particleEmitterInstance">The particle emitter instance.</param>
        void ParticleEmitterStarted(NodeInstance nodeInstance, ParticleEmitterInstance particleEmitterInstance);

        /// <summary>
        /// Indicates that a particle emitter of a node instance is stopped.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the particle emitter was changed.</param>
        /// <param name="particleEmitterInstance">The particle emitter instance.</param>
        void ParticleEmitterStopped(NodeInstance nodeInstance, ParticleEmitterInstance particleEmitterInstance);

    }
    #endregion Interface: IParticleEmitterHandler

    #region Interface: IScriptHandler
    /// <summary>
    /// A handler for scripts.
    /// </summary>
    public interface IScriptHandler : IContentHandler
    {

        /// <summary>
        /// Indicates that a script of a node instance is started.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the script was changed.</param>
        /// <param name="scriptInstance">The script instance.</param>
        void ScriptStarted(NodeInstance nodeInstance, ScriptInstance scriptInstance);

        /// <summary>
        /// Indicates that a script of a node instance is stopped.
        /// </summary>
        /// <param name="nodeInstance">The node instance of which the script was changed.</param>
        /// <param name="scriptInstance">The script instance.</param>
        void ScriptStopped(NodeInstance nodeInstance, ScriptInstance scriptInstance);

    }
    #endregion Interface: IScriptHandler

}