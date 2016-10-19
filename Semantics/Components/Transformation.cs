/**************************************************************************
 * 
 * Transformation.cs
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
using Semantics.Data;
using Semantics.Entities;
using Semantics.Utilities;

namespace Semantics.Components
{

    #region Class: Transformation
    /// <summary>
    /// A transformation effect.
    /// </summary>
    public sealed class Transformation : Effect
    {

        #region Properties and Fields

        #region Property: Entities
        /// <summary>
        /// Gets the entities to transform into.
        /// </summary>
        public ReadOnlyCollection<EntityValued> Entities
        {
            get
            {
                return Database.Current.SelectAll<EntityValued>(this.ID, ValueTables.TransformationEntity, Columns.EntityValued).AsReadOnly();
            }
        }
        #endregion Property: Entities

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: Transformation()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static Transformation()
        {
            // Entities
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.EntityValued, new Tuple<Type, EntryType>(typeof(EntityValued), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.TransformationEntity, typeof(Transformation), dict);
        }
        #endregion Static Constructor: Transformation()

        #region Constructor: Transformation()
        /// <summary>
        /// Creates a new transformation.
        /// </summary>
        public Transformation()
        {
        }
        #endregion Constructor: Transformation()

        #region Constructor: Transformation(uint id)
        /// <summary>
        /// Creates a new transformation from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a transformation from.</param>
        private Transformation(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Transformation(uint id)

        #region Constructor: Transformation(Transformation transformation)
        /// <summary>
        /// Clones a transformation.
        /// </summary>
        /// <param name="transformation">The transformation to clone.</param>
        public Transformation(Transformation transformation)
            : base()
        {
            if (transformation != null)
            {
                Database.Current.StartChange();

                foreach (EntityValued entityValued in transformation.Entities)
                    AddEntity(entityValued.Clone());

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Transformation(Transformation transformation)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddEntity(EntityValued entityValued)
        /// <summary>
        /// Adds a valued entity.
        /// </summary>
        /// <param name="entityValued">The valued entity to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddEntity(EntityValued entityValued)
        {
            if (entityValued != null && entityValued.Entity != null)
            {
                // If this valued entity is already available, there is no use to add it
                if (HasEntity(entityValued.Entity))
                    return Message.RelationExistsAlready;

                // Add the valued entity
                Database.Current.Insert(this.ID, ValueTables.TransformationEntity, Columns.EntityValued, entityValued);
                NotifyPropertyChanged("Entities");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddEntity(EntityValued entityValued)

        #region Method: RemoveEntity(EntityValued entityValued)
        /// <summary>
        /// Removes a valued entity.
        /// </summary>
        /// <param name="entityValued">The valued entity to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveEntity(EntityValued entityValued)
        {
            if (entityValued != null)
            {
                if (HasEntity(entityValued.Entity))
                {
                    // Remove the valued entity
                    Database.Current.Remove(this.ID, ValueTables.TransformationEntity, Columns.EntityValued, entityValued);
                    NotifyPropertyChanged("Entities");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveEntity(EntityValued entityValued)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasEntity(Entity entity)
        /// <summary>
        /// Checks if this transformation has a valued entity of the given entity.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>Returns true when the transformation has a valued entity of the given entity.</returns>
        public bool HasEntity(Entity entity)
        {
            if (entity != null)
            {
                foreach (EntityValued entityValued in this.Entities)
                {
                    if (entity.Equals(entityValued.Entity))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasEntity(Entity entity)

        #region Method: Remove()
        /// <summary>
        /// Remove the transformation.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the entities
            foreach (EntityValued entityValued in this.Entities)
                entityValued.Remove();
            Database.Current.Remove(this.ID, ValueTables.TransformationEntity);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: Transformation

}