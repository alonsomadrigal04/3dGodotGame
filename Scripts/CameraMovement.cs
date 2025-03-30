using Godot;
using System;

public partial class CameraMovement : Camera3D
{
	[Export] private Node3D _target;
	[Export] private float cameraDistance = 5.0f;
    [Export] private float followSpeed = 5.0f;
	[Export] private float cameraHeight = 0.5f;


	public override void _Ready()
	{
		InitiateCameraPosition();
		
		if (_target == null){
			GD.PrintErr("C3D: No Target provided");
		}
	}
    public override void _Process(double delta)
    {
		FollowPlayer(delta);
    }

    private void InitiateCameraPosition()
    {
		Vector3 targetPosition = _target.GlobalTransform.Origin - _target.GlobalTransform.Basis.Y * cameraDistance;
        
        GlobalTransform = new Transform3D(
            GlobalTransform.Basis,
            targetPosition
        );
    }




	/// <summary>
	/// This method allows de camera to follow smoothly the player always from de back
	/// </summary>
	/// <param name="delta"></param>
	private void FollowPlayer(double delta){

		Vector3 targetPosition = _target.GlobalTransform.Origin + _target.GlobalTransform.Basis.Z * cameraDistance;
        
        GlobalTransform = new Transform3D(
            GlobalTransform.Basis,
            GlobalTransform.Origin.Lerp(targetPosition, (float)delta * followSpeed)
        );

		Vector3 targetPoint = _target.GlobalTransform.Origin;
		targetPoint.Y += cameraHeight; 

        LookAt(targetPoint, Vector3.Up);
	}
}
