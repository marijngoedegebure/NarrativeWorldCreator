using Common;
using Common.Geometry;
using Common.Shapes;
using Microsoft.Xna.Framework;
using Semantics.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using TriangleNet.Geometry;
using Semantics.Components;

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
            // Fetch relationships that this instance provides
            var AddedRelationsShapes = new List<NarrativeShape>();
            foreach (var relationAsSource in addition.TangibleObject.RelationshipsAsSource)
            {
                var relationType = relationAsSource.RelationshipType;
                GeometricRelationshipBase relation = null;
                NarrativeShape shape = null;
                // For each relationship, create shape, add this to narrativeshapes, and remove from baseshape
                // Kind of relationship:
                if (relationType.DefaultName.Equals("On"))
                {
                    shape = CreateOnRelationShape(addition);
                    relation = new GeometricRelationshipBase(GeometricRelationshipBase.RelationshipTypes.On);
                }
                else if (relationType.DefaultName.Equals("Against"))
                {
                    shape = CreateAgainstRelationShape(addition, relationAsSource, destinationShape.zpos);
                    relation = new GeometricRelationshipBase(GeometricRelationshipBase.RelationshipTypes.Against);
                }
                else if (relationType.DefaultName.Equals("Around"))
                {
                    shape = CreateAroundRelationShape(addition, relationAsSource, destinationShape.zpos);
                    relation = new GeometricRelationshipBase(GeometricRelationshipBase.RelationshipTypes.Around);
                }
                else if (relationType.DefaultName.Equals("Facing"))
                {
                    shape = CreateFacingRelationShape(addition, relationAsSource, destinationShape.zpos);
                    relation = new GeometricRelationshipBase(GeometricRelationshipBase.RelationshipTypes.Facing);
                }
                else if (relationType.DefaultName.Equals("Above"))
                {
                    shape = CreateAboveRelationShape(addition, relationAsSource);
                    relation = new GeometricRelationshipBase(GeometricRelationshipBase.RelationshipTypes.Above);
                }
                else if (relationType.DefaultName.Equals("Parallel"))
                {
                    shape = CreateParallelRelationShape(addition, relationAsSource, destinationShape.zpos);
                    relation = new GeometricRelationshipBase(GeometricRelationshipBase.RelationshipTypes.Parallel);
                }
                if (relation == null || shape == null)
                    continue;
                // Make sure that any shape created for a relation, does not exceed the region bounds
                shape.Polygon = HelperFunctions.IntersectShapes(shape.Polygon, new Polygon(ntp.Location.Shape.Points));
                relation.Source = addition;
                shape.Relations.Add(relation);
                addition.RelationshipsAsSource.Add(relation);
                // Add relation to fill
                ntp.TimePointSpecificFill.Relationships.Add(relation);
                AddedRelationsShapes.Add(shape);
            }

            // Remove offlimits polygon from baseshape
            // Go through list of shapes in reverse order so that it allows deletion of shapes
            foreach(var shape in ntp.TimePointSpecificFill.NarrativeShapes.Reverse<NarrativeShape>())
            {
                if (shape.zpos == addition.OffLimitsShape.zpos)
                {
                    var adjustedPolygon = HelperFunctions.DifferenceShapes(shape.Polygon, addition.OffLimitsShape.Polygon);
                    if (adjustedPolygon == null || adjustedPolygon.Area() < 0.5)
                    {
                        // If remaining polygon equals null, remove shape from list
                        ntp.TimePointSpecificFill.NarrativeShapes.Remove(shape);
                    }
                    shape.Polygon = adjustedPolygon;
                }
            }
            // Remove clearance from baseshape and add it to timepoint, if it exists
            foreach(var clearanceShape in addition.ClearanceShapes)
            {
                if (clearanceShape.zpos == addition.OffLimitsShape.zpos)
                {
                    foreach (var shape in ntp.TimePointSpecificFill.NarrativeShapes)
                    {
                        var adjustedPolygon = HelperFunctions.DifferenceShapes(shape.Polygon, clearanceShape.Polygon);
                        if (adjustedPolygon == null || adjustedPolygon.Area() < 0.5)
                        {
                            // If remaining polygon equals null, remove shape from list
                            ntp.TimePointSpecificFill.NarrativeShapes.Remove(shape);
                        }
                        shape.Polygon = adjustedPolygon;
                    }
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
                            if (differenceRelationAndShape == null || differenceRelationAndShape.Area() < 0.5)
                            {
                                // Dont add relational shape to shapes, update shape
                                shape.Polygon = differenceShapeAndRelation;
                                continue;
                            }
                            else if (differenceShapeAndRelation == null || differenceShapeAndRelation.Area() < 0.5)
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
            ntp.TimePointSpecificFill.OtherObjectInstances.Add(addition);
            return ntp;
        }

        private static NarrativeShape CreateFacingRelationShape(EntikaInstance addition, Relationship relationAsTarget, float zpos)
        {
            var maxRange = ((NumericalValueCondition)relationAsTarget.GetParameterValue("max range")).Value.Value;
            var polygon = CreatePolygonForBBAndRadius(addition, maxRange);
            return new NarrativeShape(zpos, polygon, NarrativeShape.ShapeType.Relationship, addition);
        }

        private static NarrativeShape CreateAroundRelationShape(EntikaInstance addition, Relationship relationAsTarget, float zpos)
        {
            var radius = ((NumericalValueCondition)relationAsTarget.GetParameterValue("radius")).Value.Value;
            var polygon = CreatePolygonForBBAndRadius(addition, radius);
            return new NarrativeShape(zpos, polygon, NarrativeShape.ShapeType.Relationship, addition);
        }

        private static NarrativeShape CreateAgainstRelationShape(EntikaInstance addition, Relationship relationAsTarget, float zpos)
        {
            var radius = ((NumericalValueCondition)relationAsTarget.GetParameterValue("radius")).Value.Value;

            // Remove center off limit shape from this relationshape as it is off limits
            var polygon = CreatePolygonForBBAndRadius(addition, radius);
            return new NarrativeShape(zpos, polygon, NarrativeShape.ShapeType.Relationship, addition);
        }

        private static NarrativeShape CreateParallelRelationShape(EntikaInstance addition, Relationship relationAsTarget, float zpos)
        {
            var maxRange = ((NumericalValueCondition)relationAsTarget.GetParameterValue("max range")).Value.Value;

            var polygon = CreatePolygonForBBAndRadius(addition, maxRange);
            return new NarrativeShape(zpos, polygon, NarrativeShape.ShapeType.Relationship, addition);
        }

        private static Polygon CreatePolygonForBBAndRadius(EntikaInstance addition, float radius)
        {
            // Z height should be the same as where the item is put on, 0 should later be translated into a proper z height
            Vector3[] corners = addition.BoundingBox.GetCorners();
            var XNApoints = new List<Vector2>();
            XNApoints.Add(new Vector2(corners[0].X, corners[0].Y));
            XNApoints.Add(new Vector2(corners[1].X, corners[1].Y));
            XNApoints.Add(new Vector2(corners[2].X, corners[2].Y));
            XNApoints.Add(new Vector2(corners[3].X, corners[3].Y));
            var scaledPoints = new List<Vector2>();
            // First translate to 0,0 scale and then translate back
            var translationMatrix = Matrix.CreateTranslation(addition.Position);
            var inverseTranslationMatrix = Matrix.CreateTranslation(-addition.Position);
            var scaleMatrix = Matrix.CreateScale(radius);
            foreach (var xnaPoint in XNApoints)
            {
                var translatedPoint = Vector2.Transform(xnaPoint, inverseTranslationMatrix);
                var scaledPoint = Vector2.Transform(translatedPoint, scaleMatrix);
                scaledPoints.Add(Vector2.Transform(scaledPoint, translationMatrix));
            }
            // Convert into polygon points
            List<Vec2> points = new List<Vec2>();
            foreach (var point in scaledPoints)
            {
                points.Add(new Vec2(point.X, point.Y));
            }
            return HelperFunctions.DifferenceShapes(new Polygon(points), addition.OffLimitsShape.Polygon);
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

        private static NarrativeShape CreateAboveRelationShape(EntikaInstance addition, Relationship relationAsTarget)
        {
            var height = ((NumericalValueCondition)relationAsTarget.GetParameterValue("height")).Value.Value;
            Vector3[] corners = addition.BoundingBox.GetCorners();
            // 8 corners, the last 4 are the top 4
            List<Vec2> points = new List<Vec2>();
            points.Add(new Vec2(corners[4].X, corners[4].Y));
            points.Add(new Vec2(corners[5].X, corners[5].Y));
            points.Add(new Vec2(corners[6].X, corners[6].Y));
            points.Add(new Vec2(corners[7].X, corners[7].Y));

            return new NarrativeShape(height, new Polygon(points), NarrativeShape.ShapeType.Relationship, addition);
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

        public static void RemoveEntikaInstanceFromTimePoint(NarrativeTimePoint ntp, EntikaInstance ei)
        {
            // Remove object from list of entika instances
            ntp.TimePointSpecificFill.OtherObjectInstances.Remove(ei);
            // Add shape back to shapes too which it belonged.
        }
    }
}
