﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds
{
    public class TimePointSpecificFill
    {
        // List of Narrative Character instances
        public List<NarrativeShape> NarrativeCharacterInstances { get; set; }

        // List of geometric relationships of character instances
        public List<GeometricRelationshipBase> GeometricRelationshipsOfNarrativeCharacters { get; set; }

        // List of Narrative Object instances
        public List<NarrativeShape> NarrativeThingInstances { get; set; }

        // List of geometric relationships of object instances
        public List<GeometricRelationshipBase> GeometricRelationshipsOfNarrativeThings { get; set; }

        public List<EntikaInstance> OtherObjectInstances { get; set; }

        // List of shapes
        public List<NarrativeShape> NarrativeShapes { get; set; }

        public TimePointSpecificFill()
        {
            NarrativeCharacterInstances = new List<NarrativeShape>();
            GeometricRelationshipsOfNarrativeCharacters = new List<GeometricRelationshipBase>();
            NarrativeThingInstances = new List<NarrativeShape>();
            GeometricRelationshipsOfNarrativeThings = new List<GeometricRelationshipBase>();
            OtherObjectInstances = new List<EntikaInstance>();
            NarrativeShapes = new List<NarrativeShape>();
        }
    }
}
