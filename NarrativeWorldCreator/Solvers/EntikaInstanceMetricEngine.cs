using NarrativeWorldCreator.Models.Metrics;
using NarrativeWorldCreator.Models.NarrativeTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Solvers
{
    public static class EntikaInstanceMetricEngine
    {
        public static MetricType AreaMT = new MetricType("Area");
        public static MetricType IncEdgesMT = new MetricType("Incoming edges");
        public static MetricType OutEdgesMT = new MetricType("Outgoing edges");

        public static List<EntikaInstanceValued> GetDecorationOrderingEI(NarrativeTimePoint currentNTP)
        {
            var valuedInstances = new List<EntikaInstanceValued>();

            // For each instance, create new entika instance valued 
            foreach (var instance in currentNTP.GetEntikaInstancesWithoutFloor())
            {
                valuedInstances.Add(new EntikaInstanceValued(instance));
            }

            // For each instance, for each decorationType, calculate value
            foreach (var instance in valuedInstances)
            {
                var AreaMetric = new Metric(AreaMT, CalculateAreaMetric(instance));
                instance.Metrics.Add(AreaMetric);
                var IncEdgesMetric = new Metric(IncEdgesMT, CalculateIncEdgesMetric(instance));
                instance.Metrics.Add(IncEdgesMetric);
                var OutEdgesMetric = new Metric(OutEdgesMT, CalculateOutEdgesMetric(instance));
                instance.Metrics.Add(OutEdgesMetric);


                instance.EndValue = (
                    AreaMetric.Value
                    + IncEdgesMetric.Value
                    + OutEdgesMetric.Value
                    ) / instance.Metrics.Count;
            }
            // Normalize before sorting?

            // Sort list using end values
            valuedInstances = valuedInstances.OrderByDescending(inst => inst.EndValue).ToList();
            return valuedInstances;
        }

        private static double CalculateIncEdgesMetric(EntikaInstanceValued instance)
        {
            return instance.EntikaInstance.TangibleObject.RelationshipsAsTarget.Count;
        }

        private static double CalculateOutEdgesMetric(EntikaInstanceValued instance)
        {
            return instance.EntikaInstance.TangibleObject.RelationshipsAsSource.Count;
        }

        private static double CalculateAreaMetric(EntikaInstanceValued instance)
        {
            var difference = (instance.EntikaInstance.BoundingBox.Max - instance.EntikaInstance.BoundingBox.Min);
            // Area = height x width
            return Math.Abs(difference.X) * Math.Abs(difference.Y);
        }
    }
}
