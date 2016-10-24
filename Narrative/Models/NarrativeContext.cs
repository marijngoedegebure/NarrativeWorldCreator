using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Narratives
{
    public class NarrativeContext : DbContext
    {
        public DbSet<Narrative> Narratives { get; set; }
        public DbSet<NarrativeAction> NarrativeActions { get; set; }
        public DbSet<NarrativeArgument> NarrativeArguments { get; set; }
        public DbSet<NarrativeEvent> NarrativeEvents { get; set; }
        public DbSet<NarrativeObject> NarrativeObjects { get; set; }
        public DbSet<NarrativePredicate> NarrativePredicates { get; set; }
        public DbSet<NarrativeObjectType> NarrativeObjectTypes { get; set; }
        public DbSet<PredicateType> PredicateTypes { get; set; }

        public NarrativeContext() : base("NarrativeWorldSQLConnection")
        {

        }
    }
}
