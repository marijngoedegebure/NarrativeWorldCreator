/**************************************************************************
 * 
 * Group.cs
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
using Semantics.Components;
using Semantics.Data;
using Semantics.Entities;
using Semantics.Utilities;

namespace Semantics.Abstractions
{

    #region Class: Group
    /// <summary>
    /// Any number of entities considered as one.
    /// </summary>
    public class Group : Abstraction, IComparable<Group>
    {

        #region Properties and Fields

        #region Property: Entities
        /// <summary>
        /// Gets the personal and inherited entities that belong to the group.
        /// </summary>
        public ReadOnlyCollection<Entity> Entities
        {
            get
            {
                List<Entity> entities = new List<Entity>();
                entities.AddRange(this.PersonalEntities);
                entities.AddRange(this.InheritedEntities);
                return entities.AsReadOnly();
            }
        }
        #endregion Property: Entities

        #region Property: PersonalEntities
        /// <summary>
        /// Gets the personal entities that belong to the group.
        /// </summary>
        public ReadOnlyCollection<Entity> PersonalEntities
        {
            get
            {
                return Database.Current.SelectAll<Entity>(this.ID, GenericTables.GroupEntity, Columns.Entity).AsReadOnly();
            }
        }
        #endregion Property: PersonalEntities

        #region Property: InheritedEntities
        /// <summary>
        /// Gets the inherited entities that belong to the group.
        /// </summary>
        public ReadOnlyCollection<Entity> InheritedEntities
        {
            get
            {
                // The inherited entities are the entities of the children!
                List<Entity> inheritedEntities = new List<Entity>();
                foreach (Node child in this.PersonalChildren)
                    inheritedEntities.AddRange(((Group)child).Entities);
                return inheritedEntities.AsReadOnly();
            }
        }
        #endregion Property: InheritedEntities

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: Group()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static Group()
        {
            // Entities
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Entity, new Tuple<Type, EntryType>(typeof(Entity), EntryType.Intermediate));
            Database.Current.AddTableDefinition(GenericTables.GroupEntity, typeof(Group), dict);
        }
        #endregion Static Constructor: Group()

        #region Constructor: Group()
        /// <summary>
        /// Creates a new group.
        /// </summary>
        public Group()
            : base()
        {
        }
        #endregion Constructor: Group()

        #region Constructor: Group(uint id)
        /// <summary>
        /// Creates a new group from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new group from.</param>
        protected Group(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Group(uint id)

        #region Constructor: Group(string name)
        /// <summary>
        /// Creates a new group with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the group.</param>
        public Group(string name)
            : base(name)
        {
        }
        #endregion Constructor: Group(string name)

        #region Constructor: Group(Group group)
        /// <summary>
        /// Clones a group.
        /// </summary>
        /// <param name="group">The group to clone.</param>
        public Group(Group group)
            : base(group)
        {
            if (group != null)
            {
                Database.Current.StartChange();

                foreach (Entity entity in group.PersonalEntities)
                    AddEntity(entity);

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Group(Group group)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddEntity(Entity entity)
        /// <summary>
        /// Adds the given entity.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddEntity(Entity entity)
        {
            if (entity != null)
            {
                // If the entity is already available in all entities, there is no use to add it
                if (HasEntity(entity))
                    return Message.RelationExistsAlready;

                // Add the entity
                Database.Current.Insert(this.ID, GenericTables.GroupEntity, Columns.Entity, entity);
                NotifyPropertyChanged("PersonalEntities");
                NotifyPropertyChanged("Entities");
                entity.NotifyPropertyChanged("PersonalGroups");
                entity.NotifyPropertyChanged("InheritedGroups");
                entity.NotifyPropertyChanged("Groups");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddEntity(Entity entity)

        #region Method: RemoveEntity(Entity entity)
        /// <summary>
        /// Removes a entity.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveEntity(Entity entity)
        {
            if (entity != null)
            {
                if (HasEntity(entity))
                {
                    // Remove the entity
                    Database.Current.Remove(this.ID, GenericTables.GroupEntity, Columns.Entity, entity);
                    NotifyPropertyChanged("PersonalEntities");
                    NotifyPropertyChanged("Entities");
                    entity.NotifyPropertyChanged("PersonalGroups");
                    entity.NotifyPropertyChanged("InheritedGroups");
                    entity.NotifyPropertyChanged("Groups");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveEntity(Entity entity)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasEntity(Entity entity)
        /// <summary>
        /// Checks if this group has the given entity.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>Returns true when this group has the entity.</returns>
        public bool HasEntity(Entity entity)
        {
            if (entity != null)
            {
                foreach (Entity myEntity in this.Entities)
                {
                    if (entity.Equals(myEntity))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasEntity(Entity entity)

        #region Method: Remove()
        /// <summary>
        /// Remove the group.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();
            Database.Current.StartRemove();

            // Remove the entities
            Database.Current.Remove(this.ID, GenericTables.GroupEntity);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(Group other)
        /// <summary>
        /// Compares the group to the other group.
        /// </summary>
        /// <param name="other">The group to compare to this group.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Group other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(Group other)

        #endregion Method Group: Other

    }
    #endregion Class: Group

}