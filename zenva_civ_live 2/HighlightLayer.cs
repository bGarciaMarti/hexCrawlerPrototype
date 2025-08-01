using Godot;
using System;
using System.Collections.Generic;

public partial class HighlightLayer : TileMapLayer
{

	int w;
	int h;

	List<Hex> currentlyHighlighted = new();

	City current;

	public void SetupHighlightLayer(int w, int h)
	{
		this.w = w;
		this.h = h;

		for (int x = 0; x < w; x++)
		{
			for (int y = 0; y < h; y++)
			{
				SetCell(new Vector2I(x, y), 0, new Vector2I(0, 3));
			}
		}
		Visible = false;
	}

	public void SetHighlightLayerForCity(City c)
	{
		ResetHighlightLayer();

		current = c;

		foreach (Hex h in c.territory)
		{
			currentlyHighlighted.Add(h);
			SetCell(h.coordinates, -1);
		}

		foreach (Hex h in c.borderTilePool)
		{
			currentlyHighlighted.Add(h);
			SetCell(h.coordinates, -1);
		}

		Visible = true;
	}

	public void ResetHighlightLayer()
	{
		foreach (Hex h in currentlyHighlighted)
		{
			SetCell(h.coordinates, 0, new Vector2I(0, 3));
		}
		current = null;
		Visible = false;
	}

	public void RefreshLayer()
	{
		if (current != null)
		{
			foreach (Hex h in current.territory)
			{
				currentlyHighlighted.Add(h);
				SetCell(h.coordinates, -1);
			}

			foreach (Hex h in current.borderTilePool)
			{
				currentlyHighlighted.Add(h);
				SetCell(h.coordinates, -1);
			}	
			Visible = true;
		} else {
			Visible = false;
		}

	}

}
