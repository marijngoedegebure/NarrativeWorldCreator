using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Models.NarrativeRegionFill
{
    public class Configuration
    {
        public List<RelationshipInstance> InstancedRelations { get; set; }
        public List<EntikaInstance> InstancedObjects { get; set; }

        public Configuration()
        {
            InstancedRelations = new List<RelationshipInstance>();
            InstancedObjects = new List<EntikaInstance>();
        }

        internal Configuration Copy()
        {
            var config = new Configuration();
            foreach (var obj in this.InstancedObjects)
            {
                config.InstancedObjects.Add(new EntikaInstance(obj));
            }
            foreach (var rel in this.InstancedRelations)
            {
                var relcopy = new RelationshipInstance(rel);
                var source = config.InstancedObjects.Where(io => io.Name.Equals(rel.Source.Name)).FirstOrDefault();
                var target = config.InstancedObjects.Where(io => io.Name.Equals(rel.Target.Name)).FirstOrDefault();
                relcopy.Source = source;
                relcopy.Target = target;
                source.RelationshipsAsSource.Add(relcopy);
                target.RelationshipsAsTarget.Add(relcopy);
                config.InstancedRelations.Add(relcopy);
            }
            return config;
        }

        internal List<RelationshipInstance> GetValuedRelationships()
        {
            return this.InstancedRelations.Where(ir => ir.Valued).ToList();
        }

        public List<EntikaInstance> GetEntikaInstancesWithoutFloor()
        {
            return this.InstancedObjects.Where(io => !io.Name.Equals(Constants.Floor)).ToList();
        }
    }
}
