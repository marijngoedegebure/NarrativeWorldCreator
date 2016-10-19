using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    [Serializable()]
    public class CameraSetting
    {
        double yaw, pitch, roll;
        Vec3 position;

        public double Yaw
        {
            get { return yaw; }
            set { yaw = value; }
        }

        public double Pitch
        {
            get { return pitch; }
            set { pitch = value; }
        }

        public double Roll
        {
            get { return roll; }
            set { roll = value; }
        }

        public Vec3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public CameraSetting(double yaw, double pitch, double roll, Vec3 position)
        {
            this.yaw = yaw;
            this.pitch = pitch;
            this.roll = roll;
            this.position = position;
        }
    }
}
