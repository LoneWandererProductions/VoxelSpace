using System;
using System.Drawing;
using System.Numerics;

namespace Rays
{
    public static class RasterHelpers
    {
        /// <summary>
        /// Projects a 3D point in clip space to screen coordinates.
        /// </summary>
        public static PointF ProjectToScreen(Vector3 pos, int screenWidth, int screenHeight)
        {
            // Assuming pos is in clip space [-1,1]
            float x = (pos.X + 1f) * 0.5f * screenWidth;
            float y = (1f - (pos.Y + 1f) * 0.5f) * screenHeight; // Flip Y
            return new PointF(x, y);
        }

        /// <summary>
        /// Transforms world position by VP matrix and performs perspective divide.
        /// </summary>
        public static Vector3 TransformToClip(Vector3 worldPos, Matrix4x4 vp)
        {
            Vector4 clip = Vector4.Transform(new Vector4(worldPos, 1f), vp);
            if (clip.W != 0f)
                clip /= clip.W; // perspective divide
            return new Vector3(clip.X, clip.Y, clip.Z);
        }

        /// <summary>
        /// Projects 4 vertices and returns screen-space points.
        /// </summary>
        public static PointF[] ProjectQuad(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Matrix4x4 vp, int screenWidth, int screenHeight)
        {
            return new[]
            {
                ProjectToScreen(TransformToClip(v0, vp), screenWidth, screenHeight),
                ProjectToScreen(TransformToClip(v1, vp), screenWidth, screenHeight),
                ProjectToScreen(TransformToClip(v2, vp), screenWidth, screenHeight),
                ProjectToScreen(TransformToClip(v3, vp), screenWidth, screenHeight)
            };
        }

        /// <summary>
        /// Computes normal of the quad (for backface culling).
        /// </summary>
        public static Vector3 ComputeNormal(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            return Vector3.Normalize(Vector3.Cross(v1 - v0, v2 - v0));
        }
    }
}
