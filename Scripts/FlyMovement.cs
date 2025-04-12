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
    public bool canMove = false;
    [ExportGroup("energyBar")]
    [Export] private Sprite2D energyBar;
    [Export] private Label energyLabel;
    public int energy = 100;
    private int maxEnergy = 100; // Define the maximum energy
    private float originalBarScaleX;
    private float energyTickCooldown = 0.3f; // cada cuántos segundos se aplica daño
    private float energyTickTimer = 0f;






    private Vector2 mouseDelta = Vector2.Zero;
    private float currentSpeed;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        currentSpeed = maxSpeed;
        originalBarScaleX = energyBar.Scale.X;


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

    public void EatStraw(){
        energy = Mathf.Min(maxEnergy, energy + 5);

        float energyPercent = (float)energy / maxEnergy;
        float targetScaleX = originalBarScaleX * energyPercent;
        float currentScaleY = energyBar.Scale.Y;

        var tween = GetTree().CreateTween();

        tween.Parallel().TweenProperty(energyBar, "scale", new Vector2(targetScaleX, currentScaleY), 0.3f)
            .SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);
        
        tween.Parallel().TweenProperty(energyBar, "rotation", Mathf.DegToRad(5.0f), 0.3f)
            .SetTrans(Tween.TransitionType.Elastic)
            .SetEase(Tween.EaseType.Out);

        tween.Parallel().TweenProperty(energyBar, "modulate", new Color(0f, 1f, 0f), 0.3f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);

        tween.TweenProperty(energyBar, "modulate", Colors.White, 0.3f)
            .SetDelay(0.3f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.In);

        tween.Parallel().TweenProperty(energyBar, "rotation", 0, 0.3f)
            .SetTrans(Tween.TransitionType.Elastic)
            .SetEase(Tween.EaseType.Out);

        energyLabel.Text = "< " + energy.ToString() + "% >";
        AudioManager.Instance.PlaySound("pick");
        //CameraSchake(2);
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            mouseDelta = mouseMotion.Relative;
        }
    }

    public void CameraSchake(int intensity){
        // Intensidad máxima del sacudido
        float shakeMagnitude = intensity * 0.1f;
        float duration = 0.4f;

        var tween = GetTree().CreateTween();
        var camera = GetViewport().GetCamera3D(); // O GetCamera2D() si es 2D

        if (camera == null) return;

        Vector3 originalPosition = camera.GlobalTransform.Origin;

        // Sacudir hacia adelante y atrás varias veces
        for (int i = 0; i < 5; i++)
        {
            Vector3 randomOffset = new Vector3(
                (float)GD.RandRange(-shakeMagnitude, shakeMagnitude),
                (float)GD.RandRange(-shakeMagnitude, shakeMagnitude),
                (float)GD.RandRange(-shakeMagnitude, shakeMagnitude / 2)
            );

            tween.TweenProperty(camera, "global_transform:origin", originalPosition + randomOffset, duration / 10f)
                .SetEase(Tween.EaseType.InOut)
                .SetTrans(Tween.TransitionType.Sine);
        }

        // Volver a la posición original
        tween.TweenProperty(camera, "global_transform:origin", originalPosition, duration / 2f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Sine);
    }


    public override void _PhysicsProcess(double delta)
    {
        if(canMove){
            RotatePlayer(delta);
            AdjustSpeed(delta);
            MovePlayer(delta);
            LinesAnimation(delta);
            ChangeEnergy(delta);
        }
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

        tween.SetTrans(Tween.TransitionType.Elastic)
            .SetEase(Tween.EaseType.Out)
            .SetProcessMode(Tween.TweenProcessMode.Physics);

        tween.TweenProperty(this, "scale", this.Scale * 0.5f, 0.3f);
        tween.Chain();
        tween.TweenProperty(this, "scale", this.Scale, 0.6f);

        
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

    private void ChangeEnergy(double delta){

        energyTickTimer += (float)delta;

        if (energyTickTimer >= energyTickCooldown)
        {
            float speed = LinearVelocity.Length();
            float energyLossRate = 0.2f; 

            int energyLoss = Mathf.Clamp((int)(speed * energyLossRate), 0, 5); 

            energy = Mathf.Max(0, energy - energyLoss);

            energyTickTimer = 0f;

            float energyPercent = (float)energy / maxEnergy;

            var tween = GetTree().CreateTween();
            float targetScaleX = originalBarScaleX * energyPercent;
            float currentScaleY = energyBar.Scale.Y;

            tween.TweenProperty(energyBar, "scale", new Vector2(targetScaleX, currentScaleY), 0.3f)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.Out);

            energyLabel.Text = "< " + energy.ToString() + "% >";
        }
    }


}
