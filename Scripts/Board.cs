using Godot;
using MazeRunner.Scripts.Data;
using MazeRunner.Scripts.Logic;
using System;

public partial class Board : TileMapLayer
{
	private Tile[,] _map;
	private Global _global;
	[Export] bool useChessBoardPattern = true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_global = GetNode<Global>("/root/Global");
		_map = _global.Map;
		PaintBoardTileMapLayer(_map);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		PaintBoardTileMapLayer(_map);
	}

	public void PaintBoardTileMapLayer(Tile[,] map)
	{
		Clear();
		for (int x = 0; x < map.GetLength(0); x++)
		{
			for (int y = 0; y < map.GetLength(1); y++)
			{
				Type tileType = map[x, y].GetType();
				if (useChessBoardPattern)
				{
					if (tileType == typeof(Empty))
					{
						SetCell(new Vector2I(x, y), -1, new Vector2I(1, 1), 0);
					}
					else if (tileType == typeof(Wall))
					{
						if (Math.Pow(-1, x + y) == 1)
						{
							SetCell(new Vector2I(x, y), 0, new Vector2I(1, 1), 0);
						}
						else if (Math.Pow(-1, x + y) == -1)
						{
							SetCell(new Vector2I(x, y), 0, new Vector2I(3, 2), 0);
						}
					}
				}
				else
				{
					if (tileType == typeof(Empty))
					{
						SetCell(new Vector2I(x, y), 0, new Vector2I(1, 1), 0);
					}
					else if (tileType == typeof(Wall))
					{
						SetCell(new Vector2I(x, y), 0, new Vector2I(3, 2), 0);
					}
				}
			}
		}
	}
}
