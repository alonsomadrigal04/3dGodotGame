using Godot;
using Player;
using System;

public partial class enemiesMovement : RigidBody3D
{
    [Export] public float moveSpeed = 3.0f;
    [Export] public float detectionRadius = 5.0f;
    [Export] public NodePath playerPath;
    [Export] private GpuParticles3D exclamation;
    
    private Node3D player;
    private Vector3 currentDirection;
    private float directionTimer = 0f;
    private float directionDuration = 0f;
    
    private Vector3 targetDirection;
    private float directionChangeSpeed = 3.0f;

    private Random random = new Random();
    private bool isChasing = false;

    public override void _Ready()
    {
        player = GetNode<RigidBody3D>("/root/AnotherScenario/player");

        PickNewDirection();
    }

    public override void _PhysicsProcess(double delta)
    {
        float distanceToPlayer = GlobalPosition.DistanceTo(player.GlobalPosition);

        if (distanceToPlayer <= detectionRadius)
        {
            isChasing = true;
            exclamation.Emitting = true;
            AudioManager.Instance.PlaySound("exclamation");
        }
        else
        {
            isChasing = false;
        }

        if (isChasing)
        {
            FollowPlayer((float)delta);
        }
        else
        {
            MoveErratically((float)delta);
        }
    }

    private void MoveErratically(float delta)
    {
        directionTimer += delta;

        if (directionTimer >= directionDuration)
        {
            PickNewDirection();
        }

        currentDirection = currentDirection.Lerp(targetDirection, directionChangeSpeed * delta);

        Vector3 velocity = currentDirection * moveSpeed;
        LinearVelocity = velocity;

        LookAt(GlobalPosition + currentDirection, Vector3.Up);
    }


    private void PickNewDirection()
    {
        float angle = (float)(random.NextDouble() * Math.PI * 2);
        targetDirection = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).Normalized();

        directionDuration = (float)(random.NextDouble() * 3 + 2);
        directionTimer = 0f;
    }


    private void FollowPlayer(float delta)
    {
        Vector3 toPlayer = (player.GlobalPosition - GlobalPosition).Normalized();
        LinearVelocity = toPlayer * moveSpeed * 1.2f; // Hardcode momento yeah

        LookAt(player.GlobalPosition, Vector3.Up);
    }
}
