using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NarrativeWorlds.NarrativeShape;

namespace NarrativeWorlds
{
    public class TimePointSpecificFill
    {
        public List<string> WishList = new List<string>() { "dinner table", "lamp", "couch", "wall", "chair" };

        // List of Narrative Character instances
        public List<NarrativeShape> NarrativeCharacterInstances { get; set; }

        // List of geometric relationships of character instances
        public List<GeometricRelationshipBase> GeometricRelationshipsOfNarrativeCharacters { get; set; }

        // List of Narrative Object instances
        public List<NarrativeShape> NarrativeThingInstances { get; set; }

        // List of geometric relationships of object instances
        public List<GeometricRelationshipBase> GeometricRelationshipsOfNarrativeThings { get; set; }

        public EntikaInstance FloorInstance { get; set; }
        public List<EntikaInstance> OtherObjectInstances { get; set; }

        // List of relationships, first relationship is always baseshape (ground)
        public List<GeometricRelationshipBase> Relationships { get; set; }
        // List of shapes that allow placement of objects, first shape is always baseshape (ground)
        public List<NarrativeShape> NarrativeShapes { get; set; }
        // List of shapes that designate clearances
        public List<NarrativeShape> ClearanceShapes { get; internal set; }

        public TimePointSpecificFill()
        {
            NarrativeCharacterInstances = new List<NarrativeShape>();
            GeometricRelationshipsOfNarrativeCharacters = new List<GeometricRelationshipBase>();
            NarrativeThingInstances = new List<NarrativeShape>();
            GeometricRelationshipsOfNarrativeThings = new List<GeometricRelationshipBase>();
            OtherObjectInstances = new List<EntikaInstance>();
            NarrativeShapes = new List<NarrativeShape>();
            Relationships = new List<GeometricRelationshipBase>();
            ClearanceShapes = new List<NarrativeShape>();
        }
    }
}
