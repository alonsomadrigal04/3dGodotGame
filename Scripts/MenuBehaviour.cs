using Godot;
using System;
using System.IO;

public partial class MenuBehaviour : Control
{
    [ExportGroup("botones")]
    private Button startButton;
    private Button optionsButton;
    private Button exitButton;

    [ExportGroup("Music")]
    [Export] Slider musicSlider;
    [Export] Slider sfxSlider;
    private bool isSound = false;

    [ExportGroup("CheckBoxes")]
    [Export] WorldEnvironment worldEnvironment;
    [Export] CheckBox metalMode;
    [Export] CheckBox fogMode;


    [ExportGroup("Options")]
    private bool optionsDisplay = false;
    Vector2 optionsControlPos;
    [Export] private Control optionsMenu;
    
    [ExportGroup("animation Settings")]
    [Export] private float animationDelay = 0.2f;
    [Export] private Vector2 offsetPosition = new Vector2(0, -100);


    public override void _Process(double delta)
    {
        if(!isSound){
            AudioManager.Instance.PlaySound("explosion", 2.0f);
            isSound = true;
            AudioManager.Instance.PlayMusic("metal");
        }




        if(metalMode.ButtonPressed){
            worldEnvironment.Environment.FogLightColor = new Color("a42733"); // Rojo puro
        }
        else{
            worldEnvironment.Environment.FogLightColor = new Color("b87784"); // Rojo puro
        }

        if(fogMode.ButtonPressed){
            worldEnvironment.Environment.FogEnabled = true;
        }
        else{
            worldEnvironment.Environment.FogEnabled = false;
        }

        AudioManager.Instance.audioStreamMusic.VolumeDb = AudioManager.Instance.linearToDb(musicSlider.Value);
        AudioManager.Instance.audioStreamSFX.VolumeDb = AudioManager.Instance.linearToDb(sfxSlider.Value);

    }

    private void DisplayOptions()
    {
        optionsDisplay = true;
        Vector2 startPos = startButton.Position + new Vector2(-500, 0);
        Vector2 exitPos = exitButton.Position + new Vector2(-500, 0);

        AnimateMoveToPosition(startButton, startPos);
        AnimateMoveToPosition(exitButton, exitPos);

        AnimateMoveToPosition(optionsMenu, optionsControlPos);

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


    public override void _Ready()
    {

        startButton = GetNode<Button>("StartButton");
        optionsButton = GetNode<Button>("OptionsButton");
        exitButton = GetNode<Button>("ExitButton");

        // Animación de entrada de los botones
        AnimateButtonEntry(startButton);
        AnimateButtonEntry(optionsButton);
        AnimateButtonEntry(exitButton);

        SetupButtonAnimations(startButton);
        SetupButtonAnimations(optionsButton);
        SetupButtonAnimations(exitButton);

        optionsControlPos = optionsMenu.Position;
        optionsMenu.Position = new Vector2(800, 0);

    }




    private void AnimateButtonEntry(Control button)
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
        
        if (button == optionsButton && !optionsDisplay)
        {
            DisplayOptions();
        }
        else if(button == optionsButton && optionsDisplay){
            HideOptions();
        }
    }

    private void AnimateRelease(Button button)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(button, "scale", new Vector2(1.1f, 1.1f), 0.1f)
             .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
    }
}
