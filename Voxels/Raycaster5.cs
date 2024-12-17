using System;
using System.Diagnostics;
using System.Drawing;

public class Camera5
{
    public int CellSize { get; set; } = 64; // Default cell size
    public double WorldX { get; private set; }
    public double WorldY { get; private set; }
    public double Angle { get; set; }
    public double FieldOfView { get; set; } = Math.PI / 2;
    public double MovementSpeed { get; set; } = 0.1;

    public Camera5(double worldX, double worldY, double angle)
    {
        WorldX = worldX;
        WorldY = worldY;
        Angle = angle;
    }

    public void Move(double deltaX, double deltaY)
    {
        WorldX += deltaX;
        WorldY += deltaY;
    }
}

public class Raycaster5
{
    private readonly int[,] Map;
    private const int ScreenWidth = 800;
    private const int ScreenHeight = 600;
    private int MaxViewDistance; // Dynamically set based on CellSize
    private const int RayCount = ScreenWidth;

    public Raycaster5(int[,] map, int cellSize)
    {
        Map = map;
        MaxViewDistance = 4 * cellSize; // Adjust max view distance dynamically
    }

    public Bitmap RenderBitmap(Camera2 camera)
    {
        Bitmap bitmap = new Bitmap(ScreenWidth, ScreenHeight);
        using Graphics g = Graphics.FromImage(bitmap);
        g.Clear(Color.Black);

        double rayStep = camera.FieldOfView / RayCount;

        for (int i = 0; i < RayCount; i++)
        {
            double rayAngle = camera.Angle - camera.FieldOfView / 2 + rayStep * i;
            double distance = CastRay(camera.WorldX, camera.WorldY, rayAngle, camera.CellSize, out bool hitWall);

            if (hitWall)
            {
                DrawWall(g, i, distance);
            }
        }

        return bitmap;
    }

    private double CastRay(double startX, double startY, double angle, int cellSize, out bool hitWall)
    {
        hitWall = false;

        double rayX = startX;
        double rayY = startY;

        double rayDirX = Math.Cos(angle);
        double rayDirY = Math.Sin(angle);
        double distance = 0;
        double stepSize = 0.1 * cellSize; // Scale step size by cell size

        while (distance < MaxViewDistance)
        {
            rayX += rayDirX * stepSize;
            rayY += rayDirY * stepSize;
            distance += stepSize;

            int mapX = (int)(rayX / cellSize);
            int mapY = (int)(rayY / cellSize);

            Trace.WriteLine($"Ray Position: ({rayX:F2}, {rayY:F2}) -> Map Cell: ({mapX}, {mapY}), Distance: {distance:F2}");

            if (mapX < 0 || mapY < 0 || mapX >= Map.GetLength(0) || mapY >= Map.GetLength(1))
            {
                Console.WriteLine("Ray went out of bounds.");
                break;
            }

            if (Map[mapX, mapY] > 0)
            {
                Console.WriteLine($"Wall hit at ({mapX}, {mapY})!");
                hitWall = true;
                break;
            }
        }

        return distance;
    }

    private void DrawWall(Graphics g, int screenColumn, double distance)
    {
        // Scale wall height based on distance
        int wallHeight = Math.Min(ScreenHeight, (int)(ScreenHeight / Math.Max(1, distance)));
        int wallTop = (ScreenHeight - wallHeight) / 2;
        int wallBottom = wallTop + wallHeight;

        g.DrawLine(Pens.Green, screenColumn, wallTop, screenColumn, wallBottom);
    }
}
