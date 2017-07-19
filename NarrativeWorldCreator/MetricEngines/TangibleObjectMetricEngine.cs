using Microsoft.Xna.Framework;
using NarrativeWorldCreator.Models;
using NarrativeWorldCreator.Models.Metrics;
using NarrativeWorldCreator.Models.Metrics.InstanceTree;
using NarrativeWorldCreator.Models.Metrics.TOTree;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Models.NarrativeTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Semantics.Entities;

namespace NarrativeWorldCreator.MetricEngines
{
    public static class TangibleObjectMetricEngine
    {
        public static MetricType IncEdgesMT = new MetricType("incoming edges");
        public static MetricType OutEdgesMT = new MetricType("outgoing edges");
        public static MetricType IncEdgesDecorativeMT = new MetricType("incoming decorative edges");
        public static MetricType OutEdgesDecorativeMT = new MetricType("outgoing decorative edges");
        public static MetricType IncEdgesRequiredMT = new MetricType("incoming required edges");
        public static MetricType OutEdgesRequiredMT = new MetricType("outgoing required edges");
        public static MetricType IncEdgesAvailableMT = new MetricType("incoming edges available");
        public static MetricType OutEdgesAvailableMT = new MetricType("outgoing edges available");
        public static MetricType requiredMT = new MetricType("required");
        public static MetricType requiredDependencyMT = new MetricType("required dependency");
        public static MetricType DecorationMT = new MetricType("decoration weight");

        public static MetricType AreaMT = new MetricType("area");

        public static List<TOTreeTangibleObject> TTOs = new List<TOTreeTangibleObject>();
        public static List<TOTreeRelationship> TreeRelationships = new List<TOTreeRelationship>();

        public static List<InstanceTreeEntikaInstance> ITEIs = new List<InstanceTreeEntikaInstance>();

        public static List<InstanceTreeRelationship> InstanceTreeRelationships = new List<InstanceTreeRelationship>();

        public static Dictionary<MetricType, Normalization> normalizationDictionary = new Dictionary<MetricType, Normalization>();

        public static int MetricTypeCount = 0;

        public static Dictionary<string, double> Weights = new Dictionary<string, double>();

        public static void BuildUpTOTree(List<Semantics.Entities.TangibleObject> tangibleObjects)
        {
            foreach (var to in tangibleObjects)
            {
                var treeTOSource = (from tto in TTOs
                                    where tto.TangibleObject.Equals(to)
                                    select tto).FirstOrDefault();
                if (treeTOSource == null)
                {
                    treeTOSource = new TOTreeTangibleObject(to);
                    TTOs.Add(treeTOSource);
                }
                // Check all geometric relationships
                foreach (var relationship in to.RelationshipsAsSource.Where(ras => Constants.GeometricRelationshipTypes.Contains(ras.RelationshipType.DefaultName)))
                {
                    if (!tangibleObjects.Contains(relationship.Targets[0]))
                        continue;
                    var treeTOTarget = (from tto in TTOs
                                        where tto.TangibleObject.Equals((Semantics.Entities.TangibleObject)relationship.Targets[0])
                                        select tto).FirstOrDefault();

                    if (treeTOTarget  == null)
                    {
                        treeTOTarget = new TOTreeTangibleObject((Semantics.Entities.TangibleObject)relationship.Targets[0]);
                        TTOs.Add(treeTOTarget);
                    }
                    if (relationship.RelationshipType.DefaultName.Equals(Constants.On))
                        treeTOTarget.Dependent = true;

                    var Trelationship = new TOTreeRelationship(relationship);
                    Trelationship.Source = treeTOSource;
                    Trelationship.Target = treeTOTarget;
                    TreeRelationships.Add(Trelationship);
                    treeTOSource.RelationshipsAsSource.Add(Trelationship);
                    treeTOTarget.RelationshipsAsTarget.Add(Trelationship);
                }
            }
        }

        public static void BuildUpInstanceTree(List<EntikaInstance> instances)
        {
            foreach (var instance in instances)
            {
                var instanceTreeSource = (from itei in ITEIs
                                          where itei.EntikaInstance.Equals(instance)
                                          select itei).FirstOrDefault();
                if (instanceTreeSource == null)
                {
                    instanceTreeSource = new InstanceTreeEntikaInstance(instance);
                    ITEIs.Add(instanceTreeSource);
                }
                foreach (var relationship in instanceTreeSource.EntikaInstance.RelationshipsAsSource)
                {
                    var instanceTreeTarget = (from itei in ITEIs
                                              where itei.EntikaInstance.Equals(relationship.Target)
                                              select itei).FirstOrDefault();

                    if (instanceTreeTarget == null)
                    {
                        instanceTreeTarget = new InstanceTreeEntikaInstance(relationship.Target);
                        ITEIs.Add(instanceTreeTarget);
                    }
                    var instanceTreeRelationship = new InstanceTreeRelationship(relationship);
                    instanceTreeRelationship.Source = instanceTreeSource;
                    instanceTreeRelationship.Target = instanceTreeTarget;
                    InstanceTreeRelationships.Add(instanceTreeRelationship);
                    instanceTreeSource.RelationshipsAsSource.Add(instanceTreeRelationship);
                    instanceTreeTarget.RelationshipsAsTarget.Add(instanceTreeRelationship);
                }
            }
        }

        public static void ResetTree()
        {
            TTOs = new List<TOTreeTangibleObject>();
            TreeRelationships = new List<TOTreeRelationship>();

            normalizationDictionary = new Dictionary<MetricType, Normalization>();
        }

        public static void CheckForDependencies()
        {
            foreach (var tto in TTOs.Where(tto => tto.Dependent))
            {
                foreach (var itei in ITEIs)
                {
                    foreach (var relationship in tto.RelationshipsAsTarget) {
                        if (relationship.Source.TangibleObject.Equals(itei.EntikaInstance.TangibleObject))
                            tto.DependencyAvailable = true;
                    }
                }
            }
        }

        public static List<TOTreeTangibleObject> CalculateValues(NarrativeTimePoint ntp, Configuration c, List<TangibleObject> tangibleObjects, List<Predicate> predicates)
        {
            ResetTree();
            BuildUpTOTree(tangibleObjects);
            BuildUpInstanceTree(c.InstancedObjects);
            CheckForDependencies();

            MetricTypeCount = 0;
            // Incoming and outgoing geometric relationships
            normalizationDictionary[IncEdgesMT] = IncomingEdges();

            normalizationDictionary[OutEdgesMT] = OutgoingEdges();

            // Add decoration weight metric
            normalizationDictionary[DecorationMT] = DecorationWeights(ntp);

            // Incoming and outgoing decorative edges, this is defined as an edge from or to an node with a decorative score
            normalizationDictionary[IncEdgesDecorativeMT] = IncomingDecorativeEdges();

            normalizationDictionary[OutEdgesDecorativeMT] = OutgoingDecorativeEdges();

            // Go through objects and highlight objects and relationships that are required by the predicates or are dependencies of a required object
            ApplyRequiredAndDependencies(predicates);
            normalizationDictionary[requiredMT] = GetRequiredNormalization();
            normalizationDictionary[requiredDependencyMT] = GetRequiredDependencyNormalization();

            // Incoming and outgoing required edges
            normalizationDictionary[IncEdgesRequiredMT] = IncomingRequiredEdges();

            normalizationDictionary[OutEdgesRequiredMT] = OutgoingRequiredEdges();

            // Determine relationships for which instances have already been placed
            normalizationDictionary[IncEdgesAvailableMT] = InAvailableEdges();

            normalizationDictionary[OutEdgesAvailableMT] = OutAvailableEdges();

            // Calculate size metric
            normalizationDictionary[AreaMT] = SizeMetric();

            // Normalize before sorting?
            foreach (var tto in TTOs)
            {
                foreach (var metric in tto.Metrics)
                {
                    // (x - min) / (max - min)
                    if (normalizationDictionary[metric.MetricType].Max != normalizationDictionary[metric.MetricType].Min)
                        metric.Value = (metric.Value - normalizationDictionary[metric.MetricType].Min) / (normalizationDictionary[metric.MetricType].Max - normalizationDictionary[metric.MetricType].Min); 
                }
            }

            // Sum and divide by amount of metrics
            foreach (var tto in TTOs)
            {
                var value = 0.0;
                foreach (var metric in tto.Metrics)
                {
                    value += metric.Value * metric.Weight;
                }
                tto.EndValue = value;
            }

            // Sort list using end values
            TTOs = TTOs.OrderByDescending(inst => inst.EndValue).ToList();
            return TTOs;
        }

        public static List<TOTreeTangibleObject> GetOrderingTO(NarrativeTimePoint ntp, Configuration c, List<TangibleObject> tangibleObjects, List<Predicate> predicates)
        {
            Weights = Constants.AllMetricWeights;
            return CalculateValues(ntp, c, tangibleObjects, predicates);
        }

        internal static List<TOTreeTangibleObject> GetDecorationOrderingTO(NarrativeTimePoint ntp, Configuration c, List<TangibleObject> tangibleObjects, List<Predicate> predicates)
        {
            Weights = Constants.DecorationMetricWeights;
            return CalculateValues(ntp, c, tangibleObjects, predicates);
        }

        internal static List<TOTreeTangibleObject> GetRequiredOrderingTO(NarrativeTimePoint ntp, Configuration c, List<TangibleObject> tangibleObjects, List<Predicate> predicates)
        {
            Weights = Constants.RequiredMetricWeights;
            return CalculateValues(ntp, c, tangibleObjects, predicates);
        }

        private static Normalization GetRequiredNormalization()
        {
            Normalization ret = new Normalization();
            foreach (var tto in TTOs)
            {
                var requiredMetric = tto.Metrics.Where(m => m.MetricType.Equals(requiredMT)).FirstOrDefault();
                if (requiredMetric == null)
                    continue;
                var currentValue = requiredMetric.Value;
                if (currentValue < ret.Min)
                    ret.Min = currentValue;
                if (currentValue > ret.Max)
                    ret.Max = currentValue;
            }
            return ret;
        }

        private static Normalization GetRequiredDependencyNormalization()
        {
            Normalization ret = new Normalization();
            foreach (var tto in TTOs)
            {
                var requiredMetric = tto.Metrics.Where(m => m.MetricType.Equals(requiredDependencyMT)).FirstOrDefault();
                if (requiredMetric == null)
                    continue;
                var currentValue = requiredMetric.Value;
                if (currentValue < ret.Min)
                    ret.Min = currentValue;
                if (currentValue > ret.Max)
                    ret.Max = currentValue;
            }
            return ret;
        }

        private static Normalization SizeMetric()
        {
            Normalization ret = new Normalization();
            foreach (var tto in TTOs)
            {
                // Change default model to model of classname
                if (SystemStateTracker.DefaultModel != null)
                {
                    BoundingBox bb = EntikaInstance.GetBoundingBox(SystemStateTracker.DefaultModel, Matrix.Identity);
                    var difference = (bb.Max - bb.Min);
                    var currentArea = Math.Abs(difference.X) * Math.Abs(difference.Y);
                    if (currentArea < ret.Min)
                        ret.Min = currentArea;
                    if (currentArea > ret.Max)
                        ret.Max = currentArea;
                    tto.Metrics.Add(new Metric(AreaMT, currentArea, Weights[AreaMT.Name]));
                }
            }
            MetricTypeCount++;
            return ret;
        }

        private static Normalization OutAvailableEdges()
        {
            Normalization ret = new Normalization();
            foreach (var tto in TTOs)
            {
                var count = 0;
                foreach (var relationship in tto.RelationshipsAsSource)
                {
                    foreach (var treeInstance in ITEIs)
                    {
                        if (relationship.Target.TangibleObject.Equals(treeInstance.EntikaInstance.TangibleObject))
                        {
                            // if this is the case than there is an object available to link the relationship to
                            count++;
                        }
                    }
                }
                if (count < ret.Min)
                    ret.Min = count;
                if (count > ret.Max)
                    ret.Max = count;
                tto.Metrics.Add(new Metric(OutEdgesAvailableMT, count, Weights[OutEdgesAvailableMT.Name]));
                
            }
            MetricTypeCount++;
            return ret;

        }

        private static Normalization InAvailableEdges()
        {
            Normalization ret = new Normalization();
            foreach (var tto in TTOs)
            {
                var count = 0;
                foreach (var relationship in tto.RelationshipsAsTarget)
                {
                    foreach (var treeInstance in ITEIs)
                    {
                        if (relationship.Source.TangibleObject.Equals(treeInstance.EntikaInstance.TangibleObject))
                        {
                            // if this is the case than there is an object available to link the relationship to
                            count++;
                        }
                    }
                }
                if (count < ret.Min)
                    ret.Min = count;
                if (count > ret.Max)
                    ret.Max = count;
                tto.Metrics.Add(new Metric(IncEdgesAvailableMT, count, Weights[IncEdgesAvailableMT.Name]));
            }
            MetricTypeCount++;
            return ret;
        }

        private static Normalization OutgoingRequiredEdges()
        {
            Normalization ret = new Normalization();
            foreach (var tto in TTOs)
            {
                var currentCount = tto.RelationshipsAsSource.Where(ras => ras.Target.Required).ToList().Count;
                if (currentCount < ret.Min)
                    ret.Min = currentCount;
                if (currentCount > ret.Max)
                    ret.Max = currentCount;
                tto.Metrics.Add(new Metric(OutEdgesRequiredMT, currentCount, Weights[OutEdgesRequiredMT.Name]));
            }
            MetricTypeCount++;
            return ret;
        }

        private static Normalization IncomingRequiredEdges()
        {
            Normalization ret = new Normalization();
            foreach (var tto in TTOs)
            {
                var currentCount = tto.RelationshipsAsTarget.Where(ras => ras.Source.Required).ToList().Count;
                if (currentCount < ret.Min)
                    ret.Min = currentCount;
                if (currentCount > ret.Max)
                    ret.Max = currentCount;
                tto.Metrics.Add(new Metric(IncEdgesRequiredMT, currentCount, Weights[IncEdgesRequiredMT.Name]));
            }
            MetricTypeCount++;
            return ret;
        }

        internal static void ApplyRequiredAndDependencies(List<Predicate> predicates)
        {
            foreach (var tto in TTOs)
            {
                foreach (var predicate in predicates)
                {
                    foreach (var name in predicate.EntikaClassNames)
                    {
                        if (tto.TangibleObject.DefaultName.Equals(name))
                        {
                            tto.Required = true;
                            var found = false;
                            foreach (var metric in tto.Metrics)
                            {
                                if (metric.MetricType.Equals(requiredMT))
                                {
                                    metric.Value += 1.0;
                                    found = true;
                                }
                            }
                            if (!found)
                                tto.Metrics.Add(new Metric(requiredMT, 1.0, Weights[requiredMT.Name]));
                            // Cascade through relations and get relations on which this object depends and add metrics
                            CascadeRequiredMetricOnRelationships(tto);
                        }
                    }
                }
            }
        }

        private static Normalization OutgoingDecorativeEdges()
        {
            Normalization ret = new Normalization();
            foreach (var tto in TTOs)
            {
                var currentCount = tto.RelationshipsAsTarget.Where(ras => ras.Target.Decorative).ToList().Count;
                if (currentCount < ret.Min)
                    ret.Min = currentCount;
                if (currentCount > ret.Max)
                    ret.Max = currentCount;
                tto.Metrics.Add(new Metric(OutEdgesDecorativeMT, currentCount, Weights[OutEdgesDecorativeMT.Name]));
            }
            MetricTypeCount++;
            return ret;
        }

        private static Normalization IncomingDecorativeEdges()
        {
            Normalization ret = new Normalization();
            foreach (var tto in TTOs)
            {
                var currentCount = tto.RelationshipsAsSource.Where(ras => ras.Target.Decorative).ToList().Count;
                if (currentCount < ret.Min)
                    ret.Min = currentCount;
                if (currentCount > ret.Max)
                    ret.Max = currentCount;
                tto.Metrics.Add(new Metric(IncEdgesDecorativeMT, currentCount, Weights[IncEdgesDecorativeMT.Name]));
            }
            MetricTypeCount++;
            return ret;
        }

        private static Normalization DecorationWeights(NarrativeTimePoint ntp)
        {
            Normalization ret = new Normalization();
            foreach (var tto in TTOs)
            {
                foreach (var relationship in tto.TangibleObject.RelationshipsAsTarget)
                {
                    if (relationship.RelationshipType.DefaultName.Equals(Constants.DecorationRelationshipType))
                    {
                        if (relationship.Source.DefaultName.Equals(ntp.Location.LocationType))
                        {
                            // Add decorationweight metric
                            tto.Decorative = true;
                            var currentDecorativeWeight = Double.Parse(relationship.Attributes[0].Value.ToString());
                            if (currentDecorativeWeight < ret.Min)
                                ret.Min = currentDecorativeWeight;
                            if (currentDecorativeWeight > ret.Max)
                                ret.Max = currentDecorativeWeight;
                            tto.Metrics.Add(new Metric(DecorationMT, currentDecorativeWeight, Weights[DecorationMT.Name]));
                        }
                    }
                }
            }
            MetricTypeCount++;
            return ret;
        }

        private static Normalization OutgoingEdges()
        {
            Normalization ret = new Normalization();
            foreach (var tto in TTOs)
            {
                var currentCount = tto.RelationshipsAsSource.Where(ras => Constants.GeometricRelationshipTypes.Contains(ras.Relationship.RelationshipType.DefaultName)).ToList().Count;
                if (currentCount < ret.Min)
                    ret.Min = currentCount;
                if (currentCount > ret.Max)
                    ret.Max = currentCount;
                tto.Metrics.Add(new Metric(OutEdgesMT, currentCount, Weights[OutEdgesMT.Name]));
            }
            MetricTypeCount++;
            return ret;
        }

        private static Normalization IncomingEdges()
        {
            Normalization ret = new Normalization();
            foreach (var tto in TTOs)
            {
                var currentCount = tto.RelationshipsAsTarget.Where(ras => Constants.GeometricRelationshipTypes.Contains(ras.Relationship.RelationshipType.DefaultName)).ToList().Count;
                if (currentCount < ret.Min)
                    ret.Min = currentCount;
                if (currentCount > ret.Max)
                    ret.Max = currentCount;
                tto.Metrics.Add(new Metric(IncEdgesMT, currentCount, Weights[IncEdgesMT.Name]));
            }
            MetricTypeCount++;
            return ret;
        }


        private static void CascadeRequiredMetricOnRelationships(TOTreeTangibleObject tto)
        {
            foreach (var relation in tto.RelationshipsAsTarget)
            {
                if (relation.Relationship.RelationshipType.DefaultName.Equals(Constants.On))
                {
                    relation.Source.RequiredDependency = true;
                    var found = false;
                    foreach (var metric in relation.Source.Metrics)
                    {
                        if (metric.MetricType.Equals(requiredDependencyMT))
                        {
                            metric.Value += 1.0;
                            found = true;
                        }
                    }
                    if (!found)
                        relation.Source.Metrics.Add(new Metric(requiredDependencyMT, 1.0, Weights[requiredDependencyMT.Name]));
                    CascadeRequiredMetricOnRelationships(relation.Source);
                }
            }
        }
    }
}
