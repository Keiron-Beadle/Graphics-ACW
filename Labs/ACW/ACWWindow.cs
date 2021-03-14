using Labs.ACW.Lights;
using Labs.ACW.Objects;
using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;

namespace Labs.ACW
{
    public class ACWWindow : GameWindow
    {
        private Camera staticCam, dynCam, ActiveCam;
        private int[] VAO_IDs = new int[3];
        private ShaderUtility shader;
        private Cube cube, ground;
        private Tetrahedron tet;
        List<Objects.Object> entities = new List<Objects.Object>();
        List<Light> lights = new List<Light>();

        public ACWWindow()
            : base(
                1600, // Width
                1200, // Height
                GraphicsMode.Default,
                "Assessed Coursework",
                GameWindowFlags.Default,
                DisplayDevice.Default,
                3, // major
                3, // minor
                GraphicsContextFlags.ForwardCompatible
                )
        {
        }

        public struct Material
        {
            public Vector3 AmbientRef;
            public Vector3 DiffuseRef;
            public Vector3 SpecRef;
            public float Shininess;
        }

        public struct LightProperties
        {
            public Vector4 Position;
            public Vector3 AmbientLight;
            public Vector3 DiffuseLight;
            public Vector3 SpecularLight;
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            shader = new ShaderUtility(@"ACW/Shaders/vShader.vert", @"ACW/Shaders/fShader.frag");
            GL.UseProgram(shader.ShaderProgramID);

            GenerateCameras();

            GenerateLights();

            GL.GenVertexArrays(VAO_IDs.Length, VAO_IDs);

            GenerateEntities();

            GL.BindVertexArray(0);

            base.OnLoad(e);
        }

        private void GenerateEntities()
        {
            Material groundMat = MakeMaterial(new Vector3(0f, 0.05f, 0f),
            new Vector3(0.4f, 0.5f, 0.4f),
                new Vector3(0.04f, 0.7f, 0.04f), 0.078125f);
            ground = new Cube(new Vector3(0f, -0.06f, 0f), new Vector3(10f, 0.01f, 10f), shader.ShaderProgramID, VAO_IDs[0], groundMat);
            entities.Add(ground);


            Material cubeMat = MakeMaterial(new Vector3(0f, 0f, 0f),
                new Vector3(0.55f, 0.55f, 0.55f),
                new Vector3(0.7f, 0.7f, 0.7f), 0.25f);
            cube = new Cube(new Vector3(0f, 0.25f, 0f), new Vector3(0.15f, 0.15f, 0.15f), new Vector3(1, 1, 1),
                new Vector3(1.5f, -0.1f, 1f), shader.ShaderProgramID, VAO_IDs[1], cubeMat);
            cube.Updatable = true;
            //entities.Add(cube);

            tet = new Tetrahedron(new Vector3(0f, 0.25f, 0f), new Vector3(0.2f, 0.2f, 0.2f), new Vector3(1, 1, 1),
                new Vector3(0, 0, 0), shader.ShaderProgramID, VAO_IDs[2], cubeMat);
            tet.Updatable = true;
            entities.Add(tet);
        }

        private void GenerateCameras()
        {
            staticCam = new Camera(new Vector3(4f, 5f, 6f), new Vector3(0, 0, 0), this.Width, this.Height, shader.ShaderProgramID);
            dynCam = new Camera(new Vector3(0, 0f, 2f), new Vector3(0,0,0), this.Width, this.Height, shader.ShaderProgramID);
            dynCam.Active = true;
        }

        private void GenerateLights()
        {
            LightProperties p1 = MakeLightPropertes(new Vector4(0, 0.5f, 0, 1), new Vector3(0.33f, 0.15f, 0.46f), new Vector3(0.66f, 0.329f, 0.9215f), new Vector3(0.33f, 0.15f, 0.46f));
            LightProperties p2 = MakeLightPropertes(new Vector4(0.2f, 0.2f, 0.2f, 1), new Vector3(0.46f, 0.12f, 0.44f), new Vector3(1f, 0f, 0.533f), new Vector3(0.46f, 0.12f, 0.44f));
            LightProperties p3 = MakeLightPropertes(new Vector4(-0.2f, 0.2f, 0.2f, 1), new Vector3(0.11f, 0.12f, 0.49f), new Vector3(0.901f, 0.239f, 1f), new Vector3(0.11f, 0.12f, 0.49f));
            lights.Add(new PointLight(p1, shader.ShaderProgramID));
            lights.Add(new PointLight(p2, shader.ShaderProgramID));
            lights.Add(new PointLight(p3, shader.ShaderProgramID));
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
            dynCam.OnKeyDown(e);
            if (e.Key == Key.O)
            {
                if (dynCam.Active) { dynCam.Active = false; staticCam.Active = true; }
                else { dynCam.Active = true; staticCam.Active = false; }
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
 	        base.OnUpdateFrame(e);
            dynCam.Update();
            staticCam.Update();
            if (dynCam.Active) { ActiveCam = dynCam; }
            else { ActiveCam = staticCam; }
            for (int i = 0; i < lights.Count; i++) { lights[i].Update(ActiveCam, i); }
            for (int i = 0; i < entities.Count; i++) 
            {
                if (entities[i].Updatable)
                    entities[i].Update(ActiveCam, e.Time);
            }

            for (int i = 0; i < 900000; i++)
            {
                long j = i * i;
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            for (int i = 0; i < entities.Count; i++)
            {
                GL.BindVertexArray(VAO_IDs[i]);
                entities[i].RenderUpdate();
                entities[i].Draw();
            }

            GL.BindVertexArray(0);
            SwapBuffers();
        }

        private LightProperties MakeLightPropertes(Vector4 pPosition, Vector3 pAmbientLight, Vector3 pDiffuseLight, Vector3 pSpecularLight)
        {
            return new LightProperties
            {
                Position = pPosition,
                AmbientLight = pAmbientLight,
                DiffuseLight = pDiffuseLight,
                SpecularLight = pSpecularLight
            };
        }

        private Material MakeMaterial(Vector3 pAmbientRef, Vector3 pDiffuseRef, Vector3 pSpecRef, float pShininess)
        {
            return new Material()
            {
                AmbientRef = pAmbientRef,
                DiffuseRef = pDiffuseRef,
                SpecRef = pSpecRef,
                Shininess = pShininess
            };
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(this.ClientRectangle);
            if (shader != null)
            {
                int uProjectionLocation = GL.GetUniformLocation(shader.ShaderProgramID, "uProjection");
                Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(1, (float)Width / Height, 0.01f, 50f);
                GL.UniformMatrix4(uProjectionLocation, true, ref projection);
            }
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            for (int i = 0; i < entities.Count; i++) { entities[i].Dispose(); }
            GL.DeleteVertexArrays(VAO_IDs.Length, VAO_IDs);
            shader.Delete();
            base.OnUnload(e);
        }
    }
}
