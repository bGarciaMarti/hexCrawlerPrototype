using Godot;
using System;
using System.Collections.Generic;

public class Civilization
{

	public int id;
	public List<City> cities;
	public List<Unit> units;
	public Color territoryColor;
	public int territoryColorAltTileId;
	public string name;
	public bool playerCiv;

	public int maxUnits = 3;

	public Civilization()
	{
		cities = new List<City>();
		units = new List<Unit>();
	}

	public void SetRandomColor()
	{
		Random r = new Random();
		territoryColor = new Color(r.Next(255)/255.0f, r.Next(255)/255.0f, r.Next(255)/255.0f);
	}

	public void ProcessTurn()
	{
		foreach (City c in cities)
		{
			c.ProcessTurn();
		}

		if (!playerCiv)
		{
			Random r = new Random();

			foreach (City c in cities)
			{
				int rand = r.Next(30);

				if (rand > 27)
				{
					c.AddUnitToBuildQueue(new Warrior());
				}

				if (rand > 28)
				{
					c.AddUnitToBuildQueue(new Settler());
				}
			}

			List<Settler> citiesToFound = new List<Settler>();
			for (int i = 0; i < units.Count; i++)
			{
				Unit u = units[i];
				u.RandomMove();

				if (u is Settler && r.Next(10) > 8)
				{
					Settler s = u as Settler;
					citiesToFound.Add(s);
				}
			}

			for (int i = 0; i < citiesToFound.Count; i++)
			{
				citiesToFound[i].FoundCity();
			}
		}

		maxUnits = this.cities.Count * 3;
	}

}
