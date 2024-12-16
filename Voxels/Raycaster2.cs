using System;
using System.Drawing;

public class Camera2
{
    public int CellSize { get; set; } = 64;
    public double WorldX { get; private set; }
    public double WorldY { get; private set; }
    public double Angle { get; set; }
    public double FieldOfView { get; set; } = Math.PI / 2;
    public double MovementSpeed { get; set; } = 0.1;

    public Camera2(double worldX, double worldY, double angle)
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

public class Raycaster2
{
    private readonly int[,] Map;
    private const int ScreenWidth = 800;
    private const int ScreenHeight = 600;
    private const double MaxViewDistance = 64 * 4; // Max render distance in world units
    private const int RayCount = ScreenWidth;

    public Raycaster2(int[,] map)
    {
        Map = map;
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
            double distance = CastRay(camera.WorldX, camera.WorldY, rayAngle, out bool hitWall);

            if (hitWall)
            {
                DrawWall(g, i, distance);
            }
        }

        return bitmap;
    }

    private double CastRay(double startX, double startY, double angle, out bool hitWall)
    {
        hitWall = false;
        double rayX = startX;
        double rayY = startY;
        double rayDirX = Math.Cos(angle);
        double rayDirY = Math.Sin(angle);
        double distance = 0;
        double stepSize = 0.1;

        while (distance < MaxViewDistance)
        {
            rayX += rayDirX * stepSize;
            rayY += rayDirY * stepSize;
            distance += stepSize;

            int mapX = (int)rayX;
            int mapY = (int)rayY;

            if (mapX < 0 || mapY < 0 || mapX >= Map.GetLength(0) || mapY >= Map.GetLength(1))
            {
                break; // Out of bounds
            }

            if (Map[mapX, mapY] > 0)
            {
                hitWall = true;
                break;
            }
        }

        return distance;
    }

    private void DrawWall(Graphics g, int screenColumn, double distance)
    {
        int wallHeight = (int)(ScreenHeight / distance);
        int wallTop = (ScreenHeight - wallHeight) / 2;
        int wallBottom = wallTop + wallHeight;

        g.DrawLine(Pens.White, screenColumn, wallTop, screenColumn, wallBottom);
    }
}