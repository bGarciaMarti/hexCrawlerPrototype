using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Unit : Node2D
{

	[Signal]
	public delegate void UnitClickedEventHandler(Unit u);

	public delegate void SelectedUnitDestroyedEventHandler();
	public event SelectedUnitDestroyedEventHandler SelectedUnitDestroyed;

	public static Dictionary<Type, PackedScene> unitSceneResources;
	public static Dictionary<Type, Texture2D> uiImages;

	public static void LoadUnitScenes()
	{
		unitSceneResources = new Dictionary<Type, PackedScene> {
			{ typeof(Settler), ResourceLoader.Load<PackedScene>("res://Settler.tscn")},
			{ typeof(Warrior), ResourceLoader.Load<PackedScene>("res://Warrior.tscn")}
		};
	}

	public static void LoadTextures()
	{
		uiImages = new Dictionary<Type, Texture2D>
		{
			{ typeof(Settler), (Texture2D) ResourceLoader.Load("res://textures/settler_image.png")},
			{ typeof(Warrior), (Texture2D) ResourceLoader.Load("res://textures/warrior_image.jpg")}
		};
	}

	public string unitName = "DEFAULT";

	public int productionRequired;

	public Civilization civ;

	public int maxHp;
	public int hp;

	public int maxMovePoints;
	public int movePoints;

	public int attackVal;


	public Vector2I coords = new Vector2I();

	public bool selected = false;

	public Area2D collider;

	public HexTileMap map;


	public HashSet<TerrainType> impassible = new HashSet<TerrainType>
	{
		TerrainType.WATER,
		TerrainType.SHALLOW_WATER,
		TerrainType.ICE,
		TerrainType.MOUNTAIN
	};

	List<Hex> validMovementHexes = new List<Hex>();

	public static Dictionary<Hex, List<Unit>> unitLocations = new Dictionary<Hex, List<Unit>>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		collider = GetNode<Area2D>("Sprite2D/Area2D");

		UIManager manager = GetNode<UIManager>("/root/Game/CanvasLayer/UiManager");
		this.UnitClicked += manager.SetUnitUI;

		manager.EndTurn += this.ProcessTurn;
		this.SelectedUnitDestroyed += manager.HideAllPopups;

		map = GetNode<HexTileMap>("/root/Game/HexTileMap");
		this.UnitClicked += map.DeselectCurrentCell;


		map.RightClickOnMap += Move;

		validMovementHexes = CalculateValidAdjacentMovementHexes();

		if (unitLocations.ContainsKey(map.GetHex(this.coords)))
		{
			unitLocations[map.GetHex(this.coords)].Add(this);
		} else {
			unitLocations[map.GetHex(this.coords)] = new List<Unit>{this};
		}

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void ProcessTurn()
	{
		movePoints = maxMovePoints;
	}

	public void CalculateCombat(Unit attacker, Unit defender)
	{
		defender.hp -= attacker.attackVal;
		attacker.hp -= defender.attackVal/2;

		if (defender.hp <= 0)
		{
			defender.DestroyUnit();
		}

		if (attacker.hp <= 0)
		{
			attacker.DestroyUnit();
		}
	}


	public void MoveToHex(Hex h)
	{
		if (!unitLocations.ContainsKey(h) || (unitLocations.ContainsKey(h) && unitLocations[h].Count == 0))
		{
			unitLocations[map.GetHex(this.coords)].Remove(this);

			Position = map.MapToLocal(h.coordinates);
			coords = h.coordinates;

			if (!unitLocations.ContainsKey(h))
			{
				Unit.unitLocations[h] = new List<Unit>{this};
			} else {
				unitLocations[h].Add(this);
			}

			validMovementHexes = CalculateValidAdjacentMovementHexes();
			movePoints -= 1;

			if (h.isCityCenter && h.ownerCity.civ != this.civ && this is Warrior)
			{
				h.ownerCity.ChangeOwnership(this.civ);
			}

		} else {
			// Combat
			Unit opp = unitLocations[h][0];

			if (opp.civ != this.civ)
			{
				CalculateCombat(this, opp);
			}

		}
	}

	public void Move(Hex h)
	{
		if (selected && movePoints > 0)
		{
			if (validMovementHexes.Contains(h))
			{
				MoveToHex(h);
				EmitSignal(SignalName.UnitClicked, this);
			}	
		}
	}


	public void SetCiv(Civilization civ)
	{
		this.civ = civ;
		GetNode<Sprite2D>("Sprite2D").Modulate = civ.territoryColor;
		this.civ.units.Add(this);
	}

	public void SetSelected()
	{
		if (!selected)
		{
			selected = true;

			Sprite2D sprite = GetNode<Sprite2D>("Sprite2D");
			Color c = new Color(sprite.Modulate);
			c.V = c.V - 0.25f;
			sprite.Modulate = c;

			validMovementHexes = CalculateValidAdjacentMovementHexes();
		}
	}

	public void SetDeselected()
	{
		selected = false;
		
		validMovementHexes.Clear();

		GetNode<Sprite2D>("Sprite2D").Modulate = civ.territoryColor;
	}

	public List<Hex> CalculateValidAdjacentMovementHexes()
	{
		List<Hex> hexes = new List<Hex>();

		hexes.AddRange(map.GetSurroundingHexes(this.coords));
		hexes = hexes.Where(h => !impassible.Contains(h.terrainType)).ToList();

		return hexes;
	}

	public void DestroyUnit()
	{
		map.RightClickOnMap -= Move;

		if (selected)
		{
			SelectedUnitDestroyed?.Invoke();
		}

		this.civ.units.Remove(this);
		unitLocations[map.GetHex(this.coords)].Remove(this);

		this.QueueFree();
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouse && mouse.ButtonMask == MouseButtonMask.Left)
		{
			var spaceState = GetWorld2D().DirectSpaceState;
			var point = new PhysicsPointQueryParameters2D();
			point.CollideWithAreas = true;
			point.Position = GetGlobalMousePosition();
			var result = spaceState.IntersectPoint(point);
			if (result.Count > 0 && (Area2D) result[0]["collider"] == collider)
			{
				EmitSignal(SignalName.UnitClicked, this);
				SetSelected();
				GetViewport().SetInputAsHandled();
			} else {
				SetDeselected();
			}
		}
    }

	public void RandomMove()
	{
		Random r = new Random();
		validMovementHexes = CalculateValidAdjacentMovementHexes();
		Hex h = validMovementHexes.ElementAt(r.Next(validMovementHexes.Count));

		MoveToHex(h);
	}

}
