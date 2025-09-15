using Godot;
using System;
using System.Collections.Generic; // DICT
using System.Linq; // Aggregate

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
} // end of public class Hex

public partial class HexTileMap : Node2D
{
  [Export]
  public int width = 46;
  [Export]
  public int height = 51;
  [Export]

  // Map data
  TileMapLayer baseLayer, borderLayer, overlayLayer;
  Dictionary<Vector2I, Hex> mapData;
  Dictionary<TerrainType, Vector2I> terrainTextures;

  public static readonly Vector2I[] oddr_direction_differences_even = new [] {
		new Vector2I(1, 0), //east
		new Vector2I(0, -1), // north-east
		new Vector2I(-1, -1), // north-west
		new Vector2I(-1, 0), // west
		new Vector2I(-1, 1), // south-west
		new Vector2I(0, 1) }; // south-east
  public static readonly Vector2I[] oddr_direction_differences_odd = new [] {
		new Vector2I(1, 0), //east
		new Vector2I(1, -1), // north-east
		new Vector2I(0, -1), // north-west
		new Vector2I(-1, 0), // west
		new Vector2I(0, +1), // south-west
		new Vector2I(1, 1) }; // south-east
  public static readonly Vector2I[] oddr_direction_differences_diagonals = new [] {
		new Vector2I(0, -2), //north
		new Vector2I(1, -1), // north-east
		new Vector2I(-2, -1), // north-west
		new Vector2I(0, +2), // south
		new Vector2I(-2, 1), // south-west
		new Vector2I(1,1) }; // south-east
	
  public static IEnumerable<Hex> Neighbors(Hex CenterHex, Dictionary<Vector2I, Hex> sillyMapData) {
	Vector2I[] DIRS = parity(Convert.ToInt32(CenterHex.coordinates.X))? oddr_direction_differences_odd: oddr_direction_differences_even;
	foreach (var dir in DIRS) {
		Vector2I neighborCoords = new Vector2I(CenterHex.coordinates.X + dir.X, CenterHex.coordinates.Y + dir.Y);
		if (sillyMapData.ContainsKey(neighborCoords)) {
			yield return sillyMapData[neighborCoords];
		} else {
			yield break;
		}
	}	
}
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
	
  } // end of public partial class HexTileMap : Node2D


List<Hex> journeyData = new List<Hex>(); // create an empty list to keep the hexes in the journey
bool planning = true;
// interactivity
// if input has not already been consumed by another element
public override void _UnhandledInput(InputEvent @event)
{	if ((journeyData.Count < 2) & (planning == true))
	{ 
		if (@event is InputEventMouseButton mouse) 
		{	
			Vector2I mapCoords = baseLayer.LocalToMap(ToLocal(GetGlobalMousePosition()));
			if (mapCoords.X >= 0 && mapCoords.X < width && mapCoords.Y >= 0 && mapCoords.Y < height) {// keep click in bounds of the map
				if (mouse.ButtonMask == MouseButtonMask.Left) {
					GD.Print(mapData[mapCoords]);
					overlayLayer.SetCell(mapCoords, 0, new Vector2I(0, 1));
					journeyData.Add(mapData[mapCoords]); //
				}
			}
		}
	}
	else if (planning == true) {
		
		var astar = new AStarSearch(journeyData[0].coordinates, journeyData[1].coordinates, mapData);	
		Dictionary<Vector2I, Hex>.ValueCollection values = astar.cameFrom.Values;  
		foreach (Hex h in values)
		{  
			journeyData.Add(h);
			planning = false;
		}
		foreach (Hex h in journeyData)
		{   overlayLayer.SetCell(h.coordinates, 0, new Vector2I(0, 1));
			GD.Print(h);
		}
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
 
// Function to find the parity
static bool parity(int x)
{	// Rightmost bit of y holds the parity value
	int y = x ^ (x >> 1);
		y = y ^ (y >> 2);
		y = y ^ (y >> 4);
		y = y ^ (y >> 8);
		y = y ^ (y >> 16);
		
	// if (y&1) is 1 then parity is odd else even
	if ((y & 1) > 0)
		return true;
	return false;
}

public class AStarSearch
{	// Dictionary<Vector2I, Hex> mapData;
	public Dictionary<Vector2I, Hex> cameFrom = new Dictionary<Vector2I, Hex>();
	public Dictionary<Vector2I, double> costSoFar = new Dictionary<Vector2I, double>();

	static public double Heuristic(Vector2I a, Vector2I b) {
		return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
	}

	public AStarSearch(Vector2I start, Vector2I goal, Dictionary<Vector2I, Hex> thisMapsData)
	{
		var frontier = new PriorityQueue<Hex, double>();
		Hex h = new Hex(new Vector2I(start.X, start.Y));
		frontier.Enqueue(h, 0);
		
		cameFrom[start] = h;
		costSoFar[start] = 0;
		double costOfAnyHexes = 1.0;
		
		while (frontier.Count > 0)
		{	
			var current = frontier.Dequeue();
			// GD.Print("current == ",current);
			
			if (current.coordinates.Equals(goal))
			{
				GD.Print("WE HAVE A ROUTE TO FOLLOW");
				break;
			}
			
			IEnumerable<Hex> neighbors = Neighbors(current, thisMapsData); //, width, height);
			foreach (var neighbor in neighbors)
			{   
				double newCost = costSoFar[current.coordinates] + costOfAnyHexes;
				Hex quickestNeighbor = neighbors.Aggregate((quickestNeighbor, neighbor)
				  => (Heuristic(quickestNeighbor.coordinates, goal)) < (Heuristic(neighbor.coordinates, goal)) 
				  ? quickestNeighbor : neighbor);
				if (!costSoFar.ContainsKey(quickestNeighbor.coordinates) || newCost < costSoFar[quickestNeighbor.coordinates])
				{
					costSoFar[quickestNeighbor.coordinates] = newCost;
					double priority = costOfAnyHexes + Heuristic(quickestNeighbor.coordinates, goal);
					frontier.Enqueue(quickestNeighbor, priority);
					cameFrom[quickestNeighbor.coordinates] = current;
				}
			}
		}
	}
}

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

public Vector2 MapToLocal(Vector2I coords) {
	return baseLayer.MapToLocal(coords);
	}
}  
