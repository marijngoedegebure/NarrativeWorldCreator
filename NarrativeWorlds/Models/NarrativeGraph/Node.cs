using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PDDLNarrativeParser;
using PolygonCuttingEar;
using SharpNav;
using SharpNavVector3 = SharpNav.Geometry.Vector3;
using SharpNavTriangle3 = SharpNav.Geometry.Triangle3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds
{
    public class Node
    {
        public String LocationName { get; set; }
        public List<Vector3> RegionOutlinePoints { get; set; }
        // Triangulated region points
        public CPolygonShape TriangulatedPolygon { get; set; }

        public List<int> triangleListIndices { get; set; }
        public List<NarrativeEvent> NarrativeEvents { get; set; }
        public List<NarrativeObject> NarrativeObjects { get; set; }
        public List<EntikaClassInstance> EntikaClassInstances { get; set; }
        public RegionBaseFill RegionBaseFill { get; set; }

        public Node(String locationName)
        {
            this.LocationName = locationName;
            RegionOutlinePoints = new List<Vector3>();
            triangleListIndices = new List<int>();
            NarrativeEvents = new List<NarrativeEvent>();
            NarrativeObjects = new List<NarrativeObject>();
            EntikaClassInstances = new List<EntikaClassInstance>();
        }

        public override bool Equals(Object obj)
        {
            // Check for null values and compare run-time types.
            if (obj == null || GetType() != obj.GetType())
                return false;

            Node n = (Node)obj;
            return LocationName.Equals(n.LocationName);
        }

        public void triangulatePolygon()
        {
            if (RegionOutlinePoints.Count > 2)
            {
                TriangulatedPolygon = new CPolygonShape(ConvertVectorToPoint().ToArray());
                TriangulatedPolygon.CutEar();
            }
        }

        private List<CPoint2D> ConvertVectorToPoint()
        {
            List<CPoint2D> ret = new List<CPoint2D>();
            foreach(Vector3 vector in RegionOutlinePoints)
            {
                ret.Add(new CPoint2D(vector.X, vector.Y));
            }
            return ret;
        }

        public List<VertexPositionColor> GetDrawableTriangles(Color color)
        {
            List<VertexPositionColor> ret = new List<VertexPositionColor>();
            for(int i = 0; i < TriangulatedPolygon.NumberOfPolygons; i++)
            {
                CPoint2D[] vertices = TriangulatedPolygon.Polygons(i);
                foreach(CPoint2D vert in vertices)
                {
                    ret.Add(new VertexPositionColor(new Vector3((float) vert.X, (float) vert.Y, 0), color));
                }
            }
            return ret;
        }
    }
}
