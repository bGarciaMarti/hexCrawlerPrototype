using Godot;
using System;
using System.Collections.Generic;

public partial class TerrainTileUI : Panel
{

	public static Dictionary<TerrainType, string> terrainTypeStrings = new Dictionary<TerrainType, string>
	{
		{ TerrainType.PLAINS, "Plains" },
		{ TerrainType.BEACH, "Beach"},
		{ TerrainType.DESERT, "Desert"},
		{ TerrainType.MOUNTAIN, "Mountain"},
		{ TerrainType.ICE, "Ice"},
		{ TerrainType.WATER, "Water"},
		{ TerrainType.SHALLOW_WATER, "Shallow Water"},
		{ TerrainType.FOREST, "Forest"},
	};

	public static Dictionary<TerrainType, Texture2D> terrainTypeImages = new();

	public static void LoadTerrainImages()
	{
		Texture2D plains = ResourceLoader.Load("res://textures/plains.jpg") as Texture2D;
		Texture2D beach = ResourceLoader.Load("res://textures/beach.jpg") as Texture2D;
		Texture2D desert = ResourceLoader.Load("res://textures/desert.jpg") as Texture2D;
		Texture2D mountain = ResourceLoader.Load("res://textures/mountain.jpg") as Texture2D;
		Texture2D ice = ResourceLoader.Load("res://textures/ice.jpg") as Texture2D;
		Texture2D ocean = ResourceLoader.Load("res://textures/ocean.jpg") as Texture2D;
		Texture2D shallow = ResourceLoader.Load("res://textures/shallow.jpg") as Texture2D;
		Texture2D forest = ResourceLoader.Load("res://textures/forest.jpg") as Texture2D;
	
		terrainTypeImages = new Dictionary<TerrainType, Texture2D>
		{
			{ TerrainType.PLAINS, plains},
			{ TerrainType.BEACH, beach},
			{ TerrainType.DESERT, desert},
			{ TerrainType.MOUNTAIN, mountain},
			{ TerrainType.ICE, ice},
			{ TerrainType.WATER, ocean},
			{ TerrainType.SHALLOW_WATER, shallow},
			{ TerrainType.FOREST, forest}
		};
	
	}


	// Data hex
	Hex h = null;

	// UI Components
	TextureRect terrainImage;
	Label terrainLabel, foodLabel, productionLabel;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		terrainLabel = GetNode<Label>("TerrainLabel");
		foodLabel = GetNode<Label>("FoodLabel");
		productionLabel = GetNode<Label>("ProductionLabel");
		terrainImage = GetNode<TextureRect>("TerrainImage");
	}

	public void SetHex(Hex h)
	{
		this.h = h;

		terrainImage.Texture = terrainTypeImages[h.terrainType];
		foodLabel.Text = $"Food: {h.food}";
		productionLabel.Text = $"Production: {h.production}";
		terrainLabel.Text = $"Terrain: {terrainTypeStrings[h.terrainType]}";
	}
}
