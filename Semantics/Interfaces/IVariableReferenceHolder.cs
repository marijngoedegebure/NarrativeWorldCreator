/**************************************************************************
 * 
 * IVariableReferenceHolder.cs
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

namespace Semantics.Interfaces
{

    #region Interface: IVariableReferenceHolder
    /// <summary>
    /// An interface for a class that should hold variables and references.
    /// </summary>
    public interface IVariableReferenceHolder
    {

        /// <summary>
        /// Adds the given variable.
        /// </summary>
        /// <param name="variable">The variable to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        Message AddVariable(Variable variable);

        /// <summary>
        /// Removes an variable.
        /// </summary>
        /// <param name="variable">The variable to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        Message RemoveVariable(Variable variable);

        /// <summary>
        /// Adds the given reference.
        /// </summary>
        /// <param name="reference">The reference to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        Message AddReference(Reference reference);

        /// <summary>
        /// Removes an reference.
        /// </summary>
        /// <param name="reference">The reference to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        Message RemoveReference(Reference reference);

    }
    #endregion Interface: IVariableReferenceHolder

}
