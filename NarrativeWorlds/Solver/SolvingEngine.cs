using Common;
using Common.Geometry;
using Common.Shapes;
using Microsoft.Xna.Framework;
using Semantics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using TriangleNet.Geometry;

namespace NarrativeWorlds
{
    public static class SolvingEngine
    {
        // The basic version only removes shapes from the base shape and adds the new shape
        public static NarrativeTimePoint AddEntikaInstanceToTimePointBasic(NarrativeTimePoint ntp, EntikaInstance addition, NarrativeShape destinationShape)
        {
            // Create new off limits shape based on addition
            var offLimitPolygon = new Polygon(EntikaInstance.GetBoundingBoxAsPoints(addition.BoundingBox));
            // Add shape to instance
            addition.OffLimitsShape = new NarrativeShape(0, offLimitPolygon, NarrativeShape.ShapeType.Offlimits, addition);

            // Create new clearance limits shapes based on addition
            // Input is the shape description and the boundingbox
            foreach (SpaceValued space in addition.TangibleObject.Spaces)
            {
                if (space.Space.DefaultName.Equals("Clearance"))
                {
                    // Clearance should always be described as a extruded line, this implies three parameters and results in four coordinates which can be used as a definition of shape
                    var clearancePolygon = new Polygon(ParseShapeDescription(space.ShapeDescription, addition.BoundingBox));
                    addition.ClearanceShapes.Add(new NarrativeShape(0, clearancePolygon, NarrativeShape.ShapeType.Clearance, addition));
                }
            }

            // Create new relationship shapes based on addition
            // Fetch relationships
            var AddedRelationsShapes = new List<NarrativeShape>();
            foreach (var relationAsTarget in addition.TangibleObject.RelationshipsAsTarget)
            {
                var relationType = relationAsTarget.RelationshipType;
                // For each relationship, create shape, add this to narrativeshapes, and remove from baseshape
                // Kind of relationship:
                if (relationType.DefaultName.Equals("On"))
                {
                    var shape = CreateOnRelationShape(addition);
                    var relation = new GeometricRelationshipBase(GeometricRelationshipBase.RelationshipTypes.On);
                    relation.Target = addition;
                    shape.Relations.Add(relation);
                    addition.RelationshipsAsTarget.Add(relation);
                    // Add relation to fill
                    ntp.TimePointSpecificFill.Relationships.Add(relation);
                    AddedRelationsShapes.Add(shape);
                }
            }

            // Remove offlimits polygon from baseshape
            // Go through list of shapes in reverse order so that it allows deletion of shapes
            foreach(var shape in ntp.TimePointSpecificFill.NarrativeShapes.Reverse<NarrativeShape>())
            {
                var adjustedPolygon = HelperFunctions.DifferenceShapes(shape.Polygon, addition.OffLimitsShape.Polygon);
                if (adjustedPolygon == null)
                {
                    // If remaining polygon equals null, remove shape from list
                    ntp.TimePointSpecificFill.NarrativeShapes.Remove(shape);
                }
                shape.Polygon = adjustedPolygon;
            }
            // Remove clearance from baseshape and add it to timepoint, if it exists
            foreach(var clearanceShape in addition.ClearanceShapes)
            {
                foreach (var shape in ntp.TimePointSpecificFill.NarrativeShapes)
                {
                    shape.Polygon = HelperFunctions.DifferenceShapes(shape.Polygon, clearanceShape.Polygon);
                }
                ntp.TimePointSpecificFill.ClearanceShapes.Add(clearanceShape);
            }
            foreach (var relationalShape in AddedRelationsShapes)
            {
                var intersectionNarrativeShape = relationalShape;
                foreach (var shape in ntp.TimePointSpecificFill.NarrativeShapes.Reverse<NarrativeShape>())
                {
                    if (shape.zpos == relationalShape.zpos)
                    {
                        // Intersection of shapes
                        var intersection = HelperFunctions.IntersectShapes(shape.Polygon, relationalShape.Polygon);
                        if (intersection != null)
                        {
                            // Update intersection shape to intersection polygon
                            intersectionNarrativeShape.Polygon = intersection;
                            // Inherit relations of other shape
                            foreach (var relation in shape.Relations)
                            {
                                intersectionNarrativeShape.Relations.Add(relation);
                            }

                            // Check whether one of areas becomes null when differenced
                            var differenceRelationAndShape = HelperFunctions.DifferenceShapes(relationalShape.Polygon, shape.Polygon);
                            var differenceShapeAndRelation = HelperFunctions.DifferenceShapes(shape.Polygon, relationalShape.Polygon);
                            if (differenceRelationAndShape == null)
                            {
                                // Dont add relational shape to shapes, update shape
                                shape.Polygon = differenceShapeAndRelation;
                                continue;
                            }
                            else if (differenceShapeAndRelation == null)
                            {
                                // Remove existing shape
                                ntp.TimePointSpecificFill.NarrativeShapes.Remove(shape);
                                relationalShape.Polygon = differenceRelationAndShape;
                            }
                            else
                            {
                                // Update both shapes to remove intersection
                                relationalShape.Polygon = differenceRelationAndShape;
                                shape.Polygon = differenceShapeAndRelation;
                            }
                            // Add intersection
                            ntp.TimePointSpecificFill.NarrativeShapes.Add(intersectionNarrativeShape);
                        }
                    }
                }
                // Add shape to narrative shapes after it has been adjusted by (and has adjusted the baseshape)
                ntp.TimePointSpecificFill.NarrativeShapes.Add(relationalShape);
            }
            // Do not add off limit shape to narrative shapes as it is unusable
            //ntp.TimePointSpecificFill.NarrativeShapes.Add(additionOffLimitShape);
            ntp.TimePointSpecificFill.OtherObjectInstances.Add(addition);
            return ntp;
        }

        private static NarrativeShape CreateOnRelationShape(EntikaInstance addition)
        {
            // On relationship uses the max height of the boundingbox as zpos
            var zpos = addition.BoundingBox.Max.Z;

            // On relationship uses the heighest 4 points as points for the shape
            Vector3[] corners = addition.BoundingBox.GetCorners();
            // 8 corners, the last 4 are the top 4
            List<Vec2> points = new List<Vec2>();
            points.Add(new Vec2(corners[4].X, corners[4].Y));
            points.Add(new Vec2(corners[5].X, corners[5].Y));
            points.Add(new Vec2(corners[6].X, corners[6].Y));
            points.Add(new Vec2(corners[7].X, corners[7].Y));

            return new NarrativeShape(zpos, new Polygon(points), NarrativeShape.ShapeType.Relationship, addition);
        }

        private static List<Vec2> ParseShapeDescription(BaseShapeDescription ShapeDescription, Microsoft.Xna.Framework.BoundingBox bb)
        {
            List<float> values = new List<float>();
            // Parse parameters into values
            foreach (var parameterName in ShapeDescription.GetParameterNames())
            {
                var descriptor = ShapeDescription.GetParameterValue(parameterName);
                var valueDescriptor = descriptor.Substring(4, 3);
                var axis = descriptor.Substring(7, 1);
                float value = 0.0f;
                if (valueDescriptor.Equals("Min"))
                {
                    if (axis.Equals("X"))
                    {
                        value = bb.Min.X;
                    }
                    else if (axis.Equals("Y"))
                    {
                        value = bb.Min.Y;
                    }
                    else if (axis.Equals("Z"))
                    {
                        value = bb.Min.Z;
                    }
                }
                else if (valueDescriptor.Equals("Max"))
                {
                    if (axis.Equals("X"))
                    {
                        value = bb.Max.X;
                    }
                    else if (axis.Equals("Y"))
                    {
                        value = bb.Max.Y;
                    }
                    else if (axis.Equals("Z"))
                    {
                        value = bb.Max.Z;
                    }
                }
                else if (valueDescriptor.Equals("Mid"))
                {
                    if (axis.Equals("X"))
                    {
                        value = bb.Max.X - ((bb.Max.X - bb.Min.X)/2);
                    }
                    else if (axis.Equals("Y"))
                    {
                        value = bb.Max.Y - ((bb.Max.Y - bb.Min.Y) / 2);
                    }
                    else if (axis.Equals("Z"))
                    {
                        value = bb.Max.Z - ((bb.Max.Z - bb.Min.Z) / 2);
                    }
                }
                else
                {
                    break;
                }
                var translation = descriptor.Substring(8);
                translation = translation.Replace("(", "");
                translation = translation.Replace(")", "");
                if (!translation.Equals(""))
                {
                    var sign = translation.Substring(0, 1);
                    if (sign.Equals("+"))
                    {
                        value = value + (float)Double.Parse(translation.Substring(1));
                    }
                    else if (sign.Equals("-"))
                    {
                        value = value - (float)Double.Parse(translation.Substring(1));
                    }
                }
                values.Add(value);
            }
            // parse derived paramater values into list of points
            var clearanceBB = new Microsoft.Xna.Framework.BoundingBox(new Vector3(values[0], values[1], values[2]), new Vector3(values[3], values[4], values[5]));
            return EntikaInstance.GetBoundingBoxAsPoints(clearanceBB);
        }

        // Important method, returns an updated narrative time point. The narrative time point includes a list of shapes adjusted for the addition of the new shape
        public static NarrativeTimePoint AddShapeToTimePoint(NarrativeTimePoint ntp, NarrativeShape addition)
        {
            // First should always be ground/base shape
            NarrativeShape BaseShape = ntp.TimePointSpecificFill.NarrativeShapes[0];
            // Create list so that addition shape can be manipulated in each for loop.
            List<NarrativeShape> additionList = new List<NarrativeShape>();
            additionList.Add(addition);

            // Determine in which shapes the addition lies, loop through all shapes and determine the overlap
            foreach(NarrativeShape ns in ntp.TimePointSpecificFill.NarrativeShapes)
            {
                Polygon polygon = ns.Polygon;
                foreach (NarrativeShape add in additionList)
                {
                    // If it overlaps, the intersection should be removed from each shape
                    if (polygon.Overlaps(add.Polygon))
                    {
                        Polygon result = HelperFunctions.IntersectShapes(polygon, add.Polygon);
                        Polygon shapeResult = HelperFunctions.DifferenceShapes(polygon, result);
                        Polygon addResult = HelperFunctions.DifferenceShapes(add.Polygon, result);
                        // Retrieve all shapes from the results and add new shapes to the right lists, remove the adjusted shapes
                        // additionList.Add(new NarrativeShape(add.Name, add.Position, ));
                        // additionList.Remove(add);
                    }

                }
            }
            return ntp;
        }
    }
}
