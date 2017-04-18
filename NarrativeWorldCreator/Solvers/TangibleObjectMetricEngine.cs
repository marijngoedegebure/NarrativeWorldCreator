using NarrativeWorldCreator.Models.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Solvers
{
    public static class TangibleObjectMetricEngine
    {
        public static MetricType IncEdgesMT = new MetricType("Incoming edges");
        public static MetricType OutEdgesMT = new MetricType("Outgoing edges");

        public static List<TangibleObjectValued> GetDecorationOrderingTO(List<Semantics.Entities.TangibleObject> tangibleObjects)
        {
            var tosValued = new List<TangibleObjectValued>();

            // For each instance, create new entika instance valued 
            foreach (var to in tangibleObjects)
            {
                tosValued.Add(new TangibleObjectValued(to));
            }

            // For each instance, for each decorationType, calculate value
            foreach (var toValued in tosValued)
            {
                var IncEdgesMetric = new Metric(IncEdgesMT, CalculateIncEdgesMetric(toValued));
                toValued.Metrics.Add(IncEdgesMetric);
                var OutEdgesMetric = new Metric(OutEdgesMT, CalculateOutEdgesMetric(toValued));
                toValued.Metrics.Add(OutEdgesMetric);


                toValued.EndValue = (
                    IncEdgesMetric.Value
                    + OutEdgesMetric.Value
                    ) / toValued.Metrics.Count;
            }
            // Normalize before sorting?

            // Sort list using end values
            tosValued = tosValued.OrderByDescending(inst => inst.EndValue).ToList();
            return tosValued;
        }

        private static double CalculateIncEdgesMetric(TangibleObjectValued toValued)
        {
            return toValued.TangibleObject.RelationshipsAsTarget.Count;
        }

        private static double CalculateOutEdgesMetric(TangibleObjectValued toValued)
        {
            return toValued.TangibleObject.RelationshipsAsSource.Count;
        }
    }
}
