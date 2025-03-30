using Godot;
using System;

namespace Player;
public partial class FlyMovement : RigidBody3D
{
	[ExportGroup("Movimiento")]
    [Export] private float minSpeed = 3f;
    [Export] private float maxSpeed = 8f;
    [Export] private float rotationSpeed = 2f;

    [ExportGroup("Control")]
    [Export] private float speedLerpFactor = 5f;
    [Export] private float aceleration = 5f;
    [Export] public AnimationPlayer animationPlayer;

    [ExportGroup("Shader")]
    [Export] private ColorRect ShaderLines;
    [Export] private float MinIntensityOfLines = 0.2f;
    [Export] private float MaxIntensityOfLines = 1.5f;
    [ExportGroup("Arc")]
    [Export] public bool InArc = false;
    [Export] private float maxSpeedArc = 11f;
    [Export] private float timeSpeedArc = 11f;
    private Timer arcTimer;





    private Vector2 mouseDelta = Vector2.Zero;
    private float currentSpeed;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        currentSpeed = maxSpeed;

        ArcTimerSetUp();
        
    }

    private void ArcTimerSetUp()
    {
        arcTimer = new Timer();
        arcTimer.WaitTime = timeSpeedArc;
        arcTimer.OneShot = true;
        arcTimer.Timeout += () => {
            InArc = false; 
            maxSpeed = 15f;
            MaxIntensityOfLines = 1.5f;
            
            };
        AddChild(arcTimer);
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
        LinesAnimation(delta);
    }

    private void LinesAnimation(double delta){
        if (ShaderLines.Material is ShaderMaterial shaderMat)
    {
        float speed = LinearVelocity.Length();

        float normalizedSpeed = Mathf.InverseLerp(minSpeed, maxSpeed * aceleration, speed);
        float lineDensity = Mathf.Lerp(MinIntensityOfLines, MaxIntensityOfLines, normalizedSpeed);

        shaderMat.SetShaderParameter("line_density", lineDensity);
    }
    }

    public void ArcAnimation()
{
    if (InArc) return;

    InArc = true;
    arcTimer.Start();

    maxSpeed = maxSpeedArc; 
    MaxIntensityOfLines = 2.0f; 

    Tween tween = GetTree().CreateTween();

    tween.SetTrans(Tween.TransitionType.Back)
         .SetEase(Tween.EaseType.Out)
         .SetProcessMode(Tween.TweenProcessMode.Physics);

    Vector3 rotationAxis = GlobalTransform.Basis.Z.Normalized();
    float rotationAmount = Mathf.DegToRad(360); 

    GD.Print(rotationAmount);

    tween.TweenMethod(Callable.From<float>((t) =>
    {
        Rotate(rotationAxis, 0.202f);
    }), 0, 1, 0.5f);
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
