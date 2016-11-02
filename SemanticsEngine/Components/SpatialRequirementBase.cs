/**************************************************************************
 * 
 * SpatialRequirementBase.cs
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
using SemanticsEngine.Entities;
using SemanticsEngine.Interfaces;

namespace SemanticsEngine.Components
{

    #region Class: SpatialRequirementBase
    /// <summary>
    /// A spatial requirement states to be within, or be away from a certain distance.
    /// </summary>
    public sealed class SpatialRequirementBase : NumericalValueRangeBase
    {

        #region Properties and Fields

        #region Property: SpatialRequirement
        /// <summary>
        /// Gets the spatial requirement of which this is a spatial requirement base.
        /// </summary>
        internal SpatialRequirement SpatialRequirement
        {
            get
            {
                return this.IdHolder as SpatialRequirement;
            }
        }
        #endregion Property: SpatialRequirement

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SpatialRequirementBase(SpatialRequirement spatialRequirement)
        /// <summary>
        /// Creates a base of the given spatial requirement.
        /// </summary>
        /// <param name="spatialRequirement">The spatial requirement to create a base of.</param>
        internal SpatialRequirementBase(SpatialRequirement spatialRequirement)
            : base(spatialRequirement)
        {
        }
        #endregion Constructor: SpatialRequirementBase(SpatialRequirement spatialRequirement)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: IsSatisfied(PhysicalEntityInstance instance1, PhysicalEntityInstance instance2, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the spatial requirement is satisfied for the given two instances.
        /// </summary>
        /// <param name="instance1">The first instance.</param>
        /// <param name="instance2">The second instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the spatial requirement is satisfied for the two instances.</returns>
        public bool IsSatisfied(PhysicalEntityInstance instance1, PhysicalEntityInstance instance2, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (instance1 != null && instance2 != null)
            {
                // Get the distance between both instances and compare them
                float distanceSquared = instance1.GetDistance(instance2, true, true);
                float val1 = GetValue(iVariableInstanceHolder);
                float val2 = GetValue2(iVariableInstanceHolder);
                return Toolbox.Compare(distanceSquared, this.ValueSign, val1 * val1, val2 * val2);
            }
            return true;
        }
        #endregion Method: IsSatisfied(PhysicalEntityInstance instance1, PhysicalEntityInstance instance2, IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: SpatialRequirementBase

}