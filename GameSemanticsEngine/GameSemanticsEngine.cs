/**************************************************************************
 * 
 * GameSemanticsEngine.cs
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
using GameSemantics.Components;
using GameSemantics.Data;
using GameSemantics.Utilities;
using GameSemanticsEngine.Components;
using GameSemanticsEngine.GameContent;
using GameSemanticsEngine.Tools;
using Semantics.Data;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Components;
using SemanticsEngine.Entities;
using SemanticsEngine.Worlds;

namespace GameSemanticsEngine
{

    #region Class: GameSemanticsEngine
    /// <summary>
    /// The Game Semantics Engine handles all content of instances.
    /// </summary>
    public class GameSemanticsEngine : SemanticsEngine.SemanticsEngine
    {

        #region Properties and Fields

        #region Property: Current
        /// <summary>
        /// Gets the current semantics engine. Make sure to call Initialize() first!
        /// </summary>
        public static new GameSemanticsEngine Current
        {
            get
            {
                return SemanticsEngine.SemanticsEngine.Current as GameSemanticsEngine;
            }
        }
        #endregion Property: Current

        #region Field: gameObjectBases
        /// <summary>
        /// All game object bases.
        /// </summary>
        private List<GameObjectBase> gameObjectBases = null;
        #endregion Field: gameObjectBases

        #region Field: gameNodeBases
        /// <summary>
        /// All game node bases.
        /// </summary>
        private List<GameNodeBase> gameNodeBases = null;
        #endregion Field: gameNodeBases

        #region Field: contextTypeAddedHandler
        /// <summary>
        /// A handler for an added context type.
        /// </summary>
        private EntityInstance.ContextTypeHandler contextTypeAddedHandler;
        #endregion Field: contextTypeAddedHandler

        #region Field: contextTypeRemovedHandler
        /// <summary>
        /// A handler for a removed context type.
        /// </summary>
        private EntityInstance.ContextTypeHandler contextTypeRemovedHandler;
        #endregion Field: contextTypeRemovedHandler

        #region Field: startedActionHandler
        /// <summary>
        /// A handler for a started action.
        /// </summary>
        private EntityInstance.ActionHandler startedActionHandler;
        #endregion Field: startedActionHandler

        #region Field: executedActionHandler
        /// <summary>
        /// A handler for an executed action.
        /// </summary>
        private EntityInstance.ActionHandler executedActionHandler;
        #endregion Field: executedActionHandler

        #region Field: stoppedActionHandler
        /// <summary>
        /// A handler for a stopped action.
        /// </summary>
        private EntityInstance.ActionHandler stoppedActionHandler;
        #endregion Field: stoppedActionHandler

        #region Field: entityInstanceAddedHandler
        /// <summary>
        /// A handler for an added entity instance.
        /// </summary>
        private SemanticWorld.EntityInstanceHandler entityInstanceAddedHandler;
        #endregion Field: entityInstanceAddedHandler

        #region Field: entityInstanceRemovedHandler
        /// <summary>
        /// A handler for a removed entity instance.
        /// </summary>
        private SemanticWorld.EntityInstanceHandler entityInstanceRemovedHandler;
        #endregion Field: entityInstanceRemovedHandler

        #region Field: contentToSkip
        /// <summary>
        /// The entity instances for which the content creation should be skipped.
        /// </summary>
        internal List<EntityInstance> contentToSkip = new List<EntityInstance>();
        #endregion Field: contentToSkip

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: GameSemanticsEngine()
        /// <summary>
        /// Creates a new game semantics engine.
        /// </summary>
        protected GameSemanticsEngine()
        {
            // Create handlers
            this.contextTypeAddedHandler = new EntityInstance.ContextTypeHandler(entityInstance_ContextTypeAdded);
            this.contextTypeRemovedHandler = new EntityInstance.ContextTypeHandler(entityInstance_ContextTypeRemoved);
            this.startedActionHandler = new EntityInstance.ActionHandler(entityInstance_StartedAction);
            this.executedActionHandler = new EntityInstance.ActionHandler(entityInstance_ExecutedAction);
            this.stoppedActionHandler = new EntityInstance.ActionHandler(entityInstance_StoppedAction);
            this.entityInstanceAddedHandler = new SemanticWorld.EntityInstanceHandler(world_EntityInstanceAdded);
            this.entityInstanceRemovedHandler = new SemanticWorld.EntityInstanceHandler(world_EntityInstanceRemoved);
        }
        #endregion Constructor: GameSemanticsEngine()

        #endregion Method Group: Constructors

        #region Method Group: General

        #region Method: Initialize()
        /// <summary>
        /// Initialize the game semantics engine, the game base manager, and the game instance manager.
        /// </summary>
        public static new void Initialize()
        {
            // Make this semantics engine the current semantics engine
            SemanticsEngine.SemanticsEngine.Current = new GameSemanticsEngine();

            // Initialize the game base manager
            GameBaseManager.Initialize();

            // Initialize the game instance manager
            GameInstanceManager.Initialize();
        }
        #endregion Method: Initialize()

        #region Method: Uninitialize()
        /// <summary>
        /// Uninitialize the game semantics engine.
        /// </summary>
        public static new void Uninitialize()
        {
            SemanticsEngine.SemanticsEngine.Uninitialize();
        }
        #endregion Method: Uninitialize()

        #region Method: LoadGameNodesAndObjects()
        /// <summary>
        /// Create bases for all game nodes and game objects.
        /// </summary>
        private void LoadGameNodesAndObjects()
        {
            if (GameDatabase.Current != null)
            {
                // Create bases
                this.gameObjectBases = new List<GameObjectBase>();
                foreach (GameObject gameObject in DatabaseSearch.GetNodes<GameObject>())
                    this.gameObjectBases.Add(GameBaseManager.Current.GetBase<GameObjectBase>(gameObject));

                this.gameNodeBases = new List<GameNodeBase>();
                foreach (GameNode gameNode in DatabaseSearch.GetNodes<GameNode>())
                    this.gameNodeBases.Add(GameBaseManager.Current.GetBase<GameNodeBase>(gameNode));
            }
        }
        #endregion Method: LoadGameNodesAndObjects()

        #endregion Method Group: General

        #region Method Group: Worlds and instances

        #region Method: AddWorld(SemanticWorld world)
        /// <summary>
        /// Add a new world to the Semantics Engine.
        /// </summary>
        /// <param name="world">The world to add to the Semantics Engine.</param>
        /// <returns>Returns whether the world has been added successfully.</returns>
        public override bool AddWorld(SemanticWorld world)
        {
            if (world != null)
            {
                base.AddWorld(world);

                // Subscribe to updated of added or removed entity instances
                world.EntityInstanceAdded += this.entityInstanceAddedHandler;
                world.EntityInstanceRemoved += this.entityInstanceRemovedHandler;

                // Create content for all instances for which this has not yet been done
                foreach (EntityInstance entityInstance in world.Instances)
                {
                    if (ContentManager.GetContentWrapper(entityInstance) == null)
                        CreateContent(entityInstance, GetAbstractGameNodeBase(entityInstance));
                }
            }
            return false;
        }
        #endregion Method: AddWorld(SemanticWorld world)

        #region Method: RemoveWorld(SemanticWorld world)
        /// <summary>
        /// Removes a world from the Semantics Engine.
        /// </summary>
        /// <param name="instance">The world to remove from the Semantics Engine.</param>
        /// <returns>Returns whether the world has been removed successfully.</returns>
        public override bool RemoveWorld(SemanticWorld world)
        {
            if (world != null)
            {
                base.RemoveWorld(world);

                // Unsubscribe from updates of added or removed entity instances
                world.EntityInstanceAdded -= this.entityInstanceAddedHandler;
                world.EntityInstanceRemoved -= this.entityInstanceRemovedHandler;
            }
            return false;
        }
        #endregion Method: RemoveWorld(SemanticWorld world)

        #region Method: world_EntityInstanceAdded(SemanticWorld world, EntityInstance entityInstance)
        /// <summary>
        /// Handle the addition of the given entity instance from the world.
        /// </summary>
        /// <param name="world">The world.</param>
        /// <param name="entityInstance">The added entity instance.</param>
        private void world_EntityInstanceAdded(SemanticWorld world, EntityInstance entityInstance)
        {
            if (entityInstance != null && !contentToSkip.Contains(entityInstance))
                CreateContent(entityInstance, GetAbstractGameNodeBase(entityInstance));
        }
        #endregion Method: world_EntityInstanceAdded(SemanticWorld world, EntityInstance entityInstance)

        #region Method: world_EntityInstanceRemoved(SemanticWorld world, EntityInstance entityInstance)
        /// <summary>
        /// Handle the removal of the given entity instance from the given world.
        /// </summary>
        /// <param name="world">The world.</param>
        /// <param name="entityInstance">The removed entity instance.</param>
        private void world_EntityInstanceRemoved(SemanticWorld world, EntityInstance entityInstance)
        {
            if (entityInstance != null)
            {
                ContentWrapper contentWrapper = null;
                if (ContentManager.TryGetContentWrapper(entityInstance, out contentWrapper))
                {
                    // Notify that all game content should be hidden/stopped
                    foreach (StaticContentInstance staticContentInstance in contentWrapper.StaticContent)
                        contentWrapper.NotifyOfHiddenContent(staticContentInstance, null);
                    foreach (DynamicContentInstance dynamicContentInstance in contentWrapper.DynamicContent)
                        contentWrapper.NotifyOfStoppedContent(dynamicContentInstance);
                }

                // Unsubscribe from updates
                entityInstance.ContextTypeAdded -= this.contextTypeAddedHandler;
                entityInstance.ContextTypeRemoved -= this.contextTypeRemovedHandler;
                entityInstance.StartedAction -= this.startedActionHandler;
                entityInstance.ExecutedAction -= this.executedActionHandler;
                entityInstance.StoppedAction -= this.stoppedActionHandler;
            }
        }
        #endregion Method: world_EntityInstanceRemoved(SemanticWorld world, EntityInstance entityInstance)

        #endregion Method Group: Worlds and instances

        #region Method Group: Content

        #region Method: CreateContent(EntityInstance entityInstance, AbstractGameNodeBase abstractGameNodeBase)
        /// <summary>
        /// Create game content for the given entity instance.
        /// </summary>
        /// <param name="entityInstance">The entity instance to create game content for.</param>
        /// <param name="abstractGameNodeBase">The abstract game node to get the content from.</param>
        internal void CreateContent(EntityInstance entityInstance, AbstractGameNodeBase abstractGameNodeBase)
        {
            if (entityInstance != null && abstractGameNodeBase != null)
            {
                // Make sure that a content wrapper is defined for the entity instance
                ContentWrapper contentWrapper = ContentManager.GetContentWrapper(entityInstance);
                if (contentWrapper == null)
                {
                    contentWrapper = new ContentWrapper(entityInstance);
                    ContentManager.AddContentWrapper(entityInstance, contentWrapper);
                }

                // Set the abstract game node base of the wrapper
                contentWrapper.AbstractGameNodeBase = abstractGameNodeBase;

                // Create content instances of all valued content
                foreach (ContentValuedBase contentValuedBase in abstractGameNodeBase.GetAllContent())
                    contentWrapper.AddContent(GameInstanceManager.Current.Create<ContentInstance>(contentValuedBase));

                // Perform checks and subscriptions when there is content
                if (contentWrapper.AllContent.Count > 0)
                {
                    // Subscribe to updates in the context types
                    entityInstance.ContextTypeAdded += this.contextTypeAddedHandler;
                    entityInstance.ContextTypeRemoved += this.contextTypeRemovedHandler;

                    // Get the current context types and check their current content
                    foreach (ContextTypeBase contextTypeBase in entityInstance.ContextTypes)
                        contentWrapper.CheckForShownContent(contextTypeBase);

                    // Also check for non context type based content
                    contentWrapper.CheckForShownContent(null);

                    // Subscribe to started, executed, or stopped actions
                    if (abstractGameNodeBase.EventContent.Count > 0)
                    {
                        entityInstance.StartedAction += this.startedActionHandler;
                        entityInstance.ExecutedAction += this.executedActionHandler;
                        entityInstance.StoppedAction += this.stoppedActionHandler;
                    }
                }
            }
        }
        #endregion Method: CreateContent(EntityInstance entityInstance, AbstractGameNodeBase abstractGameNodeBase)

        #region Method: GetAbstractGameNodeBase(EntityInstance entityInstance)
        /// <summary>
        /// Look for a suitable game object or game node for the given entity instance.
        /// </summary>
        /// <param name="entityInstance">The entity instance for which a game object or node should be found.</param>
        /// <returns>The suitable game object or game node for the entity instance.</returns>
        private AbstractGameNodeBase GetAbstractGameNodeBase(EntityInstance entityInstance)
        {
            if (entityInstance != null)
                return GetAbstractGameNodeBase(entityInstance.EntityBase);
            return null;
        }
        #endregion Method: GetAbstractGameNodeBase(EntityInstance entityInstance)

        #region Method: GetAbstractGameNodeBase(EntityBase entityBase)
        /// <summary>
        /// Look for a suitable game object or game node for the given entity base.
        /// </summary>
        /// <param name="entityBase">The entity base for which a game object or node should be found.</param>
        /// <returns>The suitable game object or game node for the entity base.</returns>
        internal AbstractGameNodeBase GetAbstractGameNodeBase(EntityBase entityBase)
        {
            if (entityBase != null)
            {
                TangibleObjectBase tangibleObjectBase = entityBase as TangibleObjectBase;
                if (tangibleObjectBase != null)
                {
                    if (this.gameObjectBases == null)
                        LoadGameNodesAndObjects();

                    // Get the game object of which the tangible object is closest to the added tangible object
                    GameObjectBase bestGameObject = null;
                    int bestDepth = -1;
                    foreach (GameObjectBase gameObjectBase in this.gameObjectBases)
                    {
                        int depth = tangibleObjectBase.IsNodeOfDepth(gameObjectBase.TangibleObject);
                        if ((depth < bestDepth || bestDepth == -1) && depth > -1)
                        {
                            bestGameObject = gameObjectBase;
                            bestDepth = depth;
                        }
                    }

                    return bestGameObject;
                }
                else
                {
                    if (this.gameNodeBases == null)
                        LoadGameNodesAndObjects();

                    // Get the game node of which the entity is closest to the added entity
                    GameNodeBase bestGameNode = null;
                    int bestDepth = -1;
                    foreach (GameNodeBase gameNodeBase in this.gameNodeBases)
                    {
                        int depth = entityBase.IsNodeOfDepth(gameNodeBase.Node);
                        if ((depth < bestDepth || bestDepth == -1) && depth > -1)
                        {
                            bestGameNode = gameNodeBase;
                            bestDepth = depth;
                        }
                    }

                    return bestGameNode;
                }
            }
            return null;
        }
        #endregion Method: GetAbstractGameNodeBase(EntityBase entityBase)

        #region Method: entityInstance_ContextTypeAdded(EntityInstance entityInstance, ContextTypeBase contextTypeBase)
        /// <summary>
        /// Checks for content to be shown when an entity instance gets a context type.
        /// </summary>
        /// <param name="entityInstance">The entity instance.</param>
        /// <param name="contextTypeBase">The context type; can be null.</param>
        private void entityInstance_ContextTypeAdded(EntityInstance entityInstance, ContextTypeBase contextTypeBase)
        {
            if (entityInstance != null)
            {
                // Check for content that should be shown
                ContentWrapper contentWrapper = null;
                if (ContentManager.TryGetContentWrapper(entityInstance, out contentWrapper))
                    contentWrapper.CheckForShownContent(contextTypeBase);
            }
        }
        #endregion Method: entityInstance_ContextTypeAdded(EntityInstance entityInstance, ContextTypeBase contextTypeBase)

        #region Method: entityInstance_ContextTypeRemoved(EntityInstance entityInstance, ContextTypeBase contextTypeBase)
        /// <summary>
        /// Checks for content to be hidden when an entity instance loses a context type.
        /// </summary>
        /// <param name="entityInstance">The entity instance.</param>
        /// <param name="contextTypeBase">The context type; can be null.</param>
        private void entityInstance_ContextTypeRemoved(EntityInstance entityInstance, ContextTypeBase contextTypeBase)
        {
            if (entityInstance != null)
            {
                // Check for content that should be hidden
                ContentWrapper contentWrapper = null;
                if (ContentManager.TryGetContentWrapper(entityInstance, out contentWrapper))
                    contentWrapper.CheckForHiddenContent(contextTypeBase);
            }
        }
        #endregion Method: entityInstance_ContextTypeRemoved(EntityInstance entityInstance, ContextTypeBase contextTypeBase)

        #region Method: entityInstance_StartedAction(EntityInstance actor, ActionBase action, EntityInstance target)
        /// <summary>
        /// Checks for started or stopped content when the actor starts an action.
        /// </summary>
        private void entityInstance_StartedAction(EntityInstance actor, ActionBase action, EntityInstance target)
        {
            if (actor != null)
            {
                // Check for started or stopped content
                ContentWrapper contentWrapper = null;
                if (ContentManager.TryGetContentWrapper(actor, out contentWrapper))
                    contentWrapper.CheckForActionContent(EventStateExtended.Starts, action, target);
            }
        }
        #endregion Method: entityInstance_StartedAction(EntityInstance actor, ActionBase action, EntityInstance target)

        #region Method: entityInstance_ExecutedAction(EntityInstance actor, ActionBase action, EntityInstance target)
        /// <summary>
        /// Checks for started or stopped content when the actor executes an action.
        /// </summary>
        private void entityInstance_ExecutedAction(EntityInstance actor, ActionBase action, EntityInstance target)
        {
            if (actor != null)
            {
                // Check for started or stopped content
                ContentWrapper contentWrapper = null;
                if (ContentManager.TryGetContentWrapper(actor, out contentWrapper))
                    contentWrapper.CheckForActionContent(EventStateExtended.IsExecuting, action, target);
            }
        }
        #endregion Method: entityInstance_ExecutedAction(EntityInstance actor, ActionBase action, EntityInstance target)

        #region Method: entityInstance_StoppedAction(EntityInstance actor, ActionBase action, EntityInstance target)
        /// <summary>
        /// Checks for started or stopped content when the actor stops an action.
        /// </summary>
        private void entityInstance_StoppedAction(EntityInstance actor, ActionBase action, EntityInstance target)
        {
            if (actor != null)
            {
                // Check for started or stopped content
                ContentWrapper contentWrapper = null;
                if (ContentManager.TryGetContentWrapper(actor, out contentWrapper))
                    contentWrapper.CheckForActionContent(EventStateExtended.Stops, action, target);
            }
        }
        #endregion Method: entityInstance_StoppedAction(EntityInstance actor, ActionBase action, EntityInstance target)

        #endregion Method Group: Content

        #region Method Group: Effect handling

        #region Method: ApplyChange(ChangeBase changeBase, EventInstance eventInstance, EntityInstance target)
        /// <summary>
        /// Apply a change to a target.
        /// </summary>
        /// <param name="changeBase">The change instance to execute.</param>
        /// <param name="eventInstance">The event instance belonging to the change instance.</param>
        /// <param name="target">The target.</param>
        /// <returns>Returns whether the application has been successful.</returns>
        protected override bool ApplyChange(ChangeBase changeBase, EventInstance eventInstance, EntityInstance target)
        {
            if (target != null)
            {
                if (changeBase is ContentChangeBase)
                {
                    // Apply content changes to the content wrapper of the target
                    ContentWrapper contentWrapper = null;
                    if (ContentManager.TryGetContentWrapper(target, out contentWrapper))
                        return contentWrapper.Apply(changeBase, eventInstance);
                    return false;
                }
                else
                {
                    // Otherwise, let the Semantics Engine apply the change to the target
                    return base.ApplyChange(changeBase, eventInstance, target);
                }
            }
            return false;
        }
        #endregion Method: ApplyChange(ChangeBase changeBase, EventInstance eventInstance, EntityInstance target)

        #endregion Method Group: Effect handling

    }
    #endregion Class: GameSemanticsEngine

}