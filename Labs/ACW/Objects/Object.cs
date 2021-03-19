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
        protected int shaderID, VAO_ID, texture_ID = 0;
        protected string textureFilePath;
        protected BitmapData TextureData;
        protected int[] VBO_IDs;
        protected float[] vboData;
        protected uint[] indices;
        protected Matrix4 mLocalTransform = Matrix4.Identity;
        protected Object parent;
        protected Material thisMaterial;
        protected Vector3 position;

        public Matrix4 LocalTransform => mLocalTransform;
        public bool Updatable;

        public Object(Vector3 inPosition, int shaderProgramID, int vao_ID, Material pMaterial = new Material()) 
            : this(inPosition, Vector3.One, Vector3.Zero, shaderProgramID, vao_ID, pMaterial) { }

        public Object(Vector3 inPosition, Vector3 inScale, Vector3 inRotation, int shaderProgramID, int vao_ID, Material pMaterial, string texFilePath = null)
        {
            textureFilePath = texFilePath;
            Updatable = false;
            position = inPosition;
            thisMaterial = pMaterial;
            this.VAO_ID = vao_ID;
            shaderID = shaderProgramID;
            VBO_IDs = new int[2];
            Matrix4 rotationMatrix = CreateRotationMatrix(inRotation);
            if (parent != null)
            {
                mLocalTransform = parent.LocalTransform *
                    Matrix4.CreateScale(inScale) *
                    Matrix4.CreateTranslation(position) *
                    rotationMatrix;
            }
            else
            {
                mLocalTransform = Matrix4.CreateTranslation(-position / 2) *
                                  Matrix4.CreateScale(inScale) *
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

        protected void LoadTexture()
        {
            Bitmap texBitmap;
            if (System.IO.File.Exists(textureFilePath))
            {
                texBitmap = new Bitmap(textureFilePath);
                texBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                BitmapData texData = texBitmap.LockBits(new Rectangle(0, 0, texBitmap.Width, texBitmap.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                TextureData = texData;
            }
            else
            {
                throw new Exception("Could not find file: " + textureFilePath);
            }
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.GenTextures(1, out texture_ID);
            GL.BindTexture(TextureTarget.Texture2D, texture_ID);
            GL.TexImage2D(TextureTarget.Texture2D,
                0, PixelInternalFormat.Rgba, TextureData.Width, TextureData.Height,
                0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, TextureData.Scan0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            texBitmap.UnlockBits(TextureData);
        }

        protected Matrix4 CreateRotationMatrix(Vector3 inRotation)
        {
            Matrix4 temp = Matrix4.Identity;
            if (inRotation.X != 0) { temp *= Matrix4.CreateRotationX(inRotation.X); }
            if (inRotation.Y != 0) { temp *= Matrix4.CreateRotationY(inRotation.Y); }
            if (inRotation.Z != 0) { temp *= Matrix4.CreateRotationZ(inRotation.Z); }
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
