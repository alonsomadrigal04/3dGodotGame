using Godot;
using System;

public partial class StrawBerry : Node3D
{
	[Export] public float RotationSpeed = 20f; 
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Rotation += new Vector3(0, RotationSpeed * (float)delta, 0); 

	}
}
