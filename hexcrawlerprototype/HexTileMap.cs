using Godot;
using System;
using System.Collections.Generic; // DICT

public enum TerrainType { PLAINS, WATER, MIST, MOUNTAIN, HILLS, SETTLEMENT, FOREST, FARMLAND }

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
  public int width = 45;
  [Export]
  public int height = 50;

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

// interactivity
// if input has not already been consumed by another element

Vector2I currentSelectedCell = new Vector2I(-1, -1);
public override void _UnhandledInput(InputEvent @event)
{	if (@event is InputEventMouseButton mouse) 
	{
	Vector2I mapCoords = baseLayer.LocalToMap(ToLocal(GetGlobalMousePosition()));
	if (mapCoords.X >= 0 && mapCoords.X < width && mapCoords.Y >= 0 && mapCoords.Y < height) {// keep click in bounds of the map
		if (mouse.ButtonMask == MouseButtonMask.Left) {
				GD.Print(mapData[mapCoords]);
				overlayLayer.SetCell(mapCoords, 0, new Vector2I(0, 1));
			}
		}
	}
}

public override void _Process(double delta) { }

  public void GenerateTerrain()
  {
	for (int x = 0; x < width; x++) {
	  for (int y = 0; y < height; y++) {
		Hex h = new Hex(new Vector2I(x, y));
		baseLayer.SetCell(new Vector2I(x, y), 0, new Vector2I(0, 3));
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
		
		// int _tileCellSourceID = baseLayer.GetCellSourceId(new Vector2I(x, y));
		// if (_tileCellSourceID != -1) {
			// var tile = baseLayer.GetCellTileData(_tileSetAtlasPos);
			// if (IsInstanceValid(tile))
				// GD.Print(tile.GetCustomData("TerrainType"));
				
					// h.terrainType = TerrainType.SETTLEMENT;
			// }
		// if atlas_coords == 
						//	terrainTextures = new Dictionary<TerrainType, Vector2I>
						//	{
						//	{ TerrainType.SETTLEMENT, new Vector2I(0, 0) },
		

		// Set tile borders
		borderLayer.SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));
	  } }
	} // end of GenerateTerrain
  
public Vector2 MapToLocal(Vector2I coords) {
	return baseLayer.MapToLocal(coords);
	}
}  
