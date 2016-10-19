/**************************************************************************
 * 
 * ParticleProperties.cs
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
using Common;
using GameSemantics.Data;

namespace GameSemantics.GameContent
{

    #region Class: ParticleProperties
    /// <summary>
    /// Particle properties.
    /// </summary>
    public class ParticleProperties : StaticContent, IComparable<ParticleProperties>
    {

        #region Method Group: Constructors

        #region Constructor: ParticleProperties()
        /// <summary>
        /// Creates a new particle properties.
        /// </summary>
        public ParticleProperties()
            : base()
        {
        }
        #endregion Constructor: ParticleProperties()

        #region Constructor: ParticleProperties(uint id)
        /// <summary>
        /// Creates a new particle properties from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a particle properties from.</param>
        protected ParticleProperties(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ParticleProperties(uint id)

        #region Constructor: ParticleProperties(string file)
        /// <summary>
        /// Creates new particle properties with the given file.
        /// </summary>
        /// <param name="file">The file to assign to the particle properties.</param>
        public ParticleProperties(string file)
            : base(file)
        {
        }
        #endregion Constructor: ParticleProperties(string file)

        #region Constructor: ParticleProperties(ParticleProperties particleProperties)
        /// <summary>
        /// Clones a particle properties.
        /// </summary>
        /// <param name="particleProperties">The particle properties to clone.</param>
        public ParticleProperties(ParticleProperties particleProperties)
            : base(particleProperties)
        {
        }
        #endregion Constructor: ParticleProperties(ParticleProperties particleProperties)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the particle properties.
        /// </summary>
        public override void Remove()
        {
            GameDatabase.Current.StartChange();
            GameDatabase.Current.StartRemove();

            base.Remove();

            GameDatabase.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(ParticleProperties other)
        /// <summary>
        /// Compares the particle properties to the other particle properties.
        /// </summary>
        /// <param name="other">The particle properties to compare to this particle properties.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(ParticleProperties other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(ParticleProperties other)

        #endregion Method Group: Other

    }
    #endregion Class: ParticleProperties

    #region Class: ParticlePropertiesValued
    /// <summary>
    /// A valued version of a particle properties.
    /// </summary>
    public class ParticlePropertiesValued : StaticContentValued
    {

        #region Properties and Fields

        #region Property: ParticleProperties
        /// <summary>
        /// Gets the particle properties of which this is a valued particle properties.
        /// </summary>
        public ParticleProperties ParticleProperties
        {
            get
            {
                return this.Node as ParticleProperties;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: ParticleProperties

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ParticlePropertiesValued(uint id)
        /// <summary>
        /// Creates a new valued particle properties from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the valued particle properties from.</param>
        protected ParticlePropertiesValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ParticlePropertiesValued(uint id)

        #region Constructor: ParticlePropertiesValued(ParticlePropertiesValued particlePropertiesValued)
        /// <summary>
        /// Clones a valued particle properties.
        /// </summary>
        /// <param name="particlePropertiesValued">The valued particle properties to clone.</param>
        public ParticlePropertiesValued(ParticlePropertiesValued particlePropertiesValued)
            : base(particlePropertiesValued)
        {
        }
        #endregion Constructor: ParticlePropertiesValued(ParticlePropertiesValued particlePropertiesValued)

        #region Constructor: ParticlePropertiesValued(ParticleProperties particleProperties)
        /// <summary>
        /// Creates new valued particle properties from the given particle properties.
        /// </summary>
        /// <param name="particleProperties">The particle properties to create valued particle properties from.</param>
        public ParticlePropertiesValued(ParticleProperties particleProperties)
            : base(particleProperties)
        {
        }
        #endregion Constructor: ParticlePropertiesValued(ParticleProperties particleProperties)

        #endregion Method Group: Constructors

    }
    #endregion Class: ParticlePropertiesValued

    #region Class: ParticlePropertiesCondition
    /// <summary>
    /// A condition on particle properties.
    /// </summary>
    public class ParticlePropertiesCondition : StaticContentCondition
    {

        #region Properties and Fields

        #region Property: ParticleProperties
        /// <summary>
        /// Gets or sets the required particle properties.
        /// </summary>
        public ParticleProperties ParticleProperties
        {
            get
            {
                return this.Node as ParticleProperties;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: ParticleProperties

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ParticlePropertiesCondition()
        /// <summary>
        /// Creates a new particle properties condition.
        /// </summary>
        public ParticlePropertiesCondition()
            : base()
        {
        }
        #endregion Constructor: ParticlePropertiesCondition()

        #region Constructor: ParticlePropertiesCondition(uint id)
        /// <summary>
        /// Creates a new particle properties condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the particle properties condition from.</param>
        protected ParticlePropertiesCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ParticlePropertiesCondition(uint id)

        #region Constructor: ParticlePropertiesCondition(ParticlePropertiesCondition particlePropertiesCondition)
        /// <summary>
        /// Clones a particle properties condition.
        /// </summary>
        /// <param name="particlePropertiesCondition">The particle properties condition to clone.</param>
        public ParticlePropertiesCondition(ParticlePropertiesCondition particlePropertiesCondition)
            : base(particlePropertiesCondition)
        {
        }
        #endregion Constructor: ParticlePropertiesCondition(ParticlePropertiesCondition particlePropertiesCondition)

        #region Constructor: ParticlePropertiesCondition(ParticleProperties particleProperties)
        /// <summary>
        /// Creates a condition for the given particle properties.
        /// </summary>
        /// <param name="particleProperties">The particle properties to create a condition for.</param>
        public ParticlePropertiesCondition(ParticleProperties particleProperties)
            : base(particleProperties)
        {
        }
        #endregion Constructor: ParticlePropertiesCondition(ParticleProperties particleProperties)

        #endregion Method Group: Constructors

    }
    #endregion Class: ParticlePropertiesCondition

    #region Class: ParticlePropertiesChange
    /// <summary>
    /// A change on particle properties.
    /// </summary>
    public class ParticlePropertiesChange : StaticContentChange
    {

        #region Properties and Fields

        #region Property: ParticleProperties
        /// <summary>
        /// Gets or sets the affected particle properties.
        /// </summary>
        public ParticleProperties ParticleProperties
        {
            get
            {
                return this.Node as ParticleProperties;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: ParticleProperties

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ParticlePropertiesChange()
        /// <summary>
        /// Creates a particle properties change.
        /// </summary>
        public ParticlePropertiesChange()
            : base()
        {
        }
        #endregion Constructor: ParticlePropertiesChange()

        #region Constructor: ParticlePropertiesChange(uint id)
        /// <summary>
        /// Creates a new particle properties change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create an particle properties change from.</param>
        protected ParticlePropertiesChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ParticlePropertiesChange(uint id)

        #region Constructor: ParticlePropertiesChange(ParticlePropertiesChange particlePropertiesChange)
        /// <summary>
        /// Clones a particle properties change.
        /// </summary>
        /// <param name="particlePropertiesChange">The particle properties change to clone.</param>
        public ParticlePropertiesChange(ParticlePropertiesChange particlePropertiesChange)
            : base(particlePropertiesChange)
        {
        }
        #endregion Constructor: ParticlePropertiesChange(ParticlePropertiesChange particlePropertiesChange)

        #region Constructor: ParticlePropertiesChange(ParticleProperties particleProperties)
        /// <summary>
        /// Creates a change for the given particle properties.
        /// </summary>
        /// <param name="particleProperties">The particle properties to create a change for.</param>
        public ParticlePropertiesChange(ParticleProperties particleProperties)
            : base(particleProperties)
        {
        }
        #endregion Constructor: ParticlePropertiesChange(ParticleProperties particleProperties)

        #endregion Method Group: Constructors

    }
    #endregion Class: ParticlePropertiesChange

}