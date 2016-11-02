/**************************************************************************
 * 
 * Base.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using Semantics.Components;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: Base
    /// <summary>
    /// A base for an ID holder.
    /// </summary>
    public abstract class Base : PropertyChangedComponent
    {

        #region Event: BaseChanged
        /// <summary>
        /// A handler for changed bases.
        /// </summary>
        /// <param name="newBase">The new base.</param>
        internal delegate void BaseHandler(Base newBase);

        /// <summary>
        /// Invoked when a base is changed.
        /// </summary>
        internal event BaseHandler BaseChanged;
        #endregion Event: BaseChanged

        #region Property: ID
        /// <summary>
        /// Gets the original ID.
        /// </summary>
        public uint ID
        {
            get
            {
                if (idHolder != null)
                    return idHolder.ID;
                return 0;
            }
        }
        #endregion Property: ID

        #region Property: IdHolder
        /// <summary>
        /// The ID holder of which this is a base.
        /// </summary>
        private IdHolder idHolder = null;

        /// <summary>
        /// Gets the ID holder of which this is a base.
        /// </summary>
        protected internal IdHolder IdHolder
        {
            get
            {
                return idHolder;
            }
        }
        #endregion Property: IdHolder

        #region Constructor: Base(IdHolder idHolder)
        /// <summary>
        /// Creates a new base for the given ID holder.
        /// </summary>
        /// <param name="idHolder">The ID holder to create a base for.</param>
        protected internal Base(IdHolder idHolder)
        {
            this.idHolder = idHolder;

            BaseManager.Current.RegisterBase(idHolder, this);
        }
        #endregion Constructor: Base(IdHolder idHolder)

        #region Method: ReplaceBy(Base newBase)
        /// <summary>
        /// Handles the case where this base is replaced by the new base.
        /// </summary>
        /// <param name="newBase">The new base.</param>
        internal void ReplaceBy(Base newBase)
        {
            // Notify the instances of this base by the new base
            if (BaseChanged != null)
                BaseChanged(newBase);
        }
        #endregion Method: ReplaceBy(Base newBase)

        #region Method: Equals(object obj)
        /// <summary>
        /// Check whether the other object is equal to this one.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>Returns whether the other object is equal to this one.</returns>
        public override bool Equals(object obj)
        {
            Base bas = obj as Base;
            if (bas != null)
                return this.ID.Equals(bas.ID);

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

    }
    #endregion Class: Base	

}