﻿using Common;
using Common.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using NarrativeWorldCreator.GraphicScenes.Primitives;
using NarrativeWorldCreator.Models;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Models.NarrativeTime;
using NarrativeWorldCreator.Solvers;
using NarrativeWorldCreator.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NarrativeWorldCreator.GraphicScenes
{
    public class GenerationSceneTopRight : GenerationScene
    {
        protected override void drawEntikaInstances()
        {
            if (_currentRegionPage.TopRightSelectedGPUConfigurationResult == -1)
            {
                foreach (EntikaInstance instance in _currentRegionPage.Configuration.GetEntikaInstancesWithoutFloor())
                {
                    drawEntikaInstance2(instance);
                }
            }
            else
            {
                // Draw each entika instance in GeneratedConfigurations
                foreach (var gpuResultInstance in _currentRegionPage.GeneratedConfigurations[_currentRegionPage.TopRightSelectedGPUConfigurationResult].Instances)
                {
                    drawGPUResultInstance(gpuResultInstance);
                }
            }
        }
    }
}