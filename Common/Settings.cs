/**************************************************************************
 * 
 * Settings.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.Globalization;

namespace Common
{

    #region Struct: Settings
    /// <summary>
    /// A class to store several common settings.
    /// </summary>
    public struct CommonSettings
    {

        /// <summary>
        /// The default culture info.
        /// </summary>
        public static CultureInfo Culture = new CultureInfo("en-US");

        /// <summary>
        /// The symbol to call a function, used by the math parser.
        /// </summary>
        public const string FunctionCallSymbol = "->";

        /// <summary>
        /// The symbol to end a function, used by the math parser.
        /// </summary>
        public const string FunctionEndSymbol = "()";

    }
    #endregion Struct: Settings

}
