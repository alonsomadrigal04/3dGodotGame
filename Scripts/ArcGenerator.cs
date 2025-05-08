using Godot;
using System;

public partial class ArcGenerator : Area3D
{
    [Export] public PackedScene ArcScene;
    [Export] public float SpawnRadius = 5f;
    [Export] public float SpawnInterval = 2f;
    [Export] public float ArcLifetime = 5f;

    private Timer _spawnTimer;

    public override void _Ready()
    {
        _spawnTimer = new Timer();
        _spawnTimer.WaitTime = SpawnInterval;
        _spawnTimer.Autostart = true;
        _spawnTimer.Timeout += SpawnArc;
        AddChild(_spawnTimer);
    }

    private void SpawnArc()
    {
        if (ArcScene == null) return;

        Node3D arc = (Node3D)ArcScene.Instantiate();
        arc.Position = GetRandomPosition();
        arc.Rotation = GetRandomRotation();
        GetTree().CurrentScene.AddChild(arc);

        Tween tween = CreateTween();
        arc.Scale = Vector3.One * 0.05f;
        tween.TweenProperty(arc, "scale", Vector3.One * 3, 0.5f).SetTrans(Tween.TransitionType.Elastic);

        GetTree().CreateTimer(ArcLifetime).Timeout += () => RemoveArc(arc);
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(
            (float)GD.RandRange(-SpawnRadius, SpawnRadius),
            (float)GD.RandRange(-SpawnRadius, SpawnRadius),
            (float)GD.RandRange(-SpawnRadius, SpawnRadius)
        );
    }

    private Vector3 GetRandomRotation()
    {
        return new Vector3(
            (float)GD.RandRange(0, Mathf.Tau),
            (float)GD.RandRange(0, Mathf.Tau),
            (float)GD.RandRange(0, Mathf.Tau)
        );
    }

    private void RemoveArc(Node3D arc)
    {
        if(arc == null) return;
        Tween tween = CreateTween();
        tween.TweenProperty(arc, "scale", Vector3.One * 0.05f, 0.5f).SetTrans(Tween.TransitionType.Quad);
        tween.Finished += () => arc.QueueFree();
    }
}
