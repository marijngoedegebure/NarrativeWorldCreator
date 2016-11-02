/**************************************************************************
 * 
 * Enums.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 * 
 **************************************************************************
 * 
 * A collection of enums.
 *
 *************************************************************************/

using System;
using System.ComponentModel;
using Common;

namespace SemanticsEngine.Tools
{

    #region Enum: ActionType
    /// <summary>
    /// The type of an action.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ActionType
    {
        /// <summary>
        /// A manual action.
        /// </summary>
        Manual = 0,

        /// <summary>
        /// A special action.
        /// </summary>
        Special = 1
    }
    #endregion Enum: ActionType

    #region Enum: CreateOptions
    /// <summary>
    /// Defines options for the creation of instances.
    /// </summary>
    [Flags]
    public enum CreateOptions : int
    {
        /// <summary>
        /// Indicates that nothing should be created.
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates whether connections of tangible objects should be created.
        /// </summary>
        Connections = 1,

        /// <summary>
        /// Indicates whether covers should be created.
        /// </summary>
        Covers = 2,

        /// <summary>
        /// Indicates whether space items should be created.
        /// </summary>
        Items = 4,

        /// <summary>
        /// Indicates whether matter of tangible objects should be created.
        /// </summary>
        Matter = 8,

        /// <summary>
        /// Indicates whether parts of tangible objects should be created.
        /// </summary>
        Parts = 16,

        /// <summary>
        /// Indicates whether spaces of physical objects should be created.
        /// </summary>
        Spaces = 32,

        /// <summary>
        /// Indicates whether tangible matter in spaces should be created.
        /// </summary>
        TangibleMatter = 64
    }
    #endregion Enum: CreateOptions

    #region Enum: SpecialAction
    /// <summary>
    /// The special actions.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum SpecialAction
    {
        /// <summary>
        /// Apply.
        /// </summary>
        Apply,

        /// <summary>
        /// Attach.
        /// </summary>
        Attach,

        /// <summary>
        /// Connect.
        /// </summary>
        Connect,

        /// <summary>
        /// Cover.
        /// </summary>
        Cover,

        /// <summary>
        /// Detach.
        /// </summary>
        Detach,

        /// <summary>
        /// Discard.
        /// </summary>
        Discard,

        /// <summary>
        /// Disconnect.
        /// </summary>
        Disconnect,

        /// <summary>
        /// Give.
        /// </summary>
        Give,

        /// <summary>
        /// Remove.
        /// </summary>
        Remove,

        /// <summary>
        /// Take.
        /// </summary>
        Take,

        /// <summary>
        /// Uncover.
        /// </summary>
        Uncover
    }
    #endregion Enum: SpecialAction

    #region Class: LocalizedEnumConverter
    /// <summary>
    /// Defines a type converter for enum types defined in this project.
    /// http://www.codeproject.com/KB/cs/LocalizingEnums.aspx
    /// </summary>
    class LocalizedEnumConverter : ResourceEnumConverter
    {
        /// <summary>
        /// Create a new instance of the converter using translations from the given resource manager.
        /// </summary>
        public LocalizedEnumConverter(Type type)
            : base(type, Enums.ResourceManager)
        {
        }
    }
    #endregion Class: LocalizedEnumConverter

}