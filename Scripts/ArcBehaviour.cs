using Godot;
using System;
using Player; // Replace 'YourNamespace' with the namespace where the Player class is defined

public partial class ArcBehaviour : MeshInstance3D
{
	[Export] private Area3D detectionZone; // Referencia al área de detección
    [Export] private float disappearTime = 3f; // Tiempo antes de reaparecer

    private Tween tween;

    public override void _Ready()
    {
        if (detectionZone == null)
        {
            GD.PrintErr("ArcBehaviour: No detectionZone assigned.");
            return;
        }

        // Conectar las señales de detección
        detectionZone.BodyEntered += OnBodyEntered;
        detectionZone.BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(Node body)
    {
        if (body is FlyMovement player)
        {
            player.ArcAnimation();
            AnimateDisappear(); // 🔥 Se activa la animación al atravesar el arco
            GD.Print("Player entered the Arc!");
        }
    }

    private void OnBodyExited(Node body)
    {
        if (body is FlyMovement player)
        {
            player.InArc = false;
            GD.Print("Player left the Arc.");
        }
    }

    private void AnimateDisappear()
    {
        if (tween != null && tween.IsRunning()) tween.Kill();

        tween = GetTree().CreateTween();
        tween.SetTrans(Tween.TransitionType.Expo)
             .SetEase(Tween.EaseType.Out)
             .SetProcessMode(Tween.TweenProcessMode.Physics);

		Vector3 rotationAxis = GlobalTransform.Basis.Z.Normalized();

        // Reducir la escala y el color alfa para desaparecer
        tween.TweenProperty(this, "scale", Vector3.One * 0.005f, 0.7f);

		tween.TweenMethod(Callable.From<float>((t) =>
        {
            Rotate(rotationAxis, 0.482f);
        }), 0, 1, 0.2f);

        // Esperar y reaparecer
        tween.TweenCallback(Callable.From(() => QueueFree()));
    }

    private void AnimateAppear()
    {
        if (tween != null && tween.IsRunning()) tween.Kill();

        tween = GetTree().CreateTween();
        tween.SetTrans(Tween.TransitionType.Sine)
             .SetEase(Tween.EaseType.InOut)
             .SetProcessMode(Tween.TweenProcessMode.Physics);

        // Restaurar escala y color alfa para aparecer
        tween.TweenProperty(this, "scale", Vector3.One, 0.5f);
        tween.TweenProperty(this, "modulate:a", 1, 0.5f); 
    }
}
