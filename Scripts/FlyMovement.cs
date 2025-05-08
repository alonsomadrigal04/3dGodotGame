using Godot;
using System;

namespace Player;
public partial class FlyMovement : RigidBody3D
{
	[ExportGroup("Movimiento")]
    [Export] private float minSpeed = 3f;
    [Export] private float maxSpeed = 8f;
    [Export] private float rotationSpeed = 2f;
    [Export] private Label speedText;
    private bool feedbackTriggered = false; // Asegura que solo se active una vez


    [ExportGroup("Control")]
    [Export] private float speedLerpFactor = 5f;
    [Export] private float aceleration = 5f;
    [Export] public AnimationPlayer animationPlayer;

    [ExportGroup("Shader")]
    [Export] private ColorRect ShaderLines;
    [Export] private float MinIntensityOfLines = 0.2f;
    [Export] private float MaxIntensityOfLines = 1.5f;
    [Export] private ColorRect saturation;
    [ExportGroup("Arc")]
    [Export] public bool InArc = false;
    [Export] public Label arcTimeLabel;
    [Export] private float maxSpeedArc = 11f;
    [Export] private float timeSpeedArc = 11f;
    private Timer arcTimer;
    public bool canMove = false;
    [ExportGroup("energyBar")]
    [Export] private Sprite2D energyBar;
    [Export] private Label energyLabel;
    [Export] private Sprite2D energyBarMarc;

    [Export] private Area3D detectionArea;
    private float barShakeTime = 0f;
    public int energy = 100;
    private int maxEnergy = 100; // Define the maximum energy
    private float originalBarScaleX;
    private Vector2 originalLabelScale;
    private float energyTickCooldown = 0.3f; // cada cuántos segundos se aplica daño
    private float energyTickTimer = 0f;

    [ExportGroup("Shooting")]
    [Export] private PackedScene bulletScene;
    [Export] private Node3D bulletSpawnPoint; // opcional si quieres un punto específico

    private Vector2 mouseDelta = Vector2.Zero;
    private float currentSpeed;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        currentSpeed = maxSpeed;
        originalBarScaleX = energyBar.Scale.X;
        originalLabelScale = energyLabel.Scale;

    if (saturation.Material is ShaderMaterial satMat)
        {
            satMat.SetShaderParameter("saturation", 1);
        }

        detectionArea.BodyEntered += _on_enemy_entered;

        ArcTimerSetUp();
    }

    private void _on_enemy_entered(Node body)
    {
        if (body is enemiesMovement enemy)
        {
            if(currentSpeed < 19f){
                float percentageToRemove = 0.05f; 
                int damage = Mathf.RoundToInt(energy * percentageToRemove);
                energy = Mathf.Max(0, energy - damage);

                AudioManager.Instance?.PlaySound("hit");
            }
            else{
                EatStraw();

                AudioManager.Instance?.PlaySound("pick");

            }

            enemy.PlayDead(); 
        }
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
        
        tween.Parallel().TweenProperty(energyBar, "modulate", new Color(0f, 1f, 0f), 0.3f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);

        tween.TweenProperty(energyBar, "modulate", Colors.White, 0.3f)
            .SetDelay(0.3f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.In);


        ////////////////////////////////////
        
        AnimateLabel(energyLabel);

        energyLabel.Text = "< " + energy.ToString() + "% >";
        AudioManager.Instance.PlaySound("pick");
        //CameraSchake(2);
    }

    private void AnimateLabel(Label label){

        var tween = GetTree().CreateTween();


        float targetScaleX = energyLabel.Scale.X * 2f;
        float currentScaleY = energyLabel.Scale.Y * 2f;

        tween.Parallel().TweenProperty(label, "scale", new Vector2(targetScaleX, currentScaleY), 0.3f)
            .SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);
        
        tween.Parallel().TweenProperty(label, "rotation", Mathf.DegToRad(5.0f), 0.3f)
            .SetTrans(Tween.TransitionType.Elastic)
            .SetEase(Tween.EaseType.Out);

        tween.Parallel().TweenProperty(label, "modulate", new Color(0f, 1f, 0f), 0.3f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.Out);

        tween.TweenProperty(label, "modulate", Colors.White, 0.3f)
            .SetDelay(0.3f)
            .SetTrans(Tween.TransitionType.Sine)
            .SetEase(Tween.EaseType.In);

        tween.Parallel().TweenProperty(label, "rotation", 0, 0.3f)
            .SetTrans(Tween.TransitionType.Elastic)
            .SetEase(Tween.EaseType.Out);

            tween.Parallel().TweenProperty(label, "scale", new Vector2(targetScaleX * 0.5f, currentScaleY * 0.5f), 0.3f)
            .SetTrans(Tween.TransitionType.Back)
            .SetEase(Tween.EaseType.Out);
    }

    

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            mouseDelta = mouseMotion.Relative;
        }

        if (@event is InputEventMouseButton mouseButton && mouseButton.ButtonIndex == MouseButton.Right && mouseButton.Pressed)
        {
            Shoot();
        }
        
    }

    public void CameraSchake(int intensity){
        float shakeMagnitude = intensity * 0.1f;
        float duration = 0.4f;

        var tween = GetTree().CreateTween();
        var camera = GetViewport().GetCamera3D(); // O GetCamera2D() si es 2D

        if (camera == null) return;

        Vector3 originalPosition = camera.GlobalTransform.Origin;

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
            animateBar(delta);
            animateTimeArc(delta);
        }
    }

    private void animateTimeArc(double delta)
{
    if (InArc)
    {
        if (arcTimer.TimeLeft > 0)
        {
            float timeLeft = (float)arcTimer.TimeLeft;
            
            arcTimeLabel.Text = timeLeft.ToString("INFINITE ENERGY\n");
            arcTimeLabel.Text += timeLeft.ToString("0.00");

            float t = Mathf.InverseLerp(0f, (float)arcTimer.WaitTime, timeLeft);
            Color startColor = new Color(1f, 1f, 1f); // blanco
            Color endColor = new Color(1f, 0f, 0f);   // rojo
            arcTimeLabel.Modulate = endColor.Lerp(startColor, t);

            float baseScale = 1.0f;
            float maxScale = 1.5f;
            float scaleFactor = Mathf.Lerp(maxScale, baseScale, t);
            arcTimeLabel.Scale = new Vector2(scaleFactor, scaleFactor);
        }
        else
        {
            arcTimeLabel.Text = "";
        }
    }
    else
    {
        arcTimeLabel.Text = "";
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
        AudioManager.Instance.PlaySound("powerUp");
        arcTimer.Start();

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
        float targetSpeed;

        if (Input.IsMouseButtonPressed(MouseButton.Left))
        {
            targetSpeed = maxSpeed * aceleration;
        }
        else
        {
            targetSpeed = minSpeed;
        }

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, (float)delta * 2f);

        Vector3 targetVelocity = -GlobalTransform.Basis.Z * currentSpeed;
        LinearVelocity = targetVelocity;

        speedText.Text = Mathf.RoundToInt(currentSpeed) + " m/s";

        // Feedback cuando supera 13 m/s
        if (currentSpeed > 19f && !feedbackTriggered)
        {
            feedbackTriggered = true;

            var tween = GetTree().CreateTween();
            tween.SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);

            // Aumentar tamaño y rotar
            tween.Parallel().TweenProperty(speedText, "scale", new Vector2(1.5f, 1.5f), 0.3f);
            tween.Parallel().TweenProperty(speedText, "rotation", Mathf.DegToRad(10f), 0.3f);
            tween.Parallel().TweenProperty(speedText, "modulate", new Color(1, 0, 0), 0.3f);

            // Revertir después
            tween.TweenInterval(0.5f);
            tween.Parallel().TweenProperty(speedText, "scale", Vector2.One, 0.3f);
            tween.Parallel().TweenProperty(speedText, "rotation", 0f, 0.3f);
            tween.Parallel().TweenProperty(speedText, "modulate", Colors.White, 0.3f);

            AudioManager.Instance?.PlaySound("mosca_ready"); // Asegúrate de tener este sonido

        }
        else if (currentSpeed <= 13f && feedbackTriggered)
        {
            feedbackTriggered = false; // Permite que se vuelva a activar si baja y sube otra vez
        }

    }



    private void animateBar(double delta)
    {
        float energyPercent = (float)energy / maxEnergy;
        float del0al1 = energyPercent; 
        if (InArc)
        {
            float t = (float)Time.GetTicksMsec() / 500f;
            float r = 0.5f + 0.5f * Mathf.Sin(t);
            float g = 0.5f + 0.5f * Mathf.Sin(t + 2f);
            float b = 0.5f + 0.5f * Mathf.Sin(t + 4f);
            energyBar.Modulate = new Color(r, g, b);
            energyBarMarc.Modulate = new Color(r, g, b);
        }
        else{
            if (energy <= 0) return;

            barShakeTime += (float)delta;


            float shakeIntensity = Mathf.Lerp(5f, 10f, del0al1); // más intensidad con poca energía
            float frequency = Mathf.Lerp(12f, 4f, del0al1);
            float angle = Mathf.DegToRad(Mathf.Sin(barShakeTime * frequency) * shakeIntensity);
            energyBarMarc.Rotation = angle;

            Color fullEnergyColor = new Color(1f, 1f, 1f);  // blanco
            Color lowEnergyColor = new Color(1f, 0f, 0f);   // rojo
            energyBar.Modulate = lowEnergyColor.Lerp(fullEnergyColor, del0al1);
            energyBarMarc.Modulate = lowEnergyColor.Lerp(fullEnergyColor, del0al1);
        }

        if (saturation.Material is ShaderMaterial satMat)
        {
            satMat.SetShaderParameter("saturation", del0al1);
        }
    }

    private void ChangeEnergy(double delta){
        if (InArc) return; // No gastar energía durante el arco


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

    private void Shoot()
    {
        if (bulletScene == null){
            GD.PrintErr("FM: No bullet Scene added");
        }

        var bulletInstance = (RigidBody3D)bulletScene.Instantiate();

        if (bulletSpawnPoint != null)
        {
            bulletInstance.GlobalTransform = bulletSpawnPoint.GlobalTransform;
        }
        else
        {
            bulletInstance.GlobalTransform = GlobalTransform;
        }

        GetTree().Root.AddChild(bulletInstance);
        AudioManager.Instance?.PlaySound("shoot"); // si tienes un sonido
    }
}