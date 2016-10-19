/**************************************************************************
 * 
 * Reaction.cs
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
using Common;
using Semantics.Data;
using Semantics.Utilities;
using Action = Semantics.Abstractions.Action;

namespace Semantics.Components
{

    #region Class: Reaction
    /// <summary>
    /// A reaction as an effect of an event is the execution of another action by either the actor or target on a(nother) target (in a range).
    /// </summary>
    public sealed class Reaction : Effect
    {

        #region Properties and Fields

        #region Property: Actor
        /// <summary>
        /// Gets or sets the actor of the reaction: the actor or target of the original event.
        /// </summary>
        public ActorTarget Actor
        {
            get
            {
                return Database.Current.Select<ActorTarget>(this.ID, ValueTables.Reaction, Columns.Actor);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Reaction, Columns.Actor, value);
                NotifyPropertyChanged("Actor");
            }
        }
        #endregion Property: Actor

        #region Property: Action
        /// <summary>
        /// Gets or sets the action of the reaction.
        /// </summary>
        public Action Action
        {
            get
            {
                return Database.Current.Select<Action>(this.ID, ValueTables.Reaction, Columns.Action);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Reaction, Columns.Action, value);
                NotifyPropertyChanged("Action");
            }
        }
        #endregion Property: Action

        #region Property: Target
        /// <summary>
        /// Gets or sets the target of the reaction: the actor or target of the original event, or nothing.
        /// </summary>
        public ActorTarget? Target
        {
            get
            {
                return Database.Current.Select<ActorTarget?>(this.ID, ValueTables.Reaction, Columns.Target);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Reaction, Columns.Target, value);
                NotifyPropertyChanged("Target");
            }
        }
        #endregion Property: Target

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: Reaction()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static Reaction()
        {
            // Action
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Action, new Tuple<Type, EntryType>(typeof(Action), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.Reaction, typeof(Reaction), dict);
        }
        #endregion Static Constructor: Reaction()

        #region Constructor: Reaction()
        /// <summary>
        /// Creates a new reaction.
        /// </summary>
        public Reaction()
            : base()
        {
        }
        #endregion Constructor: Reaction()

        #region Constructor: Reaction(uint id)
        /// <summary>
        /// Creates a new reaction from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a reaction from.</param>
        private Reaction(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Reaction(uint id)

        #region Constructor: Reaction(Reaction reaction)
        /// <summary>
        /// Clones a reaction.
        /// </summary>
        /// <param name="reaction">The reaction to clone.</param>
        public Reaction(Reaction reaction)
            : base(reaction)
        {
            if (reaction != null)
            {
                Database.Current.StartChange();

                this.Actor = reaction.Actor;
                this.Action = reaction.Action;
                this.Target = reaction.Target;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Reaction(Reaction reaction)

        #region Constructor: Reaction(Action action)
        /// <summary>
        /// Creates a reaction from an action.
        /// </summary>
        /// <param name="action">The action to create a reaction from.</param>
        public Reaction(Action action)
            : this()
        {
            Database.Current.StartChange();

            this.Action = action;

            Database.Current.StopChange();
        }
        #endregion Constructor: Reaction(Action action) 

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the reaction.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: Reaction

}