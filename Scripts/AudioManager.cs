using Godot;
using System;
using System.Collections.Generic;


public partial class AudioManager : AudioStreamPlayer
{
    public static AudioManager Instance {get; private set;}
    public bool IsReady { get; private set; } = false;


    public override void _Ready()
    {
        Instance = this;
        IsReady = true;

    }

    [Export] public Godot.Collections.Dictionary<string, AudioStream> Sounds = new();

    public void PlaySound(string name, float delay = 0f)
    {
        if (!Sounds.ContainsKey(name))
        {
            GD.PrintErr($"AudioManager: Sound '{name}' not found.");
            return;
        }

        if (delay <= 0f)
        {
            Stream = Sounds[name];
            Play();
        }
        else
        {
            var tween = GetTree().CreateTween();
            tween.TweenCallback(Callable.From(() =>
            {
                Stream = Sounds[name];
                Play();
            })).SetDelay(delay);
        }
    }


}
