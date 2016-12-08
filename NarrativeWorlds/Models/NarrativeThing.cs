using Semantics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds
{
    public class NarrativeThing
    {
        public string Name { get; set; }
        public bool Placed { get; set; }
        public TangibleObject TangibleObject { get; set; }

        public NarrativeThing(string Name, TangibleObject to)
        {
            this.Name = Name;
            this.Placed = false;
            this.TangibleObject = to;
        }
    }
}
