using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.NarrativeInput
{

    public class NarrativeArgument
    {
        public string Name { get; set; }
        public NarrativeObjectType Type { get; set; }

        public NarrativeArgument()
        {
        }

        public override bool Equals(object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            NarrativeArgument e = (NarrativeArgument)obj;
            // Equals if either both from nodes are equal and both to nodes are equal or if they are reversed.
            return this.Name.Equals(e.Name) && this.Type.Equals(e.Type);
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode() + this.Type.GetHashCode();
        }
    }
}
