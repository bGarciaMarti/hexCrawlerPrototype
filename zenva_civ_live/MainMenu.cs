using Godot;
using System;

public partial class MainMenu : Node2D
{

	public void Start()
	{
		Game g = ResourceLoader.Load<PackedScene>("Game.tscn").Instantiate() as Game;
		HexTileMap map = g.GetNode<HexTileMap>("HexTileMap");

		map.height = (int) this.GetNode<SpinBox>("VBoxContainer/HBoxContainer/SpinBox2").Value;
		map.width = (int) this.GetNode<SpinBox>("VBoxContainer/HBoxContainer/SpinBox").Value;

		map.NUM_AI_CIVS = (int) this.GetNode<SpinBox>("VBoxContainer/HBoxContainer2/SpinBox").Value;

		map.PLAYER_COLOR = this.GetNode<ColorPickerButton>("VBoxContainer/HBoxContainer3/ColorPickerButton").Color;

		GetNode("/root/MainMenu").QueueFree();
		GetTree().Root.AddChild(g);
	}


	public void Quit()
	{
		GetTree().Quit();
	}

}
