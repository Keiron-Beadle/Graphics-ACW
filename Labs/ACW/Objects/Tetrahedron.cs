using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Labs.ACW.ACWWindow;

namespace Labs.ACW.Objects
{
    class Tetrahedron : Object
    {
        public Tetrahedron(Vector3 pPosition, Vector3 pScale, Vector3 pRotation, int pShaderProgramID, int pVAO_ID, Material pMaterial, string pTexFilePath = null)
            : base(pPosition, pScale, pRotation, pShaderProgramID, pVAO_ID, pMaterial, pTexFilePath)
        {
            Vector3 p1 = new Vector3(0.9428f, -0.33333f, 0);
            Vector3 p2 = new Vector3(-0.4714f, -0.33333f, -0.81649f);
            Vector3 p3 = new Vector3(-0.4714f, -0.3333f, 0.81649f);
            Vector3 p4 = new Vector3(0, 1, 0) ;

            //Trial ground for finding the averaged normals for tetrahedron. 
            //Vector3 norm1 = Vector3.Cross(p3 - p1, p2 - p1) + Vector3.Cross(p3 - p1, p4 - p1) + Vector3.Cross(p2 - p1, p4 - p1);
            //Vector3 avg1 = Vector3.Normalize(norm1);
            //Vector3 norm2 = Vector3.Cross(p3-p2, p1-p2) + Vector3.Cross(p3-p2, p4-p2) + Vector3.Cross(p4-p2, p1-p2);
            //Vector3 norm3 = Vector3.Cross(p2-p3, p1-p3) + Vector3.Cross(p4-p3, p2-p3) + Vector3.Cross(p4-p3, p1-p3);
            //Vector3 norm4 = Vector3.Cross(p2-p4, p3-p4) + Vector3.Cross(p1-p4, p2-p4) + Vector3.Cross(p1-p4, p3-p4);
            //Vector3 avg2 = Vector3.Normalize(norm2);
            //Vector3 avg3 = Vector3.Normalize(norm3);
            //Vector3 avg4 = Vector3.Normalize(norm4);

            vboData = new float[]
            {
                p1.X,p1.Y,p1.Z,-0.0000005553247f,-0.5222294f,-0.852805f,
                p2.X,p2.Y,p2.Z,-0.246176f,0.8703966f, -0.4264148f,
                p3.X,p3.Y,p3.Z,-0.7385547f, -0.5222332f, -0.4263913f,
                p4.X,p4.Y,p4.Z,-0.4923665f,0.1740785f, -0.8528025f
            };

            indices = new uint[]
            {
                0,1,2,
                1,3,0,
                2,0,3,
                1,2,3
            };

            int vPositionLocation = GL.GetAttribLocation(shaderID, "vPosition");
            int vNormalLocation = GL.GetAttribLocation(shaderID, "vNormal");

            GL.GenBuffers(VBO_IDs.Length, VBO_IDs);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO_IDs[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vboData.Length * sizeof(float)), vboData, BufferUsageHint.StaticDraw);
            CheckVBODataLoad();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBO_IDs[1]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(uint)), indices, BufferUsageHint.StaticDraw);
            CheckIndicesLoad();

            GL.BindVertexArray(VAO_ID);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(vNormalLocation);
        }

        public override void Draw()
        {
            base.Draw();
            GL.BindVertexArray(VAO_ID);
            //GL.DrawArrays(PrimitiveType.Triangles, 0, 12);
            GL.DrawElements(PrimitiveType.TriangleStrip, indices.Length, DrawElementsType.UnsignedInt, indices);
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
            Matrix4 rot = CreateRotationMatrix(new Vector3(0f, 1f, 0f) * (float)pDeltaTime);
            //Console.WriteLine((int)Math.Round(1/pDeltaTime,0));
            Matrix4.Mult(ref rot, ref mLocalTransform, out mLocalTransform);
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
