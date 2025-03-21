using Godot;
using System;
using Godot.Collections;

public partial class StrawBerry : Area3D
{
	[Export] public float RotationSpeed = 20f; // Velocidad de rotación en grados por segundo
    [Export] public float FloatSpeed = 1f; // Velocidad del movimiento de arriba a abajo
    [Export] public float FloatAmplitude = 0.5f; // Amplitud del movimiento vertical
    [Export] public PackedScene ParticleEffect; // Efecto de partículas al desaparecer
    [Export] public CollisionShape3D CollisionShape; // Referencia al CollisionShape para desactivarlo
    
    private Vector3 initialPosition;
    private float timeElapsed = 0f;
    private Vector3 rotationDirection;
    private bool isDisappearing = false;

    public override void _Ready()
    {
        initialPosition = Position;
        RandomizeRotationDirection();

        BodyEntered += _on_body_entered;
    }

    public override void _Process(double delta)
    {
        if (!isDisappearing)
        {
            RotateStrawberry((float)delta);
            FloatStrawberry((float)delta);
        }
    }

    private void RotateStrawberry(float delta)
    {
        Rotation += rotationDirection * RotationSpeed * delta;
    }

    private void FloatStrawberry(float delta)
    {
        timeElapsed += delta;
        Position = initialPosition + new Vector3(0, Mathf.Sin(timeElapsed * FloatSpeed) * FloatAmplitude, 0);
    }

    private void RandomizeRotationDirection()
    {
        RandomNumberGenerator rng = new RandomNumberGenerator();
        rotationDirection = new Vector3(
            rng.RandfRange(-1f, 1f), 
            rng.RandfRange(-1f, 1f), 
            rng.RandfRange(-1f, 1f)
        ).Normalized();
    }

    private async void Disappear()
    {
        isDisappearing = true;
        if (CollisionShape != null)
        {
            CollisionShape.Disabled = true;
        }

        Tween tween = GetTree().CreateTween();
        //tween.TweenProperty(this, "scale", Scale * 0.05f, 0.3f).SetTrans(Tween.TransitionType.Expo);
        
        if (ParticleEffect != null)
        {
            Node3D particles = (Node3D)ParticleEffect.Instantiate();
            GetParent().AddChild(particles);
            particles.GlobalPosition = GlobalPosition;
            
        }
        
        await ToSignal(tween, "finished");
        QueueFree();    
    }

    private void _on_body_entered(Node body)
    {
        Disappear();
    }
}
