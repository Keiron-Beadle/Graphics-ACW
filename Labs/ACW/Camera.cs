using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labs.ACW
{
    class Camera
    {
        private Matrix4 viewMat;
        private Matrix4 projMat;
        private Vector4 eyePosition;
        int uViewLocation, uProjectionLocation;
        int[] shaderIDs;
        public bool Active { get; set; }
        private const float moveSpd = 0.05f;
        private const float rotSpd = 0.025f;
        public Matrix4 ProjectionMatrix { get { return projMat; } }
        public Matrix4 ViewMatrix { get { return viewMat; } }
        public Vector4 Position { get { return eyePosition; } }

        public void SetViewMatrix(Matrix4 mat) { viewMat = mat; }

        public Camera(Vector3 inPosition, float clientWidth, float clientHeight, int[] shaderIDs)
        : this (inPosition, Vector3.Zero,clientWidth, clientHeight, shaderIDs) { }

        public Camera(Vector3 inPosition, Vector3 pLookAt, float clientWidth, float clientHeight, int[] pShaderIDs)
        {
            shaderIDs = pShaderIDs;
            //eyePosition = new Vector4(inPosition,1);
            Vector3 lookAt = pLookAt;
            projMat = Matrix4.CreatePerspectiveFieldOfView(1, clientWidth / clientHeight, 0.01f, 50f);
            viewMat = Matrix4.LookAt(inPosition, lookAt, Vector3.UnitY);
            //viewMat = Matrix4.Identity;
            for (int i = 0; i < shaderIDs.Length; i++)
            {
                GL.UseProgram(shaderIDs[i]);
                uViewLocation = GL.GetUniformLocation(shaderIDs[i], "uView");
                //GL.UniformMatrix4(uViewLocation, true, ref viewMat);
                uProjectionLocation = GL.GetUniformLocation(shaderIDs[i], "uProjection");
                GL.UniformMatrix4(uProjectionLocation, true, ref projMat);
                UpdateUEyeLocation(shaderIDs[i]);
            }
        }

        public void Update()
        {
            if (!Active) { return; }
            for (int i = 0; i < shaderIDs.Length; i++)
            {
                GL.UseProgram(shaderIDs[i]);
                uViewLocation = GL.GetUniformLocation(shaderIDs[i], "uView");
                GL.UniformMatrix4(uViewLocation, true, ref viewMat);
            }
        }

        public void OnKeyDown(KeyboardKeyEventArgs e)
        {
            if (!Active) { return; }
            Matrix4 temp = Matrix4.Identity;
            if (e.Key == Key.Down)
            {
                temp *= Matrix4.CreateRotationX(rotSpd);
            }
            if (e.Key == Key.Up)
            {
                temp *= Matrix4.CreateRotationX(-rotSpd);
            }
            if (e.Key == Key.Q)
            {
                temp *= Matrix4.CreateRotationY(-rotSpd);
            }
            if (e.Key == Key.E)
            {
                temp *= Matrix4.CreateRotationY(rotSpd);
            }
            if (e.Key == Key.W)
            {
                temp *= Matrix4.CreateTranslation(0.0f, 0.0f, moveSpd);
            }
            if (e.Key == Key.S)
            {
                temp *= Matrix4.CreateTranslation(0.0f, 0.0f, -moveSpd);
            }
            if (e.Key == Key.A)
            {
                temp *= Matrix4.CreateTranslation(moveSpd, 0.0f, 0.0f);
            }
            if (e.Key == Key.D)
            {
                temp *= Matrix4.CreateTranslation(-moveSpd, 0.0f, 0.0f);
            }
            if (e.Key == Key.Space)
            {
                temp *= Matrix4.CreateTranslation(0.0f, -moveSpd, 0.0f);
            }
            if (e.Key == Key.ShiftLeft)
            {
                temp *= Matrix4.CreateTranslation(0.0f, moveSpd, 0.0f);
            }
            viewMat *= temp;
            for (int i = 0; i < shaderIDs.Length; i++)
            {
              //  GL.UseProgram(shaderIDs[i]);
                uViewLocation = GL.GetUniformLocation(shaderIDs[i], "uView");
                GL.UniformMatrix4(uViewLocation, true, ref viewMat);
                UpdateUEyeLocation(shaderIDs[i]);
            }
        }

        private void UpdateUEyeLocation(int shaderProgramID)
        {
            eyePosition = new Vector4(viewMat.ExtractTranslation(), 1);
            int eyeLocation = GL.GetUniformLocation(shaderProgramID, "uEyePosition");
            GL.Uniform4(eyeLocation, eyePosition);
        }
    }
}
