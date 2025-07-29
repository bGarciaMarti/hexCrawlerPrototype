using Godot;
using System;

public partial class HexTileMap : Node2D
{
  [Export]
  public int width = 45;
  [Export]
  public int height = 50;

  // Map data
  TileMapLayer baseLayer, borderLayer, overlayLayer;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
	baseLayer = GetNode<TileMapLayer>("BaseLayer");
	borderLayer = GetNode<TileMapLayer>("HexBordersLayer");
	overlayLayer = GetNode<TileMapLayer>("SelectionOverlayLayer");

	GenerateTerrain();
  }

public override void _Process(double delta) {
	
}

  public void GenerateTerrain()
  {
	for (int x = 0; x < width; x++) {
	  for (int y = 0; y < height; y++) {
		baseLayer.SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));

		// Set tile borders
		borderLayer.SetCell(new Vector2I(x, y), 0, new Vector2I(0, 0));
	  } }
	} // end of GenerateTerrain
  
public Vector2 MapToLocal(Vector2I coords) {
	return baseLayer.MapToLocal(coords);
	}
}  
