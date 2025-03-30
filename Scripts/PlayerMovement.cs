using Godot;
using System;

public partial class PlayerMovement : CharacterBody3D
{
    private Camera3D camera;
    private float rotationSpeed = 5f;
    [Export] private float constantVelocity = 5f;

    [Export] private float axisVelocity = 10f;
    
    public override void _Ready()
    {
        camera = GetNode<Camera3D>("Camera3D");

        GD.Print(camera);
    }
    
    public override void _Process(double delta)
    {
        
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 mousePosition = camera.ProjectRayOrigin(GetViewport().GetMousePosition());
        Vector3 mouseDirection = camera.ProjectRayNormal(GetViewport().GetMousePosition());

        Vector3 toMouse = mousePosition - GlobalTransform.Origin;
        toMouse.Y = 0;  

        float targetAngle = Mathf.Atan2(toMouse.X, toMouse.Z);

        float currentAngle = Rotation.Y;
        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, rotationSpeed * (float)delta);

        GD.Print(mouseDirection);

        Velocity = new Vector3(mouseDirection.X * axisVelocity, mouseDirection.Y * axisVelocity, -constantVelocity);

        MoveAndSlide(); 
    }

}
