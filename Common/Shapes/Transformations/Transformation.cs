using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.IO;

namespace Common.Shapes.Transformations
{
    public class Transformation : INotifyPropertyChanged
    {
        const short VERSION = 0;

        ShapeNode root;
        List<ShapeNode> shapeNodes = new List<ShapeNode>();
        List<TransformationNode> transformationNodes = new List<TransformationNode>();
        List<Group> groups = new List<Group>();

        public ShapeNode Root { get { return root; } }

        public IEnumerable<ShapeNode> NamedShapeNodes
        {
            get
            {
                foreach (ShapeNode s in shapeNodes)
                    if (s.Name != "")
                        yield return s;
            }
        }

        public Transformation(ShapeType inputShapeType)
        {
            root = new ShapeNode(this, inputShapeType, "root", true);
            AddNode(root);
        }

        private void AddNode(ShapeNode node)
        {
            shapeNodes.Add(node);
            node.PropertyChanged += new PropertyChangedEventHandler(shapeNode_PropertyChanged);
        }

        private void AddNode(TransformationNode node)
        {
            transformationNodes.Add(node);
        }

        public void Transform(ShapeNode input, TransformationType type)
        {
            System.Diagnostics.Debug.Assert(input != null && type != null);
            if (shapeNodes.Contains(input))
            {
                List<ShapeNode> output;
                TransformationNode t = type.Transform(input, out output);
                foreach (ShapeNode o in output)
                    AddNode(o);
                AddNode(t);
            }
        }

        void shapeNode_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "Name":
                    NotifyPropertyChange("NamedShapeNodes");
                    break;
            }
        }

        private void NotifyPropertyChange(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public void Save(System.IO.BinaryWriter w)
        {
            w.Write(VERSION);

            this.root.Save(w);
        }

        public Transformation(BinaryReader r)
        {
            short version = r.ReadInt16();

            this.root = new ShapeNode(this, r, version);
        }

        /// <summary>
        /// WARNING: Only use when loading from file
        /// </summary>
        /// <param name="shapeNode"></param>
        internal void AddShapeNodeFromFile(ShapeNode shapeNode)
        {
            this.shapeNodes.Add(shapeNode);
        }

        /// <summary>
        /// WARNING: Only use when loading from file
        /// </summary>
        /// <param name="transformationNode"></param>
        internal void AddTransformationNodeFromFile(TransformationNode transformationNode)
        {
            this.transformationNodes.Add(transformationNode);
        }
    }
}
