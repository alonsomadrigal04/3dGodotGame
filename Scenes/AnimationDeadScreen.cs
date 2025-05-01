using Godot;
using System;

public partial class AnimationDeadScreen : Control
{
    [Export] private RichTextLabel deadText;

    private string[] deathMessages = new string[]
    {
        "You have literally \n[color=#c21807][shake rate=20.0 level=20 connected=1]DIED[/shake][/color]",
        "Well...\n[color=red][wave freq=10 amp=20]That was quick[/wave][/color]",
        "Game over, man!\n[color=darkred][shake]You're toast[/shake][/color]",
        "Hope you had fun!\n[color=crimson][rainbow freq=4 sat=0.5 val=0.8]NOT.[/rainbow][/color]",
        "RIP little fly\n[color=orangered][shake rate=15 level=15]squashed[/shake][/color]"
    };

    public override void _Ready()
    {
        ShowDeathText();
    }

    public void ShowDeathText()
    {
        // Elegir un texto aleatorio
        string selectedMessage = deathMessages[GD.Randi() % deathMessages.Length];
        deadText.Text = selectedMessage;
        deadText.VisibleCharacters = -1; // Asegura que se muestre todo el texto
        deadText.Scale = new Vector2(3f, 3f); // Empieza grande

        // Crear tween para "chocar" con la pantalla
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(deadText, "scale", new Vector2(1f, 1f), 0.4f)
            .SetTrans(Tween.TransitionType.Elastic)
            .SetEase(Tween.EaseType.Out);
    }
}
