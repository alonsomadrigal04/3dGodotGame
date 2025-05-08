using Godot;
using Player;
using System;

public partial class enemiesMovement : RigidBody3D
{
    [Export] public float moveSpeed = 3.0f;
    [Export] public float detectionRadius = 5.0f;
    [Export] public NodePath playerPath;
    [Export] private GpuParticles3D exclamation;
    [Export] private bool exclamationPlayed;
    [Export] private GpuParticles3D deathParticles;
    
    private Node3D player;
    private Vector3 currentDirection;
    private float directionTimer = 0f;
    private float directionDuration = 0f;
    
    private Vector3 targetDirection;
    private float directionChangeSpeed = 3.0f;

    private Random random = new Random();
    private bool isChasing = false;

    [Export] private Area3D bulletCollision;
    [Export] private float stopDuration = 0.5f;

    private bool canMove = true;

    public override void _Ready()
    {
        player = GetNode<RigidBody3D>("/root/AnotherScenario/player");
        CollisionLayer = 1;

        PickNewDirection();

        bulletCollision.BodyEntered += _on_body_entered;
    }

    public void PlayDead()
    {
        Tween tween = GetTree().CreateTween();
        deathParticles.Emitting = true;

        tween.SetTrans(Tween.TransitionType.Expo);
        tween.SetEase(Tween.EaseType.Out);

        tween.TweenProperty(this, "scale", Vector3.Zero, 0.5f);
        tween.Parallel().TweenProperty(this, "rotation_degrees", RotationDegrees + new Vector3(0, 360, 0), 0.5f);

        tween.TweenCallback(Callable.From(() => QueueFree()));
    }


    public override void _PhysicsProcess(double delta)
    {
        if(!canMove) return;
        float distanceToPlayer = GlobalPosition.DistanceTo(player.GlobalPosition);

        if (distanceToPlayer <= detectionRadius)
        {
            isChasing = true;
            exclamation.Emitting = true;
            if(!exclamationPlayed){
                AudioManager.Instance.PlaySound("exclamation");
                exclamationPlayed = true;
            }
        }
        else
        {
            isChasing = false;
            exclamationPlayed = false;
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

    private void _on_body_entered(Node body)
    {
        if(body is Bullet bullet){
            GD.Print("something has entered");
            PlayStop();
        }
    }

    private void PlayStop()
    {
        canMove = false;

        var tween = GetTree().CreateTween();
        Vector2 originalPosition = new Vector2(Position.X, Position.Z);

        for (int i = 0; i < 4; i++) // 4 pequeños sacudones
        {
            Vector2 offset = new Vector2(
                GD.Randf() * 10f - 5f, // X entre -5 y 5
                GD.Randf() * 10f - 5f  // Y entre -5 y 5
            );

            tween.TweenProperty(this, "position", originalPosition + offset, 0.05f);
            tween.TweenProperty(this, "position", originalPosition, 0.05f);
        }

        tween.TweenCallback(Callable.From(() =>
        {
            canMove = true;
        })).SetDelay(stopDuration);
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
