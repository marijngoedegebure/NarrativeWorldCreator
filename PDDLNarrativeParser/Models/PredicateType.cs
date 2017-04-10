using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLNarrativeParser
{
    public class PredicateType
    {
        public int PredicateTypeId { get; set; }
        public string Name { get; set; }
        public IList<NarrativeArgument> Arguments { get; set; }

        public PredicateType()
        {
            Arguments = new List<NarrativeArgument>();
        }

        public override bool Equals(object obj)
        {
            var item = obj as PredicateType;

            if (item == null)
            {
                return false;
            }
            if (item.Arguments.Count != this.Arguments.Count)
            {
                return false;
            }
            for(int i = 0; i < this.Arguments.Count;i++)
            {
                if (!this.Arguments[i].Equals(item.Arguments[i]))
                {
                    return false;
                }
            }
            return this.Name.Equals(item.Name);
        }
    }
}
