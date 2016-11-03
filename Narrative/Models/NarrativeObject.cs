﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Narratives
{
    public class NarrativeObject
    {
        public int NarrativeObjectId { get; set; }
        public string Name { get; set; }
        public NarrativeObjectType Type { get; set; }
        public bool Placed { get; set; }

        public NarrativeObject()
        {
        }

        public override bool Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            NarrativeObject p = (NarrativeObject)obj;
            return (NarrativeObjectId == p.NarrativeObjectId);
        }
    }
}