using Microsoft.Xna.Framework;
using NarrativeWorldCreator.Models;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
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
        internal static extern IntPtr KernelWrapper(RelationshipStruct[] rss, PositionAndRotation[] currentConfig, Rectangle[] clearances, Rectangle[] offlimits, Vertex[] vertices, Vertex[] surfaceRectangle, [MarshalAs(UnmanagedType.Struct)] ref Surface srf, [MarshalAs(UnmanagedType.Struct)] ref gpuConfig gpuCfg);

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
        public struct Rectangle
        {
            public int point1Index;
            public int point2Index;
            public int point3Index;
            public int point4Index;
            public int SourceIndex;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct Vertex
        {
            public double x;
            public double y;
            public double z;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct Surface
        {
            public int nObjs;
            public int nRelationships;
            public int nClearances;

            // Weights
            public float WeightFocalPoint;
            public float WeightPairWise;
            public float WeightVisualBalance;
            public float WeightSymmetry;
            public float WeightClearance;
            public float WeightSurfaceArea;
            public float WeightOffLimits;

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
            public float ClearanceCosts;
            public float OffLimitsCosts;
            public float SurfaceAreaCosts;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct Result
        {
            public IntPtr points;
            public ResultCosts costs;
        };

        public static List<GPUConfigurationResult> CudaGPUWrapperCall(NarrativeTimePoint ntp, Configuration WIPconfiguration)
        {
            // Copy configuration so that it does not adjust work in progresse configuration
            var configuration = WIPconfiguration.Copy();

            // Divide configuration in sets of objects combined with each on relationship
            // SurfaceConfigurations holds configuration for individual surfaces
            Dictionary<EntikaInstance, Configuration> surfaceConfigurations = new Dictionary<EntikaInstance, Configuration>();

            // All on relationships
            var onRelationships = configuration.InstancedRelations.Where(rel => rel.BaseRelationship.RelationshipType.DefaultName.Equals(Constants.On)).ToList();

            // All distinct sources
            var distinctSources = new List<EntikaInstance>();
            foreach (var onRel in onRelationships)
            {
                if (!distinctSources.Contains(onRel.Source))
                    distinctSources.Add(onRel.Source);
            }

            // All surface instances for each surface source
            foreach (var source in distinctSources)
            {
                var surfaceRelationships = configuration.InstancedRelations.Where(ir => ir.Source.Equals(source)).ToList();
                var tempConfig = new Configuration();
                foreach (var rel in surfaceRelationships)
                {
                    // Take all targets of this surface and collect and store their relationships
                    foreach (var targetRel in rel.Target.RelationshipsAsTarget)
                    {
                        if (!tempConfig.InstancedObjects.Contains(targetRel.Target))
                            tempConfig.InstancedObjects.Add(targetRel.Target);
                        // Do not add on relationships, no use
                        if (!targetRel.BaseRelationship.RelationshipType.DefaultName.Equals(Constants.On))
                        {
                            tempConfig.InstancedRelations.Add(targetRel);
                        }
                    }
                }
                surfaceConfigurations.Add(source, tempConfig);
            }
            foreach (var surfaceInstance in surfaceConfigurations.Keys)
            {
                // Translate each instance of a configuration according to its surface
                foreach (var instance in surfaceConfigurations[surfaceInstance].InstancedObjects)
                {
                    // Go from absolute to relative position
                    instance.Position = new Vector3(instance.Position.X - surfaceInstance.Position.X, instance.Position.Y - surfaceInstance.Position.Y, instance.Position.Z);
                    instance.Rotation = instance.Rotation - surfaceInstance.Rotation;
                }
            }

            Dictionary<EntikaInstance, List<GPUConfigurationResult>> resultPerSurface = new Dictionary<EntikaInstance, List<GPUConfigurationResult>>();
            // Create strategy to resolve surface so that they do not influence each other
            foreach (var key in surfaceConfigurations.Keys)
            {
                resultPerSurface.Add(key, GPUCallSurface(key, surfaceConfigurations[key]));
            }

            var trueGPUResults = new List<GPUConfigurationResult>();
            for (int j = 0; j < SystemStateTracker.gridxDim; j++)
            {
                var gpuResult = new GPUConfigurationResult();
                foreach (var surfaceInstance in resultPerSurface.Keys)
                {
                    // Find new position of each key
                    foreach (var surfaceInstance2 in resultPerSurface.Keys)
                    {
                        foreach (var instance in resultPerSurface[surfaceInstance2][j].Instances)
                        {
                            if (instance.entikaInstance.Equals(surfaceInstance))
                            {
                                surfaceInstance.Position = instance.Position;
                                surfaceInstance.Rotation = instance.Rotation;
                            }
                        }
                    }
                    // Convert relative positions to absolute positions
                    foreach (var instance in resultPerSurface[surfaceInstance][j].Instances)
                    {
                        instance.Rotation = instance.Rotation + surfaceInstance.Rotation;
                        instance.Position = new Vector3(instance.Position.X + surfaceInstance.Position.X, instance.Position.Y + surfaceInstance.Position.Y, instance.Position.Z);
                    }
                    // Recombine results, so that surface source translation is applied to all surface objects.
                    gpuResult.Instances.AddRange(resultPerSurface[surfaceInstance][j].Instances);
                    gpuResult.TotalCosts = gpuResult.TotalCosts + resultPerSurface[surfaceInstance][j].TotalCosts;
                    gpuResult.FocalPointCosts = gpuResult.FocalPointCosts + resultPerSurface[surfaceInstance][j].FocalPointCosts;
                    gpuResult.PairWiseCosts = gpuResult.PairWiseCosts + resultPerSurface[surfaceInstance][j].PairWiseCosts;
                    gpuResult.SymmetryCosts = gpuResult.SymmetryCosts + resultPerSurface[surfaceInstance][j].SymmetryCosts;
                    gpuResult.VisualBalanceCosts = gpuResult.VisualBalanceCosts + resultPerSurface[surfaceInstance][j].VisualBalanceCosts;
                    gpuResult.ClearanceCosts = gpuResult.ClearanceCosts + resultPerSurface[surfaceInstance][j].ClearanceCosts;
                    gpuResult.OffLimitsCosts = gpuResult.OffLimitsCosts + resultPerSurface[surfaceInstance][j].OffLimitsCosts;
                    gpuResult.SurfaceAreaCosts = gpuResult.SurfaceAreaCosts + resultPerSurface[surfaceInstance][j].SurfaceAreaCosts;
                }
                trueGPUResults.Add(gpuResult);
            }
            return trueGPUResults;
        }

        public static List<GPUConfigurationResult> GPUCallSurface(EntikaInstance surfaceInstance, Configuration configuration)
        {
            // Call Gpu method for each set of objects
            var valuedRelationships = configuration.GetValuedRelationships();
            var instances = configuration.InstancedObjects;

            int N = instances.Count;
            int NRel = valuedRelationships.Count;

            var currentConfig = new PositionAndRotation[N];
            int numberOfClearances = 0;
            for (int i = 0; i < currentConfig.Length; i++)
            {
                numberOfClearances += instances[i].Clearances.Count();

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
                    width = instances[i].BoundingBox.Max.Y - instances[i].BoundingBox.Min.Y,
                };
            }


            var clearances = new Rectangle[numberOfClearances];
            var offlimits = new Rectangle[N];
            var vertices = new Vertex[numberOfClearances * 4 + N * 4];
            var clearanceIndex = 0;
            var vertexIndex = 0;
            for (int i = 0; i < N; i++)
            {
                if (!instances[i].Name.Equals(Constants.Floor))
                {
                    foreach (var clearance in instances[i].Clearances)
                    {
                        foreach (var point in clearance.GetAllVertices())
                        {
                            vertices[vertexIndex] = new Vertex
                            {
                                x = point.X,
                                y = point.Y,
                                z = 0
                            };
                            vertexIndex++;
                        }
                        int startIndexVertex = vertexIndex - 4;
                        clearances[clearanceIndex] = new Rectangle
                        {
                            SourceIndex = i,
                            point1Index = startIndexVertex,
                            point2Index = startIndexVertex + 1,
                            point3Index = startIndexVertex + 2,
                            point4Index = startIndexVertex + 3
                        };
                        clearanceIndex++;
                    }

                    // Create offlimits shape
                    var bbCorners = instances[i].BoundingBox.GetCorners();
                    for (int j = 0; j < 4; j++)
                    {
                        vertices[vertexIndex] = new Vertex
                        {
                            x = bbCorners[j].X,
                            y = bbCorners[j].Y,
                            z = bbCorners[j].Z
                        };
                        vertexIndex++;
                    }
                    int startIndex = vertexIndex - 4;
                    offlimits[i] = new Rectangle
                    {
                        SourceIndex = i,
                        point1Index = startIndex,
                        point2Index = startIndex + 1,
                        point3Index = startIndex + 2,
                        point4Index = startIndex + 3
                    };
                }
            }



            var surfaceRectangle = new Vertex[4];
            BoundingBox bb;
            if (surfaceInstance.Name.Equals(Constants.Floor))
            {
                bb = EntikaInstance.GetBoundingBox(surfaceInstance.Polygon);
                var corners = bb.GetCorners();

                for (int i = 0; i < 4; i++)
                {
                    surfaceRectangle[i].x = corners[i].X;
                    surfaceRectangle[i].y = corners[i].Y;
                    surfaceRectangle[i].z = corners[i].Z;
                }
            }
            else
            {
                bb = surfaceInstance.BoundingBox;
                surfaceRectangle[0].x = -(bb.Max.X - bb.Min.X) / 2;
                surfaceRectangle[0].y = (bb.Max.Y - bb.Min.Y) / 2;
                surfaceRectangle[0].z = (bb.Max.Z - bb.Min.Z) / 2;

                surfaceRectangle[1].x = (bb.Max.X - bb.Min.X) / 2;
                surfaceRectangle[1].y = (bb.Max.Y - bb.Min.Y) / 2;
                surfaceRectangle[1].z = (bb.Max.Z - bb.Min.Z) / 2;

                surfaceRectangle[2].x = (bb.Max.X - bb.Min.X) / 2;
                surfaceRectangle[2].y = -(bb.Max.Y - bb.Min.Y) / 2;
                surfaceRectangle[2].z = (bb.Max.Z - bb.Min.Z) / 2;

                surfaceRectangle[3].x = -(bb.Max.X - bb.Min.X) / 2;
                surfaceRectangle[3].y = -(bb.Max.Y - bb.Min.Y) / 2;
                surfaceRectangle[3].z = (bb.Max.Z - bb.Min.Z) / 2;
            }

            var surface = new Surface
            {
                nObjs = N,
                nRelationships = NRel,
                nClearances = numberOfClearances,
                WeightFocalPoint = SystemStateTracker.WeightFocalPoint,
                WeightPairWise = SystemStateTracker.WeightPairWise,
                WeightVisualBalance = SystemStateTracker.WeightVisualBalance,
                WeightSymmetry = SystemStateTracker.WeightSymmetry,
                WeightClearance = SystemStateTracker.WeightClearance,
                WeightOffLimits = SystemStateTracker.WeightOffLimits,
                WeightSurfaceArea = SystemStateTracker.WeightSurfaceArea,
                centroidX = SystemStateTracker.centroidX,
                centroidY = SystemStateTracker.centroidY,
                focalX = SystemStateTracker.focalX,
                focalY = SystemStateTracker.focalY,
                focalRot = SystemStateTracker.focalRot
            };

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

            var pointer = KernelWrapper(rss, currentConfig, clearances, offlimits, vertices, surfaceRectangle, ref surface, ref gpuCfg);

            List<GPUConfigurationResult> configs = new List<GPUConfigurationResult>();
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
                temp.OffLimitsCosts = r.costs.OffLimitsCosts;
                temp.ClearanceCosts = r.costs.ClearanceCosts;
                temp.SurfaceAreaCosts = r.costs.SurfaceAreaCosts;

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
    }
}
