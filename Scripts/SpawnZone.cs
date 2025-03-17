using Godot;
using System;
using System.Collections.Generic;

public partial class SpawnZone : Area3D
{
    [Export] public float SpawnRadius = 5f;
    [Export] public PackedScene FruitScene; // Escena de la fresa
    [Export] public float SpawnInterval = 2f; // Tiempo entre spawns
    [Export] public float FruitLifetime = 5f; // Tiempo de vida de la fresa

    private Timer _spawnTimer;
    private List<Node3D> _spawnedFruits = new List<Node3D>();

    public override void _Ready()
    {
        _spawnTimer = new Timer();
        _spawnTimer.WaitTime = SpawnInterval;
        _spawnTimer.Autostart = true;
        _spawnTimer.Timeout += SpawnFruit;
        AddChild(_spawnTimer);
    }

    private void SpawnFruit()
    {
        if (FruitScene == null) return;

        Vector3 ZoneSize = Vector3.One * SpawnRadius;
        
        Vector3 randomPosition = new Vector3(
            (float)GD.RandRange(-ZoneSize.X / 2, ZoneSize.X / 2),
            (float)GD.RandRange(-ZoneSize.Y / 2, ZoneSize.Y / 2),
            (float)GD.RandRange(-ZoneSize.Z / 2, ZoneSize.Z / 2)
        );

        Node3D fruit = (Node3D)FruitScene.Instantiate();
        fruit.Position = randomPosition;

        fruit.Position = GlobalPosition + randomPosition;

        GD.Print(randomPosition);

        GetTree().CurrentScene.AddChild(fruit);

        
        Tween tween = CreateTween();
        fruit.Scale = Vector3.Zero;

        tween.TweenProperty(fruit, "scale", Vector3.One, 0.5f).SetTrans(Tween.TransitionType.Elastic);
        tween.Play();
        
        _spawnedFruits.Add(fruit);
        GetTree().CreateTimer(FruitLifetime).Timeout += () => RemoveFruit(fruit);
    }

    

    private void RemoveFruit(Node3D fruit)
    {
        if (fruit == null){
            GD.PrintErr("SZ: NO FRUIT ADDED");
            return;
        }
        
        Tween tween = CreateTween();
        tween.TweenProperty(fruit, "scale", Vector3.Zero, 0.5f).SetTrans(Tween.TransitionType.Quad);
        tween.Play();
        tween.Finished += () => fruit.QueueFree();
    }

    

}
