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
    class Tetrahedron : Object
    {
        public Tetrahedron(Vector3 inPosition, Vector3 D, Vector3 inScale, Vector3 inRotation, int shaderProgramID, int vao_ID, Material pMaterial)
            : base(inPosition, D, inScale, inRotation, shaderProgramID, vao_ID, pMaterial)
        {
            float dx = D.X , dy = D.Y, dz = D.Z ;
            Vector3 crossFirst = Vector3.Cross(new Vector3(-dx, -dy, -dz) - new Vector3(dx,-dy, dz)
                , new Vector3(-dx, -dy, -dz) - new Vector3(0,-dy,dz));

            Vector3 crossSecond = Vector3.Cross(new Vector3(0,-dy,dz) - new Vector3(dx, -dy, -dz),
                   new Vector3(0,-dy,dz)-new Vector3(0,dy,0));

            Vector3 crossThird = Vector3.Cross(new Vector3(-dx, -dy, -dz) - new Vector3(dx, -dy, -dz),
                new Vector3(0,-dy,dz) - new Vector3(0,dy,0));

            Vector3 crossFourth = Vector3.Cross(new Vector3(0,-dy,dz) - new Vector3(-dx,-dy,-dz),
                new Vector3(0,-dy,dz) - new Vector3(0,dy,0));

            vertices = new float[]
            {
                - dx, - dy, - dz, crossFirst.X, crossFirst.Y, crossFirst.Z,
                dx, - dy, - dz, crossFirst.X, crossFirst.Y, crossFirst.Z,
                0, - dy, dz, crossFirst.X, crossFirst.Y, crossFirst.Z,

                0,  - dy, dz, crossSecond.X, crossSecond.Y, crossSecond.Z,
                dx, - dy, - dz, crossSecond.X, crossSecond.Y, crossSecond.Z,
                0, dy, 0, crossSecond.X, crossSecond.Y, crossSecond.Z,

                -dx, -dy, -dz, crossThird.X, crossThird.Y, crossThird.Z,
                dx, -dy, -dz, crossThird.X, crossThird.Y, crossThird.Z,
                0,dy,0, crossThird.X, crossThird.Y, crossThird.Z,

                0, -dy, dz, crossFourth.X, crossFourth.Y, crossFourth.Z,
                -dx, -dy, -dz, crossFourth.X, crossFourth.Y, crossFourth.Z,
                0,dy,0, crossFourth.X, crossFourth.Y, crossFourth.Z
            };

            int vPositionLocation = GL.GetAttribLocation(shaderID, "vPosition");
            int vNormalLocation = GL.GetAttribLocation(shaderID, "vNormal");

            GL.GenBuffers(VBO_IDs.Length, VBO_IDs);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO_IDs[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, BufferUsageHint.StaticDraw);
            CheckVertexLoad();

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
            GL.DrawArrays(PrimitiveType.Triangles, 0, 12);
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
            Console.WriteLine(pDeltaTime);
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
