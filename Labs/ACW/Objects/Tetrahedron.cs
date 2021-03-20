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
        public Tetrahedron(Vector3 inPosition, Vector3 inScale, Vector3 inRotation, int shaderProgramID, int vao_ID, Material pMaterial, string texFilePath = null)
            : base(inPosition, inScale, inRotation, shaderProgramID, vao_ID, pMaterial, texFilePath)
        {
            Vector3 p1 = new Vector3(0.9428f, -0.33333f, 0);
            Vector3 p2 = new Vector3(-0.4714f, -0.33333f, -0.81649f);
            Vector3 p3 = new Vector3(-0.4714f, -0.3333f, 0.81649f);
            Vector3 p4 = new Vector3(0, 1, 0) ;

            Vector3 crossFirst = Vector3.Cross(p2-p1,p3-p1);
            Vector3 crossSecond = Vector3.Cross(p2-p1,p4-p1);
            Vector3 crossThird = Vector3.Cross(p1-p3, p4-p3);
            Vector3 crossFourth = Vector3.Cross(p3-p2,p4-p3);

            //Console.WriteLine("Average of first: " + Average(crossFirst, crossSecond, crossThird));
            //Console.WriteLine("Average of second: " + Average(crossFirst, crossSecond, crossFourth));
            //Console.WriteLine("Average of third: " + Average(crossFirst, crossThird, crossFourth));
            //Console.WriteLine("Average of fourth: " + Average(crossSecond, crossThird, crossFourth));

            //vboData = new float[]
            //{
            //    p1.X, p1.Y, p1.Z, crossFirst.X, crossFirst.Y, crossFirst.Z,
            //    p2.X, p2.Y, p2.Z, crossFirst.X, crossFirst.Y, crossFirst.Z,
            //    p3.X, p3.Y, p3.Z, crossFirst.X, crossFirst.Y, crossFirst.Z,

            //    p2.X, p2.Y, p2.Z, crossSecond.X, crossSecond.Y, crossSecond.Z,
            //    p4.X, p4.Y, p4.Z, crossSecond.X, crossSecond.Y, crossSecond.Z,
            //    p1.X, p1.Y, p1.Z, crossSecond.X, crossSecond.Y, crossSecond.Z,

            //    p3.X, p3.Y, p3.Z, crossThird.X, crossThird.Y, crossThird.Z,
            //    p1.X, p1.Y, p1.Z, crossThird.X, crossThird.Y, crossThird.Z,
            //    p4.X, p4.Y, p4.Z, crossThird.X, crossThird.Y, crossThird.Z,

            //    p2.X, p2.Y, p2.Z, crossFourth.X, crossFourth.Y, crossFourth.Z,
            //    p3.X, p3.Y, p3.Z, crossFourth.X, crossFourth.Y, crossFourth.Z,
            //    p4.X, p4.Y, p4.Z, crossFourth.X, crossFourth.Y, crossFourth.Z
            //};

            vboData = new float[]
            {
                p1.X,p1.Y,p1.Z,0.7257752f,1.282978f,-0.0000023604f,
                p2.X,p2.Y,p2.Z,-0.3628672f,1.282978f,-0.6285506f,
                p3.X,p3.Y,p3.Z,-0.3628672f,1.282978f,0.6285034f,
                p4.X,p4.Y,p4.Z,-0.00000081857f,0.7697868f,-0.000001417299f
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
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, indices);
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
