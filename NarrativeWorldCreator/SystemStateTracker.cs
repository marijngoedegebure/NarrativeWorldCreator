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
    }
}
