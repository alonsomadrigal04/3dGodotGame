using Godot;
using System;

public partial class EventsControler : Node
{
    [ExportGroup("transitions")]
    [Export] private Control transitionSprite;
    [Export] private PlayerMovement playerMovementSc;


    public override void _Ready()
    {
        AnimateMoveToPosition(transitionSprite, new Vector2(1769.0f, 1003.0f));
        BeginCountDown();
    }

    private async void BeginCountDown()
    {
        // Contenedor del countdown (puede ser el propio nodo, o donde quieras poner los labels)
        Node parent = this;

        for (int i = 3; i > 0; i--)
        {
            Label countLabel = new Label();
            countLabel.Text = i.ToString();
            countLabel.AddThemeFontSizeOverride("font_size", 80);

            countLabel.HorizontalAlignment = HorizontalAlignment.Center;
            countLabel.VerticalAlignment = VerticalAlignment.Center;
            countLabel.AnchorLeft = 0.5f;
            countLabel.AnchorTop = 0.5f;
            countLabel.AnchorRight = 0.5f;
            countLabel.AnchorBottom = 0.5f;
            countLabel.Position = Vector2.Zero;

            parent.AddChild(countLabel);

            var tween = GetTree().CreateTween();
            countLabel.Scale = new Vector2(0.5f, 0.5f);

            tween.TweenProperty(countLabel, "scale", new Vector2(1.2f, 1.2f), 0.3f)
                .SetTrans(Tween.TransitionType.Elastic)
                .SetEase(Tween.EaseType.Out);

            tween.TweenProperty(countLabel, "modulate:a", 0f, 0.4f)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.In)
                .SetDelay(0.5f);

            await ToSignal(tween, "finished");
            countLabel.QueueFree();

            await ToSignal(GetTree().CreateTimer(0.1), "timeout");
        }

        GD.Print("Cuenta regresiva terminada!");
    }



    private void AnimateMoveToPosition(Node button, Vector2 targetPosition)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(button, "position", targetPosition, 0.4f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
    }


}
