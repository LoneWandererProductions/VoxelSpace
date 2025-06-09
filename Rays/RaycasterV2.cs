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

            var halfFov = _context.Fov / 2.0;
            var angleStep = _context.Fov / _context.ScreenWidth;

            for (var x = 0; x < _context.ScreenWidth; x++)
            {
                var rayAngle = camera.Angle - halfFov + x * angleStep;
                var rayAngleRad = DegreeToRadian(rayAngle);

                var rayX = Math.Cos(rayAngleRad);
                var rayY = Math.Sin(rayAngleRad);

                var (distanceToWall, hitX, hitY, wallId) = CastRay(camera.X, camera.Y, rayX, rayY);

                if (distanceToWall > _context.Distance || wallId <= 0 || wallId > _wallTextures.Length)
                    continue;

                var wallHeight = (int)(_context.ScreenHeight / distanceToWall);
                var zOffset = (int)(camera.Z / _context.CellSize * wallHeight);
                var wallTop = Math.Max(0, (_context.ScreenHeight - wallHeight) / 2 - zOffset);
                var wallBottom = Math.Min(_context.ScreenHeight, (_context.ScreenHeight + wallHeight) / 2 - zOffset);

                var texture = _wallTextures[wallId - 1]; // assuming wallId is 1-based

                // Texture X offset
                double cellSize = _context.CellSize;
                var hitInCellX = hitX % cellSize;
                var hitInCellY = hitY % cellSize;

                var isVertical = Math.Abs(hitInCellX - cellSize / 2) < Math.Abs(hitInCellY - cellSize / 2);

                var texX = isVertical
                    ? (int)((hitY % cellSize) / cellSize * texture.Width)
                    : (int)((hitX % cellSize) / cellSize * texture.Width);

                texX = Math.Clamp(texX, 0, texture.Width - 1);

                for (var y = wallTop; y < wallBottom; y++)
                {
                    var t = (y - wallTop) / (double)(wallBottom - wallTop);
                    var texY = (int)(t * texture.Height);
                    texY = Math.Clamp(texY, 0, texture.Height - 1);

                    var baseColor = texture.GetPixel(texX, texY);
                    var foggedColor = ApplyFog(baseColor, distanceToWall);
                    dbm.SetPixel(x, y, foggedColor);
                }
            }

            var imageBytes = ToByteArray(dbm.Bits);

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
            var x = startX;
            var y = startY;

            while (true)
            {
                var mapX = (int)(x / _context.CellSize);
                var mapY = (int)(y / _context.CellSize);

                if (mapX < 0 || mapX >= _mapWidth || mapY < 0 || mapY >= _mapHeight)
                    return (double.MaxValue, 0, 0, 0);

                var cell = _map[mapY, mapX];
                if (cell.WallId > 0)
                {
                    var dx = x - startX;
                    var dy = y - startY;
                    var dist = Math.Sqrt(dx * dx + dy * dy) / _context.CellSize;
                    return (dist, x, y, cell.WallId);
                }

                x += rayDirX * 0.1;
                y += rayDirY * 0.1;
            }
        }

        private Color ApplyFog(Color baseColor, double distance)
        {
            var fogFactor = Math.Min(1.0, distance / _context.Distance);
            var backgroundColor = Color.Black;

            var red = (int)((1 - fogFactor) * baseColor.R + fogFactor * backgroundColor.R);
            var green = (int)((1 - fogFactor) * baseColor.G + fogFactor * backgroundColor.G);
            var blue = (int)((1 - fogFactor) * baseColor.B + fogFactor * backgroundColor.B);

            return Color.FromArgb(red, green, blue);
        }

        private static double DegreeToRadian(double degree)
        {
            return degree * Math.PI / 180.0;
        }
    }
}
