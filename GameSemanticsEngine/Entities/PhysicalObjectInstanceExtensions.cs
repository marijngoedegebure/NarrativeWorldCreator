/**************************************************************************
 * 
 * PhysicalObjectInstanceExtensions.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.Collections.Generic;
using Common;
using GameSemanticsEngine.Components;
using SemanticsEngine.Entities;

namespace GameSemanticsEngine.Entities
{

    #region Class: PhysicalObjectInstanceExtensions
    /// <summary>
    /// Extensions for the PhysicalObjectInstance class.
    /// </summary>
    public static class PhysicalObjectInstanceExtensions
    {

        #region Field: positionPerView
        /// <summary>
        /// A dictionary keeping track of the position per view of a physical object instance.
        /// </summary>
        private static Dictionary<PhysicalObjectInstance, Dictionary<ViewBase, Vec3>> positionPerView = new Dictionary<PhysicalObjectInstance, Dictionary<ViewBase, Vec3>>();
        #endregion Field: positionPerView

        #region Field: rotationPerView
        /// <summary>
        /// A dictionary keeping track of the rotation per view of a physical object instance.
        /// </summary>
        private static Dictionary<PhysicalObjectInstance, Dictionary<ViewBase, Quaternion>> rotationPerView = new Dictionary<PhysicalObjectInstance, Dictionary<ViewBase, Quaternion>>();
        #endregion Field: rotationPerView

        #region Method: GetPosition(this PhysicalObjectInstance physicalObjectInstance, ViewBase viewBase)
        /// <summary>
        /// Get the position for the given view.
        /// </summary>
        /// <param name="physicalObjectInstance">The physical object instance to get the position of.</param>
        /// <param name="viewBase">The view to get the position of.</param>
        /// <returns>The position for the view.</returns>
        public static Vec3 GetPosition(this PhysicalObjectInstance physicalObjectInstance, ViewBase viewBase)
        {
            if (physicalObjectInstance != null)
            {
                if (viewBase != null)
                {
                    Dictionary<ViewBase, Vec3> dictionary = null;
                    if (positionPerView.TryGetValue(physicalObjectInstance, out dictionary))
                    {
                        // Try to get the position for this view
                        Vec3 position;
                        if (dictionary.TryGetValue(viewBase, out position))
                            return position;
                    }
                }
                
                // Return the position of the physical object instance
                if (physicalObjectInstance != null)
                    return physicalObjectInstance.Position;
            }

            return Vec3.Zero;
        }
        #endregion Method: GetPosition(this PhysicalObjectInstance physicalObjectInstance, ViewBase viewBase)

        #region Method: SetPosition(this PhysicalObjectInstance physicalObjectInstance, ViewBase viewBase, Vec3 position)
        /// <summary>
        /// Set the position for the given view.
        /// </summary>
        /// <param name="physicalObjectInstance">The physical object instance to set the position of.</param>
        /// <param name="viewBase">The view to set the position of.</param>
        /// <param name="position">The new position.</param>
        public static void SetPosition(this PhysicalObjectInstance physicalObjectInstance, ViewBase viewBase, Vec3 position)
        {
            if (physicalObjectInstance != null)
            {
                if (viewBase != null)
                {
                    // Get or create the dictionary with the positions per view
                    Dictionary<ViewBase, Vec3> dictionary = null;
                    if (!positionPerView.TryGetValue(physicalObjectInstance, out dictionary))
                    {
                        dictionary = new Dictionary<ViewBase, Vec3>();
                        positionPerView.Add(physicalObjectInstance, dictionary);
                    }

                    // Set or add the position for the view
                    if (dictionary.ContainsKey(viewBase))
                        dictionary[viewBase] = position;
                    else
                        dictionary.Add(viewBase, position);
                }

                // Set the position of the physical object instance
                if (physicalObjectInstance != null)
                    physicalObjectInstance.Position = position;
            }
        }
        #endregion Method: SetPosition(this PhysicalObjectInstance physicalObjectInstance, ViewBase viewBase, Vec3 position)

        #region Method: GetRotation(this PhysicalObjectInstance physicalObjectInstance, ViewBase viewBase)
        /// <summary>
        /// Get the rotation for the given view.
        /// </summary>
        /// <param name="physicalObjectInstance">The physical object instance to get the rotation of.</param>
        /// <param name="viewBase">The view to get the rotation of.</param>
        /// <returns>The rotation for the view.</returns>
        public static Quaternion GetRotation(this PhysicalObjectInstance physicalObjectInstance, ViewBase viewBase)
        {
            if (physicalObjectInstance != null)
            {
                if (viewBase != null)
                {
                    Dictionary<ViewBase, Quaternion> dictionary = null;
                    if (rotationPerView.TryGetValue(physicalObjectInstance, out dictionary))
                    {
                        // Try to get the rotation for this view
                        Quaternion rotation;
                        if (dictionary.TryGetValue(viewBase, out rotation))
                            return rotation;
                    }
                }

                // Return the rotation of the physical object instance
                if (physicalObjectInstance != null)
                    return physicalObjectInstance.Rotation;
            }
            return Quaternion.Zero;
        }
        #endregion Method: GetRotation(this PhysicalObjectInstance physicalObjectInstance, ViewBase viewBase)

        #region Method: SetRotation(this PhysicalObjectInstance physicalObjectInstance, ViewBase viewBase, Quaternion rotation)
        /// <summary>
        /// Set the rotation for the given view.
        /// </summary>
        /// <param name="physicalObjectInstance">The physical object instance to set the rotation of.</param>
        /// <param name="viewBase">The view to set the rotation of.</param>
        /// <param name="rotation">The new rotation.</param>
        public static void SetRotation(this PhysicalObjectInstance physicalObjectInstance, ViewBase viewBase, Quaternion rotation)
        {
            if (physicalObjectInstance != null)
            {
                if (viewBase != null)
                {
                    // Get or create the dictionary with the rotations per view
                    Dictionary<ViewBase, Quaternion> dictionary = null;
                    if (!rotationPerView.TryGetValue(physicalObjectInstance, out dictionary))
                    {
                        dictionary = new Dictionary<ViewBase, Quaternion>();
                        rotationPerView.Add(physicalObjectInstance, dictionary);
                    }

                    // Set or add the rotation for the view
                    if (dictionary.ContainsKey(viewBase))
                        dictionary[viewBase] = rotation;
                    else
                        dictionary.Add(viewBase, rotation);
                }

                 // Set the rotation of the physical object instance
                if (physicalObjectInstance != null)
                    physicalObjectInstance.Rotation = rotation;
            }
        }
        #endregion Method: SetRotation(this PhysicalObjectInstance physicalObjectInstance, ViewBase viewBase, Quaternion rotation)

    }
    #endregion Class: PhysicalObjectInstanceExtensions

}
