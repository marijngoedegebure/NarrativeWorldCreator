/**************************************************************************
 * 
 * ContextType.cs
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
using Semantics.Components;
using Semantics.Data;
using Semantics.Utilities;

namespace Semantics.Abstractions
{

    #region Class: ContextType
    /// <summary>
    /// A context type with contexts.
    /// </summary>
    public class ContextType : Abstraction, IComparable<ContextType>
    {

        #region Properties and Fields

        #region Property: Contexts
        /// <summary>
        /// Gets the personal and inherited contexts.
        /// </summary>
        public ReadOnlyCollection<Context> Contexts
        {
            get
            {
                List<Context> contexts = new List<Context>();
                contexts.AddRange(this.PersonalContexts);
                contexts.AddRange(this.InheritedContexts);
                return contexts.AsReadOnly();
            }
        }
        #endregion Property: Contexts

        #region Property: PersonalContexts
        /// <summary>
        /// Gets the personal contexts of the context type.
        /// </summary>
        public ReadOnlyCollection<Context> PersonalContexts
        {
            get
            {
                return Database.Current.SelectAll<Context>(GenericTables.Context, Columns.ContextType, this).AsReadOnly();
            }
        }
        #endregion Property: PersonalContexts

        #region Property: InheritedContexts
        /// <summary>
        /// Gets the inherited contexts.
        /// </summary>
        public ReadOnlyCollection<Context> InheritedContexts
        {
            get
            {
                // The inherited contexts are the contexts of the children!
                List<Context> inheritedContexts = new List<Context>();
                foreach (Node child in this.PersonalChildren)
                    inheritedContexts.AddRange(((ContextType)child).Contexts);
                return inheritedContexts.AsReadOnly();
            }
        }
        #endregion Property: InheritedContexts

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ContextType()
        /// <summary>
        /// Creates a new context type.
        /// </summary>
        public ContextType()
            : base()
        {
        }
        #endregion Constructor: ContextType()

        #region Constructor: ContextType(uint id)
        /// <summary>
        /// Creates a new context type from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the context type from.</param>
        protected ContextType(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ContextType(uint id)

        #region Constructor: ContextType(string name)
        /// <summary>
        /// Creates a new context type with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the context type.</param>
        public ContextType(string name)
            : base(name)
        {
        }
        #endregion Constructor: ContextType(string name)

        #region Constructor: ContextType(ContextType contextType)
        /// <summary>
        /// Clones a context type.
        /// </summary>
        /// <param name="contextType">The context type to clone.</param>
        public ContextType(ContextType contextType)
            : base(contextType)
        {
            if (contextType != null)
            {
                Database.Current.StartChange();

                foreach (Context context in contextType.PersonalContexts)
                    AddContext(context);

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: ContextType(ContextType contextType)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddContext(Context context)
        /// <summary>
        /// Add a context to the context type.
        /// </summary>
        /// <param name="context">The context to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        internal Message AddContext(Context context)
        {
            if (context != null)
            {
                NotifyPropertyChanged("PersonalContexts");
                NotifyPropertyChanged("Contexts");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddContext(Context context)

        #region Method: RemoveContext(Context context)
        /// <summary>
        /// Remove a context from the context type.
        /// </summary>
        /// <param name="context">The context to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        internal Message RemoveContext(Context context)
        {
            if (context != null)
            {
                if (HasContext(context))
                {
                    NotifyPropertyChanged("PersonalContexts");
                    NotifyPropertyChanged("Contexts");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveContext(Context context)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasContext(Context context)
        /// <summary>
        /// Checks whether the context type has the given context.
        /// </summary>
        /// <param name="context">The context to check.</param>
        /// <returns>Returns true when the context type has the context.</returns>
        public bool HasContext(Context context)
        {
            if (context != null)
            {
                foreach (Context myContext in this.Contexts)
                {
                    if (context.Equals(myContext))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasContext(Context context)

        #region Method: Remove()
        /// <summary>
        /// Remove the context type.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();
            Database.Current.StartRemove();

            // Remove the contexts
            foreach (Context context in this.PersonalContexts)
                context.Remove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(ContextType other)
        /// <summary>
        /// Compares the context type to the other context type.
        /// </summary>
        /// <param name="other">The context type to compare to this context type.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(ContextType other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(ContextType other)

        #endregion Method Group: Other

    }
    #endregion Class: ContextType

}