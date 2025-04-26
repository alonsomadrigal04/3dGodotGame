using Godot;

[Tool] 
public partial class DetectionGizmo : Node3D
{
    [Export] public float Radius = 5.0f;
    [Export] public Color GizmoColor = new Color(1, 0, 0, 0.2f); // rojo semi-transparente

    public override void _Ready()
    {
        UpdateGizmo();
    }

    public override void _Process(double delta)
    {
        if (Engine.IsEditorHint())
            UpdateGizmo();
    }

    private void UpdateGizmo()
    {
        var meshInstance = GetNodeOrNull<MeshInstance3D>("_gizmo_mesh");
        if (meshInstance == null)
        {
            meshInstance = new MeshInstance3D();
            meshInstance.Name = "_gizmo_mesh";
            AddChild(meshInstance);
        }

        var sphere = new SphereMesh
        {
            Radius = Radius,
            Height = Radius * 2,
            RadialSegments = 32,
            Rings = 16
        };

        var material = new StandardMaterial3D
        {
            AlbedoColor = GizmoColor,
            Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
            ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded
        };

        material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
        // Ensure the material is transparent
        material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;

        meshInstance.Mesh = sphere;
        meshInstance.MaterialOverride = material;
    }
}
