using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NarrativeWorldCreator.Models;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.GraphicScenes
{
    public class DebugScene : RegionScene
    {
        #region Draw
        protected override void Draw(GameTime time)
        {
            base.Draw(time);
            drawBoxSelect();
        }

        protected void drawBoxSelect()
        {
            if (!BoxSelectInitialCoords.Equals(new Point()))
            {
                GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = false };
                _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                _spriteBatch.Draw(SystemStateTracker.BoxSelectTexture, new Rectangle(BoxSelectInitialCoords.X, BoxSelectInitialCoords.Y, BoxSelectCurrentCoords.X - BoxSelectInitialCoords.X, BoxSelectCurrentCoords.Y - BoxSelectInitialCoords.Y), Color.White * 0.5f);

                _spriteBatch.End();
                GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            }
        }
        #endregion

        #region Update
        protected override void Update(GameTime time)
        {
            base.Update(time);
            HandleRegionFilling();
        }


        protected void HandleRegionFilling()
        {
            // Handle object selection
            if (CurrentRegionFillingMode == RegionFillingModes.None)
            {
                if (_keyboardState.IsKeyDown(Keys.LeftShift))
                {
                    if (_previousMouseState.LeftButton == ButtonState.Released && _mouseState.LeftButton == ButtonState.Pressed)
                    {
                        // Initialize box
                        BoxSelectInitialCoords = _mouseState.Position;
                        BoxSelectCurrentCoords = _mouseState.Position;
                    }
                    if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Pressed)
                    {
                        // reposition box to draw
                        BoxSelectCurrentCoords = _mouseState.Position;
                    }
                    if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released)
                    {
                        var initialCoordsHit = CalculateHitOnSurface(BoxSelectInitialCoords, new Microsoft.Xna.Framework.Plane(new Vector3(0, 0, 1), 0));
                        var currentCoordsHit = CalculateHitOnSurface(BoxSelectCurrentCoords, new Microsoft.Xna.Framework.Plane(new Vector3(0, 0, 1), 0));

                        Vector3 bottomLeft = new Vector3(0, 0, -20.0f);
                        Vector3 topRight = new Vector3(0, 0, 200f);
                        // Determine topLeft and bottomRight of rectangle
                        if (initialCoordsHit.X < currentCoordsHit.X)
                        {
                            bottomLeft.X = initialCoordsHit.X;
                            topRight.X = currentCoordsHit.X;
                        }
                        else
                        {
                            bottomLeft.X = currentCoordsHit.X;
                            topRight.X = initialCoordsHit.X;
                        }

                        if (initialCoordsHit.Y < currentCoordsHit.Y)
                        {
                            bottomLeft.Y = initialCoordsHit.Y;
                            topRight.Y = currentCoordsHit.Y;
                        }
                        else
                        {
                            bottomLeft.Y = currentCoordsHit.Y;
                            topRight.Y = initialCoordsHit.Y;
                        }
                        var selectionBoxBB = new BoundingBox(bottomLeft, topRight);

                        foreach (EntikaInstance ieo in _currentRegionPage.Configuration.GetEntikaInstancesWithoutFloor())
                        {
                            // Create translated boundingbox
                            var containmentType = selectionBoxBB.Contains(ieo.BoundingBox);
                            if (!(containmentType == ContainmentType.Disjoint))
                                this._currentRegionPage.ChangeSelectedObject(ieo);
                        }
                        this._currentRegionPage.RefreshSelectedObjectView();

                        // Figure out selection of objects inside box
                        BoxSelectCurrentCoords = new Point();
                        BoxSelectInitialCoords = new Point();
                    }
                }
                else if (_keyboardState.IsKeyDown(Keys.R))
                {
                    if (_previousMouseState.LeftButton == ButtonState.Released && _mouseState.LeftButton == ButtonState.Pressed)
                    {
                        // Initialize box
                        Ray ray = CalculateMouseRay();
                        foreach (EntikaInstance ieo in _currentRegionPage.Configuration.GetEntikaInstancesWithoutFloor())
                        {
                            // Create translated boundingbox
                            var min = ieo.BoundingBox.Min;
                            var max = ieo.BoundingBox.Max;

                            var bb = new BoundingBox(Vector3.Transform(min, Matrix.CreateTranslation(ieo.Position)), Vector3.Transform(max, Matrix.CreateTranslation(ieo.Position)));

                            // Intersect ray with bounding box, if distance then select model
                            float? distance = ray.Intersects(bb);
                            if (distance != null)
                            {
                                rotationObject = ieo;
                                var hit = CalculateHitOnSurface(_mouseState.Position, new Microsoft.Xna.Framework.Plane(new Vector3(0, 0, 1), 0));
                                // Move hit around 0,0,0
                                hit = hit - rotationObject.Position;
                                hit.Normalize();
                                // Calculate angle between forward vector (Y positive) and the hit)
                                var angle = 0.0;
                                if ((hit.X > 0 && hit.Y > 0) || (hit.X > 0 && hit.Y < 0))
                                {
                                    angle = Math.Acos(Vector3.Dot(new Vector3(0, -1, 0), hit));
                                    angle += Math.PI;
                                }
                                else
                                {
                                    angle = Math.Acos(Vector3.Dot(new Vector3(0, 1, 0), hit));
                                }
                                var delta = rotationObject.Rotation.Y - angle;
                                rotationObject.Rotation = new Vector3(0, (float)angle, 0);
                                rotationObject.UpdateBoundingBoxAndShape();
                                CascadeRotation(rotationObject, delta);
                            }
                        }
                    }
                    if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Pressed && rotationObject != null)
                    {
                        // reposition box to draw
                        var hit = CalculateHitOnSurface(_mouseState.Position, new Microsoft.Xna.Framework.Plane(new Vector3(0, 0, 1), 0));
                        // Move hit around 0,0,0
                        hit = hit - rotationObject.Position;
                        hit.Normalize();
                        // Calculate angle between forward vector (Y positive) and the hit)
                        var angle = 0.0;
                        if ((hit.X > 0 && hit.Y > 0) || (hit.X > 0 && hit.Y < 0))
                        {
                            angle = Math.Acos(Vector3.Dot(new Vector3(0, -1, 0), hit));
                            angle += Math.PI;
                        }
                        else
                        {
                            angle = Math.Acos(Vector3.Dot(new Vector3(0, 1, 0), hit));
                        }
                        var delta = rotationObject.Rotation.Y - angle;
                        rotationObject.Rotation = new Vector3(0, (float)angle, 0);
                        CascadeRotation(rotationObject, delta);
                    }
                    if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released && rotationObject != null)
                    {
                        rotationObject = null;
                    }
                }

                else if (_keyboardState.IsKeyDown(Keys.LeftControl))
                {
                    if (_previousMouseState.LeftButton == ButtonState.Released && _mouseState.LeftButton == ButtonState.Pressed)
                    {
                        // Initialize box
                        Ray ray = CalculateMouseRay();
                        foreach (EntikaInstance ieo in _currentRegionPage.Configuration.GetEntikaInstancesWithoutFloor())
                        {
                            // Create translated boundingbox
                            var min = ieo.BoundingBox.Min;
                            var max = ieo.BoundingBox.Max;

                            var bb = new BoundingBox(Vector3.Transform(min, Matrix.CreateTranslation(ieo.Position)), Vector3.Transform(max, Matrix.CreateTranslation(ieo.Position)));

                            // Intersect ray with bounding box, if distance then select model
                            float? distance = ray.Intersects(bb);
                            if (distance != null)
                            {
                                repositioningObject = ieo;
                                var hit = CalculateHitOnSurface(_mouseState.Position, new Microsoft.Xna.Framework.Plane(new Vector3(0, 0, 1), 0));
                                var newPos = new Vector3(hit.X, hit.Y, repositioningObject.Position.Z);
                                var delta = newPos - repositioningObject.Position;
                                repositioningObject.Position = newPos;
                                CascadeRepositioning(repositioningObject, delta);
                            }
                        }
                    }
                    if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Pressed && repositioningObject != null)
                    {
                        // reposition box to draw
                        var hit = CalculateHitOnSurface(_mouseState.Position, new Microsoft.Xna.Framework.Plane(new Vector3(0, 0, 1), 0));
                        var newPos = new Vector3(hit.X, hit.Y, repositioningObject.Position.Z);
                        var delta = newPos - repositioningObject.Position;
                        repositioningObject.Position = newPos;
                        CascadeRepositioning(repositioningObject, delta);
                    }
                    if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released && repositioningObject != null)
                    {
                        repositioningObject = null;
                    }
                }
                else
                {
                    if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released)
                    {
                        Ray ray = CalculateMouseRay();
                        EntikaInstance hitMaxZ = null;
                        foreach (EntikaInstance ieo in _currentRegionPage.Configuration.GetEntikaInstancesWithoutFloor())
                        {
                            // Create translated boundingbox
                            var min = ieo.BoundingBox.Min;
                            var max = ieo.BoundingBox.Max;
                            // Check whether 

                            var bb = new BoundingBox(min, max);

                            // Intersect ray with bounding box, if distance then select model
                            float? distance = ray.Intersects(bb);
                            if (distance != null)
                            {
                                if (hitMaxZ == null)
                                {
                                    hitMaxZ = ieo;
                                    continue;
                                }
                                if (hitMaxZ.Position.Z < ieo.Position.Z)
                                    hitMaxZ = ieo;
                            }
                        }
                        if (hitMaxZ != null)
                            _currentRegionPage.ChangeSelectedObject(hitMaxZ);
                    }
                }
                //if (_currentRegionPage.SelectedEntikaInstance != null && _keyboardState.IsKeyUp(Keys.Delete) && _previousKeyboardState.IsKeyDown(Keys.Delete))
                //{
                //}
            }
        }

        protected void CascadeRotation(EntikaInstance parentObj, double delta)
        {
            foreach (var relationship in parentObj.RelationshipsAsSource.Where(rel => rel.BaseRelationship.RelationshipType.DefaultName.Equals(Constants.On)))
            {
                // Translate object using parent object position, rotate and translate back
                relationship.Target.Position -= parentObj.Position;
                relationship.Target.Position = Vector3.Transform(relationship.Target.Position, Matrix.CreateRotationZ(-(float)delta));
                relationship.Target.Position += parentObj.Position;
                relationship.Target.Rotation = new Vector3(relationship.Target.Rotation.X, (float)((relationship.Target.Rotation.Y - (float)delta) % Math.PI), relationship.Target.Rotation.Z);
                relationship.Target.UpdateBoundingBoxAndShape();
                // fire of CascadeRepositioning again
                CascadeRotation(relationship.Target, delta);
            }
        }

        protected void CascadeRepositioning(EntikaInstance parentObj, Vector3 delta)
        {
            foreach (var relationship in parentObj.RelationshipsAsSource.Where(rel => rel.BaseRelationship.RelationshipType.DefaultName.Equals(Constants.On)))
            {
                // For each source on relationship, move target object and fire of CascadeRepositioning again
                relationship.Target.Position += delta;
                relationship.Target.UpdateBoundingBoxAndShape();
                CascadeRepositioning(relationship.Target, delta);
            }
        }

        protected Vector3 CalculateHitOnSurface(Point p, Microsoft.Xna.Framework.Plane plane)
        {
            Ray ray = CalculateRay(p);
            float? distance = ray.Intersects(plane);
            return ray.Position + ray.Direction * distance.Value;
        }

        protected Vector3 CalculateMouseHitOnSurface()
        {
            return CalculateHitOnSurface(_mouseState.Position, new Microsoft.Xna.Framework.Plane(new Vector3(0, 0, 1), 0));
        }

        protected Ray CalculateRay(Point p)
        {
            Vector3 nearsource = new Vector3((float)p.X, (float)p.Y, 0f);
            Vector3 farsource = new Vector3((float)p.X, (float)p.Y, 1f);

            Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(nearsource, this.proj, this.view, this.world);
            Vector3 farPoint = GraphicsDevice.Viewport.Unproject(farsource, this.proj, this.view, this.world);

            // Create ray using far and near point
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            return new Ray(nearPoint, direction);
        }

        protected Ray CalculateMouseRay()
        {
            return CalculateRay(_mouseState.Position);
        }

        protected int CalculateCollisionQuad()
        {
            Ray ray = CalculateMouseRay();
            var floorInstance = this._currentRegionPage.Configuration.InstancedObjects.Where(io => io.Name.Equals(Constants.Floor)).FirstOrDefault();
            var result = HelperFunctions.GetMeshForPolygon(floorInstance.Polygon);
            List<VertexPositionColor> points = new List<VertexPositionColor>();
            points = DrawingEngine.GetDrawableTriangles(result, Color.White);
            for (int i = 0; i < points.Count; i++)
            {
                Quad quad = new Quad(points[i].Position, new Vector3(points[i].Position.X, points[i].Position.Y, 1), Vector3.Up, 1, 1, Color.Red);
                BoundingBox box = new BoundingBox(new Vector3(points[i].Position.X - 1, points[i].Position.Y - 1, points[i].Position.Z), new Vector3(points[i].Position.X + 1, points[i].Position.Y + 1, points[i].Position.Z));
                float? distance = ray.Intersects(box);
                if (distance != null)
                    return i;
            }
            return -1;
        }

        #endregion
    }
}
