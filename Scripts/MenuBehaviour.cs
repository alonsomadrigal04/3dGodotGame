using Godot;
using System;
using System.IO;

public partial class MenuBehaviour : Control
{
    [ExportGroup("botones")]
    private Button startButton;
    private Button optionsButton;
    private Button exitButton;

    [ExportGroup("Sonidos")]
    [Export] private AudioStreamPlayer hoverSound;
    [Export] private AudioStreamPlayer pressSound;
    [Export] private AudioStreamPlayer explosionAudio;

    [ExportGroup("Others")]

	[Export] private AnimationPlayer animationPlayer;
    private bool isSound = false;
    
    [ExportGroup("animation Settings")]
    [Export] private float animationDelay = 0.2f;
    [Export] private Vector2 offsetPosition = new Vector2(0, -100);

    public override void _Process(double delta)
    {
        if(!isSound){
            AudioManager.Instance.PlaySound("explosion", 2.0f);
            isSound = true;
        }
    }


    public override void _Ready()
    {

        startButton = GetNode<Button>("StartButton");
        optionsButton = GetNode<Button>("OptionsButton");
        exitButton = GetNode<Button>("ExitButton");
        hoverSound = GetNode<AudioStreamPlayer>("HoverSound");
		pressSound = GetNode<AudioStreamPlayer>("PressSound");

        // Animación de entrada de los botones
        AnimateButtonEntry(startButton);
        AnimateButtonEntry(optionsButton);
        AnimateButtonEntry(exitButton);

        SetupButtonAnimations(startButton);
        SetupButtonAnimations(optionsButton);
        SetupButtonAnimations(exitButton);

    }




    private void AnimateButtonEntry(Button button)
    {
        var tween = GetTree().CreateTween();

        Vector2 finalPosition = button.Position;

        button.Position += offsetPosition;
        button.Scale = new Vector2(0.5f, 0.5f);

        tween.TweenProperty(button, "position", finalPosition, 0.3f)
            .SetTrans(Tween.TransitionType.Elastic)
            .SetEase(Tween.EaseType.Out)
            .SetDelay(animationDelay);

        tween.TweenProperty(button, "scale", new Vector2(1.0f, 1.0f), 0.2f)
            .SetTrans(Tween.TransitionType.Back)
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
    }

    private void AnimateRelease(Button button)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(button, "scale", new Vector2(1.1f, 1.1f), 0.1f)
             .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
    }
}
