using Godot;
using System;

public partial class Settler : Unit
{

	public Settler()
	{
		unitName = "Settler";
		productionRequired = 100;

		maxHp = 1;
		hp = 1;
		attackVal = 0;

		movePoints = 2;
		maxMovePoints = 2;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	public void FoundCity()
	{
		if (map.GetHex(this.coords).ownerCity is null && !City.invalidTiles.ContainsKey(map.GetHex(this.coords)))
		{
			bool valid = true;
			foreach (Hex h in map.GetSurroundingHexes(this.coords))
			{
				valid = h.ownerCity is null && !City.invalidTiles.ContainsKey(h);
			}

			if (valid)
			{
				map.CreateCity(this.civ, this.coords, $"Settled City {coords.X}");
				this.DestroyUnit();
			}
		}
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
