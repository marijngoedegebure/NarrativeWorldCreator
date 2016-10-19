/**************************************************************************
 * 
 * Context.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2010-2011
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

    #region Class: Context
    /// <summary>
    /// A context of a particular context type, having conditions for a subject.
    /// </summary>
    public sealed class Context : IdHolder
    {

        #region Properties and Fields

        #region Property: ContextType
        /// <summary>
        /// Gets the context type of the context.
        /// </summary>
        public ContextType ContextType
        {
            get
            {
                return Database.Current.Select<ContextType>(this.ID, GenericTables.Context, Columns.ContextType);
            }
            private set
            {
                Database.Current.Update(this.ID, GenericTables.Context, Columns.ContextType, value);
                NotifyPropertyChanged("ContextType");
            }
        }
        #endregion Property: ContextType

        #region Property: Subject
        /// <summary>
        /// Gets or sets the subject of the context.
        /// </summary>
        public Entity Subject
        {
            get
            {
                return Database.Current.Select<Entity>(this.ID, GenericTables.Context, Columns.Subject);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Context, Columns.Subject, value);
                NotifyPropertyChanged("Subject");
            }
        }
        #endregion Property: Subject

        #region Property: Conditions
        /// <summary>
        /// Gets the conditions in the context.
        /// </summary>
        public ReadOnlyCollection<Condition> Conditions
        {
            get
            {
                return Database.Current.SelectAll<Condition>(this.ID, GenericTables.ContextCondition, Columns.Condition).AsReadOnly();
            }
        }
        #endregion Property: Conditions

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: Context()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static Context()
        {
            // Context type and subject
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.ContextType, new Tuple<Type, EntryType>(typeof(ContextType), EntryType.Nullable));
            dict.Add(Columns.Subject, new Tuple<Type, EntryType>(typeof(Entity), EntryType.Nullable));
            Database.Current.AddTableDefinition(GenericTables.Context, typeof(Context), dict);

            // Conditions
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Condition, new Tuple<Type, EntryType>(typeof(Condition), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.ContextCondition, typeof(Context), dict);
        }
        #endregion Static Constructor: Context()

        #region Constructor: Context(uint id)
        /// <summary>
        /// Creates a new context from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a context from.</param>
        private Context(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Context(uint id)

        #region Constructor: Context(Context context)
        /// <summary>
        /// Clones a context.
        /// </summary>
        /// <param name="context">The context to clone.</param>
        public Context(Context context)
            : base()
        {
            if (context != null)
            {
                Database.Current.StartChange();

                this.Subject = context.Subject;
                foreach (Condition condition in context.Conditions)
                    AddCondition(condition.Clone());

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Context(Context context)

        #region Constructor: Context(ContextType contextType)
        /// <summary>
        /// Creates a context from the given context type.
        /// </summary>
        /// <param name="contextType">The context type to create a context from.</param>
        public Context(ContextType contextType)
            : base()
        {
            if (contextType != null)
            {
                Database.Current.StartChange();

                this.ContextType = contextType;
                contextType.AddContext(this);

                Database.Current.StopChange();
            }
            else
                Remove();
        }
        #endregion Constructor: Context(ContextType contextType)

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
                Database.Current.Insert(this.ID, GenericTables.ContextCondition, Columns.Condition, condition);
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
                    Database.Current.Remove(this.ID, GenericTables.ContextCondition, Columns.Condition, condition);
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
        /// Checks if this context has the given condition.
        /// </summary>
        /// <param name="condition">The condition to check.</param>
        /// <returns>Returns true when this context has the condition.</returns>
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
        /// Remove the context.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the context from the context type
            if (this.ContextType != null)
                this.ContextType.RemoveContext(this);

            // Remove the conditions
            foreach (Condition condition in this.Conditions)
                condition.Remove();
            Database.Current.Remove(this.ID, GenericTables.ContextCondition);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(Context other)
        /// <summary>
        /// Compares the context to the other context.
        /// </summary>
        /// <param name="other">The context to compare to this context.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Context other)
        {
            if (this == other)
                return 0;
            if (other != null)
                return this.ID.CompareTo(other.ID);
            return 0;
        }
        #endregion Method: CompareTo(Context other)

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString()
        {
            if (this.ContextType != null)
                return this.ContextType.ToString();

            return base.ToString();
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: Context

}
