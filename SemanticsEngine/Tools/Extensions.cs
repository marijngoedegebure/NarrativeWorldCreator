/**************************************************************************
 * 
 * Extensions.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 * 
 **************************************************************************/

using System;

namespace SemanticsEngine.Tools
{

    #region Class: EnumExtensions
    /// <summary>
    /// A class with extensions for an enum.
    /// http://www.codeproject.com/Articles/37921/Enums-Flags-and-C-Oh-my-bad-pun
    /// </summary>
    public static class EnumExtensions
    {

        #region Method: Has<T>(this Enum type, T value)
        /// <summary>
        /// Checks to see if an enumerated value contains a type.
        /// </summary>
        /// <typeparam name="T">The type to check.</typeparam>
        /// <param name="type">The enum.</param>
        /// <param name="value">The value of the type.</param>
        /// <returns>Returns whether the enumerated value contains the type.</returns>
        public static bool Has<T>(this Enum type, T value)
        {
            try
            {
                return (((int)(object)type &
                  (int)(object)value) == (int)(object)value);
            }
            catch
            {
                return false;
            }
        }
        #endregion Method: Has<T>(this Enum type, T value)

        #region Method: Add<T>(this Enum type, T value)
        /// <summary>
        /// Appends a value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Add<T>(this Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type | (int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("Could not append value from enumerated type '{0}'.", typeof(T).Name), ex);
            }
        }
        #endregion Method: Add<T>(this Enum type, T value)

        #region Method: Remove<T>(this Enum type, T value)
        /// <summary>
        /// Completely removes the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Remove<T>(this Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type & ~(int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("Could not remove value from enumerated type '{0}'.", typeof(T).Name), ex);
            }
        }
        #endregion Method: Remove<T>(this Enum type, T value)

    }
    #endregion Class: EnumExtensions

}