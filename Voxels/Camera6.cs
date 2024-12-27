public class Camera6
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Fov { get; set; }
    public int Angle { get; set; }

    public Camera6(double x, double y, double fov, int direction)
    {
        X = x;
        Y = y;
        Fov = fov;
        Angle = direction;
    }

    public override string ToString()
    {
        return $"Camera6 [X={X:F2}, Y={Y:F2}, FOV={Fov:F2}, Direction={Angle}°]";
    }
}