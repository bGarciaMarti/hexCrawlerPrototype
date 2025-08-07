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
  public int width = 51; //50
  [Export]
  public int height = 46; //45

  // Map data
  TileMapLayer baseLayer, borderLayer, overlayLayer;

  Dictionary<Vector2I, Hex> mapData;
  Dictionary<TerrainType, Vector2I> terrainTextures;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
	var myGDScript = GD.Load<GDScript>("res://hex_tile_map.gd");
  	var myGDScriptNode = (GodotObject)myGDScript.New(); // This is a GodotObject.

	baseLayer = GetNode<TileMapLayer>("BaseLayer");
	borderLayer = GetNode<TileMapLayer>("HexBordersLayer");
	overlayLayer = GetNode<TileMapLayer>("SelectionOverlayLayer");

// Initialize map data
	mapData = new Dictionary<Vector2I, Hex>();
	terrainTextures = new Dictionary<TerrainType, Vector2I>
	{
	  { TerrainType.SETTLEMENT, new Vector2I(0, 0) },
	  { TerrainType.MOUNTAIN, new Vector2I(1, 0) },
	  { TerrainType.FARMLAND, new Vector2I(0, 1)},
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
	{	if (@event is InputEventMouseButton mouse) {
			  Vector2I mapCoords = baseLayer.LocalToMap(ToLocal(GetGlobalMousePosition()));
		if (mapCoords.X >= 0 && mapCoords.X < width && mapCoords.Y >= 0 && mapCoords.Y < height) {// keep click in bounds of the map
			GD.Print(mapData[mapCoords]);
		}
		
	}
}

public override void _Process(double delta) {
	
}

  public void GenerateTerrain()
  {
	for (int x = 0; x < width; x++) {
	  for (int y = 0; y < height; y++) {
		// baseLayer.SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));
		Hex h = new Hex(new Vector2I(x,y));
		// if (the Hex(x,y) custom_data_layer TerrainType == terrainTextures TerrainType.SETTLEMENT)
		// h.terrainType = terrainTextures TerrainTypeddds
		// TileSetAtlasSource _tileSource = (TileSetAtlasSource)TileSet.GetSource(GetCellSourceId(0,new Vector2I(x, y)));
		// var _tileData = _tileSource.GetTileData(GetCellAtlasCoords(0, new Vector2I(x,y )));
		// https://docs.godotengine.org/en/stable/classes/class_tilesetscenescollectionsource.html#class-tilesetscenescollectionsource

//Get Access to Custom Tile Data
Vector2I _tileMapPos = new Vector2I(x,y);
Vector2I _tileSetAtlasPos = get_cell_atlas_coords(0, (Vector2I)_tileMapPos);// Vector2I _tileSetAtlasPos = GetCellAtlasCoords(0, (Vector2I)_tileMapPos);
int _tileCellSourceID = GetCellSourceId(0, (Vector2I)_tileMapPos);
if (_tileCellSourceID != -1)
{
	TileSetAtlasSource _tileSource = (TileSetAtlasSource)TileSet.GetSource(_tileCellSourceID);
	var _tileData = _tileSource.GetTileData(_tileSetAtlasPos, 0);
}  

		mapData[new Vector2I(x,y)] = h;
		// Set tile borders
		borderLayer.SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));
	  } }
	} // end of GenerateTerrain
  
public Vector2 MapToLocal(Vector2I coords) {
	return baseLayer.MapToLocal(coords);
	}
}  
