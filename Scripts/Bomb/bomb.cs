using Godot;
using System;

public partial class bomb : RigidBody2D
{
	Node mainNode;
	RigidBody2D bulletNode;
	Sprite2D explosionSprite;
    DirectionalLight2D expLight;
	AudioStreamPlayer2D ExplodeSound;
	AudioStreamPlayer2D SpawnSound;

    bool bombdead = false;
	Vector2 lastloc;
	Vector2 tempScale;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        mainNode = GetTree().Root.GetNode<Node>("MainMenuNode").GetNode<Node>("Main");
        expLight = GetNode<DirectionalLight2D>("ExplosionLight");
		ExplodeSound = GetNode<AudioStreamPlayer2D>("ExplodeSound");
		SpawnSound = GetNode<AudioStreamPlayer2D>("SpawnSound");
        Name = "Bomb";
		AddToGroup("bombs");
		//bombdead = false;
		SpawnSound.Play();
        explosionSprite = GetNode<Sprite2D>("ExplosionSprite");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

    }

    public override void _PhysicsProcess(double delta)
    {
        if (!(bool)mainNode.Call("check_player_alive"))
        {
            QueueFree();
        }

        if (bombdead)
        {
            Position = lastloc;
        }

        if (explosionSprite.Visible)
        {
            explosionSprite.Scale = explosionSprite.Scale.Lerp(Vector2.Zero, 0.1f);
        }

        if (expLight.Enabled)
        {
            float lerped = Mathf.Lerp(expLight.Energy, 0f, 0.1f);
            expLight.Energy = lerped;
            //GD.Print(lerped);
        }
    }


    private async void OnEnter(Node other)
	{
		//GD.Print("Hit by: " + other.Name);
		if (other.SceneFilePath == "res://Prefabs/bullet.tscn" && (bool)mainNode.Call("check_player_alive") && !bombdead)
		{
			Random rnd = new Random();
			int rndRot = rnd.Next(0, 360);
			int rndPitch = rnd.Next(80, 120);
            bombdead = true;
			ExplodeSound.PitchScale = (rndPitch / 100f);
			ExplodeSound.Play();
			lastloc.X = GlobalPosition.X;
            lastloc.Y = GlobalPosition.Y;
            //other.Call("bulletHit");
            bulletNode = mainNode.GetNode<RigidBody2D>(other.Name.ToString());
            bulletNode.Call("bulletHit", other.Name);
			mainNode.Call("add_score");
			GetNode<Sprite2D>("BombSprite").Visible = false;
            explosionSprite.GlobalRotationDegrees = rndRot;
			explosionSprite.Scale = new Vector2(0.4f, 0.4f);
            explosionSprite.Visible = true;
			expLight.Enabled = true;
			expLight.Energy = (2f + (float)mainNode.Call("get_darkensprite_alpha"));
            await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
            QueueFree();
        }

		if (other.Name == "Floor")
		{
			//GD.Print("Hit the floor!");
            mainNode.Call("kill_player");
        }
		

	}

}



