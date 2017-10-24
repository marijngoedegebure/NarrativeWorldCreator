using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NarrativeWorldCreator.Models;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.GraphicScenes
{
    public class MainRegionScene : RegionScene
    {
        protected override void Draw(GameTime time)
        {
            base.Draw(time);
            var blend = GraphicsDevice.BlendState;
            var depth = GraphicsDevice.DepthStencilState;
            var raster = GraphicsDevice.RasterizerState;
            var sampler = GraphicsDevice.SamplerStates[0];

            if (this._currentRegionPage.CurrentFillingMode == ModeBaseRegionPage.MainFillingMode.Removal ||
                this._currentRegionPage.CurrentFillingMode == ModeBaseRegionPage.MainFillingMode.ManualChange ||
                this._currentRegionPage.CurrentFillingMode == ModeBaseRegionPage.MainFillingMode.Freeze)
                drawBoxSelect();

            if (this._currentRegionPage.CurrentFillingMode == ModeBaseRegionPage.MainFillingMode.ManualPlacement)
                drawPlacementInstance();

            GraphicsDevice.BlendState = blend;
            GraphicsDevice.DepthStencilState = depth;
            GraphicsDevice.RasterizerState = raster;
            GraphicsDevice.SamplerStates[0] = sampler;
        }

        private void drawPlacementInstance()
        {
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            var position = new Vector3();
            var rotation = new Vector3();
            if (this._currentRegionPage.manualPlacementPosition.Equals(new Vector3()))
            {
                // Calculate 3D point of mouse
                position = CalculateMouseHitOnSurface();
            }
            else
            {
                position = this._currentRegionPage.manualPlacementPosition;
            }
            if (!this._currentRegionPage.manualPlacementRotation.Equals(new Vector3()))
            {
                rotation = this._currentRegionPage.manualPlacementRotation;
            }
            drawEntikaInstance(this._currentRegionPage.InstanceOfObjectToAdd, position, rotation);
        }

        protected void drawBoxSelect()
        {
            if (!BoxSelectInitialCoords.Equals(new Point()))
            {
                GraphicsDevice.RasterizerState = RasterizerState.CullNone;
                GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = false };

                // Determine points using current and initial coords:
                int topLeftX = 0;
                int topLeftY = 0;
                int width = 0;
                int height = 0;
                if (BoxSelectInitialCoords.X > BoxSelectCurrentCoords.X)
                {
                    topLeftX = BoxSelectCurrentCoords.X;
                    width = BoxSelectInitialCoords.X - BoxSelectCurrentCoords.X;
                }
                else
                {
                    topLeftX = BoxSelectInitialCoords.X;
                    width = BoxSelectCurrentCoords.X - BoxSelectInitialCoords.X;
                }
                if (BoxSelectInitialCoords.Y > BoxSelectCurrentCoords.Y)
                {
                    topLeftY = BoxSelectCurrentCoords.Y;
                    height = BoxSelectInitialCoords.Y - BoxSelectCurrentCoords.Y;
                }
                else
                {
                    topLeftY = BoxSelectInitialCoords.Y;
                    height = BoxSelectCurrentCoords.Y - BoxSelectInitialCoords.Y;
                }

                _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                _spriteBatch.Draw(SystemStateTracker.BoxSelectTexture, new Rectangle(topLeftX, topLeftY, width, height), Color.Black * 0.5f);

                _spriteBatch.End();
                GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
                GraphicsDevice.RasterizerState = RasterizerState.CullClockwise;
            }
        }

        protected override void Update(GameTime time)
        {
            base.Update(time);
            // Manual placement
            if (this._currentRegionPage.CurrentFillingMode == ModeBaseRegionPage.MainFillingMode.ManualPlacement)
            {
                if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released)
                {
                    var mouseHitOnSurface = CalculateMouseHitOnSurface();
                    this._currentRegionPage.manualPlacementPosition = new Vector3(mouseHitOnSurface.X, mouseHitOnSurface.Y, this._currentRegionPage.InstanceOfObjectToAdd.Position.Z);                    
                }

                else if (_keyboardState.IsKeyDown(Keys.R))
                {
                    // Save rotation
                    // Handle continuous update of rotation
                    var hit = CalculateHitOnSurface(_mouseState.Position, new Microsoft.Xna.Framework.Plane(new Vector3(0, 0, 1), _currentRegionPage.manualPlacementPosition.Z));
                    // Move hit around 0,0,0
                    hit = hit - _currentRegionPage.manualPlacementPosition;
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
                    var delta = _currentRegionPage.manualPlacementRotation.Y - angle;
                    _currentRegionPage.manualPlacementRotation = new Vector3(0, (float)angle, 0);
                }
                else if (_previousKeyboardState.IsKeyDown(Keys.R) && _keyboardState.IsKeyUp(Keys.R))
                {
                    foreach (var instance in this._currentRegionPage.SelectedEntikaInstances)
                    {
                        _currentRegionPage.InstanceOfObjectToAdd.UpdateBoundingBoxAndShape();
                    }
                }
            }

            // Removal or freezing
            if (this._currentRegionPage.CurrentFillingMode == ModeBaseRegionPage.MainFillingMode.Removal || this._currentRegionPage.CurrentFillingMode == ModeBaseRegionPage.MainFillingMode.Freeze)
            {
                // Box select
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
                        this._currentRegionPage.RefreshViewsUsingSelected();

                        // Figure out selection of objects inside box
                        BoxSelectCurrentCoords = new Point();
                        BoxSelectInitialCoords = new Point();
                    }
                }
                // Single selection
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
                        _currentRegionPage.RefreshViewsUsingSelected();
                    }
                }
            }
            // Manual changing
            if (this._currentRegionPage.CurrentFillingMode == ModeBaseRegionPage.MainFillingMode.ManualChange)
            {
                // Box select
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
                        this._currentRegionPage.RefreshViewsUsingSelected();

                        // Figure out selection of objects inside box
                        BoxSelectCurrentCoords = new Point();
                        BoxSelectInitialCoords = new Point();
                    }
                }
                else
                {
                    if (_previousKeyboardState.IsKeyDown(Keys.R) && _keyboardState.IsKeyDown(Keys.R))
                    {
                        // When multiple selected rotate around 0,0,0
                        var rotationPos = new Vector3(0, 0, 0);
                        if (this._currentRegionPage.SelectedEntikaInstances.Count == 1)
                        {
                            rotationPos = this._currentRegionPage.SelectedEntikaInstances[0].Position;
                        }

                        // Save rotation
                        // Handle continuous update of rotation
                        var hit = CalculateHitOnSurface(_mouseState.Position, new Microsoft.Xna.Framework.Plane(new Vector3(0, 0, 1), 0));
                        // Move hit around 0,0,0
                        hit = hit - rotationPos;
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
                        var delta = _currentRegionPage.manualPlacementRotation.Y - angle;
                        foreach (var instance in this._currentRegionPage.SelectedEntikaInstances)
                        {
                            instance.Rotation = new Vector3(0, (float)angle, 0);
                            // CascadeRotation(instance, delta);
                        }
                    }
                    else if (_previousKeyboardState.IsKeyDown(Keys.R) && _keyboardState.IsKeyUp(Keys.R))
                    {
                        // Apply changes in selected entika instances to delta's and updateconfiguration
                        foreach (var instance in this._currentRegionPage.SelectedEntikaInstances)
                        {
                            var additionDeltaOfInstance = this._currentRegionPage.selectedNode.TimePoints[this._currentRegionPage.SelectedTimePoint].InstanceDeltas.Where(id => id.DT.Equals(InstanceDeltaType.Add) && id.RelatedInstance.Equals(instance)).FirstOrDefault();
                            if (additionDeltaOfInstance == null)
                            {
                                this._currentRegionPage.selectedNode.TimePoints[this._currentRegionPage.SelectedTimePoint].InstanceDeltas.Add(new InstanceDelta(this._currentRegionPage.SelectedTimePoint, instance, InstanceDeltaType.Change, instance.Position, instance.Rotation));
                            }
                            else
                            {
                                additionDeltaOfInstance.Position = instance.Position;
                                additionDeltaOfInstance.Rotation = instance.Rotation;
                            }
                        }
                        

                        this._currentRegionPage.UpdateConfiguration();
                    }
                    // Move
                    else if (_keyboardState.IsKeyDown(Keys.M))
                    {
                        if (_previousMouseState.LeftButton == ButtonState.Released && _mouseState.LeftButton == ButtonState.Pressed)
                        {
                            Ray ray = CalculateMouseRay();
                            foreach (EntikaInstance ieo in _currentRegionPage.SelectedEntikaInstances)
                            {
                                // Create translated boundingbox
                                var min = ieo.BoundingBox.Min;
                                var max = ieo.BoundingBox.Max;
                                var bb = new BoundingBox(min, max);

                                // Intersect ray with bounding box, if distance then select model
                                float? distance = ray.Intersects(bb);
                                if (distance != null)
                                {
                                    repositioningObject = ieo;
                                }
                            }
                        }
                        if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Pressed && repositioningObject != null)
                        {
                            // reposition box to draw
                            var hit = CalculateHitOnSurface(_mouseState.Position, new Microsoft.Xna.Framework.Plane(new Vector3(0, 0, 1), 0));
                            var newPos = new Vector3(hit.X, hit.Y, repositioningObject.Position.Z);
                            var delta = newPos - repositioningObject.Position;
                            foreach (var instance in this._currentRegionPage.SelectedEntikaInstances)
                            {
                                instance.Position = instance.Position + delta;
                                // CascadeRepositioning(repositioningObject, delta);
                            }
                        }
                        if (_previousMouseState.LeftButton == ButtonState.Pressed && _mouseState.LeftButton == ButtonState.Released && repositioningObject != null)
                        {
                            foreach (var instance in this._currentRegionPage.SelectedEntikaInstances)
                            {
                                instance.UpdateBoundingBoxAndShape();
                            }
                            repositioningObject = null;
                        }
                    }
                    else if (_previousKeyboardState.IsKeyDown(Keys.M) && _keyboardState.IsKeyUp(Keys.M))
                    {
                        // Apply changes in selected entika instances to delta's and updateconfiguration
                        foreach (var instance in this._currentRegionPage.SelectedEntikaInstances)
                        {
                            var additionDeltaOfInstance = this._currentRegionPage.selectedNode.TimePoints[this._currentRegionPage.SelectedTimePoint].InstanceDeltas.Where(id => id.DT.Equals(InstanceDeltaType.Add) && id.RelatedInstance.Equals(instance)).FirstOrDefault();
                            if (additionDeltaOfInstance == null)
                            {
                                this._currentRegionPage.selectedNode.TimePoints[this._currentRegionPage.SelectedTimePoint].InstanceDeltas.Add(new InstanceDelta(this._currentRegionPage.SelectedTimePoint, instance, InstanceDeltaType.Change, instance.Position, instance.Rotation));
                            }
                            else
                            {
                                additionDeltaOfInstance.Position = instance.Position;
                                additionDeltaOfInstance.Rotation = instance.Rotation;
                            }
                        }


                        this._currentRegionPage.UpdateConfiguration();
                        repositioningObject = null;
                    }
                    else
                    {
                        // Single selection
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
                            _currentRegionPage.RefreshViewsUsingSelected();
                        }
                        
                    }
                }
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
            List<VertexPositionColor> points = new List<VertexPositionColor>();
            points = DrawingEngine.GetDrawableTriangles(floorInstance.Polygon.GetAllVertices(), Color.White);
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

    }
}
