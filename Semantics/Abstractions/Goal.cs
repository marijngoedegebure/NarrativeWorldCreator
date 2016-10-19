/**************************************************************************
 * 
 * Goal.cs
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
using Semantics.Utilities;

namespace Semantics.Abstractions
{

    #region Class: Goal
    /// <summary>
    /// A goal.
    /// </summary>
    public class Goal : Abstraction, IComparable<Goal>
    {

        #region Properties and Fields

        #region Property: Changes
        /// <summary>
        /// Gets the changes that are pursued by this goal.
        /// </summary>
        public ReadOnlyCollection<Change> Changes
        {
            get
            {
                return Database.Current.SelectAll<Change>(this.ID, GenericTables.GoalChange, Columns.Change).AsReadOnly();
            }
        }
        #endregion Property: Changes
		
        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: Goal()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static Goal()
        {
            // Changes
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Change, new Tuple<Type, EntryType>(typeof(Change), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.GoalChange, typeof(Goal), dict);
        }
        #endregion Static Constructor: Goal()

        #region Constructor: Goal()
        /// <summary>
        /// Creates a new goal.
        /// </summary>
        public Goal()
            : base()
        {
        }
        #endregion Constructor: Goal()

        #region Constructor: Goal(uint id)
        /// <summary>
        /// Creates a new goal from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the goal from.</param>
        protected Goal(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Goal(uint id)

        #region Constructor: Goal(string name)
        /// <summary>
        /// Creates a new goal with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the goal.</param>
        public Goal(string name)
            : base(name)
        {
        }
        #endregion Constructor: Goal(string name)

        #region Constructor: Goal(Goal goal)
        /// <summary>
        /// Clones a goal.
        /// </summary>
        /// <param name="goal">The goal to clone.</param>
        public Goal(Goal goal)
            : base(goal)
        {
            if (goal != null)
            {
                Database.Current.StartChange();

                foreach (Change change in this.Changes)
                    AddChange(change.Clone());

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Goal(Goal goal)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddChange(Change change)
        /// <summary>
        /// Add a new change to the goal.
        /// </summary>
        /// <param name="change">The change to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddChange(Change change)
        {
            if (change != null)
            {
                // If the change is already available in all changes, there is no use to add it
                if (HasChange(change))
                    return Message.RelationExistsAlready;

                // Add the change to the changes
                Database.Current.Insert(this.ID, GenericTables.GoalChange, Columns.Change, change);
                NotifyPropertyChanged("Changes");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddChange(Change change)

        #region Method: RemoveChange(Change change)
        /// <summary>
        /// Remove a change from the goal.
        /// </summary>
        /// <param name="change">The change to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveChange(Change change)
        {
            if (change != null)
            {
                if (HasChange(change))
                {
                    // Remove the change
                    Database.Current.Remove(this.ID, GenericTables.GoalChange, Columns.Change, change);
                    NotifyPropertyChanged("Changes");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveChange(Change change)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasChange(Change change)
        /// <summary>
        /// Checks whether the goal has the given change.
        /// </summary>
        /// <param name="change">The change to check.</param>
        /// <returns>Returns true when the goal has the change.</returns>
        public bool HasChange(Change change)
        {
            if (change != null)
            {
                foreach (Change myChange in this.Changes)
                {
                    if (change.Equals(myChange))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasChange(Change change)

        #region Method: Remove()
        /// <summary>
        /// Remove the goal.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();
            Database.Current.StartRemove();

            // Remove the changes
            foreach (Change change in this.Changes)
                change.Remove();
            Database.Current.Remove(this.ID, GenericTables.GoalChange);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(Goal other)
        /// <summary>
        /// Compares the goal to the other goal.
        /// </summary>
        /// <param name="other">The goal to compare to this goal.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Goal other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(Goal other)

        #endregion Method Group: Other

    }
    #endregion Class: Goal

}