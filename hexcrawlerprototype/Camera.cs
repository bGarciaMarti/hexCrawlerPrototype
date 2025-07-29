using Godot;
using System;

public partial class Camera : Camera2D
{
	[Export]
	int velocity =15;
	
	[Export]
	float zoom_speed = 0.05f;
	
	// Mouse states
	[Export]
	bool mouseWheelScrollingUp = false;
	bool mouseWheelScrollingDown = false;
	
	// Map boundries
	float leftBound, rightBound, topBound, bottomBound;
	
	// map reference
	HexTileMap map;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		map = GetNode<HexTileMap>("../HexTileMap");
		
		leftBound = ToGlobal(map.MapToLocal(new Vector2I(0,0))).X + 100;
		rightBound = ToGlobal(map.MapToLocal(new Vector2I(map.width,0))).X - 100;
		topBound = ToGlobal(map.MapToLocal(new Vector2I(0,0))).Y + 50;
		bottomBound = ToGlobal(map.MapToLocal(new Vector2I(0, map.height))).Y - 50;
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		//Map controls
		if (Input.IsActionPressed("map_right") && this.Position.X < rightBound){
			this.Position += new Vector2(velocity, 0);
		}
		if (Input.IsActionPressed("map_left") && this.Position.X > leftBound){
			this.Position += new Vector2(-velocity, 0);
		}
		if (Input.IsActionPressed("map_up") && this.Position.Y < topBound){
			this.Position += new Vector2(0, -velocity);
		}
		if (Input.IsActionPressed("map_down") && this.Position.Y > bottomBound){
			this.Position += new Vector2(0, velocity);
		}
		
		// Zoom controls
		if (Input.IsActionPressed("map_zoom_in") || mouseWheelScrollingUp){
			if (this.Zoom < new Vector2(3f,3f)){ // don't zoom in and out too far
				this.Zoom += new Vector2(zoom_speed, zoom_speed);
		} }
		if (Input.IsActionPressed("map_zoom_out") || mouseWheelScrollingDown){
			if (this.Zoom > new Vector2(0.01f,0.01f)){
				this.Zoom -= new Vector2(zoom_speed, zoom_speed);
		}}
		
		// check is mouse is zooming in or out
		if (Input.IsActionJustReleased("mouse_zoom_in"))
			mouseWheelScrollingUp = true;
			
		if (!Input.IsActionJustReleased("mouse_zoom_in"))
			mouseWheelScrollingUp = false;
		
		if (Input.IsActionJustReleased("mouse_zoom_out"))
			mouseWheelScrollingDown = true;
			
		if (!Input.IsActionJustReleased("mouse_zoom_out"))
			mouseWheelScrollingDown = false;
	}
	 
}
