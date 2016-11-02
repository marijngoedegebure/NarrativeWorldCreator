/**************************************************************************
 * 
 * IEventExtension.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.Collections.ObjectModel;
using SemanticsEngine.Components;

namespace SemanticsEngine.Interfaces
{

    #region Interface: IEventExtension
    /// <summary>
    /// An extension for events.
    /// </summary>
    public interface IEventExtension
    {

        /// <summary>
        /// Get the actor conditions of the extension.
        /// </summary>
        /// <returns>The actor conditions of the extension.</returns>
        ReadOnlyCollection<ConditionBase> GetActorConditions();

        /// <summary>
        /// Get the target conditions of the extension.
        /// </summary>
        /// <returns>The target conditions of the extension.</returns>
        ReadOnlyCollection<ConditionBase> GetTargetConditions();

        /// <summary>
        /// Get the artifact conditions of the extension.
        /// </summary>
        /// <returns>The artifact conditions of the extension.</returns>
        ReadOnlyCollection<ConditionBase> GetArtifactConditions();

        /// <summary>
        /// Get the effects of the extension.
        /// </summary>
        /// <returns>The effects of the extension.</returns>
        ReadOnlyCollection<EffectBase> GetEffects();

    }
    #endregion Interface: IEventExtension

}
