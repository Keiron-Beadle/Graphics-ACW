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
        public Cube(Vector3 inPosition, Vector3 Dimensions, Vector3 RGB, int shaderProgramID) 
            : this(inPosition, Dimensions, RGB,  Vector3.Zero, Vector3.Zero, shaderProgramID) { }
        public Cube(Vector3 inPosition, Vector3 Dimensions, Vector3 RGB,Vector3 inScale, Vector3 inRotation, int shaderProgramID) 
            : base(inPosition, inScale, inRotation, shaderProgramID) 
        {
            vertices = new float[] {
                -Dimensions.X, -Dimensions.Y, Dimensions.Z, RGB.X, RGB.Y, RGB.Z,
                Dimensions.X, -Dimensions.Y, Dimensions.Z, RGB.X, RGB.Y, RGB.Z,
                Dimensions.X, Dimensions.Y, Dimensions.Z, RGB.X, RGB.Y, RGB.Z,
                -Dimensions.X, Dimensions.Y, Dimensions.Z, RGB.X, RGB.Y, RGB.Z,
                Dimensions.X, -Dimensions.Y, -Dimensions.Z, RGB.X, RGB.Y, RGB.Z,
                Dimensions.X, Dimensions.Y, -Dimensions.Z, RGB.X, RGB.Y, RGB.Z,
                -Dimensions.X, Dimensions.Y, -Dimensions.Z, RGB.X, RGB.Y, RGB.Z,
                -Dimensions.X, -Dimensions.Y, -Dimensions.Z, RGB.X, RGB.Y, RGB.Z,
            };

            indices = new uint[]
            {
                0,1,2,
                2,3,0,
                1,4,5,
                5,2,1,
                3,2,5,
                5,6,3,
                7,0,3,
                3,6,7,
                4,7,6,
                6,5,4,
                0,1,4,
                4,7,0
            };

            GL.GenBuffers(VBO_IDs.Length, VBO_IDs);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO_IDs[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, BufferUsageHint.StaticDraw);
            CheckVertexLoad();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBO_IDs[1]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(int)), indices, BufferUsageHint.StaticDraw);
            CheckIndicesLoad();

            int vPositionLocation = GL.GetAttribLocation(shaderID, "vPosition");
            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            int vColourLocation = GL.GetAttribLocation(shaderID, "vColour");
            GL.EnableVertexAttribArray(vColourLocation);
            GL.VertexAttribPointer(vColourLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

        }

        public override void Dispose()
        {
            GL.DeleteBuffers(VBO_IDs.Length, VBO_IDs);
        }

        public override void Draw()
        {
            base.Draw();
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}
