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
    class Cube : Object
    {
        public Cube(Vector3 inPosition, int shaderProgramID, int vao_ID, Material pMaterial) 
            : this(inPosition,  Vector3.One, Vector3.Zero, shaderProgramID, vao_ID, -1, pMaterial) { }
        public Cube(Vector3 inPosition,Vector3 inScale, Vector3 inRotation, int shaderProgramID, int vao_ID, int textureID, Material pMaterial) 
            : base(inPosition, inScale, inRotation, shaderProgramID, vao_ID, textureID, pMaterial) 
        {
            float x = 0.15f, y = 0.15f, z = 0.15f;
            vertices = new float[] {
            -x, -y, -z,  0.0f,  0.0f, -1.0f,
            x, -y, -z,  0.0f,  0.0f, -1.0f,
            x, y, -z,  0.0f,  0.0f, -1.0f,
            x, y, -z,  0.0f,  0.0f, -1.0f,
            -x, y, -z,  0.0f,  0.0f, -1.0f,
            -x, -y, -z,  0.0f,  0.0f, -1.0f,


            -x,-y,z,  0.0f,  0.0f,  1.0f,
            x,-y,z,  0.0f,  0.0f,  1.0f,
            x,y,z,  0.0f,  0.0f,  1.0f,
            x,y,z,  0.0f,  0.0f,  1.0f,
            -x,y,z,  0.0f,  0.0f,  1.0f,
            -x,-y,z,  0.0f,  0.0f,  1.0f,


            -x,y,z, -1.0f,  0.0f,  0.0f,
            -x,y,-z, -1.0f,  0.0f,  0.0f,
            -x,-y,-z, -1.0f,  0.0f,  0.0f,
            -x,-y,-z, -1.0f,  0.0f,  0.0f,
            -x,-y,z, -1.0f,  0.0f,  0.0f,
            -x,y,z, -1.0f,  0.0f,  0.0f,


             x,y,z,  1.0f,  0.0f,  0.0f,
             x,y,-z,  1.0f,  0.0f,  0.0f,
             x,-y,-z,  1.0f,  0.0f,  0.0f,
             x,-y,-z,  1.0f,  0.0f,  0.0f,
             x,-y,z,  1.0f,  0.0f,  0.0f,
             x,y,z,  1.0f,  0.0f,  0.0f,


            -x,-y,-z,  0.0f, -1.0f,  0.0f,
            x,-y,-z,  0.0f, -1.0f,  0.0f,
            x,-y,z,  0.0f, -1.0f,  0.0f,
            x,-y,z,  0.0f, -1.0f,  0.0f,
            -x,-y,z,  0.0f, -1.0f,  0.0f,
            -x,-y,-z,  0.0f, -1.0f,  0.0f,


            -x,y,-z,  0.0f,  1.0f,  0.0f,
            x,y,-z,  0.0f,  1.0f,  0.0f,
            x,y,z,  0.0f,  1.0f,  0.0f,
            x,y,z,  0.0f,  1.0f,  0.0f,
            -x,y,z,  0.0f,  1.0f,  0.0f,
            -x,y,-z,  0.0f,  1.0f,  0.0f

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

        public override void Dispose()
        {
            GL.DeleteBuffers(VBO_IDs.Length, VBO_IDs);
        }

        public override void Draw()
        {
            base.Draw();
            GL.BindVertexArray(VAO_ID);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

        }

        public override void Update(Camera pActiveCam, double pDeltaTime)
        {
            Matrix4 rot = CreateRotationMatrix(new Vector3(-0.95f, 0.4f, 0.85f) * (float)pDeltaTime);
            //Console.WriteLine(pDeltaTime);
            Matrix4.Mult(ref rot, ref mLocalTransform, out mLocalTransform);
        }

        public override void RenderUpdate()
        {
            int uLocalLocation = GL.GetUniformLocation(shaderID, "uLocal");
            GL.UniformMatrix4(uLocalLocation, true, ref mLocalTransform);
            UpdateUMaterial();
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
