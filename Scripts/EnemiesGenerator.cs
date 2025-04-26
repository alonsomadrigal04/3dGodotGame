using Godot;
using System;

public partial class EnemiesGenerator : Node3D
{
    [Export] public PackedScene enemyScene;  
    [Export] public float generationRadius = 10.0f;  
    [Export] public float safeRadius = 15.0f;  
    [Export] public float enemySpawnInterval = 3.0f;  

    private Node3D player;
    private float spawnTimer = 0.0f;
    private Random random = new Random();

    public override void _Ready()
    {
        player = GetNode<RigidBody3D>("/root/AnotherScenario/player");
    }

    public override void _Process(double delta)
    {
        spawnTimer += (float)delta;

        if (spawnTimer >= enemySpawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemy();
        }

        foreach (Node enemy in GetChildren())
        {
            if (enemy is enemiesMovement) 
            {
                var distance = player.GlobalPosition.DistanceTo(((Node3D)enemy).GlobalPosition);


                if (distance > safeRadius)
                {
                    GD.Print("Un enemigo ha sido eliminado");
                    enemy.QueueFree(); 
                }
            }
        }
    }

    private void SpawnEnemy()
    {
        float angle = (float)(random.NextDouble() * Math.PI * 2);
        Vector3 spawnPosition = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * generationRadius;
        spawnPosition += GlobalPosition;  

        var enemy = (enemiesMovement)enemyScene.Instantiate();
        GetTree().CurrentScene.AddChild(enemy);
        enemy.GlobalPosition = spawnPosition;

        GD.Print("An enemy has spawned!");
    }
}
