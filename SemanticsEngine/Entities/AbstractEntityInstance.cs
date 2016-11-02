/**************************************************************************
 * 
 * AbstractEntityInstance.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using SemanticsEngine.Components;
using SemanticsEngine.Interfaces;

namespace SemanticsEngine.Entities
{

    #region Class: AbstractEntityInstance
    /// <summary>
    /// An instance of an abstract entity.
    /// </summary>
    public class AbstractEntityInstance : EntityInstance
    {

        #region Properties and Fields

        #region Property: AbstractEntityBase
        /// <summary>
        /// Gets the abstract entity base of which this is an abstract entity instance.
        /// </summary>
        public AbstractEntityBase AbstractEntityBase
        {
            get
            {
                return this.NodeBase as AbstractEntityBase;
            }
        }
        #endregion Property: AbstractEntityBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AbstractEntityInstance(AbstractEntityBase abstractEntityBase)
        /// <summary>
        /// Creates a new abstract entity instance from the given abstract entity base.
        /// </summary>
        /// <param name="abstractEntityBase">The abstract entity base to create the abstract entity instance from.</param>
        internal AbstractEntityInstance(AbstractEntityBase abstractEntityBase)
            : base(abstractEntityBase)
        {
        }
        #endregion Constructor: AbstractEntityInstance(AbstractEntityBase abstractEntityBase)

        #region Constructor: AbstractEntityInstance(AbstractEntityInstance abstractEntityInstance)
        /// <summary>
        /// Clones an abstract entity instance.
        /// </summary>
        /// <param name="abstractEntityInstance">The abstract entity instance to clone.</param>
        protected internal AbstractEntityInstance(AbstractEntityInstance abstractEntityInstance)
            : base(abstractEntityInstance)
        {
        }
        #endregion Constructor: AbstractEntityInstance(AbstractEntityInstance abstractEntityInstance)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the given condition satisfies the abstract entity instance.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the abstract entity instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the condition satisfies the abstract entity instance.</returns>
        public override bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (conditionBase != null)
                return base.Satisfies(conditionBase, iVariableInstanceHolder);
            return false;
        }
        #endregion Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the given change to the abstract entity instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the abstract entity instance.</param>
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
    #endregion Class: AbstractEntityInstance

}