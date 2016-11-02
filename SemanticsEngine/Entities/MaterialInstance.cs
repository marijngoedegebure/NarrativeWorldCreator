/**************************************************************************
 * 
 * MaterialInstance.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using SemanticsEngine.Components;
using SemanticsEngine.Interfaces;

namespace SemanticsEngine.Entities
{

    #region Class: MaterialInstance
    /// <summary>
    /// An instance of a material.
    /// </summary>
    public class MaterialInstance : MatterInstance
    {

        #region Properties and Fields

        #region Property: MaterialBase
        /// <summary>
        /// Gets the material base of which this is a material instance.
        /// </summary>
        public MaterialBase MaterialBase
        {
            get
            {
                return this.NodeBase as MaterialBase;
            }
        }
        #endregion Property: MaterialBase

        #region Property: MaterialValuedBase
        /// <summary>
        /// Gets the valued material base of which this is a material instance.
        /// </summary>
        public MaterialValuedBase MaterialValuedBase
        {
            get
            {
                return this.Base as MaterialValuedBase;
            }
        }
        #endregion Property: MaterialValuedBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: MaterialInstance(MaterialBase materialBase)
        /// <summary>
        /// Creates a new material instance from the given material base.
        /// </summary>
        /// <param name="materialBase">The material base to create the material instance from.</param>
        internal MaterialInstance(MaterialBase materialBase)
            : base(materialBase)
        {
        }
        #endregion Constructor: MaterialInstance(MaterialBase materialBase)

        #region Constructor: MaterialInstance(MaterialValuedBase materialValuedBase)
        /// <summary>
        /// Creates a new material instance from the given valued material base.
        /// </summary>
        /// <param name="materialValuedBase">The valued material base to create the material instance from.</param>
        internal MaterialInstance(MaterialValuedBase materialValuedBase)
            : base(materialValuedBase)
        {
        }
        #endregion Constructor: MaterialInstance(MaterialValuedBase materialValuedBase)

        #region Constructor: MaterialInstance(MaterialInstance materialInstance)
        /// <summary>
        /// Clones a material instance.
        /// </summary>
        /// <param name="materialInstance">The material instance to clone.</param>
        protected internal MaterialInstance(MaterialInstance materialInstance)
            : base(materialInstance)
        {
        }
        #endregion Constructor: MaterialInstance(MaterialInstance materialInstance)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the material instance satisfies the given condition.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the material instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the material instance satisfies the given condition.</returns>
        public override bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            // Check whether the base satisfies the condition
            if (conditionBase != null)
                return base.Satisfies(conditionBase, iVariableInstanceHolder);
            return false;
        }
        #endregion Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the given change to the material instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the material instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        protected internal override bool Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (changeBase != null)
                return base.Apply(changeBase, iVariableInstanceHolder);
            return false;
        }
        #endregion Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: MaterialInstance

}