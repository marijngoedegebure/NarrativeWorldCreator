/**************************************************************************
 * 
 * Script.cs
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

    #region Class: Script
    /// <summary>
    /// A script.
    /// </summary>
    public class Script : DynamicContent, IComparable<Script>
    {

        #region Method Group: Constructors

        #region Constructor: Script()
        /// <summary>
        /// Creates a new script.
        /// </summary>
        public Script()
            : base()
        {
        }
        #endregion Constructor: Script()

        #region Constructor: Script(uint id)
        /// <summary>
        /// Creates a new script from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a script from.</param>
        protected Script(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Script(uint id)

        #region Constructor: Script(string file)
        /// <summary>
        /// Creates a new script with the given file.
        /// </summary>
        /// <param name="file">The file to assign to the script.</param>
        public Script(string file)
            : base(file)
        {
        }
        #endregion Constructor: Script(string file)

        #region Constructor: Script(Script script)
        /// <summary>
        /// Clones a script.
        /// </summary>
        /// <param name="script">The script to clone.</param>
        public Script(Script script)
            : base(script)
        {
        }
        #endregion Constructor: Script(Script script)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the script.
        /// </summary>
        public override void Remove()
        {
            GameDatabase.Current.StartChange();
            GameDatabase.Current.StartRemove();

            base.Remove();

            GameDatabase.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(Script other)
        /// <summary>
        /// Compares the script to the other script.
        /// </summary>
        /// <param name="other">The script to compare to this script.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Script other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(Script other)

        #endregion Method Group: Other

    }
    #endregion Class: Script

    #region Class: ScriptValued
    /// <summary>
    /// A valued version of a script.
    /// </summary>
    public class ScriptValued : DynamicContentValued
    {

        #region Properties and Fields

        #region Property: Script
        /// <summary>
        /// Gets the script of which this is a valued script.
        /// </summary>
        public Script Script
        {
            get
            {
                return this.Node as Script;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: Script

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ScriptValued(uint id)
        /// <summary>
        /// Creates a new valued script from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the valued script from.</param>
        protected ScriptValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ScriptValued(uint id)

        #region Constructor: ScriptValued(ScriptValued scriptValued)
        /// <summary>
        /// Clones a valued script.
        /// </summary>
        /// <param name="scriptValued">The valued script to clone.</param>
        public ScriptValued(ScriptValued scriptValued)
            : base(scriptValued)
        {
        }
        #endregion Constructor: ScriptValued(ScriptValued scriptValued)

        #region Constructor: ScriptValued(Script script)
        /// <summary>
        /// Creates a new valued script from the given script.
        /// </summary>
        /// <param name="script">The script to create a valued script from.</param>
        public ScriptValued(Script script)
            : base(script)
        {
        }
        #endregion Constructor: ScriptValued(Script script)

        #endregion Method Group: Constructors

    }
    #endregion Class: ScriptValued

    #region Class: ScriptCondition
    /// <summary>
    /// A condition on a script.
    /// </summary>
    public class ScriptCondition : DynamicContentCondition
    {

        #region Properties and Fields

        #region Property: Script
        /// <summary>
        /// Gets or sets the required script.
        /// </summary>
        public Script Script
        {
            get
            {
                return this.Node as Script;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Script

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ScriptCondition()
        /// <summary>
        /// Creates a new script condition.
        /// </summary>
        public ScriptCondition()
            : base()
        {
        }
        #endregion Constructor: ScriptCondition()

        #region Constructor: ScriptCondition(uint id)
        /// <summary>
        /// Creates a new script condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the script condition from.</param>
        protected ScriptCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ScriptCondition(uint id)

        #region Constructor: ScriptCondition(ScriptCondition scriptCondition)
        /// <summary>
        /// Clones a script condition.
        /// </summary>
        /// <param name="scriptCondition">The script condition to clone.</param>
        public ScriptCondition(ScriptCondition scriptCondition)
            : base(scriptCondition)
        {
        }
        #endregion Constructor: ScriptCondition(ScriptCondition scriptCondition)

        #region Constructor: ScriptCondition(Script script)
        /// <summary>
        /// Creates a condition for the given script.
        /// </summary>
        /// <param name="script">The script to create a condition for.</param>
        public ScriptCondition(Script script)
            : base(script)
        {
        }
        #endregion Constructor: ScriptCondition(Script script)

        #endregion Method Group: Constructors

    }
    #endregion Class: ScriptCondition

    #region Class: ScriptChange
    /// <summary>
    /// A change on a script.
    /// </summary>
    public class ScriptChange : DynamicContentChange
    {

        #region Properties and Fields

        #region Property: Script
        /// <summary>
        /// Gets or sets the affected script.
        /// </summary>
        public Script Script
        {
            get
            {
                return this.Node as Script;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Script

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ScriptChange()
        /// <summary>
        /// Creates a script change.
        /// </summary>
        public ScriptChange()
            : base()
        {
        }
        #endregion Constructor: ScriptChange()

        #region Constructor: ScriptChange(uint id)
        /// <summary>
        /// Creates a new script change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a script change from.</param>
        protected ScriptChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ScriptChange(uint id)

        #region Constructor: ScriptChange(ScriptChange scriptChange)
        /// <summary>
        /// Clones a script change.
        /// </summary>
        /// <param name="scriptChange">The script change to clone.</param>
        public ScriptChange(ScriptChange scriptChange)
            : base(scriptChange)
        {
        }
        #endregion Constructor: ScriptChange(ScriptChange scriptChange)

        #region Constructor: ScriptChange(Script script)
        /// <summary>
        /// Creates a change for the given script.
        /// </summary>
        /// <param name="script">The script to create a change for.</param>
        public ScriptChange(Script script)
            : base(script)
        {
        }
        #endregion Constructor: ScriptChange(Script script)

        #endregion Method Group: Constructors

    }
    #endregion Class: ScriptChange

}