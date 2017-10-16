using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NarrativeWorldCreator.Models;
using NarrativeWorldCreator.Models.TestMetrics;
using Semantics.Components;
using Semantics.Entities;
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

        public static Model DefaultModel;

        public static Texture2D BoxSelectTexture;
        public static Texture2D RegionCreationTexture;

        
        // Automation settings
        public static Dictionary<string, bool> AutomationDictionary = new Dictionary<string, bool> {
            { "throneroom1", true },
            { "bedroomprincess1", false },
            { "guestroom1", true },
            { "diningroom1", true },
            { "kitchen1", true },
            { "prison1", false },
            { "corridor1", true },
        };

        // UI Settings
        internal static int NumberOfChoices = 4;
        public static int NumberOfTries = 100;

        // GPGPU settings
        // Number of results generated (threadblocks started):
        public static int gridxDim = 4;
        public const int gridyDim = 0;
        // Number of threads used for each threadblock, should be a multiple of 32 (probably 256)
        public const int blockxDim = 1;
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
        public static float WeightOffLimits = 1.0f;
        public static float WeightSurfaceArea = 1.0f;
        // Configurable centroid and focal points in scene
        public static double centroidX = 0.0;
        public static double centroidY = 0.0;
        public static double focalX = 5.0;
        public static double focalY = 5.0;
        public static double focalRot = 0.0;

        // Test measurements

        // User interface usage 
        public static int TotalNumberOfAddActions = 0;
        public static int TotalNumberOfTotalChangeActions = 0;
        public static int TotalNumberOfManualChangeActions = 0;
        public static int TotalNumberOfAutomatedChangeActions = 0;
        public static int TotalNumberOfRemoveActions = 0;

        public static DateTime Start;
        public static long TotalTimeSpent;

        // Time spent on actions
        public static DateTime StartOfAction;

        // Time spent per Location
        public static DateTime StartOfLocation;
        public static Dictionary<string, long> TimeSpentTotalPerLocation = new Dictionary<string, long>() {
            { "throneroom1", 0},
            { "bedroomprincess1", 0},
            { "guestroom1", 0},
            { "diningroom1", 0},
            { "kitchen1", 0},
            { "prison1", 0},
            { "corridor1", 0}
        };

        // Time spent on actions per location
        public static Dictionary<string, TimeSpentOnActions> TimeSpentOnActionsPerLocation = new Dictionary<string, TimeSpentOnActions>() {
            { "throneroom1", new TimeSpentOnActions()},
            { "bedroomprincess1", new TimeSpentOnActions()},
            { "guestroom1", new TimeSpentOnActions()},
            { "diningroom1", new TimeSpentOnActions()},
            { "kitchen1", new TimeSpentOnActions()},
            { "prison1", new TimeSpentOnActions()},
            { "corridor1", new TimeSpentOnActions()}
        };

        // General semantic usage
        public static Dictionary<TangibleObject, int> TimesATangibleObjectIsUsed = new Dictionary<TangibleObject, int>();
        public static Dictionary<Relationship, int> TimesARelationshipIsUsed = new Dictionary<Relationship, int>();

        // Location specific semantic usage
        public static Dictionary<Tuple<string, TangibleObject>, int> UsageOfTangibleObjectsPerLocation = new Dictionary<Tuple<string, TangibleObject>, int>();
        public static Dictionary<Tuple<string, Relationship>, int> UsageOfRelationshipsPerLocation = new Dictionary<Tuple<string, Relationship>, int>();
    }
}
