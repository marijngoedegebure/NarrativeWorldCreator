/**************************************************************************
 * 
 * GoalBase.cs
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
using Semantics.Abstractions;
using Semantics.Components;
using SemanticsEngine.Components;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Abstractions
{

    #region Class: GoalBase
    /// <summary>
    /// A base of a goal.
    /// </summary>
    public class GoalBase : AbstractionBase
    {

        #region Properties and Fields

        #region Property: Goal
        /// <summary>
        /// Gets the goal of which this is a goal base.
        /// </summary>
        protected internal Goal Goal
        {
            get
            {
                return this.IdHolder as Goal;
            }
        }
        #endregion Property: Goal

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
                if (changes == null)
                {
                    LoadChanges();
                    if (changes == null)
                        return new List<ChangeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<ChangeBase>(changes);
            }
        }

        /// <summary>
        /// Loads the changes.
        /// </summary>
        private void LoadChanges()
        {
            if (this.Goal != null)
            {
                List<ChangeBase> goalChanges = new List<ChangeBase>();
                foreach (Change change in this.Goal.Changes)
                    goalChanges.Add(BaseManager.Current.GetBase<ChangeBase>(change));
                changes = goalChanges.ToArray();
            }
        }
        #endregion Property: Changes

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: GoalBase(Goal goal)
        /// <summary>
        /// Creates a base of a goal.
        /// </summary>
        /// <param name="goal">The goal to create a base of.</param>
        protected internal GoalBase(Goal goal)
            : base(goal)
        {
            if (goal != null)
            {
                if (BaseManager.PreloadProperties)
                    LoadChanges();
            }
        }
        #endregion Constructor: GoalBase(Goal goal)

        #endregion Method Group: Constructors

    }
    #endregion Class: GoalBase

}