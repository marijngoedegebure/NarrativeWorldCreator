using Common;
using Microsoft.Xna.Framework;
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
        // List of narrative characters and their location when this event needs to occur
        public Dictionary<NarrativeCharacter, Node> LocationOfNarrativeCharacters { get; set; }
        // List of narrative objects and their location when this event needs to occur
        public Dictionary<NarrativeThing, Node> LocationOfNarrativeThings { get; set; }

        public TimePointSpecificFill TimePointSpecificFill { get; set; }

        public NarrativeTimePoint(int timePoint)
        {
            this.TimePoint = timePoint;
            LocationOfNarrativeCharacters = new Dictionary<NarrativeCharacter, Node>();
            LocationOfNarrativeThings = new Dictionary<NarrativeThing, Node>();
            TimePointSpecificFill = new TimePointSpecificFill();
        }

        internal void copy(NarrativeTimePoint initialTimePoint)
        {
            foreach(KeyValuePair<NarrativeCharacter, Node> entry in initialTimePoint.LocationOfNarrativeCharacters)
            {
                this.LocationOfNarrativeCharacters[entry.Key] = entry.Value;
            }

            foreach (KeyValuePair<NarrativeThing, Node> entry in initialTimePoint.LocationOfNarrativeThings)
            {
                this.LocationOfNarrativeThings[entry.Key] = entry.Value;
            }
        }

        public List<NarrativeCharacter> GetNarrativeCharactersByNode(Node selectedNode)
        {
            List<NarrativeCharacter> ncs = new List<NarrativeCharacter>();
            foreach (KeyValuePair<NarrativeCharacter, Node> entry in this.LocationOfNarrativeCharacters)
            {
                if(entry.Value.Equals(selectedNode))
                {
                    ncs.Add(entry.Key);
                }
            }
            return ncs;
        }

        public List<NarrativeThing> GetNarrativeThingsByNode(Node selectedNode)
        {
            List<NarrativeThing> nts = new List<NarrativeThing>();
            foreach (KeyValuePair<NarrativeThing, Node> entry in this.LocationOfNarrativeThings)
            {
                if (entry.Value.Equals(selectedNode))
                {
                    nts.Add(entry.Key);
                }
            }
            return nts;
        }

        public void SetBaseShape(Node selectedNode)
        {
            if (this.TimePointSpecificFill.NarrativeShapes.Count == 0)
            {
                // Setup EntikaInstance with on(X, floor) relationship
                var floorInstance = new EntikaInstance("floor");
                var floorShape = new NarrativeShape(0, new Polygon(selectedNode.Shape.Points), ShapeType.Relationship, floorInstance);
                var floorRelationship = new GeometricRelationshipBase(GeometricRelationshipBase.RelationshipTypes.On);
                floorShape.Relations.Add(floorRelationship);
                floorRelationship.Target = floorInstance;
                // Add on(X, floor) relation
                floorInstance.RelationshipsAsTarget.Add(floorRelationship);

                // Add everything to fill:
                this.TimePointSpecificFill.NarrativeShapes.Add(floorShape);
                this.TimePointSpecificFill.FloorInstance = floorInstance;
                this.TimePointSpecificFill.Relationships.Add(floorRelationship);
            }
            else
            {
                // Update the shape all others reference
                this.TimePointSpecificFill.NarrativeShapes[0].Polygon = new Polygon(selectedNode.Shape.Points);
            }
        }
    }
}
