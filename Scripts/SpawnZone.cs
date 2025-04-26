using Godot;
using System;
using System.Collections.Generic;

public partial class SpawnZone : Area3D
{
    [Export] public float SpawnRadius = 5f; 
    [Export] public float NegativeZone = 2f; 
    [Export] public PackedScene FruitScene;
    [Export] public float SpawnInterval = 2f; 
    [Export] public float FruitLifetime = 5f; 

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

        Vector3 randomPosition = GetValidSpawnPosition();

        Node3D fruit = (Node3D)FruitScene.Instantiate();
        fruit.Position = GlobalPosition + randomPosition;


        GetTree().CurrentScene.AddChild(fruit);

        Tween tween = CreateTween();
        fruit.Scale = Vector3.One * 0.05f;
        tween.TweenProperty(fruit, "scale", Vector3.One, 1.5f).SetTrans(Tween.TransitionType.Elastic);
        tween.Play();

        _spawnedFruits.Add(fruit);

        GetTree().CreateTimer(FruitLifetime).Timeout += () => RemoveFruit(fruit);
    }

    private Vector3 GetValidSpawnPosition()
    {
        Vector3 position;
        float distance;

        do
        {
            float theta = (float)GD.RandRange(0, Math.PI * 2);
            float phi = (float)GD.RandRange(0, Math.PI);
            float x = Mathf.Sin(phi) * Mathf.Cos(theta);
            float y = Mathf.Sin(phi) * Mathf.Sin(theta);
            float z = Mathf.Cos(phi);

            distance = (float)GD.RandRange(NegativeZone, SpawnRadius);
            position = new Vector3(x, y, z) * distance;

        } while (position.Length() < NegativeZone);

        return position;
    }

    private void RemoveFruit(Node3D fruit)
    {
        if (fruit == null || !IsInstanceValid(fruit))
        {
            GD.PrintErr("SZ: FRUIT ALREADY REMOVED");
            return;
        }

        Tween tween = CreateTween();
        tween.TweenProperty(fruit, "scale", Vector3.One * 0.01f, 0.5f).SetTrans(Tween.TransitionType.Quad);
        tween.Finished += () => 
        {
            if (IsInstanceValid(fruit))
                fruit.QueueFree();
        };
        tween.Play();
    }


}
