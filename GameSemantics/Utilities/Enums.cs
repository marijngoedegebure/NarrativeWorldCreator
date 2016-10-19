/**************************************************************************
 * 
 * Enums.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 * 
 **************************************************************************
 * 
 * A collection of many enums.
 *
 *************************************************************************/

using System;
using System.ComponentModel;
using Common;

namespace GameSemantics.Utilities
{

    #region Enum: EventStateExtended
    /// <summary>
    /// The state of an event.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum EventStateExtended
    {
        /// <summary>
        /// The event starts.
        /// </summary>
        Starts = 0,

        /// <summary>
        /// The event is being executed.
        /// </summary>
        IsExecuting = 1,

        /// <summary>
        /// The event stops.
        /// </summary>
        Stops = 2
    }
    #endregion Enum: EventStateExtended

    #region Enum: ParticleRepresentation
    /// <summary>
    /// The representation of a particle; indicates whether it is represented by a file, a tangible object, or tangible matter.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum ParticleRepresentation
    {
        /// <summary>
        /// The particle is represented by a file.
        /// </summary>
        File = 0,

        /// <summary>
        /// The particle is represented by a tangible object.
        /// </summary>
        TangibleObject = 1,

        /// <summary>
        /// The particle is represented by tangible matter.
        /// </summary>
        TangibleMatter = 2
    }
    #endregion Enum: ParticleRepresentation

    #region Enum: StartStop
    /// <summary>
    /// Start or stop.
    /// </summary>
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum StartStop
    {
        /// <summary>
        /// Start.
        /// </summary>
        Start = 0,

        /// <summary>
        /// Stop.
        /// </summary>
        Stop = 1
    }
    #endregion Enum: StartStop

    #region Class: LocalizedEnumConverter
    /// <summary>
    /// Defines a type converter for enum types defined in this project.
    /// http://www.codeproject.com/KB/cs/LocalizingEnums.aspx
    /// </summary>
    class LocalizedEnumConverter : ResourceEnumConverter
    {
        /// <summary>
        /// Create a new instance of the converter using translations from the given resource manager
        /// </summary>
        public LocalizedEnumConverter(Type type)
            : base(type, Enums.ResourceManager)
        {
        }
    }
    #endregion Class: LocalizedEnumConverter

}