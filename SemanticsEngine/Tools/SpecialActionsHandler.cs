/**************************************************************************
 * 
 * SpecialActionsHandler.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using Common;
using Semantics.Utilities;
using SemanticsEngine.Entities;

namespace SemanticsEngine.Tools
{

    #region  Class: SpecialActionsHandler
    /// <summary>
    /// Allows the execution of special actions.
    /// </summary>
    public static class SpecialActionsHandler
    {

        #region Method: Give(PhysicalObjectInstance sender, TangibleObjectInstance item, PhysicalObjectInstance receiver)
        /// <summary>
        /// Let the sender give an item to the receiver.
        /// </summary>
        /// <param name="sender">The sender of the item.</param>
        /// <param name="item">The item that is given.</param>
        /// <param name="receiver">The receiver of the item.</param>
        /// <returns>Returns whether the give action has been successful.</returns>
        public static bool Give(PhysicalObjectInstance sender, TangibleObjectInstance item, PhysicalObjectInstance receiver)
        {
            if (sender != null && item != null && receiver != null && !sender.Equals(receiver) && !sender.Equals(item) && !item.Equals(receiver) && receiver.DefaultSpace != null)
            {
                // Check which space of the sender has the item
                foreach (SpaceInstance spaceInstance in sender.Spaces)
                {
                    // Remove the item
                    if (spaceInstance.RemoveItem(item) == Message.RelationSuccess)
                    {
                        // Add the item to the default space of the receiver
                        if (receiver.DefaultSpace.AddItem(item) == Message.RelationSuccess)
                        {
                            receiver.IsModified = true;
                            return true;
                        }
                        // If this fails, give it back to the sender                        
                        else
                            spaceInstance.AddItem(item);
                    }
                }
            }
            return false;
        }
        #endregion Method: Give(PhysicalObjectInstance sender, TangibleObjectInstance item, PhysicalObjectInstance receiver)

        #region Method: Give(PhysicalObjectInstance sender, MatterInstance tangibleMatter, PhysicalObjectInstance receiver)
        /// <summary>
        /// Let the sender give tangible matter to the receiver.
        /// </summary>
        /// <param name="sender">The sender of the tangible matter.</param>
        /// <param name="tangibleMatter">The tangible matter that is given.</param>
        /// <param name="receiver">The receiver of the tangible matter.</param>
        /// <returns>Returns whether the give action has been successful.</returns>
        public static bool Give(PhysicalObjectInstance sender, MatterInstance tangibleMatter, PhysicalObjectInstance receiver)
        {
            if (sender != null && tangibleMatter != null && receiver != null && !sender.Equals(receiver) && receiver.DefaultSpace != null)
            {
                // Check which space of the sender has the tangible matter
                foreach (SpaceInstance spaceInstance in sender.Spaces)
                {
                    if (spaceInstance.TangibleMatter.Contains(tangibleMatter))
                    {
                        // Add the tangible matter to the default space of the receiver
                        if (receiver.DefaultSpace.AddTangibleMatter(tangibleMatter) == Message.RelationSuccess)
                        {
                            sender.IsModified = true;
                            tangibleMatter.IsModified = true;
                            receiver.IsModified = true;
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        #endregion Method: Give(PhysicalObjectInstance sender, MatterInstance tangibleMatter, PhysicalObjectInstance receiver)

        #region Method: Take(PhysicalObjectInstance actor, TangibleObjectInstance item)
        /// <summary>
        /// Let the actor take the item, after which it will be added to one of his spaces.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="item">The item to take.</param>
        /// <returns>Returns whether the take action has been successful.</returns>
        public static bool Take(PhysicalObjectInstance actor, TangibleObjectInstance item)
        {
            if (actor != null && item != null && !actor.Equals(item))
            {
                // Put the item in the default space of the actor
                if (actor.DefaultSpace != null)
                    return Take(actor, item, actor.DefaultSpace);
            }
            return false;
        }
        #endregion Method: Take(PhysicalObjectInstance actor, TangibleObjectInstance item)

        #region Method: Take(PhysicalObjectInstance actor, TangibleObjectInstance item, SpaceInstance space)
        /// <summary>
        /// Let the actor take the item, after which it will be added to the given space of the actor.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="item">The item to take.</param>
        /// <param name="space">A space of the actor to put the item in.</param>
        /// <returns>Returns whether the take action has been successful.</returns>
        public static bool Take(PhysicalObjectInstance actor, TangibleObjectInstance item, SpaceInstance space)
        {
            if (actor != null && item != null && space != null && !actor.Equals(item) && !actor.Equals(space) && actor.Spaces.Contains(space))
            {
                SpaceInstance itemSpace = item.Space;
                if (itemSpace != null)
                {
                    // The space may not be closed
                    if (itemSpace.SpaceType != SpaceType.Closed)
                    {
                        // Remove the item from the space it is in
                        if (itemSpace != null)
                            itemSpace.RemoveItem(item);

                        // Add the item to the space of the actor
                        if (space.AddItem(item) == Message.RelationSuccess)
                        {
                            actor.IsModified = true;
                            item.IsModified = true;
                            space.IsModified = true;
                            return true;
                        }
                        // If this fails, add it back to the original item space
                        else if (itemSpace != null)
                            itemSpace.AddItem(item);
                    }
                }
            }
            return false;
        }
        #endregion Method: Take(PhysicalObjectInstance actor, TangibleObjectInstance item, SpaceInstance space)

        #region Method: Take(PhysicalObjectInstance actor, MatterInstance tangibleMatter)
        /// <summary>
        /// Let the actor take the tangible matter, after which it will be added to one of his spaces.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="tangibleMatter">The tangible matter to take.</param>
        /// <returns>Returns whether the take action has been successful.</returns>
        public static bool Take(PhysicalObjectInstance actor, MatterInstance tangibleMatter)
        {
            if (actor != null && tangibleMatter != null && !actor.Equals(tangibleMatter))
            {
                // Put the tangible matter in the default space of the actor
                if (actor.DefaultSpace != null)
                    return Take(actor, tangibleMatter, actor.DefaultSpace);
            }
            return false;
        }
        #endregion Method: Take(PhysicalObjectInstance actor, MatterInstance tangibleMatter)

        #region Method: Take(PhysicalObjectInstance actor, MatterInstance tangibleMatter, SpaceInstance space)
        /// <summary>
        /// Let the actor take the tangible matter, after which it will be added to the given space of the actor.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="tangibleMatter">The tangible matter to take.</param>
        /// <param name="space">A space of the actor to put the tangible matter in.</param>
        /// <returns>Returns whether the take action has been successful.</returns>
        public static bool Take(PhysicalObjectInstance actor, MatterInstance tangibleMatter, SpaceInstance space)
        {
            if (actor != null && tangibleMatter != null && space != null && !actor.Equals(space) && actor.Spaces.Contains(space))
            {
                // It is not allowed to take the matter of an object
                if (tangibleMatter.TangibleObject == null)
                {
                    SpaceInstance itemSpace = tangibleMatter.Space;
                    if (itemSpace != null)
                    {
                        // The space may not be closed
                        if (itemSpace.SpaceType != SpaceType.Closed)
                        {
                            // Remove the tangible matter from the space it is in
                            if (itemSpace != null)
                                itemSpace.RemoveTangibleMatter(tangibleMatter);

                            // Add the tangible matter to the space of the actor
                            if (space.AddTangibleMatter(tangibleMatter) == Message.RelationSuccess)
                            {
                                actor.IsModified = true;
                                tangibleMatter.IsModified = true;
                                space.IsModified = true;
                                return true;
                            }
                            // If this fails, add it back to the original tangible matter space
                            else if (itemSpace != null)
                                itemSpace.AddTangibleMatter(tangibleMatter);
                        }
                    }
                }
            }
            return false;
        }
        #endregion Method: Take(PhysicalObjectInstance actor, MatterInstance tangibleMatter, SpaceInstance space)

        #region Method: Discard(PhysicalObjectInstance actor, TangibleObjectInstance item)
        /// <summary>
        /// Let the actor discard the item, after which it will be removed from its current space, and put in the space in which the actor is.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="item">The item to discard.</param>
        /// <returns>Returns whether the discard action has been successful.</returns>
        public static bool Discard(PhysicalObjectInstance actor, TangibleObjectInstance item)
        {
            if (actor != null && item != null && !actor.Equals(item))
            {
                // Get the space of the actor that contains the item
                foreach (SpaceInstance space in actor.Spaces)
                {
                    if (space.Items.Contains(item))
                    {
                        // Remove the item from the space
                        if (space.RemoveItem(item) == Message.RelationSuccess)
                        {
                            Vec3 position = new Vec3(item.Position);

                            // Try to add the item to the space in which the actor is;
                            // because the actor can be a tangible object instance, or a space instance, loop until the first tangible object instance is found
                            while (actor != null)
                            {
                                TangibleObjectInstance tangibleObjectActor = actor as TangibleObjectInstance;
                                if (tangibleObjectActor != null)
                                {
                                    if (tangibleObjectActor.Space != null)
                                    {
                                        tangibleObjectActor.Space.AddItem(item);

                                        // Restore the position
                                        item.Position = position;
                                    }
                                    return true;
                                }
                                else
                                {
                                    SpaceInstance spaceActor = actor as SpaceInstance;
                                    if (spaceActor != null)
                                        actor = spaceActor.PhysicalObject;
                                }
                            }

                            actor.IsModified = true;
                            item.IsModified = true;
                            return true;
                        }
                        else
                            return false;
                    }
                }
            }
            return false;
        }
        #endregion Method: Discard(PhysicalObjectInstance actor, TangibleObjectInstance item)

        #region Method: Discard(PhysicalObjectInstance actor, MatterInstance tangibleMatter)
        /// <summary>
        /// Let the actor discard the tangible matter, after which it will be removed from its current space, and put in the space in which the actor is.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="tangibleMatter">The tangible matter to discard.</param>
        /// <returns>Returns whether the discard action has been successful.</returns>
        public static bool Discard(PhysicalObjectInstance actor, MatterInstance tangibleMatter)
        {
            if (actor != null && tangibleMatter != null)
            {
                // Get the space of the actor that contains the tangible matter
                foreach (SpaceInstance space in actor.Spaces)
                {
                    if (space.TangibleMatter.Contains(tangibleMatter))
                    {
                        // Remove the tangible matter from the space
                        if (space.RemoveTangibleMatter(tangibleMatter) == Message.RelationSuccess)
                        {
                            Vec3 position = new Vec3(tangibleMatter.Position);

                            // Try to add the tangible matter to the space in which the actor is;
                            // because the actor can be a tangible object instance, or a space instance, loop until the first tangible object instance is found
                            while (actor != null)
                            {
                                TangibleObjectInstance tangibleObjectActor = actor as TangibleObjectInstance;
                                if (tangibleObjectActor != null)
                                {
                                    if (tangibleObjectActor.Space != null)
                                        tangibleObjectActor.Space.AddTangibleMatter(tangibleMatter, position);
                                    return true;
                                }
                                else
                                {
                                    SpaceInstance spaceActor = actor as SpaceInstance;
                                    if (spaceActor != null)
                                        actor = spaceActor.PhysicalObject;
                                }
                            }

                            actor.IsModified = true;
                            tangibleMatter.IsModified = true;
                            return true;
                        }
                        else
                            return false;
                    }
                }
            }
            return false;
        }
        #endregion Method: Discard(PhysicalObjectInstance actor, MatterInstance tangibleMatter)

        #region Method: Attach(TangibleObjectInstance part, TangibleObjectInstance whole)
        /// <summary>
        /// Attach the part to the whole.
        /// </summary>
        /// <param name="part">The part to attach.</param>
        /// <param name="whole">The whole to attach to.</param>
        /// <returns>Returns whether the attach action has been successful.</returns>
        public static bool Attach(TangibleObjectInstance part, TangibleObjectInstance whole)
        {
            if (part != null && whole != null && !part.Equals(whole))
            {
                // Remove the part from its current whole
                TangibleObjectInstance partWhole = part.Whole;
                if (partWhole != null)
                    partWhole.RemovePart(part);

                // Add the part to the whole
                if (whole.AddPart(part) == Message.RelationSuccess)
                {
                    part.IsModified = true;
                    whole.IsModified = true;
                    return true;
                }
                // If this fails, add the part back to its original whole
                else
                    partWhole.AddPart(part);
            }
            return false;
        }
        #endregion Method: Attach(TangibleObjectInstance part, TangibleObjectInstance whole)

        #region Method: Attach(TangibleObjectInstance part, TangibleObjectInstance whole, PhysicalObjectInstance actor)
        /// <summary>
        /// Let the actor attach the part to the whole, assuming the actor has the part in one of its spaces, after which the part will be removed from that space.
        /// </summary>
        /// <param name="part">The part to attach.</param>
        /// <param name="whole">The whole to attach to.</param>
        /// <param name="actor">The actor with the part to attach.</param>
        /// <returns>Returns whether the attach action has been successful.</returns>
        public static bool Attach(TangibleObjectInstance part, TangibleObjectInstance whole, PhysicalObjectInstance actor)
        {
            if (part != null && whole != null && actor != null && !part.Equals(whole) && !part.Equals(actor) && !whole.Equals(actor)
                && SemanticsEngine.Current.IsAllowedToPerformAction(actor, whole))
            {
                // Remove the part from the actor
                if (Discard(actor, part))
                {
                    // Attach the part to the whole
                    if (Attach(part, whole))
                    {
                        actor.IsModified = true;
                        return true;
                    }
                    // If this fails, let the actor take back the part
                    else
                        Take(actor, part);
                }
            }
            return false;
        }
        #endregion Method: Attach(TangibleObjectInstance part, TangibleObjectInstance whole, PhysicalObjectInstance actor)

        #region Method: Detach(TangibleObjectInstance part, TangibleObjectInstance whole)
        /// <summary>
        /// Detach the part from the whole.
        /// </summary>
        /// <param name="part">The part to detach.</param>
        /// <param name="whole">The whole to detach from.</param>
        /// <returns>Returns whether the detach action has been successful.</returns>
        public static bool Detach(TangibleObjectInstance part, TangibleObjectInstance whole)
        {
            if (part != null && whole != null && !part.Equals(whole))
            {
                if (whole.RemovePart(part) == Message.RelationSuccess)
                {
                    part.IsModified = true;
                    whole.IsModified = true;
                    return true;
                }
            }
            return false;
        }
        #endregion Method: Detach(TangibleObjectInstance part, TangibleObjectInstance whole)

        #region Method: Detach(TangibleObjectInstance part, TangibleObjectInstance whole, PhysicalObjectInstance actor)
        /// <summary>
        /// Let the actor detach the part from the whole, after which the part will be added to one of the spaces of the actor.
        /// </summary>
        /// <param name="part">The part to detach.</param>
        /// <param name="whole">The whole to detach from.</param>
        /// <param name="actor">The actor that should receive the detached part.</param>
        /// <returns>Returns whether the detach action has been successful.</returns>
        public static bool Detach(TangibleObjectInstance part, TangibleObjectInstance whole, PhysicalObjectInstance actor)
        {
            if (part != null && whole != null && actor != null)
            {
                // Put the part in the default space of the actor
                if (actor.DefaultSpace != null)
                {
                    if (Detach(part, whole, actor, actor.DefaultSpace))
                    {
                        actor.IsModified = true;
                        actor.DefaultSpace.IsModified = true;
                    }
                }
            }
            return false;
        }
        #endregion Method: Detach(TangibleObjectInstance part, TangibleObjectInstance whole, PhysicalObjectInstance actor)

        #region Method: Detach(TangibleObjectInstance part, TangibleObjectInstance whole, PhysicalObjectInstance actor, SpaceInstance space)
        /// <summary>
        /// Let the actor detach the part from the whole, after which the part will be added to the given space of the actor.
        /// </summary>
        /// <param name="part">The part to detach.</param>
        /// <param name="whole">The whole to detach from.</param>
        /// <param name="actor">The actor that should receive the detached part.</param>
        /// <param name="space">The space of the actor to put the detached part in.</param>
        /// <returns>Returns whether the detach action has been successful.</returns>
        public static bool Detach(TangibleObjectInstance part, TangibleObjectInstance whole, PhysicalObjectInstance actor, SpaceInstance space)
        {
            if (part != null && whole != null && actor != null && space != null && !part.Equals(whole) && !part.Equals(actor) && !whole.Equals(actor)
                && !actor.Equals(space) && actor.Spaces.Contains(space) && SemanticsEngine.Current.IsAllowedToPerformAction(actor, whole))
            {
                // Detach the part from the whole
                if (Detach(part, whole))
                {
                    // Let the actor take the part
                    if (Take(actor, part, space))
                        return true;
                    // If this fails, re-attach the part
                    else
                        Attach(part, whole);
                }
            }
            return false;
        }
        #endregion Method: Detach(TangibleObjectInstance part, TangibleObjectInstance whole, PhysicalObjectInstance actor, SpaceInstance space)

        #region Method: Connect(TangibleObjectInstance itemToConnect, TangibleObjectInstance itemToConnectTo)
        /// <summary>
        /// Connect an item to another item.
        /// </summary>
        /// <param name="itemToConnect">The item to connect.</param>
        /// <param name="itemToConnectTo">The item to connect to.</param>
        /// <returns>Returns whether the connect action has been successful.</returns>
        public static bool Connect(TangibleObjectInstance itemToConnect, TangibleObjectInstance itemToConnectTo)
        {
            if (itemToConnect != null && itemToConnectTo != null && !itemToConnect.Equals(itemToConnectTo))
            {
                if (TangibleObjectInstance.CreateConnection(itemToConnect, itemToConnectTo) == Message.RelationSuccess)
                {
                    itemToConnect.IsModified = true;
                    itemToConnectTo.IsModified = true;
                    return true;
                }
            }
            return false;
        }
        #endregion Method: Connect(TangibleObjectInstance itemToConnect, TangibleObjectInstance itemToConnectTo)

        #region Method: Connect(TangibleObjectInstance itemToConnect, TangibleObjectInstance itemToConnectTo, PhysicalObjectInstance actor)
        /// <summary>
        /// Let the actor connect an item to another item, assuming the actor has the item to connect in one of its spaces, after which the item will be removed from that space.
        /// </summary>
        /// <param name="itemToConnect">The item to connect.</param>
        /// <param name="itemToConnectTo">The item to connect to.</param>
        /// <param name="actor">The actor with the item to connect.</param>
        /// <returns>Returns whether the connect action has been successful.</returns>
        public static bool Connect(TangibleObjectInstance itemToConnect, TangibleObjectInstance itemToConnectTo, PhysicalObjectInstance actor)
        {
            if (itemToConnect != null && itemToConnectTo != null && actor != null && !itemToConnect.Equals(itemToConnectTo) &&
                !itemToConnect.Equals(actor) && !itemToConnectTo.Equals(actor) && SemanticsEngine.Current.IsAllowedToPerformAction(actor, itemToConnectTo))
            {
                // Remove the item to connect from the actor
                if (Discard(actor, itemToConnect))
                {
                    // Create the connection
                    if (Connect(itemToConnect, itemToConnectTo))
                        return true;
                    // If this fails, let the actor take back the item to connect
                    else
                        Take(actor, itemToConnect);
                }
            }
            return false;
        }
        #endregion Method: Connect(TangibleObjectInstance itemToConnect, TangibleObjectInstance itemToConnectTo, PhysicalObjectInstance actor)

        #region Method: Disconnect(TangibleObjectInstance itemToDisconnect, TangibleObjectInstance itemToDisconnectFrom)
        /// <summary>
        /// Disconnect an item from another item.
        /// </summary>
        /// <param name="itemToDisconnect">The item to disconnect.</param>
        /// <param name="itemToDisconnectFrom">The item to disconnect from.</param>
        /// <returns>Returns whether the disconnect action has been successful.</returns>
        public static bool Disconnect(TangibleObjectInstance itemToDisconnect, TangibleObjectInstance itemToDisconnectFrom)
        {
            if (itemToDisconnect != null && itemToDisconnectFrom != null && !itemToDisconnect.Equals(itemToDisconnectFrom))
            {
                if (TangibleObjectInstance.RemoveConnection(itemToDisconnect, itemToDisconnectFrom) == Message.RelationSuccess)
                {
                    itemToDisconnect.IsModified = true;
                    itemToDisconnectFrom.IsModified = true;
                    return true;
                }
            }
            return false;
        }
        #endregion Method: Disconnect(TangibleObjectInstance itemToDisconnect, TangibleObjectInstance itemToDisconnectFrom)

        #region Method: Disconnect(TangibleObjectInstance itemToDisconnect, TangibleObjectInstance itemToDisconnectFrom, PhysicalObjectInstance actor)
        /// <summary>
        /// Let the actor disconnect an item from another item, after which the disconnected item will be added to one of the spaces of the actor.
        /// </summary>
        /// <param name="itemToDisconnect">The item to disconnect.</param>
        /// <param name="itemToDisconnectFrom">The item to disconnect from.</param>
        /// <param name="actor">The actor that should receive the disconnected item.</param>
        /// <returns>Returns whether the disconnect action has been successful.</returns>
        public static bool Disconnect(TangibleObjectInstance itemToDisconnect, TangibleObjectInstance itemToDisconnectFrom, PhysicalObjectInstance actor)
        {
            if (itemToDisconnect != null && itemToDisconnectFrom != null && actor != null && !itemToDisconnect.Equals(itemToDisconnectFrom) && !itemToDisconnect.Equals(actor) && !itemToDisconnectFrom.Equals(actor))
            {
                // Put the disconnected item in the default space of the actor
                if (actor.DefaultSpace != null)
                    return Disconnect(itemToDisconnect, itemToDisconnectFrom, actor, actor.DefaultSpace);
            }
            return false;
        }
        #endregion Method: Disconnect(TangibleObjectInstance itemToDisconnect, TangibleObjectInstance itemToDisconnectFrom, PhysicalObjectInstance actor)

        #region Method: Disconnect(TangibleObjectInstance itemToDisconnect, TangibleObjectInstance itemToDisconnectFrom, PhysicalObjectInstance actor, SpaceInstance space)
        /// <summary>
        /// Let the actor disconnect an item from another item, after which the disconnected item will be added to the given space of the actor.
        /// </summary>
        /// <param name="itemToDisconnect">The item to disconnect.</param>
        /// <param name="itemToDisconnectFrom">The item to disconnect from.</param>
        /// <param name="actor">The actor that should receive the disconnected item.</param>
        /// <param name="space">The space of the actor to put the disconnected item in.</param>
        /// <returns>Returns whether the disconnect action has been successful.</returns>
        public static bool Disconnect(TangibleObjectInstance itemToDisconnect, TangibleObjectInstance itemToDisconnectFrom, PhysicalObjectInstance actor, SpaceInstance space)
        {
            if (itemToDisconnect != null && itemToDisconnectFrom != null && actor != null && space != null && !itemToDisconnect.Equals(itemToDisconnectFrom)
                && !itemToDisconnect.Equals(actor) && !itemToDisconnectFrom.Equals(actor) && !actor.Equals(space) && SemanticsEngine.Current.IsAllowedToPerformAction(actor, itemToDisconnectFrom))
            {
                // Disconnect the items
                if (Disconnect(itemToDisconnect, itemToDisconnectFrom))
                {
                    // Let the actor take the disconnected item
                    if (Take(actor, itemToDisconnect, space))
                        return true;
                    // If this fails, re-connect the item
                    else
                        Connect(itemToDisconnect, itemToDisconnectFrom);
                }
            }
            return false;
        }
        #endregion Method: Disconnect(TangibleObjectInstance itemToDisconnect, TangibleObjectInstance itemToDisconnectFrom, PhysicalObjectInstance actor, SpaceInstance space)

        #region Method: Cover(TangibleObjectInstance cover, TangibleObjectInstance instanceToCover)
        /// <summary>
        /// Cover an instance.
        /// </summary>
        /// <param name="cover">The cover.</param>
        /// <param name="instanceToCover">The instance to cover.</param>
        /// <returns>Returns whether the cover action has been successful.</returns>
        public static bool Cover(TangibleObjectInstance cover, TangibleObjectInstance instanceToCover)
        {
            if (cover != null && instanceToCover != null && !cover.Equals(instanceToCover))
            {
                // Remove the cover from its current owner
                TangibleObjectInstance coveredInstance = cover.CoveredObject;
                if (coveredInstance != null)
                    coveredInstance.RemoveCover(cover);

                // Add the cover to the instance to cover
                if (instanceToCover.AddCover(cover) == Message.RelationSuccess)
                {
                    cover.IsModified = true;
                    instanceToCover.IsModified = true;
                    return true;
                }
                // If this fails, add the part back to its original whole
                else
                {
                    if (coveredInstance != null)
                        coveredInstance.AddCover(cover);
                }
            }
            return false;
        }
        #endregion Method: Cover(TangibleObjectInstance cover, TangibleObjectInstance instanceToCover)

        #region Method: Cover(TangibleObjectInstance cover, TangibleObjectInstance instanceToCover, PhysicalObjectInstance actor)
        /// <summary>
        /// Let the actor cover an instance, assuming the actor has the cover in one of its spaces, after which the cover will be removed from that space.
        /// </summary>
        /// <param name="cover">The cover.</param>
        /// <param name="instanceToCover">The instance to cover.</param>
        /// <param name="actor">The actor with the cover.</param>
        /// <returns>Returns whether the cover action has been successful.</returns>
        public static bool Cover(TangibleObjectInstance cover, TangibleObjectInstance instanceToCover, PhysicalObjectInstance actor)
        {
            if (cover != null && instanceToCover != null && actor != null && !cover.Equals(instanceToCover) &&
                SemanticsEngine.Current.IsAllowedToPerformAction(actor, instanceToCover))
            {
                // Remove the cover from the actor
                if (Discard(actor, cover))
                {
                    // Cover the instance
                    if (Cover(cover, instanceToCover))
                    {
                        actor.IsModified = true;
                        return true;
                    }
                    // If this fails, let the actor take back the cover
                    else
                        Take(actor, cover);
                }
            }
            return false;
        }
        #endregion Method: Cover(TangibleObjectInstance cover, TangibleObjectInstance instanceToCover, PhysicalObjectInstance actor)

        #region Method: Uncover(TangibleObjectInstance cover, TangibleObjectInstance instanceToUncover)
        /// <summary>
        /// Uncover an instance.
        /// </summary>
        /// <param name="cover">The cover.</param>
        /// <param name="instanceToUncover">The instance to uncover.</param>
        /// <returns>Returns whether the uncover action has been successful.</returns>
        public static bool Uncover(TangibleObjectInstance cover, TangibleObjectInstance instanceToUncover)
        {
            if (cover != null && instanceToUncover != null && !cover.Equals(instanceToUncover))
            {
                if (instanceToUncover.RemoveCover(cover) == Message.RelationSuccess)
                {
                    cover.IsModified = true;
                    instanceToUncover.IsModified = true;
                    return true;
                }
            }
            return false;
        }
        #endregion Method: Uncover(TangibleObjectInstance cover, TangibleObjectInstance instanceToUncover)

        #region Method: Uncover(TangibleObjectInstance cover, TangibleObjectInstance instanceToUncover, PhysicalObjectInstance actor)
        /// <summary>
        /// Let the actor uncover an instance, after which the cover will be added to one of the spaces of the actor.
        /// </summary>
        /// <param name="cover">The cover.</param>
        /// <param name="instanceToUncover">The instance to uncover.</param>
        /// <param name="actor">The actor that should receive the detached part.</param>
        /// <returns>Returns whether the detach action has been successful.</returns>
        public static bool Uncover(TangibleObjectInstance cover, TangibleObjectInstance instanceToUncover, PhysicalObjectInstance actor)
        {
            if (cover != null && instanceToUncover != null && actor != null)
            {
                // Put the cover in the default space of the actor
                if (actor.DefaultSpace != null)
                    return Uncover(cover, instanceToUncover, actor, actor.DefaultSpace);
            }
            return false;
        }
        #endregion Method: Uncover(TangibleObjectInstance cover, TangibleObjectInstance instanceToUncover, PhysicalObjectInstance actor)

        #region Method: Uncover(TangibleObjectInstance cover, TangibleObjectInstance instanceToUncover, PhysicalObjectInstance actor, SpaceInstance space)
        /// <summary>
        /// Let the actor uncover an instance, after which the cover will be added to the given space of the actor.
        /// </summary>
        /// <param name="cover">The cover.</param>
        /// <param name="instanceToUncover">The instance to uncover.</param>
        /// <param name="actor">The actor that should receive the cover.</param>
        /// <param name="space">The space of the actor to put the cover in.</param>
        /// <returns>Returns whether the uncover action has been successful.</returns>
        public static bool Uncover(TangibleObjectInstance cover, TangibleObjectInstance instanceToUncover, PhysicalObjectInstance actor, SpaceInstance space)
        {
            if (cover != null && instanceToUncover != null && actor != null && space != null && !cover.Equals(instanceToUncover) &&
                !actor.Equals(space) && actor.Spaces.Contains(space) && SemanticsEngine.Current.IsAllowedToPerformAction(actor, instanceToUncover))
            {
                // Uncover the cover
                if (Uncover(cover, instanceToUncover))
                {
                    // Let the actor take the cover
                    if (Take(actor, cover, space))
                        return true;
                    // If this fails, re-cover the cover
                    else
                        Cover(cover, instanceToUncover);
                }
            }
            return false;
        }
        #endregion Method: Uncover(TangibleObjectInstance cover, TangibleObjectInstance instanceToUncover, PhysicalObjectInstance actor, SpaceInstance space)

        #region Method: Apply(MatterInstance layer, TangibleObjectInstance applicant)
        /// <summary>
        /// Apply a layer to a tangible object.
        /// </summary>
        /// <param name="layer">The layer to apply.</param>
        /// <param name="applicant">The tangible object to apply the layer to.</param>
        /// <returns>Returns whether the apply action has been successful.</returns>
        public static bool Apply(MatterInstance layer, TangibleObjectInstance applicant)
        {
            if (layer != null && applicant != null && !layer.Equals(applicant))
            {
                // Remove the layer from its current applicant
                TangibleObjectInstance originalApplicant = layer.Applicant;
                if (originalApplicant != null)
                    originalApplicant.RemoveLayer(layer);

                // Apply the layer to the tangible object
                if (applicant.AddLayer(layer) == Message.RelationSuccess)
                {
                    layer.IsModified = true;
                    applicant.IsModified = true;
                    return true;
                }
                // If this fails, apply the layer back to its original applicant
                else
                    originalApplicant.AddLayer(layer);
            }
            return false;
        }
        #endregion Method: Apply(MatterInstance layer, TangibleObjectInstance applicant)

        #region Method: Remove(MatterInstance layer, TangibleObjectInstance applicant)
        /// <summary>
        /// Remove a layer from a tangible object.
        /// </summary>
        /// <param name="layer">The layer to remove.</param>
        /// <param name="applicant">The tangible object from which the layer should be removed.</param>
        /// <returns>Returns whether the remove action has been successful.</returns>
        public static bool Remove(MatterInstance layer, TangibleObjectInstance applicant)
        {
            if (layer != null && applicant != null && !layer.Equals(applicant))
            {
                if (applicant.RemoveLayer(layer) == Message.RelationSuccess)
                {
                    layer.IsModified = true;
                    applicant.IsModified = true;
                    return true;
                }
            }
            return false;
        }
        #endregion Method: Remove(MatterInstance layer, TangibleObjectInstance applicant)

        #region Method: MoveTo(TangibleObjectInstance actor, SpaceInstance space)
        /// <summary>
        /// Let the actor move to the given space, removing it from its current space.
        /// Note that only the space is changed, not the position!
        /// </summary>
        public static bool MoveTo(TangibleObjectInstance actor, SpaceInstance space)
        {
            if (actor != null && space != null)
            {
                if (space.AddItem(actor, true) == Message.RelationSuccess)
                {
                    actor.IsModified = true;
                    space.IsModified = true;
                    return true;
                }
            }
            return false;
        }
        #endregion Method: MoveTo(TangibleObjectInstance actor, SpaceInstance space)
		
    }
    #endregion Class: SpecialActionsHandler

}