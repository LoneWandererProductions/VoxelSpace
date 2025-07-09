/*
 * COPYRIGHT:   See COPYING in the top level directory
 * PROJECT:     RenderEngine
 * FILE:        PerspectiveMatrizes.cs
 * PURPOSE:     Your file purpose here
 * PROGRAMMER:  Peter Geinitz (Wayfarer)
 */

using OpenTK.Mathematics;
using System;

namespace RenderEngine
{
    public class PerspectiveMatrizes
    {
        private readonly RvCamera _camera;

        public PerspectiveMatrizes(RvCamera camera)
        {
            _camera = camera ?? throw new ArgumentNullException(nameof(camera));
        }

        public Matrix4 GetProjectionMatrix(float aspectRatio)
        {
            return Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(60f), aspectRatio, 0.1f, _camera.ZFar);
        }

        public Matrix4 GetViewMatrix()
        {
            var eye = new Vector3(
                _camera.X + 0.5f,
                _camera.Y + 0.5f,
                _camera.Z
            );

            float angleRad = MathHelper.DegreesToRadians(_camera.Angle);
            float pitchRad = MathHelper.DegreesToRadians(_camera.Pitch);

            var direction = new Vector3(
                (float)Math.Cos(angleRad),
                (float)Math.Sin(angleRad),
                (float)Math.Sin(pitchRad)
            );

            return Matrix4.LookAt(eye, eye + direction, Vector3.UnitZ);
        }
    }
}
