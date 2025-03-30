using Godot;
using System;
using Player; // Replace 'YourNamespace' with the namespace where the Player class is defined

public partial class ArcBehaviour : MeshInstance3D
{
	[Export] private Area3D detectionZone; // Referencia al área de detección

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
}
