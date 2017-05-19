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
            public PositionAndRotation Source;
            public PositionAndRotation Target;
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
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct Surface
        {
            public int nObjs;

            // Weights
            public float WeightFocal;
            public float WeightPairWise;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct gpuConfig
        {
            public int gridxDim;
            public int gridyDim;
            public int blockxDim;
            public int blockyDim;
            public int blockzDim;
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

        public static void CudaGPUWrapperCall()
        {
            const int N = 10;
            var targetRange = new TargetRangeStruct
            {
                TargetRangeStart = 0.0,
                TargetRangeEnd = 2.0
            };

            var currentConfig = new PositionAndRotation[N];
            for (int i = 0; i < currentConfig.Length; i++)
            {
                currentConfig[i] = new PositionAndRotation
                {
                    x = 12.0,
                    y = 13.0,
                    z = 14.0,
                    rotX = 15.0,
                    rotY = 16.0,
                    rotZ = 17.0
                };
            }

            var surface = new Surface
            {
                nObjs = N,
                WeightPairWise = 2.0f
            };

            var rss = new RelationshipStruct[N];
            for (int i = 0; i < rss.Length; i++)
            {
                rss[i] = new RelationshipStruct
                {
                    TargetRange = targetRange,
                    Source = new PositionAndRotation
                    {
                        x = 2.0,
                        y = 3.0,
                        z = 4.0,
                        rotX = 5.0,
                        rotY = 6.0,
                        rotZ = 7.0
                    },
                    Target = new PositionAndRotation
                    {
                        x = 12.0,
                        y = 13.0,
                        z = 14.0,
                        rotX = 15.0,
                        rotY = 16.0,
                        rotZ = 17.0
                    },
                    DegreesOfAtrraction = 25.0
                };
            }

            gpuConfig gpuCfg = new gpuConfig
            {
                gridxDim = 4,
                gridyDim = 0,
                blockxDim = 4,
                blockyDim = 0,
                blockzDim = 0
            };

            Point[] result = new Point[(gpuCfg.gridxDim) * N];
            var pointer = KernelWrapper(rss, currentConfig, ref surface, ref gpuCfg);
            for (int j = 0; j < result.Length; j++)
            {
                IntPtr data = new IntPtr(pointer.ToInt64() + Marshal.SizeOf(typeof(Point)) * j);
                Point ms = (Point)Marshal.PtrToStructure(data, typeof(Point));

                Console.WriteLine();
            }
            return;
        }
    }
}
