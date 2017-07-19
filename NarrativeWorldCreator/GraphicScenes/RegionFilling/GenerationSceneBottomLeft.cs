using NarrativeWorldCreator.Models.NarrativeRegionFill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.GraphicScenes
{
    public class GenerationSceneBottomLeft : GenerationScene
    {
        protected override void drawEntikaInstances()
        {
            if (_currentRegionPage.BottomLeftSelectedGPUConfigurationResult == -1)
            {
                foreach (EntikaInstance instance in _currentRegionPage.Configuration.GetEntikaInstancesWithoutFloor())
                {
                    drawEntikaInstance2(instance);
                }
            }
            else
            {
                // Draw each entika instance in GeneratedConfigurations
                foreach (var gpuResultInstance in _currentRegionPage.GeneratedConfigurations[_currentRegionPage.BottomLeftSelectedGPUConfigurationResult].Instances)
                {
                    drawGPUResultInstance(gpuResultInstance);
                }
            }
        }
    }
}
