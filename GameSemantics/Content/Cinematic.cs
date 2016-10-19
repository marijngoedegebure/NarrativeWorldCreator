/**************************************************************************
 * 
 * Cinematic.cs
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
using GameSemantics.Data;

namespace GameSemantics.GameContent
{

    #region Class: Cinematic
    /// <summary>
    /// A cinematic.
    /// </summary>
    public class Cinematic : DynamicContent, IComparable<Cinematic>
    {

        #region Method Group: Constructors

        #region Constructor: Cinematic()
        /// <summary>
        /// Creates a new cinematic.
        /// </summary>
        public Cinematic()
            : base()
        {
        }
        #endregion Constructor: Cinematic()

        #region Constructor: Cinematic(uint id)
        /// <summary>
        /// Creates a new cinematic from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a cinematic from.</param>
        protected Cinematic(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Cinematic(uint id)

        #region Constructor: Cinematic(string file)
        /// <summary>
        /// Creates a new cinematic with the given file.
        /// </summary>
        /// <param name="file">The file to assign to the cinematic.</param>
        public Cinematic(string file)
            : base(file)
        {
        }
        #endregion Constructor: Cinematic(string file)

        #region Constructor: Cinematic(Cinematic cinematic)
        /// <summary>
        /// Clones a cinematic.
        /// </summary>
        /// <param name="cinematic">The cinematic to clone.</param>
        public Cinematic(Cinematic cinematic)
            : base(cinematic)
        {
        }
        #endregion Constructor: Cinematic(Cinematic cinematic)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the cinematic.
        /// </summary>
        public override void Remove()
        {
            GameDatabase.Current.StartChange();
            GameDatabase.Current.StartRemove();

            base.Remove();

            GameDatabase.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(Cinematic other)
        /// <summary>
        /// Compares the cinematic to the other cinematic.
        /// </summary>
        /// <param name="other">The cinematic to compare to this cinematic.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Cinematic other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(Cinematic other)

        #endregion Method Group: Other

    }
    #endregion Class: Cinematic

    #region Class: CinematicValued
    /// <summary>
    /// A valued version of a cinematic.
    /// </summary>
    public class CinematicValued : DynamicContentValued
    {

        #region Properties and Fields

        #region Property: Cinematic
        /// <summary>
        /// Gets the cinematic of which this is a valued cinematic.
        /// </summary>
        public Cinematic Cinematic
        {
            get
            {
                return this.Node as Cinematic;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: Cinematic

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: CinematicValued(uint id)
        /// <summary>
        /// Creates a new valued cinematic from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the valued cinematic from.</param>
        protected CinematicValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: CinematicValued(uint id)

        #region Constructor: CinematicValued(CinematicValued cinematicValued)
        /// <summary>
        /// Clones a valued cinematic.
        /// </summary>
        /// <param name="cinematicValued">The valued cinematic to clone.</param>
        public CinematicValued(CinematicValued cinematicValued)
            : base(cinematicValued)
        {
        }
        #endregion Constructor: CinematicValued(CinematicValued cinematicValued)

        #region Constructor: CinematicValued(Cinematic cinematic)
        /// <summary>
        /// Creates a new valued cinematic from the given cinematic.
        /// </summary>
        /// <param name="cinematic">The cinematic to create a valued cinematic from.</param>
        public CinematicValued(Cinematic cinematic)
            : base(cinematic)
        {
        }
        #endregion Constructor: CinematicValued(Cinematic cinematic)

        #endregion Method Group: Constructors

    }
    #endregion Class: CinematicValued

    #region Class: CinematicCondition
    /// <summary>
    /// A condition on a cinematic.
    /// </summary>
    public class CinematicCondition : DynamicContentCondition
    {

        #region Properties and Fields

        #region Property: Cinematic
        /// <summary>
        /// Gets or sets the required cinematic.
        /// </summary>
        public Cinematic Cinematic
        {
            get
            {
                return this.Node as Cinematic;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Cinematic

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: CinematicCondition()
        /// <summary>
        /// Creates a new cinematic condition.
        /// </summary>
        public CinematicCondition()
            : base()
        {
        }
        #endregion Constructor: CinematicCondition()

        #region Constructor: CinematicCondition(uint id)
        /// <summary>
        /// Creates a new cinematic condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the cinematic condition from.</param>
        protected CinematicCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: CinematicCondition(uint id)

        #region Constructor: CinematicCondition(CinematicCondition cinematicCondition)
        /// <summary>
        /// Clones an cinematic condition.
        /// </summary>
        /// <param name="cinematicCondition">The cinematic condition to clone.</param>
        public CinematicCondition(CinematicCondition cinematicCondition)
            : base(cinematicCondition)
        {
        }
        #endregion Constructor: CinematicCondition(CinematicCondition cinematicCondition)

        #region Constructor: CinematicCondition(Cinematic cinematic)
        /// <summary>
        /// Creates a condition for the given cinematic.
        /// </summary>
        /// <param name="cinematic">The cinematic to create a condition for.</param>
        public CinematicCondition(Cinematic cinematic)
            : base(cinematic)
        {
        }
        #endregion Constructor: CinematicCondition(Cinematic cinematic)

        #endregion Method Group: Constructors

    }
    #endregion Class: CinematicCondition

    #region Class: CinematicChange
    /// <summary>
    /// A change on a cinematic.
    /// </summary>
    public class CinematicChange : DynamicContentChange
    {

        #region Properties and Fields

        #region Property: Cinematic
        /// <summary>
        /// Gets or sets the affected cinematic.
        /// </summary>
        public Cinematic Cinematic
        {
            get
            {
                return this.Node as Cinematic;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Cinematic

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: CinematicChange()
        /// <summary>
        /// Creates a cinematic change.
        /// </summary>
        public CinematicChange()
            : base()
        {
        }
        #endregion Constructor: CinematicChange()

        #region Constructor: CinematicChange(uint id)
        /// <summary>
        /// Creates a new cinematic change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a cinematic change from.</param>
        protected CinematicChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: CinematicChange(uint id)

        #region Constructor: CinematicChange(CinematicChange cinematicChange)
        /// <summary>
        /// Clones a cinematic change.
        /// </summary>
        /// <param name="cinematicChange">The cinematic change to clone.</param>
        public CinematicChange(CinematicChange cinematicChange)
            : base(cinematicChange)
        {
        }
        #endregion Constructor: CinematicChange(CinematicChange cinematicChange)

        #region Constructor: CinematicChange(Cinematic cinematic)
        /// <summary>
        /// Creates a change for the given cinematic.
        /// </summary>
        /// <param name="cinematic">The cinematic to create a change for.</param>
        public CinematicChange(Cinematic cinematic)
            : base(cinematic)
        {
        }
        #endregion Constructor: CinematicChange(Cinematic cinematic)

        #endregion Method Group: Constructors

    }
    #endregion Class: CinematicChange

}