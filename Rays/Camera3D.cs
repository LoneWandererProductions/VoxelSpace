using System;
using System.Numerics;

namespace Rays;

/// <summary>
///     Represents a camera in 3D space for the dungeon view.
/// </summary>
public class Camera3D
{
    public Vector3 Position { get; set; } = new(0, 1.7f, 0); // Player eye height
    public float Yaw { get; set; } = 0; // Horizontal rotation
    public float Pitch { get; set; } = 0; // Vertical rotation
    public float Fov { get; set; } = (float)(Math.PI / 3); // 60°


    public Matrix4x4 GetViewMatrix()
    {
        // Build orientation
        var rotation = Matrix4x4.CreateFromYawPitchRoll(Yaw, Pitch, 0);

        // Basis vectors
        var forward = Vector3.TransformNormal(Vector3.UnitZ, rotation);
        var up = Vector3.TransformNormal(Vector3.UnitY, rotation);

        return Matrix4x4.CreateLookAt(Position, Position + forward, up);
    }

    public Matrix4x4 GetProjectionMatrix(float aspectRatio, float near = 0.1f, float far = 100f)
    {
        return Matrix4x4.CreatePerspectiveFieldOfView(Fov, aspectRatio, near, far);
    }
}
