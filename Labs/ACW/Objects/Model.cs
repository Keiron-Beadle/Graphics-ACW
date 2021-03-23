using Labs.Utility;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Labs.ACW.ACWWindow;

namespace Labs.ACW.Objects
{
    class Model : Object
    {
        int vPositionLocation, vNormalLocation;
        private ModelUtility modelUtility;

        public Model(Vector3 pPosition, Vector3 pScale, Vector3 pRotation, int pShaderID, int pVAO_ID, string pFileLocation, Material pMaterial, Object pParent = null)
            : base(pPosition, pScale, pRotation, pShaderID, pVAO_ID, pMaterial, pFileLocation, pParent)
        {
            GL.BindVertexArray(VAO_ID);
            GL.GenBuffers(VBO_IDs.Length, VBO_IDs);
            vPositionLocation = GL.GetAttribLocation(pShaderID, "vPosition");
            vNormalLocation = GL.GetAttribLocation(pShaderID, "vNormal");
            LoadModel(pFileLocation);
        }

        private void LoadModel(string pFileLocation)
        {
            modelUtility = ModelUtility.LoadModel(pFileLocation);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO_IDs[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(modelUtility.Vertices.Length * sizeof(float)), modelUtility.Vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBO_IDs[1]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(modelUtility.Indices.Length * sizeof(float)), modelUtility.Indices, BufferUsageHint.StaticDraw);
            CheckModelLoad();
            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(vNormalLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), sizeof(float) * 3);
        }

        private void CheckModelLoad()
        {
            int size;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (modelUtility.Vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (modelUtility.Indices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }
        }

        public override void Draw()
        {
            base.Draw();
            GL.BindVertexArray(VAO_ID);
            GL.DrawElements(PrimitiveType.Triangles, modelUtility.Indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public override void Dispose()
        {
            GL.DeleteBuffers(VBO_IDs.Length, VBO_IDs);
        }

        public override void RenderUpdate()
        {
            int uLocalLocation = GL.GetUniformLocation(shaderID, "uLocal");
            GL.UniformMatrix4(uLocalLocation, true, ref mLocalTransform);
            UpdateUMaterial();
        }

        public override void Update(Camera pActiveCam, double pDeltaTime)
        {
           // throw new NotImplementedException();
        }

        protected override void UpdateUMaterial()
        {
            int uAmbientRefPosition = GL.GetUniformLocation(shaderID, "uMaterial.AmbientReflectivity");
            int uDiffuseRefPosition = GL.GetUniformLocation(shaderID, "uMaterial.DiffuseReflectivity");
            int uSpecRefPosition = GL.GetUniformLocation(shaderID, "uMaterial.SpecularReflectivity");
            int uShininessPosition = GL.GetUniformLocation(shaderID, "uMaterial.Shininess");

            GL.Uniform3(uAmbientRefPosition, thisMaterial.AmbientRef);
            GL.Uniform3(uDiffuseRefPosition, thisMaterial.DiffuseRef);
            GL.Uniform3(uSpecRefPosition, thisMaterial.SpecRef);
            GL.Uniform1(uShininessPosition, thisMaterial.Shininess);
        }
    }
}
