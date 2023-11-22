using Godot;
using System;

public partial class bullet : RigidBody2D
{
	bool bulletdead;
	Vector2 lastloc;

	Node mainNode;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		mainNode = GetTree().Root.GetNode<Node>("MainMenuNode").GetNode<Node>("Main");
		ApplyForce(Vector2.Up.Rotated(Rotation) * 300);
        //Name = "Bullet";
        deleteBulletDelay();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!(bool)mainNode.Call("check_player_alive"))
		{
			QueueFree();
		}
		if (bulletdead)
		{
			Position = lastloc;
		}

    }

	public async void deleteBulletDelay()
	{
        await ToSignal(GetTree().CreateTimer(5f), SceneTreeTimer.SignalName.Timeout);
		QueueFree();
    }

    public async void deleteBullet()
    {
        await ToSignal(GetTree().CreateTimer(0.025f), SceneTreeTimer.SignalName.Timeout);
        QueueFree();
    }

    public void bulletHit(string name)
	{
        RigidBody2D bulletNode = GetTree().Root.GetNode<Node>("MainMenuNode").GetNode<Node>("Main").GetNode<RigidBody2D>(name);
        //GD.Print("bulletHit");
        Sprite2D bulletSprite = bulletNode.GetNode<Sprite2D>("Sprite2D");
        Sprite2D bulletdSprite = bulletNode.GetNode<Sprite2D>("SpriteDestroy");

		lastloc = bulletNode.GlobalPosition;
		bulletSprite.Visible = false;
		bulletdSprite.Visible = true;

		bulletdead= true;

		deleteBullet();
	}
}
