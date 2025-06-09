using System;
using System.Drawing;
using Imaging;
using Viewer;

namespace Rays
{
    public sealed class RaycasterV2
    {
        private readonly CameraContext _context;
        private readonly MapCell[,] _map;
        private readonly int _mapHeight;
        private readonly int _mapWidth;
        private readonly DirectBitmap[] _wallTextures;
        private readonly IFloorCeilingRenderer? _floorCeilingRenderer;

        public RaycasterV2(MapCell[,] map, CameraContext context, DirectBitmap[] wallTextures, IFloorCeilingRenderer? floorCeilingRenderer = null)
        {
            _map = map;
            _mapWidth = map.GetLength(1);
            _mapHeight = map.GetLength(0);
            _context = context;
            _wallTextures = wallTextures;
            _floorCeilingRenderer = floorCeilingRenderer ?? new FlatFloorCeilingRenderer();
        }

        public RenderResult Render(RvCamera camera)
        {
            DirectBitmap dbm = new(_context.ScreenWidth, _context.ScreenHeight, Color.Black);

            // Render floor and ceiling first
            _floorCeilingRenderer?.Render(dbm, camera, _context);

            double halfFov = _context.Fov / 2.0;
            double angleStep = _context.Fov / _context.ScreenWidth;

            for (int x = 0; x < _context.ScreenWidth; x++)
            {
                double rayAngle = camera.Angle - halfFov + x * angleStep;
                double rayAngleRad = DegreeToRadian(rayAngle);

                double rayX = Math.Cos(rayAngleRad);
                double rayY = Math.Sin(rayAngleRad);

                var (distanceToWall, hitX, hitY, wallId) = CastRay(camera.X, camera.Y, rayX, rayY);

                if (distanceToWall > _context.Distance || wallId <= 0 || wallId > _wallTextures.Length)
                    continue;

                int wallHeight = (int)(_context.ScreenHeight / distanceToWall);
                int zOffset = (int)(camera.Z / _context.CellSize * wallHeight);
                int wallTop = Math.Max(0, (_context.ScreenHeight - wallHeight) / 2 - zOffset);
                int wallBottom = Math.Min(_context.ScreenHeight, (_context.ScreenHeight + wallHeight) / 2 - zOffset);

                DirectBitmap texture = _wallTextures[wallId - 1]; // assuming wallId is 1-based

                // Texture X offset
                double cellSize = _context.CellSize;
                double hitInCellX = hitX % cellSize;
                double hitInCellY = hitY % cellSize;

                bool isVertical = Math.Abs(hitInCellX - cellSize / 2) < Math.Abs(hitInCellY - cellSize / 2);

                int texX = isVertical
                    ? (int)((hitY % cellSize) / cellSize * texture.Width)
                    : (int)((hitX % cellSize) / cellSize * texture.Width);

                texX = Math.Clamp(texX, 0, texture.Width - 1);

                for (int y = wallTop; y < wallBottom; y++)
                {
                    double t = (y - wallTop) / (double)(wallBottom - wallTop);
                    int texY = (int)(t * texture.Height);
                    texY = Math.Clamp(texY, 0, texture.Height - 1);

                    Color baseColor = texture.GetPixel(texX, texY);
                    Color foggedColor = ApplyFog(baseColor, distanceToWall);
                    dbm.SetPixel(x, y, foggedColor);
                }
            }

            byte[] imageBytes = ToByteArray(dbm.Bits);

            return new RenderResult
            {
                Bitmap = dbm.Bitmap,
                Bytes = imageBytes
            };
        }

        private static byte[] ToByteArray(int[] bits)
        {
            var bytes = new byte[bits.Length * sizeof(int)];
            Buffer.BlockCopy(bits, 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private (double Distance, double HitX, double HitY, int WallId) CastRay(double startX, double startY, double rayDirX, double rayDirY)
        {
            double x = startX;
            double y = startY;

            while (true)
            {
                int mapX = (int)(x / _context.CellSize);
                int mapY = (int)(y / _context.CellSize);

                if (mapX < 0 || mapX >= _mapWidth || mapY < 0 || mapY >= _mapHeight)
                    return (double.MaxValue, 0, 0, 0);

                var cell = _map[mapY, mapX];
                if (cell.WallId > 0)
                {
                    double dx = x - startX;
                    double dy = y - startY;
                    double dist = Math.Sqrt(dx * dx + dy * dy) / _context.CellSize;
                    return (dist, x, y, cell.WallId);
                }

                x += rayDirX * 0.1;
                y += rayDirY * 0.1;
            }
        }

        private Color ApplyFog(Color baseColor, double distance)
        {
            double fogFactor = Math.Min(1.0, distance / _context.Distance);
            Color backgroundColor = Color.Black;

            int red = (int)((1 - fogFactor) * baseColor.R + fogFactor * backgroundColor.R);
            int green = (int)((1 - fogFactor) * baseColor.G + fogFactor * backgroundColor.G);
            int blue = (int)((1 - fogFactor) * baseColor.B + fogFactor * backgroundColor.B);

            return Color.FromArgb(red, green, blue);
        }

        private static double DegreeToRadian(double degree)
        {
            return degree * Math.PI / 180.0;
        }
    }
}
