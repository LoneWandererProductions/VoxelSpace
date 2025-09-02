using System.Numerics;

namespace Rays;

public class Actor
{
    public bool IsOnGround = true;
    public Vector3 Position;
    public float VerticalVelocity;

    public void Update(float deltaTime, float groundHeight = 1.7f)
    {
        if (IsOnGround) return;

        // Apply gravity
        VerticalVelocity += -9.8f * deltaTime;
        Position.Y += VerticalVelocity * deltaTime;

        // Check if we've hit the ground
        if (Position.Y <= groundHeight)
        {
            Position.Y = groundHeight;
            IsOnGround = true;
            VerticalVelocity = 0f;
        }
    }

    public void Jump()
    {
        if (!IsOnGround) return;

        VerticalVelocity = 5f;
        IsOnGround = false;
    }
}
