/**************************************************************************
 * 
 * CinematicBase.cs
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

    #region Class: CinematicBase
    /// <summary>
    /// A base of a cinematic.
    /// </summary>
    public class CinematicBase : DynamicContentBase
    {

        #region Properties and Fields

        #region Property: Cinematic
        /// <summary>
        /// Gets the cinematic of which this is a cinematic base.
        /// </summary>
        protected internal Cinematic Cinematic
        {
            get
            {
                return this.Node as Cinematic;
            }
        }
        #endregion Property: Cinematic

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: CinematicBase(Cinematic cinematic)
        /// <summary>
        /// Creates a new cinematic base from the given cinematic.
        /// </summary>
        /// <param name="cinematic">The cinematic to create a cinematic base from.</param>
        protected internal CinematicBase(Cinematic cinematic)
            : base(cinematic)
        {
        }
        #endregion Constructor: CinematicBase(Cinematic cinematic)

        #endregion Method Group: Constructors

    }
    #endregion Class: CinematicBase

    #region Class: CinematicValuedBase
    /// <summary>
    /// A base of a valued cinematic.
    /// </summary>
    public class CinematicValuedBase : DynamicContentValuedBase
    {

        #region Properties and Fields

        #region Property: CinematicValued
        /// <summary>
        /// Gets the valued cinematic of which this is a valued cinematic base.
        /// </summary>
        protected internal CinematicValued CinematicValued
        {
            get
            {
                return this.NodeValued as CinematicValued;
            }
        }
        #endregion Property: CinematicValued

        #region Property: CinematicBase
        /// <summary>
        /// Gets the cinematic base.
        /// </summary>
        public CinematicBase CinematicBase
        {
            get
            {
                return this.NodeBase as CinematicBase;
            }
        }
        #endregion Property: CinematicBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: CinematicValuedBase(CinematicValued cinematicValued)
        /// <summary>
        /// Create a valued cinematic base from the given valued cinematic.
        /// </summary>
        /// <param name="cinematicValued">The valued cinematic to create a valued cinematic base from.</param>
        protected internal CinematicValuedBase(CinematicValued cinematicValued)
            : base(cinematicValued)
        {
        }
        #endregion Constructor: CinematicValuedBase(CinematicValued cinematicValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: CinematicValuedBase

    #region Class: CinematicConditionBase
    /// <summary>
    /// A condition on a cinematic.
    /// </summary>
    public class CinematicConditionBase : DynamicContentConditionBase
    {

        #region Properties and Fields

        #region Property: CinematicCondition
        /// <summary>
        /// Gets the cinematic condition of which this is a cinematic condition base.
        /// </summary>
        protected internal CinematicCondition CinematicCondition
        {
            get
            {
                return this.Condition as CinematicCondition;
            }
        }
        #endregion Property: CinematicCondition

        #region Property: CinematicBase
        /// <summary>
        /// Gets the cinematic base of which this is a cinematic condition base.
        /// </summary>
        public CinematicBase CinematicBase
        {
            get
            {
                return this.NodeBase as CinematicBase;
            }
        }
        #endregion Property: CinematicBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: CinematicConditionBase(CinematicCondition cinematicCondition)
        /// <summary>
        /// Creates a base of the given cinematic condition.
        /// </summary>
        /// <param name="cinematicCondition">The cinematic condition to create a base of.</param>
        protected internal CinematicConditionBase(CinematicCondition cinematicCondition)
            : base(cinematicCondition)
        {
        }
        #endregion Constructor: CinematicConditionBase(CinematicCondition cinematicCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: CinematicConditionBase

    #region Class: CinematicChangeBase
    /// <summary>
    /// A change on cinematic.
    /// </summary>
    public class CinematicChangeBase : DynamicContentChangeBase
    {

        #region Properties and Fields

        #region Property: CinematicChange
        /// <summary>
        /// Gets the cinematic change of which this is a cinematic change base.
        /// </summary>
        protected internal CinematicChange CinematicChange
        {
            get
            {
                return this.Change as CinematicChange;
            }
        }
        #endregion Property: CinematicChange

        #region Property: CinematicBase
        /// <summary>
        /// Gets the affected cinematic base.
        /// </summary>
        public CinematicBase CinematicBase
        {
            get
            {
                return this.NodeBase as CinematicBase;
            }
        }
        #endregion Property: CinematicBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: CinematicChangeBase(CinematicChange cinematicChange)
        /// <summary>
        /// Creates a base of the given cinematic change.
        /// </summary>
        /// <param name="cinematicChange">The cinematic change to create a base of.</param>
        protected internal CinematicChangeBase(CinematicChange cinematicChange)
            : base(cinematicChange)
        {
        }
        #endregion Constructor: CinematicChangeBase(CinematicChange cinematicChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: CinematicChangeBase

}