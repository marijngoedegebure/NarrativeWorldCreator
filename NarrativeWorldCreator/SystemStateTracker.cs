using Narratives;
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
        public static NarrativeWorld NarrativeWorld = new NarrativeWorld();
        public static string LoadedFileName;
        public static string EntikaPath;
    }
}
