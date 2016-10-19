/**************************************************************************
 * 
 * IdHolder.cs
 * 
 * Jassin Kessing & Tim Tutenel
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2010-2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using Semantics.Data;

namespace Semantics.Components
{

    #region Class: IdHolder
    /// <summary>
    /// An abstract class to hold an ID.
    /// </summary>
    public abstract class IdHolder : PropertyChangedComponent
    {

        #region Properties and Fields

        #region Property: ID
        /// <summary>
        /// The ID.
        /// </summary>
        private uint id = 0;

        /// <summary>
        /// Gets the ID.
        /// </summary>
        public uint ID
        {
            get
            {
                return id;
            }
            internal set
            {
                id = value;
            }
        }
        #endregion Property: ID

        #region Property: Type
        /// <summary>
        /// Gets the System.Type of the current instance.
        /// </summary>
        public Type Type
        {
            get
            {
                return this.GetType();
            }
        }
        #endregion Property: Type

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: IdHolder()
        /// <summary>
        /// Creates a new ID holder.
        /// </summary>
        protected IdHolder()
            : base()
        {
            Database.Current.StartChange();

            Database.Current.Add(this);

            Database.Current.StopChange();
        }
        #endregion Constructor: IdHolder()

        #region Constructor: IdHolder(string name)
        /// <summary>
        /// Creates a new ID holder with the given name.
        /// </summary>
        protected IdHolder(string name)
            : base()
        {
            Database.Current.StartChange();

            Database.Current.Add(this, name);

            Database.Current.StopChange();
        }
        #endregion Constructor: IdHolder(string name)

        #region Constructor: IdHolder(uint id)
        /// <summary>
        /// Creates a new ID holder with the given ID.
        /// </summary>
        /// <param name="id">The ID to create the ID holder with.</param>
        protected IdHolder(uint id)
            : base()
        {
            this.id = id;
        }
        #endregion Constructor: IdHolder(int id)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Equals(object obj)
        /// <summary>
        /// Check whether the other object is equal to this one.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>Returns whether the other object is equal to this one.</returns>
        public override bool Equals(object obj)
        {
            IdHolder idHolder = obj as IdHolder;
            if (idHolder != null)
                return this.ID.Equals(idHolder.ID);

            return false;
        }
        #endregion Method: Equals(object obj)

        #region Method: GetHashCode()
        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        #endregion Method: GetHashCode()

        #region Method: Remove()
        /// <summary>
        /// Remove the ID holder.
        /// </summary>
        public virtual void Remove()
        {
            Database.Current.StartChange();

            // Remove the ID holder from the database
            Database.Current.Remove(this);

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: IdHolder

}
