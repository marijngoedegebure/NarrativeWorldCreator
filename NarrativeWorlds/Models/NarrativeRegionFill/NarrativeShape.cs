using Common;
using Common.Geometry;
using GameSemantics.Components;
using GameSemantics.Data;
using GameSemanticsEngine.GameContent;
using GameSemanticsEngine.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Semantics.Data;
using Semantics.Entities;
using SemanticsEngine.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds
{
    public class NarrativeShape
    {
        public enum ShapeType
        {
            Clearance,
            Offlimits,
            Relationship
        }
        public ShapeType Type { get; set; }

        // Shape derived from spaces, models and position
        public Polygon Polygon { get; set; }
        // Zpos is simplified to be the same for the entire shape, all shapes are parallel to x and y axis
        public float zpos { get; set; }

        public EntikaInstance Parent { get; set; }

        public NarrativeShape(float zpos, Polygon polygon, ShapeType type, EntikaInstance parent)
        {
            this.zpos = zpos;
            this.Polygon = polygon;
            this.Type = type;
            this.Parent = parent;
        }
    }
}
