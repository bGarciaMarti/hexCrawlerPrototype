using Godot;
using System;
// https://www.youtube.com/watch?v=djM8EZG3Dz0

public partial class HexTileMap : Node2D
{
	[Export]
	public int width = 45;
	
	[Export]
	public int height = 50;
	
	TileMapLayer baseLayer, borderLayer, overlayLayer;
	
	// Called when the node enters the scene tree for the first time
	public override void _Ready(){
		baseLayer = GetNode<TileMapLayer>("BaseLayer");
		borderLayer = GetNode<TileMapLayer>("HexBordersLayer");
		overlayLayer = GetNode<TileMapLayer>("SelectionOverlayLayer");
		
		GenerateTerrain();
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame
	public override void _Process(double delta) {
		
	}
	
	public void GenerateTerrain() {
		for (int x=0; x < width; x++){
			for (int y = 0; y < height; y++) {
				baseLayer.SetCell(new Vector2I(x,y), 0, new Vector2I(0,0));
			}
		}
	}
	
}
