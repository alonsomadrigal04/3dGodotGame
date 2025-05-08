using Godot;
using System;

public partial class Bullet : RigidBody3D
{
    [Export] public float Speed = 30f;
    [Export] public float YOffset = 0.2f;

    public override void _Ready()
    {
        GetTree().CreateTimer(3.0f).Timeout += QueueFree;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 forward = -GlobalTransform.Basis.Z;
        Vector3 upward = GlobalTransform.Basis.Y * YOffset;

        Vector3 direction = (forward + upward).Normalized();
        LinearVelocity = direction * Speed;
    }
}
