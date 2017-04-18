using Common;
using Microsoft.Xna.Framework;
using NarrativeWorlds.Models.NarrativeRegionFill;
using PDDLNarrativeParser;
using Semantics.Entities;
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

        // List of TangibleObjects selected for this region/timepoint
        public List<TangibleObject> AvailableTangibleObjects { get; set; }

        public List<NarrativePredicateInstance> NarrativePredicateInstances { get; set; }

        // Story constraints for this timepoint
        public List<NarrativePredicateInstance> PredicatesInstancesFilteredLocation { get; set; }

        public List<RelationshipInstance> InstancedRelations { get; set; }
        public List<EntikaInstance> InstancedObjects { get; set; }


        public TimePointSpecificFill TimePointSpecificFill { get; set; }

        public NarrativeTimePoint(int timePoint, List<TangibleObject> DefaultTangibleObjects)
        {
            this.TimePoint = timePoint;
            PredicatesInstancesFilteredLocation = new List<NarrativePredicateInstance>();
            NarrativePredicateInstances = new List<NarrativePredicateInstance>();
            InstancedRelations = new List<RelationshipInstance>();
            InstancedObjects = new List<EntikaInstance>();

            TimePointSpecificFill = new TimePointSpecificFill();
            AvailableTangibleObjects = DefaultTangibleObjects;
        }

        public void CopyInstancedNarrativePredicates(NarrativeTimePoint initial)
        {
            foreach (var narrativePredicateInstance in initial.NarrativePredicateInstances)
            {
                this.NarrativePredicateInstances.Add(new NarrativePredicateInstance(narrativePredicateInstance.NarrativePredicate));
            }
        }

        public List<EntikaInstance> GetEntikaInstancesWithoutFloor()
        {
            return this.InstancedObjects.Where(io => !io.Name.Equals("Floor")).ToList();
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
                var floor = this.InstancedObjects.Where(io => io.Name.Equals("Floor")).FirstOrDefault();
                floor.Polygon = new Polygon(selectedNode.Shape.Points);
                floor.UpdateBoundingBoxAndShape(null);
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
            var floorInstance = new EntikaInstance("Floor", new Polygon(selectedNode.Shape.Points));
            this.InstancedObjects.Add(floorInstance);

            // TimepointSpecificFill stuff:
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

        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            NarrativeTimePoint e = (NarrativeTimePoint)obj;
            // Equals if either both from nodes are equal and both to nodes are equal or if they are reversed.
            return this.TimePoint.Equals(e.TimePoint);
        }
    }
}
