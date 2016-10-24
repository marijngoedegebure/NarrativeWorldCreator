using System;
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

        public NarrativeObject()
        {
        }
    }
}
