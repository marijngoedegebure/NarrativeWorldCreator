/**************************************************************************
 * 
 * IStringRepresentative.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

namespace Common
{

    /// <summary>
    /// An interface to create a string of a particular class, and load the class from a string.
    /// </summary>
    public interface IStringRepresentative
    {
        /// <summary>
        /// Create a string with all class data.
        /// </summary>
        /// <returns>A string with all class data.</returns>
        string CreateString();

        /// <summary>
        /// Load the class data from the given string.
        /// </summary>
        /// <param name="stringToLoad">The string that contains the class data.</param>
        void LoadString(string stringToLoad);
    }

}
