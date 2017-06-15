
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Semantics.Entities;
using NarrativeWorldCreator.Models.NarrativeInput;
using NarrativeWorldCreator.Models.NarrativeTime;
using NarrativeWorldCreator.Models.NarrativeGraph;
using Microsoft.Xna.Framework.Graphics;

namespace NarrativeWorldCreator.Models
{
    public class NarrativeWorld
    {
        // Narrative information
        public Narrative Narrative { get; set; }
        public Graph Graph { get; set; }
        public NarrativeTimeline NarrativeTimeline { get; set; }

        // Entika information
        public List<TangibleObject> AvailableTangibleObjects { get; set; }
        public Dictionary<TangibleObject, Model> ModelsForTangibleObjects { get; set; }

        public NarrativeWorld ()
        {
        }
    }
}
