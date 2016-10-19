using System;
using System.Collections.Generic;

using System.Text;
using System.IO;

namespace Common
{
    public class Matrix4 : IDisposable
    {
        static float[] zeros = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        internal float[] matrix = new float[16];

        /// <summary>
        /// Top row of the matrix
        /// </summary>
        public Vec4 Row0 {
            get { return new Vec4(matrix[0], matrix[1], matrix[2], matrix[3]); }
            set { matrix[0] = value.X; matrix[1] = value.Y; matrix[2] = value.Z; matrix[3] = value.W; }
        }
        /// <summary>
        /// 2nd row of the matrix
        /// </summary>
        public Vec4 Row1 {
            get { int delta = 4; return new Vec4(matrix[delta + 0], matrix[delta + 1], matrix[delta + 2], matrix[delta + 3]); }
            set { int delta = 4; matrix[delta + 0] = value.X; matrix[delta + 1] = value.Y; matrix[delta + 2] = value.Z; matrix[delta + 3] = value.W; }
        }
        /// <summary>
        /// 3rd row of the matrix
        /// </summary>
        public Vec4 Row2 {
            get { int delta = 8; return new Vec4(matrix[delta + 0], matrix[delta + 1], matrix[delta + 2], matrix[delta + 3]); }
            set { int delta = 8; matrix[delta + 0] = value.X; matrix[delta + 1] = value.Y; matrix[delta + 2] = value.Z; matrix[delta + 3] = value.W; }
        }
        /// <summary>
        /// Bottom row of the matrix
        /// </summary>
        public Vec4 Row3 {
            get { int delta = 12; return new Vec4(matrix[delta + 0], matrix[delta + 1], matrix[delta + 2], matrix[delta + 3]); }
            set { int delta = 12; matrix[delta + 0] = value.X; matrix[delta + 1] = value.Y; matrix[delta + 2] = value.Z; matrix[delta + 3] = value.W; }
        }
        /// <summary>
        /// The first column of this matrix
        /// </summary>
        public Vec4 Column0 {
            get { return new Vec4(Row0.X, Row1.X, Row2.X, Row3.X); }
        }

        /// <summary>
        /// The second column of this matrix
        /// </summary>
        public Vec4 Column1 {
            get { return new Vec4(Row0.Y, Row1.Y, Row2.Y, Row3.Y); }
        }

        /// <summary>
        /// The third column of this matrix
        /// </summary>
        public Vec4 Column2 {
            get { return new Vec4(Row0.Z, Row1.Z, Row2.Z, Row3.Z); }
        }

        /// <summary>
        /// The fourth column of this matrix
        /// </summary>
        public Vec4 Column3 {
            get { return new Vec4(Row0.W, Row1.W, Row2.W, Row3.W); }
        }
        /// <summary>
        /// Gets or sets the value at row 1, column 1 of this instance.
        /// </summary>
        public float M11 { get { return Row0.X; } set { Row0.X = value; } }
        /// <summary>
        /// Gets or sets the value at row 1, column 2 of this instance.
        /// </summary>
        public float M12 { get { return Row0.Y; } set { Row0.Y = value; } }
        /// <summary>
        /// Gets or sets the value at row 1, column 3 of this instance.
        /// </summary>
        public float M13 { get { return Row0.Z; } set { Row0.Z = value; } }
        /// <summary>
        /// Gets or sets the value at row 1, column 4 of this instance.
        /// </summary>
        public float M14 { get { return Row0.W; } set { Row0.W = value; } }
        /// <summary>
        /// Gets or sets the value at row 2, column 1 of this instance.
        /// </summary>
        public float M21 { get { return Row1.X; } set { Row1.X = value; } }
        /// <summary>
        /// Gets or sets the value at row 2, column 2 of this instance.
        /// </summary>
        public float M22 { get { return Row1.Y; } set { Row1.Y = value; } }
        /// <summary>
        /// Gets or sets the value at row 2, column 3 of this instance.
        /// </summary>
        public float M23 { get { return Row1.Z; } set { Row1.Z = value; } }
        /// <summary>
        /// Gets or sets the value at row 2, column 4 of this instance.
        /// </summary>
        public float M24 { get { return Row1.W; } set { Row1.W = value; } }
        /// <summary>
        /// Gets or sets the value at row 3, column 1 of this instance.
        /// </summary>
        public float M31 { get { return Row2.X; } set { Row2.X = value; } }
        /// <summary>
        /// Gets or sets the value at row 3, column 2 of this instance.
        /// </summary>
        public float M32 { get { return Row2.Y; } set { Row2.Y = value; } }
        /// <summary>
        /// Gets or sets the value at row 3, column 3 of this instance.
        /// </summary>
        public float M33 { get { return Row2.Z; } set { Row2.Z = value; } }
        /// <summary>
        /// Gets or sets the value at row 3, column 4 of this instance.
        /// </summary>
        public float M34 { get { return Row2.W; } set { Row2.W = value; } }
        /// <summary>
        /// Gets or sets the value at row 4, column 1 of this instance.
        /// </summary>
        public float M41 { get { return Row3.X; } set { Row3.X = value; } }
        /// <summary>
        /// Gets or sets the value at row 4, column 2 of this instance.
        /// </summary>
        public float M42 { get { return Row3.Y; } set { Row3.Y = value; } }
        /// <summary>
        /// Gets or sets the value at row 4, column 3 of this instance.
        /// </summary>
        public float M43 { get { return Row3.Z; } set { Row3.Z = value; } }
        /// <summary>
        /// Gets or sets the value at row 4, column 4 of this instance.
        /// </summary>
        public float M44 { get { return Row3.W; } set { Row3.W = value; } }

        /// <summary>
        /// The identity matrix
        /// </summary>
        public static Matrix4 Identity
        {
            get { return new Matrix4(Vec4.UnitX, Vec4.UnitY, Vec4.UnitZ, Vec4.UnitW); }
        }

        public float GetElement(int row, int col) { return matrix[col + (row << 2)]; }
        public void SetElement(int row, int col, float value)
        {
            matrix[col + (row << 2)] = value;
        }

        public float this[int row, int col]
        {
            get { return matrix[col + (row << 2)]; }
            set { matrix[col + (row << 2)] = value; }
        }

        public Matrix4() { }
        public Matrix4(float[] values) { for (int i = 0; i < 16; ++i) matrix[i] = values[i]; }
        public Matrix4(Matrix4 copy) : this(copy.matrix) { }
        public Matrix4(Vec4 row0, Vec4 row1, Vec4 row2, Vec4 row3) 
        {
            Row0 = row0;
            Row1 = row1;
            Row2 = row2;
            Row3 = row3;
        }

        public static Matrix4 Translation(Vec3 translation)
        {
            Matrix4 m = Identity;
            m.matrix[3] = translation.X;
            m.matrix[7] = translation.Y;
            m.matrix[11] = translation.Z;
            return m;
        }

        public static Matrix4 TranslationRuben(Vec3 translation) {
            Matrix4 m = Identity;
            m.matrix[12] = translation.X;
            m.matrix[13] = translation.Y;
            m.matrix[14] = translation.Z;
            return m;
        }

        public static Matrix4 Scalation(Vec3 m_scalation)
        {
            Matrix4 m = Identity;
            m.matrix[0] = m_scalation.X;
            m.matrix[5] = m_scalation.Y;
            m.matrix[10] = m_scalation.Z;
            return m;
        }

        public static Matrix4 RotationX(float angle)
        {
            Matrix4 m = Identity;
            m.matrix[5] = (float)Math.Cos(angle);
            m.matrix[6] = (float)Math.Sin(angle);
            m.matrix[9] = -(float)Math.Sin(angle);
            m.matrix[10] = (float)Math.Cos(angle);
            return m;
        }

        public static Matrix4 RotationmX(float angle)
        {
            Matrix4 m = Identity;
            m.matrix[5] = (float)Math.Cos(angle);
            m.matrix[6] = -(float)Math.Sin(angle);
            m.matrix[9] = (float)Math.Sin(angle);
            m.matrix[10] = (float)Math.Cos(angle);
            return m;
        }

        public static Matrix4 RotationCCWY(float angle)
        {
            Matrix4 m = Identity;
            m.matrix[0] = (float)Math.Cos(angle);
            m.matrix[2] = (float)Math.Sin(angle);
            m.matrix[8] = -(float)Math.Sin(angle);
            m.matrix[10] = (float)Math.Cos(angle);
            return m;
        }

        public static Matrix4 RotationCWY(float angle)
        {
            Matrix4 m = Identity;
            m.matrix[0] = (float)Math.Cos(angle);
            m.matrix[2] = -(float)Math.Sin(angle);
            m.matrix[8] = (float)Math.Sin(angle);
            m.matrix[10] = (float)Math.Cos(angle);
            return m;
        }

        public static Matrix4 RotationY(float angle)
        {
            Matrix4 m = Identity;
            m.matrix[0] = (float)Math.Cos(angle);
            m.matrix[2] = -(float)Math.Sin(angle);
            m.matrix[8] = (float)Math.Sin(angle);
            m.matrix[10] = (float)Math.Cos(angle);
            return m;
        }

        public static Matrix4 RotationmY(float angle) {
            Matrix4 m = Identity;
            m.matrix[0] = (float)Math.Cos(angle);
            m.matrix[2] = (float)Math.Sin(angle);
            m.matrix[8] = -(float)Math.Sin(angle);
            m.matrix[10] = (float)Math.Cos(angle);
            return m;
        }

        public static Matrix4 RotationYPython(float angle)
        {
            Matrix4 m = Identity;
            m.matrix[0] = (float)Math.Cos(angle);
            m.matrix[2] = (float)Math.Sin(angle);
            m.matrix[8] = -(float)Math.Sin(angle);
            m.matrix[10] = (float)Math.Cos(angle);
            return m;
        }

        public static Matrix4 RotationZ(float angle)
        {
            Matrix4 m = Identity;
            m.matrix[0] = (float)Math.Cos(angle);
            m.matrix[1] = (float)Math.Sin(angle);
            m.matrix[4] = -(float)Math.Sin(angle);
            m.matrix[5] = (float)Math.Cos(angle);
            return m;
        }

        public static Matrix4 RotationmZ(float angle)
        {
            Matrix4 m = Identity;
            m.matrix[0] = (float)Math.Cos(angle);
            m.matrix[1] = -(float)Math.Sin(angle);
            m.matrix[4] = (float)Math.Sin(angle);
            m.matrix[5] = (float)Math.Cos(angle);
            return m;
        }

        /// <summary>
        /// Build a rotation matrix from the specified axis/angle rotation.
        /// </summary>
        /// <param name="axis">The axis to rotate about.</param>
        /// <param name="angle">Angle in radians to rotate counter-clockwise (looking in the direction of the given axis).</param>
        /// <returns>The resulting rotation Matrix4.</returns>
        public static Matrix4 RotateArbitraryAxis(Vec3 axis, float angle) {
            float cos = (float)System.Math.Cos(angle);
            float sin = (float)System.Math.Sin(angle);
            float t = 1.0f - cos;
            axis.Normalize();
            return new Matrix4(new float[] { t * axis.X * axis.X + cos, t * axis.X * axis.Y - sin * axis.Z, t * axis.X * axis.Z + sin * axis.Y, 0.0f,
                                 t * axis.X * axis.Y + sin * axis.Z, t * axis.Y * axis.Y + cos, t * axis.Y * axis.Z - sin * axis.X, 0.0f,
                                 t * axis.X * axis.Z - sin * axis.Y, t * axis.Y * axis.Z + sin * axis.X, t * axis.Z * axis.Z + cos, 0.0f,
                                 0, 0, 0, 1});
        }

        public static Matrix4 ArbitraryRotation(float angle, Vec3 rotationAxis)
        {
            if (angle == 0)
                return Matrix4.Identity;
            float u = rotationAxis.X; float u2 = u * u;
            float v = rotationAxis.Y; float v2 = v * v;
            float w = rotationAxis.Z; float w2 = w * w;
            float cos = (float)Math.Cos(angle);
            float sin = (float)Math.Sin(angle);
            float sum = u2 + v2 + w2;
            float mult = 1 / sum;
            float sqrt = (float)Math.Sqrt(sum);
            float sqrtTsin = sqrt * sin;
            float oneMcos = (1 - cos);
            Matrix4 m = Identity;
            m.matrix[0] = mult * (u2 + (v2 + w2) * cos);
            m.matrix[1] = mult * ((u * v * oneMcos) - (w * sqrtTsin));
            m.matrix[2] = mult * ((u * w * oneMcos) + (v * sqrtTsin));

            m.matrix[4] = mult * ((u * v * oneMcos) + (w * sqrtTsin));
            m.matrix[5] = mult * (v2 + (u2 + w2) * cos);
            m.matrix[6] = mult * ((v * w * oneMcos) - (u * sqrtTsin));

            m.matrix[8] = mult * ((u * w * oneMcos) - (v * sqrtTsin));
            m.matrix[9] = mult * ((v * w * oneMcos) + (u * sqrtTsin));
            m.matrix[10] = mult * (w2 + (u2 + v2) * cos);
            return m;
        }

        public static Vec3 operator *(Matrix4 m, Vec3 v)
        {
            Vec4 v2 = new Vec4(v);
            return new Vec3(m * v2);
        }

        public static Vec3 operator *(Vec3 v, Matrix4 m)
        {
            if (m.matrix[0] == 1 && m.matrix[1] == 0 && m.matrix[2] == 0 && m.matrix[3] == 0 &&
                m.matrix[4] == 0 && m.matrix[5] == 1 && m.matrix[6] == 0 && m.matrix[7] == 0 &&
                m.matrix[8] == 0 && m.matrix[9] == 0 && m.matrix[10] == 1 && m.matrix[11] == 0 &&
                m.matrix[12] == 0 && m.matrix[13] == 0 && m.matrix[14] == 0 && m.matrix[15] == 1)
                return new Vec3(v);
            return new Vec3(new Vec4(v.X * m.matrix[0] + v.Y * m.matrix[1] + v.Z * m.matrix[2] + m.matrix[3],
                            v.X * m.matrix[4] + v.Y * m.matrix[5] + v.Z * m.matrix[6] + m.matrix[7],
                            v.X * m.matrix[8] + v.Y * m.matrix[9] + v.Z * m.matrix[10] + m.matrix[11],
                            v.X * m.matrix[12] + v.Y * m.matrix[13] + v.Z * m.matrix[14] + m.matrix[15]));
        }

        public static Vec4 operator *(Matrix4 m, Vec4 v)
        {
            float t1 = v.X * m.matrix[0] + v.Y * m.matrix[4] + v.Z * m.matrix[8] + v.W * m.matrix[12];
            float t2 = v.X * m.matrix[1] + v.Y * m.matrix[5] + v.Z * m.matrix[9] + v.W * m.matrix[13];
            float t3 = v.X * m.matrix[2] + v.Y * m.matrix[6] + v.Z * m.matrix[10] + v.W * m.matrix[14];
            float t4 = v.X * m.matrix[3] + v.Y * m.matrix[7] + v.Z * m.matrix[11] + v.W * m.matrix[15];

            return new Vec4(t1, t2, t3, t4);
        }

        public static Vec4 operator *(Vec4 v, Matrix4 m)
        {
            return new Vec4(v.X * m.matrix[0] + v.Y * m.matrix[1] + v.Z * m.matrix[2] + v.W * m.matrix[3], 
                            v.X * m.matrix[4] + v.Y * m.matrix[5] + v.Z * m.matrix[6] + v.W * m.matrix[7], 
                            v.X * m.matrix[8] + v.Y * m.matrix[9] + v.Z * m.matrix[10] + v.W * m.matrix[11], 
                            v.X * m.matrix[12] + v.Y * m.matrix[13] + v.Z * m.matrix[14] + v.W * m.matrix[15]);
        }

        public static int nrOfMatrixMult = 0;

        public static Matrix4 operator *(Matrix4 m1, Matrix4 m2)
        {
            ++nrOfMatrixMult;
            Matrix4 m = new Matrix4();
            float[] u = m1.matrix;
            float[] v = m2.matrix;
            float[] mats = m.matrix;

            mats[0] = u[0] * v[0] + u[4] * v[1] + u[8] * v[2] + u[12] * v[3];
            mats[1] = u[1] * v[0] + u[5] * v[1] + u[9] * v[2] + u[13] * v[3];
            mats[2] = u[2] * v[0] + u[6] * v[1] + u[10] * v[2] + u[14] * v[3];
            mats[3] = u[3] * v[0] + u[7] * v[1] + u[11] * v[2] + u[15] * v[3];
            mats[4] = u[0] * v[4] + u[4] * v[5] + u[8] * v[6] + u[12] * v[7];
            mats[5] = u[1] * v[4] + u[5] * v[5] + u[9] * v[6] + u[13] * v[7];
            mats[6] = u[2] * v[4] + u[6] * v[5] + u[10] * v[6] + u[14] * v[7];
            mats[7] = u[3] * v[4] + u[7] * v[5] + u[11] * v[6] + u[15] * v[7];
            mats[8] = u[0] * v[8] + u[4] * v[9] + u[8] * v[10] + u[12] * v[11];
            mats[9] = u[1] * v[8] + u[5] * v[9] + u[9] * v[10] + u[13] * v[11];
            mats[10] = u[2] * v[8] + u[6] * v[9] + u[10] * v[10] + u[14] * v[11];
            mats[11] = u[3] * v[8] + u[7] * v[9] + u[11] * v[10] + u[15] * v[11];
            mats[12] = u[0] * v[12] + u[4] * v[13] + u[8] * v[14] + u[12] * v[15];
            mats[13] = u[1] * v[12] + u[5] * v[13] + u[9] * v[14] + u[13] * v[15];
            mats[14] = u[2] * v[12] + u[6] * v[13] + u[10] * v[14] + u[14] * v[15];
            mats[15] = u[3] * v[12] + u[7] * v[13] + u[11] * v[14] + u[15] * v[15];

            return m;
        }

        public static Matrix4 operator *(float v, Matrix4 mat)
        {
            Matrix4 nmat = new Matrix4();
            float[] nm = nmat.matrix;
            float[] m = mat.matrix;
            nm[0] = v * m[0];
            nm[1] = v * m[1];
            nm[2] = v * m[2];
            nm[3] = v * m[3];
            nm[4] = v * m[4];
            nm[5] = v * m[5];
            nm[6] = v * m[6];
            nm[7] = v * m[7];
            nm[8] = v * m[8];
            nm[9] = v * m[9];
            nm[10] = v * m[10];
            nm[11] = v * m[11];
            nm[12] = v * m[12];
            nm[13] = v * m[13];
            nm[14] = v * m[14];
            nm[15] = v * m[15];
            return nmat;
        }

        public Matrix3 GetMatrixWithoutColumnAndRow(int row, int column)
        {
            Matrix3 ret = new Matrix3();
            int rr = 0;
            for (int r = 0; r < 4; ++r)
            {
                if (r != row)
                {
                    int cc = 0;
                    for (int c = 0; c < 4; ++c)
                    {
                        if (c != column)
                        {
                            ret.SetElement(rr, cc, GetElement(r, c));
                            ++cc;
                        }
                    }
                    ++rr;
                }
            }
            return ret;
        }

        static float[] cofs = { 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1, -1, 1 };

        public float Cofactor(int row, int column)
        {
            return cofs[row + column] * GetMatrixWithoutColumnAndRow(row, column).Determinant();
        }

        public float Determinant(Matrix4 adjunct)
        {
            float sum = 0;
            for (int i = 0; i < 4; ++i)
                sum += GetElement(i, 0) * adjunct.GetElement(0, i);
            return sum;
        }

        public float Determinant()
        {
            float sum = 0;
            for (int i = 0; i < 4; ++i)
                sum += Cofactor(i, 0);
            return sum;
        }

        private Matrix4 Adjunct()
        {
            float[] t = new float[16];
            for (int r = 0; r < 4; ++r)
            {
                for (int c = 0; c < 4; ++c)
                {
                    // normaal: matrix[col + row * 4] => in geinverteerde: matrix[row + col * 4]
                    t[r + c * 4] = Cofactor(r, c);
                }
            }
            return new Matrix4(t);
        }

        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < 4; ++i)
            {
                str += "[ [ ";
                for (int j = 0; j < 4; ++j)
                {
                    str += GetElement(i, j);
                    if (j < 3)
                        str += ", ";
                }
                if (i == 3)
                    str += " ] ]";
                else
                    str += " ]; ";
            }
            return str;
        }

        public static System.Diagnostics.Stopwatch logCounterInverse = new System.Diagnostics.Stopwatch();

        public Matrix4 Inverse()
        {
            //logCounterInverse.Start();
            Matrix4 adjunct = Adjunct();
            Matrix4 ret = (1 / Determinant(adjunct)) * adjunct;
            //logCounterInverse.Stop();
            return ret;
        }

        public void Save(BinaryWriter w)
        {
            for (int i = 0; i < 16; ++i)
                w.Write((double)this.matrix[i]);
        }

        public Matrix4(BinaryReader r)
        {
            for (int i = 0; i < 16; ++i)
                matrix[i] = (float)r.ReadDouble();
        }

        public List<Vec3> Transform(List<Vec3> geom) {
            List<Vec3> result = new List<Vec3>(geom.Count);
            foreach (Vec3 v in geom) {
                Vec4 vTH = this * new Vec4(v, 1.0f);
                Vec3 vT = new Vec3(vTH.X, vTH.Y, vTH.Z);
                result.Add(vT);
            }
            return result;
        }

        public void Dispose()
        {
            this.matrix = null;
            GC.SuppressFinalize(this);
        }
    }
}
