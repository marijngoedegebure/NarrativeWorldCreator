/**************************************************************************
 * 
 * ParticlePropertiesInstance.cs
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

namespace GameSemanticsEngine.GameContent
{

    #region Class: ParticlePropertiesInstance
    /// <summary>
    /// An instance of particle properties.
    /// </summary>
    public class ParticlePropertiesInstance : StaticContentInstance
    {

        #region Properties and Fields

        #region Property: ParticlePropertiesBase
        /// <summary>
        /// Gets the particle properties base of which this is a particle properties instance.
        /// </summary>
        public ParticlePropertiesBase ParticlePropertiesBase
        {
            get
            {
                if (this.ParticlePropertiesValuedBase != null)
                    return this.ParticlePropertiesValuedBase.ParticlePropertiesBase;
                return null;
            }
        }
        #endregion Property: ParticlePropertiesBase

        #region Property: ParticlePropertiesValuedBase
        /// <summary>
        /// Gets the valued particle properties base of which this is a particle properties instance.
        /// </summary>
        public ParticlePropertiesValuedBase ParticlePropertiesValuedBase
        {
            get
            {
                return this.Base as ParticlePropertiesValuedBase;
            }
        }
        #endregion Property: ParticlePropertiesValuedBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ParticlePropertiesInstance(ParticlePropertiesValuedBase particlePropertiesValuedBase)
        /// <summary>
        /// Creates a new particle properties instance from the given valued particle properties base.
        /// </summary>
        /// <param name="particlePropertiesValuedBase">The valued particle properties base to create the particle properties instance from.</param>
        internal ParticlePropertiesInstance(ParticlePropertiesValuedBase particlePropertiesValuedBase)
            : base(particlePropertiesValuedBase)
        {
        }
        #endregion Constructor: ParticlePropertiesInstance(ParticlePropertiesValuedBase particlePropertiesValuedBase)

        #region Constructor: ParticlePropertiesInstance(ParticlePropertiesInstance particlePropertiesInstance)
        /// <summary>
        /// Clones a particle properties instance.
        /// </summary>
        /// <param name="particlePropertiesInstance">The particle properties instance to clone.</param>
        protected internal ParticlePropertiesInstance(ParticlePropertiesInstance particlePropertiesInstance)
            : base(particlePropertiesInstance)
        {
        }
        #endregion Constructor: ParticlePropertiesInstance(ParticlePropertiesInstance particlePropertiesInstance)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the particle properties instance satisfies the given condition.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the particle properties instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the particle properties instance is satisfies the given condition.</returns>
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
        /// Apply the change to the particle properties instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the particle properties instance.</param>
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
    #endregion Class: ParticlePropertiesInstance

}