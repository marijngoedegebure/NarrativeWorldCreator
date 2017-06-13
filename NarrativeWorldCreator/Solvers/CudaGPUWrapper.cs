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
            public float SurfaceAreaCosts;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        struct Result
        {
            public IntPtr points;
            public ResultCosts costs;
        };

        public static List<GPUConfigurationResult> CudaGPUWrapperCall(NarrativeTimePoint ntp, Configuration configuration)
        {
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
            var bb = EntikaInstance.GetBoundingBox(configuration.InstancedObjects.Where(io => io.Name.Equals(Constants.Floor)).FirstOrDefault().Polygon);
            var corners = bb.GetCorners();
            for (int i = 0; i < 4; i++)
            {
                surfaceRectangle[i].x = corners[i].X;
                surfaceRectangle[i].y = corners[i].Y;
                surfaceRectangle[i].z = corners[i].Z;
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
                // TODO: allow input for centroid and focal
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
