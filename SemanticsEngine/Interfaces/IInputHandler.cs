/**************************************************************************
 * 
 * IInputHandler.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using Semantics.Utilities;

namespace SemanticsEngine.Interfaces
{

    #region Interface: IInputHandler
    /// <summary>
    /// An interface that handles manual input.
    /// </summary>
    public interface IInputHandler
    {

        /// <summary>
        /// Request manual input for the given variable name and value type.
        /// </summary>
        /// <param name="variableName">The name of the variable.</param>
        /// <param name="valueType">The required value type.</param>
        /// <returns>Returns the manual input for the variable name of the given value type.</returns>
        object GetManualInput(string variableName, ValueType valueType);

    }
    #endregion Interface: IInputHandler

}