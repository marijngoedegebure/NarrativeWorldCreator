using Microsoft.Xna.Framework;
using NarrativeWorldCreator.Models.Metrics;
using NarrativeWorldCreator.Models.Metrics.TOTree;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Models.NarrativeTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static MetricType requiredMT = new MetricType("required");
        public static MetricType requiredDependencyMT = new MetricType("required dependency");
        public static MetricType DecorationMT = new MetricType("decoration weight");
        public static MetricType AreaMT = new MetricType("area");

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

        public static List<TreeTangibleObject> GetDecorationOrderingTO(NarrativeTimePoint ntp, List<Semantics.Entities.TangibleObject> tangibleObjects, List<Predicate> predicates)
        {
            ResetTree();
            BuildUpTOTree(tangibleObjects);

            // For each instance, for each decorationType, calculate value
            foreach (var tto in TTOs)
            {
                tto.Metrics.Add(new Metric(IncEdgesMT, tto.RelationshipsAsTarget.Count));
            }

            foreach (var tto in TTOs)
            {
                tto.Metrics.Add(new Metric(OutEdgesMT, tto.RelationshipsAsSource.Count));
            }

            foreach (var tto in TTOs)
            {
                tto.Metrics.Add(new Metric(IncEdgesDecorativeMT, tto.RelationshipsAsTarget.Where(ras => ras.Relationship.RelationshipType.DefaultName.Equals("regionTypeClassAssociation")).ToList().Count));
            }

            foreach (var tto in TTOs)
            {
                tto.Metrics.Add(new Metric(OutEdgesDecorativeMT, tto.RelationshipsAsSource.Where(ras => ras.Relationship.RelationshipType.DefaultName.Equals("regionTypeClassAssociation")).ToList().Count));
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
                            tto.Metrics.Add(new Metric(requiredMT, 1.0));
                            // Cascade through relations and get relations on which this object depends and add metrics
                            CascadeRequiredMetricOnRelationships(tto);
                        }
                    }
                }
            }

            // Add decoration weight metric
            foreach (var tto in TTOs)
            {
                foreach (var relationship in tto.TangibleObject.RelationshipsAsTarget)
                {
                    if (relationship.RelationshipType.DefaultName.Equals("regionTypeClassAssociation"))
                    {
                        if (relationship.Source.DefaultName.Equals(ntp.Location.LocationType))
                        {
                            // Add decorationweight metric
                            tto.Metrics.Add(new Metric(DecorationMT, Double.Parse(relationship.Attributes[0].Value.ToString())));
                        }
                    }
                }
            }

            // Calculate size metric
            foreach (var tto in TTOs)
            {
                // Change default model to model of classname
                if (SystemStateTracker.DefaultModel != null)
                {
                    BoundingBox bb = EntikaInstance.GetBoundingBox(SystemStateTracker.DefaultModel, Matrix.Identity);
                    var difference = (bb.Max - bb.Min);
                    tto.Metrics.Add(new Metric(AreaMT, Math.Abs(difference.X) * Math.Abs(difference.Y)));
                }
            }

            // Normalize before sorting?

            // Sum and divide by amount of metrics
            foreach (var tto in TTOs)
            {
                var value = 0.0;
                foreach (var metric in tto.Metrics)
                {
                    value += metric.Value;
                }
                tto.EndValue = value / tto.Metrics.Count;
            }

            // Sort list using end values
            TTOs = TTOs.OrderByDescending(inst => inst.EndValue).ToList();
            return TTOs;
        }

        private static void CascadeRequiredMetricOnRelationships(TreeTangibleObject tto)
        {
            foreach (var relation in tto.RelationshipsAsTarget)
            {
                if (relation.Relationship.RelationshipType.DefaultName.Equals("on"))
                {
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
                        relation.Source.Metrics.Add(new Metric(requiredDependencyMT, 1.0));
                    CascadeRequiredMetricOnRelationships(relation.Source);
                }
            }
        }
    }
}
