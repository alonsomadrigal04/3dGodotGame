using Godot;
using Player;
using System;

public partial class EventsControler : Node
{
    [ExportGroup("transitions")]
    [Export] private Control transitionSprite;
    [Export] private FlyMovement playerMovementSc;
    [Export] public Label countLabel2;
    [Export] public Label countDown;

    public override async void _Ready()
    {
        AnimateMoveToPosition(transitionSprite, new Vector2(1769.0f, 1003.0f));
        
        await ToSignal(GetTree().CreateTimer(2.0f), "timeout");

        //BeginCountDown();
        StartGame();
    }

    private async void BeginCountDown()
    {
        AudioManager.Instance.PlayMusic("metalMusicFondo");
        Node parent = this;

        for (int i = 3; i > 0; i--)
        {
            playerMovementSc.CameraSchake(1);
            countDown.Text = i.ToString();

            var tween = GetTree().CreateTween();

            countDown.Scale = new Vector2(0.8f, 0.8f);
            var modulateColor = countDown.Modulate;
            modulateColor.A = 1.0f;
            countDown.Modulate = modulateColor;
            tween.TweenProperty(countDown, "scale", new Vector2(1.0f, 1.0f), 0.25f)
                .SetTrans(Tween.TransitionType.Elastic)
                .SetEase(Tween.EaseType.Out);

            tween.TweenProperty(countDown, "modulate:a", 0f, 0.4f)
                .SetTrans(Tween.TransitionType.Cubic)
                .SetEase(Tween.EaseType.In)
                .SetDelay(0.5f);
            AudioManager.Instance.PlaySound("guitar" + i.ToString());

            await ToSignal(tween, "finished");

            await ToSignal(GetTree().CreateTimer(0.1), "timeout");


            GD.Print(countLabel2.Scale);
        }
        StartGame();
        GD.Print("Cuenta regresiva terminada!");
    }

    public async void StartGame()
    {
        var fontSizes = new int[] { 80, 120, 160 }; // Tamaños crecientes
        AudioManager.Instance.PlaySound("go");
        int shakeintensity = 2;

        foreach (var size in fontSizes)
        {
            countLabel2.Text = "GO!";
            countLabel2.AddThemeFontSizeOverride("font_size", size);

            // Pequeño efecto de escala visual (opcional)
            var tween = GetTree().CreateTween();
            countLabel2.Scale = new Vector2(0.8f, 0.8f);
            tween.TweenProperty(countLabel2, "scale", new Vector2(1.0f, 1.0f), 0.25f)
                .SetTrans(Tween.TransitionType.Elastic)
                .SetEase(Tween.EaseType.Out);

            // Espera entre cada "GO"
            playerMovementSc.CameraSchake(shakeintensity++);
            await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
        }
        // Al terminar, oculta el label o lo limpia
        playerMovementSc.canMove = true;
        AudioManager.Instance.PlayMusic("DoomMusic");
        //AudioManager.Instance.FadeInMusic();
        countLabel2.Text = "";
    }

    private void AnimateMoveToPosition(Node button, Vector2 targetPosition)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(button, "position", targetPosition, 0.4f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
    }
}
