using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labs.ACW.Objects
{
    class Cube : Object
    {
        public Cube(Vector3 inPosition, Vector3 Dimensions, Vector3 RGB, int shaderProgramID, int vao_ID) 
            : this(inPosition, Dimensions, RGB,  Vector3.Zero, Vector3.Zero, shaderProgramID, vao_ID) { }
        public Cube(Vector3 inPosition, Vector3 D, Vector3 RGB,Vector3 inScale, Vector3 inRotation, int shaderProgramID, int vao_ID) 
            : base(inPosition, inScale, inRotation, shaderProgramID, vao_ID) 
        {
            float x = D.X, y = D.Y, z = D.Z;
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

        public override void Update()
        {
            UpdateUMaterial();
        }

        protected override void UpdateUMaterial()
        {
            Vector3 ambientRef = new Vector3(0.3f, 0.3f, 0.3f);
            Vector3 diffuseRef = new Vector3(0.1f, 0.1f, 0.1f);
            Vector3 specRef = new Vector3(0.3f, 0.5f, 0.7f);
            float shininess = 0.6f;

            int uAmbientRefPosition = GL.GetUniformLocation(shaderID, "uMaterial.AmbientReflectivity");
            int uDiffuseRefPosition = GL.GetUniformLocation(shaderID, "uMaterial.DiffuseReflectivity");
            int uSpecRefPosition = GL.GetUniformLocation(shaderID, "uMaterial.SpecularReflectivity");
            int uShininessPosition = GL.GetUniformLocation(shaderID, "uMaterial.Shininess");

            GL.Uniform3(uAmbientRefPosition, ambientRef);
            GL.Uniform3(uDiffuseRefPosition, diffuseRef);
            GL.Uniform3(uSpecRefPosition, specRef);
            GL.Uniform1(uShininessPosition, shininess);
        }
    }
}
