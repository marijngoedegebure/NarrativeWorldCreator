/**************************************************************************
 * 
 * ScriptBase.cs
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

    #region Class: ScriptBase
    /// <summary>
    /// A base of a script.
    /// </summary>
    public class ScriptBase : DynamicContentBase
    {

        #region Properties and Fields

        #region Property: Script
        /// <summary>
        /// Gets the script of which this is a script base.
        /// </summary>
        protected internal Script Script
        {
            get
            {
                return this.Node as Script;
            }
        }
        #endregion Property: Script

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ScriptBase(Script script)
        /// <summary>
        /// Creates a new script base from the given script.
        /// </summary>
        /// <param name="script">The script to create a script base from.</param>
        protected internal ScriptBase(Script script)
            : base(script)
        {
        }
        #endregion Constructor: ScriptBase(Script script)

        #endregion Method Group: Constructors

    }
    #endregion Class: ScriptBase

    #region Class: ScriptValuedBase
    /// <summary>
    /// A base of a valued script.
    /// </summary>
    public class ScriptValuedBase : DynamicContentValuedBase
    {

        #region Properties and Fields

        #region Property: ScriptValued
        /// <summary>
        /// Gets the valued script of which this is a valued script base.
        /// </summary>
        protected internal ScriptValued ScriptValued
        {
            get
            {
                return this.NodeValued as ScriptValued;
            }
        }
        #endregion Property: ScriptValued

        #region Property: ScriptBase
        /// <summary>
        /// Gets the script base.
        /// </summary>
        public ScriptBase ScriptBase
        {
            get
            {
                return this.NodeBase as ScriptBase;
            }
        }
        #endregion Property: ScriptBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ScriptValuedBase(ScriptValued scriptValued)
        /// <summary>
        /// Create a valued script base from the given valued script.
        /// </summary>
        /// <param name="scriptValued">The valued script to create a valued script base from.</param>
        protected internal ScriptValuedBase(ScriptValued scriptValued)
            : base(scriptValued)
        {
        }
        #endregion Constructor: ScriptValuedBase(ScriptValued scriptValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: ScriptValuedBase

    #region Class: ScriptConditionBase
    /// <summary>
    /// A condition on a script.
    /// </summary>
    public class ScriptConditionBase : DynamicContentConditionBase
    {

        #region Properties and Fields

        #region Property: ScriptCondition
        /// <summary>
        /// Gets the script condition of which this is a script condition base.
        /// </summary>
        protected internal ScriptCondition ScriptCondition
        {
            get
            {
                return this.Condition as ScriptCondition;
            }
        }
        #endregion Property: ScriptCondition

        #region Property: ScriptBase
        /// <summary>
        /// Gets the script base of which this is a script condition base.
        /// </summary>
        public ScriptBase ScriptBase
        {
            get
            {
                return this.NodeBase as ScriptBase;
            }
        }
        #endregion Property: ScriptBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ScriptConditionBase(ScriptCondition scriptCondition)
        /// <summary>
        /// Creates a base of the given script condition.
        /// </summary>
        /// <param name="scriptCondition">The script condition to create a base of.</param>
        protected internal ScriptConditionBase(ScriptCondition scriptCondition)
            : base(scriptCondition)
        {
        }
        #endregion Constructor: ScriptConditionBase(ScriptCondition scriptCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: ScriptConditionBase

    #region Class: ScriptChangeBase
    /// <summary>
    /// A change on script.
    /// </summary>
    public class ScriptChangeBase : DynamicContentChangeBase
    {

        #region Properties and Fields

        #region Property: ScriptChange
        /// <summary>
        /// Gets the script change of which this is a script change base.
        /// </summary>
        protected internal ScriptChange ScriptChange
        {
            get
            {
                return this.Change as ScriptChange;
            }
        }
        #endregion Property: ScriptChange

        #region Property: ScriptBase
        /// <summary>
        /// Gets the affected script base.
        /// </summary>
        public ScriptBase ScriptBase
        {
            get
            {
                return this.NodeBase as ScriptBase;
            }
        }
        #endregion Property: ScriptBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ScriptChangeBase(ScriptChange scriptChange)
        /// <summary>
        /// Creates a base of the given script change.
        /// </summary>
        /// <param name="scriptChange">The script change to create a base of.</param>
        protected internal ScriptChangeBase(ScriptChange scriptChange)
            : base(scriptChange)
        {
        }
        #endregion Constructor: ScriptChangeBase(ScriptChange scriptChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: ScriptChangeBase

}