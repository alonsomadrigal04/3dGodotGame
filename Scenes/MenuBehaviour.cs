using Godot;
using System;

public partial class MenuBehaviour : Control
{
        private Button startButton;
    private Button optionsButton;
    private Button exitButton;
    private AudioStreamPlayer hoverSound;
    private AudioStreamPlayer pressSound;
	[Export] private AnimationPlayer animationPlayer;


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
        button.Scale = new Vector2(0.5f, 0.5f);
        tween.TweenProperty(button, "scale", new Vector2(1.0f, 1.0f), 0.2f)
             .SetTrans(Tween.TransitionType.Back)
             .SetEase(Tween.EaseType.Out)
			 .SetDelay(0.2f);
    }

    private void SetupButtonAnimations(Button button)
    {
        button.MouseEntered += () => {
            AnimateHover(button, true);
            hoverSound.Play();
        };
        button.MouseExited += () => AnimateHover(button, false);
        button.Pressed += () => {
			AnimatePress(button);
			pressSound.Play();
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
