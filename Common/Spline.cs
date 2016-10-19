using System;
using System.Collections.Generic;
using System.Text;

namespace Common {
    public class Spline {

        public int Count {
            get { return midLine.Count; }
        }

        private List<Vec2d> midLine;
        private List<Vec2d> sideA;
        private List<Vec2d> sideB;
        private List<double> elevation;
        // TODO bounding box

        public Spline() {
            this.midLine = new List<Vec2d>();
            this.sideA = new List<Vec2d>();
            this.sideB = new List<Vec2d>();
            this.elevation = new List<double>();
        }

        public void AddSegment(Vec2d mid, Vec2d a, Vec2d b, double elevation) {
			if (double.IsNaN(mid.X) || double.IsNaN(a.X) || double.IsNaN(b.X)) {
				throw new Exception("Trying to add illegal segment to spline");
			}
			this.midLine.Add(mid);
            this.sideA.Add(a);
            this.sideB.Add(b);
            this.elevation.Add(elevation);
        }

        public void InsertSegment(int index, Vec2d mid, Vec2d a, Vec2d b, double elevation) {
            if (index >= 0 && index < Count) {
                // Append and swap to correct position
                AddSegment(mid, a, b, elevation);
                for (int i = this.Count - 1; i > index; i--) {
                    int j = i - 1;
                    Vec2d iMid, iA, iB, jMid, jA, jB;
                    double iElevation, jElevation;
                    GetSegment(i, out iMid, out iA, out iB, out iElevation);
                    GetSegment(j, out jMid, out jA, out jB, out jElevation);
                    SetSegment(i, jMid, jA, jB, jElevation);
                    SetSegment(j, iMid, iA, iB, iElevation);
                }
            }
        }

        public void SetSegment(int index, Vec2d mid, Vec2d a, Vec2d b, double elevation) {
            if (index >= 0 && index < Count) {
                this.midLine[index] = mid;
                this.sideA[index] = a;
                this.sideB[index] = b;
                this.elevation[index] = elevation;
            } 
        }

        public void GetSegment(int index, out Vec2d mid, out Vec2d a, out Vec2d b, out double elevation) {
            if (index >= 0 && index < Count) {
                mid = this.midLine[index];
                a = this.sideA[index];
                b = this.sideB[index];
                elevation = this.elevation[index];
            } else {
                mid = a = b = null;
                elevation = 0.0;
            }
        }

        /// <summary>
        /// Gets the three points of a segment at the given index in an array.
        /// </summary>
        /// <param name="index">segment index</param>
        /// <param name="points">{left, middle, right}</param>
        public void GetSegment(int index, out Vec2d[] points) {
            if (index >= 0 && index < Count) {
                points = new Vec2d[3];
                points[0] = this.sideA[index];
                points[1] = this.midLine[index];
                points[2] = this.sideB[index];
            } else {
                points = null;
            }
        }

        public void GetSegment(int index, out Vec2d mid, out Vec2d a, out Vec2d b) {
            if (index >= 0 && index < Count) {
                mid = this.midLine[index];
                a = this.sideA[index];
                b = this.sideB[index];
            } else {
                mid = a = b = null;
            }
        }

        public void GetSegment(int index, out Vec2d a, out Vec2d b) {
            if (index >= 0 && index < Count) {
                a = this.sideA[index];
                b = this.sideB[index];
            } else {
                a = b = null;
            }
        }

        public void GetSegment(int index, out Vec2d a, out Vec2d b, out double elevation) {
            if (index >= 0 && index < Count) {
                a = this.sideA[index];
                b = this.sideB[index];
                elevation = this.elevation[index];
            } else {
                a = b = null;
                elevation = 0.0;
            }
        }

        public void GetSegment(int index, out Vec2d mid) {
            if (index >= 0 && index < Count) {
                mid = this.midLine[index];
            } else {
                mid = null;
            }
        }

        public Spline Reverse() {
            Spline result = new Spline();
            for (int i = this.Count - 1; i >= 0; i--) {
                double e;
                Vec2d m, a, b;
                GetSegment(i, out m, out a, out b, out e);
                result.AddSegment(m, b, a, e);
            }
            return result;
        }

        public List<Vec2d> GetBoundingPolygon() {
            List<Vec2d> result = new List<Vec2d>(2 * this.Count + 2);
            result.Add(this.midLine[0]);
            result.AddRange(sideA);
            result.Add(this.midLine[this.Count - 1]);
            result.AddRange(sideB);
            result.Reverse(this.Count + 2, this.Count);
            return result;
        }

        public List<Vec3d> GetBoundingPolygon3D() {
            List<Vec3d> result = new List<Vec3d>(2 * this.Count + 2);
            result.Add(new Vec3d(this.midLine[0], this.elevation[0]));
            for (int i = 0; i < this.Count; i++) {
                result.Add(new Vec3d(this.sideA[i], this.elevation[i]));
            }
            result.Add(new Vec3d(this.midLine[this.Count - 1], this.elevation[this.Count - 1]));
            for (int i = this.Count - 1; i >= 0; i--) {
                result.Add(new Vec3d(this.sideB[i], this.elevation[i]));
            }
            return result;
        }
    }
}