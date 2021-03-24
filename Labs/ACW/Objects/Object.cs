using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
        protected int shaderID, VAO_ID;
        protected int textureID;
        protected Vector3 scale, rotation;
        protected int[] VBO_IDs;
        protected float[] vboData;
        protected uint[] indices;
        protected Matrix4 mLocalTransform = Matrix4.Identity;
        protected Object parent;
        protected Material thisMaterial;
        protected Vector3 position;

        public Matrix4 LocalTransform => mLocalTransform;
        public bool Updatable;

        public Object(Vector3 pPosition, int pShaderID, int pVAO_ID, Material pMaterial = new Material()) 
            : this(pPosition, Vector3.One, Vector3.Zero, pShaderID, pVAO_ID, pMaterial) { }

        public Object(Vector3 pPosition, Vector3 pScale, Vector3 pRotation, int pShaderID, 
            int pVAO_ID, Material pMaterial, string pModelLocation = null, int pTexID = -1, Object pParent = null)
        {
            scale = pScale;
            rotation = pRotation;
            parent = pParent;
            textureID = pTexID;
            Updatable = false;
            position = pPosition;
            thisMaterial = pMaterial;
            this.VAO_ID = pVAO_ID;
            shaderID = pShaderID;
            VBO_IDs = new int[2];
            Matrix4 rotationMatrix = CreateRotationMatrix(pRotation);
            if (parent != null)
            {
                mLocalTransform = Matrix4.CreateTranslation(-position / 2) *
                    Matrix4.CreateScale(scale) *
                    rotationMatrix *
                    Matrix4.CreateTranslation(position) *
                    Matrix4.CreateTranslation(parent.LocalTransform.ExtractTranslation());
            }
            else
            {
                mLocalTransform = Matrix4.CreateTranslation(-position / 2) *
                                  Matrix4.CreateScale(scale) *
                                  rotationMatrix *
                                  Matrix4.CreateTranslation(position);
            }
            int uLocalLocation = GL.GetUniformLocation(shaderID, "uLocal");
            GL.UniformMatrix4(uLocalLocation, false, ref mLocalTransform);
        }

        public abstract void Update(Camera pActiveCam, double pDeltaTime);

        public abstract void RenderUpdate();

        protected abstract void UpdateUMaterial();

        public virtual void Draw()
        {
            int uLocalLocation = GL.GetUniformLocation(shaderID, "uLocal");
            GL.UniformMatrix4(uLocalLocation, true, ref mLocalTransform);
        }

        public abstract void Dispose();

        protected Matrix4 CreateRotationMatrix(Vector3 pRotation)
        {
            Matrix4 temp = Matrix4.Identity;
            if (pRotation.X != 0) { temp *= Matrix4.CreateRotationX(pRotation.X); }
            if (pRotation.Y != 0) { temp *= Matrix4.CreateRotationY(pRotation.Y); }
            if (pRotation.Z != 0) { temp *= Matrix4.CreateRotationZ(pRotation.Z); }
            return temp;
        }

        protected void CheckVBODataLoad()
        {
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out int size);
            if (vboData.Length * sizeof(float) != size)
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
