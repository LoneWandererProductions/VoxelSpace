public class Camera6
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Fov { get; set; }
    public double Direction { get; set; }

    public Camera6(double x, double y, double fov, double direction)
    {
        X = x;
        Y = y;
        Fov = fov;
        Direction = direction;
    }
}