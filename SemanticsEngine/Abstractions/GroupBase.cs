/**************************************************************************
 * 
 * GroupBase.cs
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
using Semantics.Abstractions;
using Semantics.Entities;
using SemanticsEngine.Entities;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Abstractions
{

    #region Class: GroupBase
    /// <summary>
    /// A base of a group.
    /// </summary>
    public class GroupBase : AbstractionBase
    {

        #region Properties and Fields

        #region Property: Group
        /// <summary>
        /// Gets the group of which this is a group base.
        /// </summary>
        protected internal Group Group
        {
            get
            {
                return this.IdHolder as Group;
            }
        }
        #endregion Property: Group

        #region Property: Entities
        /// <summary>
        /// The entities that belong to the group.
        /// </summary>
        private EntityBase[] entities = null;

        /// <summary>
        /// Gets the entities that belong to the group.
        /// </summary>
        public ReadOnlyCollection<EntityBase> Entities
        {
            get
            {
                if (entities == null)
                {
                    LoadEntities();
                    if (entities == null)
                        return new List<EntityBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<EntityBase>(entities);
            }
        }

        /// <summary>
        /// Loads the entities.
        /// </summary>
        private void LoadEntities()
        {
            if (this.Group != null)
            {
                List<EntityBase> groupEntities = new List<EntityBase>();
                foreach (Entity entity in this.Group.Entities)
                    groupEntities.Add(BaseManager.Current.GetBase<EntityBase>(entity));
                entities = groupEntities.ToArray();
            }
        }
        #endregion Property: Entities

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: GroupBase(Group group)
        /// <summary>
        /// Creates a base of a group.
        /// </summary>
        /// <param name="group">The group to create a base of.</param>
        protected internal GroupBase(Group group)
            : base(group)
        {
            if (group != null)
            {
                if (BaseManager.PreloadProperties)
                    LoadEntities();
            }
        }
        #endregion Constructor: GroupBase(Group group)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: BelongsToGroup(EntityInstance entityInstance)
        /// <summary>
        /// Checks whether the given entity instance belongs to this group.
        /// </summary>
        /// <param name="entityInstance">The entity instance to check.</param>
        /// <returns>Returns whether the given entity instance belongs to this group.</returns>
        public bool BelongsToGroup(EntityInstance entityInstance)
        {
            if (entityInstance != null && entityInstance.EntityBase != null)
            {
                foreach (EntityBase entityBase in this.Entities)
                {
                    if (entityInstance.EntityBase.IsNodeOf(entityBase))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: BelongsToGroup(EntityInstance entityInstance)

        #endregion Method Group: Other

    }
    #endregion Class: GroupBase

}