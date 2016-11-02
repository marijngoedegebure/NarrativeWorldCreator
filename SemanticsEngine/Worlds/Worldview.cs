/**************************************************************************
 * 
 * WorldView.cs
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
using Semantics.Components;
using Semantics.Utilities;
using SemanticsEngine.Entities;

namespace SemanticsEngine.Worlds
{

    #region Class: WorldView
    /// <summary>
    /// A view on a semantic world.
    /// </summary>
    public class Worldview : PropertyChangedComponent
    {

        #region Properties and Field

        #region Property: World
        /// <summary>
        /// The world this is a view of.
        /// </summary>
        private SemanticWorld world = null;

        /// <summary>
        /// Gets the world this is a view of.
        /// </summary>
        public SemanticWorld World
        {
            get
            {
                return world;
            }
            private set
            {
                world = value;
                NotifyPropertyChanged("World");
            }
        }
        #endregion Property: World

        #region Property: Instances
        /// <summary>
        /// All the instances in the world view.
        /// </summary>
        private List<EntityInstance> instances = new List<EntityInstance>();

        /// <summary>
        /// Gets all the instances in the world view.
        /// </summary>
        public ReadOnlyCollection<EntityInstance> Instances
        {
            get
            {
                return instances.AsReadOnly();
            }
        }
        #endregion Property: Instances

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: Worldview(Worldview worldview)
        /// <summary>
        /// Clones a worldview.
        /// </summary>
        /// <param name="worldview">The worldview to clone.</param>
        public Worldview(Worldview worldview)
        {
            if (worldview != null)
            {
                this.World = worldview.World;
                foreach (EntityInstance instance in worldview.Instances)
                    AddInstance(instance);
            }
        }
        #endregion Constructor: Worldview(Worldview worldview)

        #region Constructor: Worldview(SemanticWorld world)
        /// <summary>
        /// Create a new worldview on the given world.
        /// </summary>
        /// <param name="world">The world to create a view for.</param>
        public Worldview(SemanticWorld world)
        {
            this.World = world;
        }
        #endregion Constructor: Worldview(SemanticWorld world)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddInstance(EntityInstance entityInstance)
        /// <summary>
        /// Add an entity instance to the worldview.
        /// </summary>
        /// <param name="entityInstance">The entity instance to add to the worldview.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddInstance(EntityInstance entityInstance)
        {           
            // Make sure the entity is also in the world
            if (entityInstance != null && this.World.Instances.Contains(entityInstance))
            {
                // If the entity instance is already available in all instances, there is no use to add it
                if (this.instances.Contains(entityInstance))
                    return Message.RelationExistsAlready;

                // Add the entity instance
                this.instances.Add(entityInstance);
                NotifyPropertyChanged("Instances");

                return Message.RelationSuccess;
            }

            return Message.RelationFail;
        }
        #endregion Method: AddInstance(EntityInstance entityInstance)

        #region Method: RemoveInstance(EntityInstance entityInstance)
        /// <summary>
        /// Removes an entity instance from the worldview.
        /// </summary>
        /// <param name="entityInstance">The entity instance to remove from the worldview.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveInstance(EntityInstance entityInstance)
        {
            if (entityInstance != null)
            {
                if (this.instances.Remove(entityInstance))
                {
                    NotifyPropertyChanged("Instances");
                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveInstance(EntityInstance entityInstance)

        #endregion Method Group: Add/Remove

    }
    #endregion Class: WorldView

}