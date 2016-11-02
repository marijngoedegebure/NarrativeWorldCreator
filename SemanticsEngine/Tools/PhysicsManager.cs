/**************************************************************************
 * 
 * PhysicsManager.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Common;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Components;
using SemanticsEngine.Entities;
using SemanticsEngine.Interfaces;

namespace SemanticsEngine.Tools
{

    #region Class: PhysicsManager
    /// <summary>
    /// A manager to keep track of all physics handlers.
    /// </summary>
    public class PhysicsManager
    {

        #region Properties and Fields

        #region Property: Current
        /// <summary>
        /// The current physics manager.
        /// </summary>
        private static PhysicsManager current = new PhysicsManager();

        /// <summary>
        /// The current physics manager.
        /// </summary>
        public static PhysicsManager Current
        {
            get
            {
                return current;
            }
        }
        #endregion Property: Current

        #region Property: PhysicsHandlers
        /// <summary>
        /// All the placement handlers.
        /// </summary>
        private List<IPhysicsHandler> physicsHandlers = new List<IPhysicsHandler>();

        /// <summary>
        /// Gets all the placement handlers.
        /// </summary>
        public ReadOnlyCollection<IPhysicsHandler> PhysicsHandlers
        {
            get
            {
                return physicsHandlers.AsReadOnly();
            }
        }
        #endregion Property: PhysicsHandlers

        #endregion Properties and Fields

        #region Constructor: PhysicsManager()
        /// <summary>
        /// Creates a new physics manager.
        /// </summary>
        protected PhysicsManager()
        {
        }
        #endregion Constructor: PhysicsManager()

        #region Method: AddPhysicsHandler(IPhysicsHandler physicsHandler)
        /// <summary>
        /// Adds the given physics handler.
        /// </summary>
        /// <param name="physicsHandler">The physics handler to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddPhysicsHandler(IPhysicsHandler physicsHandler)
        {
            if (physicsHandler != null)
            {
                if (this.PhysicsHandlers.Contains(physicsHandler))
                    return Message.RelationExistsAlready;

                this.physicsHandlers.Add(physicsHandler);
                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddPhysicsHandler(IPhysicsHandler physicsHandler)

        #region Method: RemovePhysicsHandler(IPhysicsHandler physicsHandler)
        /// <summary>
        /// Removes the given physics handler.
        /// </summary>
        /// <param name="physicsHandler">The physics handler to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemovePhysicsHandler(IPhysicsHandler physicsHandler)
        {
            if (physicsHandler != null)
            {
                if (this.PhysicsHandlers.Contains(physicsHandler))
                {
                    this.physicsHandlers.Remove(physicsHandler);
                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemovePhysicsHandler(IPhysicsHandler physicsHandler)

        #region Method: WillItFit(TangibleObjectInstance item, SpaceInstance space, out Vec3 position)
        /// <summary>
        /// Check whether the given item fits into the given space, and if so, get the position.
        /// </summary>
        /// <param name="item">The item to add to the space.</param>
        /// <param name="space">The space to add the item to.</param>
        /// <param name="position">The position the item should get.</param>
        /// <returns>Returns whether the item fits in the space.</returns>
        public bool WillItFit(TangibleObjectInstance item, SpaceInstance space, out Vec3 position)
        {
            if (item != null && space != null)
            {
                if (this.PhysicsHandlers.Count == 0)
                {
                    position = space.Position;
                    return true;
                }

                foreach (IPhysicsHandler physicsHandler in this.PhysicsHandlers)
                {
                    if (physicsHandler.WillItFit(item, space, out position))
                        return true;
                }

                position = space.Position;
                return false;
            }

            position = Vec3.Zero;
            return false;
        }
        #endregion Method: WillItFit(TangibleObjectInstance item, SpaceInstance space, out Vec3 position)

        #region Method: Collide(PhysicalEntityInstance physicalEntity1, PhysicalEntityInstance physicalEntity2)
        /// <summary>
        /// Check whether both physical entities collide.
        /// </summary>
        /// <param name="physicalEntity1">The first physical entity.</param>
        /// <param name="physicalEntity2">The second physical entity.</param>
        /// <returns>Returns whether both physical entities collide.</returns>
        public bool Collide(PhysicalEntityInstance physicalEntity1, PhysicalEntityInstance physicalEntity2)
        {
            if (physicalEntity1 != null && physicalEntity2 != null)
            {
                if (this.PhysicsHandlers.Count == 0)
                {
                    // Adjust the epsilon based on the scale of the possible physical objects
                    float scaleAverage = 1;
                    PhysicalObjectInstance physicalObjectInstance1 = physicalEntity1 as PhysicalObjectInstance;
                    if (physicalObjectInstance1 != null)
                    {
                        Vec3 scale = physicalObjectInstance1.Scale;
                        scaleAverage = (scaleAverage + ((scale.X + scale.Y + scale.Z) / 3)) / 2;
                    }
                    PhysicalObjectInstance physicalObjectInstance2 = physicalEntity2 as PhysicalObjectInstance;
                    if (physicalObjectInstance2 != null)
                    {
                        Vec3 scale = physicalObjectInstance2.Scale;
                        scaleAverage = (scaleAverage + ((scale.X + scale.Y + scale.Z) / 3)) / 2;
                    }

                    // Check whether the distance is smaller than an epsilon, adjusted by the scale
                    return Vec3.Distance(physicalEntity1.Position, physicalEntity2.Position) < (SemanticsEngineSettings.DistanceEpsilon * scaleAverage);
                }

                foreach (IPhysicsHandler physicsHandler in this.PhysicsHandlers)
                {
                    if (physicsHandler.Collide(physicalEntity1, physicalEntity2))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: Collide(PhysicalEntityInstance physicalEntity1, PhysicalEntityInstance physicalEntity2)

        #region Method: GetSize(SpaceInstance spaceInstance)
        /// <summary>
        /// Get the size of the space instance.
        /// </summary>
        /// <param name="spaceInstance">The space instance to get the size of.</param>
        /// <returns>The size of the space instance.</returns>
        public NumericalValueInstance GetSize(SpaceInstance spaceInstance)
        {
            if (this.PhysicsHandlers.Count == 0)
            {
                UnitBase unit = null;
                UnitCategoryBase unitCategory = Utils.GetSpecialUnitCategory(SpecialUnitCategories.Quantity);
                if (unitCategory != null)
                    unit = unitCategory.BaseUnit;
                NumericalValueInstance size = new NumericalValueInstance(0, Prefix.None, unit, 0, SemanticsSettings.Values.MaxValue);

                // Add the volume of the items
                foreach (TangibleObjectInstance item in spaceInstance.Items)
                    size += item.Volume;

                // Add the quantity of the tangible matter
                foreach (MatterInstance tangibleMatter in spaceInstance.TangibleMatter)
                    size += tangibleMatter.Quantity;

                return size;
            }
            else
                return this.PhysicsHandlers[0].GetSize(spaceInstance);
        }
        #endregion Method: GetSize(SpaceInstance spaceInstance)

        #region Method: GetVolume(TangibleObjectInstance tangibleObjectInstance)
        /// <summary>
        /// Get the volume of the tangible object instance.
        /// </summary>
        /// <param name="tangibleObjectInstance">The tangible object instance to get the volume of.</param>
        /// <returns>The volume of the tangible object instance.</returns>
        public NumericalValueInstance GetVolume(TangibleObjectInstance tangibleObjectInstance)
        {
            if (this.PhysicsHandlers.Count == 0)
            {
                UnitBase unit = null;
                UnitCategoryBase unitCategory = Utils.GetSpecialUnitCategory(SpecialUnitCategories.Quantity);
                if (unitCategory != null)
                    unit = unitCategory.BaseUnit;
                NumericalValueInstance volume = new NumericalValueInstance(0, Prefix.None, unit, 0, SemanticsSettings.Values.MaxValue);

                // Add the quantity of the matter
                foreach (MatterInstance matterInstance in tangibleObjectInstance.Matter)
                    volume += matterInstance.Quantity;

                // Add the volume of the parts
                foreach (TangibleObjectInstance part in tangibleObjectInstance.Parts)
                    volume += part.Volume;

                // Add the size of the spaces
                foreach (SpaceInstance space in tangibleObjectInstance.Spaces)
                    volume += space.Size;

                return volume;
            }
            else
                return this.PhysicsHandlers[0].GetVolume(tangibleObjectInstance);
        }
        #endregion Method: GetVolume(TangibleObjectInstance tangibleObjectInstance)

    }
    #endregion Class: PhysicsManager

}