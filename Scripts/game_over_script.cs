using Godot;
using System;

public partial class game_over_script : Node
{
    Node mainNode;
    Node mainmenuNode;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        mainNode = GetTree().Root.GetNode<Node>("MainMenuNode").GetNode<Node>("Main");
        mainmenuNode = GetTree().Root.GetNode<Node>("MainMenuNode");
        //mainNode.Call("kill_player");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void _on_try_again_button_pressed()
    {
        //Try again pressed
        mainNode.Call("reset_level");
        mainNode.Call("reset_score");
        mainNode.Call("setup_new_level");
        QueueFree();
    }

    public void _on_menu_button_pressed()
    {
        mainmenuNode.Call("show_mainmenu");
        mainNode.QueueFree();
        QueueFree();
        //Menu pressed
    }
}
