using Godot;
using System;
using System.Linq;
using static System.Reflection.Metadata.BlobBuilder;

public partial class main : Node
{
	int difficulty;
	int level = 1;
    int ammountBombs;
	int playerScore;
    float bombSpawnRate;
	bool playerAlive;
	bool bombSpawnDone;
	bool levelComplete;
	string diffword;

	Timer bombSpawnTimer;
	Random rnd = new Random();
	Color DSColor;


    PackedScene player_prefab;
	PackedScene floor_prefab;
	PackedScene bomb_prefab;
	PackedScene gameover_prefab;
    PackedScene nuke_prefab;
    Label labelNode;
	Label infoNode;
	Node2D playerNode;
	AudioStreamPlayer2D NukeSound;
	Sprite2D DarkenSprite;
	//DirectionalLight2D expLight;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		//GD.Print(this);
		player_prefab = (PackedScene)GD.Load("res://Prefabs/player.tscn");
		floor_prefab = (PackedScene)GD.Load("res://Prefabs/floor.tscn");
		bomb_prefab = (PackedScene)GD.Load("res://Prefabs/bomb.tscn");
		gameover_prefab = (PackedScene)GD.Load("res://Prefabs/game_over_node.tscn");
		nuke_prefab = (PackedScene)GD.Load("res://Prefabs/nuke_sprite.tscn");
		NukeSound = GetNode<AudioStreamPlayer2D>("NukeSound");
        labelNode = GetTree().Root.GetNode<Node>("MainMenuNode").GetNode<Node>("Main").GetNode<Label>("txtLabel");
        infoNode = GetTree().Root.GetNode<Node>("MainMenuNode").GetNode<Node>("Main").GetNode<Label>("infoLabel");
		DarkenSprite = GetNode<Sprite2D>("DarkenSprite");

        setup_new_level();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{


	}

    public override void _PhysicsProcess(double delta)
    {
        var bombs = GetTree().GetNodesInGroup("bombs");
        infoNode.Text = "Score: " + playerScore + "\nLevel: " + level + "\nDifficulty: " + diffword;

        if (!playerAlive)
        {
            labelNode.Text = "Game Over\nScore: " + playerScore;
        }

        if (playerAlive && bombSpawnDone && bombs.Count == 0 && !levelComplete)
        {
            //GD.Print("bombs group is null");

            level_complete();
        }
    }

    public void setup_new_level()
	{
		if (!playerAlive)
		{
			if(IsInstanceValid(GetNode<Sprite2D>("nuke_sprite")))
			{
				GetNode<Sprite2D>("nuke_sprite").QueueFree();
			}
            GetNode<DirectionalLight2D>("NukeLight").Enabled = false;
            spawn_player();
			spawn_floor();
		}


		levelComplete = false;
        playerAlive = true;
        start_level();

	}

	public async void setLabel(string msg)
	{
		
		labelNode.Text = msg;
        await ToSignal(GetTree().CreateTimer(2f), SceneTreeTimer.SignalName.Timeout);
		labelNode.Text = "";

    }

	public void spawn_player()
	{
		if (!playerAlive)
		{
            playerNode = (Node2D)player_prefab.Instantiate();
            playerNode.Position = new Vector2(350, 900);
			AddChild(playerNode);
		}
	}

	public void spawn_floor()
	{
		if (!playerAlive)
		{
			StaticBody2D floorNode = (StaticBody2D)floor_prefab.Instantiate();
			floorNode.Position = new Vector2(350, 900);
			AddChild(floorNode);
		}
	}

	public void spawn_bomb()
	{
		if (playerAlive)
		{
			RigidBody2D bombNode = (RigidBody2D)bomb_prefab.Instantiate();
			bombNode.Position = new Vector2(rnd.Next(25, 675), -50);
			bombNode.Name = "Bomb";
			AddChild(bombNode);
		}
	}

	public async void start_level()
	{
        bombSpawnDone = false;
        switch (difficulty)
        {
            case 0:
				//ammountBombs = 0;
				ammountBombs = 5 * level;
				bombSpawnRate = 1.5f;
                diffword = "Easy";
                break;
            case 1:
                ammountBombs = 10 * level;
                bombSpawnRate = 1.0f;
                diffword = "Medium";
                break;
            case 2:
                ammountBombs = 20 * level;
                bombSpawnRate = 0.5f;
                diffword = "Hard";
                break;
        }

		bool laserHelper = (bool)GetTree().Root.GetNode<Node>("MainMenuNode").Call("getLaser");
		if (laserHelper)
		{
			GetNode<Node2D>("Player").GetNode<Sprite2D>("Gun").GetNode<Sprite2D>("HelpingLaser").Visible = true;
		}
		else
		{
            GetNode<Node2D>("Player").GetNode<Sprite2D>("Gun").GetNode<Sprite2D>("HelpingLaser").Visible = false;
        }

        float alpha = rnd.Next(0, 200);
        DSColor.A =  alpha / 100;
        DarkenSprite.Modulate = DSColor;

		int starryChance = rnd.Next(0, 1000);
		if (starryChance == 420)
        {
            GetNode<Sprite2D>("BackgroundStarry").Visible = true;
            GetNode<Sprite2D>("BackgroundSprite").Visible = false;
			DSColor.A = 0;
            DarkenSprite.Modulate = DSColor;
        }
        else
        {
            GetNode<Sprite2D>("BackgroundSprite").Visible = true;
            GetNode<Sprite2D>("BackgroundStarry").Visible = false;
        }

		if (level >= 100)
		{
            setLabel("Level " + level.ToString() + "\nPlease get a life.");
        }
		else
		{
			setLabel("Level " + level.ToString());
		}


		if (playerAlive)
		{
            spawn_bomb();
            for (int i = 1; i < ammountBombs; i++)
			{
				await ToSignal(GetTree().CreateTimer(bombSpawnRate), SceneTreeTimer.SignalName.Timeout);
				spawn_bomb();
			}
			bombSpawnDone = true;
		}
    }

	public async void level_complete()
	{
		levelComplete= true;
            for (int i = 4; i >= 0; i--)
            {
                labelNode.Text = "Level Complete!\nNext level starting.. " + (i+1);
                await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
            }
			level++;
			start_level();
			
        levelComplete = false;
    }

	public void kill_player()
	{
        playerAlive = false;
        Node gameovernode = (Node)gameover_prefab.Instantiate();
        AddChild(gameovernode);
        playerNode.QueueFree();
		Sprite2D nukesprite = (Sprite2D)nuke_prefab.Instantiate();
		nukesprite.Name = "nuke_sprite";
		AddChild(nukesprite);
		GetNode<DirectionalLight2D>("NukeLight").Enabled = true;
		NukeSound.Play();
	}

	public bool check_player_alive()
	{
		return playerAlive;
	}

	public void add_score()
	{
		playerScore = playerScore + (1 * (difficulty + 1));
	}

	public void reset_level()
	{
		level = 1;
	}

	public void reset_score()
	{
		playerScore = 0;
	}

	public void set_difficulty(int diff)
	{
		difficulty = diff;
	}

	public float get_darkensprite_alpha()
	{
		return DSColor.A;
	}

	public void testfunc()
	{
		GD.Print("testfunc triggered");
	}
}
