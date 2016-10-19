/**************************************************************************
 * 
 * Settings.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

namespace GameSemantics.Utilities
{

    #region Struct: GameSemanticsSettings
    /// <summary>
    /// Several default settings.
    /// </summary>
    public struct GameSemanticsSettings
    {

        #region Struct: Files
        /// <summary>
        /// Files.
        /// </summary>
        public struct Files
        {
            /// <summary>
            /// The SQL file for semantics queries.
            /// </summary>
            public const string SemanticsSQLFile = "game_semantics.sql";

            /// <summary>
            /// The SQL file for values queries.
            /// </summary>
            public const string ValuesSQLFile = "game_values.sql";

            /// <summary>
            /// The SQL file for localization queries.
            /// </summary>
            public const string LocalizationSQLFile = "game_localization.sql";
        }
        #endregion Struct: Files

        #region Struct: Values
        /// <summary>
        /// Default values.
        /// </summary>
        public struct Values
        {
            /// <summary>
            /// The default value for loop.
            /// </summary>
            public const bool Loop = false;

            /// <summary>
            /// The default value for the X value of a dimension.
            /// </summary>
            public const float DimensionX = 10;

            /// <summary>
            /// The default value for the Y value of a dimension.
            /// </summary>
            public const float DimensionY = 10;

            /// <summary>
            /// The default value for a rotation.
            /// </summary>
            public const float Rotation = 0;

            /// <summary>
            /// The default value for the X value of a rotation.
            /// </summary>
            public const float RotationX = 0;

            /// <summary>
            /// The default value for the Y value of a rotation.
            /// </summary>
            public const float RotationY = 0;

            /// <summary>
            /// The default value for the Z value of a rotation.
            /// </summary>
            public const float RotationZ = 0;

            /// <summary>
            /// The default value for the X value of a scale.
            /// </summary>
            public const float ScaleX = 1;

            /// <summary>
            /// The default value for the Y value of a scale.
            /// </summary>
            public const float ScaleY = 1;

            /// <summary>
            /// The default value for the Z value of a scale.
            /// </summary>
            public const float ScaleZ = 1;

            /// <summary>
            /// The default value for playback speed.
            /// </summary>
            public const float Speed = 1;

            /// <summary>
            /// The default value for the X value of a translation.
            /// </summary>
            public const float TranslationX = 0;

            /// <summary>
            /// The default value for the Y value of a translation.
            /// </summary>
            public const float TranslationY = 0;

            /// <summary>
            /// The default value for the Z value of a translation.
            /// </summary>
            public const float TranslationZ = 0;

            /// <summary>
            /// The default value for a volume.
            /// </summary>
            public const int Volume = 100;

            /// <summary>
            /// The default value for a Z-index.
            /// </summary>
            public const int ZIndex = 0;
        }
        #endregion Struct: Values

    }
    #endregion Struct: GameSemanticsSettings

}
