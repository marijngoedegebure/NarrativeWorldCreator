/**************************************************************************
 * 
 * Family.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2011
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
using Semantics.Utilities;

namespace Semantics.Abstractions
{

    #region Class: Family
    /// <summary>
    /// A family is a collection of (derived) entities that meet certain conditions.
    /// </summary>
    public class Family : Abstraction, IComparable<Family>
    {

        #region Properties and Fields

        #region Property: Conditions
        /// <summary>
        /// Gets the conditions of the family.
        /// </summary>
        public ReadOnlyCollection<Condition> Conditions
        {
            get
            {
                return Database.Current.SelectAll<Condition>(this.ID, GenericTables.FamilyCondition, Columns.Condition).AsReadOnly();
            }
        }
        #endregion Property: Conditions

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: Family()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static Family()
        {
            // Conditions
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Condition, new Tuple<Type, EntryType>(typeof(Condition), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.FamilyCondition, typeof(Family), dict);
        }
        #endregion Static Constructor: Family()

        #region Constructor: Family()
        /// <summary>
        /// Creates a new family.
        /// </summary>
        public Family()
            : base()
        {
        }
        #endregion Constructor: Family()

        #region Constructor: Family(uint id)
        /// <summary>
        /// Creates a new family from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new family from.</param>
        protected Family(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Family(uint id)

        #region Constructor: Family(string name)
        /// <summary>
        /// Creates a new family with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the family.</param>
        public Family(string name)
            : base(name)
        {
        }
        #endregion Constructor: Family(string name)

        #region Constructor: Family(Family family)
        /// <summary>
        /// Clones a family.
        /// </summary>
        /// <param name="family">The family to clone.</param>
        public Family(Family family)
            : base(family)
        {
            if (family != null)
            {
                Database.Current.StartChange();

                foreach (Condition condition in family.Conditions)
                    AddCondition(condition.Clone());

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Family(Family family)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddCondition(Condition condition)
        /// <summary>
        /// Adds the given condition.
        /// </summary>
        /// <param name="condition">The condition to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddCondition(Condition condition)
        {
            if (condition != null)
            {
                // If the condition is already available in all conditions, there is no use to add it
                if (HasCondition(condition))
                    return Message.RelationExistsAlready;

                // Add the condition
                Database.Current.Insert(this.ID, GenericTables.FamilyCondition, Columns.Condition, condition);
                NotifyPropertyChanged("Conditions");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddCondition(Condition condition)

        #region Method: RemoveCondition(Condition condition)
        /// <summary>
        /// Removes a condition.
        /// </summary>
        /// <param name="condition">The condition to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveCondition(Condition condition)
        {
            if (condition != null)
            {
                if (HasCondition(condition))
                {
                    // Remove the condition
                    Database.Current.Remove(this.ID, GenericTables.FamilyCondition, Columns.Condition, condition);
                    NotifyPropertyChanged("Conditions");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveCondition(Condition condition)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasCondition(Condition condition)
        /// <summary>
        /// Checks if this family has the given condition.
        /// </summary>
        /// <param name="condition">The condition to check.</param>
        /// <returns>Returns true when this family has the condition.</returns>
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
        /// Remove the family.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();
            Database.Current.StartRemove();

            // Remove the conditions
            foreach (Condition condition in this.Conditions)
                condition.Remove();
            Database.Current.Remove(this.ID, GenericTables.FamilyCondition);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(Family other)
        /// <summary>
        /// Compares the family to the other family.
        /// </summary>
        /// <param name="other">The family to compare to this family.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Family other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(Family other)

        #endregion Method Group: Other

    }
    #endregion Class: Family

}