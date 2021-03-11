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
        private int[] VAO_IDs = new int[2];
        private ShaderUtility shader;
        private Cube cube, ground;
        List<Objects.Object> entities = new List<Objects.Object>();
        List<Lights.Light> lights = new List<Lights.Light>();

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
            GL.ClearColor(0.1f,0.1f,0.1f,1.0f);
            GL.Enable(EnableCap.DepthTest);
            shader = new ShaderUtility(@"ACW/Shaders/vShader.vert", @"ACW/Shaders/fShader.frag");
            GL.UseProgram(shader.ShaderProgramID);

            staticCam = new Camera(new Vector3(4f, 5f, 6f), new Vector3(0,0,0), this.Width, this.Height, shader.ShaderProgramID);
            dynCam = new Camera(new Vector3(0, 0f, 2f), this.Width, this.Height, shader.ShaderProgramID);
            dynCam.Active = true;

            lights.Add(new PointLight(new Vector4(0, 0.4f, 1f, 1), shader.ShaderProgramID));

            GL.GenVertexArrays(VAO_IDs.Length, VAO_IDs);

            Material groundMat = MakeMaterial(new Vector3(0.25f, 0.20725f, 0.20725f), 
                new Vector3(1, 0.829f, 0.829f), 
                new Vector3(0.296648f, 0.296648f, 0.296648f), 0.088f);
            ground = new Cube(new Vector3(0f, -0.06f, 0f), new Vector3(10f, 0.01f, 10f), new Vector3(0f, 0.5f, 0.5f), shader.ShaderProgramID, VAO_IDs[0], groundMat);
            entities.Add(ground);


            Material cubeMat = MakeMaterial(new Vector3(0, 0.1f, 0.06f),
                new Vector3(0,0.50980392f, 0.50980392f),
                new Vector3(0.50196078f, 0.50196078f, 0.50196078f), 0.25f);
            cube = new Cube(new Vector3(0f, 0.1f, 0f), new Vector3(0.15f, 0.15f, 0.15f), new Vector3(0.4f, 0.3f, 0.8f), shader.ShaderProgramID, VAO_IDs[1], cubeMat);
            entities.Add(cube);

            GL.BindVertexArray(0);

 	        base.OnLoad(e);
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

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
 	        base.OnUpdateFrame(e);

            dynCam.Update();
            staticCam.Update();
            if (dynCam.Active) { ActiveCam = dynCam; }
            else { ActiveCam = staticCam; }
            for (int i = 0; i < lights.Count; i++) { lights[i].Update(ActiveCam); }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            for (int i = 0; i < entities.Count; i++)
            {
                GL.BindVertexArray(VAO_IDs[i]);
                entities[i].Update();
                entities[i].Draw();
            }

            GL.BindVertexArray(0);
            SwapBuffers();
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            cube.Dispose();
            ground.Dispose();
            GL.DeleteVertexArrays(VAO_IDs.Length, VAO_IDs);
            shader.Delete();
            base.OnUnload(e);
        }
    }
}
