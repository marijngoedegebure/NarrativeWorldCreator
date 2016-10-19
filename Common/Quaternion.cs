/**************************************************************************
 * 
 * Quaternion.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2010-2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 * 
 * http://www.koders.com/csharp/fid3746F0DC8996D8D8E41ADADD796E5D3B907AA0F6.aspx
 *
 *************************************************************************/

using System;

namespace Common
{

    #region Class: Quaternion
    /// <summary>
    /// A quaternion.
    /// </summary>
    public class Quaternion
    {
        public delegate void ValueChangedHandler(Quaternion sender);
        public event ValueChangedHandler ValueChanged;

        #region Properties and Fields

        #region Property: W
        /// <summary>
        /// The W value.
        /// </summary>
        private float w = 1.0f;

        /// <summary>
        /// Gets or sets the W value.
        /// </summary>
        public float W
        {
            get
            {
                return w;
            }
            set
            {
                w = value;
                if (ValueChanged != null)
                    ValueChanged.Invoke(this);
            }
        }
        #endregion Property: W

        #region Property: X
        /// <summary>
        /// The X value.
        /// </summary>
        private float x = 0.0f;

        /// <summary>
        /// Gets or sets the X value.
        /// </summary>
        public float X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
                if (ValueChanged != null)
                    ValueChanged.Invoke(this);
            }
        }
        #endregion Property: X

        #region Property: Y
        /// <summary>
        /// The Y value.
        /// </summary>
        private float y = 0.0f;

        /// <summary>
        /// Gets or sets the Y value.
        /// </summary>
        public float Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
                if (ValueChanged != null)
                    ValueChanged.Invoke(this);
            }
        }
        #endregion Property: Y

        #region Property: Z
        /// <summary>
        /// The Z value.
        /// </summary>
        private float z = 0.0f;

        /// <summary>
        /// Gets or sets the Z value.
        /// </summary>
        public float Z
        {
            get
            {
                return z;
            }
            set
            {
                z = value;
                if (ValueChanged != null)
                    ValueChanged.Invoke(this);
            }
        }
        #endregion Property: Z

        #region Property: Identity
        /// <summary>
        /// Gets the identity quaternion.
        /// </summary>
        public static Quaternion Identity
        {
            get
            {
                return new Quaternion(1.0f, 0.0f, 0.0f, 0.0f);
            }
        }
        #endregion Property: Identity

        #region Property: Zero
        /// <summary>
        /// Gets a quaternion with zeroes.
        /// </summary>
        public static Quaternion Zero
        {
            get
            {
                return new Quaternion(0.0f, 0.0f, 0.0f, 0.0f);
            }
        }
        #endregion Property: Zero

        #region Property: Norm
        /// <summary>
        /// Gets the squared 'length' of this quaternion.
        /// </summary>
        public float Norm
        {
            get
            {
                return x * x + y * y + z * z + w * w;
            }
        }
        #endregion Property: Norm

        #region Property: XAxis
        /// <summary>
        /// Gets the local X-axis portion of this rotation.
        /// </summary>
        public Vec3 XAxis
        {
            get
            {
                float fTx = 2.0f * x;
                float fTy = 2.0f * y;
                float fTz = 2.0f * z;
                float fTwy = fTy * w;
                float fTwz = fTz * w;
                float fTxy = fTy * x;
                float fTxz = fTz * x;
                float fTyy = fTy * y;
                float fTzz = fTz * z;

                return new Vec3(1.0f - (fTyy + fTzz), fTxy + fTwz, fTxz - fTwy);
            }
        }
        #endregion Property: XAxis

        #region Property: YAxis
        /// <summary>
        /// Gets the local Y-axis portion of this rotation.
        /// </summary>
        public Vec3 YAxis
        {
            get
            {
                float fTx = 2.0f * x;
                float fTy = 2.0f * y;
                float fTz = 2.0f * z;
                float fTwx = fTx * w;
                float fTwz = fTz * w;
                float fTxx = fTx * x;
                float fTxy = fTy * x;
                float fTyz = fTz * y;
                float fTzz = fTz * z;

                return new Vec3(fTxy - fTwz, 1.0f - (fTxx + fTzz), fTyz + fTwx);
            }
        }
        #endregion Property: YAxis

        #region Property: ZAxis
        /// <summary>
        /// Gets the local Z-axis portion of this rotation.
        /// </summary>
        public Vec3 ZAxis
        {
            get
            {
                float fTx = 2.0f * x;
                float fTy = 2.0f * y;
                float fTz = 2.0f * z;
                float fTwx = fTx * w;
                float fTwy = fTy * w;
                float fTxx = fTx * x;
                float fTxz = fTz * x;
                float fTyy = fTy * y;
                float fTyz = fTz * y;

                return new Vec3(fTxz + fTwy, fTyz - fTwx, 1.0f - (fTxx + fTyy));
            }
        }
        #endregion Property: ZAxis

        #region Property: Pitch
        /// <summary>
        /// Gets or sets the pitch (in radians).
        /// </summary>
        public float Pitch
        {
            get
            {
                float test = x * y + z * w;
                if (System.Math.Abs(test) > 0.499f) // singularity at north and south pole
                    return 0f;
                return (float)System.Math.Atan2(2 * x * w - 2 * y * z, 1 - 2 * x * x - 2 * z * z);
            }
            set
            {
                float pitch, yaw, roll;
                ToEulerAngles(out pitch, out yaw, out roll);
                this.Set(FromEulerAngles(value, yaw, roll));
            }
        }
        #endregion Property: Pitch

        #region Property: Yaw
        /// <summary>
        /// Gets or sets the yaw (in radians).
        /// </summary>
        public float Yaw
        {
            get
            {
                float test = x * y + z * w;
                if (System.Math.Abs(test) > 0.499f) // singularity at north and south pole
                    return System.Math.Sign(test) * 2 * (float)System.Math.Atan2(x, w);
                return (float)System.Math.Atan2(2 * y * w - 2 * x * z, 1 - 2 * y * y - 2 * z * z);
            }
            set
            {
                float pitch, yaw, roll;
                ToEulerAngles(out pitch, out yaw, out roll);
                this.Set(FromEulerAngles(pitch, value, roll));
            }
        }
        #endregion Property: Yaw

        #region Property: Roll
        /// <summary>
        /// Gets or sets the roll (in radians).
        /// </summary>
        public float Roll
        {
            get
            {
                float test = x * y + z * w;
                if (System.Math.Abs(test) > 0.499f) // singularity at north and south pole
                    return (float)(System.Math.Sign(test) * Math.PI / 2);
                return (float)System.Math.Asin(2 * test);
            }
            set
            {
                float pitch, yaw, roll;
                ToEulerAngles(out pitch, out yaw, out roll);
                this.Set(FromEulerAngles(pitch, yaw, value));
            }
        }
        #endregion Property: Roll

        #region Field: EPSILON
        /// <summary>
        /// Epsilon.
        /// </summary>
        private const float EPSILON = 1e-03f;
        #endregion Field: EPSILON

        #endregion Properties and Fields

        #region Constructor: Quaternion()
        /// <summary>
        /// Creates a default quaternion.
        /// </summary>
        public Quaternion()
        {
        }
        #endregion Constructor: Quaternion()

        #region Constructor: Quaternion(float w, float x, float y, float z)
        /// <summary>
        /// Creates a new quaternion with the given values.
        /// </summary>
        /// <param name="w">The W value.</param>
        /// <param name="x">The X value.</param>
        /// <param name="y">The Y value.</param>
        /// <param name="z">The Z value.</param>
        public Quaternion(float w, float x, float y, float z)
        {
            this.W = w;
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        #endregion Constructor: Quaternion(float w, float x, float y, float z)

        #region Constructor: Quaternion(Vec4 vector)
        /// <summary>
        /// Creates a quaternion with the four values from the vector.
        /// </summary>
        /// <param name="vector">The vector from which the values should be used to create the quaternion.</param>
        public Quaternion(Vec4 vector)
        {
            if (vector != null)
            {
                this.W = vector.W;
                this.X = vector.X;
                this.Y = vector.Y;
                this.Z = vector.Z;
            }
        }
        #endregion Constructor: Quaternion(Vec4 vector)

        #region Constructor: Quaternion(Quaternion quaternion)
        /// <summary>
        /// Clones a quaternion.
        /// </summary>
        /// <param name="quaternion">The quaternion to clone.</param>
        public Quaternion(Quaternion quaternion)
        {
            if (quaternion != null)
            {
                this.W = quaternion.W;
                this.X = quaternion.X;
                this.Y = quaternion.Y;
                this.Z = quaternion.Z;
            }
        }
        #endregion Constructor: Quaternion(Quaternion quaternion)

        #region Method: Slerp(float time, Quaternion quatA, Quaternion quatB)
        /// <summary>
        /// Slerp.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="quatA"></param>
        /// <param name="quatB"></param>
        /// <returns></returns>
        public static Quaternion Slerp(float time, Quaternion quatA, Quaternion quatB)
        {
            return Slerp(time, quatA, quatB, false);
        }
        #endregion Method: Slerp(float time, Quaternion quatA, Quaternion quatB)

        #region Method: Slerp(float time, Quaternion quatA, Quaternion quatB, bool useShortestPath)
        /// <summary>
        /// Slerp.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="quatA"></param>
        /// <param name="quatB"></param>
        /// <param name="useShortestPath"></param>
        /// <returns></returns>
        public static Quaternion Slerp(float time, Quaternion quatA, Quaternion quatB, bool useShortestPath)
        {
            float cos = quatA.Dot(quatB);

            float angle = (float)Math.Acos(cos);

            if (Math.Abs(angle) < EPSILON)
            {
                return quatA;
            }

            float sin = (float)Math.Sin(angle);
            float inverseSin = 1.0f / sin;
            float coeff0 = (float)(Math.Sin((1.0f - time) * angle) * inverseSin);
            float coeff1 = (float)(Math.Sin(time * angle) * inverseSin);

            Quaternion result;

            if (cos < 0.0f && useShortestPath)
            {
                coeff0 = -coeff0;
                
                Quaternion t = coeff0 * quatA + coeff1 * quatB;
                t.Normalize();
                result = t;
            }
            else
            {
                result = (coeff0 * quatA + coeff1 * quatB);
            }

            return result;
        }
        #endregion Method: Slerp(float time, Quaternion quatA, Quaternion quatB, bool useShortestPath)

        #region Method: FromAngleAxis(float angle, Vec3 axis)
        /// <summary>
        /// Creates a quaternion from the supplied angle and axis.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="angle">The angle.</param>
        /// <returns>A quaternion from the supplied angle and axis.</returns>
        public static Quaternion FromAxisAngle(Vec3 axis, float angle)
        {
            Quaternion quat = new Quaternion();

            float halfAngle = 0.5f * angle;
            float sin = (float)Math.Sin(halfAngle);

            quat.W = (float)Math.Cos(halfAngle);
            quat.X = sin * axis.X;
            quat.Y = sin * axis.Y;
            quat.Z = sin * axis.Z;

            return quat;
        }
        #endregion Method: FromAngleAxis(float angle, Vec3 axis)

        #region Method: Squad(float t, Quaternion p, Quaternion a, Quaternion b, Quaternion q)
        /// <summary>
        /// Performs spherical quadratic interpolation.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="p"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static Quaternion Squad(float t, Quaternion p, Quaternion a, Quaternion b, Quaternion q)
        {
            return Squad(t, p, a, b, q, false);
        }
        #endregion Method: Squad(float t, Quaternion p, Quaternion a, Quaternion b, Quaternion q)

        #region Method: Squad(float t, Quaternion p, Quaternion a, Quaternion b, Quaternion q, bool useShortestPath)
        /// <summary>
        /// Performs spherical quadratic interpolation.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="p"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="q"></param>
        /// <param name="useShortestPath"></param>
        /// <returns></returns>
        public static Quaternion Squad(float t, Quaternion p, Quaternion a, Quaternion b, Quaternion q, bool useShortestPath)
        {
            float slerpT = 2.0f * t * (1.0f - t);

            // Use spherical linear interpolation
            Quaternion slerpP = Slerp(t, p, q, useShortestPath);
            Quaternion slerpQ = Slerp(t, a, b);

            // Run another Slerp on the results of the first 2, and return the results
            return Slerp(slerpT, slerpP, slerpQ);
        }
        #endregion Method: Squad(float t, Quaternion p, Quaternion a, Quaternion b, Quaternion q, bool useShortestPath)

        #region Method: ToEulerAngles()
        /// <summary>
        /// Calculates the Euler angles (pitch, yaw, roll) in radians.
        /// </summary>
        /// <returns>A vector with the Euler angles (pitch, yaw, roll) in radians.</returns>
        public Vec3 ToEulerAngles()
        {
            float pitch, yaw, roll;
            ToEulerAngles(out pitch, out yaw, out roll);
            return new Vec3(pitch, yaw, roll);
        }
        #endregion Method: ToEulerAngles()

        #region Method: ToEulerAngles(out float pitch, out float yaw, out float roll)
        /// <summary>
        /// Calculates the Euler angles (pitch, yaw, roll) in radians.
        /// </summary>
        /// <param name="pitch">The pitch.</param>
        /// <param name="yaw">The yaw.</param>
        /// <param name="roll">The roll.</param>
        public void ToEulerAngles(out float pitch, out float yaw, out float roll)
        {
            float halfPi = (float)System.Math.PI / 2;
            float test = x * y + z * w;
            if (test > 0.499f)
            { // singularity at north pole
                yaw = 2 * (float)System.Math.Atan2(x, w);
                roll = halfPi;
                pitch = 0;
            }
            else if (test < -0.499f)
            { // singularity at south pole
                yaw = -2 * (float)System.Math.Atan2(x, w);
                roll = -halfPi;
                pitch = 0;
            }
            else
            {
                float sqx = x * x;
                float sqy = y * y;
                float sqz = z * z;
                yaw = (float)System.Math.Atan2(2 * y * w - 2 * x * z, 1 - 2 * sqy - 2 * sqz);
                roll = (float)System.Math.Asin(2 * test);
                pitch = (float)System.Math.Atan2(2 * x * w - 2 * y * z, 1 - 2 * sqx - 2 * sqz);
            }

            //if (pitch <= float.Epsilon)
            //    pitch = 0f;
            //if (yaw <= float.Epsilon)
            //    yaw = 0f;
            //if (roll <= float.Epsilon)
            //    roll = 0f;
        }
        #endregion Method: ToEulerAngles(out float pitch, out float yaw, out float roll)

        #region Method: FromEulerAngles(float pitch, float yaw, float roll)
        /// <summary>
        /// Combines the Euler angles in the order yaw, pitch, roll (in radians) to create a rotation quaternion.
        /// </summary>
        /// <param name="pitch">The pitch.</param>
        /// <param name="yaw">The yaw.</param>
        /// <param name="roll">The roll.</param>
        /// <returns>A quaternion from the given Euler angles.</returns>
        public static Quaternion FromEulerAngles(float pitch, float yaw, float roll)
        {
            return Quaternion.FromAxisAngle(Vec3.Up, yaw) *
                   Quaternion.FromAxisAngle(Vec3.Right, pitch) *
                   Quaternion.FromAxisAngle(Vec3.Forward, roll);
        }
        #endregion Method: FromEulerAngles(float pitch, float yaw, float roll)

        #region Method: ToAngleAxis(ref float angle, ref Vec3 axis)
        /// <summary>
        /// Rotates the given axis with the angle.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="axis">The axis.</param>
        public void ToAngleAxis(ref float angle, ref Vec3 axis)
        {
            // The quaternion representing the rotation is
            //   q = cos(A/2)+sin(A/2)*(x*i+y*j+z*k)

            float sqrLength = x * x + y * y + z * z;

            if (sqrLength > 0.0f)
            {
                angle = 2.0f * (float)Math.Acos(w);
                float invLength = 1.0f / (float)Math.Sqrt(sqrLength);
                axis.X = x * invLength;
                axis.Y = y * invLength;
                axis.Z = z * invLength;
            }
            else
            {
                angle = 0.0f;
                axis.X = 1.0f;
                axis.Y = 0.0f;
                axis.Z = 0.0f;
            }
        }
        #endregion Method: ToAngleAxis(ref float angle, ref Vec3 axis)

        #region Method: ToRotationMatrix()
        /// <summary>
        /// Gets a 3x3 rotation matrix from this quaternion.
        /// </summary>
        /// <returns>A 3x3 rotation matrix from this quaternion.</returns>
        public Matrix3 ToRotationMatrix()
        {
            float tx = 2.0f * this.x;
            float ty = 2.0f * this.y;
            float tz = 2.0f * this.z;
            float twx = tx * this.w;
            float twy = ty * this.w;
            float twz = tz * this.w;
            float txx = tx * this.x;
            float txy = ty * this.x;
            float txz = tz * this.x;
            float tyy = ty * this.y;
            float tyz = tz * this.y;
            float tzz = tz * this.z;

            float[] pars = {1.0f - (tyy + tzz), txy - twz, txz + twy,
                             txy + twz, 1.0f - (txx + tzz), tyz - twx,
                             txz - twy, tyz + twx, 1.0f - (txx + tyy)};

            return new Matrix3(pars);
        }
        #endregion Method: ToRotationMatrix()

        #region Method: FromRotationMatrix(Matrix3 matrix)
        /// <summary>
        /// Sets the quaternion with values from the rotation matrix.
        /// </summary>
        /// <param name="matrix">The rotation matrix.</param>
        public void FromRotationMatrix(Matrix3 matrix)
        {
            float trace = matrix.m00 + matrix.m11 + matrix.m22;

            float root = 0.0f;

            if (trace > 0.0f)
            {
                // |this.w| > 1/2, may as well choose this.w > 1/2
                root = (float)Math.Sqrt(trace + 1.0f);  // 2w
                this.w = 0.5f * root;

                root = 0.5f / root;  // 1/(4w)

                this.x = (matrix.m21 - matrix.m12) * root;
                this.y = (matrix.m02 - matrix.m20) * root;
                this.z = (matrix.m10 - matrix.m01) * root;
            }
            else
            {
                // |this.w| <= 1/2

                int[] next = new int[3] { 1, 2, 0 };

                int i = 0;
                if (matrix.m11 > matrix.m00)
                    i = 1;
                if (matrix.m22 > matrix[i, i])
                    i = 2;

                int j = next[i];
                int k = next[j];

                root = (float)Math.Sqrt(matrix[i, i] - matrix[j, j] - matrix[k, k] + 1.0f);

                //unsafe
                //{
                //    fixed (float* apkQuat = &this.x)
                //    {
                //        apkQuat[i] = 0.5f * root;
                //        root = 0.5f / root;

                        this.w = (matrix[k, j] - matrix[j, k]) * root;

                //        apkQuat[j] = (matrix[j, i] + matrix[i, j]) * root;
                //        apkQuat[k] = (matrix[k, i] + matrix[i, k]) * root;
                //    }
                //}
            }
        }
        #endregion Method: FromRotationMatrix(Matrix3 matrix)

        #region Method: ToAxes(out Vec3 xAxis, out Vec3 yAxis, out Vec3 zAxis)
        /// <summary>
        /// Get the axes of the quaternion.
        /// </summary>
        /// <param name="xAxis">The X axis.</param>
        /// <param name="yAxis">The Y axis.</param>
        /// <param name="zAxis">The Z axis.</param>
        public void ToAxes(out Vec3 xAxis, out Vec3 yAxis, out Vec3 zAxis)
        {
            xAxis = new Vec3();
            yAxis = new Vec3();
            zAxis = new Vec3();

            Matrix3 rotation = this.ToRotationMatrix();

            xAxis.X = rotation.m00;
            xAxis.Y = rotation.m10;
            xAxis.Z = rotation.m20;

            yAxis.X = rotation.m01;
            yAxis.Y = rotation.m11;
            yAxis.Z = rotation.m21;

            zAxis.X = rotation.m02;
            zAxis.Y = rotation.m12;
            zAxis.Z = rotation.m22;
        }
        #endregion Method: ToAxes(out Vec3 xAxis, out Vec3 yAxis, out Vec3 zAxis)

        #region Method: FromAxes(Vec3 xAxis, Vec3 yAxis, Vec3 zAxis)
        /// <summary>
        /// Set the quaternion from the rotation axes.
        /// </summary>
        /// <param name="xAxis">The X axis.</param>
        /// <param name="yAxis">The Y axis.</param>
        /// <param name="zAxis">The Z axis.</param>
        public void FromAxes(Vec3 xAxis, Vec3 yAxis, Vec3 zAxis)
        {
            Matrix3 rotation = new Matrix3();

            rotation.m00 = xAxis.X;
            rotation.m10 = xAxis.Y;
            rotation.m20 = xAxis.Z;

            rotation.m01 = yAxis.X;
            rotation.m11 = yAxis.Y;
            rotation.m21 = yAxis.Z;

            rotation.m02 = zAxis.X;
            rotation.m12 = zAxis.Y;
            rotation.m22 = zAxis.Z;

            // Set the quaternion values from the rotation matrix
            FromRotationMatrix(rotation);
        }
        #endregion Method: FromAxes(Vec3 xAxis, Vec3 yAxis, Vec3 zAxis)

        #region Method: Dot(Quaternion quat)
        /// <summary>
        /// Performs a dot product operation on this and the other quaternion.
        /// </summary>
        /// <param name="quat">The other quaternion.</param>
        /// <returns>The dot product operation this and the other quaternion.</returns>
        public float Dot(Quaternion quat)
        {
            return this.W * quat.W + this.X * quat.X + this.Y * quat.Y + this.Z * quat.Z;
        }
        #endregion Method: Dot(Quaternion quat)

        #region Method: Normalize()
        /// <summary>
        ///	Normalizes elements of this quaterion to the range [0,1].
        /// </summary>
        public void Normalize()
        {
            float factor = 1.0f / (float)Math.Sqrt(this.Norm);

            this.W *= factor;
            this.X *= factor;
            this.Y *= factor;
            this.Z *= factor;
        }
        #endregion Method: Normalize()

        #region Method: Inverse()
        /// <summary>
        /// Computes the inverse.
        /// </summary>
        /// <returns>The inverse.</returns>
        public Quaternion Inverse()
        {
            float norm = this.Norm;
            if (norm > 0.0f)
            {
                float inverseNorm = 1.0f / norm;
                return new Quaternion(this.w * inverseNorm, -this.x * inverseNorm, -this.y * inverseNorm, -this.z * inverseNorm);
            }
            else
            {
                return Quaternion.Zero;
            }
        }
        #endregion Method: Inverse()

        #region Method: Conjugate()
        /// <summary>
        /// Returns the conjugate of the quaternion.
        /// </summary>
        /// <returns>The conjugate.</returns>
        public Quaternion Conjugate()
        {
            return new Quaternion(this.W, -this.X, -this.Y, - this.Z);
        }
        #endregion Method: Conjugate()

        #region Method: Log()
        /// <summary>
        /// Calculates the logarithm of the quaternion.
        /// </summary>
        /// <returns>The logarithm.</returns>
        public Quaternion Log()
        {
            // If q = cos(A)+sin(A)*(x*i+y*j+z*k) where (x,y,z) is unit length, then
            // log(q) = A*(x*i+y*j+z*k).  If sin(A) is near zero, use log(q) =
            // sin(A)*(x*i+y*j+z*k) since sin(A)/A has limit 1.

            // start off with a zero quat
            Quaternion result = Quaternion.Zero;

            if (Math.Abs(w) < 1.0f)
            {
                float angle = (float)Math.Acos(w);
                float sin = (float)Math.Sin(angle);

                if (Math.Abs(sin) >= EPSILON)
                {
                    float coeff = angle / sin;
                    result.x = coeff * x;
                    result.y = coeff * y;
                    result.z = coeff * z;
                }
                else
                {
                    result.x = x;
                    result.y = y;
                    result.z = z;
                }
            }

            return result;
        }
        #endregion Method: Log()

        #region Method: Exp()
        /// <summary>
        /// Calculates the exponent of the quaternion.
        /// </summary>
        /// <returns>The exponent.</returns>
        public Quaternion Exp()
        {
            // If q = A*(x*i+y*j+z*k) where (x,y,z) is unit length, then
            // exp(q) = cos(A)+sin(A)*(x*i+y*j+z*k).  If sin(A) is near zero,
            // use exp(q) = cos(A)+A*(x*i+y*j+z*k) since A/sin(A) has limit 1.

            float angle = (float)Math.Sqrt(x * x + y * y + z * z);
            float sin = (float)Math.Sin(angle);

            // start off with a zero quat
            Quaternion result = Quaternion.Zero;

            result.w = (float)Math.Cos(angle);

            if (Math.Abs(sin) >= EPSILON)
            {
                float coeff = sin / angle;

                result.x = coeff * x;
                result.y = coeff * y;
                result.z = coeff * z;
            }
            else
            {
                result.x = x;
                result.y = y;
                result.z = z;
            }

            return result;
        }
        #endregion Method: Exp()

        #region Method Group: Operator overloads

        public static Quaternion operator *(Quaternion left, Quaternion right)
        {
            Quaternion q = new Quaternion();

            q.w = left.w * right.w - left.x * right.x - left.y * right.y - left.z * right.z;
            q.x = left.w * right.x + left.x * right.w + left.y * right.z - left.z * right.y;
            q.y = left.w * right.y + left.y * right.w + left.z * right.x - left.x * right.z;
            q.z = left.w * right.z + left.z * right.w + left.x * right.y - left.y * right.x;

            return q;
        }

        public static Vec3 operator *(Quaternion quat, Vec3 vector)
        {
            Vec3 uv, uuv;
            Vec3 qvec = new Vec3(quat.x, quat.y, quat.z);

            uv = qvec.Cross(vector);
            uuv = qvec.Cross(uv);
            uv *= (2.0f * quat.w);
            uuv *= 2.0f;

            return vector + uv + uuv;
        }

        public static Quaternion operator *(float scalar, Quaternion right)
        {
            return new Quaternion(scalar * right.w, scalar * right.x, scalar * right.y, scalar * right.z);
        }

        public static Quaternion operator *(Quaternion left, float scalar)
        {
            return new Quaternion(scalar * left.w, scalar * left.x, scalar * left.y, scalar * left.z);
        }

        public static Quaternion operator +(Quaternion left, Quaternion right)
        {
            return new Quaternion(left.w + right.w, left.x + right.x, left.y + right.y, left.z + right.z);
        }

        public static Quaternion operator -(Quaternion right)
        {
            return new Quaternion(-right.w, -right.x, -right.y, -right.z);
        }

        public static bool operator ==(Quaternion left, Quaternion right)
        {
            if ((object)left == null)
                return (object)right == null;
            if ((object)right == null)
                return false;
            return (left.w == right.w && left.x == right.x && left.y == right.y && left.z == right.z);
        }

        public static bool operator !=(Quaternion left, Quaternion right)
        {
            if ((object)left == null)
                return (object)right != null;
            if ((object)right == null)
                return true;
            return (left.w != right.w || left.x != right.x || left.y != right.y || left.z != right.z);
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is Quaternion && this == (Quaternion)obj;
        }

        #endregion Method Group: Operator overloads

        /// <summary>
        /// Set the quaternion with the same values as the given quaternion
        /// </summary>
        /// <param name="quaternion">The given quaternion whose values should be copied.</param>
        private void Set(Quaternion quaternion)
        {
            this.x = quaternion.X;
            this.y = quaternion.Y;
            this.z = quaternion.Z;
            this.w = quaternion.W;
            if (this.ValueChanged != null)
                ValueChanged(this);
        }

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return ToEulerAngles().ToString();
        }
        #endregion Method: ToString()

    }
    #endregion Class: Quaternion

}