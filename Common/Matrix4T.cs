using System;
using System.Collections.Generic;
using System.Text;
using Common;

namespace Common {
    public class Matrix4T {
        /// <summary>
        /// Top row of the matrix
        /// </summary>
        public Vec4 Row0 {
            get { return new Vec4(m[m11], m[m12], m[m13], m[m14]); }
            set { 
                m[m11] = value.X;
                m[m12] = value.Y;
                m[m13] = value.Z;
                m[m14] = value.W; 
            }
        }
        /// <summary>
        /// 2nd row of the matrix
        /// </summary>
        public Vec4 Row1 {
            get { return new Vec4(m[m21], m[m22], m[m23], m[m24]); }
            set {
                m[m21] = value.X;
                m[m22] = value.Y;
                m[m23] = value.Z;
                m[m24] = value.W;
            }
        }
        /// <summary>
        /// 3rd row of the matrix
        /// </summary>
        public Vec4 Row2 {
            get { return new Vec4(m[m31], m[m32], m[m33], m[m34]); }
            set {
                m[m31] = value.X;
                m[m32] = value.Y;
                m[m33] = value.Z;
                m[m34] = value.W;
            }
        }
        /// <summary>
        /// Bottom row of the matrix
        /// </summary>
        public Vec4 Row3 {
            get { return new Vec4(m[m41], m[m42], m[m43], m[m44]); }
            set {
                m[m41] = value.X;
                m[m42] = value.Y;
                m[m43] = value.Z;
                m[m44] = value.W;
            }
        }
        /// <summary>
        /// The first column of this matrix
        /// </summary>
        public Vec4 Column1 {
            get { return new Vec4(Row0.X, Row1.X, Row2.X, Row3.X); }
        }

        /// <summary>
        /// The second column of this matrix
        /// </summary>
        public Vec4 Column2 {
            get { return new Vec4(Row0.Y, Row1.Y, Row2.Y, Row3.Y); }
        }

        /// <summary>
        /// The third column of this matrix
        /// </summary>
        public Vec4 Column3 {
            get { return new Vec4(Row0.Z, Row1.Z, Row2.Z, Row3.Z); }
        }

        /// <summary>
        /// The fourth column of this matrix
        /// </summary>
        public Vec4 Column4 {
            get { return new Vec4(Row0.W, Row1.W, Row2.W, Row3.W); }
        }

        /// <summary>
        /// Gets or sets the value at row 1, column 1 of this instance.
        /// </summary>
        public float M11 { get { return m[m11]; } set { m[m11] = value; } }
        /// <summary>
        /// Gets or sets the value at row 1, column 2 of this instance.
        /// </summary>
        public float M12 { get { return m[m12]; } set { m[m12] = value; } }
        /// <summary>
        /// Gets or sets the value at row 1, column 3 of this instance.
        /// </summary>
        public float M13 { get { return m[m13]; } set { m[m13] = value; } }
        /// <summary>
        /// Gets or sets the value at row 1, column 4 of this instance.
        /// </summary>
        public float M14 { get { return m[m14]; } set { m[m14] = value; } }
        /// <summary>
        /// Gets or sets the value at row 2, column 1 of this instance.
        /// </summary>
        public float M21 { get { return m[m21]; } set { m[m21] = value; } }
        /// <summary>
        /// Gets or sets the value at row 2, column 2 of this instance.
        /// </summary>
        public float M22 { get { return m[m22]; } set { m[m22] = value; } }
        /// <summary>
        /// Gets or sets the value at row 2, column 3 of this instance.
        /// </summary>
        public float M23 { get { return m[m23]; } set { m[m23] = value; } }
        /// <summary>
        /// Gets or sets the value at row 2, column 4 of this instance.
        /// </summary>
        public float M24 { get { return m[m24]; } set { m[m24] = value; } }
        /// <summary>
        /// Gets or sets the value at row 3, column 1 of this instance.
        /// </summary>
        public float M31 { get { return m[m31]; } set { m[m31] = value; } }
        /// <summary>
        /// Gets or sets the value at row 3, column 2 of this instance.
        /// </summary>
        public float M32 { get { return m[m32]; } set { m[m32] = value; } }
        /// <summary>
        /// Gets or sets the value at row 3, column 3 of this instance.
        /// </summary>
        public float M33 { get { return m[m33]; } set { m[m33] = value; } }
        /// <summary>
        /// Gets or sets the value at row 3, column 4 of this instance.
        /// </summary>
        public float M34 { get { return m[m34]; } set { m[m34] = value; } }
        /// <summary>
        /// Gets or sets the value at row 4, column 1 of this instance.
        /// </summary>
        public float M41 { get { return m[m41]; } set { m[m41] = value; } }
        /// <summary>
        /// Gets or sets the value at row 4, column 2 of this instance.
        /// </summary>
        public float M42 { get { return m[m42]; } set { m[m42] = value; } }
        /// <summary>
        /// Gets or sets the value at row 4, column 3 of this instance.
        /// </summary>
        public float M43 { get { return m[m43]; } set { m[m43] = value; } }
        /// <summary>
        /// Gets or sets the value at row 4, column 4 of this instance.
        /// </summary>
        public float M44 { get { return m[m44]; } set { m[m44] = value; } }

        private const int m11 = 0;
        private const int m12 = 1;
        private const int m13 = 2;
        private const int m14 = 3;
        private const int m21 = 4;
        private const int m22 = 5;
        private const int m23 = 6;
        private const int m24 = 7;
        private const int m31 = 8;
        private const int m32 = 9;
        private const int m33 = 10;
        private const int m34 = 11;
        private const int m41 = 12;
        private const int m42 = 13;
        private const int m43 = 14;
        private const int m44 = 15;
        private const int NUM_ELEMENTS = 4 * 4;

        private readonly float[] m;

        /// <summary>
        /// The identity matrix
        /// </summary>
        public static Matrix4T Identity {
            get { return new Matrix4T(Vec4.UnitX, Vec4.UnitY, Vec4.UnitZ, Vec4.UnitW); }
        }

        public Matrix4T() : this(Vec4.Zero, Vec4.Zero, Vec4.Zero, Vec4.Zero) {
        }

        public Matrix4T(Matrix4T toCopy) {
            this.m = new float[NUM_ELEMENTS];
            Row0 = toCopy.Row0;
            Row1 = toCopy.Row1;
            Row2 = toCopy.Row2;
            Row3 = toCopy.Row3;
        }

        public Matrix4T(Vec4 row0, Vec4 row1, Vec4 row2, Vec4 row3) {
            this.m = new float[NUM_ELEMENTS];
            Row0 = row0;
            Row1 = row1;
            Row2 = row2;
            Row3 = row3;
        }

        public Matrix4T(
            float m00, float m01, float m02, float m03,
            float m10, float m11, float m12, float m13,
            float m20, float m21, float m22, float m23,
            float m30, float m31, float m32, float m33) {
            this.m = new float[NUM_ELEMENTS];
            this.Row0 = new Vec4(m00, m01, m02, m03);
            this.Row1 = new Vec4(m10, m11, m12, m13);
            this.Row2 = new Vec4(m20, m21, m22, m23);
            this.Row3 = new Vec4(m30, m31, m32, m33);
        }

        /// <summary>
        /// Build a rotation matrix from the specified axis/angle rotation.
        /// </summary>
        /// <param name="axis">The axis to rotate about.</param>
        /// <param name="angle">Angle in radians to rotate counter-clockwise (looking in the direction of the given axis).</param>
        /// <returns>The resulting rotation Matrix4.</returns>
        public static Matrix4T RotateArbitraryAxis(Vec3 axis, float angle) {
            float cos = (float)System.Math.Cos(angle);
            float sin = (float)System.Math.Sin(angle);
            float t = 1.0f - cos;
            axis.Normalize();
            return new Matrix4T(t * axis.X * axis.X + cos, t * axis.X * axis.Y - sin * axis.Z, t * axis.X * axis.Z + sin * axis.Y, 0.0f,
                                 t * axis.X * axis.Y + sin * axis.Z, t * axis.Y * axis.Y + cos, t * axis.Y * axis.Z - sin * axis.X, 0.0f,
                                 t * axis.X * axis.Z - sin * axis.Y, t * axis.Y * axis.Z + sin * axis.X, t * axis.Z * axis.Z + cos, 0.0f,
                                 0, 0, 0, 1);
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the x-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting rotation Matrix4.</returns>
        public static Matrix4T RotateXAxis(float angle) {
            float cos = (float)System.Math.Cos(angle);
            float sin = (float)System.Math.Sin(angle);
            Matrix4T result = new Matrix4T(Vec4.UnitX, new Vec4(0, cos, -sin, 0), new Vec4(0, sin, cos, 0), Vec4.UnitW);
            return result;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the y-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting rotation Matrix4.</returns>
        public static Matrix4T RotateYAxis(float angle) {
            float cos = (float)System.Math.Cos(angle);
            float sin = (float)System.Math.Sin(angle);
            Matrix4T result = new Matrix4T( new Vec4(cos, 0, sin, 0), Vec4.UnitY, new Vec4(-sin, 0, cos, 0), Vec4.UnitW);
            return result;
        }

        /// <summary>
        /// Builds a rotation matrix for a rotation around the z-axis.
        /// </summary>
        /// <param name="angle">The counter-clockwise angle in radians.</param>
        /// <returns>The resulting rotation Matrix4.</returns>
        public static Matrix4T RotateZAxis(float angle) {
            float cos = (float)System.Math.Cos(angle);
            float sin = (float)System.Math.Sin(angle);
            Matrix4T result = new Matrix4T(new Vec4(cos, -sin, 0, 0), new Vec4(sin, cos, 0, 0), Vec4.UnitZ, Vec4.UnitW);
            return result;
        }

        /// <summary>
        /// Creates a translation matrix.
        /// </summary>
        /// <param name="vector">The translation vector.</param>
        /// <returns>The resulting translation Matrix4.</returns>
        public static Matrix4T Translate(Vec3 trans) {
            Matrix4T result = Identity;
            result.M14 = trans.X;
            result.M24 = trans.Y;
            result.M34 = trans.Z;
            return result;
        }

        /// <summary>
        /// Build a scaling matrix.
        /// </summary>
        /// <param name="s">Scale vector</param>
        /// <returns>The resulting scaling Matrix4.</returns>
        public static Matrix4T Scale(Vec3 s) {
            return Scale(s.X, s.Y, s.Z);
        }

        /// <summary>
        /// Build a scaling matrix.
        /// </summary>
        /// <param name="x">Scale factor for x-axis</param>
        /// <param name="y">Scale factor for y-axis</param>
        /// <param name="z">Scale factor for z-axis</param>
        /// <returns>The resulting scaling Matrix4.</returns>
        public static Matrix4T Scale(float x, float y, float z) {
            Matrix4T result = new Matrix4T(Vec4.UnitX * x, Vec4.UnitY * y, Vec4.UnitZ * z, Vec4.UnitW);
            return result;
        }

        public static Vec4 operator *(Matrix4T mat, Vec4 vec) {
            return new Vec4(
                vec.X * mat.Row0.X + vec.Y * mat.Row0.Y + vec.Z * mat.Row0.Z + vec.W * mat.Row0.W,
                vec.X * mat.Row1.X + vec.Y * mat.Row1.Y + vec.Z * mat.Row1.Z + vec.W * mat.Row1.W,
                vec.X * mat.Row2.X + vec.Y * mat.Row2.Y + vec.Z * mat.Row2.Z + vec.W * mat.Row2.W,
                vec.X * mat.Row3.X + vec.Y * mat.Row3.Y + vec.Z * mat.Row3.Z + vec.W * mat.Row3.W);
        }

        public static Vec3 operator *(Matrix4T mat, Vec3 vec) {
            return new Vec3(
                vec.X * mat.Row0.X + vec.Y * mat.Row0.Y + vec.Z * mat.Row0.Z + 1.0f * mat.Row0.W,
                vec.X * mat.Row1.X + vec.Y * mat.Row1.Y + vec.Z * mat.Row1.Z + 1.0f * mat.Row1.W,
                vec.X * mat.Row2.X + vec.Y * mat.Row2.Y + vec.Z * mat.Row2.Z + 1.0f * mat.Row2.W);
        }

        /// <summary>
        /// Multiplies two instances.
        /// </summary>
        /// <param name="left">The left operand of the multiplication.</param>
        /// <param name="right">The right operand of the multiplication.</param>
        /// <param name="result">A new instance that is the result of the multiplication</param>
        public static Matrix4T operator *(Matrix4T left, Matrix4T right) {
            Matrix4T result = new Matrix4T();
            result.M11 = left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31 + left.M14 * right.M41;
            result.M12 = left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32 + left.M14 * right.M42;
            result.M13 = left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33 + left.M14 * right.M43;
            result.M14 = left.M11 * right.M14 + left.M12 * right.M24 + left.M13 * right.M34 + left.M14 * right.M44;
            result.M21 = left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31 + left.M24 * right.M41;
            result.M22 = left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32 + left.M24 * right.M42;
            result.M23 = left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33 + left.M24 * right.M43;
            result.M24 = left.M21 * right.M14 + left.M22 * right.M24 + left.M23 * right.M34 + left.M24 * right.M44;
            result.M31 = left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31 + left.M34 * right.M41;
            result.M32 = left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32 + left.M34 * right.M42;
            result.M33 = left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33 + left.M34 * right.M43;
            result.M34 = left.M31 * right.M14 + left.M32 * right.M24 + left.M33 * right.M34 + left.M34 * right.M44;
            result.M41 = left.M41 * right.M11 + left.M42 * right.M21 + left.M43 * right.M31 + left.M44 * right.M41;
            result.M42 = left.M41 * right.M12 + left.M42 * right.M22 + left.M43 * right.M32 + left.M44 * right.M42;
            result.M43 = left.M41 * right.M13 + left.M42 * right.M23 + left.M43 * right.M33 + left.M44 * right.M43;
            result.M44 = left.M41 * right.M14 + left.M42 * right.M24 + left.M43 * right.M34 + left.M44 * right.M44;
            return result;
        }

        /// <summary>
        /// Calculate the transpose of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to transpose</param>
        /// <returns>The transpose of the given matrix</returns>
        public static Matrix4T Transpose(Matrix4T mat) {
            return new Matrix4T(mat.Column1, mat.Column2, mat.Column3, mat.Column4);
        }

        /// <summary>
        /// Calculate the inverse of the given matrix
        /// </summary>
        /// <param name="mat">The matrix to invert</param>
        /// <returns>The inverse of the given matrix if it has one, or the input if it is singular</returns>
        /// <exception cref="InvalidOperationException">Thrown if the Matrix4 is singular.</exception>
        public static Matrix4T Invert(Matrix4T mat) {
            int[] colIdx = { 0, 0, 0, 0 };
            int[] rowIdx = { 0, 0, 0, 0 };
            int[] pivotIdx = { -1, -1, -1, -1 };

            // convert the matrix to an array for easy looping
            float[,] inverse = {{mat.Row0.X, mat.Row0.Y, mat.Row0.Z, mat.Row0.W}, 
                                {mat.Row1.X, mat.Row1.Y, mat.Row1.Z, mat.Row1.W}, 
                                {mat.Row2.X, mat.Row2.Y, mat.Row2.Z, mat.Row2.W}, 
                                {mat.Row3.X, mat.Row3.Y, mat.Row3.Z, mat.Row3.W} };
            int icol = 0;
            int irow = 0;
            for (int i = 0; i < 4; i++) {
                // Find the largest pivot value
                float maxPivot = 0.0f;
                for (int j = 0; j < 4; j++) {
                    if (pivotIdx[j] != 0) {
                        for (int k = 0; k < 4; ++k) {
                            if (pivotIdx[k] == -1) {
                                float absVal = System.Math.Abs(inverse[j, k]);
                                if (absVal > maxPivot) {
                                    maxPivot = absVal;
                                    irow = j;
                                    icol = k;
                                }
                            } else if (pivotIdx[k] > 0) {
                                return mat;
                            }
                        }
                    }
                }

                ++(pivotIdx[icol]);

                // Swap rows over so pivot is on diagonal
                if (irow != icol) {
                    for (int k = 0; k < 4; ++k) {
                        float f = inverse[irow, k];
                        inverse[irow, k] = inverse[icol, k];
                        inverse[icol, k] = f;
                    }
                }

                rowIdx[i] = irow;
                colIdx[i] = icol;

                float pivot = inverse[icol, icol];
                // check for singular matrix
                if (pivot == 0.0f) {
                    throw new InvalidOperationException("Matrix is singular and cannot be inverted.");
                    //return mat;
                }

                // Scale row so it has a unit diagonal
                float oneOverPivot = 1.0f / pivot;
                inverse[icol, icol] = 1.0f;
                for (int k = 0; k < 4; ++k)
                    inverse[icol, k] *= oneOverPivot;

                // Do elimination of non-diagonal elements
                for (int j = 0; j < 4; ++j) {
                    // check this isn't on the diagonal
                    if (icol != j) {
                        float f = inverse[j, icol];
                        inverse[j, icol] = 0.0f;
                        for (int k = 0; k < 4; ++k)
                            inverse[j, k] -= inverse[icol, k] * f;
                    }
                }
            }

            for (int j = 3; j >= 0; --j) {
                int ir = rowIdx[j];
                int ic = colIdx[j];
                for (int k = 0; k < 4; ++k) {
                    float f = inverse[k, ir];
                    inverse[k, ir] = inverse[k, ic];
                    inverse[k, ic] = f;
                }
            }
            Matrix4T result = new Matrix4T();
            result.Row0 = new Vec4(inverse[0, 0], inverse[0, 1], inverse[0, 2], inverse[0, 3]);
            result.Row1 = new Vec4(inverse[1, 0], inverse[1, 1], inverse[1, 2], inverse[1, 3]);
            result.Row2 = new Vec4(inverse[2, 0], inverse[2, 1], inverse[2, 2], inverse[2, 3]);
            result.Row3 = new Vec4(inverse[3, 0], inverse[3, 1], inverse[3, 2], inverse[3, 3]);
            return result;
        }

        public override string ToString() {
            return String.Format("{0}\n{1}\n{2}\n{3}", Row0, Row1, Row2, Row3);
        }

        public List<Vec3> Transform(List<Vec3> geom) {
            List<Vec3> result = new List<Vec3>(geom.Count);
            foreach (Vec3 v in geom) {
                result.Add(this * v);
            }
            return result;
        }

        public Matrix4T GetInverse() {
            return Invert(this);
        }
    }
}
