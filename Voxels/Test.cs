using System;
using System.Collections.Generic;
using System.Drawing;

namespace Voxels
{
    public class VoxelRendererWithFov
    {
        private readonly int[,,] _voxelMap; // 3D Voxel Map
        private readonly int _cellSize;
        private readonly int _mapWidth;
        private readonly int _mapHeight;
        private readonly int _mapDepth;

        public VoxelRendererWithFov(int[,,] voxelMap, int cellSize)
        {
            _voxelMap = voxelMap;
            _cellSize = cellSize;
            _mapWidth = voxelMap.GetLength(0);
            _mapHeight = voxelMap.GetLength(1);
            _mapDepth = voxelMap.GetLength(2);
        }

        public Bitmap RenderVoxelMap(Camera6 camera, int screenWidth, int screenHeight)
        {
            Bitmap bitmap = new(screenWidth, screenHeight);
            using var g = Graphics.FromImage(bitmap);
            g.Clear(Color.Black);

            var halfFov = DegreeToRadian(camera.Fov / 2.0);
            var angleStep = camera.Fov / screenWidth;

            for (int x = 0; x < screenWidth; x++)
            {
                var rayAngle = DegreeToRadian((camera.Angle - (camera.Fov / 2.0)) + x * angleStep);
                var rayDirX = Math.Cos(rayAngle);
                var rayDirY = Math.Sin(rayAngle);

                var (distance, hitZ) = CastVoxelRay(camera.X, camera.Y, camera.Z, rayDirX, rayDirY);

                if (distance < double.MaxValue)
                {
                    RenderVoxelColumn(g, x, screenWidth, screenHeight, distance, hitZ, halfFov);
                }
            }

            return bitmap;
        }

        private (double distance, int hitZ) CastVoxelRay(double startX, double startY, double startZ, double rayDirX, double rayDirY)
        {
            double x = startX;
            double y = startY;
            double z = startZ;

            while (true)
            {
                int mapX = (int)(x / _cellSize);
                int mapY = (int)(y / _cellSize);
                int mapZ = (int)(z / _cellSize);

                if (mapX < 0 || mapX >= _mapWidth || mapY < 0 || mapY >= _mapHeight || mapZ < 0 || mapZ >= _mapDepth)
                    return (double.MaxValue, -1); // Out of bounds.

                if (_voxelMap[mapX, mapY, mapZ] > 0)
                    return (Math.Sqrt((x - startX) * (x - startX) + (y - startY) * (y - startY)), mapZ);

                x += rayDirX * 0.1; // Step ray.
                y += rayDirY * 0.1;
                z += 0.1; // Step through Z.
            }
        }

        private void RenderVoxelColumn(Graphics g, int screenX, int screenWidth, int screenHeight, double distance, int hitZ, double halfFov)
        {
            var wallHeight = (int)((screenHeight / distance) * halfFov);
            var wallTop = Math.Max(0, (screenHeight - wallHeight) / 2);
            var wallBottom = Math.Min(screenHeight, (screenHeight + wallHeight) / 2);

            var color = GetVoxelColor(hitZ, distance);
            g.DrawLine(new Pen(color), screenX, wallTop, screenX, wallBottom);
        }

        private Color GetVoxelColor(int z, double distance)
        {
            var baseColor = Color.FromArgb(50 + z * 20, 50 + z * 20, 255 - z * 20);
            var intensity = Math.Max(0, 255 - (int)(distance * 10));
            return Color.FromArgb(intensity, baseColor.R, baseColor.G, baseColor.B);
        }

        private static double DegreeToRadian(double degree) => degree * Math.PI / 180.0;
    }

    public class Camera6
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public double Angle { get; set; }
        public double Fov { get; set; }

        public Camera6(double x, double y, double z, double angle, double fov)
        {
            X = x;
            Y = y;
            Z = z;
            Angle = angle;
            Fov = fov;
        }
    }
}
