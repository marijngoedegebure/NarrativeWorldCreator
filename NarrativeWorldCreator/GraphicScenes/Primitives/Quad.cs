#region File Description
//-----------------------------------------------------------------------------
// Quad.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace NarrativeWorldCreator
{
    public struct Quad
    {
        public Vector3 Origin;
        public Vector3 UpperLeft;
        public Vector3 LowerLeft;
        public Vector3 UpperRight;
        public Vector3 LowerRight;
        public Vector3 Up;
        public Vector3 Left;

        public VertexPositionColor[] Vertices;
        //        public int[] Indexes;
        public short[] Indexes;


        public Quad(Vector3 origin, Vector3 normal, Vector3 up,
            float width, float height)
        {
            Vertices = new VertexPositionColor[4];
            Indexes = new short[6];
            Origin = origin;
            Up = up;

            // Calculate the quad corners
            Left = Vector3.Cross(normal, Up);
            Vector3 uppercenter = (Up * height / 2) + origin;
            UpperLeft = new Vector3(origin.X - width, origin.Y + height, origin.Z);
            UpperRight = new Vector3(origin.X + width, origin.Y + height, origin.Z);
            LowerLeft = new Vector3(origin.X - width, origin.Y - height, origin.Z);
            LowerRight = new Vector3(origin.X + width, origin.Y - height, origin.Z);
            //UpperLeft = uppercenter + (Left * width / 2);
            //UpperRight = uppercenter - (Left * width / 2);
            //LowerLeft = UpperLeft - (Up * height);
            //LowerRight = UpperRight - (Up * height);

            FillVertices();
        }

        private void FillVertices()
        {
            // Fill in texture coordinates to display full texture
            // on quad
            Vector2 textureUpperLeft = new Vector2(0.0f, 0.0f);
            Vector2 textureUpperRight = new Vector2(1.0f, 0.0f);
            Vector2 textureLowerLeft = new Vector2(0.0f, 1.0f);
            Vector2 textureLowerRight = new Vector2(1.0f, 1.0f);

            // Set the position and texture coordinate for each
            // vertex
            Vertices[0].Position = LowerLeft;
            Vertices[0].Color = Color.Red;
            Vertices[1].Position = UpperLeft;
            Vertices[1].Color = Color.Red;
            Vertices[2].Position = LowerRight;
            Vertices[2].Color = Color.Red;
            Vertices[3].Position = UpperRight;
            Vertices[3].Color = Color.Red;

            // Set the index buffer for each vertex, using
            // clockwise winding
            Indexes[0] = 0;
            Indexes[1] = 1;
            Indexes[2] = 2;
            Indexes[3] = 2;
            Indexes[4] = 1;
            Indexes[5] = 3;
        }
    }
}
