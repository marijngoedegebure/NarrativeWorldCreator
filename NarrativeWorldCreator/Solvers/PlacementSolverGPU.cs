using Alea;
using Alea.CSharp;
using Microsoft.Xna.Framework;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Solvers
{
    public static class PlacementSolverGPU
    {

        public const double DegreeOfAttraction = 2.0;

        public struct TargetRangeStruct
        {
            public double TargetRangeStart;
            public double TargetRangeEnd;
        }

        public struct Vector3Struct
        {
            public double x;
            public double y;
            public double z;
        }

        [GpuManaged]
        public static double[] CalculatePairwiseEnergies(List<RelationshipInstance> instances)
        {
            var gpu = Gpu.Default;
            var lp = new LaunchParam(16, 256);
            var arg1 = new TargetRangeStruct[instances.Count];
            var arg2 = DegreeOfAttraction;
            // Source position
            var arg3 = new Vector3Struct[instances.Count];
            // Target position
            var arg4 = new Vector3Struct[instances.Count];
            for (int i = 0; i < instances.Count; i++)
            {
                arg1[i] = new TargetRangeStruct
                {
                    TargetRangeStart = instances[i].TargetRangeStart.GetValueOrDefault(),
                    TargetRangeEnd = instances[i].TargetRangeEnd.GetValueOrDefault()
                };
                arg3[i] = new Vector3Struct
                {
                    x = instances[i].Source.Position.X,
                    y = instances[i].Source.Position.Y,
                    z = instances[i].Source.Position.Z
                };
                arg4[i] = new Vector3Struct
                {
                    x = instances[i].Target.Position.X,
                    y = instances[i].Target.Position.Y,
                    z = instances[i].Target.Position.Z

                };
            }
            var result = new double[instances.Count];

            gpu.Launch(PairwiseEnergyKernel, lp, result, arg1, arg2, arg3, arg4);
            return result;
        }

        private static void PairwiseEnergyKernel(double[] result, TargetRangeStruct[] arg1, double arg2, Vector3Struct[] sourcePositions, Vector3Struct[] targetPositions)
        {
            var start = blockIdx.x * blockDim.x + threadIdx.x;
            var stride = gridDim.x * blockDim.x;
            for (var i = start; i < result.Length; i += stride)
            {
                var dX = sourcePositions[i].x - targetPositions[i].x;
                var dY = sourcePositions[i].y - targetPositions[i].y;
                var dZ = sourcePositions[i].z - targetPositions[i].z;
                var distance = Math.Sqrt(dX * dX + dY * dY + dZ * dZ);
                if (distance < arg1[i].TargetRangeStart)
                {
                    var fraction = distance / arg1[i].TargetRangeStart;
                    result[i] = fraction * fraction;
                }
                else if (distance > arg1[i].TargetRangeEnd)
                {
                    var fraction = arg1[i].TargetRangeEnd / distance;
                    // currently hardcoded on fraction^2:
                    result[i] = fraction * fraction;
                }
                else
                {
                    result[i] = 1;
                }
            }

        }
    }
}