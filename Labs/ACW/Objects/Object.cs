using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using static Labs.ACW.ACWWindow;

namespace Labs.ACW.Objects
{
    abstract class Object
    {
        protected int VAO_ID;
        protected int[] VBO_IDs;
        protected float[] vertices;
        protected uint[] indices;
        protected Matrix4 mLocalTransform = Matrix4.Identity;
        protected Object parent;
        protected int shaderID;
        protected Material thisMaterial;

        public Matrix4 LocalTransform => mLocalTransform;

        public Object(Vector3 inPosition, int shaderProgramID, int vao_ID, Material pMaterial) 
            : this(inPosition, Vector3.Zero, Vector3.Zero, shaderProgramID, vao_ID, pMaterial) { }

        public Object(Vector3 inPosition, Vector3 inScale, Vector3 inRotation, int shaderProgramID, int vao_ID, Material pMaterial)
        {
            thisMaterial = pMaterial;
            this.VAO_ID = vao_ID;
            shaderID = shaderProgramID;
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
                mLocalTransform = Matrix4.CreateTranslation(inPosition);
            }
            int uLocalLocation = GL.GetUniformLocation(shaderID, "uLocal");
            GL.UniformMatrix4(uLocalLocation, true, ref mLocalTransform);
        }

        protected abstract void UpdateUMaterial();

        public virtual void Draw()
        {
            int uLocalLocation = GL.GetUniformLocation(shaderID, "uLocal");
            GL.UniformMatrix4(uLocalLocation, true, ref mLocalTransform);
        }

        public abstract void Update();

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
