﻿using Labs.ACW.Objects;
using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

namespace Labs.ACW
{
    public class ACWWindow : GameWindow
    {
        private Camera staticCam, dynCam;
        private int[] VAO_IDs = new int[2];
        private ShaderUtility shader;
        private Cube cube, ground; 

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

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.1f,0.1f,0.1f,1.0f);
            GL.Enable(EnableCap.DepthTest);
            shader = new ShaderUtility(@"ACW/Shaders/vShader.vert", @"ACW/Shaders/fShader.frag");
            GL.UseProgram(shader.ShaderProgramID);
            int vPositionLocation = GL.GetAttribLocation(shader.ShaderProgramID, "vPosition");
            int vColourLocation = GL.GetAttribLocation(shader.ShaderProgramID, "vColour");

            //staticCam = new Camera(new Vector3(0, 0, 2), this.Width, this.Height, shader.ShaderProgramID);
            dynCam = new Camera(new Vector3(0, 0f, 1f), this.Width, this.Height, shader.ShaderProgramID);
            dynCam.Active = true;

            GL.GenVertexArrays(VAO_IDs.Length, VAO_IDs);

            GL.BindVertexArray(VAO_IDs[0]);
            ground = new Cube(new Vector3(0, -0.2f, 0), new Vector3(10f, 0.1f, 10f), new Vector3(0.7f, 0.1f, 0.2f), shader.ShaderProgramID);

            GL.BindVertexArray(VAO_IDs[1]);
            cube = new Cube(new Vector3(0f, 0f, 0f), new Vector3(0.15f, 0.15f, 0.15f), new Vector3(0.6f, 0.3f, 0f), shader.ShaderProgramID);

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(vColourLocation);
            GL.VertexAttribPointer(vColourLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

            GL.BindVertexArray(0);

 	        base.OnLoad(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            dynCam.OnKeyPress(e);
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
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindVertexArray(VAO_IDs[0]);
            ground.Draw();
           // GL.BindVertexArray(VAO_IDs[1]);
           // cube.Draw();

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
