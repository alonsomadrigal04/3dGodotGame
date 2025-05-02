using Godot;
using Player;
using System;

public partial class Pause_menu : Control
{
    [ExportGroup("Menu")]
    [Export] private Control menuPanel;
    [Export] private FlyMovement playerMovementSc;
    [Export] public bool pauseDisplayed = false;
    [ExportGroup("buttons")]
    [Export] private Button optionsButton;
    [Export] private Button startButton;
    [Export] private Button exitButton;

    [ExportGroup("options")]
    private bool optionsDisplay = false;
    Vector2 optionsControlPos;
    [Export] private Control optionsMenu;



    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.Escape && playerMovementSc.canMove && !pauseDisplayed)
        {
            PauseDisplay();
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
        else if(@event is InputEventKey keyEvent2 && keyEvent2.Pressed && keyEvent2.Keycode == Key.Escape && pauseDisplayed){
            HidePause();
        }
    }

    public override void _Ready()
    {
        SetupButtonAnimations(startButton);
        SetupButtonAnimations(optionsButton);
        SetupButtonAnimations(exitButton);
    }


    private void HidePause(){
        pauseDisplayed = false;
        GetTree().Paused = false;
        Input.MouseMode = Input.MouseModeEnum.Captured;

        Vector2 originalPosition = menuPanel.Position;

        var tween = GetTree().CreateTween();

        // Fade-in y scale
        tween.Parallel().TweenProperty(menuPanel, "modulate:a", 0.0f, 0.4f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Sine);

        tween.Parallel().TweenProperty(menuPanel, "scale", new Vector2(1.8f, 1.8f), 0.4f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);

        menuPanel.Visible = false;
        AudioManager.Instance.audioStreamMusic.StreamPaused = false;
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
            //Engine.TimeScale = 0.5f;
            GetTree().Paused = true;
        }));

        AudioManager.Instance.PlaySound("pausa");
        AudioManager.Instance.audioStreamMusic.StreamPaused = true;
    }

    private void HideOptions()
    {
        optionsDisplay = false;

        Vector2 startPos = startButton.Position + new Vector2(500, 0);
        Vector2 exitPos = exitButton.Position + new Vector2(500, 0);
        Vector2 newoptionsControlPos = new Vector2(800, 0);

        AnimateMoveToPosition(startButton, startPos);
        AnimateMoveToPosition(exitButton, exitPos);

        AnimateMoveToPosition(optionsMenu, newoptionsControlPos);

    }

    private void AnimateMoveToPosition(Node button, Vector2 targetPosition)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(button, "position", targetPosition, 0.4f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
    }

    private void SetupButtonAnimations(Button button)
    {
        button.MouseEntered += () => {
            AnimateHover(button, true);
            AudioManager.Instance.PlaySound("hover");
        };
        button.MouseExited += () => AnimateHover(button, false);
        button.Pressed += () => {
			AnimatePress(button);
			AudioManager.Instance.PlaySound("press");       
			};
        button.ButtonUp += () => AnimateRelease(button);
    }

    private void AnimateHover(Button button, bool isHovered)
    {
        GD.Print("ESTAS ENCIMA");
        var tween = GetTree().CreateTween();
        float targetScale = isHovered ? 1.1f : 1.0f;
        tween.TweenProperty(button, "scale", new Vector2(targetScale, targetScale), 0.2f)
             .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
    }

    private void AnimatePress(Button button)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(button, "scale", new Vector2(0.9f, 0.85f), 0.1f)
             .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.InOut);
        
        if (button == optionsButton && !optionsDisplay)
        {
            DisplayOptions();
        }
        else if(button == optionsButton && optionsDisplay){
            HideOptions();
        }

        if(button == startButton){
            HidePause();
        }
    }

    private void AnimateRelease(Button button)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(button, "scale", new Vector2(1.1f, 1.1f), 0.1f)
             .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
    }

    private void DisplayOptions()
    {
        optionsDisplay = true;
        Vector2 startPos = startButton.Position + new Vector2(-900, 0);
        Vector2 exitPos = exitButton.Position + new Vector2(-900, 0);

        AnimateMoveToPosition(startButton, startPos);
        AnimateMoveToPosition(exitButton, exitPos);

        AnimateMoveToPosition(optionsMenu, optionsControlPos);

    }
}
