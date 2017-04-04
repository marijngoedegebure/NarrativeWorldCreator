using Common;
using Microsoft.Xna.Framework;
using NarrativeWorlds.Models.NarrativeRegionFill;
using PDDLNarrativeParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NarrativeWorlds.NarrativeShape;

namespace NarrativeWorlds
{
    public class NarrativeTimePoint
    {
        // Narrative time point, increases by 1 for each time point, initial time point is 0
        public int TimePoint { get; set; }
        // Narrative event associated with this timepoint, just for potential later reference, should not be necessary
        public NarrativeEvent NarrativeEvent { get; set; }
        // Location mentioned in the narrative event
        public Node Location { get; set; }

        // Story constraints for this timepoint
        public List<NarrativeObjectEntikaLink> NarrativeObjectEntikaLinks { get; set; }
        public List<NarrativePredicateInstance> NarrativePredicateInstances { get; set; }

        public TimePointSpecificFill TimePointSpecificFill { get; set; }

        public NarrativeTimePoint(int timePoint)
        {
            this.TimePoint = timePoint;
            NarrativeObjectEntikaLinks = new List<NarrativeObjectEntikaLink>();
            NarrativePredicateInstances = new List<NarrativePredicateInstance>();
            TimePointSpecificFill = new TimePointSpecificFill();
        }

        public void CopyInstancedNarrativePredicates(NarrativeTimePoint initial)
        {
            foreach (var narrativePredicateInstance in initial.NarrativePredicateInstances)
            {
                this.NarrativePredicateInstances.Add(new NarrativePredicateInstance(narrativePredicateInstance.NarrativePredicate));
            }
        }

        // This function allows updating
        public void SetBaseShape(Node selectedNode)
        {
            if (this.TimePointSpecificFill.NarrativeShapes.Count == 0)
            {
                SetupFloorInstance(selectedNode);
            }
            else
            {
                // Update the shape all others reference
                this.TimePointSpecificFill.NarrativeShapes[0].Polygon = new Polygon(selectedNode.Shape.Points);
            }
        }

        // This only allows the new instantiation of the current floor instance when the timepoint is newly selected
        public void SwitchTimePoints(Node selectedNode)
        {
            if (this.TimePointSpecificFill.NarrativeShapes.Count == 0)
            {
                SetupFloorInstance(selectedNode);
            }
        }

        private void SetupFloorInstance(Node selectedNode)
        {
            // Setup EntikaInstance with on(X, floor) relationship
            var floorInstance = new EntikaInstance("floor");
            var floorShape = new NarrativeShape(0, new Polygon(selectedNode.Shape.Points), ShapeType.Relationship, floorInstance);
            var floorRelationship = new GeometricRelationshipBase(GeometricRelationshipBase.RelationshipTypes.On);
            floorShape.Relations.Add(floorRelationship);
            floorRelationship.Source = floorInstance;
            // Add on(X, floor) relation
            floorInstance.RelationshipsAsSource.Add(floorRelationship);

            // Add everything to fill:
            this.TimePointSpecificFill.NarrativeShapes.Add(floorShape);
            this.TimePointSpecificFill.FloorInstance = floorInstance;
            this.TimePointSpecificFill.Relationships.Add(floorRelationship);
        }
    }
}
