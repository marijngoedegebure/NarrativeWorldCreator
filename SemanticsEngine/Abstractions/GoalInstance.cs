/**************************************************************************
 * 
 * GoalInstance.cs
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
using System.Collections.ObjectModel;
using Semantics.Utilities;
using SemanticsEngine.Components;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Abstractions
{

    #region Class: GoalInstance
    /// <summary>
    /// An instance of a goal.
    /// </summary>
    public class GoalInstance : AbstractionInstance
    {

        #region Properties and Fields

        #region Property: GoalBase
        /// <summary>
        /// Gets the goal base of which this is a goal instance.
        /// </summary>
        public GoalBase GoalBase
        {
            get
            {
                return this.NodeBase as GoalBase;
            }
        }
        #endregion Property: GoalBase

        #region Property: Changes
        /// <summary>
        /// The changes that are pursued by this goal.
        /// </summary>
        private ChangeBase[] changes = null;
        
        /// <summary>
        /// Gets the changes that are pursued by this goal.
        /// </summary>
        public ReadOnlyCollection<ChangeBase> Changes
        {
            get
            {
                // Add the changes defined for this instance
                List<ChangeBase> changes = null;
                if (changes != null)
                    changes = new List<ChangeBase>(changes);
                else
                    changes = new List<ChangeBase>();

                // Add the changes of the base
                if (this.GoalBase != null)
                    changes.AddRange(this.GoalBase.Changes);

                return changes.AsReadOnly();
            }
        }
        #endregion Property: Changes

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: GoalInstance(GoalBase goalBase)
        /// <summary>
        /// Creates a new goal instance from the given goal base.
        /// </summary>
        /// <param name="goalBase">The goal base to create the goal instance from.</param>
        internal GoalInstance(GoalBase goalBase)
            : base(goalBase)
        {
        }
        #endregion Constructor: GoalInstance(GoalBase goalBase)

        #region Constructor: GoalInstance(GoalInstance goalInstance)
        /// <summary>
        /// Clones a goal instance.
        /// </summary>
        /// <param name="goalInstance">The goal instance to clone.</param>
        protected internal GoalInstance(GoalInstance goalInstance)
            : base(goalInstance)
        {
            if (goalInstance != null)
            {
                foreach (ChangeBase changeBase in goalInstance.Changes)
                    AddChangeBase(changeBase);
            }
        }
        #endregion Constructor: GoalInstance(GoalInstance goalInstance)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddChangeBase(ChangeBase changeBase)
        /// <summary>
        /// Add a new change to the goal.
        /// </summary>
        /// <param name="changeBase">The change to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddChangeBase(ChangeBase changeBase)
        {
            if (changeBase != null)
            {
                // If the change is already available in all changes, there is no use to add it
                if (HasChange(changeBase))
                    return Message.RelationExistsAlready;

                // Add the change to the changes
                Utils.AddToArray<ChangeBase>(ref this.changes, changeBase);
                NotifyPropertyChanged("Changes");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddChangeBase(ChangeBase changeBase)

        #region Method: RemoveChangeBase(ChangeBase changeBase)
        /// <summary>
        /// Remove a change from the goal.
        /// </summary>
        /// <param name="changeBase">The change to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveChangeBase(ChangeBase changeBase)
        {
            if (changeBase != null)
            {
                if (HasChange(changeBase))
                {
                    // Remove the change
                    Utils.RemoveFromArray<ChangeBase>(ref this.changes, changeBase);
                    NotifyPropertyChanged("Changes");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveChangeBase(ChangeBase changeBase)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasChange(ChangeBase change)
        /// <summary>
        /// Checks whether the goal has the given change.
        /// </summary>
        /// <param name="change">The change to check.</param>
        /// <returns>Returns true when the goal has the change.</returns>
        public bool HasChange(ChangeBase changeBase)
        {
            if (changeBase != null)
            {
                foreach (ChangeBase myChangeBase in this.Changes)
                {
                    if (changeBase.Equals(myChangeBase))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasChange(ChangeBase change)

        #endregion Method Group: Other

    }
    #endregion Class: GoalInstance

}