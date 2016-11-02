/**************************************************************************
 * 
 * ParticlePropertiesBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using GameSemantics.GameContent;

namespace GameSemanticsEngine.GameContent
{

    #region Class: ParticlePropertiesBase
    /// <summary>
    /// A base of particle properties.
    /// </summary>
    public class ParticlePropertiesBase : StaticContentBase
    {

        #region Properties and Fields

        #region Property: ParticleProperties
        /// <summary>
        /// Gets the particle properties of which this is a particle properties base.
        /// </summary>
        protected internal ParticleProperties ParticleProperties
        {
            get
            {
                return this.Node as ParticleProperties;
            }
        }
        #endregion Property: ParticleProperties

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ParticlePropertiesBase(ParticleProperties particleProperties)
        /// <summary>
        /// Creates a new particle properties base from the given particle properties.
        /// </summary>
        /// <param name="particleProperties">The particle properties to create a particle properties base from.</param>
        protected internal ParticlePropertiesBase(ParticleProperties particleProperties)
            : base(particleProperties)
        {
        }
        #endregion Constructor: ParticlePropertiesBase(ParticleProperties particleProperties)

        #endregion Method Group: Constructors

    }
    #endregion Class: ParticlePropertiesBase

    #region Class: ParticlePropertiesValuedBase
    /// <summary>
    /// A base of valued particle properties.
    /// </summary>
    public class ParticlePropertiesValuedBase : StaticContentValuedBase
    {

        #region Properties and Fields

        #region Property: ParticlePropertiesValued
        /// <summary>
        /// Gets the valued particle properties of which this is a valued particle properties base.
        /// </summary>
        protected internal ParticlePropertiesValued ParticlePropertiesValued
        {
            get
            {
                return this.NodeValued as ParticlePropertiesValued;
            }
        }
        #endregion Property: ParticlePropertiesValued

        #region Property: ParticlePropertiesBase
        /// <summary>
        /// Gets the particle properties base.
        /// </summary>
        public ParticlePropertiesBase ParticlePropertiesBase
        {
            get
            {
                return this.NodeBase as ParticlePropertiesBase;
            }
        }
        #endregion Property: ParticlePropertiesBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ParticlePropertiesValuedBase(ParticlePropertiesValued particlePropertiesValued)
        /// <summary>
        /// Create a valued particle properties base from the given valued particle properties.
        /// </summary>
        /// <param name="particlePropertiesValued">The valued particle properties to create a valued particle properties base from.</param>
        protected internal ParticlePropertiesValuedBase(ParticlePropertiesValued particlePropertiesValued)
            : base(particlePropertiesValued)
        {
        }
        #endregion Constructor: ParticlePropertiesValuedBase(ParticlePropertiesValued particlePropertiesValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: ParticlePropertiesValuedBase

    #region Class: ParticlePropertiesConditionBase
    /// <summary>
    /// A condition on particle properties.
    /// </summary>
    public class ParticlePropertiesConditionBase : StaticContentConditionBase
    {

        #region Properties and Fields

        #region Property: ParticlePropertiesCondition
        /// <summary>
        /// Gets the particle properties condition of which this is a particle properties condition base.
        /// </summary>
        protected internal ParticlePropertiesCondition ParticlePropertiesCondition
        {
            get
            {
                return this.Condition as ParticlePropertiesCondition;
            }
        }
        #endregion Property: ParticlePropertiesCondition

        #region Property: ParticlePropertiesBase
        /// <summary>
        /// Gets the particle properties base of which this is a particle properties condition base.
        /// </summary>
        public ParticlePropertiesBase ParticlePropertiesBase
        {
            get
            {
                return this.NodeBase as ParticlePropertiesBase;
            }
        }
        #endregion Property: ParticlePropertiesBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ParticlePropertiesConditionBase(ParticlePropertiesCondition particlePropertiesCondition)
        /// <summary>
        /// Creates a base of the given particle properties condition.
        /// </summary>
        /// <param name="particlePropertiesCondition">The particle properties condition to create a base of.</param>
        protected internal ParticlePropertiesConditionBase(ParticlePropertiesCondition particlePropertiesCondition)
            : base(particlePropertiesCondition)
        {
        }
        #endregion Constructor: ParticlePropertiesConditionBase(ParticlePropertiesCondition particlePropertiesCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: ParticlePropertiesConditionBase

    #region Class: ParticlePropertiesChangeBase
    /// <summary>
    /// A change on particle properties.
    /// </summary>
    public class ParticlePropertiesChangeBase : StaticContentChangeBase
    {

        #region Properties and Fields

        #region Property: ParticlePropertiesChange
        /// <summary>
        /// Gets the particle properties change of which this is a particle properties change base.
        /// </summary>
        protected internal ParticlePropertiesChange ParticlePropertiesChange
        {
            get
            {
                return this.Change as ParticlePropertiesChange;
            }
        }
        #endregion Property: ParticlePropertiesChange

        #region Property: ParticlePropertiesBase
        /// <summary>
        /// Gets the affected particle properties base.
        /// </summary>
        public ParticlePropertiesBase ParticlePropertiesBase
        {
            get
            {
                return this.NodeBase as ParticlePropertiesBase;
            }
        }
        #endregion Property: ParticlePropertiesBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ParticlePropertiesChangeBase(ParticlePropertiesChange particlePropertiesChange)
        /// <summary>
        /// Creates a base of the given particle properties change.
        /// </summary>
        /// <param name="particlePropertiesChange">The particle properties change to create a base of.</param>
        protected internal ParticlePropertiesChangeBase(ParticlePropertiesChange particlePropertiesChange)
            : base(particlePropertiesChange)
        {
        }
        #endregion Constructor: ParticlePropertiesChangeBase(ParticlePropertiesChange particlePropertiesChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: ParticlePropertiesChangeBase

}