using Godot;
using System;
using System.Collections.Generic; // DICT

public enum TerrainType { 
	PLAINS = 0b_0000_0000,  // 0
	WATER = 0b_0000_0001,  // 1
	MIST = 0b_0000_0010,  // 2
	MOUNTAIN = 0b_0000_0100,  // 4
	HILLS = 0b_0000_1000,  // 8
	SETTLEMENT = 0b_0001_0000,  // 16
	FOREST = 0b_0010_0000,  // 32
	FARMLAND  = 0b_0100_0000  // 64
}

public class Hex
{
  public readonly Vector2I coordinates;
  public TerrainType terrainType;
  public Hex(Vector2I coords)
  {
	this.coordinates = coords;
  }
  
  public bool hasVisited = false;

// interactivity
   public override string ToString()
   {
	   return $"Coordinates: ({this.coordinates.X}, {this.coordinates.Y}) Terrain type: {this.terrainType} Has been visited: {this.hasVisited})";
   }
}

public partial class HexTileMap : Node2D
{
  [Export]
  public int width = 46;
  [Export]
  public int height = 51;

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
	  { TerrainType.SETTLEMENT, new Vector2I(0, 0) },
	  { TerrainType.MOUNTAIN, new Vector2I(0, 1) },
	  { TerrainType.FARMLAND, new Vector2I(1, 0)},
	  { TerrainType.WATER, new Vector2I(1, 1)},
	  { TerrainType.MIST, new Vector2I(1, 2)},
	  { TerrainType.PLAINS, new Vector2I(0, 2)},
	  { TerrainType.HILLS, new Vector2I(1, 3)},
	  { TerrainType.FOREST, new Vector2I(0, 3)},
	};
	
	GenerateTerrain();
  }

List<Hex> journeyData = new List<Hex>(); // create an empty list
// interactivity
// if input has not already been consumed by another element
public override void _UnhandledInput(InputEvent @event)
{	if (journeyData.Count < 2)
	{ 
		if (@event is InputEventMouseButton mouse) 
		{
			Vector2I mapCoords = baseLayer.LocalToMap(ToLocal(GetGlobalMousePosition()));
			if (mapCoords.X >= 0 && mapCoords.X < width && mapCoords.Y >= 0 && mapCoords.Y < height) {// keep click in bounds of the map
				if (mouse.ButtonMask == MouseButtonMask.Left) {
					GD.Print(mapData[mapCoords]);
					overlayLayer.SetCell(mapCoords, 0, new Vector2I(0, 1));
					// overlayLayer.SetCell(currentLocationCell, -1); // unhighlight the first selected hex
					// journeyData.Add(the hex where mapCoords == coordinates of Hex); //
				}
			}
		}
	}
	else { // the two ends of a journey have been saved
		axial_linedraw(journeyData);
		GD.Print(journeyData);
	}
}

public override void _Process(double delta) { }

public void GenerateTerrain()
{
	for (int x = 0; x < width; x++) {
	  for (int y = 0; y < height; y++) {
		Hex h = new Hex(new Vector2I(x, y));
		// baseLayer.SetCell(new Vector2I(x, y), 0, new Vector2I(0, 3));
		mapData[new Vector2I(x, y)] = h;
		
		Vector2I _tileSetAtlasPos = baseLayer.GetCellAtlasCoords(new Vector2I(x, y));
		if (_tileSetAtlasPos == terrainTextures[TerrainType.SETTLEMENT]) {
			h.terrainType = TerrainType.SETTLEMENT;
		} else if (_tileSetAtlasPos == terrainTextures[TerrainType.MOUNTAIN]) {
			h.terrainType = TerrainType.MOUNTAIN;
		} else if (_tileSetAtlasPos == terrainTextures[TerrainType.PLAINS]) {
			h.terrainType = TerrainType.PLAINS;
		} else if (_tileSetAtlasPos == terrainTextures[TerrainType.FOREST]) {
			h.terrainType = TerrainType.FOREST;
		} else if (_tileSetAtlasPos == terrainTextures[TerrainType.FARMLAND]) {
			h.terrainType = TerrainType.FARMLAND;
		} else if (_tileSetAtlasPos == terrainTextures[TerrainType.WATER]) {
			h.terrainType = TerrainType.WATER;
		} else if (_tileSetAtlasPos == terrainTextures[TerrainType.MIST]) {
			h.terrainType = TerrainType.MIST;
		} else if (_tileSetAtlasPos == terrainTextures[TerrainType.HILLS]) {
			h.terrainType = TerrainType.HILLS;
		}

		// Set tile borders
		borderLayer.SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));
	  } 
	}
} // end of GenerateTerrain
  
public int axial_distance(Vector2I a, Vector2I b)
{	return (Math.Abs(a.X - b.X) 
		  + Math.Abs(a.X + a.Y - b.X - b.Y)
		  + Math.Abs(a.Y - b.Y)) / 2;
}

public Vector2 axial_round (double x, double y) {
  int xgrid = Convert.ToInt32(Math.Round(x));
  int ygrid = Convert.ToInt32(Math.Round(y));
  x -= xgrid; y -= ygrid; // remainder

  int dx = Convert.ToInt32(xgrid + Math.Round(x + 0.5*y));
  int dy = Convert.ToInt32(ygrid + Math.Round(y + 0.5*x));

	if (Math.Abs(x) >= Math.Abs(y)) {
		return new Vector2I(dx, ygrid);
		}
	else {
		return new Vector2I (xgrid, dy);
		}
}

public float lerp(double a, double b, double t) {
	return (float) (a * (1-t) + b * t);
	/* better for floating point precision than
	   a + (b - a) * t, which is what I usually write */
}

function axial_lerp(a, b, t) { // for hexes
	return Hex(Vector2I coords) {
		this.coordinates =
			 (lerp(a.X, b.X, t),
			lerp(a.Y, b.Y, t) );
			}
}

public void axial_linedraw(List<Hex> journey) {
	Hex current = (journey[1]);
	var N = axial_distance(journey[0],journey[1]);
	while (current != journey[0]){
// 		journey.Add(current)
		//current = came_from[current]
	}
	// Vector2I mapCoords = new Vector2I(-1, -1);
	// Hex h = new Hex(new Vector2I(x, y));
	for (int i = 0; i < N; i++) {
		var t = 1.0/N * i;
		journey.Add(axial_round(axial_lerp(journeyData[0], journeyData[1], t)));
	}
}

public Vector2 MapToLocal(Vector2I coords) {
	return baseLayer.MapToLocal(coords);
	}
}  
