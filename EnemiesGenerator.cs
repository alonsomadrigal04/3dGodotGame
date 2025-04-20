using Godot;
using System;

public partial class EnemiesGenerator : Node3D
{
    [Export] public PackedScene enemyScene;  // La escena del enemigo
    [Export] public float generationRadius = 10.0f;  // Radio de generación de enemigos
    [Export] public float safeRadius = 15.0f;  // Radio de prudencia (cuando se aleja más de este valor, el enemigo desaparece)
    [Export] public float enemySpawnInterval = 3.0f;  // Intervalo de tiempo entre generaciones

    private Node3D player;
    private float spawnTimer = 0.0f;
    private Random random = new Random();

    public override void _Ready()
    {
        player = GetNode<Node3D>("Player"); // Asumiendo que el nodo del jugador está en el árbol de nodos
    }

    public override void _Process(double delta)
    {
        // Temporizador para generar enemigos
        spawnTimer += (float)delta;

        if (spawnTimer >= enemySpawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemy();
        }

        // Comprobar y eliminar enemigos fuera de la zona de prudencia
        // foreach (Node enemy in GetChildren())
        // {
        //     if (enemy is enemiesMovement) // Asegúrate de que es un enemigo
        //     {
        //         var distance = player.GlobalPosition.DistanceTo(((Node3D)enemy).GlobalPosition);

        //         if (distance > safeRadius)
        //         {
        //             enemy.QueueFree();  // Elimina el enemigo si está fuera de la zona de prudencia
        //         }
        //     }
        // }
    }

    private void SpawnEnemy()
    {
        // Elegir una posición aleatoria dentro del radio de generación
        float angle = (float)(random.NextDouble() * Math.PI * 2);
        Vector3 spawnPosition = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * generationRadius;
        spawnPosition += GlobalPosition;  // Asegura que se genere en el espacio global correcto

        // Instanciar el enemigo
        var enemy = (enemiesMovement)enemyScene.Instantiate();
        GD.Print(enemy);
        enemy.GlobalPosition = spawnPosition;
        GetTree().CurrentScene.AddChild(enemy);

        GD.Print("An enemy has spawned!");
    }
}
