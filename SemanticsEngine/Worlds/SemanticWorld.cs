/**************************************************************************
 * 
 * SemanticWorld.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Common;
using Semantics.Abstractions;
using Semantics.Components;
using Semantics.Data;
using Semantics.Entities;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Components;
using SemanticsEngine.Entities;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Worlds
{

    #region Class: SemanticWorld
    /// <summary>
    /// A semantic world containing instances.
    /// </summary>
    public class SemanticWorld : PropertyChangedComponent
    {

        #region Events, Properties, and Fields

        #region Event: InstanceHandler
        /// <summary>
        /// A handler for the entity instance events.
        /// </summary>
        /// <param name="world">The world to which the entity instance was added.</param>
        /// <param name="entityInstance">The entity instance that was added to the world.</param>
        public delegate void EntityInstanceHandler(SemanticWorld world, EntityInstance entityInstance);

        /// <summary>
        /// An event for an added entity instance.
        /// </summary>
        public event EntityInstanceHandler EntityInstanceAdded;

        /// <summary>
        /// An event for a removed entity instance.
        /// </summary>
        public event EntityInstanceHandler EntityInstanceRemoved;
        #endregion Event: InstanceHandler

        #region Property: Space
        /// <summary>
        /// The space of this world.
        /// </summary>
        private SpaceInstance worldSpace = null;

        /// <summary>
        /// The space of this world.
        /// </summary>
        public SpaceInstance Space
        {
            get
            {
                return worldSpace;
            }
        }
        #endregion Property: Space

        #region Property: Instances
        /// <summary>
        /// All the instances in the world.
        /// </summary>
        private List<EntityInstance> instances = new List<EntityInstance>();

        /// <summary>
        /// Gets all the instances in the world.
        /// </summary>
        public ReadOnlyCollection<EntityInstance> Instances
        {
            get
            {
                return instances.AsReadOnly();
            }
        }
        #endregion Property: Instances

        #region Property: ModifiedInstances
        /// <summary>
        /// Gets all instances that have been modified by the engine.
        /// </summary>
        internal IEnumerable<EntityInstance> ModifiedInstances
        {
            get
            {
                foreach (EntityInstance instance in instances)
                {
                    if (instance.IsModified)
                        yield return instance;
                }
            }
        }
        #endregion Property: ModifiedInstances

        #endregion Events, Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SemanticWorld()
        /// <summary>
        /// Create a new semantic world.
        /// </summary>
        public SemanticWorld()
        {
            SetDefaultWorldSpace();
        }
        #endregion Constructor: SemanticWorld()

        #region Constructor: SemanticWorld(string spaceName)
        /// <summary>
        /// Create a new semantic world from the given space name.
        /// </summary>
        /// <param name="spaceName">The name of the space.</param>
        public SemanticWorld(string spaceName)
        {
            Space space = DatabaseSearch.GetNode<Space>(spaceName);
            if (space != null)
            {
                this.worldSpace = new SpaceInstance(new SpaceBase(space));
                if (this.worldSpace == null)
                {
                    SetDefaultWorldSpace();
                    this.worldSpace.DefaultName = spaceName;
                }
                else
                    this.worldSpace.World = this;
            }
            else
            {
                SetDefaultWorldSpace();
                this.worldSpace.DefaultName = spaceName;
            }
        }
        #endregion Constructor: SemanticWorld()

        #region Constructor: SemanticWorld(SpaceValuedBase spaceValuedBase)
        /// <summary>
        /// Create a new semantic world from the given valued space.
        /// </summary>
        /// <param name="spaceValuedBase">The valued space to create the semantic world from.</param>
        public SemanticWorld(SpaceValuedBase spaceValuedBase)
        {
            this.worldSpace = InstanceManager.Current.Create<SpaceInstance>(spaceValuedBase);
            if (this.worldSpace == null)
                SetDefaultWorldSpace();
            else
                this.worldSpace.World = this;
        }
        #endregion Constructor: SemanticWorld(SpaceValuedBase spaceValuedBase)

        #region Constructor: SemanticWorld(SpaceInstance spaceInstance)
        /// <summary>
        /// Create a new semantic world from the given space instance.
        /// </summary>
        /// <param name="spaceInstance">The space instance to create the semantic world from.</param>
        public SemanticWorld(SpaceInstance spaceInstance)
        {
            if (spaceInstance != null)
            {
                this.worldSpace = spaceInstance;
                this.worldSpace.World = this;
            }
            else
                SetDefaultWorldSpace();
        }
        #endregion Constructor: SemanticWorld(SpaceInstance spaceInstance)

        #region Method: SetDefaultWorldSpace()
        /// <summary>
        /// Set the default world space.
        /// </summary>
        private void SetDefaultWorldSpace()
        {
            this.worldSpace = new SpaceInstance(new SpaceBase(null));
            this.worldSpace.DefaultName = SemanticsEngineSettings.WorldName;
            this.worldSpace.World = this;
        }
        #endregion Method: SetDefaultWorldSpace()
		
        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddInstance(AbstractEntity abstractEntity)
        /// <summary>
        /// Add an instance of the given abstract entity to the world.
        /// </summary>
        /// <param name="abstractEntity">The abstract entity to create and add an instance of.</param>
        /// <returns>The instance of the abstract entity that was added to the world.</returns>
        public AbstractEntityInstance AddInstance(AbstractEntity abstractEntity)
        {
            // Create and add a new instance
            AbstractEntityInstance abstractEntityInstance = InstanceManager.Current.Create<AbstractEntityInstance>(abstractEntity);
            AddInstance(abstractEntityInstance);
            return abstractEntityInstance;
        }
        #endregion Method: AddInstance(AbstractEntity abstractEntity)

        #region Method: AddInstance(PhysicalEntity physicalEntity)
        /// <summary>
        /// Add an instance of the given physical entity to the world.
        /// </summary>
        /// <param name="physicalEntity">The physical entity to create and add an instance of.</param>
        /// <returns>The instance of the physical entity that was added to the world.</returns>
        public PhysicalEntityInstance AddInstance(PhysicalEntity physicalEntity)
        {
            return AddInstance(physicalEntity, Vec3.Zero);
        }
        #endregion Method: AddInstance(PhysicalEntity physicalEntity)

        #region Method: AddInstance(PhysicalEntity physicalEntity, Vec3 position)
        /// <summary>
        /// Add an instance of the given physical entity to the world on the given position.
        /// </summary>
        /// <param name="physicalEntity">The physical entity to create and add an instance of.</param>
        /// <param name="position">The position in the world the instance should get.</param>
        /// <returns>The instance of the physical entity that was added to the world.</returns>
        public PhysicalEntityInstance AddInstance(PhysicalEntity physicalEntity, Vec3 position)
        {
            PhysicalObject physicalObject = physicalEntity as PhysicalObject;
            if (physicalObject != null)
                return AddInstance(physicalObject, position, Quaternion.Identity);
            else
            {
                // Create and add a new instance
                PhysicalEntityInstance physicalEntityInstance = InstanceManager.Current.Create<PhysicalEntityInstance>(physicalEntity);
                AddInstance(physicalEntityInstance, position);
                return physicalEntityInstance;
            }
        }
        #endregion Method: AddInstance(PhysicalEntity physicalEntity, Vec3 position)

        #region Method: AddInstance(PhysicalEntityInstance physicalEntityInstance, Vec3 position)
        /// <summary>
        /// Add the given physical entity instance to the world on the given position.
        /// </summary>
        /// <param name="physicalEntityInstance">The physical entity instance to add.</param>
        /// <param name="position">The position in the world the instance should get.</param>
        /// <returns>The instance of the physical entity that was added to the world.</returns>
        public Message AddInstance(PhysicalEntityInstance physicalEntityInstance, Vec3 position)
        {
            if (physicalEntityInstance != null)
            {
                if (position == null)
                    position = Vec3.Zero;
                else
                    position = new Vec3(position);

                // In case of matter, already set the position, as it may be used for merging
                MatterInstance matterInstance = physicalEntityInstance as MatterInstance;
                if (matterInstance != null)
                    matterInstance.Position = position;

                // Add the instance
                Message message = AddInstance(physicalEntityInstance);
                
                // Set the position
                physicalEntityInstance.Position = position;

                return message;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddInstance(PhysicalEntityInstance physicalEntityInstance, Vec3 position)

        #region Method: AddInstance(PhysicalObject physicalObject, Vec3 position, Quaternion rotation)
        /// <summary>
        /// Add an instance of the given physical object to the world on the given position with the given rotation.
        /// </summary>
        /// <param name="physicalObject">The physical object to create and add an instance of.</param>
        /// <param name="position">The position in the world the instance should get.</param>
        /// <param name="rotation">The rotation the instance should get.</param>
        /// <returns>The instance of the physical object that was added to the world.</returns>
        public PhysicalObjectInstance AddInstance(PhysicalObject physicalObject, Vec3 position, Quaternion rotation)
        {
            if (physicalObject != null)
            {
                // Create and add a new instance
                PhysicalObjectInstance physicalObjectInstance = InstanceManager.Current.Create<PhysicalObjectInstance>(physicalObject);
                AddInstance(physicalObjectInstance, position, rotation);
                return physicalObjectInstance;
            }
            return null;
        }
        #endregion Method: AddInstance(PhysicalObject physicalObject, Vec3 position, Quaternion rotation)

        #region Method: AddInstance(PhysicalObjectInstance physicalObjectInstance, Vec3 position, Quaternion rotation)
        /// <summary>
        /// Add the given physical object instance to the world on the given position with the given rotation.
        /// </summary>
        /// <param name="physicalObjectInstance">The physical object instance to add.</param>
        /// <param name="position">The position in the world the instance should get.</param>
        /// <param name="rotation">The rotation the instance should get.</param>
        /// <returns>The instance of the physical object that was added to the world.</returns>
        public Message AddInstance(PhysicalObjectInstance physicalObjectInstance, Vec3 position, Quaternion rotation)
        {
            if (physicalObjectInstance != null)
            {
                // Add the instance
                Message message = AddInstance(physicalObjectInstance);

                // Set the position
                if (position == null)
                    position = Vec3.Zero;
                else
                    position = new Vec3(position);
                physicalObjectInstance.Position = position;

                // Set the rotation
                if (rotation == null)
                    rotation = Quaternion.Identity;
                else
                    rotation = new Quaternion(rotation);
                physicalObjectInstance.Rotation = rotation;

                return message;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddInstance(PhysicalObjectInstance physicalObjectInstance, Vec3 position, Quaternion rotation)

        #region Method: AddInstance(EntityInstance entityInstance)
        /// <summary>
        /// Add the given entity instance to the world, including its parts, connections, and space items (if applicable).
        /// </summary>
        /// <param name="entityInstance">The entity instance to add to the world.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddInstance(EntityInstance entityInstance)
        {
            if (entityInstance != null && entityInstance.EntityBase != null)
            {
                if (entityInstance.World != null)
                {
                    // If the entity instance is already in this world, there is no use to add it
                    if (this.Equals(entityInstance.World))
                        return Message.RelationExistsAlready;
                    else
                    {
                        // If the entity is already in another world, remove it from that one
                        entityInstance.World.RemoveInstance(entityInstance);
                    }
                }
                
                // Add the entity instance
                entityInstance.World = this;
                this.instances.Add(entityInstance);
                NotifyPropertyChanged("Instances");

                // Make sure that the engine updates the instance for the first time
                entityInstance.IsModified = true;

                // Add all included instances to the world as well
                PhysicalObjectInstance physicalObjectInstance = entityInstance as PhysicalObjectInstance;
                if (physicalObjectInstance != null)
                {
                    // For physical objects, add the spaces
                    foreach (SpaceInstance space in physicalObjectInstance.Spaces)
                        AddInstance(space);

                    SpaceInstance spaceInstance = physicalObjectInstance as SpaceInstance;
                    if (spaceInstance != null)
                    {
                        // For spaces, add the items and tangible matter
                        foreach (PhysicalObjectInstance item in spaceInstance.Items)
                            AddInstance(item);
                        foreach (MatterInstance tangibleMatter in spaceInstance.TangibleMatter)
                            AddInstance(tangibleMatter);

                        // Also add the space to the world space if it is not yet of another physical object
                        if (this.worldSpace != null && spaceInstance.PhysicalObject == null)
                            this.worldSpace.AddSpace(spaceInstance);
                    }

                    TangibleObjectInstance tangibleObjectInstance = physicalObjectInstance as TangibleObjectInstance;
                    if (tangibleObjectInstance != null)
                    {
                        // For tangible objects, add the parts, connections, covers, and matter
                        foreach (TangibleObjectInstance part in tangibleObjectInstance.Parts)
                            AddInstance(part);
                        foreach (TangibleObjectInstance connectionItem in tangibleObjectInstance.Connections)
                            AddInstance(connectionItem);
                        foreach (TangibleObjectInstance cover in tangibleObjectInstance.Covers)
                            AddInstance(cover);
                        foreach (MatterInstance matter in tangibleObjectInstance.Matter)
                            AddInstance(matter);

                        // Also add the item to the world space if it is not yet in another space
                        if (this.worldSpace != null && tangibleObjectInstance.Space == null)
                            this.worldSpace.AddItem(tangibleObjectInstance, true);
                    }
                }
                else
                {
                    MatterInstance matterInstance = entityInstance as MatterInstance;
                    if (matterInstance != null)
                    {
                        // For matter, add the elements
                        foreach (ElementInstance element in matterInstance.Elements)
                            AddInstance(element);

                        // Also add the matter to the world space if it is not yet in another space
                        if (this.worldSpace != null && matterInstance.Space == null)
                            this.worldSpace.AddTangibleMatter(matterInstance, matterInstance.Position);

                        // For compounds, also add the substances
                        CompoundInstance compoundInstance = matterInstance as CompoundInstance;
                        if (compoundInstance != null)
                        {
                            foreach (SubstanceInstance substance in compoundInstance.Substances)
                                AddInstance(substance);
                        }

                        // For mixtures, also add the substances
                        MixtureInstance mixtureInstance = matterInstance as MixtureInstance;
                        if (mixtureInstance != null)
                        {
                            foreach (SubstanceInstance substance in mixtureInstance.Substances)
                                AddInstance(substance);
                        }
                    }
                }

                // Invoke an event
                if (EntityInstanceAdded != null)
                    EntityInstanceAdded(this, entityInstance);

                // Make a notification to the engine
                if (SemanticsEngine.Current != null && SemanticsEngine.Current.Worlds.Contains(this))
                    SemanticsEngine.Current.HandleAddedEntityInstance(entityInstance);
                
                return Message.RelationSuccess;
            }

            return Message.RelationFail;
        }
        #endregion Method: AddInstance(EntityInstance entityInstance)

        #region Method: RemoveInstance(EntityInstance entityInstance)
        /// <summary>
        /// Removes an entity instance from the world.
        /// </summary>
        /// <param name="entityInstance">The entity instance to remove from the world.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveInstance(EntityInstance entityInstance)
        {
            return RemoveInstance(entityInstance, SemanticsSettings.Values.RemoveEverything);
        }
        
        /// <summary>
        /// Removes an entity instance from the world.
        /// </summary>
        /// <param name="entityInstance">The entity instance to remove from the world.</param>
        /// <param name="removeEverything">Indicates whether everything of the instance should be removed.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveInstance(EntityInstance entityInstance, bool removeEverything)
        {
            if (entityInstance != null)
            {
                // Remove the instance
                if (this.instances.Remove(entityInstance))
                {
                    // The engine does not have to update the instance anymore
                    entityInstance.IsModified = false;

                    NotifyPropertyChanged("Instances");

                    entityInstance.World = null;
                    
                    // Remove all included instances of the instance, and also remove all references
                    PhysicalObjectInstance physicalObjectInstance = entityInstance as PhysicalObjectInstance;
                    if (physicalObjectInstance != null)
                    {
                        // For physical objects, remove the spaces
                        List<SpaceInstance> spaces = new List<SpaceInstance>(physicalObjectInstance.Spaces);
                        foreach (SpaceInstance space in spaces)
                            RemoveInstance(space, removeEverything);

                        SpaceInstance spaceInstance = physicalObjectInstance as SpaceInstance;
                        if (spaceInstance != null)
                        {
                            // For spaces, remove the items and tangible matter
                            if (removeEverything)
                            {
                                List<TangibleObjectInstance> items = new List<TangibleObjectInstance>(spaceInstance.Items);
                                foreach (TangibleObjectInstance item in items)
                                    RemoveInstance(item, removeEverything);
                                List<MatterInstance> tangibleMatter = new List<MatterInstance>(spaceInstance.TangibleMatter);
                                foreach (MatterInstance matterInstance in tangibleMatter)
                                    RemoveInstance(matterInstance, removeEverything);
                            }

                            // Remove the space from its physical object
                            if (spaceInstance.PhysicalObject != null)
                                spaceInstance.PhysicalObject.RemoveSpace(spaceInstance);

                            // Remove the space from the world space
                            if (this.worldSpace != null)
                                this.worldSpace.RemoveSpace(spaceInstance);
                        }

                        TangibleObjectInstance tangibleObjectInstance = physicalObjectInstance as TangibleObjectInstance;
                        if (tangibleObjectInstance != null)
                        {
                            if (removeEverything)
                            {
                                // For tangible objects, remove the parts and covers
                                List<TangibleObjectInstance> parts = new List<TangibleObjectInstance>(tangibleObjectInstance.Parts);
                                foreach (TangibleObjectInstance part in parts)
                                    RemoveInstance(part, removeEverything);
                                List<TangibleObjectInstance> covers = new List<TangibleObjectInstance>(tangibleObjectInstance.Covers);
                                foreach (TangibleObjectInstance cover in covers)
                                    RemoveInstance(cover, removeEverything);
                            }

                            // Remove the matter, but possibly turn it into tangible matter
                            List<MatterInstance> matter = new List<MatterInstance>(tangibleObjectInstance.Matter);
                            SpaceInstance spaceOfTarget = tangibleObjectInstance.Space;
                            foreach (MatterInstance matterInstance in matter)
                            {
                                if (!removeEverything && spaceOfTarget != null)
                                {
                                    tangibleObjectInstance.RemoveMatter(matterInstance);
                                    spaceOfTarget.AddTangibleMatter(matterInstance, new Vec3(tangibleObjectInstance.Position));
                                }
                                else
                                    RemoveInstance(matterInstance, removeEverything);
                            }

                            // Remove the instance from the whole it is a part of
                            if (tangibleObjectInstance.Whole != null)
                                tangibleObjectInstance.Whole.RemovePart(tangibleObjectInstance);

                            // Remove the instance from the object it covers
                            if (tangibleObjectInstance.CoveredObject != null)
                                tangibleObjectInstance.CoveredObject.RemoveCover(tangibleObjectInstance);

                            // Remove the instance from the space it is in
                            if (tangibleObjectInstance.Space != null)
                                tangibleObjectInstance.Space.RemoveItem(tangibleObjectInstance);

                            // Disconnect from the instances the instance is connected to
                            List<TangibleObjectInstance> connections = new List<TangibleObjectInstance>(tangibleObjectInstance.Connections);
                            foreach (TangibleObjectInstance connectionItem in connections)
                                TangibleObjectInstance.RemoveConnection(tangibleObjectInstance, connectionItem);

                            // Remove the item from the world space
                            if (this.worldSpace != null)
                                this.worldSpace.RemoveItem(tangibleObjectInstance);
                        }
                    }
                    else
                    {
                        MatterInstance matterInstance = entityInstance as MatterInstance;
                        if (matterInstance != null)
                        {
                            // For compounds, remove the substances
                            CompoundInstance compoundInstance = matterInstance as CompoundInstance;
                            if (compoundInstance != null)
                            {
                                List<SubstanceInstance> substances = new List<SubstanceInstance>(compoundInstance.Substances);
                                foreach (SubstanceInstance substance in substances)
                                    RemoveInstance(substance, removeEverything);
                            }

                            // For mixtures, remove the substances
                            MixtureInstance mixtureInstance = matterInstance as MixtureInstance;
                            if (mixtureInstance != null)
                            {
                                List<SubstanceInstance> substances = new List<SubstanceInstance>(mixtureInstance.Substances);
                                foreach (SubstanceInstance substance in substances)
                                    RemoveInstance(substance, removeEverything);
                            }

                            // For substances, remove the instance from its compound or mixture
                            SubstanceInstance substanceInstance = matterInstance as SubstanceInstance;
                            if (substanceInstance != null)
                            {
                                if (substanceInstance.Compound != null)
                                    substanceInstance.Compound.RemoveSubstance(substanceInstance);
                                if (substanceInstance.Mixture != null)
                                    substanceInstance.Mixture.RemoveSubstance(substanceInstance);
                            }

                            // Remove the matter from the tangible object or space it is from
                            if (matterInstance.TangibleObject != null)
                                matterInstance.TangibleObject.RemoveMatter(matterInstance);
                            if (matterInstance.Space != null)
                                matterInstance.Space.RemoveTangibleMatter(matterInstance);
                        }
                    }

                    // Remove all relationships
                    List<RelationshipInstance> relationshipsAsSource = new List<RelationshipInstance>(entityInstance.RelationshipsAsSource);
                    foreach (RelationshipInstance relationshipInstance in relationshipsAsSource)
                        EntityInstance.RemoveRelationship(relationshipInstance.RelationshipBase, relationshipInstance.Source, relationshipInstance.Target);
                    List<RelationshipInstance> relationshipsAsTarget = new List<RelationshipInstance>(entityInstance.RelationshipsAsTarget);
                    foreach (RelationshipInstance relationshipInstance in relationshipsAsTarget)
                        EntityInstance.RemoveRelationship(relationshipInstance.RelationshipBase, relationshipInstance.Source, relationshipInstance.Target);

                    // Invoke an event
                    if (EntityInstanceRemoved != null)
                        EntityInstanceRemoved(this, entityInstance);

                    // Make a notification to the engine
                    if (SemanticsEngine.Current != null)
                        SemanticsEngine.Current.HandleRemovedEntityInstance(entityInstance);

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveInstance(EntityInstance entityInstance)

        #region Method: AddScene(Scene scene)
        /// <summary>
        /// Create instances of all physical objects in the scene and add them to the world.
        /// </summary>
        /// <param name="scene">The scene that contains the physical objects of which instances should be created and added to the world.</param>
        public void AddScene(Scene scene)
        {
            // Create a base of the scene and add it
            if (scene != null)
                AddScene(BaseManager.Current.GetBase<SceneBase>(scene));
        }

        /// <summary>
        /// Create instances of all physical objects in the scene and add them to the world.
        /// </summary>
        /// <param name="sceneBase">The scene that contains the physical objects of which instances should be created and added to the world.</param>
        private void AddScene(SceneBase sceneBase)
        {
            if (sceneBase != null)
            {
                // Add instances of the physical objects
                foreach (PhysicalObjectValuedBase physicalObjectValuedBase in sceneBase.PhysicalObjects)
                {
                    for (int i = 0; i < physicalObjectValuedBase.Quantity.Value; i++)
                        AddInstance(InstanceManager.Current.Create<PhysicalObjectInstance>(physicalObjectValuedBase.PhysicalObjectBase));
                }

                // Add the inner scenes
                foreach (SceneValuedBase sceneValuedBase in sceneBase.Scenes)
                {
                    for (int i = 0; i < sceneValuedBase.Quantity.Value; i++)
                        AddScene(sceneValuedBase.SceneBase);
                }
            }
        }
        #endregion Method: AddScene(Scene scene)
		
        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: Clear()
        /// <summary>
        /// Clear all instances in the world.
        /// </summary>
        public void Clear()
        {
            // Remove all instances
            List<EntityInstance> entityInstances = new List<EntityInstance>(this.instances);
            foreach (EntityInstance entityInstance in entityInstances)
                RemoveInstance(entityInstance);

            // Reset the world space
            SetDefaultWorldSpace();
        }
        #endregion Method: Clear()

        #region Method: GetInstances<T>()
        /// <summary>
        /// Get the instances of the given type.
        /// </summary>
        /// <typeparam name="T">The type of instances to get.</typeparam>
        /// <returns>The instances of the given type.</returns>
        public ReadOnlyCollection<T> GetInstances<T>()
            where T : EntityInstance
        {
            List<T> instancesOfType = new List<T>();

            Type type = typeof(T);
            foreach (EntityInstance entityInstance in this.Instances)
            {
                if (type.IsAssignableFrom(entityInstance.GetType()))
                    instancesOfType.Add((T)entityInstance);
            }

            return instancesOfType.AsReadOnly();
        }
        #endregion Method: GetInstances<T>()
		
        #region Method: GetInstancesOfEntity(Entity entity)
        /// <summary>
        /// Get all instances of the given entity.
        /// </summary>
        /// <param name="entity">The entity to get the instances from.</param>
        /// <returns>All instances of the given entity.</returns>
        public ReadOnlyCollection<EntityInstance> GetInstancesOfEntity(Entity entity)
        {
            List<EntityInstance> instancesOfEntity = new List<EntityInstance>();
            if (entity != null)
            {
                foreach (EntityInstance entityInstance in this.Instances)
                {
                    if (entityInstance.EntityBase.IsNodeOf(entity))
                        instancesOfEntity.Add(entityInstance);
                }
            }
            return instancesOfEntity.AsReadOnly();
        }
        #endregion Method: GetInstancesOfEntity(Entity entity)

        #region Method: GetInstancesOfEntity(EntityBase entityBase)
        /// <summary>
        /// Get all instances of the given entity base.
        /// </summary>
        /// <param name="entityBase">The entity to get the instances from.</param>
        /// <returns>All instances of the given entity base.</returns>
        public ReadOnlyCollection<EntityInstance> GetInstancesOfEntity(EntityBase entityBase)
        {
            List<EntityInstance> instancesOfEntityBase = new List<EntityInstance>();
            if (entityBase != null)
            {
                foreach (EntityInstance entityInstance in this.Instances)
                {
                    if (entityInstance.EntityBase.IsNodeOf(entityBase))
                        instancesOfEntityBase.Add(entityInstance);
                }
            }
            return instancesOfEntityBase.AsReadOnly();
        }
        #endregion Method: GetInstancesOfEntity(EntityBase entityBase)

        #region Method: GetInstancesWithName(string name)
        /// <summary>
        /// Get all instances with the given name.
        /// </summary>
        /// <param name="name">The name to get the instances from.</param>
        /// <returns>All instances with the given name.</returns>
        public ReadOnlyCollection<EntityInstance> GetInstancesWithName(string name)
        {
            List<EntityInstance> instancesWithName = new List<EntityInstance>();
            if (name != null)
            {
                foreach (EntityInstance entityInstance in this.Instances)
                {
                    if (entityInstance.DefaultName.Equals(name))
                        instancesWithName.Add(entityInstance);
                }
            }
            return instancesWithName.AsReadOnly();
        }
        #endregion Method: GetInstancesWithName(string name)

        #region Method: GetInstancesWithName<T>(string name)
        /// <summary>
        /// Get all instances with the given name.
        /// </summary>
        /// <typeparam name="T">The type of instances to get.</typeparam>
        /// <param name="name">The name to get the instances from.</param>
        /// <returns>All instances with the given name.</returns>
        public ReadOnlyCollection<T> GetInstancesWithName<T>(string name)
            where T : EntityInstance
        {
            List<T> instancesWithName = new List<T>();
            if (name != null)
            {
                Type type = typeof(T);
                foreach (EntityInstance entityInstance in this.Instances)
                {
                    if (entityInstance.DefaultName.Equals(name) && type.IsAssignableFrom(entityInstance.GetType()))
                        instancesWithName.Add((T)entityInstance);
                }
            }
            return instancesWithName.AsReadOnly();
        }
        #endregion Method: GetInstancesWithName<T>(string name)

        #region Method: GetInstancesAtPosition(Vec3 position)
        /// <summary>
        /// Get the physical entity instances that are at the given position.
        /// </summary>
        /// <param name="position">The position to get the physical entity instances from.</param>
        /// <returns>Returns the physical entity instances that are at the position.</returns>
        public ReadOnlyCollection<PhysicalEntityInstance> GetInstancesAtPosition(Vec3 position)
        {
            return GetInstancesAtPosition(position, SemanticsEngineSettings.DistanceEpsilon);
        }
        #endregion Method: GetInstancesAtPosition(Vec3 position)

        #region Method: GetInstancesAtPosition(Vec3 position, float epsilon)
        /// <summary>
        /// Get the physical entity instances that are at the given position.
        /// </summary>
        /// <param name="position">The position to get the physical entity instances from.</param>
        /// <param name="epsilon">The epsilon for distance checking.</param>
        /// <returns>Returns the physical entity instances that are at the position.</returns>
        public ReadOnlyCollection<PhysicalEntityInstance> GetInstancesAtPosition(Vec3 position, float epsilon)
        {
            List<PhysicalEntityInstance> physicalEntityInstances = new List<PhysicalEntityInstance>();

            foreach (EntityInstance entityInstance in this.Instances)
            {
                PhysicalEntityInstance physicalEntityInstance = entityInstance as PhysicalEntityInstance;
                if (physicalEntityInstance != null)
                {
                    if (Vec3.Distance(position, physicalEntityInstance.Position) < epsilon)
                        physicalEntityInstances.Add(physicalEntityInstance);
                }
            }

            return physicalEntityInstances.AsReadOnly();
        }
        #endregion Method: GetInstancesAtPosition(Vec3 position, float epsilon)

        #endregion Method Group: Other

    }
    #endregion Class: SemanticWorld
		
}