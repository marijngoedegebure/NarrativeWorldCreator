﻿using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Models.NarrativeTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Solvers
{
    public class CudaGPUWrapper
    {
        [DllImport("Kernel.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr KernelWrapper(RelationshipStruct[] rss, PositionAndRotation[] currentConfig, [MarshalAs(UnmanagedType.Struct)] ref Surface srf, [MarshalAs(UnmanagedType.Struct)] ref gpuConfig gpuCfg);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct TargetRangeStruct
        {
            public double TargetRangeStart;
            public double TargetRangeEnd;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct RelationshipStruct
        {
            public TargetRangeStruct TargetRange;
            public int SourceIndex;
            public int TargetIndex;
            public double DegreesOfAtrraction;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct PositionAndRotation
        {
            public double x;
            public double y;
            public double z;

            public double rotX;
            public double rotY;
            public double rotZ;
            public bool frozen;

            public double length;
            public double width;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct Surface
        {
            public int nObjs;
            public int nRelationships;

            // Weights
            public float WeightFocalPoint;
            public float WeightPairWise;
            public float WeightVisualBalance;
            public float WeightSymmetry;

            //Centroid
            public double centroidX;
            public double centroidY;

            // Focal point
            public double focalX;
            public double focalY;
            public double focalRot;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct gpuConfig
        {
            public int gridxDim;
            public int gridyDim;
            public int blockxDim;
            public int blockyDim;
            public int blockzDim;
            public int iterations;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct Point
        {
            public float x;
            public float y;
            public float z;

            public float rotX;
            public float rotY;
            public float rotZ;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct ResultCosts
        {
            public float totalCosts;
            public float PairWiseCosts;
            public float VisualBalanceCosts;
            public float FocalPointCosts;
            public float SymmetryCosts;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct Result
        {
            public IntPtr points;
            public ResultCosts costs;
        };

        public static List<GPUConfigurationResult> CudaGPUWrapperCall(Configuration configuration)
        {
            var valuedRelationships = configuration.GetValuedRelationships();
            var instances = configuration.InstancedObjects;

            int N = instances.Count;
            int NRel = valuedRelationships.Count;

            var surface = new Surface
            {
                nObjs = N,
                nRelationships = NRel,
                WeightFocalPoint = SystemStateTracker.WeightFocalPoint,
                WeightPairWise = SystemStateTracker.WeightPairWise,
                WeightVisualBalance = SystemStateTracker.WeightVisualBalance,
                WeightSymmetry = SystemStateTracker.WeightSymmetry,
                // TODO: allow input for centroid and focal
                centroidX = SystemStateTracker.centroidX,
                centroidY = SystemStateTracker.centroidY,
                focalX = SystemStateTracker.focalX,
                focalY = SystemStateTracker.focalY,
                focalRot = SystemStateTracker.focalRot
            };

            var currentConfig = new PositionAndRotation[N];
            for (int i = 0; i < currentConfig.Length; i++)
            {
                currentConfig[i] = new PositionAndRotation
                {
                    x = instances[i].Position.X,
                    y = instances[i].Position.Y,
                    z = instances[i].Position.Z,
                    rotX = instances[i].Rotation.X,
                    rotY = instances[i].Rotation.Y,
                    rotZ = instances[i].Rotation.Z,
                    frozen = instances[i].Frozen,
                    length = instances[i].BoundingBox.Max.X - instances[i].BoundingBox.Min.X,
                    width = instances[i].BoundingBox.Max.Y - instances[i].BoundingBox.Min.Y
                };
            }

            var rss = new RelationshipStruct[NRel];
            for (int i = 0; i < rss.Length; i++)
            {
                rss[i] = new RelationshipStruct
                {
                    TargetRange = new TargetRangeStruct
                    {
                        TargetRangeStart = valuedRelationships[i].TargetRangeStart.GetValueOrDefault(),
                        TargetRangeEnd = valuedRelationships[i].TargetRangeEnd.GetValueOrDefault()
                    },
                    SourceIndex = instances.FindIndex(inst => inst.Equals(valuedRelationships[i].Source)),
                    TargetIndex = instances.FindIndex(inst => inst.Equals(valuedRelationships[i].Target)),
                    // Allow degrees of attraction to later be configurable
                    DegreesOfAtrraction = 2.0
                };
            }

            gpuConfig gpuCfg = new gpuConfig
            {
                gridxDim = SystemStateTracker.gridxDim,
                gridyDim = SystemStateTracker.gridyDim,
                blockxDim = SystemStateTracker.blockxDim,
                blockyDim = SystemStateTracker.blockyDim,
                blockzDim = SystemStateTracker.blockzDim,
                iterations = SystemStateTracker.iterations
            };

            var pointer = KernelWrapper(rss, currentConfig, ref surface, ref gpuCfg);

            List <GPUConfigurationResult> configs = new List<GPUConfigurationResult>();
            GPUConfigurationResult temp = new GPUConfigurationResult();
            // Do the initial one
            for (int j = 0; j < gpuCfg.gridxDim; j++)
            {
                IntPtr resultPointer = new IntPtr(pointer.ToInt64() + Marshal.SizeOf(typeof(Result)) * j);
                Result r = (Result)Marshal.PtrToStructure(resultPointer, typeof(Result));
                temp.TotalCosts = r.costs.totalCosts;
                temp.FocalPointCosts = r.costs.FocalPointCosts;
                temp.PairWiseCosts = r.costs.PairWiseCosts;
                temp.SymmetryCosts = r.costs.SymmetryCosts;
                temp.VisualBalanceCosts = r.costs.VisualBalanceCosts;

                // Retrieve all points of result:
                for (int k = 0; k < N; k++)
                {
                    IntPtr pointsPointer = new IntPtr(r.points.ToInt64() + Marshal.SizeOf(typeof(Point)) * k);
                    Point point = (Point)Marshal.PtrToStructure(pointsPointer, typeof(Point));
                    temp.Instances.Add(new GPUInstanceResult(configuration.InstancedObjects[k], point));
                }
                configs.Add(temp);
                temp = new GPUConfigurationResult();
            }

            //Point ms = (Point)Marshal.PtrToStructure(data, typeof(Point));
            //temp.Instances.Add(new GPUInstanceResult(configuration.InstancedObjects[0], ms));
            //for (int j = 1; j < (gpuCfg.gridxDim) * N; j++)
            //{
            //    data = new IntPtr(pointer.ToInt64() + Marshal.SizeOf(typeof(Point)) * j);
            //    ms = (Point)Marshal.PtrToStructure(data, typeof(Point));


            //    var objectIndex = j % N;
            //    // If 0 is reached again, save configuration and renew configuration
            //    if (objectIndex == 0)
            //    {
            //        configs.Add(temp);
            //        temp = new GPUConfigurationResult();
            //    }
            //    temp.Instances.Add(new GPUInstanceResult(configuration.InstancedObjects[objectIndex], ms));
            //    Console.WriteLine();
            //}
            return configs;
        }

        public static void CudaGPUWrapperCallTestFunction()
        {
            const int N = 2;
            const int NRel = 1;

            var surface = new Surface
            {
                nObjs = N,
                nRelationships = NRel,
                WeightFocalPoint = -2.0f,
                WeightPairWise = -2.0f,
                WeightVisualBalance = 1.5f,
                WeightSymmetry = -2.0f,
                centroidX = 0.0,
                centroidY = 0.0,
                focalX = 5.0,
                focalY = 5.0,
                focalRot = 0.0
            };

            var currentConfig = new PositionAndRotation[N];
            for (int i = 0; i < currentConfig.Length; i++)
            {
                currentConfig[i] = new PositionAndRotation
                {
                    x = 2.0,
                    y = 2.0,
                    z = 0.0,
                    rotX = 0.0,
                    rotY = 0.0,
                    rotZ = 0.0,
                    frozen = false,
                    length = 1.0,
                    width = 1.0
                };
            }


            var targetRange = new TargetRangeStruct
            {
                TargetRangeStart = 2.0,
                TargetRangeEnd = 4.0
            };

            var rss = new RelationshipStruct[NRel];
            for (int i = 0; i < rss.Length; i++)
            {
                rss[i] = new RelationshipStruct
                {
                    TargetRange = targetRange,
                    SourceIndex = 0,
                    TargetIndex = 1,
                    DegreesOfAtrraction = 2.0
                };
            }

            gpuConfig gpuCfg = new gpuConfig
            {
                gridxDim = 1,
                gridyDim = 0,
                blockxDim = 1,
                blockyDim = 0,
                blockzDim = 0,
                iterations = 10
            };

            Point[] result = new Point[(gpuCfg.gridxDim) * N];
            var pointer = KernelWrapper(rss, currentConfig, ref surface, ref gpuCfg);
            for (int j = 0; j < result.Length; j++)
            {
                IntPtr data = new IntPtr(pointer.ToInt64() + Marshal.SizeOf(typeof(Point)) * j);
                Point ms = (Point)Marshal.PtrToStructure(data, typeof(Point));
                result[j] = ms;
                Console.WriteLine();
            }
            return;
        }
    }
}
