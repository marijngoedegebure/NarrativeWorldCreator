/**************************************************************************
 * 
 * ChanceBase.cs
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
using Semantics.Utilities;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: ChanceBase
    /// <summary>
    /// A chance with a value.
    /// </summary>
    public sealed class ChanceBase : Base
    {

        #region Properties and Fields

        #region Property: Chance
        /// <summary>
        /// Gets the chance this is a base of.
        /// </summary>
        internal Chance Chance
        {
            get
            {
                return this.IdHolder as Chance;
            }
        }
        #endregion Property: Chance

        #region Property: Value
        /// <summary>
        /// The value of the chance.
        /// </summary>
        private float val = SemanticsSettings.Values.Chance;

        /// <summary>
        /// Gets the value of the chance.
        /// </summary>
        public float Value
        {
            get
            {
                return val;
            }
        }
        #endregion Property: Value

        #region Property: Variable
        /// <summary>
        /// The variable that represents the chance value instead. Only valid for numerical, random, and term variables!
        /// </summary>
        private VariableBase variable = null;

        /// <summary>
        /// Gets the variable that represents the chance value instead. Only valid for numerical, random, and term variables!
        /// </summary>
        public VariableBase Variable
        {
            get
            {
                return variable;
            }
        }
        #endregion Property: Variable

        #region Property: IsRandom
        /// <summary>
        /// The value that indicates whether the chance should be random.
        /// </summary>
        private bool isRandom = SemanticsSettings.Values.IsRandom;

        /// <summary>
        /// Gets the value that indicates whether the chance should be random.
        /// </summary>
        public bool IsRandom
        {
            get
            {
                return isRandom;
            }
        }
        #endregion Property: IsRandom

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ChanceBase(Chance chance)
        /// <summary>
        /// Creates a base of the given chance.
        /// </summary>
        /// <param name="chance">The chance to create a base of.</param>
        internal ChanceBase(Chance chance)
            : base(chance)
        {
            if (chance != null)
            {
                this.val = chance.Value;
                this.variable = BaseManager.Current.GetBase<VariableBase>(chance.Variable);
                this.isRandom = chance.IsRandom;
            }
        }
        #endregion Constructor: ChanceBase(Chance chance)

        #endregion Method Group: Constructors

    }
    #endregion Class: ChanceBase

}
