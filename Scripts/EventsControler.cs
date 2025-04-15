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

    [ExportGroup("Pause")]
    [Export] private Control pauseMenu;
    private bool pauseDisplayed;




    public override async void _Ready()
    {
        AnimateMoveToPosition(transitionSprite, new Vector2(1769.0f, 1003.0f));
        
        await ToSignal(GetTree().CreateTimer(2.0f), "timeout");

        BeginCountDown();
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
        countLabel2.Text = "";
    }


    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent && keyEvent.Pressed && keyEvent.Keycode == Key.Escape && playerMovementSc.canMove && !pauseDisplayed)
        {
            PauseDisplay();
        }
        else if(pauseDisplayed){
            HidePause();
        }
    }

    private void HidePause(){
        pauseDisplayed = false;
    }

    private void PauseDisplay()
    {
        pauseDisplayed = true;
        pauseMenu.Visible = true;
        pauseMenu.Modulate = new Color(1, 1, 1, 0);
        pauseMenu.Scale = new Vector2(1.2f, 1.2f);

        Vector2 originalPosition = pauseMenu.Position;

        var tween = GetTree().CreateTween();

        // Fade-in y scale
        tween.Parallel().TweenProperty(pauseMenu, "modulate:a", 1.0f, 0.4f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Sine);

        tween.Parallel().TweenProperty(pauseMenu, "scale", new Vector2(1.0f, 1.0f), 0.4f)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Elastic);



        AudioManager.Instance.PlaySound("pausa");
        //GetTree().Paused = true;
    }


    private void AnimateMoveToPosition(Node button, Vector2 targetPosition)
    {
        var tween = GetTree().CreateTween();
        tween.TweenProperty(button, "position", targetPosition, 0.4f)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);
    }


}
