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

    private string[] deathMessages = new string[]
    {
        "You have literally \n[color=#c21807][shake rate=20.0 level=20 connected=1]DIED[/shake][/color]",
        "Well...\n[color=red][wave freq=10 amp=20]That was quick[/wave][/color]",
        "Game over, man!\n[color=darkred][shake]You’re toast[/shake][/color]",
        "Hope you had fun!\n[color=crimson][rainbow freq=4 sat=0.5 val=0.8]NOT.[/rainbow][/color]",
        "RIP little fly\n[color=orangered][shake rate=15 level=15]squashed[/shake][/color]"
    };

    public override void _Ready()
    {
        ShowDeathText();
        AudioManager.Instance.PlayMusic("DeadMusic");
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
}
