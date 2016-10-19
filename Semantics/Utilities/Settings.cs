/**************************************************************************
 * 
 * Settings.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using Common;

namespace Semantics.Utilities
{

    #region Struct: SemanticsSettings
    /// <summary>
    /// Several default settings.
    /// </summary>
    public struct SemanticsSettings
    {

        #region Struct: General
        /// <summary>
        /// General settings.
        /// </summary>
        public struct General
        {
            /// <summary>
            /// The default bounding box name.
            /// </summary>
            public const String BoundingBoxName = "bb";

            /// <summary>
            /// The default equation result variable.
            /// </summary>
            public const String EquationResultVariable = "y";

            /// <summary>
            /// The default equation variable.
            /// </summary>
            public const String EquationVariable = "x";

            /// <summary>
            /// The default new name.
            /// </summary>
            public const String NewName = "New node";
        }
        #endregion Struct: General		

        #region Struct: Files
        /// <summary>
        /// Files.
        /// </summary>
        public struct Files
        {
            /// <summary>
            /// The path containing the database files.
            /// </summary>
            public const string DatabasePath = "..\\Content\\Databases\\";

            /// <summary>
            /// The extension for the database project files.
            /// </summary>
            public const string DatabaseProjectExtension = ".edp";

            /// <summary>
            /// The filter for the database project files.
            /// </summary>
            public const string DatabaseProjectFilter = "Entika Database Project (*.edp)|*.edp";

            /// <summary>
            /// The SQL file for semantics queries.
            /// </summary>
            public const string SemanticsSQLFile = "semantics.sql";

            /// <summary>
            /// The SQL file for values queries.
            /// </summary>
            public const string ValuesSQLFile = "values.sql";

            /// <summary>
            /// The SQL file for localization queries.
            /// </summary>
            public const string LocalizationSQLFile = "localization.sql";
        }
        #endregion Struct: Files

        #region Struct: Values
        /// <summary>
        /// Default values.
        /// </summary>
        public struct Values
        {
            /// <summary>
            /// The default value for an atomic number.
            /// </summary>
            public const byte AtomicNumber = 0;

            /// <summary>
            /// The default value for a boolean.
            /// </summary>
            public const bool Boolean = false;

            /// <summary>
            /// The default definition for a bounding box.
            /// </summary>
            public const string BoundingBoxDefinition = "ExtrudedShape(BB->MinX(), BB->MinZ(), BB->MinX(), BB->MaxZ(), BB->MaxX(), BB->MaxZ(), BB->MaxX(), BB->MinZ(), BB->MinY(), BB->DimY())";

            /// <summary>
            /// The default value for a capacity.
            /// </summary>
            public const float Capacity = 100;

            /// <summary>
            /// The default value for a chance.
            /// </summary>
            public const float Chance = 1;

            /// <summary>
            /// The default value for a chance dependency.
            /// </summary>
            public const bool ChanceDependency = false;

            /// <summary>
            /// The default value for a delay.
            /// </summary>
            public const float Delay = 0;

            /// <summary>
            /// The default value for a distance.
            /// </summary>
            public const float Distance = float.MaxValue;

            /// <summary>
            /// The default value for a distance sign.
            /// </summary>
            public const EqualitySignExtendedDual DistanceSign = EqualitySignExtendedDual.LowerOrEqual;

            /// <summary>
            /// The default value for a duration.
            /// </summary>
            public const int Duration = 10;

            /// <summary>
            /// The default equation string.
            /// </summary>
            public const String Equation = General.EquationVariable;

            /// <summary>
            /// The default value for a frequency.
            /// </summary>
            public const int Frequency = 1;

            /// <summary>
            /// The default value for has unlimited capacity.
            /// </summary>
            public const bool HasUnlimitedCapacity = false;

            /// <summary>
            /// The default value for an index.
            /// </summary>
            public const int Index = 1;

            /// <summary>
            /// The default value for an interval.
            /// </summary>
            public const float Interval = 1;

            /// <summary>
            /// The default value for is exclusive.
            /// </summary>
            public const bool IsExclusive = false;

            /// <summary>
            /// The default value for is random.
            /// </summary>
            public const bool IsRandom = false;

            /// <summary>
            /// The default value for removing everything.
            /// </summary>
            public const bool RemoveEverything = true;

            /// <summary>
            /// The default value for a level of detail.
            /// </summary>
            public const byte LevelOfDetail = 0;

            /// <summary>
            /// The default value for loop.
            /// </summary>
            public const bool Loop = false;

            /// <summary>
            /// The default maximum value for a chance.
            /// </summary>
            public const float MaxChance = 1;

            /// <summary>
            /// The default maximum for the number of items.
            /// </summary>
            public const int MaximumNumberOfItems = 100;

            /// <summary>
            /// The default maximum length.
            /// </summary>
            public const uint MaxLength = 100;

            /// <summary>
            /// The default maximum value.
            /// </summary>
            public const float MaxValue = float.MaxValue;

            /// <summary>
            /// The default minimum value for a chance.
            /// </summary>
            public const float MinChance = 0;

            /// <summary>
            /// The default minimum value.
            /// </summary>
            public const float MinValue = float.MinValue;

            /// <summary>
            /// The default value for the number of simultaneous uses of an event.
            /// </summary>
            public const int NrOfSimultaneousUses = 1;

            /// <summary>
            /// The default value for the number of atoms.
            /// </summary>
            public const int NumberOfAtoms = 0;

            /// <summary>
            /// The default value for the W-value of an offset.
            /// </summary>
            public const float OffsetW = 1;

            /// <summary>
            /// The default value for the X-value of an offset.
            /// </summary>
            public const float OffsetX = 0;

            /// <summary>
            /// The default value for the Y-value of an offset.
            /// </summary>
            public const float OffsetY = 0;

            /// <summary>
            /// The default value for the Z-value of an offset.
            /// </summary>
            public const float OffsetZ = 0;

            /// <summary>
            /// The default playback speed.
            /// </summary>
            public const float PlaybackSpeed = 1;

            /// <summary>
            /// The default value for the X-value of a position.
            /// </summary>
            public const float PositionX = 0;

            /// <summary>
            /// The default value for the Y-value of a position.
            /// </summary>
            public const float PositionY = 0;

            /// <summary>
            /// The default value for the Z-value of a position.
            /// </summary>
            public const float PositionZ = 0;

            /// <summary>
            /// The default value for a priority.
            /// </summary>
            public const int Priority = 0;

            /// <summary>
            /// The default value for a quantity.
            /// </summary>
            public const float Quantity = 1;

            /// <summary>
            /// The default value for a range.
            /// </summary>
            public const int Range = 1;

            /// <summary>
            /// The default value for a ratio.
            /// </summary>
            public const int Ratio = 1;

            /// <summary>
            /// The default value for the W-value of a rotation.
            /// </summary>
            public const float RotationW = 1;

            /// <summary>
            /// The default value for the X-value of a rotation.
            /// </summary>
            public const float RotationX = 0;

            /// <summary>
            /// The default value for the Y-value of a rotation.
            /// </summary>
            public const float RotationY = 0;

            /// <summary>
            /// The default value for the Z-value of a rotation.
            /// </summary>
            public const float RotationZ = 0;

            /// <summary>
            /// The default value for the selection amount.
            /// </summary>
            public const int SelectionAmount = 1;

            /// <summary>
            /// The default value for a string.
            /// </summary>
            public const string String = "";

            /// <summary>
            /// The default value for a symbol.
            /// </summary>
            public const string Symbol = "";

            /// <summary>
            /// The default value for thickness.
            /// </summary>
            public const int Thickness = 1;

            /// <summary>
            /// The default value.
            /// </summary>
            public const float Value = 0;

            /// <summary>
            /// The default value for the W-value of a 4-dimensional vector.
            /// </summary>
            public const float Vector4W = 1;

            /// <summary>
            /// The default value for the X-value of a 4-dimensional vector.
            /// </summary>
            public const float Vector4X = 0;

            /// <summary>
            /// The default value for the Y-value of a 4-dimensional vector.
            /// </summary>
            public const float Vector4Y = 0;

            /// <summary>
            /// The default value for the Z-value of a 4-dimensional vector.
            /// </summary>
            public const float Vector4Z = 0;

            /// <summary>
            /// The default value for a volume.
            /// </summary>
            public const int Volume = 100;
        }
        #endregion Struct: Values

    }
    #endregion Struct: SemanticsSettings

}
