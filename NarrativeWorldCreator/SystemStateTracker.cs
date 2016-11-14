using Narratives;
using NarrativeWorlds;
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

        public static NarrativeWorld NarrativeWorld = new NarrativeWorld();
        public static string LoadedFileName;
        public static string EntikaPath;
    }
}
