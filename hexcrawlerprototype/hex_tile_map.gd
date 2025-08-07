extends Node2D

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.
# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass

# Vector2i get_cell_atlas_coords(layer: int, coords: Vector2i, use_proxies: bool = false) const
# Returns the tile atlas coordinates ID of the cell on layer layer at coordinates coords. Returns Vector2i(-1, -1) if the cell does not exist.
# If use_proxies is false, ignores the TileSet's tile proxies, returning the raw atlas coordinate identifier. See TileSet.map_tile_proxy.
# If layer is negative, the layers are accessed from the last one.
func gd_get_cell_atlas_coords (layer: int, coords: Vector2i):
	var cell_atlas_coords: Vector2i
	cell_atlas_coords = get_cell_atlas_coords(layer,coords,false)
	return cell_atlas_coords
