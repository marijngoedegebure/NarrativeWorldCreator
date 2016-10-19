using System;
using System.Collections.Generic;

using System.Text;
using System.IO;
using Common.Util;
using System.ComponentModel;

namespace Common.Geometry
{
    public abstract class MaterialMap : INotifyPropertyChanged
    {
        internal class Comparer : IComparer<MaterialMap>
        {
            #region IComparer<MaterialMap> Members

            public int Compare(MaterialMap x, MaterialMap y)
            {
                return ((int)x.mapType).CompareTo((int)y.mapType);
            }

            #endregion
        }

        public enum MapType { Diffuse = 0, Normal = 1, Specular = 2, Displacement = 3, Bump = 4, Reflection = 5, Unspecified = 6 };
        MapType mapType;
        public MapType Type { 
            get { return mapType; } 
            set 
            { 
                mapType = value;
                HandlePropertyChange("Type");
            }
        }
        public bool createMipmaps = true;

        public static bool operator ==(MaterialMap m1, MaterialMap m2)
        {
            if ((object)m1 == null)
            {
                if ((object)m2 == null)
                    return true;
                return false;
            }
            if ((object)m2 == null)
                return false;

            if (m1 is FileMaterialMap)
            {
                if (m2 is FileMaterialMap)
                    return ((FileMaterialMap)m1).MapFilename == ((FileMaterialMap)m2).MapFilename;
                else
                    return false;
            }
            else if (m1 is ArrayMaterialMap)
            {
                if (m2 is ArrayMaterialMap)
                    return ((ArrayMaterialMap)m1).IsArrayEqual((ArrayMaterialMap)m2);
                else
                    return false;
            }
            else if (m1 is DynamicMaterialMap)
            {
                return (object)m1 == (object)m2;
            }
            else
                throw new NotImplementedException();

        }

        public static bool operator !=(MaterialMap m1, MaterialMap m2)
        {
            return !(m1 == m2);
        }

        public override bool Equals(object obj)
        {
            return this == (MaterialMap)obj;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        protected MaterialMap(MapType type)
        {
            mapType = type;
        }

        protected MaterialMap(BinaryReader r)
        {
            mapType = (MapType)r.ReadByte();
        }

        internal static MaterialMap Clone(MaterialMap map)
        {
            if (map is FileMaterialMap)
                return new FileMaterialMap((FileMaterialMap)map);
            else if (map is ArrayMaterialMap)
                return new ArrayMaterialMap((ArrayMaterialMap)map);
            else if (map is DynamicMaterialMap)
                return new DynamicMaterialMap((DynamicMaterialMap)map);
            else
                throw new NotImplementedException();
        }

        internal abstract void Save(BinaryWriter wr);

        protected void SaveBase(BinaryWriter wr)
        {
            wr.Write((byte)mapType);
        }

        internal static MaterialMap Load(BinaryReader r)
        {
            switch (r.ReadChar())
            {
                case 'f':
                    return new FileMaterialMap(r);
                case 'a':
                    return new ArrayMaterialMap(r);
                default:
                    throw new NotImplementedException();
            }
        }

        public new abstract string GetType();

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected void HandlePropertyChange(string p)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(p));
        }
    }

    public class FileMaterialMap : MaterialMap
    {
        private string m_mapFilename;
        public string MapFilename
        {
            get { return m_mapFilename; }
            set
            {
                m_mapFilename = value;
                base.HandlePropertyChange("MapFilename");
            }
        }

        public FileMaterialMap(string filename, MapType type)
            : base(type)
        {
            MapFilename = filename;
        }

        internal FileMaterialMap(BinaryReader r)
            : base(r)
        {
            this.MapFilename = r.ReadString();
        }

        public FileMaterialMap(FileMaterialMap copy)
            : base(copy.Type)
        {
            MapFilename = copy.MapFilename;
        }

        internal override void Save(BinaryWriter wr)
        {
            wr.Write('f');
            SaveBase(wr);
            wr.Write(MapFilename);
        }

        public override string GetType()
        {
            return "file";
        }
    }

    public class ArrayMaterialMap : MaterialMap
    {
        public readonly byte[] colorArray;
        public readonly int width, height;

        private ArrayMaterialMap(byte[] array, int width, int height, MapType type)
            : base(type)
        {
            this.colorArray = array;
            this.width = width;
            this.height = height;
            if (colorArray.Length != width * height * 4)
                throw new Exception("An ArrayMaterialMap should contain an array of the form RGBA: width * height * 4 bytes");
        }

        internal ArrayMaterialMap(BinaryReader r)
            : base(r)
        {
            this.width = r.ReadInt32();
            this.height = r.ReadInt32();
            this.colorArray = new byte[width * height * 4];
            for (int i = 0; i < width * height * 4; ++i)
                colorArray[i] = r.ReadByte();
        }

        public static ArrayMaterialMap CreateRGBMaterial(byte[] rgbData, int width, int height, MapType mapType)
        {
            if (rgbData.Length != width * height * 3)
                throw new Exception("An rgb material should contain an array of the form RGB: width * height * 3 bytes");
            byte[] rgbaData = new byte[width * height * 4];
            for (int i = 0; i < width * height; ++i)
            {
                for (int j = 0; j < 3; ++j)
                    rgbaData[i * 4 + j] = rgbData[i * 3 + j];
                rgbaData[i * 4 + 3] = 255;
            }
            return new ArrayMaterialMap(rgbaData, width, height, mapType);
        }

        public static ArrayMaterialMap CreateRGBAMaterial(byte[] rgbaData, int width, int height, MapType mapType)
        {
            if (rgbaData.Length != width * height * 4)
                throw new Exception("An rgb material should contain an array of the form RGBA: width * height * 4 bytes");
            return new ArrayMaterialMap(rgbaData, width, height, mapType);
        }

        public static ArrayMaterialMap CreateRGBAMaterialFromVec4List(List<Vec4> colorMap, int width, int height, MapType mapType)
        {
            if (colorMap.Count != width * height)
                throw new Exception("An rgba material should contain a list of width * height Vec4's");
            byte[] array = new byte[width * height * 4];
            int count = 0;
            foreach (Vec4 v in colorMap)
            {
                array[count++] = (byte)(v.X * 255.0);
                array[count++] = (byte)(v.Y * 255.0);
                array[count++] = (byte)(v.Z * 255.0);
                array[count++] = (byte)(v.W * 255.0);
            }
            return new ArrayMaterialMap(array, width, height, mapType);
        }

        public ArrayMaterialMap(ArrayMaterialMap copy)
            : this(copy.colorArray, copy.width, copy.height, copy.Type)
        {
        }

        internal bool IsArrayEqual(ArrayMaterialMap copy)
        {
            if (width != copy.width || height != copy.height)
                return false;
            for (int i = 0; i < colorArray.Length; ++i)
                if (colorArray[i] != copy.colorArray[i])
                    return false;
            return true;
        }

        internal override void Save(BinaryWriter wr)
        {
            wr.Write('a');
            SaveBase(wr);
            wr.Write(width);
            wr.Write(height);
            for (int i = 0; i < colorArray.Length; ++i)
                wr.Write(colorArray[i]);
        }

        public override string GetType()
        {
            return "array";
        }

        public static ArrayMaterialMap CreatePerlinNoiseMap(PerlinNoise perlinNoise, int textureSize, Random random, MapType mapType)
        {
            int displacementX = random.Next() / 32;
            int displacementY = random.Next() / 32;
            List<Vec4> noise = new List<Vec4>();
            for (int x = 0; x < textureSize; ++x)
            {
                for (int y = 0; y < textureSize; ++y)
                {
                    float val = Math.Max(0, Math.Min(1, 0.5f * ((float)perlinNoise.Function2D(x + displacementX, y + displacementY) + 1)));
                    noise.Add(new Vec4(val, val, val, 1));
                }
            }
            return CreateRGBAMaterialFromVec4List(noise, textureSize, textureSize, mapType);
        }
    }

    public class DynamicMaterialMap : MaterialMap
    {
        public class UpdateData
        {
            public int x, y, width, height;
            public int changedXMin = int.MaxValue, changedYMin = int.MaxValue,
                        changedXMax = int.MinValue, changedYMax = int.MinValue;
            public byte[] colorArray;

            public UpdateData(int x, int y, int width, int height, byte[] colorArray)
            {
                this.x = x;
                this.y = y;
                this.width = width;
                this.height = height;
                this.colorArray = colorArray;
            }
        }

        public delegate void UpdateHandler(DynamicMaterialMap sender, UpdateData update);
        public event UpdateHandler Updated;

        public readonly int width, height;
        public byte[] colorData;

        UpdateData lockedData = null;
        int lockX, lockY, lockWidth, lockHeight;

        public DynamicMaterialMap(int width, int height, byte[] colorData, MapType type)
            : base(type)
        {
            this.width = width;
            this.height = height;
            this.colorData = colorData;
        }

        public DynamicMaterialMap(DynamicMaterialMap copy)
            : base(copy.Type)
        {
            this.width = copy.width;
            this.height = copy.height;
            this.colorData = copy.colorData;
            this.Updated += copy.Updated;
        }

        public void Refresh()
        {
            Updated(this, new UpdateData(0, 0, width, height, colorData));
        }

        private void Update(UpdateData update)
        {
            if (update.changedXMin <= update.changedXMax)
            {
                for (int yy = lockY; yy < lockY + lockHeight; ++yy)
                {
                    int source = (yy - lockY) * lockWidth * 4;
                    int dest = this.width * yy * 4 + lockX * 4;
                    Array.Copy(update.colorArray, source, colorData, dest, lockWidth * 4);
                }
                if (Updated != null)
                    Updated(this, update);
            }
        }

        public void LockArea(int x, int y, int width, int height)
        {
            if (lockedData != null)
                throw new Exception("A dynamic material cannot be locked again before being unlocked!");
            byte[] currentData = new byte[width * height * 4];
            for (int yy = y; yy < y + height; ++yy)
            {
                int source = this.width * yy * 4 + x * 4;
                int dest = (yy - y) * width * 4;
                Array.Copy(this.colorData, source, currentData, dest, width * 4);
            }
            lockedData = new UpdateData(x, y, width, height, currentData);
            lockX = x;
            lockY = y;
            lockWidth = width;
            lockHeight = height;
        }

        public void SetRGBA(int x, int y, byte r, byte g, byte b, byte a)
        {
            if (lockedData == null)
                throw new Exception("A value cannot be changed when an area is not locked!");
            if (x < lockX || y < lockY || x >= lockX + lockWidth || y >= lockY + lockHeight)
                throw new Exception("A value cannot be changed outside the locked area!");
            int index = lockWidth * 4 * (y - lockY) + (x - lockX) * 4;
            bool changed = false;
            if (lockedData.colorArray[index] != r)
            {
                lockedData.colorArray[index] = r;
                changed = true;
            }
            index++;
            if (lockedData.colorArray[index] != g)
            {
                lockedData.colorArray[index] = g;
                changed = true;
            }
            index++;
            if (lockedData.colorArray[index] != b)
            {
                lockedData.colorArray[index] = b;
                changed = true;
            }
            index++;
            if (lockedData.colorArray[index] != a)
            {
                lockedData.colorArray[index] = a;
                changed = true;
            }
            if (changed)
            {
                if (x < lockedData.changedXMin)
                    lockedData.changedXMin = x;
                if (x > lockedData.changedXMax)
                    lockedData.changedXMax = x;
                if (y < lockedData.changedYMin)
                    lockedData.changedYMin = y;
                if (y > lockedData.changedYMax)
                    lockedData.changedYMax = y;
            }
        }

        public void UnlockArea()
        {
            if (lockedData == null)
                throw new Exception("There is no area locked, so you cannot unlock it!");
            Update(lockedData);
            lockedData = null;
        }

        internal override void Save(BinaryWriter wr)
        {
            throw new NotImplementedException();
        }

        public override string GetType()
        {
            return "dynamic";
        }

        public void ClearData(byte r, byte g, byte b, byte a)
        {
            for (int i = 0; i < width * height; ++i)
            {
                int index = i * 4;
                colorData[index++] = r;
                colorData[index++] = g;
                colorData[index++] = b;
                colorData[index++] = a;
                if (Updated != null)
                    Updated(this, new UpdateData(0, 0, width, height, colorData));
            }
        }
    }
}
