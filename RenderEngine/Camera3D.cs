using System;
using System.Collections.Generic;
using System.Drawing;

namespace Voxels
{
    public sealed class Camera3D
    {
        public Camera3D(int x, int y, int direction)
        {
            X = x;
            Y = y;
            Angle = direction;
        }

    public Camera3D()
        {
        }

        public Color BackgroundColor { get; set; } = Color.Cyan;
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int Horizon { get; set; }
        public float ZFar { get; init; }
        public int Angle { get; set; }
        public int Pitch { get; set; }
        public float FieldOfView { get; set; } = 90f; // Field of view in degrees
        public Size ViewportSize { get; set; } = new Size(800, 600); // Screen size for rendering

        public override string ToString()
        {
            return $"Camera [X={X}, Y={Y}, Z={Z}, Angle={Angle}, Horizon={Horizon}, ZFar={ZFar}], Pitch= {Pitch}, FOV= {FieldOfView}";
        }

        /// <summary>
        /// Projects a 3D point into 2D space based on the camera's position and perspective.
        /// </summary>
        public PointF ProjectPoint(int worldX, int worldY, int worldZ)
        {
            float dx = worldX - X;
            float dy = worldY - Y;
            float dz = worldZ - Z;

            // Rotate point based on the camera angle
            float cosAngle = (float)Math.Cos(Angle * Math.PI / 180);
            float sinAngle = (float)Math.Sin(Angle * Math.PI / 180);
            float rotatedX = dx * cosAngle - dy * sinAngle;
            float rotatedY = dx * sinAngle + dy * cosAngle;

            // Perspective projection
            float distance = rotatedX; // Distance along the camera's viewing direction
            if (distance <= 0.1f) return PointF.Empty; // Point is behind the camera

            float scale = ViewportSize.Width / (2f * (float)Math.Tan(FieldOfView * Math.PI / 360));
            float screenX = scale * rotatedY / distance + ViewportSize.Width / 2;
            float screenY = scale * dz / distance + ViewportSize.Height / 2;

            return new PointF(screenX, screenY);
        }

        /// <summary>
        /// Calculates the visible edges of a voxel at the given world coordinates.
        /// </summary>
        public List<(PointF, PointF)> CalculateVoxelEdges(int voxelX, int voxelY, int voxelZ, int voxelSize)
        {
            var edges = new List<(PointF, PointF)>();

            // Define the voxel's corners
            var corners = new[]
            {
                new Point3D(voxelX, voxelY, voxelZ),
                new Point3D(voxelX + voxelSize, voxelY, voxelZ),
                new Point3D(voxelX, voxelY + voxelSize, voxelZ),
                new Point3D(voxelX, voxelY, voxelZ + voxelSize),
                new Point3D(voxelX + voxelSize, voxelY + voxelSize, voxelZ),
                new Point3D(voxelX + voxelSize, voxelY, voxelZ + voxelSize),
                new Point3D(voxelX, voxelY + voxelSize, voxelZ + voxelSize),
                new Point3D(voxelX + voxelSize, voxelY + voxelSize, voxelZ + voxelSize),
            };

            // Define the voxel's edges (pairs of corner indices)
            var edgeIndices = new[]
            {
                (0, 1), (0, 2), (0, 3),
                (1, 4), (1, 5), (2, 4),
                (2, 6), (3, 5), (3, 6),
                (4, 7), (5, 7), (6, 7)
            };

            // Project edges into 2D space
            foreach (var (startIndex, endIndex) in edgeIndices)
            {
                var start = ProjectPoint(corners[startIndex].X, corners[startIndex].Y, corners[startIndex].Z);
                var end = ProjectPoint(corners[endIndex].X, corners[endIndex].Y, corners[endIndex].Z);

                if (start != PointF.Empty && end != PointF.Empty)
                {
                    edges.Add((start, end));
                }
            }

            return edges;
        }

        private struct Point3D
        {
            public int X, Y, Z;
            public Point3D(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }
        }
    }
}
