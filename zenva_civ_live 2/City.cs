using Godot;
using System;
using System.Collections.Generic;

public partial class City : Node2D
{

	public static Dictionary<Hex, City> invalidTiles = new Dictionary<Hex, City>();

	public HexTileMap map;
	public Vector2I centerCoordinates;

	public List<Hex> territory;
	public List<Hex> borderTilePool; 

	public Civilization civ;


	// Gameplay constant
	public static int POPULATION_THRESHOLD_INCREASE = 15;

	// City name
	public string name;

	// Units
	public List<Unit> unitBuildQueue;
	public Unit currentUnitBeingBuilt;
	public int unitBuildTracker = 0;

	// Population
	public int population = 1;
	public int populationGrowthThreshold;
	public int populationGrowthTracker;


	// Resources
	public int totalFood;
	public int totalProduction;


	// Scene nodes
	Label label;
	Sprite2D sprite;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		label = GetNode<Label>("Label");
		sprite = GetNode<Sprite2D>("Sprite2D");

		label.Text = name;

		territory = new List<Hex>();
		borderTilePool = new List<Hex>();
		unitBuildQueue = new List<Unit>();
	}

	public void ProcessTurn()
	{
		CleanUpBorderPool();

		populationGrowthTracker += totalFood;
		if (populationGrowthTracker > populationGrowthThreshold) // Grow population
		{
			population++;
			populationGrowthTracker = 0;
			populationGrowthThreshold += POPULATION_THRESHOLD_INCREASE;

			// Grow territory
			AddRandomNewTile();
			map.UpdateCivTerritoryMap(civ);

		}

		ProcessUnitBuildQueue();
	}

	public void CleanUpBorderPool()
	{
		List<Hex> toRemove = new List<Hex>();
		foreach (Hex b in borderTilePool)
		{
			if (invalidTiles.ContainsKey(b) && invalidTiles[b] != this)
			{
				toRemove.Add(b);
			}
		}

		foreach (Hex b in toRemove)
		{
			borderTilePool.Remove(b);
		}
	}

	public void AddRandomNewTile()
	{
		if (borderTilePool.Count > 0)
		{
			Random r = new Random();
			int index = r.Next(borderTilePool.Count);
			this.AddTerritory(new List<Hex>{borderTilePool[index]});
			borderTilePool.RemoveAt(index);
		}
	}

	public void AddTerritory(List<Hex> territoryToAdd)
	{

		foreach (Hex h in territoryToAdd)
		{
			h.ownerCity = this;

			// Add new border hexes to the border tile pool
			AddValidNeighborsToBorderPool(h);
		}

		territory.AddRange(territoryToAdd);
		CalculateTerritoryResourceTotals();

	}

	public void AddValidNeighborsToBorderPool(Hex h)
	{
		List<Hex> neighbors = map.GetSurroundingHexes(h.coordinates);

		foreach (Hex n in neighbors)
		{
			if (IsValidNeighborTile(n)) borderTilePool.Add(n);

			invalidTiles[n] = this;
		}
	}

	public bool IsValidNeighborTile(Hex n)
	{
		if (n.terrainType == TerrainType.WATER ||
			n.terrainType == TerrainType.ICE ||
			n.terrainType == TerrainType.SHALLOW_WATER ||
			n.terrainType == TerrainType.MOUNTAIN)
		{
			return false;
		}

		if (n.ownerCity != null && n.ownerCity.civ != null)
		{
			return false;
		}

		if (invalidTiles.ContainsKey(n) && invalidTiles[n] != this)
		{
			return false;
		}

		return true;
	}

	public void CalculateTerritoryResourceTotals()
	{
		totalFood = 0;
		totalProduction = 0;
		foreach (Hex h in territory)
		{
			totalFood += h.food;
			totalProduction += h.production;
		}
	}

	// Unit functions

	public void AddUnitToBuildQueue(Unit u)
	{
		if (this.civ.maxUnits > this.civ.units.Count)
			unitBuildQueue.Add(u);
	}

	public void SpawnUnit(Unit u)
	{
		Unit unitToSpawn = (Unit) Unit.unitSceneResources[u.GetType()].Instantiate();
		unitToSpawn.Position = map.MapToLocal(this.centerCoordinates);
		unitToSpawn.SetCiv(this.civ);
		unitToSpawn.coords = this.centerCoordinates;

		map.AddChild(unitToSpawn);
	}

	public void ProcessUnitBuildQueue()
	{
		if (unitBuildQueue.Count > 0)
		{
			if (currentUnitBeingBuilt == null)
			{
				currentUnitBeingBuilt = unitBuildQueue[0];
			}

			unitBuildTracker += totalProduction;

			if (unitBuildTracker >= currentUnitBeingBuilt.productionRequired)
			{
				SpawnUnit(currentUnitBeingBuilt);

				unitBuildQueue.RemoveAt(0);
				currentUnitBeingBuilt = null;

				unitBuildTracker = 0;
			}
		}
	}

	public void ChangeOwnership(Civilization newOwner)
	{
		Civilization oldOwner = this.civ;

		this.civ.cities.Remove(this);
		newOwner.cities.Add(this);

		this.civ = newOwner;

		SetIconColor(newOwner.territoryColor);

		map.UpdateCivTerritoryMap(newOwner);
		map.UpdateCivTerritoryMap(oldOwner);
	}

	public void SetCityName(string newName)
	{
		name = newName;
		label.Text = newName;
	}

	public void SetIconColor(Color c)
	{
		sprite.Modulate = c;
	}
}
