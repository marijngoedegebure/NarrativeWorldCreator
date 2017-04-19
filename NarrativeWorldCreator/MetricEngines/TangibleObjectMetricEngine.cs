using NarrativeWorldCreator.Models.Metrics;
using NarrativeWorldCreator.Models.Metrics.TOTree;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.MetricEngines
{
    public static class TangibleObjectMetricEngine
    {
        public static MetricType IncEdgesMT = new MetricType("Incoming edges");
        public static MetricType OutEdgesMT = new MetricType("Outgoing edges");
        public static MetricType requiredMT = new MetricType("Required");
        public static MetricType requiredDependencyMT = new MetricType("Required dependency");

        public static List<TreeTangibleObject> TTOs = new List<TreeTangibleObject>();
        public static List<TreeRelationship> TreeRelationships = new List<TreeRelationship>();

        public static void BuildUpTOTree(List<Semantics.Entities.TangibleObject> tangibleObjects)
        {
            foreach (var to in tangibleObjects)
            {
                var treeTOSource = (from tto in TTOs
                                    where tto.TangibleObject.Equals(to)
                                    select tto).FirstOrDefault();
                if (treeTOSource == null)
                {
                    treeTOSource = new TreeTangibleObject(to);
                    TTOs.Add(treeTOSource);
                }
                foreach (var relationship in to.RelationshipsAsSource)
                {
                    var treeTOTarget = (from tto in TTOs
                                        where tto.TangibleObject.Equals((Semantics.Entities.TangibleObject)relationship.Targets[0])
                                        select tto).FirstOrDefault();

                    if (treeTOTarget  == null)
                    {
                        treeTOTarget = new TreeTangibleObject((Semantics.Entities.TangibleObject)relationship.Targets[0]);
                        TTOs.Add(treeTOTarget);
                    }
                    var Trelationship = new TreeRelationship(relationship);
                    Trelationship.Source = treeTOSource;
                    Trelationship.Target = treeTOTarget;
                    TreeRelationships.Add(Trelationship);
                    treeTOSource.RelationshipsAsSource.Add(Trelationship);
                    treeTOTarget.RelationshipsAsTarget.Add(Trelationship);
                }
            }
        }

        public static void ResetTree()
        {
            TTOs = new List<TreeTangibleObject>();
            TreeRelationships = new List<TreeRelationship>();
        }

        public static List<TreeTangibleObject> GetDecorationOrderingTO(List<Semantics.Entities.TangibleObject> tangibleObjects, List<Predicate> predicates)
        {
            ResetTree();
            BuildUpTOTree(tangibleObjects);

            // For each instance, for each decorationType, calculate value
            foreach (var tto in TTOs)
            {
                var IncEdgesMetric = new Metric(IncEdgesMT, CalculateIncEdgesMetric(tto));
                tto.Metrics.Add(IncEdgesMetric);
                var OutEdgesMetric = new Metric(OutEdgesMT, CalculateOutEdgesMetric(tto));
                tto.Metrics.Add(OutEdgesMetric);
            }

            // Go through objects and highlight the ones that are required by the predicates
            foreach (var tto in TTOs)
            {
                foreach (var predicate in predicates)
                {
                    foreach (var name in predicate.EntikaClassNames)
                    {
                        if (tto.TangibleObject.DefaultName.Equals(name))
                        {
                            tto.Metrics.Add(new Metric (requiredMT, 1.0));
                            // Cascade through relations and get relations on which this object depends and add metrics
                            CascadeRequiredMetricOnRelationships(tto);
                        }
                    }
                }
            }

            foreach (var tto in TTOs)
            {
                var value = 0.0;
                foreach (var metric in tto.Metrics)
                {
                    value += metric.Value;
                }
                tto.EndValue = value / tto.Metrics.Count;
            }

            // Normalize before sorting?

            // Sort list using end values
            TTOs = TTOs.OrderByDescending(inst => inst.EndValue).ToList();
            return TTOs;
        }

        private static void CascadeRequiredMetricOnRelationships(TreeTangibleObject tto)
        {
            foreach (var relation in tto.RelationshipsAsTarget)
            {
                if (relation.Relationship.RelationshipType.DefaultName.Equals("On"))
                {
                    relation.Source.Metrics.Add(new Metric(requiredDependencyMT, 1.0));
                    CascadeRequiredMetricOnRelationships(relation.Source);
                }
            }
        }

        private static double CalculateIncEdgesMetric(TreeTangibleObject tto)
        {
            return tto.RelationshipsAsTarget.Count;
        }

        private static double CalculateOutEdgesMetric(TreeTangibleObject tto)
        {
            return tto.RelationshipsAsSource.Count;
        }
    }
}
