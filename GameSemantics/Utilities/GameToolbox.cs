/**************************************************************************
 * 
 * GameToolbox.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.Globalization;

namespace GameSemantics.Utilities
{

    #region Class: GameToolbox
    /// <summary>
    /// Several useful tools, which can be used by several classes.
    /// </summary>
    public static class GameToolbox
    {

        #region Method: ChangeCulture(CultureInfo culture)
        /// <summary>
        /// Changes the culture for enums and new names.
        /// </summary>
        /// <param name="culture">The culture to change to.</param>
        public static void ChangeCulture(CultureInfo culture)
        {
            if (culture != null)
            {
                Enums.Culture = culture;
                NewNames.Culture = culture;
            }
        }
        #endregion Method: ChangeCulture(CultureInfo culture)

    }
    #endregion Class: GameToolbox

}