/**************************************************************************
 * 
 * SubstanceInstance.cs
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

    #region Class: SubstanceInstance
    /// <summary>
    /// An instance of a substance.
    /// </summary>
    public class SubstanceInstance : MatterInstance
    {

        #region Properties and Fields

        #region Property: SubstanceBase
        /// <summary>
        /// Gets the substance base of which this is a substance instance.
        /// </summary>
        public SubstanceBase SubstanceBase
        {
            get
            {
                return this.NodeBase as SubstanceBase;
            }
        }
        #endregion Property: SubstanceBase

        #region Property: SubstanceValuedBase
        /// <summary>
        /// Gets the valued substance base of which this is a substance instance.
        /// </summary>
        public SubstanceValuedBase SubstanceValuedBase
        {
            get
            {
                return this.Base as SubstanceValuedBase;
            }
        }
        #endregion Property: SubstanceValuedBase

        #region Property: Compound
        /// <summary>
        /// The compound this is a substance of.
        /// </summary>
        private CompoundInstance compound = null;

        /// <summary>
        /// Gets the compound this is a substance of.
        /// </summary>
        public CompoundInstance Compound
        {
            get
            {
                return compound;
            }
            internal set
            {
                if (compound != value)
                {
                    compound = value;
                    NotifyPropertyChanged("Compound");
                }
            }
        }
        #endregion Property: Compound

        #region Property: Mixture
        /// <summary>
        /// The mixture this is a substance of.
        /// </summary>
        private MixtureInstance mixture = null;

        /// <summary>
        /// Gets the mixture this is a substance of.
        /// </summary>
        public MixtureInstance Mixture
        {
            get
            {
                return mixture;
            }
            internal set
            {
                if (mixture != value)
                {
                    mixture = value;
                    NotifyPropertyChanged("Mixture");
                }
            }
        }
        #endregion Property: Mixture
		
        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SubstanceInstance(SubstanceBase substanceBase)
        /// <summary>
        /// Creates a new substance instance from the given substance base.
        /// </summary>
        /// <param name="substanceBase">The substance base to create the substance instance from.</param>
        internal SubstanceInstance(SubstanceBase substanceBase)
            : this(substanceBase, false)
        {
        }

        /// <summary>
        /// Creates a new substance instance from the given substance base.
        /// </summary>
        /// <param name="substanceBase">The substance base to create the substance instance from.</param>
        /// <param name="ignoreCreation">Indicates whether creation of elements should be ignored.</param>
        internal SubstanceInstance(SubstanceBase substanceBase, bool ignoreCreation)
            : base(substanceBase, ignoreCreation)
        {
        }
        #endregion Constructor: SubstanceInstance(SubstanceBase substanceBase)

        #region Constructor: SubstanceInstance(SubstanceValuedBase substanceValuedBase)
        /// <summary>
        /// Creates a new substance instance from the given valued substance base.
        /// </summary>
        /// <param name="substanceValuedBase">The valued substance base to create the substance instance from.</param>
        internal SubstanceInstance(SubstanceValuedBase substanceValuedBase)
            : this(substanceValuedBase, false)
        {
        }

        /// <summary>
        /// Creates a new substance instance from the given valued substance base.
        /// </summary>
        /// <param name="substanceValuedBase">The valued substance base to create the substance instance from.</param>
        /// <param name="ignoreCreation">Indicates whether creation of elements should be ignored.</param>
        internal SubstanceInstance(SubstanceValuedBase substanceValuedBase, bool ignoreCreation)
            : base(substanceValuedBase, ignoreCreation)
        {
        }
        #endregion Constructor: SubstanceInstance(SubstanceValuedBase substanceValuedBase)

        #region Constructor: SubstanceInstance(SubstanceInstance substanceInstance)
        /// <summary>
        /// Clones a substance instance.
        /// </summary>
        /// <param name="substanceInstance">The substance instance to clone.</param>
        protected internal SubstanceInstance(SubstanceInstance substanceInstance)
            : base(substanceInstance)
        {
        }
        #endregion Constructor: SubstanceInstance(SubstanceInstance substanceInstance)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: MarkAsModified(bool modified, bool spread)
        /// <summary>
        /// Mark this instance as modified.
        /// </summary>
        /// <param name="modified">The value that indicates whether the instance has been modified.</param>
        /// <param name="spread">The value that indicates whether the marking should spread further.</param>
        internal override void MarkAsModified(bool modified, bool spread)
        {
            base.MarkAsModified(modified, spread);

            if (spread)
            {
                if (this.Compound != null)
                    this.Compound.MarkAsModified(modified, false);
                if (this.Mixture != null)
                    this.Mixture.MarkAsModified(modified, false);
            }
        }
        #endregion Method: MarkAsModified(bool modified, bool spread)

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the substance instance satisfies the given condition.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the substance instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the substance instance satisfies the given condition.</returns>
        public override bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (conditionBase != null)
                return base.Satisfies(conditionBase, iVariableInstanceHolder);
            return false;
        }
        #endregion Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the given change to the substance instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the substance instance.</param>
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
    #endregion Class: SubstanceInstance

}