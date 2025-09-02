using System.Numerics;

namespace Rays;

public partial class DungeonRenderer
{
    /// <summary>
    /// Lightweight frustum culling helper
    /// </summary>
    public static class FrustumCulling
    {
        /// <summary>
        /// Checks if a cell (axis-aligned cube) is at least partially inside the view frustum.
        /// </summary>
        public static bool IsCellVisible(Matrix4x4 vp, Vector3 min, Vector3 max)
        {
            // 8 corners of the cube
            var corners = new[]
            {
            new Vector3(min.X, min.Y, min.Z),
            new Vector3(max.X, min.Y, min.Z),
            new Vector3(min.X, max.Y, min.Z),
            new Vector3(max.X, max.Y, min.Z),
            new Vector3(min.X, min.Y, max.Z),
            new Vector3(max.X, min.Y, max.Z),
            new Vector3(min.X, max.Y, max.Z),
            new Vector3(max.X, max.Y, max.Z)
        };

            foreach (var corner in corners)
            {
                var clip = Vector3.Transform(corner, vp);

                // simple frustum check in clip space
                if (clip.Z > 0 && clip.X >= -clip.Z && clip.X <= clip.Z &&
                    clip.Y >= -clip.Z && clip.Y <= clip.Z)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if a point is visible in the frustum.
        /// </summary>
        public static bool IsPointVisible(Matrix4x4 vp, Vector3 point)
        {
            var clip = Vector3.Transform(point, vp);
            return clip.Z > 0 &&
                   clip.X >= -clip.Z && clip.X <= clip.Z &&
                   clip.Y >= -clip.Z && clip.Y <= clip.Z;
        }

        /// <summary>
        /// Checks if a sphere is at least partially visible.
        /// </summary>
        public static bool IsSphereVisible(Matrix4x4 vp, Vector3 center, float radius)
        {
            var clip = Vector3.Transform(center, vp);

            // simple AABB-like approximation in clip space
            return clip.Z - radius <= clip.Z &&
                   clip.X + radius >= -clip.Z && clip.X - radius <= clip.Z &&
                   clip.Y + radius >= -clip.Z && clip.Y - radius <= clip.Z;
        }
    }
}