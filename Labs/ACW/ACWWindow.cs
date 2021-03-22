using Labs.ACW.Lights;
using Labs.ACW.Objects;
using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using static Labs.ACW.Lights.Light;

namespace Labs.ACW
{
    public class ACWWindow : GameWindow
    {
        private Camera staticCam, dynCam, ActiveCam;
        private int[] VAO_IDs = new int[7];
        private ShaderUtility shader;
        private Cube cube, ground, wallL, wallR, wallF;
        private Tetrahedron tet;
        private Model werewolfModel;
        List<Objects.Object> entities = new List<Objects.Object>();
        List<Light> lights = new List<Light>();

        private string[] textureFilePaths = { "ACW/Resources/woodTex.jpg" };

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

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.392f, 0.584f, 0.929f, 1.0f);
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
            new Vector3(0.3f, 0.3f, 0.3f),
                new Vector3(0.05f, 0.05f, 0.05f), 0.078125f);
            ground = new Cube(new Vector3(0f, -0.2f, 0f), new Vector3(15f, 0.1f, 15f), Vector3.Zero, shader.ShaderProgramID, VAO_IDs[0], groundMat);
            entities.Add(ground);


            Material cubeMat = MakeMaterial(new Vector3(0f, 0f, 0f),
                new Vector3(0.55f, 0.55f, 0.55f),
                new Vector3(0.7f, 0.7f, 0.7f), 0.25f);

            cube = new Cube(new Vector3(0.5f, 0.25f, -0.2f), new Vector3(0.8f, 0.8f, 0.8f),
                new Vector3(1.1f, -0.1f, 1f), shader.ShaderProgramID, VAO_IDs[1], cubeMat);
            cube.Updatable = true;
            entities.Add(cube);

            tet = new Tetrahedron(new Vector3(-0.7f, 0f, 0f), new Vector3(0.2f, 0.2f, 0.2f),
                new Vector3(0f, 0, 0), shader.ShaderProgramID, VAO_IDs[2], cubeMat);
            tet.Updatable = true;
            entities.Add(tet);


            wallL = new Cube(new Vector3(-1.1f, -0.6f, 0f), new Vector3(0.3f, 3f, 7f),
                new Vector3(0, 0, 0), shader.ShaderProgramID, VAO_IDs[3], cubeMat, textureFilePaths[0]);
            entities.Add(wallL);

            wallR = new Cube(new Vector3(1.1f, -0.6f, 0f), new Vector3(0.3f, 3f, 7f),
                 new Vector3(0, 0, 0), shader.ShaderProgramID, VAO_IDs[4], cubeMat, textureFilePaths[0]);
            entities.Add(wallR);

            wallF = new Cube(new Vector3(0f, -0.6f, -1f), new Vector3(7f, 3f, 0.3f),
                new Vector3(0, 0, 0), shader.ShaderProgramID, VAO_IDs[5], cubeMat, textureFilePaths[0]);
            entities.Add(wallF);

            werewolfModel = new Model(new Vector3(-0.4f, 0.3f, 1f), shader.ShaderProgramID, VAO_IDs[6], "Utility/Models/model.bin", cubeMat);
            entities.Add(werewolfModel);
        }

        private void GenerateCameras()
        {
            staticCam = new Camera(new Vector3(-0.9f, 0.86f, -0.8f), new Vector3(0, 0, 0), this.Width, this.Height, shader.ShaderProgramID);
            dynCam = new Camera(new Vector3(0, 0f, 2f), new Vector3(0,0,0), this.Width, this.Height, shader.ShaderProgramID);
            dynCam.Active = true;
        }

        private void GenerateLights()
        {
            LightProperties p1 = MakeLightPropertes(new Vector4(-0.7f, 0.5f, -0.5f, 1), new Vector3(0.04f, 0.04f, 0.04f), 
                new Vector3(0.1804f, 0.85098f, 1f), new Vector3(0.045f, 0.2125f, 0.25f));
            LightProperties p2 = MakeLightPropertes(new Vector4(0f, 2f, -5f, 1), new Vector3(0f, 0f, 0f), 
                new Vector3(0.2f, 0.2f, 0.2f), new Vector3(0f, 0, 0));
            LightProperties p3 = MakeLightPropertes(new Vector4(0.5f, 0.4f, 0.8f, 1), 
                new Vector3(0.04f, 0.04f, 0.04f), new Vector3(0.8219f, 0.03635f, 0.62274f), 
                new Vector3(0.055f, 0.06f, 0.25f));
            LightProperties spot = MakeLightPropertes(new Vector4(-0.1f,1,0,1), new Vector3(0.01f, 0.01f, 0.01f), 
                new Vector3(1f, 1f, 1f), new Vector3(0.01f,0.01f,0.01f), (float)Math.Cos(0.4187f), new Vector3(0,-1,0));
            lights.Add(new Light(p1, shader.ShaderProgramID));
            lights.Add(new Light(p2, shader.ShaderProgramID));
            lights.Add(new Light(p3, shader.ShaderProgramID));
            lights.Add(new Light(spot, shader.ShaderProgramID));
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

            GL.UseProgram(shader.ShaderProgramID);
            for (int i = 0; i < entities.Count; i++)
            {
                GL.BindVertexArray(VAO_IDs[i]);
                entities[i].RenderUpdate();
                entities[i].Draw();
            }

            GL.BindVertexArray(0);
            SwapBuffers();
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
