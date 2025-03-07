using Godot;
using System;

public partial class PlayerMovementNew : CharacterBody3D
{
	[Export] private float maxSpeed = 80.0f;
    [Export] private float minSpeed = 20.0f;
    [Export] private float acceleration = 15.5f;
    [Export] private float deceleration = 10.5f;
    [Export] private float currentSpeed = 50.0f;

    [Export] private float yawSpeed = 45.0f;
    [Export] private float pitchSpeed = 45.0f;
    [Export] private float rollSpeed = 45.0f;

    // [Export] private Node3D prop;
    [Export] private Node3D planeMesh;
    private Vector2 turnInput = Vector2.Zero;

    public override void _Ready()
    {
        pitchSpeed = Mathf.DegToRad(pitchSpeed);
        yawSpeed = Mathf.DegToRad(yawSpeed);
        rollSpeed = Mathf.DegToRad(rollSpeed);
        
        //prop = GetNode<Node3D>("Plane2/Plane/propellor");
        // planeMesh = GetNode<Node3D>("Plane2");
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;
        Vector2 input = Input.GetVector("ui_left", "ui_right", "ui_down", "ui_up");
        float roll = Input.GetAxis("roll_left", "roll_right");

		GD.Print(input, roll);

        if (input.Y > 0 && currentSpeed < maxSpeed)
            currentSpeed += acceleration * dt;
        else if (input.Y < 0 && currentSpeed > minSpeed)
            currentSpeed -= deceleration * dt;

        Velocity = -Basis.Z * currentSpeed;

        
        MoveAndSlide();

        Vector3 turnDir = new Vector3(-turnInput.Y, -turnInput.X, -roll);
        ApplyRotation(turnDir, dt);
        turnInput = Vector2.Zero;

        // SpinPropellor(dt);
    }

    private void ApplyRotation(Vector3 vector, float delta)
    {
        Rotate(Basis.Z, vector.Z * rollSpeed * delta);
        Rotate(Basis.X, vector.X * pitchSpeed * delta);
        Rotate(Basis.Y, vector.Y * yawSpeed * delta);

        // Lean mesh
        if (vector.Y < 0)
        {
            planeMesh.Rotation = new Vector3(
                planeMesh.Rotation.X,
                planeMesh.Rotation.Y,
                Mathf.LerpAngle(planeMesh.Rotation.Z, Mathf.DegToRad(-45) * -vector.Y, delta)
            );
        }
        else if (vector.Y > 0)
        {
            planeMesh.Rotation = new Vector3(
                planeMesh.Rotation.X,
                planeMesh.Rotation.Y,
                Mathf.LerpAngle(planeMesh.Rotation.Z, Mathf.DegToRad(45) * vector.Y, delta)
            );
        }
        else
        {
            planeMesh.Rotation = new Vector3(
                planeMesh.Rotation.X,
                planeMesh.Rotation.Y,
                Mathf.LerpAngle(planeMesh.Rotation.Z, 0, delta)
            );
        }
    }

    /*
    private void SpinPropellor(float delta)
    {
        float m = currentSpeed / maxSpeed;
        prop.RotateZ(150 * delta * m);
        if (prop.Rotation.Z > Mathf.Tau)
        {
            prop.Rotation = new Vector3(prop.Rotation.X, prop.Rotation.Y, 0);
        }
    }
    */
    public void OnMouseAnalogInput(Vector2 analog)
    {
        turnInput = analog;
    }
}
