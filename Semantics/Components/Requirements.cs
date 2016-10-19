/**************************************************************************
 * 
 * Requirements.cs
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
using Semantics.Data;
using Semantics.Entities;
using Semantics.Utilities;

namespace Semantics.Components
{

    #region Class: Requirements
    /// <summary>
    /// Requirements on a subject.
    /// </summary>
    public sealed class Requirements : IdHolder
    {

        #region Properties and Fields

        #region Property: ContextTypes
        /// <summary>
        /// Gets the required context types.
        /// </summary>
        public ReadOnlyCollection<ContextType> ContextTypes
        {
            get
            {
                return Database.Current.SelectAll<ContextType>(this.ID, GenericTables.RequirementsContextType, Columns.ContextType).AsReadOnly();
            }
        }
        #endregion Property: ContextTypes

        #region Property: Conditions
        /// <summary>
        /// Gets the extra conditions, besides the required context types.
        /// </summary>
        public ReadOnlyCollection<Condition> Conditions
        {
            get
            {
                return Database.Current.SelectAll<Condition>(this.ID, GenericTables.RequirementsCondition, Columns.Condition).AsReadOnly();
            }
        }
        #endregion Property: Conditions

        #region Property: SpatialRequirements
        /// <summary>
        /// Gets the spatial requirement between the subject and other entities.
        /// </summary>
        public Dictionary<EntityCondition, SpatialRequirement> SpatialRequirements
        {
            get
            {
                return Database.Current.SelectAll<EntityCondition, SpatialRequirement>(this.ID, GenericTables.RequirementsSpatialRequirement, Columns.EntityCondition, Columns.SpatialRequirement);
            }
        }
        #endregion Property: SpatialRequirements

        #endregion Properties and Fields

        #region Constructors

        #region Static Constructor: Requirements()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static Requirements()
        {
            // Context types
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.ContextType, new Tuple<Type, EntryType>(typeof(ContextType), EntryType.Intermediate));
            Database.Current.AddTableDefinition(GenericTables.RequirementsContextType, typeof(Requirements), dict);

            // Spatial requirements
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.EntityCondition, new Tuple<Type, EntryType>(typeof(EntityCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.RequirementsSpatialRequirement, typeof(Requirements), dict);
        }
        #endregion Static Constructor: Requirements()

        #region Constructor: Requirements()
        /// <summary>
        /// Creates new requirements.
        /// </summary>
        public Requirements()
            : base()
        {
        }
        #endregion Constructor: Requirements()

        #region Constructor: Requirements(uint id)
        /// <summary>
        /// Creates new requirements from the given ID.
        /// </summary>
        /// <param name="id">The ID to create requirements from.</param>
        private Requirements(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Requirements(uint id)

        #region Constructor: Requirements(Requirements requirements)
        /// <summary>
        /// Clones requirements.
        /// </summary>
        /// <param name="requirements">The requirements to clone.</param>
        public Requirements(Requirements requirements)
            : base()
        {
            if (requirements != null)
            {
                Database.Current.StartChange();

                foreach (ContextType contextType in requirements.ContextTypes)
                    AddContextType(contextType);
                foreach (Condition condition in requirements.Conditions)
                    AddCondition(condition.Clone());
                Dictionary<EntityCondition, SpatialRequirement> spatialRequirements = requirements.SpatialRequirements;
                foreach (EntityCondition entityCondition in spatialRequirements.Keys)
                    SetSpatialRequirement(entityCondition.Clone(), new SpatialRequirement(spatialRequirements[entityCondition]));

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Requirements(Requirements requirements)

        #endregion Constructors

        #region Method Group: Add/Remove

        #region Method: AddContextType(ContextType contextType)
        /// <summary>
        /// Adds a required context type.
        /// </summary>
        /// <param name="contextType">The context type.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddContextType(ContextType contextType)
        {
            if (contextType != null)
            {
                // If the context type is already there, there's no use to add it again
                if (HasContextType(contextType))
                    return Message.RelationExistsAlready;

                // Add the context type
                Database.Current.Insert(this.ID, GenericTables.RequirementsContextType, Columns.ContextType, contextType);
                NotifyPropertyChanged("ContextTypes");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddContextType(ContextType contextType)

        #region Method: RemoveContextType(ContextType contextType)
        /// <summary>
        /// Removes a required context type.
        /// </summary>
        /// <param name="contextType">The context type.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveContextType(ContextType contextType)
        {
            if (contextType != null)
            {
                if (HasContextType(contextType))
                {
                    // Remove the context type
                    Database.Current.Remove(this.ID, GenericTables.RequirementsContextType, Columns.ContextType, contextType);
                    NotifyPropertyChanged("ContextTypes");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveContextType(ContextType contextType)

        #region Method: AddCondition(Condition condition)
        /// <summary>
        /// Adds an extra condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddCondition(Condition condition)
        {
            if (condition != null)
            {
                // If the condition is already there, there's no use to add it again
                if (HasCondition(condition))
                    return Message.RelationExistsAlready;

                // Add the condition
                Database.Current.Insert(this.ID, GenericTables.RequirementsCondition, Columns.Condition, condition);
                NotifyPropertyChanged("Conditions");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddCondition(Condition condition)

        #region Method: RemoveCondition(Condition condition)
        /// <summary>
        /// Removes an extra condition.
        /// </summary>
        /// <param name="condition">The condition.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveCondition(Condition condition)
        {
            if (condition != null)
            {
                if (HasCondition(condition))
                {
                    // Remove the condition
                    Database.Current.Remove(this.ID, GenericTables.RequirementsCondition, Columns.Condition, condition);
                    NotifyPropertyChanged("Conditions");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveCondition(Condition condition)

        #region Method: SetSpatialRequirement(EntityCondition entityCondition, SpatialRequirement spatialRequirement)
        /// <summary>
        /// Assign the given spatial requirement to the given entity.
        /// </summary>
        /// <param name="entityCondition">The entity to assign the spatial requirement to.</param>
        /// <param name="spatialRequirement">The spatial requirement to assign to the entity, can be null.</param>
        /// <returns>Returns whether the relation has been succesfully established.</returns>
        public Message SetSpatialRequirement(EntityCondition entityCondition, SpatialRequirement spatialRequirement)
        {
            if (entityCondition != null)
            {
                // Link the entity and the spatial requirement by removing or updating the existing value, or by inserting it in the database
                if (this.SpatialRequirements.ContainsKey(entityCondition))
                {
                    if (spatialRequirement == null)
                        Database.Current.Remove(this.ID, GenericTables.RequirementsSpatialRequirement, Columns.EntityCondition, entityCondition, false);
                    else
                        Database.Current.Update(this.ID, GenericTables.RequirementsSpatialRequirement, Columns.SpatialRequirement, spatialRequirement);
                }
                else
                    Database.Current.Insert(this.ID, GenericTables.RequirementsSpatialRequirement, new string[] { Columns.EntityCondition, Columns.SpatialRequirement }, new object[] { entityCondition, spatialRequirement });

                NotifyPropertyChanged("SpatialRequirements");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: SetSpatialRequirement(EntityCondition entityCondition, SpatialRequirement spatialRequirement)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasContextType(ContextType contextType)
        /// <summary>
        /// Checks if these requirements have the given required context type.
        /// </summary>
        /// <param name="contextType">The context type to check.</param>
        /// <returns>Returns true when these requirements have the required context type.</returns>
        public bool HasContextType(ContextType contextType)
        {
            if (contextType != null)
            {
                foreach (ContextType myContextType in this.ContextTypes)
                {
                    if (contextType.Equals(myContextType))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasContextType(ContextType contextType)

        #region Method: HasCondition(Condition condition)
        /// <summary>
        /// Checks if these requirements have the given extra condition.
        /// </summary>
        /// <param name="condition">The condition to check.</param>
        /// <returns>Returns true when these requirements have the extra condition.</returns>
        public bool HasCondition(Condition condition)
        {
            if (condition != null)
            {
                foreach (Condition myCondition in this.Conditions)
                {
                    if (condition.Equals(myCondition))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasCondition(Condition condition)

        #region Method: Remove()
        /// <summary>
        /// Remove the requirements.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the context types
            Database.Current.Remove(this.ID, GenericTables.RequirementsContextType);

            // Remove the conditions
            foreach (Condition condition in this.Conditions)
                condition.Remove();
            Database.Current.Remove(this.ID, GenericTables.RequirementsCondition);

            // Remove the spatial requirements
            Dictionary<EntityCondition, SpatialRequirement> spatialRequirements = this.SpatialRequirements;
            foreach (KeyValuePair<EntityCondition, SpatialRequirement> pair in spatialRequirements)
            {
                pair.Key.Remove();
                pair.Value.Remove();
            }
            Database.Current.Remove(this.ID, GenericTables.RequirementsSpatialRequirement);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: Requirements
		
}