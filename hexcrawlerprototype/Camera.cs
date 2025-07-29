using Godot;
using System;

public partial class Camera : Camera2D
{
	[Export]
	int velocity =15;
	
	[Export]
	float zoom_speed = 0.05f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		//Map controls
		if (Input.IsActionPressed("map_right")){
			this.Position += new Vector2(velocity, 0);
		}
		if (Input.IsActionPressed("map_left")){
			this.Position += new Vector2(-velocity, 0);
		}
		if (Input.IsActionPressed("map_up")){
			this.Position += new Vector2(0, -velocity);
		}
		if (Input.IsActionPressed("map_down")){
			this.Position += new Vector2(0, velocity);
		}
	}
}
