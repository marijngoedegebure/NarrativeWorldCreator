using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NarrativeWorldCreator.Solvers;

namespace NarrativeWorldCreator.Models.NarrativeRegionFill
{
    public class GPUInstanceResult
    {
        public EntikaInstance entikaInstance;

        public GPUInstanceResult(EntikaInstance entikaInstance, CudaGPUWrapper.Point ms)
        {
            this.entikaInstance = entikaInstance;
            this.Position = new Vector3(ms.x, ms.y, ms.z);
            this.Rotation = new Vector3(ms.rotX, ms.rotY, ms.rotZ);
        }

        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
    }
}
