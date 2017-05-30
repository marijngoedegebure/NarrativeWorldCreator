//using Alea;
//using Alea.CSharp;
//using Alea.cuRAND;
//using Alea.Parallel;
//using Microsoft.Xna.Framework;
//using NarrativeWorldCreator.Models.NarrativeRegionFill;
//using NarrativeWorldCreator.Models.NarrativeTime;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NarrativeWorldCreator.Solvers
//{
//    public static class PlacementSolverGPU
//    {
//        public const int IterationsAmount = 1;
//        public const double DegreeOfAttraction = 2.0;

//        // Focal point weight
//        public const float WeightFocalDefault = -2.0f;
//        public const float WeightPairWiseDefault = -2.0f;

//        [GpuParam]
//        public const RngType RngType = Alea.cuRAND.RngType.PSEUDO_XORWOW;

//        public struct TargetRangeStruct
//        {
//            public double TargetRangeStart;
//            public double TargetRangeEnd;
//        }


//        public struct PositionAndRotation
//        {
//            public double x;
//            public double y;
//            public double z;

//            public double rotX;
//            public double rotY;
//            public double rotZ;
//        }

//        /// <summary>
//        /// Configuration structure for the surface currently being optimized
//        /// </summary>
//        public struct Surface
//        {
//            public int nObjs;

//            // Weights
//            public float WeightFocal;
//            public float WeightPairWise;
//        }

//        public struct Config
//        {
//            public double[] x;
//            public double[] y;
//            public double[] z;

//            public double[] rotX;
//            public double[] rotY;
//            public double[] rotZ;
//        }

//        public struct RelationshipStruct
//        {
//            public TargetRangeStruct TargetRange;
//            public PositionAndRotation Source;
//            public PositionAndRotation Target;
//            public double DegreesOfAtrraction;
//        }

//        /// <summary>
//        /// Function that will call the kernel, in charge of putting data inside appropriate containers and structures
//        /// </summary>
//        /// <param name="ntp"></param>
//        /// <returns></returns>
//        [GpuManaged]
//        public static PositionAndRotation[,] Optimization(NarrativeTimePoint ntp)
//        {
//            // Initialize GPU
//            var gpu = Gpu.Default;
//            var lp = new LaunchParam(1, 1);

//            var config = new Config
//            {
//                x = new double[ntp.Configuration.InstancedObjects.Count],
//                y = new double[ntp.Configuration.InstancedObjects.Count],
//                z = new double[ntp.Configuration.InstancedObjects.Count],
//                rotX = new double[ntp.Configuration.InstancedObjects.Count],
//                rotY = new double[ntp.Configuration.InstancedObjects.Count],
//                rotZ = new double[ntp.Configuration.InstancedObjects.Count]
//            };

//            // Convert all objects to a Config
//            for (int i = 0; i < ntp.Configuration.InstancedObjects.Count; i++)
//            {
//                config.x[i] = ntp.Configuration.InstancedObjects[i].Position.X;
//                config.y[i] = ntp.Configuration.InstancedObjects[i].Position.Y;
//                config.z[i] = ntp.Configuration.InstancedObjects[i].Position.Z;
//                config.rotX[i] = ntp.Configuration.InstancedObjects[i].Rotation.X;
//                config.rotY[i] = ntp.Configuration.InstancedObjects[i].Rotation.Y;
//                config.rotZ[i] = ntp.Configuration.InstancedObjects[i].Rotation.Z;
//            }

//            var relationships = ntp.GetValuedRelationships();
//            var relationshipStructs = new RelationshipStruct[relationships.Count];
//            // Convert al relationships to relationship structure
//            for (int j = 0; j < relationships.Count; j++)
//            {
//                relationshipStructs[j] = new RelationshipStruct
//                {
//                    TargetRange = new TargetRangeStruct
//                    {
//                        TargetRangeStart = relationships[j].TargetRangeStart.GetValueOrDefault(),
//                        TargetRangeEnd = relationships[j].TargetRangeEnd.GetValueOrDefault()
//                    },
//                    Source = new PositionAndRotation
//                    {
//                        x = relationships[j].Source.Position.X,
//                        y = relationships[j].Source.Position.Y,
//                        z = relationships[j].Source.Position.Z,
//                        rotX = relationships[j].Source.Rotation.X,
//                        rotY = relationships[j].Source.Rotation.Y,
//                        rotZ = relationships[j].Source.Rotation.Z
//                    },
//                    Target = new PositionAndRotation
//                    {
//                        x = relationships[j].Target.Position.X,
//                        y = relationships[j].Target.Position.Y,
//                        z = relationships[j].Target.Position.Z,
//                        rotX = relationships[j].Target.Rotation.X,
//                        rotY = relationships[j].Target.Rotation.Y,
//                        rotZ = relationships[j].Target.Rotation.Z
//                    }
//                };
//            }

//            // Create Surface structure with all essential information
//            var surface = new Surface
//            {
//                nObjs = ntp.Configuration.InstancedObjects.Count,
//                WeightPairWise = WeightPairWiseDefault
//            };

//            // Initialize return array
//            var result = new PositionAndRotation[16, ntp.Configuration.InstancedObjects.Count];

//            using (var rng = Generator.CreateGpu(gpu, RngType))
//            {
//                gpu.Launch(OptimizationKernel, lp, result, surface, config, relationshipStructs, RngType);
//            }

//            return result;
//        }

//        private static void OptimizationKernel(PositionAndRotation[,] result, Surface surface, Config config, RelationshipStruct[] relationships, RngType rngType)
//        {
//            //deviceptr<uint> randomResult = new deviceptr<uint>();
//            //Alea.cuRAND.Generator.Get().Generate()
//            Config cfgCurrent = config;
//            double costCur = Costs(surface, cfgCurrent, relationships);
//            Console.WriteLine("Current costs: {0}", costCur);

//            Config cfgBest = cfgCurrent;

//            double costBest = costCur;
//            Console.WriteLine("Best costs: {0}", costBest);

//            for (int i = 0; i < IterationsAmount; i++)
//            {
//                Config cfgStar = Propose(surface, cfgCurrent);
//                double costStar = Costs(surface, cfgStar, relationships);

//                if (costStar < costBest)
//                {
//                    cfgBest = cfgStar;
//                    costBest = costStar;
//                }

//                if (Accept(costStar, costCur))
//                {
//                    cfgCurrent = cfgStar;
//                    costCur = costStar;
//                }
//            }

//            for (int i = 0; i < cfgBest.x.Length; i++)
//            {
//                result[blockIdx.x, i] = new PositionAndRotation
//                {
//                    x = cfgBest.x[i],
//                    y = cfgBest.y[i],
//                    z = cfgBest.z[i]
//                };
//            }
//        }

//        private static Config Propose(Surface surface, Config cfgCurrent)
//        {
//            Config cfgStar = cfgCurrent;

//            return cfgStar;
//        }

//        private static double Costs(Surface surface, Config config, RelationshipStruct[] relationships)
//        {
//            double cost = 0;
//            cost += surface.WeightPairWise * PairWiseCosts(surface, config, relationships);
//            return cost;
//        }

//        private static double PairWiseCosts(Surface surface, Config config, RelationshipStruct[] relationships)
//        {
//            double result = 0;
//            for (var i = 0; i < relationships.Length; i++)
//            {
//                var dX = relationships[i].Source.x - relationships[i].Target.x;
//                var dY = relationships[i].Source.y - relationships[i].Target.y;
//                var distance = Math.Sqrt(dX * dX + dY * dY);
//                if (distance < relationships[i].TargetRange.TargetRangeStart)
//                {
//                    Console.WriteLine("Distance is smaller");
//                    var fraction = distance / relationships[i].TargetRange.TargetRangeStart;
//                    result += fraction * fraction;
//                }
//                else if (distance > relationships[i].TargetRange.TargetRangeEnd)
//                {
//                    Console.WriteLine("Distance is bigger");
//                    var fraction = relationships[i].TargetRange.TargetRangeEnd / distance;
//                    // currently hardcoded on fraction^2:
//                    result += fraction * fraction;
//                }
//                else
//                {
//                    Console.WriteLine("Distance is inside target range, max score");
//                    result += 1;
//                }
//            }
//            Console.WriteLine("Result: {0}", result);
//            return result;
//        }

//        private static bool Accept(double costStar, double costCur)
//        {
//            return true;
//        }
//    }
//}