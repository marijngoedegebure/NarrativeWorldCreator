using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NarrativeWorldCreator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator
{
    public static class SystemStateTracker
    {
        // Input constants
        public const string LocationTypeName = "place";
        public const string CharacterTypeName = "character";
        public const string ObjectTypeName = "thing";
        public const string MoveActionName = "move";
        public const string AtPredicateName = "at";

        public static NarrativeWorld NarrativeWorld = new NarrativeWorld();
        public static string LoadedFileName;
        public static string EntikaPath;

        public static GraphicsDevice RegionGraphicsDevice;
        public static Matrix view;
        public static Matrix world;
        public static Matrix proj;

        public static Model DefaultModel;

        public static Texture2D BoxSelectTexture;
        public static Texture2D RegionCreationTexture;

        // GPGPU settings
        // Number of results generated (threadblocks started):
        public static int gridxDim = 10;
        public const int gridyDim = 0;
        // Number of threads used for each threadblock, should be a multiple of 32 (probably 256)
        public const int blockxDim = 256;
        public const int blockyDim = 0;
        public const int blockzDim = 0;
        // Number of iterations a configuration will see before the best encountered configuration is returned
        public const int iterations = 100;

        // Algorithm
        // Weights for each cost function
        public static float WeightFocalPoint = 1.0f;
        public static float WeightPairWise = 1.0f;
        public static float WeightVisualBalance = 1.0f;
        public static float WeightSymmetry = 1.0f;
        public static float WeightClearance = 1.0f;
        public static float WeightSurfaceArea = 1.0f;
        // Configurable centroid and focal points in scene
        public static double centroidX = 0.0;
        public static double centroidY = 0.0;
        public static double focalX = 5.0;
        public static double focalY = 5.0;
        public static double focalRot = 0.0;
    }
}
