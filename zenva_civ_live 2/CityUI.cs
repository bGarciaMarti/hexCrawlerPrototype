using Godot;
using System;

public partial class CityUI : Panel
{

	Label cityName, population, food, production;

	// City data
	City city;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cityName = GetNode<Label>("CityName");
		population = GetNode<Label>("Population");
		food = GetNode<Label>("Food");
		production = GetNode<Label>("Production");
	}

	public void SetCityUI(City city)
	{
		this.city = city;

		Refresh();

		ConnectUnitBuildSignals(this.city);
	}

	public void ConnectUnitBuildSignals(City city)
	{
		VBoxContainer buttons = GetNode<VBoxContainer>("UnitBuildButtons/VBoxContainer");

		UnitBuildButton settlerButton = buttons.GetNode<UnitBuildButton>("SettlerButton");
		settlerButton.u = new Settler();

		UnitBuildButton warriorButton = buttons.GetNode<UnitBuildButton>("WarriorButton");
		warriorButton.u = new Warrior();

		settlerButton.OnPressed += city.AddUnitToBuildQueue;
		settlerButton.OnPressed += this.Refresh;
		warriorButton.OnPressed += city.AddUnitToBuildQueue;
		warriorButton.OnPressed += this.Refresh;
	}

	public void PopulateUnitQueueUI(City city)
	{
		VBoxContainer queue = GetNode<VBoxContainer>("QueueContainer/VBoxContainer");

		foreach (Node n in queue.GetChildren())
		{
			queue.RemoveChild(n);
			n.QueueFree();
		}

		for (int i = 0; i < city.unitBuildQueue.Count; i++)
		{
			Unit u = city.unitBuildQueue[i];

			if (i == 0) // Unit is currently being built
			{
				queue.AddChild(new Label() {
					Text = $"{u.unitName} {city.unitBuildTracker}/{u.productionRequired}"
				});
			} else {
				queue.AddChild(new Label() {
					Text = $"{u.unitName} 0/{u.productionRequired}"
				});
			}

		}
	}

	public void Refresh()
	{
		cityName.Text = this.city.name;
		population.Text = "Population: " + this.city.population;
		food.Text = "Food: " + this.city.totalFood;
		production.Text = "Production: " + this.city.totalProduction;

		PopulateUnitQueueUI(this.city);

	}

	public void Refresh(Unit u)
	{
		Refresh();
	}
}
