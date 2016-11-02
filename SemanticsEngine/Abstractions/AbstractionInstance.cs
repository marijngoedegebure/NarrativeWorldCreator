/**************************************************************************
 * 
 * AbstractionInstance.cs
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

namespace SemanticsEngine.Abstractions
{

    #region Class: AbstractionInstance
    /// <summary>
    /// An instance of an abstraction.
    /// </summary>
    public abstract class AbstractionInstance : NodeInstance
    {

        #region Properties and Fields

        #region Property: AbstractionBase
        /// <summary>
        /// Gets the abstraction base of which this is an abstraction instance.
        /// </summary>
        public AbstractionBase AbstractionBase
        {
            get
            {
                return this.NodeBase as AbstractionBase;
            }
        }
        #endregion Property: AbstractionBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AbstractionInstance(AbstractionBase abstractionBase)
        /// <summary>
        /// Creates a new abstraction instance from the given abstraction base.
        /// </summary>
        /// <param name="abstractionBase">The abstraction base to create the abstraction instance from.</param>
        protected AbstractionInstance(AbstractionBase abstractionBase)
            : base(abstractionBase)
        {
        }
        #endregion Constructor: AbstractionInstance(AbstractionBase abstractionBase)

        #region Constructor: AbstractionInstance(NodeValuedBase abstractionValuedBase)
        /// <summary>
        /// Creates a new abstraction instance from the given valued abstraction base.
        /// </summary>
        /// <param name="abstractionValuedBase">The valued abstraction base to create the abstraction instance from.</param>
        protected AbstractionInstance(NodeValuedBase abstractionValuedBase)
            : base(abstractionValuedBase)
        {
        }
        #endregion Constructor: AbstractionInstance(NodeValuedBase abstractionValuedBase)

        #region Constructor: AbstractionInstance(AbstractionInstance abstractionInstance)
        /// <summary>
        /// Clones an abstraction instance.
        /// </summary>
        /// <param name="abstractionInstance">The abstraction instance to clone.</param>
        protected AbstractionInstance(AbstractionInstance abstractionInstance)
            : base(abstractionInstance)
        {
        }
        #endregion Constructor: AbstractionInstance(AbstractionInstance abstractionInstance)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the given condition satisfies the abstraction instance.
        /// </summary>
        /// <param name="conditionBase">The abstraction condition to compare to the abstraction instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the condition satisfies the abstraction instance.</returns>
        public override bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (conditionBase != null)
                return base.Satisfies(conditionBase, iVariableInstanceHolder);
            return false;
        }
        #endregion Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the given change to the abstraction instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the abstraction instance.</param>
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
    #endregion Class: AbstractionInstance

}