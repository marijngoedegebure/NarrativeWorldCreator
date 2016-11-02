/**************************************************************************
 * 
 * ContentWrapper.cs
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
using System.Collections.ObjectModel;
using Common;
using GameSemantics.Components;
using GameSemantics.Utilities;
using GameSemanticsEngine.Components;
using GameSemanticsEngine.GameContent;
using Semantics.Components;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Components;
using SemanticsEngine.Entities;
using SemanticsEngine.Tools;
using SemanticsEngine.Interfaces;

namespace GameSemanticsEngine.Tools
{

    #region Class: ContentWrapper
    /// <summary>
    /// A wrapper for content.
    /// </summary>
    public class ContentWrapper : PropertyChangedComponent
    {

        #region Properties and Fields

        #region Property: NodeInstance
        /// <summary>
        /// The node instance for which this is a content wrapper.
        /// </summary>
        private NodeInstance nodeInstance = null;

        /// <summary>
        /// The node instance for which this is a content wrapper.
        /// </summary>
        public NodeInstance NodeInstance
        {
            get
            {
                return nodeInstance;
            }
            set
            {
                nodeInstance = value;
            }
        }
        #endregion Property: NodeInstance

        #region Property: AbstractGameNodeBase
        /// <summary>
        /// The abstract game node base of which the base content should be retrieved.
        /// </summary>
        private AbstractGameNodeBase abstractGameNodeBase = null;

        /// <summary>
        /// The abstract game node base of which the base content should be retrieved.
        /// </summary>
        internal AbstractGameNodeBase AbstractGameNodeBase
        {
            get
            {
                return abstractGameNodeBase;
            }
            set
            {
                abstractGameNodeBase = value;
            }
        }
        #endregion Property: AbstractGameNodeBase

        #region Property: AllContent
        /// <summary>
        /// Gets all the available content.
        /// </summary>
        public ReadOnlyCollection<ContentInstance> AllContent
        {
            get
            {
                List<ContentInstance> allContent = new List<ContentInstance>();
                foreach (StaticContentInstance staticContentInstance in this.StaticContent)
                    allContent.Add(staticContentInstance);
                foreach (DynamicContentInstance dynamicContentInstance in this.DynamicContent)
                    allContent.Add(dynamicContentInstance);
                foreach (CustomContentInstance customContentInstance in this.CustomContent)
                    allContent.Add(customContentInstance);
                return allContent.AsReadOnly();
            }
        }
        #endregion Property: AllContent
        
        #region Property: StaticContent
        /// <summary>
        /// All static content.
        /// </summary>
        private StaticContentInstance[] staticContent = null;

        /// <summary>
        /// Gets all static content.
        /// </summary>
        public ReadOnlyCollection<StaticContentInstance> StaticContent
        {
            get
            {
                if (staticContent != null)
                    return new ReadOnlyCollection<StaticContentInstance>(staticContent);
                return new List<StaticContentInstance>(0).AsReadOnly();
            }
        }
        #endregion Property: StaticContent

        #region Property: DynamicContent
        /// <summary>
        /// All dynamic content.
        /// </summary>
        private DynamicContentInstance[] dynamicContent = null;

        /// <summary>
        /// Gets all dynamic content.
        /// </summary>
        public ReadOnlyCollection<DynamicContentInstance> DynamicContent
        {
            get
            {
                if (dynamicContent != null)
                    return new ReadOnlyCollection<DynamicContentInstance>(dynamicContent);
                return new List<DynamicContentInstance>(0).AsReadOnly();
            }
        }
        #endregion Property: DynamicContent

        #region Property: CustomContent
        /// <summary>
        /// All custom content.
        /// </summary>
        private CustomContentInstance[] customContent = null;

        /// <summary>
        /// Gets all custom content.
        /// </summary>
        public ReadOnlyCollection<CustomContentInstance> CustomContent
        {
            get
            {
                if (customContent != null)
                    return new ReadOnlyCollection<CustomContentInstance>(customContent);
                return new List<CustomContentInstance>(0).AsReadOnly();
            }
        }
        #endregion Property: CustomContent

        #region Property: CurrentContent
        /// <summary>
        /// The current content.
        /// </summary>
        private List<ContentInstance> currentContent = new List<ContentInstance>();

        /// <summary>
        /// Gets the current content.
        /// </summary>
        public ReadOnlyCollection<ContentInstance> CurrentContent
        {
            get
            {
                return currentContent.AsReadOnly();
            }
        }
        #endregion Property: CurrentContent

        #region Field: customContentPerViewAndContextType
        /// <summary>
        /// The custom content per view and context type.
        /// </summary>
        private Dictionary<CustomContentInstance, Tuple<ViewBase, ContextTypeBase>> customContentPerViewAndContextType = new Dictionary<CustomContentInstance,Tuple<ViewBase,ContextTypeBase>>();
        #endregion Field: customContentPerViewAndContextType

        #region Field: customContentPerAction
        /// <summary>
        /// The custom content per action.
        /// </summary>
        private Dictionary<CustomContentInstance, List<Tuple<StartStop, EventStateExtended, ActionBase, EntityInstance>>> customContentPerAction = new Dictionary<CustomContentInstance, List<Tuple<StartStop, EventStateExtended, ActionBase, EntityInstance>>>();
        #endregion Field: customContentPerAction

        #region Property: AnimationsLocked
        /// <summary>
        /// Indicates whether the animations are locked, meaning that they cannot be changed anymore.
        /// </summary>
        private bool animationsLocked = false;

        /// <summary>
        /// Gets the value that indicates whether the animations are locked, meaning that they cannot be changed anymore.
        /// </summary>
        public bool AnimationsLocked
        {
            get
            {
                return animationsLocked;
            }
            set
            {
                animationsLocked = value;
                NotifyPropertyChanged("AnimationsLocked");
            }
        }
        #endregion Property: AnimationsLocked

        #region Property: AudioLocked
        /// <summary>
        /// Indicates whether the audio are locked, meaning that they cannot be changed anymore.
        /// </summary>
        private bool audioLocked = false;

        /// <summary>
        /// Gets the value that indicates whether the audio are locked, meaning that they cannot be changed anymore.
        /// </summary>
        public bool AudioLocked
        {
            get
            {
                return audioLocked;
            }
            set
            {
                audioLocked = value;
                NotifyPropertyChanged("AudioLocked");
            }
        }
        #endregion Property: AudioLocked

        #region Property: CinematicsLocked
        /// <summary>
        /// Indicates whether the cinematics are locked, meaning that they cannot be changed anymore.
        /// </summary>
        private bool cinematicsLocked = false;

        /// <summary>
        /// Gets the value that indicates whether the cinematics are locked, meaning that they cannot be changed anymore.
        /// </summary>
        public bool CinematicsLocked
        {
            get
            {
                return cinematicsLocked;
            }
            set
            {
                cinematicsLocked = value;
                NotifyPropertyChanged("CinematicsLocked");
            }
        }
        #endregion Property: CinematicsLocked

        #region Property: GameMaterialsLocked
        /// <summary>
        /// Indicates whether the game materials are locked, meaning that they cannot be changed anymore.
        /// </summary>
        private bool gameMaterialsLocked = false;

        /// <summary>
        /// Gets the value that indicates whether the game materials are locked, meaning that they cannot be changed anymore.
        /// </summary>
        public bool GameMaterialsLocked
        {
            get
            {
                return gameMaterialsLocked;
            }
            set
            {
                gameMaterialsLocked = value;
                NotifyPropertyChanged("GameMaterialsLocked");
            }
        }
        #endregion Property: GameMaterialsLocked

        #region Property: IconsLocked
        /// <summary>
        /// Indicates whether the icons are locked, meaning that they cannot be changed anymore.
        /// </summary>
        private bool iconsLocked = false;

        /// <summary>
        /// Gets the value that indicates whether the icons are locked, meaning that they cannot be changed anymore.
        /// </summary>
        public bool IconsLocked
        {
            get
            {
                return iconsLocked;
            }
            set
            {
                iconsLocked = value;
                NotifyPropertyChanged("IconsLocked");
            }
        }
        #endregion Property: IconsLocked

        #region Property: ModelsLocked
        /// <summary>
        /// Indicates whether the models are locked, meaning that they cannot be changed anymore.
        /// </summary>
        private bool modelsLocked = false;

        /// <summary>
        /// Gets the value that indicates whether the models are locked, meaning that they cannot be changed anymore.
        /// </summary>
        public bool ModelsLocked
        {
            get
            {
                return modelsLocked;
            }
            set
            {
                modelsLocked = value;
                NotifyPropertyChanged("ModelsLocked");
            }
        }
        #endregion Property: ModelsLocked

        #region Property: ParticleEmittersLocked
        /// <summary>
        /// Indicates whether the particle emitters are locked, meaning that they cannot be changed anymore.
        /// </summary>
        private bool particleEmittersLocked = false;

        /// <summary>
        /// Gets the value that indicates whether the particle emitters are locked, meaning that they cannot be changed anymore.
        /// </summary>
        public bool ParticleEmittersLocked
        {
            get
            {
                return particleEmittersLocked;
            }
            set
            {
                particleEmittersLocked = value;
                NotifyPropertyChanged("ParticleEmittersLocked");
            }
        }
        #endregion Property: ParticleEmittersLocked

        #region Property: ParticlePropertiesLocked
        /// <summary>
        /// Indicates whether the particle properties are locked, meaning that they cannot be changed anymore.
        /// </summary>
        private bool particlePropertiesLocked = false;

        /// <summary>
        /// Gets the value that indicates whether the particle properties are locked, meaning that they cannot be changed anymore.
        /// </summary>
        public bool ParticlePropertiesLocked
        {
            get
            {
                return particlePropertiesLocked;
            }
            set
            {
                particlePropertiesLocked = value;
                NotifyPropertyChanged("ParticlePropertiesLocked");
            }
        }
        #endregion Property: ParticlePropertiesLocked

        #region Property: ScriptsLocked
        /// <summary>
        /// Indicates whether the scripts are locked, meaning that they cannot be changed anymore.
        /// </summary>
        private bool scriptsLocked = false;

        /// <summary>
        /// Gets the value that indicates whether the scripts are locked, meaning that they cannot be changed anymore.
        /// </summary>
        public bool ScriptsLocked
        {
            get
            {
                return scriptsLocked;
            }
            set
            {
                scriptsLocked = value;
                NotifyPropertyChanged("ScriptsLocked");
            }
        }
        #endregion Property: ScriptsLocked

        #region Property: SpritesLocked
        /// <summary>
        /// Indicates whether the sprites are locked, meaning that they cannot be changed anymore.
        /// </summary>
        private bool spritesLocked = false;

        /// <summary>
        /// Gets the value that indicates whether the sprites are locked, meaning that they cannot be changed anymore.
        /// </summary>
        public bool SpritesLocked
        {
            get
            {
                return spritesLocked;
            }
            set
            {
                spritesLocked = value;
                NotifyPropertyChanged("SpritesLocked");
            }
        }
        #endregion Property: SpritesLocked
		
        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ContentWrapper(NodeInstance nodeInstance)
        /// <summary>
        /// Creates a new content wrapper for the given node instance.
        /// </summary>
        /// <param name="nodeInstance">The node instance to assign to the content wrapper.</param>
        public ContentWrapper(NodeInstance nodeInstance)
        {
            this.nodeInstance = nodeInstance;
        }
        #endregion Constructor: ContentWrapper(NodeInstance nodeInstance)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddContent(ContentInstance content)
        /// <summary>
        /// Adds content.
        /// </summary>
        /// <param name="content">The content to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        internal Message AddContent(ContentInstance content)
        {
            if (content != null)
            {
                // If the content is already available, there is no use to add it
                if (this.AllContent.Contains(content))
                    return Message.RelationExistsAlready;

                // Try to add the content
                if (content is StaticContentInstance)
                {
                    // Don't add content when it is locked
                    if ((content is GameMaterialInstance && this.GameMaterialsLocked) ||
                        (content is IconInstance && this.IconsLocked) ||
                        (content is ModelInstance && this.ModelsLocked) ||
                        (content is ParticlePropertiesInstance && this.ParticlePropertiesLocked) ||
                        (content is SpriteInstance && this.SpritesLocked))
                        return Message.RelationFail;

                    Utils.AddToArray<StaticContentInstance>(ref this.staticContent, (StaticContentInstance)content);
                    NotifyPropertyChanged("StaticContent");
                }
                else if (content is DynamicContentInstance)
                {
                    // Don't add content when it is locked
                    if ((content is AnimationInstance && this.AnimationsLocked) ||
                        (content is AudioInstance && this.AudioLocked) ||
                        (content is CinematicInstance && this.CinematicsLocked) ||
                        (content is ParticleEmitterInstance && this.ParticleEmittersLocked) ||
                        (content is ScriptInstance && this.ScriptsLocked))
                        return Message.RelationFail;

                    Utils.AddToArray<DynamicContentInstance>(ref this.dynamicContent, (DynamicContentInstance)content);
                    NotifyPropertyChanged("DynamicContent");
                }
                else if (content is CustomContentInstance)
                {
                    Utils.AddToArray<CustomContentInstance>(ref this.customContent, (CustomContentInstance)content);
                    NotifyPropertyChanged("CustomContent");
                }
                NotifyPropertyChanged("Content");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddContent(ContentInstance content)

        #region Method: RemoveContent(ContentInstance content)
        /// <summary>
        /// Removes content.
        /// </summary>
        /// <param name="content">The content to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        internal Message RemoveContent(ContentInstance content)
        {
            if (content != null)
            {
                if (this.AllContent.Contains(content))
                {
                    // Remove the content
                    if (content is StaticContentInstance)
                    {
                        Utils.RemoveFromArray<StaticContentInstance>(ref this.staticContent, (StaticContentInstance)content);
                        NotifyPropertyChanged("StaticContent");
                    }
                    else if (content is DynamicContentInstance)
                    {
                        Utils.RemoveFromArray<DynamicContentInstance>(ref this.dynamicContent, (DynamicContentInstance)content);
                        NotifyPropertyChanged("DynamicContent");
                    }
                    else if (content is CustomContentInstance)
                    {
                        CustomContentInstance customContentInstance = (CustomContentInstance)content;

                        Utils.RemoveFromArray<CustomContentInstance>(ref this.customContent, customContentInstance);
                        NotifyPropertyChanged("CustomContent");

                        if (this.customContentPerViewAndContextType.ContainsKey(customContentInstance))
                            this.customContentPerViewAndContextType.Remove(customContentInstance);
                        if (this.customContentPerAction.ContainsKey(customContentInstance))
                            this.customContentPerAction.Remove(customContentInstance);
                    }

                    NotifyPropertyChanged("Content");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveContent(ContentInstance content)

        #region Method: AddCustomContent(CustomContentInstance customContentInstance, ViewBase viewBase, ContextTypeBase contextTypeBase)
        /// <summary>
        /// Add custom content for a particular view and context type.
        /// </summary>
        /// <param name="customContentInstance">The custom content instance to add.</param>
        /// <param name="viewBase">The view base; can be null.</param>
        /// <param name="contextTypeBase">The context type base; can be null.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddCustomContent(CustomContentInstance customContentInstance, ViewBase viewBase, ContextTypeBase contextTypeBase)
        {
            if (customContentInstance != null && !this.customContentPerViewAndContextType.ContainsKey(customContentInstance))
            {
                this.customContentPerViewAndContextType.Add(customContentInstance, new Tuple<ViewBase, ContextTypeBase>(viewBase, contextTypeBase));
                AddContent(customContentInstance);

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddCustomContent(CustomContentInstance customContentInstance, ViewBase viewBase, ContextTypeBase contextTypeBase)

        #region Method: AddCustomContent(CustomContentInstance customContentInstance, StartStop startStop, EventState eventState, ActionBase action, EntityInstance target)
        /// <summary>
        /// Add custom content when the instance of this wrapper starts/is executing/stops an action on a possible target.
        /// </summary>
        /// <param name="customContentInstance">The custom content instance to add.</param>
        /// <param name="startStop">Indicates whether the custom content should be started/enabled or stopped/disabled when the action is starts/is executing/stops.</param>
        /// <param name="eventState">The event state to indicate whether the action started, is being executed, or stopped.</param>
        /// <param name="action">The action.</param>
        /// <param name="target">The (optional) target.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddCustomContent(CustomContentInstance customContentInstance, StartStop startStop, EventStateExtended eventState, ActionBase action, EntityInstance target)
        {
            if (customContentInstance != null && action != null)
            {
                List<Tuple<StartStop, EventStateExtended, ActionBase, EntityInstance>> list = null;
                if (!this.customContentPerAction.TryGetValue(customContentInstance, out list))
                {
                    list = new List<Tuple<StartStop, EventStateExtended, ActionBase, EntityInstance>>();
                    list.Add(new Tuple<StartStop, EventStateExtended, ActionBase, EntityInstance>(startStop, eventState, action, target));
                    this.customContentPerAction.Add(customContentInstance, list);
                    AddContent(customContentInstance);
                }
                else
                {
                    bool contains = false;
                    foreach (Tuple<StartStop, EventStateExtended, ActionBase, EntityInstance> tuple in list)
                    {
                        if (startStop == tuple.Item1 && eventState == tuple.Item2 && action == tuple.Item3 && ((target == null && tuple.Item4 == null) || (target != null && target.Equals(tuple.Item4))))
                        {
                            contains = true;
                            break;
                        }
                    }
                    if (!contains)
                        list.Add(new Tuple<StartStop, EventStateExtended, ActionBase, EntityInstance>(startStop, eventState, action, target));
                }

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddCustomContent(CustomContentInstance customContentInstance, StartStop startStop, EventState eventState, ActionBase action, EntityInstance target)

        #region Method: RemoveCustomContent(CustomContentInstance customContentInstance)
        /// <summary>
        /// Remove custom content.
        /// </summary>
        /// <param name="customContentInstance">The custom content instance to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveCustomContent(CustomContentInstance customContentInstance)
        {
            return RemoveContent(customContentInstance);
        }
        #endregion Method: RemoveCustomContent(CustomContentInstance customContentInstance)

        #endregion Method Group: Add/Remove

        #region Method Group: Content checking

        #region Method: CheckForShownContent(ContextTypeBase contextTypeBase)
        /// <summary>
        /// Check for content to be shown when the abstract game node instance gets a context type.
        /// </summary>
        /// <param name="contextTypeBase">The context type; can be null.</param>
        internal void CheckForShownContent(ContextTypeBase contextTypeBase)
        {
            // Get all content per view for the context type
            foreach (KeyValuePair<ViewBase, List<ContentInstance>> viewContent in GetContentOfContextTypePerView(contextTypeBase))
            {
                ViewBase viewBase = viewContent.Key;

                if (viewBase is NullViewBase)
                    viewBase = null;

                // Notify of showing the content
                foreach (ContentInstance contentInstance in viewContent.Value)
                {
                    if (contentInstance is StaticContentInstance)
                        NotifyOfShownContent((StaticContentInstance)contentInstance, viewBase);
                    else if (contentInstance is CustomContentInstance)
                        NotifyOfEnabledContent((CustomContentInstance)contentInstance, viewBase);
                }
            }
        }
        #endregion Method: CheckForShownContent(ContextTypeBase contextTypeBase)

        #region Method: CheckForHiddenContent(ContextTypeBase contextTypeBase)
        /// <summary>
        /// Check for content to be hidden when the abstract game node instance loses a context type.
        /// </summary>
        /// <param name="contextTypeBase">The context type; can be null.</param>
        internal void CheckForHiddenContent(ContextTypeBase contextTypeBase)
        {
            // Get all content per view for the context type
            foreach (KeyValuePair<ViewBase, List<ContentInstance>> viewContent in GetContentOfContextTypePerView(contextTypeBase))
            {
                ViewBase viewBase = viewContent.Key;

                if (viewBase is NullViewBase)
                    viewBase = null;

                // Notify of hiding the content
                foreach (ContentInstance contentInstance in viewContent.Value)
                {
                    if (contentInstance is StaticContentInstance)
                        NotifyOfHiddenContent((StaticContentInstance)contentInstance, viewBase);
                    else if (contentInstance is CustomContentInstance)
                        NotifyOfDisabledContent((CustomContentInstance)contentInstance, viewBase);
                }
            }
        }
        #endregion Method: CheckForHiddenContent(ContextTypeBase contextTypeBase)

        #region Method: CheckForActionContent(EventStateExtended eventState, ActionBase action, EntityInstance target)
        /// <summary>
        /// Checks for started or stopped content when an action starts/is being executed/stops (on a target).
        /// </summary>
        /// <param name="eventState">The event state to indicate whether the action started, is being executed, or stopped.</param>
        /// <param name="action">The action.</param>
        /// <param name="target">The (optional) target.</param>
        internal void CheckForActionContent(EventStateExtended eventState, ActionBase action, EntityInstance target)
        {
            if (action != null)
            {
                // Check for all event content
                foreach (EventContentBase eventContentBase in this.AbstractGameNodeBase.EventContent)
                {
                    // Look for event with the correct state
                    if (eventContentBase.EventState == eventState)
                    {
                        EventBase even = eventContentBase.Event;
                        if (even != null)
                        {
                            // Check whether the action and target correspond
                            if (action.Equals(even.Action) &&
                                ((target == null && even.Target == null) || (target != null && target.EntityBase != null && target.EntityBase.Equals(even.Target))))
                            {
                                // Get the correct content
                                if (eventContentBase.Content != null)
                                {
                                    foreach (DynamicContentInstance dynamicContentInstance in this.DynamicContent)
                                    {
                                        if (eventContentBase.Content.Equals(dynamicContentInstance.ContentValuedBase))
                                        {
                                            // Notify of started or stopped content
                                            switch (eventContentBase.ContentBehavior)
                                            {
                                                case StartStop.Start:
                                                    NotifyOfStartedContent(dynamicContentInstance);
                                                    break;
                                                case StartStop.Stop:
                                                    NotifyOfStoppedContent(dynamicContentInstance);
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Check the custom content
                foreach (KeyValuePair<CustomContentInstance, List<Tuple<StartStop, EventStateExtended, ActionBase, EntityInstance>>> pair in this.customContentPerAction)
                {
                    foreach (Tuple<StartStop, EventStateExtended, ActionBase, EntityInstance> tuple in pair.Value)
                    {
                        if (eventState.Equals(tuple.Item2) && action.Equals(tuple.Item3) && ((target == null && tuple.Item4 == null) || (target != null && target.Equals(tuple.Item4))))
                        {
                            // Notify of enabled or disabled content
                            switch (tuple.Item1)
                            {
                                case StartStop.Start:
                                    NotifyOfEnabledContent(pair.Key, null);
                                    break;
                                case StartStop.Stop:
                                    NotifyOfDisabledContent(pair.Key, null);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
        }
        #endregion Method: CheckForActionContent(EventStateExtended eventState, ActionBase action, EntityInstance target)

        #region Method: NotifyOfShownContent(StaticContentInstance staticContentInstance, ViewBase viewBase)
        /// <summary>
        /// Notify of showing of the given content instance.
        /// </summary>
        /// <param name="staticContentInstance">The content instance to show.</param>
        /// <param name="viewBase">The view; can be null.</param>
        internal void NotifyOfShownContent(StaticContentInstance staticContentInstance, ViewBase viewBase)
        {
            if (staticContentInstance != null)
            {
                if (!this.currentContent.Contains(staticContentInstance))
                {
                    this.currentContent.Add(staticContentInstance);
                    NotifyPropertyChanged("CurrentContent");
                }

                // Invoke the correct event
                GameMaterialInstance gameMaterialInstance = staticContentInstance as GameMaterialInstance;
                if (gameMaterialInstance != null)
                {
                    ContentManager.InvokeGameMaterialShown(this.NodeInstance, viewBase, gameMaterialInstance);
                    return;
                }
                IconInstance iconInstance = staticContentInstance as IconInstance;
                if (iconInstance != null)
                {
                    ContentManager.InvokeIconShown(this.NodeInstance, viewBase, iconInstance);
                    return;
                }
                ModelInstance modelInstance = staticContentInstance as ModelInstance;
                if (modelInstance != null)
                {
                    ContentManager.InvokeModelShown(this.NodeInstance, viewBase, modelInstance);
                    return;
                }
                ParticlePropertiesInstance particlePropertiesInstance = staticContentInstance as ParticlePropertiesInstance;
                if (particlePropertiesInstance != null)
                {
                    ContentManager.InvokeParticlePropertiesShown(this.NodeInstance, viewBase, particlePropertiesInstance);
                    return;
                }
                SpriteInstance spriteInstance = staticContentInstance as SpriteInstance;
                if (spriteInstance != null)
                {
                    ContentManager.InvokeSpriteShown(this.NodeInstance, viewBase, spriteInstance);
                    return;
                }
            }
        }
        #endregion Method: NotifyOfShownContent(StaticContentInstance staticContentInstance, ViewBase viewBase)

        #region Method: NotifyOfHiddenContent(StaticContentInstance staticContentInstance, ViewBase viewBase)
        /// <summary>
        /// Notify of hiding of the given content instance.
        /// </summary>
        /// <param name="staticContentInstance">The content instance to hide.</param>
        /// <param name="viewBase">The view; can be null.</param>
        internal void NotifyOfHiddenContent(StaticContentInstance staticContentInstance, ViewBase viewBase)
        {
            if (staticContentInstance != null)
            {
                if (this.currentContent.Contains(staticContentInstance))
                {
                    this.currentContent.Remove(staticContentInstance);
                    NotifyPropertyChanged("CurrentContent");
                }

                // Invoke the correct event
                GameMaterialInstance gameMaterialInstance = staticContentInstance as GameMaterialInstance;
                if (gameMaterialInstance != null)
                {
                    ContentManager.InvokeGameMaterialHidden(this.NodeInstance, viewBase, gameMaterialInstance);
                    return;
                }
                IconInstance iconInstance = staticContentInstance as IconInstance;
                if (iconInstance != null)
                {
                    ContentManager.InvokeIconHidden(this.NodeInstance, viewBase, iconInstance);
                    return;
                }
                ModelInstance modelInstance = staticContentInstance as ModelInstance;
                if (modelInstance != null)
                {
                    ContentManager.InvokeModelHidden(this.NodeInstance, viewBase, modelInstance);
                    return;
                }
                ParticlePropertiesInstance particlePropertiesInstance = staticContentInstance as ParticlePropertiesInstance;
                if (particlePropertiesInstance != null)
                {
                    ContentManager.InvokeParticlePropertiesHidden(this.NodeInstance, viewBase, particlePropertiesInstance);
                    return;
                }
                SpriteInstance spriteInstance = staticContentInstance as SpriteInstance;
                if (spriteInstance != null)
                {
                    ContentManager.InvokeSpriteHidden(this.NodeInstance, viewBase, spriteInstance);
                    return;
                }
            }
        }
        #endregion Method: NotifyOfHiddenContent(StaticContentInstance staticContentInstance, ViewBase viewBase)

        #region Method: NotifyOfEnabledContent(CustomContentInstance customContentInstance, ViewBase viewBase)
        /// <summary>
        /// Notify of enabling of the given custom content instance.
        /// </summary>
        /// <param name="customContentInstance">The custom content instance to enable.</param>
        /// <param name="viewBase">The view; can be null.</param>
        internal void NotifyOfEnabledContent(CustomContentInstance customContentInstance, ViewBase viewBase)
        {
            if (customContentInstance != null)
            {
                if (!this.currentContent.Contains(customContentInstance))
                {
                    this.currentContent.Add(customContentInstance);
                    NotifyPropertyChanged("CurrentContent");
                }

                ContentManager.InvokeCustomContentEnabled(this.NodeInstance, viewBase, customContentInstance);
            }
        }
        #endregion Method: NotifyOfEnabledContent(CustomContentInstance customContentInstance, ViewBase viewBase)

        #region Method: NotifyOfDisabledContent(CustomContentInstance customContentInstance, ViewBase viewBase)
        /// <summary>
        /// Notify of disabling of the given custom content instance.
        /// </summary>
        /// <param name="customContentInstance">The custom content instance to disable.</param>
        /// <param name="viewBase">The view; can be null.</param>
        internal void NotifyOfDisabledContent(CustomContentInstance customContentInstance, ViewBase viewBase)
        {
            if (customContentInstance != null)
            {
                if (this.currentContent.Contains(customContentInstance))
                {
                    this.currentContent.Remove(customContentInstance);
                    NotifyPropertyChanged("CurrentContent");
                }

                ContentManager.InvokeCustomContentDisabled(this.NodeInstance, viewBase, customContentInstance);
            }
        }
        #endregion Method: NotifyOfDisabledContent(CustomContentInstance customContentInstance, ViewBase viewBase)

        #region Method: NotifyOfStartedContent(DynamicContentInstance dynamicContentInstance)
        /// <summary>
        /// Notify of started content.
        /// </summary>
        /// <param name="dynamicContentInstance">The started content.</param>
        internal void NotifyOfStartedContent(DynamicContentInstance dynamicContentInstance)
        {
            if (dynamicContentInstance != null)
            {
                if (!this.currentContent.Contains(dynamicContentInstance))
                {
                    this.currentContent.Add(dynamicContentInstance);
                    NotifyPropertyChanged("CurrentContent");
                }

                // Invoke the correct event
                AudioInstance audioInstance = dynamicContentInstance as AudioInstance;
                if (audioInstance != null)
                {
                    ContentManager.InvokeAudioStarted(this.NodeInstance, audioInstance);
                    return;
                }
                AnimationInstance animationInstance = dynamicContentInstance as AnimationInstance;
                if (animationInstance != null)
                {
                    ContentManager.InvokeAnimationStarted(this.NodeInstance, animationInstance);
                    return;
                }
                CinematicInstance cinematicInstance = dynamicContentInstance as CinematicInstance;
                if (cinematicInstance != null)
                {
                    ContentManager.InvokeCinematicStarted(this.NodeInstance, cinematicInstance);
                    return;
                }
                ParticleEmitterInstance particleEmitterInstance = dynamicContentInstance as ParticleEmitterInstance;
                if (particleEmitterInstance != null)
                {
                    ContentManager.InvokeParticleEmitterStarted(this.NodeInstance, particleEmitterInstance);
                    return;
                }
                ScriptInstance scriptInstance = dynamicContentInstance as ScriptInstance;
                if (scriptInstance != null)
                {
                    ContentManager.InvokeScriptStarted(this.NodeInstance, scriptInstance);
                    return;
                }
            }
        }
        #endregion Method: NotifyOfStartedContent(DynamicContentInstance dynamicContentInstance)

        #region Method: NotifyOfStoppedContent(DynamicContentInstance dynamicContentInstance)
        /// <summary>
        /// Notify of stopped content.
        /// </summary>
        /// <param name="dynamicContentInstance">The stopped content.</param>
        internal void NotifyOfStoppedContent(DynamicContentInstance dynamicContentInstance)
        {
            if (dynamicContentInstance != null)
            {
                if (this.currentContent.Contains(dynamicContentInstance))
                {
                    this.currentContent.Remove(dynamicContentInstance);
                    NotifyPropertyChanged("CurrentContent");
                }

                // Invoke the correct event
                AudioInstance audioInstance = dynamicContentInstance as AudioInstance;
                if (audioInstance != null)
                {
                    ContentManager.InvokeAudioStopped(this.NodeInstance, audioInstance);
                    return;
                }
                AnimationInstance animationInstance = dynamicContentInstance as AnimationInstance;
                if (animationInstance != null)
                {
                    ContentManager.InvokeAnimationStopped(this.NodeInstance, animationInstance);
                    return;
                }
                CinematicInstance cinematicInstance = dynamicContentInstance as CinematicInstance;
                if (cinematicInstance != null)
                {
                    ContentManager.InvokeCinematicStopped(this.NodeInstance, cinematicInstance);
                    return;
                }
                ParticleEmitterInstance particleEmitterInstance = dynamicContentInstance as ParticleEmitterInstance;
                if (particleEmitterInstance != null)
                {
                    ContentManager.InvokeParticleEmitterStopped(this.NodeInstance, particleEmitterInstance);
                    return;
                }
                ScriptInstance scriptInstance = dynamicContentInstance as ScriptInstance;
                if (scriptInstance != null)
                {
                    ContentManager.InvokeScriptStopped(this.NodeInstance, scriptInstance);
                    return;
                }
            }
        }
        #endregion Method: NotifyOfStoppedContent(DynamicContentInstance dynamicContentInstance)

        #endregion Method Group: Content checking

        #region Method Group: Other

        #region Method: GetContentOfContextTypePerView(ContextTypeBase contextTypeBase)
        /// <summary>
        /// Get all the content per view that has been defined of the given context type.
        /// </summary>
        /// <param name="contextTypeBase">The context type; can be null.</param>
        /// <returns>All the content per view that has been defined for the context type.</returns>
        private Dictionary<ViewBase, List<ContentInstance>> GetContentOfContextTypePerView(ContextTypeBase contextTypeBase)
        {
            Dictionary<ViewBase, List<ContentInstance>> viewContentInstances = new Dictionary<ViewBase, List<ContentInstance>>();
            if (this.AbstractGameNodeBase != null)
            {
                foreach (ViewBase viewBase in this.AbstractGameNodeBase.GetViewsOfContextType(contextTypeBase))
                {
                    List<ContentInstance> contentInstances = new List<ContentInstance>();

                    // Add the correct static content
                    foreach (StaticContentValuedBase staticContentValuedBase in this.AbstractGameNodeBase.GetContentOfViewOfContextType(viewBase, contextTypeBase))
                    {
                        foreach (StaticContentInstance staticContentInstance in this.StaticContent)
                        {
                            if (staticContentValuedBase.Equals(staticContentInstance.ContentValuedBase))
                            {
                                contentInstances.Add(staticContentInstance);
                                break;
                            }
                        }
                    }

                    // Add the matching custom content
                    foreach (KeyValuePair<CustomContentInstance, Tuple<ViewBase, ContextTypeBase>> pair in this.customContentPerViewAndContextType)
                    {
                        if (viewBase.Equals(pair.Value.Item1) && contextTypeBase.Equals(pair.Value.Item2))
                            contentInstances.Add(pair.Key);
                    }

                    if (contentInstances.Count > 0)
                    {
                        if (viewBase == null)
                            viewContentInstances.Add(new NullViewBase(), contentInstances);
                        else
                            viewContentInstances.Add(viewBase, contentInstances);
                    }
                }
            }
            return viewContentInstances;
        }
        #endregion Method: GetContentOfContextTypePerView(ContextTypeBase contextTypeBase)

        #region Method: GetContent<T>()
        /// <summary>
        /// Get all content of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the content to get.</typeparam>
        /// <returns>All content of the type.</returns>
        public IEnumerable<T> GetContent<T>()
            where T : ContentInstance
        {
            Type type = typeof(T);
            foreach (ContentInstance contentInstance in this.AllContent)
            {
                if (type.Equals(contentInstance.GetType()))
                    yield return (T)contentInstance;
            }
        }
        #endregion Method: GetContent<T>()

        #region Method: GetCurrentContent<T>()
        /// <summary>
        /// Get all current content of the given type.
        /// </summary>
        /// <typeparam name="T">The type of the content to get.</typeparam>
        /// <returns>All current content of the type.</returns>
        public IEnumerable<T> GetCurrentContent<T>()
            where T : ContentInstance
        {
            Type type = typeof(T);
            foreach (ContentInstance contentInstance in this.CurrentContent)
            {
                if (type.Equals(contentInstance.GetType()))
                    yield return (T)contentInstance;
            }
        }
        #endregion Method: GetCurrentContent<T>()

        #region Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the change to the content instances.
        /// </summary>
        /// <param name="changeBase">The change to apply to the content instances.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        public bool Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (changeBase != null)
            {
                ContentChangeBase contentChangeBase = changeBase as ContentChangeBase;
                if (contentChangeBase != null)
                {
                    foreach (ContentInstance contentInstance in this.AllContent)
                        contentInstance.Apply(contentChangeBase, iVariableInstanceHolder);
                }
                return true;
            }
            return false;
        }
        #endregion Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: ContentWrapper

}