using Godot;
using System;

public partial class Warrior : Unit
{

	public Warrior()
	{
		unitName = "Warrior";
		productionRequired = 50;

		maxHp = 3;
		hp = 3;
		attackVal = 2;
		
		movePoints = 1;
		maxMovePoints = 1;
	}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
