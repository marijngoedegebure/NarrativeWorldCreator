using System;
using System.Collections.Generic;

using System.Text;
using System.Collections.ObjectModel;

namespace Common
{
    public class Boxi
    {
        Vec3i min, max;

        public Vec3i Min { get { return min; } }
        public Vec3i Max { get { return max; } }
        public Vec3i Midpoint { get { return (max + min) / 2; } }

        public Boxi(Vec3i min, Vec3i max)
        {
            this.min = new Vec3i(min);
            this.max = new Vec3i(max);
        }

        public void AddPointToBoundingBox(Vec3i v)
        {
            for (int i = 0; i < 3; ++i)
            {
                if (v[i] < min[i])
                    min[i] = v[i];
                if (v[i] > max[i])
                    max[i] = v[i];
            }
        }

        private void AddPointToBoundingBox(Vec2i v)
        {
            for (int i2 = 0; i2 < 2; ++i2)
            {
                int i = i2 == 0 ? 0 : 2;
                if (v[i2] < min[i])
                    min[i] = v[i2];
                if (v[i2] > max[i])
                    max[i] = v[i2];
            }
        }

        public void AddBox(Boxi box)
        {
            for (int i = 0; i < 3; ++i)
            {
                if (box.min[i] < min[i])
                    min[i] = box.min[i];
                if (box.max[i] > max[i])
                    max[i] = box.max[i];
            }
        }

        public static Boxi CreateFromPointList(Vec3i[] points)
        {
            Boxi b = MaxMinBox();
            foreach (Vec3i point in points)
                b.AddPointToBoundingBox(point);
            return b;
        }

        public static Boxi CreateFromPointList(Vec2i[] points, Intervali heightInterval)
        {
            Boxi b = MaxMinBox();
            foreach (Vec2i point in points)
                b.AddPointToBoundingBox(point);
            if (heightInterval.Min < b.min[1])
                b.min[1] = heightInterval.Min;
            if (heightInterval.Max > b.max[1])
                b.max[1] = heightInterval.Max;

            return b;
        }

        public static Boxi CreateFromPointList(Vec2i[] points, int height)
        {
            return CreateFromPointList(points, new Intervali(height, height));
        }

        public static Boxi MaxMinBox()
        {
            return new Boxi(new Vec3i(int.MaxValue, int.MaxValue, int.MaxValue), new Vec3i(int.MinValue, int.MinValue, int.MinValue));
        }

        internal static Boxi CreateFromPointList(System.Collections.ObjectModel.ReadOnlyCollection<Vec2i> points, Intervali interval)
        {
            Vec2i[] array = new Vec2i[points.Count];
            for (int i = 0; i < points.Count; ++i)
                array[i] = points[i];
            return CreateFromPointList(array, interval);
        }

        internal static Boxi CreateFromPointList(System.Collections.ObjectModel.ReadOnlyCollection<Vec2i> points, int positionY)
        {
            Vec2i[] array = new Vec2i[points.Count];
            for (int i = 0; i < points.Count; ++i)
                array[i] = points[i];
            return CreateFromPointList(array, positionY);
        }

        public Box2i Flatten()
        {
            return new Box2i(new Vec2i(min.X, min.Z), new Vec2i(max.X, max.Z));
        }

        public Common.Shapes.FlatBoxi GetBottom()
        {
            return new Common.Shapes.FlatBoxi(Flatten(), min.Y);
        }
    }

    public class Box : IStringRepresentative
    {

        #region Box properties
        
        private const string MinX = "MinX";
        private const string MinY = "MinY";
        private const string MinZ = "MinZ";
        private const string MaxX = "MaxX";
        private const string MaxY = "MaxY";
        private const string MaxZ = "MaxZ";
        private const string MidX = "MidX";
        private const string MidY = "MidY";
        private const string MidZ = "MidZ";
        private const string DimX = "DimX";
        private const string DimY = "DimY";
        private const string DimZ = "DimZ";

        /// <summary>
        /// The box properties.
        /// </summary>
        public static ReadOnlyCollection<string> Properties 
        {
            get
            {
                List<string> properties = new List<string>();
                properties.Add(MinX);
                properties.Add(MinY);
                properties.Add(MinZ);
                properties.Add(MaxX);
                properties.Add(MaxY);
                properties.Add(MaxZ);
                properties.Add(MidX);
                properties.Add(MidY);
                properties.Add(MidZ);
                properties.Add(DimX);
                properties.Add(DimY);
                properties.Add(DimZ);
                return properties.AsReadOnly();
            }
        }

        #endregion Box properties

        #region Event: BoxHandler
        /// <summary>
        /// A handler for the BoxHandler event.
        /// </summary>
        /// <param name="box">The changed box.</param>
        public delegate void BoxHandler(Box box);

        /// <summary>
        /// An event for a changed value (Minimum or Maximum).
        /// </summary>
        public event BoxHandler BoxChanged;
        #endregion Event: BoxHandler

        Vec3 min, max;

        private Vec3.Vec3Handler minimumChanged, maximumChanged;

        public Vec3 Minimum
        {
            get
            {
                if (min != null && minimumChanged == null)
                {
                    minimumChanged = new Vec3.Vec3Handler(minimum_ValueChanged);
                    min.ValueChanged += minimumChanged;
                }

                return min;
            }
            set
            {
                if (minimumChanged == null)
                    minimumChanged = new Vec3.Vec3Handler(minimum_ValueChanged);

                if (min != null)
                    min.ValueChanged -= minimumChanged;

                min = value;

                if (min != null)
                    min.ValueChanged += minimumChanged;
            }
        }
        private void minimum_ValueChanged(Vec3 vector)
        {
            if (this.BoxChanged != null)
                this.BoxChanged(this);
        }
        public Vec3 Maximum
        {
            get
            {
                if (max != null && maximumChanged == null)
                {
                    maximumChanged = new Vec3.Vec3Handler(maximum_ValueChanged);
                    max.ValueChanged += maximumChanged;
                }

                return max;
            }
            set
            {
                if (maximumChanged == null)
                    maximumChanged = new Vec3.Vec3Handler(maximum_ValueChanged);

                if (max != null)
                    max.ValueChanged -= maximumChanged;

                max = value;

                if (max != null)
                    max.ValueChanged += maximumChanged;
            }
        }
        private void maximum_ValueChanged(Vec3 vector)
        {
            if (this.BoxChanged != null)
                this.BoxChanged(this);
        }

        public Vec3 Midpoint { get { return (max + min) / 2; } }
        public Vec3 Dimensions { get { return max - min; } }

        public Box(Vec3 min, Vec3 max)
        {
            this.Minimum = new Vec3(min);
            this.Maximum = new Vec3(max);
        }

        public Box(Vec3 dimensions)
            : this(-dimensions / 2, dimensions / 2)
        {
        }

        public Box(float dimX, float dimY, float dimZ)
            : this(new Vec3(dimX, dimY, dimZ))
        {
        }

        public Box(Vec3[] box)
        {
            this.Minimum = new Vec3(box[0]);
            this.Maximum = new Vec3(box[1]);
        }

        public Box(Box2 flatBox, float height)
        {
            this.Minimum = new Vec3(flatBox.min);
            this.Maximum = new Vec3(flatBox.max);
            this.Maximum.Y = height;
        }

        public Box(Box box)
            : this(box.min, box.max)
        {
        }

        public Interval HeightInterval { get { return new Interval(min.Y, max.Y); } }

        public static implicit operator Box(Box2 b)
        {
            return new Box(b.min, b.max);
        }

        public Box ScaledBox(float scale)
        {
            Vec3 min = Midpoint - 0.5f * scale * Dimensions;
            Vec3 max = Midpoint + 0.5f * scale * Dimensions;
            return new Box(min, max);
        }

        public double GetValue(string val)
        {
            switch (val)
            {
                case MinX:
                    return this.min.X;
                case MinY:
                    return this.min.Y;
                case MinZ:
                    return this.min.Z;
                case MaxX:
                    return this.max.X;
                case MaxY:
                    return this.max.Y;
                case MaxZ:
                    return this.max.Z;
                case MidX:
                    return (this.min.X + this.max.X) / 2;
                case MidY:
                    return (this.min.Y + this.max.Y) / 2;
                case MidZ:
                    return (this.min.Z + this.max.Z) / 2;
                case DimX:
                    return this.max.X - this.min.X;
                case DimY:
                    return this.max.Y - this.min.Y;
                case DimZ:
                    return this.max.Z - this.min.Z;
            }
            return 0;
        }

        public void AddBox(Box box)
        {
            for (int i = 0; i < 3; ++i)
            {
                if (box.min[i] < min[i])
                    min[i] = box.min[i];
                if (box.max[i] > max[i])
                    max[i] = box.max[i];
            }
        }

        public void Cut(Box box, List<Box> list)
        {
            List<Box> newBoxes = new List<Box>();
            newBoxes.Add(new Box(this.min, this.max));
            newBoxes = Box.Cut(newBoxes, box, Vec3.Component.X, box.min.X);
            newBoxes = Box.Cut(newBoxes, box, Vec3.Component.X, box.max.X);
            newBoxes = Box.Cut(newBoxes, box, Vec3.Component.Y, box.min.Y);
            newBoxes = Box.Cut(newBoxes, box, Vec3.Component.Y, box.max.Y);
            newBoxes = Box.Cut(newBoxes, box, Vec3.Component.Z, box.min.Z);
            newBoxes = Box.Cut(newBoxes, box, Vec3.Component.Z, box.max.Z);

            List<Box> temp = new List<Box>();
            foreach (Box b in newBoxes)
            {
                if (!box.Contains(b.Midpoint))
                    temp.Add(b);
            }

            temp = MergeBack(temp);
            foreach (Box b in temp)
                list.Add(b);
            return;
        }

        private List<Box> MergeBack(List<Box> temp)
        {
            bool changed;
            do
            {
                temp = MergeBack(temp, out changed, 1);
            } while (changed);
            do
            {
                temp = MergeBack(temp, out changed, 0);
            } while (changed);
            do
            {
                temp = MergeBack(temp, out changed, 2);
            } while (changed);

            return temp;
        }

        private List<Box> MergeBack(List<Box> temp, out bool isChanged, int comp)
        {
            List<Box> newList = new List<Box>();
            Box changed = null;
            for (int i = 0; i < temp.Count; ++i)
            {
                if (changed == null)
                {
                    for (int j = 0; j < temp.Count && changed == null; ++j)
                    {
                        if (i != j)
                        {
                            bool diff = false;
                            for (int k = 0; k < 3; ++k)
                            {
                                if (k != comp && (temp[i].min[k] != temp[j].min[k] || temp[i].max[k] != temp[j].max[k]))
                                    diff = true;
                            }
                            if (!diff && temp[j].min[comp] == temp[i].max[comp])
                            {
                                changed = temp[j];
                                newList.Add(new Box(temp[i].min, temp[j].max));
                            }
                        }
                    }
                    if (changed == null)
                        newList.Add(temp[i]);
                }
                else if (changed != temp[i])
                    newList.Add(temp[i]);
            }
            isChanged = changed != null;
            return newList;
        }

        public bool Contains(Vec3 v)
        {
            return v.X >= min.X && v.X <= max.X && v.Y >= min.Y && v.Y <= max.Y && v.Z >= min.Z && v.Z <= max.Z;
        }

        internal bool Contains(Vec3 v, double xzRadius)
        {
            return v.X >= min.X - xzRadius && v.X <= max.X + xzRadius &&
                    v.Y >= min.Y && v.Y <= max.Y &&
                    v.Z >= min.Z - xzRadius && v.Z <= max.Z + xzRadius;
        }

        private static List<Box> Cut(List<Box> list, Box toCut, Vec3.Component c, float val)
        {
            List<Box> newList = new List<Box>();
            foreach (Box b in list)
                b.Cut(toCut, c, val, newList);
            return newList;
        }

        private void Cut(Box toCut, Vec3.Component c, float val, List<Box> newList)
        {
            if (val <= min.GetComponent(c) || val >= max.GetComponent(c))
            {
                newList.Add(new Box(min, max));
                return;
            }
            Box b1 = new Box(min, max);
            b1.max.SetComponent(c, val);
            Box b2 = new Box(min, max);
            b2.min.SetComponent(c, val);
            newList.Add(b1);
            newList.Add(b2);
        }

        public void AddPointToBoundingBox(Vec3 v)
        {
            for (int i = 0; i < 3; ++i)
            {
                if (v[i] < min[i])
                    min[i] = v[i];
                if (v[i] > max[i])
                    max[i] = v[i];
            }
        }

        public void Add2dPointToBoundingBox(Vec2 v)
        {
            for (int i = 0; i < 3; ++i)
            {
                int j = i;
                if (i == 2)
                    j = 1;
                if (i != 1)
                {
                    if (v[j] < min[i])
                        min[i] = v[j];
                    if (v[j] > max[i])
                        max[i] = v[j];
                }
            }
        }

        public static Box FromMidpointAndDimensions(Vec3 midpoint, Vec3 dimensions)
        {
            Vec3 halfDimensions = dimensions / 2;
            return new Box(midpoint - halfDimensions, midpoint + halfDimensions);
        }

        public void Move(Vec3 p)
        {
            min += p;
            max += p;
        }

        public Vec3 GetRandomPosition(Random r)
        {
            float x = (float)r.NextDouble() * (max.X - min.X) + min.X;
            float y = (float)r.NextDouble() * (max.Y - min.Y) + min.Y;
            float z = (float)r.NextDouble() * (max.Z - min.Z) + min.Z;
            return new Vec3(x, y, z);
        }

        public void RotateY(float p)
        {
            Vec3 newDim = new Vec3(Dimensions);
            newDim.RotateY(p);
            newDim.Abs();
            Vec3 newMid = new Vec3(Midpoint);
            newMid.RotateY(p);
            Box b = FromMidpointAndDimensions(newMid, newDim);
            this.Minimum = b.min;
            this.Maximum = b.max;
        }

        public static List<Box> Intersection(List<Box> b1, List<Box> b2)
        {
            List<Box> inters = new List<Box>();
            foreach (Box b in b1)
            {
                foreach (Box bb in b2)
                {
                    if (b != bb)
                    {
                        b.Intersection(bb, ref inters);
                    }
                }
            }
            return inters;
        }

        public Vec3[, ,] GetPoints()
        {
            Vec3[, ,] list = new Vec3[2, 2, 2];
            int[] t = new int[3];
            for (t[0] = 0; t[0] < 2; ++t[0])
                for (t[1] = 0; t[1] < 2; ++t[1])
                    for (t[2] = 0; t[2] < 2; ++t[2])
                    {
                        float[] temp = new float[3];
                        for (int i = 0; i < 3; ++i)
                            temp[i] = this.Minimum[i] + (float)t[i] * this.Dimensions[i];
                        list[t[0], t[1], t[2]] = new Vec3(temp);
                    }
            return list;
        }

        private void Intersection(Box bb, ref List<Box> inters)
        {
            Vec3 newmin = new Vec3();
            Vec3 newmax = new Vec3();
            for (int i = 0; i < 3; ++i)
            {
                if (min[i] > bb.max[i] || bb.min[i] > max[i])
                    return;
                min[i] = Math.Max(min[i], bb.min[i]);
                max[i] = Math.Min(max[i], bb.max[i]);
                if (min[i] == max[i])
                    return;
            }
            inters.Add(new Box(min, max));
        }

        public double GetBoundingRadius()
        {
            double minX = min.X; minX *= minX;
            double minZ = min.Z; minZ *= minZ;
            double maxX = max.X; maxX *= maxX;
            double maxZ = max.Z; maxZ *= maxZ;
            return Math.Sqrt(Math.Max(minX + minZ, Math.Max(minX + maxZ, Math.Max(maxX + minZ, maxX + maxZ))));
        }

        public double GetMinimalXZRadius()
        {
            if (min.X < 0 && max.X > 0 && min.Z < 0 && max.Z > 0)
            {
                return Math.Min(
                        Math.Min(Math.Abs(min.X), max.X),
                        Math.Min(Math.Abs(min.Z), max.Z)
                    );
            }
            Vec3 dims = Dimensions;
            return Math.Min(dims.X, dims.Z) / 2;
        }

        public Line2 Cut(Line2 l)
        {
            Line2 l1 = new Line2(new Vec2(this.min.X, this.min.Z), new Vec2(this.min.X, this.max.Z));
            Line2 l2 = new Line2(new Vec2(this.max.X, this.min.Z), new Vec2(this.max.X, this.max.Z));
            Line2 l3 = new Line2(new Vec2(this.min.X, this.min.Z), new Vec2(this.max.X, this.min.Z));
            Line2 l4 = new Line2(new Vec2(this.min.X, this.max.Z), new Vec2(this.max.X, this.max.Z));
            List<Vec2> points = new List<Vec2>();
            Vec2 v = null;
            bool dummy;
            if ((v = l1.IntersectionOnLine(l, out dummy)) != null)
                points.Add(v);
            if ((v = l2.IntersectionOnLine(l, out dummy)) != null)
                points.Add(v);
            if ((v = l3.IntersectionOnLine(l, out dummy)) != null)
                points.Add(v);
            if ((v = l4.IntersectionOnLine(l, out dummy)) != null)
                points.Add(v);
            if (points.Count != 2)
                throw new Exception("Jefke sucks!");
            return new Line2(points[0], points[1]);
        }

        public bool Intersects(Box otherBox)
        {
            if (otherBox != null &&
                ((this.min[0] >= otherBox.min[0] && this.min[0] <= otherBox.max[0] &&
                  this.min[1] >= otherBox.min[1] && this.min[1] <= otherBox.max[1] &&
                  this.min[2] >= otherBox.min[2] && this.min[2] <= otherBox.max[2]) ||
                 (otherBox.min[0] >= this.min[0] && otherBox.min[0] <= this.max[0] &&
                  otherBox.min[1] >= this.min[1] && otherBox.min[1] <= this.max[1] &&
                  otherBox.min[2] >= this.min[2] && otherBox.min[2] <= this.max[2])))
                return true;

            return false;
        }

        public bool Intersects(Line line)
        {
            double st, et, fst = 0, fet = 1;
            Vec3 s = line.P1;
            Vec3 e = line.P2;

            for (int i = 0; i < 3; i++)
            {
                double si = s[i]; double ei = e[i];
                double mini = min[i]; double maxi = max[i];
                if (si < ei)
                {
                    if (si > maxi || ei < mini)
                        return false;
                    double di = ei - si;
                    st = (si < mini) ? (mini - si) / di : 0;
                    et = (ei > maxi) ? (maxi - si) / di : 1;
                }
                else
                {
                    if (ei > maxi || si < mini)
                        return false;
                    double di = ei - si;
                    st = (si > maxi) ? (maxi - si) / di : 0;
                    et = (ei < mini) ? (mini - si) / di : 1;
                }

                if (st > fst) fst = st;
                if (et < fet) fet = et;
                if (fet < fst)
                    return false;
            }
            return true;
        }

        public override string ToString()
        {
            return "Box(" + min + "-" + max + ")";
        }

        public static Box Load(System.IO.BinaryReader r)
        {
            Vec3 min = Vec3.Load(r), max = Vec3.Load(r);
            return new Box(min, max);
        }

        public void Save(System.IO.BinaryWriter w)
        {
            min.Save(w);
            max.Save(w);
        }

        #region Method: LoadFromString(string saveString)
        /// <summary>
        /// Load the box from the given string.
        /// </summary>
        /// <param name="saveString">The string to load a box from.</param>
        /// <returns>The created box.</returns>
        public static Box LoadFromString(string saveString)
        {
            Box ret = null;
            ret.LoadString(saveString);
            return ret;
        }
        #endregion Method: LoadFromString(string saveString)

        /// <summary>
        /// Create a string with all class data.
        /// </summary>
        /// <returns>A string with all class data.</returns>
        public string CreateString()
        {
            return this.Minimum.CreateString() + ":" + this.Maximum.CreateString();
        }

        /// <summary>
        /// Load the class data from the given string.
        /// </summary>
        /// <param name="stringToLoad">The string that contains the class data.</param>
        public void LoadString(string stringToLoad)
        {
            try
            {
                string[] values = stringToLoad.Split(':');
                if (values.Length == 2)
                {
                    this.Minimum = Vec3.LoadFromString(values[0]);
                    this.Maximum = Vec3.LoadFromString(values[1]);
                }
            }
            catch (Exception)
            {
            }
        }

    }
}
