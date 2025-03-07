using Godot;
using System;

public partial class FlyMovement : RigidBody3D
{
	[ExportGroup("Movimiento")]
    [Export] private float minSpeed = 3f;
    [Export] private float maxSpeed = 8f;
    [Export] private float rotationSpeed = 2f;

    [ExportGroup("Control")]
    [Export] private float speedLerpFactor = 5f;
    [Export] private float aceleration = 5f;


    private Vector2 mouseDelta = Vector2.Zero;
    private float currentSpeed;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        currentSpeed = maxSpeed;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            mouseDelta = mouseMotion.Relative;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        RotatePlayer(delta);
        AdjustSpeed(delta);
        MovePlayer(delta);
    }

    private void RotatePlayer(double delta)
    {
        Rotate(Vector3.Up, -mouseDelta.X * rotationSpeed * 0.0005f);

        Vector3 newRotation = Rotation;
        newRotation.X = Mathf.Clamp(newRotation.X - mouseDelta.Y * rotationSpeed * 0.0005f	, -Mathf.Pi / 2, Mathf.Pi / 2);
        Rotation = newRotation;

        mouseDelta = Vector2.Zero;
    }

    private void AdjustSpeed(double delta)
    {
        float targetSpeed = Input.GetLastMouseVelocity().Length() > 0 ? minSpeed : maxSpeed;

        GD.Print(LinearVelocity.Length());
		
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, (float)delta * speedLerpFactor);
    }

    private void MovePlayer(double delta)
    {

        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed * aceleration, (float)delta * speedLerpFactor);
        }

        Vector3 targetVelocity = -GlobalTransform.Basis.Z * currentSpeed;

        LinearVelocity = targetVelocity;
    }
}
