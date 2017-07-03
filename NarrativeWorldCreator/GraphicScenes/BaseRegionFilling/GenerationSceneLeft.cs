﻿using NarrativeWorldCreator.Models.NarrativeRegionFill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.GraphicScenes
{
    public class GenerationSceneLeft : GenerationScene
    {
        protected override void drawEntikaInstances()
        {
            if (_currentRegionPage.LeftSelectedGPUConfigurationResult == -1)
            {
                foreach (EntikaInstance instance in _currentRegionPage.SelectedTimePoint.Configuration.GetEntikaInstancesWithoutFloor())
                {
                    drawEntikaInstance2(instance);
                }
            }
            else
            {
                // Draw each entika instance in GeneratedConfigurations
                foreach (var gpuResultInstance in _currentRegionPage.GeneratedConfigurations[_currentRegionPage.LeftSelectedGPUConfigurationResult].Instances)
                {
                    drawGPUResultInstance(gpuResultInstance);
                }
            }
        }
    }
}
