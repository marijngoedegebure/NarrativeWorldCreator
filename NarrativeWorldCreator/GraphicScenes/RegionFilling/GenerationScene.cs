using Common;
using Common.Geometry;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using NarrativeWorldCreator.GraphicScenes.Primitives;
using NarrativeWorldCreator.Models;
using NarrativeWorldCreator.Models.NarrativeRegionFill;
using NarrativeWorldCreator.Models.NarrativeTime;
using NarrativeWorldCreator.Pages;
using NarrativeWorldCreator.Solvers;
using NarrativeWorldCreator.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TriangleNet.Data;

namespace NarrativeWorldCreator.GraphicScenes
{
    public class GenerationScene : RegionScene
    {
        #region Fields

        #endregion

        #region Methods
        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void Draw(GameTime time)
        {
            base.Draw(time);
            drawCentroidAndFocalPoint();
        }

        protected override void drawEntikaInstances()
        {
            foreach (EntikaInstance instance in _currentRegionPage.SelectedTimePoint.Configuration.GetEntikaInstancesWithoutFloor())
            {
                drawEntikaInstance2(instance);
            }
        }

        private void drawCentroidAndFocalPoint()
        {
            BasicEffect basicEffect = new BasicEffect(GraphicsDevice);

            basicEffect.World = this.world;
            basicEffect.View = this.view;
            basicEffect.Projection = this.proj;
            basicEffect.VertexColorEnabled = true;
            // When in placement mode, visualize focal and centroid
            Vector3 centroid = new Vector3((float)SystemStateTracker.centroidX, (float)SystemStateTracker.centroidY, 0);

            // Create square based on centroid coordinates
            Quad quad = new Quad(centroid, new Vector3(centroid.X, centroid.Y, 1), Vector3.Up,(float) (0.005 * cam._pos.Z), (float)(0.005 * cam._pos.Z), Color.Blue);

            // Draw centroid
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleStrip,
                    quad.Vertices,
                    0,
                    2);
            }

            // Draw focal
            Vector3 focalPoint = new Vector3((float)SystemStateTracker.focalX, (float)SystemStateTracker.focalY, 0);
            Vector3 focalRotation = new Vector3(0.0f, (float)SystemStateTracker.focalRot, 0.0f);

            // Draw rotation indicator
            var rotMatrix = Matrix.CreateRotationZ(focalRotation.Y);

            VertexPositionColor[] vertices = new VertexPositionColor[3];
            vertices[0] = new VertexPositionColor(Vector3.Transform(new Vector3(0, (float)(0.015 * cam._pos.Z), 0), rotMatrix) + focalPoint, Color.DarkRed);
            vertices[1] = new VertexPositionColor(Vector3.Transform(new Vector3((float)(0.0075 * cam._pos.Z), 0, 0), rotMatrix) + focalPoint, Color.DarkRed);
            vertices[2] = new VertexPositionColor(Vector3.Transform(new Vector3(-(float)(0.0075 * cam._pos.Z), 0, 0), rotMatrix) + focalPoint, Color.DarkRed);
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleList,
                    vertices,
                    0,
                    1);
            }

            // Draw basic point
            Vector3 focalLinePointEnd = new Vector3(focalPoint.X + 2.0f, focalPoint.Y + 2.0f, 0);
            focalLinePointEnd = Vector3.Transform(focalLinePointEnd, Matrix.CreateRotationY(focalRotation.Y));

            Quad quad2 = new Quad(focalPoint, new Vector3(focalPoint.X, focalPoint.Y, 1), Vector3.Up, (float)(0.005 * cam._pos.Z), (float)(0.005 * cam._pos.Z), Color.Red);
            var temp = CreateLine(focalPoint, focalLinePointEnd, Color.Red);
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                    PrimitiveType.TriangleStrip,
                    quad2.Vertices,
                    0,
                    2);
            }
        }

        
        protected override void drawGPUResultInstance(GPUInstanceResult gpuResultInstance)
        {
            if (gpuResultInstance.entikaInstance.Name != Constants.Floor)
                drawEntikaInstance(gpuResultInstance.entikaInstance, gpuResultInstance.Position, gpuResultInstance.Rotation);
        }

        protected override void Update(GameTime time)
        {
            base.Update(time);
        }
        #endregion
    }
}