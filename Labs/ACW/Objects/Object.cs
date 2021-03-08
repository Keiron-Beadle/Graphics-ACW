using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Labs.ACW.Objects
{
    abstract class Object
    {
        protected int[] VBO_IDs;
        protected float[] vertices;
        protected int[] indices;
        protected Matrix4 mLocalTransform;
        protected Object parent;

        public Matrix4 LocalTransform { get { return mLocalTransform; } }

        public Object(Vector3 inPosition) : this(inPosition, Vector3.Zero, Vector3.Zero) { }

        public Object(Vector3 inPosition, Vector3 inScale, Vector3 inRotation)
        {
            VBO_IDs = new int[2];
            Matrix4 rotationMatrix = CreateRotationMatrix(inRotation);
            if (parent != null)
            {
                mLocalTransform = parent.LocalTransform *
                    Matrix4.CreateScale(inScale) *
                    Matrix4.CreateTranslation(inPosition) *
                    rotationMatrix;
            }
            else
            {
                mLocalTransform = Matrix4.CreateScale(inScale) *
                    Matrix4.CreateTranslation(inPosition) *
                    rotationMatrix;
            }
        }

        public abstract void Draw();

        public abstract void Dispose();

        private Matrix4 CreateRotationMatrix(Vector3 inRotation)
        {
            Matrix4 temp = Matrix4.Identity;
            if (inRotation.X != 0) { temp *= Matrix4.CreateRotationX(inRotation.X); }
            if (inRotation.Y != 0) { temp *= Matrix4.CreateRotationY(inRotation.Y); }
            if (inRotation.Z != 0) { temp *= Matrix4.CreateRotationZ(inRotation.Z); }
            return temp;
        }

        protected void CheckVertexLoad()
        {
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int size);
            if (vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }
        }

        protected void CheckIndicesLoad()
        {
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out int size);
            if (indices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }
        }
    }
}
