using Godot;
using System;

public partial class AnimationDeadScreen : Control
{
    [Export] private RichTextLabel deadText;
    [Export] private float waitTimeBeforeSlide = 2.0f;
    [Export] private float slideDistance = 150f;
    [Export] private float slideDuration = 0.5f;
    [Export] private Button startAgain;
    [Export] private Button exit;
    [Export] private Sprite2D transitionSprite;
    [Export] private PackedScene gameScene;

    private string[] deathMessages = new string[]
    {
        "You have literally \n[color=#c21807][shake rate=20.0 level=20 connected=1]DIED[/shake][/color]",
        "Well...\n[color=red][wave freq=10 amp=20]That was quick[/wave][/color]",
        "One more time, [color=#888888]with feeling.[/color]",
        "Hope you had fun!\n[color=crimson][rainbow freq=4 sat=0.5 val=0.8]NOT.[/rainbow][/color]",
        "RIP little fly\n[color=orangered][shake rate=15 level=15]squashed[/shake][/color]",
        "Hope you like jam.\n[color=crimson][rainbow freq=3 val=0.7]Because you're part of it now[/rainbow][/color]",
        "[color=#888888][shake rate=20.0 level=20 connected=1]Classic.[/shake][/color]",
        "Again? [color=#aaaaaa][shake rate=4 level=4]Of course.[/shake][/color]"

    };

    public override void _Ready()
    {
        ShowDeathText();
        Input.MouseMode = Input.MouseModeEnum.Visible;
        AudioManager.Instance.PlayMusic("DeadMusic");

        SetupButtonAnimations(startAgain);
        SetupButtonAnimations(exit);
    }

    public async void ShowDeathText()
    {
        string selectedMessage = deathMessages[GD.Randi() % deathMessages.Length];
        deadText.Text = selectedMessage;
        deadText.VisibleCharacters = -1;
        deadText.Scale = new Vector2(3f, 3f);

        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(deadText, "scale", new Vector2(1f, 1f), 0.4f)
            .SetTrans(Tween.TransitionType.Elastic)
            .SetEase(Tween.EaseType.Out);

        await ToSignal(GetTree().CreateTimer(waitTimeBeforeSlide), "timeout");

        // Slide text
        Vector2 finalPosition = deadText.Position - new Vector2(0, slideDistance);
        Tween moveTween = GetTree().CreateTween();
        moveTween.TweenProperty(deadText, "position", finalPosition, slideDuration)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);

        // Llama a los botones justo al iniciar el slide
        SummonButtons();
    }

    private void SummonButtons()
    {
        startAgain.Visible = true;
        exit.Visible = true;

        startAgain.Scale = new Vector2(0, 0);
        exit.Scale = new Vector2(0, 0);

        Tween tween = GetTree().CreateTween();

        tween.Parallel().TweenProperty(startAgain, "scale", new Vector2(1, 1), 0.5f)
            .SetTrans(Tween.TransitionType.Elastic)
            .SetEase(Tween.EaseType.Out);

        tween.Parallel().TweenProperty(exit, "scale", new Vector2(1, 1), 0.5f)
            .SetTrans(Tween.TransitionType.Elastic)
            .SetEase(Tween.EaseType.Out);
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

        if(button == startAgain){
            StartGame();
        }
    }

    private void StartGame(){
        AudioManager.Instance.FadeOutMusic(1f);
        var tween = GetTree().CreateTween();
        tween.TweenProperty(transitionSprite, "position", new Vector2(103.0f, -38.0f), 1f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);

        tween.Finished += () => ChangeScene("res://3dGodotGame/Scenes/MainScenario.tscn");

    }
    private void ChangeScene(string escena)
    { 
        GD.Print("holamundo");
        GetTree().ChangeSceneToFile(escena);
    }

    private void AnimateRelease(Button button)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(button, "scale", new Vector2(1.1f, 1.1f), 0.1f)
             .SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
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
}
