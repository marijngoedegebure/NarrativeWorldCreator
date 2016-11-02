/**************************************************************************
 * 
 * IPhysicsHandler.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using Common;
using SemanticsEngine.Components;
using SemanticsEngine.Entities;

namespace SemanticsEngine.Interfaces
{

    #region Interface: IPhysicsHandler
    /// <summary>
    /// The physics handler takes care of physics checks required by the Semantics Engine.
    /// </summary>
    public interface IPhysicsHandler
    {

        /// <summary>
        /// Check whether the given item fits into the given space, and if so, get the position.
        /// </summary>
        /// <param name="item">The item to add to the space.</param>
        /// <param name="space">The space to add the item to.</param>
        /// <param name="position">The position the item should get.</param>
        /// <returns>Returns whether the item fits in the space.</returns>
        bool WillItFit(TangibleObjectInstance item, SpaceInstance space, out Vec3 position);

        /// <summary>
        /// Check whether both physical entities collide.
        /// </summary>
        /// <param name="physicalEntity1">The first physical entity.</param>
        /// <param name="physicalEntity2">The second physical entity.</param>
        /// <returns>Returns whether both physical entities collide.</returns>
        bool Collide(PhysicalEntityInstance physicalEntity1, PhysicalEntityInstance physicalEntity2);

        /// <summary>
        /// Get the size of the space instance.
        /// </summary>
        /// <param name="spaceInstance">The space instance to get the size of.</param>
        /// <returns>The size of the space instance.</returns>
        NumericalValueInstance GetSize(SpaceInstance spaceInstance);

        /// <summary>
        /// Get the volume of the tangible object instance.
        /// </summary>
        /// <param name="tangibleObjectInstance">The tangible object instance to get the volume of.</param>
        /// <returns>The volume of the tangible object instance.</returns>
        NumericalValueInstance GetVolume(TangibleObjectInstance tangibleObjectInstance);

    }
    #endregion Interface: IPhysicsHandler

}