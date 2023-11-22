using Godot;
using GodotPlugins.Game;
using System;

public partial class Gun : Sprite2D
{
    bool canShoot = true;
    float shootRate = 0.3f;
    float rotation_speed = 0.02f;
    PackedScene bullet_prefab;
    AudioStreamPlayer2D SoundPlayer;
    Node mainNode;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        mainNode = GetTree().Root.GetNode<Node>("MainMenuNode").GetNode<Node>("Main");
        SoundPlayer = mainNode.GetNode<Node2D>("Player").GetNode<Sprite2D>("Gun").GetNode<Node2D>("GunPoint").GetNode<AudioStreamPlayer2D>("FireSound");
        bullet_prefab = (PackedScene)GD.Load("res://Prefabs/bullet.tscn");
    }

    // Called every visual frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }

    //Called every physics frame.
    public override void _PhysicsProcess(double delta)
    {
        //GD.Print("Current angle: " + Rotation.ToString());
        if (Input.IsActionPressed("Right") && RotationDegrees < 70f && (bool)mainNode.Call("check_player_alive"))
        {
            if (Input.IsKeyPressed(Key.Ctrl))
            {
                Rotate(rotation_speed * 0.5f);
            }
            else
            {
                Rotate(rotation_speed);
            }
        }

        if (Input.IsActionPressed("Left") && RotationDegrees > -70f && (bool)mainNode.Call("check_player_alive"))
        {
            if (Input.IsKeyPressed(Key.Ctrl))
            {
                Rotate(-rotation_speed * 0.5f);
            }
            else
            {
                Rotate(-rotation_speed);
            }
        }


        if (Input.IsActionPressed("Fire") && (bool)mainNode.Call("check_player_alive"))
        {
            //GD.Print("Fire");
            ShootGun();
        }

        if (Input.IsActionJustPressed("FireAlt"))
        {
            //GD.Print("FireAlt");
        }
        //GD.Print(RotationDegrees);
    }

    public void ShootGun()
    {
        if (canShoot)
        {
            Random rnd = new Random();
            int rndPitch = rnd.Next(95, 105);
            RigidBody2D bulletNode = (RigidBody2D)bullet_prefab.Instantiate();
            bulletNode.Position = GetNode<Node2D>("GunPoint").GlobalPosition;
            bulletNode.Rotation = GetNode<Node2D>("GunPoint").GlobalRotation;
            mainNode.AddChild(bulletNode);
            SoundPlayer.PitchScale = (rndPitch / 100f);
            SoundPlayer.Play();
            canShoot = false;
            ShootCooldown();
        }
    }

    public async void ShootCooldown()
    {
        await ToSignal(GetTree().CreateTimer(shootRate), SceneTreeTimer.SignalName.Timeout);
        canShoot = true;
    }


}
