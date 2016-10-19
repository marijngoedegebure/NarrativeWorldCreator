using System;
using System.Collections.Generic;

using System.Text;

namespace Common
{
    public class DataQuadTree
    {
        public class Node
        {
            internal List<CustomTuple<short, float>> data;
            internal Node[] children;
            internal float halfSize;

            public float Size { get { return 2.0f * halfSize; } }
            public Node Child00 { get { return children[0]; } }
            public Node Child01 { get { return children[1]; } }
            public Node Child10 { get { return children[2]; } }
            public Node Child11 { get { return children[3]; } }

            internal Node()
            {
                data = new List<CustomTuple<short, float>>(1);
                children = null;
                halfSize = 0;
            }

            internal void Create(float halfSize)
            {
                this.halfSize = halfSize;
            }

            internal void Reset()
            {
                data.Clear();
                if (children != null)
                {
                    foreach (Node c in children)
                        c.Dispose();
                    children = null;
                }
            }

            internal void Dispose()
            {
                DataQuadTree.AddToAvailableNodes(this);
            }

            public bool TryGetValue(short type, out float value)
            {
                foreach (CustomTuple<short, float> t in data)
                {
                    if (t.Item1 == type)
                    {
                        value = t.Item2;
                        return true;
                    }
                }
                value = 0.0f;
                return false;
            }

            internal float GetValue(short type, float x, float y)
            {
                float value;
                if (TryGetValue(type, out value))
                    return value;
                float xMin, yMin;
                int quadrant = GetQuadrant(x, y, out xMin, out yMin);
                return children[quadrant].GetValue(type, x - xMin, y - yMin);
            }

            private int GetQuadrant(float x, float y, out float xMin, out float yMin)
            {
                if (x < halfSize && y < halfSize)
                {
                    xMin = 0; yMin = 0;
                    return 0;
                }
                else if (x < halfSize && y >= halfSize)
                {
                    xMin = 0; yMin = halfSize;
                    return 1;
                }
                else if (y < halfSize)
                {
                    xMin = halfSize; yMin = 0;
                    return 2;
                }
                xMin = halfSize; yMin = halfSize;
                return 3;
            }

            internal void SetValue(short type, float x, float y, float newValue, float? oldValue, short depthToGo)
            {
                bool divide = false;
                float value;
                if (TryGetValue(type, out value))
                {
                    if (value != newValue)
                    {
                        if (depthToGo == 1)
                            GetDataTuple(type).Item2 = newValue;
                        else
                        {
                            oldValue = RemoveData(type);
                            divide = true;
                        }
                    }
                }
                else
                {
                    if (depthToGo == 1)
                        data.Add(new CustomTuple<short, float>(type, newValue));
                    else
                        divide = true;
                }
                if (divide)
                {
                    if (children == null)
                    {
                        children = new Node[4];
                        for (int i = 0; i < 4; ++i)
                            DataQuadTree.Create(this, i);
                    }
                    float?[] vals = new float?[] { oldValue, oldValue, oldValue, oldValue };
                    float xMin, yMin;
                    int quadrant = GetQuadrant(x, y, out xMin, out yMin);
                    vals[quadrant] = newValue;

                    for (int i = 0; i < 4; ++i)
                        if (vals[i] != null)
                            children[i].SetValue(type, x - xMin, y - yMin, (float)vals[i], oldValue, (short)(i == quadrant ? depthToGo - 1 : 1));
                }
            }

            private bool AllSame(short type, out float sameVal)
            {
                float same = float.NaN;

                float val;
                if (TryGetValue(type, out val))
                {
                    sameVal = val;
                    return true;
                }

                foreach (Node c in children)
                {
                    float temp;
                    if (!c.AllSame(type, out temp))
                    {
                        sameVal = float.NaN;
                        return false;
                    }
                    if (float.IsNaN(same))
                        same = temp;
                    else if (same != temp)
                    {
                        sameVal = float.NaN;
                        return false;
                    }
                }
                sameVal = same;
                return true;
            }

            private CustomTuple<short, float> GetDataTuple(short type)
            {
                foreach (CustomTuple<short, float> t in data)
                    if (t.Item1 == type)
                        return t;
                return null;
            }

            private float RemoveData(short type)
            {
                foreach (CustomTuple<short, float> t in data)
                {
                    if (t.Item1 == type)
                    {
                        data.Remove(t);
                        return t.Item2;
                    }
                }
                throw new Exception("Trying to remove data that is not available!");
            }

            internal int CountVal(short type, float val)
            {
                float value;
                if (TryGetValue(type, out value))
                {
                    if (Math.Abs(value - val) < float.Epsilon)
                        return (int)(halfSize * 2 * halfSize * 2);
                    else
                        return 0;
                }
                int ret = 0;
                foreach (Node c in children)
                    ret += c.CountVal(type, val);
                return ret;
            }

            internal void Optimize(short type)
            {
                float sameVal;
                if (children != null)
                {
                    foreach (Node c in children)
                        c.Optimize(type);
                } 
                if (children != null && AllSame(type, out sameVal))
                {
                    foreach (Node c in children)
                        c.Dispose();
                    children = null;
                    data.Add(new CustomTuple<short, float>(type, sameVal));
                }
            }
        }

        private static List<Node> availableNodes = new List<Node>();

        private static Node Create(Node parent, int index)
        {
            if (availableNodes.Count == 0)
                FillAvailableNodes();

            Node n = availableNodes[availableNodes.Count - 1];
            availableNodes.RemoveAt(availableNodes.Count - 1);
            n.Create(parent.halfSize / 2.0f);
            parent.children[index] = n;
            return n;
        }

        private static void FillAvailableNodes()
        {
            for (int i = 0; i < 1000; ++i)
                availableNodes.Add(new Node());
        }

        static void AddToAvailableNodes(Node node)
        {
            node.Reset();
            if (availableNodes.Count < 1000)
                availableNodes.Add(node);
        }

        private short depth;
        private int size;
        private Node root;
        List<short> registeredTypes = new List<short>();

        public int Size { get { return size; } }
        public Node Root { get { return root; } }

        public DataQuadTree(short depth)
        {
            this.depth = depth;
            this.size = (int)Math.Pow(2, depth - 1);
            root = new Node();
            root.Create(this.size / 2);
        }

        public void RegisterType(short type, float defaultValue)
        {
            if (registeredTypes.Contains(type))
                throw new Exception("Type " + type + " is already registered at DataQuadTree!");
            root.data.Add(new CustomTuple<short, float>(type, defaultValue));
            registeredTypes.Add(type);
        }

        public float[,] CreateDatamap(short type)
        {
            if (!registeredTypes.Contains(type))
                throw new Exception("Type " + type + " is not registered at DataQuadTree!");
            float[,] data = new float[size, size];
            for (int i = 0; i < size; ++i)
            {
                for (int j = 0; j < size; ++j)
                {
                    data[i, j] = root.GetValue(type, i, j);
                }
            }
            return data;
        }

        public void SetValue(short type, float x, float y, float newValue)
        {
            root.SetValue(type, x, y, newValue, null, depth);
        }

        public float GetValue(short type, float x, float y)
        {
            return root.GetValue(type, x, y);
        }

        public int CountVal(short type, float val)
        {
            return root.CountVal(type, val);
        }

        public void Optimize(short type)
        {
            root.Optimize(type);
        }
    }
}
