using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.NarrativeInput
{
    public class NarrativeObjectType
    {
        public int NarrativeObjectTypeId { get; set; }
        public String Name { get; set; }
        public NarrativeObjectType ParentType { get; set; }

        public NarrativeObjectType()
        {
        }

        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            NarrativeObjectType e = (NarrativeObjectType)obj;
            // Equals if either both from nodes are equal and both to nodes are equal or if they are reversed.
            return Name.Equals(e.Name);
        }
    }
}
