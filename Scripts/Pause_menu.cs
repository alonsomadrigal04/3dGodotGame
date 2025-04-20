using Godot;
using Player;
using System;

public partial class Pause_menu : Control
{
    [ExportGroup("Menu")]
    [Export] private Control menuPanel;
    [Export] private FlyMovement playerMovementSc;
    [Export] public bool pauseDisplayed = false;

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.Escape && playerMovementSc.canMove && !pauseDisplayed)
        {
            PauseDisplay();
        }
    }

    private void HidePause(){
        pauseDisplayed = false;

        Vector2 originalPosition = menuPanel.Position;

        var tween = GetTree().CreateTween();

        // Fade-in y scale
        tween.Parallel().TweenProperty(menuPanel, "modulate:a", 0.0f, 0.4f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Sine);

        tween.Parallel().TweenProperty(menuPanel, "scale", new Vector2(1.8f, 1.8f), 0.4f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);
    }

    private void PauseDisplay()
    {
        pauseDisplayed = true;
        menuPanel.Visible = true;
        menuPanel.Modulate = new Color(1, 1, 1, 0);
        menuPanel.Scale = new Vector2(1.2f, 1.2f);

        Vector2 originalPosition = menuPanel.Position;

        var tween = GetTree().CreateTween();

        tween.Parallel().TweenProperty(menuPanel, "modulate:a", 1.0f, 0.4f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Sine);

        tween.Parallel().TweenProperty(menuPanel, "scale", new Vector2(1.0f, 1.0f), 0.4f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);

        tween.TweenCallback(Callable.From(() =>
        {
            Engine.TimeScale = 0.5f;  // La mitad de velocidad
        }));

        AudioManager.Instance.PlaySound("pausa");
    }

}
