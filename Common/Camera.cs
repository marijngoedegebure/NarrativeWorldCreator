using System;
using System.Collections.Generic;

using System.Text;

namespace Common
{
    public class Camera
    {
        static Dictionary<string, Camera> cameras = new Dictionary<string, Camera>();

        Vec3 pos = new Vec3(50, 25, 50), dir = new Vec3(-1, -0.5f, -1);
        public Vec3 Eye { get { return pos; } set { pos = new Vec3(value); } }
        public Vec3 Target { get { return pos + dir; } }
        public Vec3 Dir { get { return dir; } set { dir = value.normalize(); } }

        public Camera()
        {
            dir.Normalize();
        }

        public static Camera GetCamera(string name)
        {
            if (cameras.ContainsKey(name))
                return cameras[name];
            Camera c = new Camera();
            cameras.Add(name, c);
            return c;
        }

        public void GoForward(float dist)
        {
            pos += dist * dir;
        }

        public void GoBackward(float dist)
        {
            pos -= dist * dir;
        }

        public void GoRight(float dist)
        {
            Vec3 cross = dir.Cross(new Vec3(0, 1, 0));
            cross.Normalize();
            pos += dist * cross;
        }

        public void GoLeft(float dist)
        {
            Vec3 cross = dir.Cross(new Vec3(0, 1, 0));
            cross.Normalize();
            pos -= dist * cross;
        }

        public void RotateYaw(float angle)
        {
            Vec2 flat = new Vec2(dir.X, dir.Z);
            float len = flat.length();
            float ang = flat.GetAngle();
            ang += angle;
            dir.X = len * (float)Math.Cos(ang);
            dir.Z = len * (float)Math.Sin(ang);
        }

        public void RotatePitch(float angle)
        {
            float pitch = (float)Math.Asin(dir.Y);
            Vec2 flat = new Vec2(dir.X, dir.Z);
            float ang = flat.GetAngle();
            pitch += angle;
            float len = (float)Math.Cos(pitch);
            dir.Y = (float)Math.Sin(pitch);
            dir.X = len * (float)Math.Cos(ang);
            dir.Z = len * (float)Math.Sin(ang);
        }
    }
}
