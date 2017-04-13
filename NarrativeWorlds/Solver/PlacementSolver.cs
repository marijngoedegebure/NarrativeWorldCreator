using Microsoft.Xna.Framework;
using NarrativeWorlds.Models.NarrativeRegionFill;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds.Solver
{
    public static class PlacementSolver
    {
        public static List<Vector3> GenerateRandomPosition(NarrativeTimePoint ntp, EntikaInstance instanceToPlace)
        {
            var ret = new List<Vector3>();

            var floor = ntp.InstancedObjects.Where(io => io.Name.Equals("Floor")).FirstOrDefault();
            var minBB = floor.BoundingBox.Min;
            var maxBB = floor.BoundingBox.Max;

            var differenceX = maxBB.X - minBB.X;
            var differenceY = maxBB.Y - minBB.Y;

            Random r = new Random();

            for (int i = 0; i < 8; i++)
            {
                float X = (float)((r.NextDouble() * differenceX) - Math.Abs(minBB.X));
                float Y = (float)((r.NextDouble() * differenceY) - Math.Abs(minBB.Y));
                ret.Add(new Vector3(X, Y, 0));
            }
            return ret;
        }

        public static double CalculatePairwiseEnergy(RelationshipInstance instance)
        {
            // Calculate distance between two entikainstances
            var distance = CalculateDistance(instance.Source, instance.Targets[0]);
            // Determine output energy based distance, targetrange start, targetrange end and degree of attraction (static)

            return -1 * EnergyFunction(distance, instance.TargetRangeStart.GetValueOrDefault(), instance.TargetRangeEnd.GetValueOrDefault(), 2.0);
        }

        public static double EnergyFunction(double distance, double rangeStart, double rangeEnd, double degreeOfAtrraction)
        {
            if (distance < rangeStart)
            {
                var fraction = distance / rangeStart;
                return Math.Pow(fraction, degreeOfAtrraction);
            }
            else if (distance > rangeEnd)
            {
                var fraction = rangeEnd / distance;
                return Math.Pow(fraction, degreeOfAtrraction);
            }
            else
            {
                return 1;
            }
        }

        public static double CalculateDistance(EntikaInstance start, EntikaInstance end)
        {
            var distanceF = Vector3.Distance(start.Position, end.Position);
            return (double) distanceF;
        }

    }
}
