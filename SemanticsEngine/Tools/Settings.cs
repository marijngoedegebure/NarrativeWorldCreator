/**************************************************************************
 * 
 * Settings.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

namespace SemanticsEngine.Tools
{

    #region Struct: SemanticsEngineSettings
    /// <summary>
    /// Several default settings.
    /// </summary>
    public struct SemanticsEngineSettings
    {

        /// <summary>
        /// The default values for the create options.
        /// </summary>
        public const CreateOptions DefaultCreateOptions = CreateOptions.Parts | CreateOptions.Items | CreateOptions.Covers | CreateOptions.Spaces | CreateOptions.Matter | CreateOptions.TangibleMatter;

        /// <summary>
        /// The multiplier to generate random values that are greater than a particular value.
        /// </summary>
        internal const int GreaterMultiplier = 3;

        /// <summary>
        /// The default name for a semantic world.
        /// </summary>
        internal const string WorldName = "World";

        /// <summary>
        /// The epsilon for distance checking.
        /// </summary>
        internal const float DistanceEpsilon = 10;

    }
    #endregion Struct: SemanticsEngineSettings

}
