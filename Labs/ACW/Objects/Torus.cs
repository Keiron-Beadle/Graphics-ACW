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
    class Torus : Object
    {
        private int mainSegments = 20;
        private int tubeSegments = 20;
        private float mainRadius = 10;
        private float tubeRadius = 5;
        private uint restartIndex;
        int numIndices;

        public Torus(Vector3 pPosition, Vector3 pDimensions, Vector3 pScale, Vector3 pRotation, int pShaderID, int pVAO_ID, Material pMaterial)
            : base(pPosition, pDimensions, pScale, pRotation, pShaderID, pVAO_ID, pMaterial)
        {
            float x = pDimensions.X, y = pDimensions.Y, z = pDimensions.Z;
            int numVertices = (mainSegments + 1) * (tubeSegments + 1);
            restartIndex = (uint)numVertices;
            numIndices = (mainSegments * 2 * (tubeSegments + 1)) + mainSegments - 1;

            int vPositionLocation = GL.GetAttribLocation(shaderID, "vPosition");
            int vNormalLocation = GL.GetAttribLocation(shaderID, "vNormal");

            GL.BindVertexArray(pVAO_ID);
            GL.GenBuffers(VBO_IDs.Length, VBO_IDs);

            List<Vector3> verticesList = CreateVertexList();
            List<Vector3> normalsList = CreateNormalList();
            List<uint> indicesList = CreateIndiceList();

            vertices = new float[verticesList.Count * 3];
            for (int i = 0; i < verticesList.Count; i++)
            {
                vertices[i*3] = verticesList[i].X;
                vertices[i*3+1] = verticesList[i].Y;
                vertices[i*3+2] = verticesList[i].Z;
            }
            normals = new float[normalsList.Count * 3];
            for (int i = 0; i < normalsList.Count; i++)
            {
                normals[i*3] = normalsList[i].X;
                normals[i*3+1] = normalsList[i].Y;
                normals[i*3+2] = normalsList[i].Z;
            }
            float[] data = new float[vertices.Length + normals.Length];
            for (int i = 0; i < vertices.Length; i++) { data[i] = vertices[i]; }
            for (int j = vertices.Length; j < data.Length; j++) { data[j] = normals[j]; }
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO_IDs[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(data.Length * sizeof(float)), data, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(vNormalLocation);

            indices = indicesList.ToArray();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBO_IDs[2]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(uint)), indices, BufferUsageHint.StaticDraw);
            CheckIndicesLoad();

        }

        private List<uint> CreateIndiceList()
        {
            List<uint> temp = new List<uint>();
            uint currentVertexOffset = 0;
            for (int i = 0; i < mainSegments; i++)
            {
                for (int j = 0; j <= tubeSegments; j++)
                {
                    uint vertexIndexA = currentVertexOffset;
                    temp.Add(vertexIndexA);
                    uint vertexIndexB = currentVertexOffset + (uint)tubeSegments + 1;
                    temp.Add(vertexIndexB);
                    currentVertexOffset++;
                }
                if (i != mainSegments - 1)
                    temp.Add(restartIndex);
            }

            return temp;
        }

        private List<Vector3> CreateNormalList()
        {
            List<Vector3> temp = new List<Vector3>();
            float currentMainAngle = 0.0f;
            for (int i = 0; i <= mainSegments; i++)
            {
                float sinMainSeg = (float)Math.Sin(currentMainAngle);
                float cosMainSeg = (float)Math.Cos(currentMainAngle);
                float currentTubeAngle = 0.0f;
                for (int j = 0; j <= tubeSegments; j++)
                {
                    float sinTubeSeg = (float)Math.Sin(currentTubeAngle);
                    float cosTubeSeg = (float)Math.Cos(currentTubeAngle);

                    Vector3 normal = new Vector3(cosMainSeg * cosTubeSeg,
                        sinMainSeg * cosTubeSeg,
                        sinTubeSeg);
                    temp.Add(normal);
                    currentTubeAngle += 18;
                }
                currentMainAngle += 18;
            }

            return temp;
        }

        private List<Vector3> CreateVertexList()
        {
            List<Vector3> temp = new List<Vector3>(40);
            float currentMainAngle = 0.0f;
            for (int i = 0; i <= mainSegments; i++)
            {
                float sinMainSeg = (float)Math.Sin(currentMainAngle);
                float cosMainSeg = (float)Math.Cos(currentMainAngle);
                float currentTubeAngle = 0.0f;
                for (int j = 0; j <= tubeSegments; j++)
                {
                    float sinTubeSeg = (float)Math.Sin(currentTubeAngle);
                    float cosTubeSeg = (float)Math.Cos(currentTubeAngle);

                    Vector3 pos = new Vector3((mainRadius + tubeRadius * cosTubeSeg) * cosMainSeg,
                                (mainRadius + tubeRadius * cosTubeSeg) * sinMainSeg,
                                tubeRadius * sinTubeSeg);
                    temp.Add(pos);
                    currentTubeAngle += 18;
                }
                currentMainAngle += 18;
            }
            return temp;
        }

        public override void Draw()
        {
            base.Draw();
            GL.BindVertexArray(VAO_ID);
            GL.Enable(EnableCap.PrimitiveRestart);
            GL.PrimitiveRestartIndex(restartIndex);
            GL.DrawElements(PrimitiveType.TriangleStrip, numIndices, DrawElementsType.UnsignedInt, indices);
            GL.Disable(EnableCap.PrimitiveRestart);
        }

        public override void RenderUpdate()
        {
            int uLocalLocation = GL.GetUniformLocation(shaderID, "uLocal");
            GL.UniformMatrix4(uLocalLocation, true, ref mLocalTransform);
            UpdateUMaterial();
        }

        public override void Update(Camera pActiveCam, double pDeltaTime)
        {
            Matrix4 rot = CreateRotationMatrix(new Vector3(-0.95f, 0.4f, 0.85f) * (float)pDeltaTime);
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

        public override void Dispose()
        {
            GL.DeleteBuffers(VBO_IDs.Length, VBO_IDs);
        }
    }
}
