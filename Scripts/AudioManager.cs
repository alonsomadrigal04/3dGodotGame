using Godot;
using System;
using System.Collections.Generic;


public partial class AudioManager : Node
{
	public static AudioManager Instance {get; private set;}
	public bool IsReady { get; private set; } = false;

	[Export] public AudioStreamPlayer audioStreamMusic;
	[Export] public AudioStreamPlayer audioStreamSFX;


	public override void _Ready()
	{
		Instance = this;
		IsReady = true;

	}

    public float linearToDb(double linear){
        if(linear <= 0)
            return -80;
        return (float)(20.0f * Math.Log10(linear));
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
			audioStreamSFX.Stream = Sounds[name];
			audioStreamSFX.Play();
		}
		else
		{
			var tween = GetTree().CreateTween();
			tween.TweenCallback(Callable.From(() =>
			{
				audioStreamSFX.Stream = Sounds[name];
				audioStreamSFX.Play();
			})).SetDelay(delay);
		}
	}

	public void PlayMusic(string name, float delay = 0f)
	{
		if (!Sounds.ContainsKey(name))
		{
			GD.PrintErr($"AudioManager: Sound '{name}' not found.");
			
			return;
		}

		if (delay <= 0f)
		{
			audioStreamMusic.Stream = Sounds[name];
			audioStreamMusic.Play();
		}
		else
		{
			var tween = GetTree().CreateTween();
			tween.TweenCallback(Callable.From(() =>
			{
				audioStreamMusic.Stream = Sounds[name];
				audioStreamMusic.Play();
			})).SetDelay(delay);
		}
	}


}
