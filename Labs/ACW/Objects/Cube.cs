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
        public Cube(Vector3 inPosition) : this(inPosition, Vector3.Zero, Vector3.Zero) { }
        public Cube(Vector3 inPosition, Vector3 inScale, Vector3 inRotation) : base(inPosition, inScale, inRotation) 
        {
            vertices = new float[] {
                0, 0, 0, 0f, 0f, 0f,
                0.3f, 0, 0, 0f, 0f, 0.5f,
                0.3f, 0.3f, 0,0f, 0.5f, 0f,
                0, 0.3f, 0, 0f, 0f, 0f,
                0, 0, 0.3f, 0f, 0f, 0f,
                0.3f, 0, 0.3f, 1f, 0f, 1f,
                0.3f, 0.3f, 0.3f, 0f, 0f, 0f,
                0, 0.3f, 0.3f, 0f, 1f, 0f,
            };

            indices = new int[]
            {
                0,1,2,3,4,5,6,7
            };

            GL.GenBuffers(VBO_IDs.Length, VBO_IDs);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO_IDs[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, BufferUsageHint.StaticDraw);
            CheckVertexLoad();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBO_IDs[1]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(int)), indices, BufferUsageHint.StaticDraw);
            CheckIndicesLoad();
            
        }

        public override void Dispose()
        {
            GL.DeleteBuffers(VBO_IDs.Length, VBO_IDs);
        }

        public override void Draw()
        {
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, indices.Length);
        }
    }
}
