using Godot;
using System;

public partial class main_menu_node : Node
{
    Node mainNode;
    PackedScene main_prefab;
	int difficulty = 0;
	bool muted = false;
	bool laserHelp = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        main_prefab = (PackedScene)GD.Load("res://main.tscn");
        
        
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (Input.IsKeyPressed(Key.Escape))
        {
            GetTree().Quit();
        }

    }

	public void _on_option_button_item_selected(int index)
	{
		//GD.Print("Diff changed. Item index: " + index);
		difficulty = index;
	}

	public void _on_button_start_pressed()
	{
		//GD.Print("Start pressed");
        mainNode = main_prefab.Instantiate();
        mainNode.Call("set_difficulty", difficulty);
        AddChild(mainNode);
        hide_mainmenu();
		
	}

	public void _on_sound_button_pressed()
	{
		if (!muted)
		{
			GetNode<Sprite2D>("BackgroundSprite").GetNode<Button>("SoundButton").Text = "Sounds OFF";
            var masterSound = AudioServer.GetBusIndex("Master");
            AudioServer.SetBusMute(masterSound, true);
        }
		else
		{
            GetNode<Sprite2D>("BackgroundSprite").GetNode<Button>("SoundButton").Text = "Sounds ON";
            var masterSound = AudioServer.GetBusIndex("Master");
            AudioServer.SetBusMute(masterSound, false);
        }
	}

	public void _on_laser_button_pressed()
	{
		if (!laserHelp)
		{
            GetNode<Sprite2D>("BackgroundSprite").GetNode<Button>("LaserButton").Text = "Laser sight helper: ON";
			laserHelp = true;
        }
		else
		{
            GetNode<Sprite2D>("BackgroundSprite").GetNode<Button>("LaserButton").Text = "Laser sight helper: OFF";
            laserHelp = false;
        }
		GD.Print(laserHelp);
	}

	public bool getLaser()
	{
		return laserHelp;
	}


    public void hide_mainmenu()
	{ 
		GetTree().Root.GetNode<Node>("MainMenuNode").GetNode<Sprite2D>("BackgroundSprite").Visible = false;
	}

	public void show_mainmenu()
	{
        GetTree().Root.GetNode<Node>("MainMenuNode").GetNode<Sprite2D>("BackgroundSprite").Visible = true;
    }
}
