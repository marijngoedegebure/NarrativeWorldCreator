using NarrativeWorldCreator.Models.NarrativeRegionFill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.GraphicScenes
{
    public class MainRegionScene : RegionScene
    {
        protected override void drawEntikaInstances()
        {
            foreach (EntikaInstance instance in _currentRegionPage.SelectedTimePoint.Configuration.GetEntikaInstancesWithoutFloor())
            {
                drawEntikaInstance2(instance);
            }
        }
    }
}
