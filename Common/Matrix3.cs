using System;
using System.Collections.Generic;

using System.Text;

namespace Common
{
    public class Matrix3
    {
        float[] matrix = new float[9];
        static float[] cofs = { 1, -1, 1, -1, 1, -1, 1, -1, 1 };

        public float m00
        {
            get { return matrix[0]; }
            set { matrix[0] = value; }
        }
        public float m01
        {
            get { return matrix[1]; }
            set { matrix[1] = value; }
        }
        public float m02
        {
            get { return matrix[2]; }
            set { matrix[2] = value; }
        }
        public float m10
        {
            get { return matrix[3]; }
            set { matrix[3] = value; }
        }
        public float m11
        {
            get { return matrix[4]; }
            set { matrix[4] = value; }
        }
        public float m12
        {
            get { return matrix[5]; }
            set { matrix[5] = value; }
        }
        public float m20
        {
            get { return matrix[6]; }
            set { matrix[6] = value; }
        }
        public float m21
        {
            get { return matrix[7]; }
            set { matrix[7] = value; }
        }
        public float m22
        {
            get { return matrix[8]; }
            set { matrix[8] = value; }
        }

        public float this[int row, int col]
        {
            get { return matrix[3 * row + col]; }
            set { matrix[3 * row + col] = value; }
        }

        private float Element(int row, int col) { return GetElement(row % 3, col % 3); }
        public float GetElement(int row, int col) { return matrix[col + row * 3]; }
        public void SetElement(int row, int col, float value) { matrix[col + row * 3] = value; }

        public Matrix3() { for (int i = 0; i < 9; ++i) matrix[i] = 0; }
        public Matrix3(Vec3 v1, Vec3 v2, Vec3 v3)
        {
            matrix[0] = v1.X; matrix[1] = v2.X; matrix[2] = v3.X;
            matrix[3] = v1.Y; matrix[4] = v2.Y; matrix[5] = v3.Y;
            matrix[6] = v1.Z; matrix[7] = v2.Z; matrix[8] = v3.Z;
        }
        public Matrix3(params float[] parameters)
        {
            for (int i = 0; i < 9; ++i)
                matrix[i] = parameters[i];
        }

        public float Determinant()
        {
            //--- regel van sarrus: hoofddiagonalen - nevendiagonalen
            float sum = 0;
            for (int i = 0; i < 3; ++i)
            {
                float main = 1;
                float next = 1;
                for (int j = 0; j < 3; ++j)
                {
                    main *= Element(j, i + j);
                    next *= Element(j, i + 2 - j);
                }
                sum += main - next;
            }
            return sum;
        }

        public static Vec3 operator* (Matrix3 matrix, Vec3 vector)
        {
            return new Vec3(matrix.matrix[0] * vector.X + matrix.matrix[1] * vector.Y + matrix.matrix[2] * vector.Z,
                            matrix.matrix[3] * vector.X + matrix.matrix[4] * vector.Y + matrix.matrix[5] * vector.Z,
                            matrix.matrix[6] * vector.X + matrix.matrix[7] * vector.Y + matrix.matrix[8] * vector.Z);
        }

        public static Matrix3 operator *(float a, Matrix3 matrix)
        {
            float[] elements = new float[9];
            for (int i = 0; i < 9; ++i)
                elements[i] = matrix.matrix[i];
            return new Matrix3(elements);
        }


        public Matrix3 Inverse()
        {
            //logCounterInverse.Start();
            Matrix3 adjunct = Adjunct();
            Matrix3 ret = (1 / Determinant()) * adjunct;
            //logCounterInverse.Stop();
            return ret;
        }

        private Matrix3 Adjunct()
        {
            float[] t = new float[9];
            for (int r = 0; r < 3; ++r)
            {
                for (int c = 0; c < 3; ++c)
                {
                    // normaal: matrix[col + row * 3] => in geinverteerde: matrix[row + col * 3]
                    t[r + c * 3] = Cofactor(r, c);
                }
            }
            return new Matrix3(t);
        }

        private float Cofactor(int row, int column)
        {
            return cofs[row + column] * GetDeterminantWithoutColumnAndRow(row, column);
        }

        private float GetDeterminantWithoutColumnAndRow(int row, int column)
        {
            int r1 = row > 0 ? 0 : 1;
            int r2 = row != 1 ? 1 : 2;
            int c1 = column > 0 ? 0 : 1;
            int c2 = column != 1 ? 1 : 2;
            return GetElement(r1, c1) * GetElement(r2, c2) - GetElement(r1, c2) * GetElement(r2, c1);
        }
    }
}
