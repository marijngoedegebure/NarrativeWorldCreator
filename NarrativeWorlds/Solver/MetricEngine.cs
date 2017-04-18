using NarrativeWorlds.Models.Metrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds.Solver
{
    public static class MetricEngine
    {
        public static List<MetricType> MetricTypes = new List<MetricType>();

        public static List<MetricType> DecorationMetricTypes = new List<MetricType>();

        public static bool IsSetup = false;

        public static void Setup()
        {
            var sizeMT = new MetricType("Size");
            MetricTypes.Add(sizeMT);
            DecorationMetricTypes.Add(sizeMT);

            IsSetup = true;
        }

        public static List<EntikaInstanceValued> GetDecorationOrdering(NarrativeTimePoint currentNTP)
        {
            if (!IsSetup)
                Setup();


            var valuedInstances = new List<EntikaInstanceValued>();

            // For each instance, create new entika instance valued 
            foreach (var instance in currentNTP.GetEntikaInstancesWithoutFloor())
            {
                valuedInstances.Add(new EntikaInstanceValued(instance));
            }

            // For each instance, for each decorationType, calculate value
            foreach (var instance in valuedInstances)
            {
                foreach (var mt in DecorationMetricTypes)
                {
                    DetermineMTAndValue(mt, instance);
                }
            }
            
            // Combine values of each metric into end values
            foreach (var instance in valuedInstances)
            {
                double metricSum = 0.0;
                foreach(var metric in instance.Metrics)
                {
                    metricSum += metric.Value;
                }
                instance.EndValue = metricSum / instance.Metrics.Count;
            }
            // Sort list using end values
            valuedInstances.Sort((inst1, inst2) => inst1.EndValue.CompareTo(inst2.EndValue));
            return valuedInstances;
        }

        private static void DetermineMTAndValue(MetricType mt, EntikaInstanceValued instance)
        {
            if (mt.Name.Equals("Size"))
            {
                instance.Metrics.Add(new Metric(mt, CalculateSizeMetric(instance)));
            }
        }

        private static double CalculateSizeMetric(EntikaInstanceValued instance)
        {
            return (instance.EntikaInstance.BoundingBox.Max - instance.EntikaInstance.BoundingBox.Min).Length();
        }
    }
}
