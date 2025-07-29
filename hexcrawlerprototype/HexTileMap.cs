using Godot;
using System;
using System.Collections.Generic;

//public enum TerrainType{ SETTLEMENT, MOUNTAIN, PLAINS, FOREST, WATER, FARMLAND, MIST, HILLS}

// represent a hex in game
//public class Hex {
	//public readonly Vector2I coordinates;
	//public TerrainType terrainType;
	//
	//public Hex(Vector2I coords) {
		//this.coordinates = coords;
	//}
//}

public partial class HexTileMap : Node2D
{
	[Export]
	public int width = 45;
	
	[Export]
	public int height = 50;
	
	TileMapLayer baseLayer, borderLayer, overlayLayer;
	
	//Dictionary<Vector2I, Hex> mapData;
	//Dictionary<TerrainType, Vector2I> terrainTextures;

	// Called when the node enters the scene tree for first time.
	public override void _Ready(){
		baseLayer = GetNode<TileMapLayer>("BaseLayer");
		borderLayer = GetNode<TileMapLayer>("HexBordersLayer");
		overlayLayer = GetNode<TileMapLayer>("SelectionOverlayLayer");
		
		//mapData = new Dictionary< Vector2I, Hex>(); // init map data
		//terrainTextures = new Dictionary<TerrainType, Vector2I> {
			//{TerrainType.SETTLEMENT, new Vector2I(0,0)	},
			//{TerrainType.MOUNTAIN, new Vector2I(1,0)	},
			//{TerrainType.PLAINS, new Vector2I(2,0)	},
			//{TerrainType.FOREST, new Vector2I(3,0)	},
//
			//{TerrainType.FARMLAND, new Vector2I(0,1)	},
			//{TerrainType.WATER, new Vector2I(1,1)	},
			//{TerrainType.MIST, new Vector2I(2,1)	},
			//{TerrainType.HILLS, new Vector2I(3,1)	}
		//};
		
		GenerateTerrain();
	}
	
	// Called every frame. 'delta' is the elapsed time since the previous frame
	public override void _Process(double delta){
	}
	
	public void GenerateTerrain(){
		
		for (int x = 0; x < width; x++){
			for (int y = 0; y < height; y ++){
				baseLayer.SetCell(new Vector2I(x,y), 0, new Vector2I(0,0));
				
				// set tile borders
				borderLayer.SetCell(new Vector2I(x,y), 0, new Vector2I(0,0));
			}
		}
	}
		// convert tile location to 2D word coordinates relative to the TileMap object
	public Vector2 MapToLocal(Vector2I coords) {
		return baseLayer.MapToLocal(coords);
	}
}
