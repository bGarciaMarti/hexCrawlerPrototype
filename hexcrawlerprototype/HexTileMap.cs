using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public enum TerrainType { PLAINS, WATER, DESERT, MOUNTAIN, ICE, SHALLOW_WATER, FOREST, BEACH }

public class Hex
{
  public readonly Vector2I coordinates;

  public TerrainType terrainType;

  public Hex(Vector2I coords)
  {
	this.coordinates = coords;
  }

	// interactivity
   public override string ToString()
   {
	   return $"Coordinates: ({this.coordinates.X}, {this.coordinates.Y}. Terrain type: {this.terrainType})";
   }

}

public partial class HexTileMap : Node2D
{

  [Export]
  public int width = 100;
  [Export]
  public int height = 60;

  // Map data
  TileMapLayer baseLayer, borderLayer, overlayLayer;

  Dictionary<Vector2I, Hex> mapData;
  Dictionary<TerrainType, Vector2I> terrainTextures;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
	baseLayer = GetNode<TileMapLayer>("BaseLayer");
	borderLayer = GetNode<TileMapLayer>("HexBordersLayer");
	overlayLayer = GetNode<TileMapLayer>("SelectionOverlayLayer");

	// Initialize map data
	mapData = new Dictionary<Vector2I, Hex>();
	terrainTextures = new Dictionary<TerrainType, Vector2I>
	{
	  { TerrainType.PLAINS, new Vector2I(0, 0) },
	  { TerrainType.WATER, new Vector2I(1, 0) },
	  { TerrainType.DESERT, new Vector2I(0, 1)},
	  { TerrainType.MOUNTAIN, new Vector2I(1, 1)},
	  { TerrainType.SHALLOW_WATER, new Vector2I(1, 2)},
	  { TerrainType.BEACH, new Vector2I(0, 2)},
	  { TerrainType.FOREST, new Vector2I(1, 3)},
	  { TerrainType.ICE, new Vector2I(0, 3)},
	};

	GenerateTerrain();

  }

// interactivity
// if input has not already been consumed by another element

Vector2I currentSelectedCell = new Vector2I(-1, -1);
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouse) {
	  Vector2I mapCoords = baseLayer.LocalToMap(ToLocal(GetGlobalMousePosition()));
	  if (mapCoords.X >= 0 && mapCoords.X < width && mapCoords.Y >= 0 && mapCoords.Y < height) // keep click in bounds of the map
	  {
		Hex h = mapData[mapCoords];
		if (mouse.ButtonMask == MouseButtonMask.Left)
		{ GD.Print(mapData[mapCoords]);
			 //SendHexData?.Invoke(h);
		  if (mapCoords != currentSelectedCell) overlayLayer.SetCell(currentSelectedCell, -1);
		  overlayLayer.SetCell(mapCoords, 0, new Vector2I(0, 1));
		  currentSelectedCell = mapCoords;
		}
	  } else {
		overlayLayer.SetCell(currentSelectedCell, -1);
		// EmitSignal(SignalName.ClickOffMap);
	  }
	
	}
	}
  public void GenerateTerrain()
  {
	float[,] noiseMap = new float[width, height];
	float[,] forestMap = new float[width, height];
	float[,] desertMap = new float[width, height];
	float[,] mountainMap = new float[width, height];

	Random r = new Random();
	int seed = r.Next(100000);

	// BASE TERRAIN (Water, Beach, Plains)
	FastNoiseLite noise = new FastNoiseLite();

	noise.Seed = seed;
	noise.Frequency = 0.008f;
	noise.FractalType = FastNoiseLite.FractalTypeEnum.Fbm;
	noise.FractalOctaves = 4;
	noise.FractalLacunarity = 2.25f;

	float noiseMax = 0f;


	// Forest
	FastNoiseLite forestNoise = new FastNoiseLite();

	forestNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Cellular;
	forestNoise.Seed = seed;
	forestNoise.Frequency = 0.04f;
	forestNoise.FractalType = FastNoiseLite.FractalTypeEnum.Fbm;
	forestNoise.FractalLacunarity = 2f;

	float forestNoiseMax = 0f;


	// Desert
	FastNoiseLite desertNoise = new FastNoiseLite();

	desertNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.SimplexSmooth;
	desertNoise.Seed = seed;
	desertNoise.Frequency = 0.015f;
	desertNoise.FractalType = FastNoiseLite.FractalTypeEnum.Fbm;
	desertNoise.FractalLacunarity = 2f;

	float desertNoiseMax = 0f;

	// Mountain
	FastNoiseLite mountainNoise = new FastNoiseLite();

	mountainNoise.NoiseType = FastNoiseLite.NoiseTypeEnum.Simplex;
	mountainNoise.Seed = seed;
	mountainNoise.Frequency = 0.02f;
	mountainNoise.FractalType = FastNoiseLite.FractalTypeEnum.Ridged;
	mountainNoise.FractalLacunarity = 2f;

	float mountainNoiseMax = 0f;



	// Generating noise values
	for (int x = 0; x < width; x++)
	{
	  for (int y = 0; y < height; y++)
	  {
		// Base terrain
		noiseMap[x, y] = Math.Abs(noise.GetNoise2D(x, y));
		if (noiseMap[x, y] > noiseMax) noiseMax = noiseMap[x ,y];

		// Desert
		desertMap[x, y] = Math.Abs(desertNoise.GetNoise2D(x, y));
		if (desertMap[x, y] > desertNoiseMax) desertNoiseMax = desertMap[x ,y];

		// Forest
		forestMap[x, y] = Math.Abs(forestNoise.GetNoise2D(x, y));
		if (forestMap[x, y] > forestNoiseMax) forestNoiseMax = forestMap[x ,y];

		// Mountain
		mountainMap[x, y] = Math.Abs(mountainNoise.GetNoise2D(x, y));
		if (mountainMap[x, y] > mountainNoiseMax) mountainNoiseMax = mountainMap[x ,y];

	  }
	}


	List<(float Min, float Max, TerrainType Type)> terrainGenValues = new List<(float Min, float Max, TerrainType Type)>
	{
	  (0, noiseMax/10 * 2.5f, TerrainType.WATER),
	  (noiseMax/10 * 2.5f, noiseMax/10 * 4, TerrainType.SHALLOW_WATER),
	  (noiseMax/10 * 4, noiseMax/10 * 4.5f, TerrainType.BEACH),
	  (noiseMax/10 * 4.5f, noiseMax + 0.05f, TerrainType.PLAINS)
	};

	// Forest gen values
	Vector2 forestGenValues = new Vector2(forestNoiseMax/10 * 7, forestNoiseMax + 0.05f);
	// Desert gen values
	Vector2 desertGenValues = new Vector2(desertNoiseMax/10 * 6, desertNoiseMax + 0.05f);
	// Mountain gen values
	Vector2 mountainGenValues = new Vector2(mountainNoiseMax/10 * 5.5f, mountainNoiseMax + 0.05f);


	for (int x = 0; x < width; x++)
	{
	  for (int y = 0; y < height; y++)
	  {
		Hex h = new Hex(new Vector2I(x, y));
		float noiseValue = noiseMap[x, y];

		h.terrainType = terrainGenValues.First(range => noiseValue >= range.Min
									&& noiseValue < range.Max).Type;
		mapData[new Vector2I(x, y)] = h;

		// Desert
		if (desertMap[x, y] >= desertGenValues[0] &&
		  desertMap[x, y] <= desertGenValues[1] &&
		  h.terrainType == TerrainType.PLAINS)
		{
		  h.terrainType = TerrainType.DESERT;
		}

		// Forest
		if (forestMap[x, y] >= forestGenValues[0] &&
		  forestMap[x, y] <= forestGenValues[1] &&
		  h.terrainType == TerrainType.PLAINS)
		{
		  h.terrainType = TerrainType.FOREST;
		}

		// Mountain
		if (mountainMap[x, y] >= mountainGenValues[0] &&
		  mountainMap[x, y] <= mountainGenValues[1] &&
		  h.terrainType == TerrainType.PLAINS)
		{
		  h.terrainType = TerrainType.MOUNTAIN;
		}


		baseLayer.SetCell(new Vector2I(x, y), 0, terrainTextures[h.terrainType]);

		// Set tile borders
		borderLayer.SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));
	  }
	}

	// Ice cap gen
	int maxIce = 5;
	for (int x = 0; x < width; x++)
	{
	  // North pole
	  for (int y = 0; y < r.Next(maxIce) + 1; y++)
	  {
		Hex h = mapData[new Vector2I(x, y)];
		h.terrainType = TerrainType.ICE;
		baseLayer.SetCell(new Vector2I(x, y), 0, terrainTextures[h.terrainType]);
	  }

	  // South pole
	  for (int y = height - 1; y > height - 1 - r.Next(maxIce) - 1; y--)
	  {
		Hex h = mapData[new Vector2I(x, y)];
		h.terrainType = TerrainType.ICE;
		baseLayer.SetCell(new Vector2I(x, y), 0, terrainTextures[h.terrainType]);
	  }


	}



  }

  public Vector2 MapToLocal(Vector2I coords)
  {
	return baseLayer.MapToLocal(coords);
  }  

}
