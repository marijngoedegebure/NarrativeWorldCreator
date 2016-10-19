using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Geometry
{
    public class AbstractNode : IDisposable
    {
        public delegate void NodeChangeHandler();
        public event NodeChangeHandler PositionChanged, RotationChanged, ScalationChanged;

        private Vec3 m_position = new Vec3(0, 0, 0), m_rotation = new Vec3(0, 0, 0), m_scalation = new Vec3(1, 1, 1);
        private Matrix4 m_rotationOverride = null, m_rotationXmZYOverride = null;
        private bool m_matrixChanged = true;
        private Matrix4 m_transformation = Matrix4.Identity, m_inverseTransformation = Matrix4.Identity;
        private bool m_forceTransfRecalc = false, m_forceInverseTransfRecalc = false;
        private Matrix4 m_rotTransformation = Matrix4.Identity, m_inverseRotTransformation = Matrix4.Identity;
        private Matrix4 m_transformationOverride = null;

        public Matrix4 OverrideTransformation { get { return m_transformationOverride; } set { m_transformationOverride = value; } }

        public Vec3 Position
        {
            get { return m_position; }
            set { m_position = value; m_matrixChanged = true; if (PositionChanged != null) PositionChanged(); }
        }
        public Vec3 Rotation
        {
            get { return m_rotation; }
            set { m_rotation = value; m_matrixChanged = true; if (RotationChanged != null) RotationChanged(); }
        }
        public Vec3 Scalation
        {
            get { return m_scalation; }
            set { m_scalation = value; m_matrixChanged = true; if (ScalationChanged != null) ScalationChanged(); }
        }

        public void OverrideRotationMatrix(Matrix4 rotation, Matrix4 rotationXmZY)
        {
            m_rotationOverride = rotation;
            m_rotationXmZYOverride = rotationXmZY;
            m_matrixChanged = true;
        }

        public AbstractNode()
        {
        }

        public AbstractNode(AbstractNode node)
        {
            m_position = new Vec3(node.Position);
            m_rotation = new Vec3(node.Rotation);
            m_scalation = new Vec3(node.Scalation);
            if (node.m_rotationOverride != null)
                m_rotationOverride = new Matrix4(node.m_rotationOverride);
            m_matrixChanged = node.m_matrixChanged;
            m_transformation = new Matrix4(node.m_transformation);
            m_inverseTransformation = new Matrix4(node.m_inverseTransformation);
            m_forceTransfRecalc = node.m_forceTransfRecalc;
            m_forceInverseTransfRecalc = node.m_forceInverseTransfRecalc;
        }

        public AbstractNode(NodeLocation location)
            : this((AbstractNode)location)
        {
        }

        public Matrix4 GetRotationTransformation()
        {
            CalcTransf();
            return m_rotTransformation;
        }

        private void CalcTransf()
        {
            if (m_transformationOverride != null)
            {
                m_transformation = m_transformationOverride;
                return;
            }
            bool tp = m_position.ChangedTillLastCheck();
            bool tr = m_rotation.ChangedTillLastCheck();
            bool ts = m_scalation.ChangedTillLastCheck();
            if (m_forceTransfRecalc || m_matrixChanged || tp || tr || ts)
            {
                m_rotTransformation = m_rotationOverride;
                if (m_rotTransformation == null)
                    m_rotTransformation = Matrix4.RotationZ(Rotation.Z) *
                                            Matrix4.RotationY(Rotation.Y) *
                                            Matrix4.RotationX(Rotation.X);

                m_transformation = Matrix4.Scalation(Scalation) *
                    m_rotTransformation *
                    Matrix4.Translation(Position);

                if (m_matrixChanged || tp || tr || ts)
                    m_forceInverseTransfRecalc = true;
                m_matrixChanged = false;
            }
        }

        private void CalcInverseTransf()
        {
            if (m_transformationOverride != null)
            {
                m_inverseTransformation = m_transformationOverride.Inverse();
                return;
            }

            bool tp = m_position.ChangedTillLastCheck();
            bool tr = m_rotation.ChangedTillLastCheck();
            bool ts = m_scalation.ChangedTillLastCheck();
            if (m_forceInverseTransfRecalc || m_matrixChanged || tp || tr || ts)
            {
                m_inverseRotTransformation = null;
                if (m_rotationOverride != null)
                    m_inverseRotTransformation = m_rotationOverride.Inverse();
                else
                    m_inverseRotTransformation = Matrix4.RotationX(-Rotation.X) *
                                                    Matrix4.RotationY(-Rotation.Y) *
                                                    Matrix4.RotationZ(-Rotation.Z);

                m_inverseTransformation = Matrix4.Translation(-Position) *
                    m_inverseRotTransformation *
                    Matrix4.Scalation((float)1 / Scalation);

                if (m_matrixChanged || tp || tr || ts)
                    m_forceTransfRecalc = true;
                m_matrixChanged = false;
            }
        }

        public Matrix4 GetTransformation()
        {
            CalcTransf();
            return m_transformation;
        }

        public Matrix4 GetTransformationXmZY()
        {
            return CalcTransformationXmZY();
        }

        private Matrix4 CalcTransformationXmZY()
        {
            Matrix4 rotTransformation = m_rotationXmZYOverride != null ? m_rotationXmZYOverride :
                                                Matrix4.RotationY(-Rotation.Z) *
                                                Matrix4.RotationZ(Rotation.Y) *
                                                Matrix4.RotationmX(Rotation.X);

            return Matrix4.Scalation(Scalation.XmZY()) *
                    rotTransformation *
                    Matrix4.Translation(Position.XmZY());
        }

        public Matrix4 SpecialTransformationXmZYmRotY()
        {
            Matrix4 rotTransformation = m_rotationXmZYOverride != null ? m_rotationXmZYOverride : 
                                                Matrix4.RotationmZ(Rotation.Y) *
                                                Matrix4.RotationY(Rotation.Z) *
                                                Matrix4.RotationX(Rotation.X);

            return Matrix4.Scalation(Scalation.XmZY()) *
                    rotTransformation *
                    Matrix4.Translation(Position.XmZY());
        }

        public Matrix4 GetInverseRotationTransformation()
        {
            CalcInverseTransf();
            return m_inverseRotTransformation;
        }

        public Matrix4 GetInverseTransformation()
        {
            CalcInverseTransf();
            return m_inverseTransformation;
        }

        public override string ToString()
        {
            return "Pos: (" + Position + "), Rot: (" + Rotation + "), Scal: (" + Scalation + ")";
        }

        public void Dispose()
        {
            if (this.m_inverseRotTransformation != null) 
            {
                this.m_inverseRotTransformation.Dispose();
                this.m_inverseRotTransformation = null;
            }
            if (this.m_inverseTransformation != null) 
            {
                this.m_inverseTransformation.Dispose(); 
                this.m_inverseTransformation = null;
            }
            if (this.m_position != null) 
            {
                this.m_position.Dispose();
                this.m_position = null;
            }
            if (this.m_rotation != null) 
            {
                this.m_rotation.Dispose();
                this.m_rotation = null;
            }
            if (this.m_rotationOverride != null)
            {
                this.m_rotationOverride.Dispose(); 
                this.m_rotationOverride = null;
            }
            if (this.m_rotationXmZYOverride != null)
            {
                this.m_rotationXmZYOverride.Dispose(); 
                this.m_rotationXmZYOverride = null;
            }
            if (this.m_rotTransformation != null) {
                this.m_rotTransformation.Dispose();
                this.m_rotTransformation = null;
            }
            if (this.m_scalation != null) 
            {
                this.m_scalation.Dispose();
                this.m_scalation = null;
            }
            if (this.m_transformation != null) 
            {
                this.m_transformation.Dispose();
                this.m_transformation = null;
            }
            if (this.m_transformationOverride != null)
            {
                this.m_transformationOverride.Dispose(); 
                this.m_transformationOverride = null;
            }
        }
    }


    public class ConcreteNode : AbstractNode
    {
    }
}
