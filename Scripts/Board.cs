using Godot;
using MazeRunner.Scripts.Data;
using MazeRunner.Scripts.Logic;
using System;

public partial class Board : TileMapLayer
{
	TileMapLayer boardTileMapLayer;
	Tile[,] Map { get; set; }
	Global global;
	[Export] bool useChessBoardPattern = true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		boardTileMapLayer = GetNode<Board>("/root/Main/Board");
		global = GetNode<Global>("/root/Global");
		Map = global.map;
		PaintBoardTileMapLayer(Map);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Map = global.map;
		PaintBoardTileMapLayer(Map);
	}

	public void PaintBoardTileMapLayer(Tile[,] map)
	{
		boardTileMapLayer.Clear();
		for (int x = 0; x < map.GetLength(0); x++)
		{
			for (int y = 0; y < map.GetLength(1); y++)
			{
				Type tileType = map[x, y].GetType();
				if (useChessBoardPattern)
				{
					if (tileType == typeof(Empty))
					{
						boardTileMapLayer.SetCell(new Vector2I(x, y), -1, new Vector2I(1, 1), 0);
					}
					else if (tileType == typeof(Wall))
					{
						if (Math.Pow(-1, x + y) == 1)
						{
							boardTileMapLayer.SetCell(new Vector2I(x, y), 0, new Vector2I(1, 1), 0);
						}
						else if (Math.Pow(-1, x + y) == -1)
						{
							boardTileMapLayer.SetCell(new Vector2I(x, y), 0, new Vector2I(3, 2), 0);
						}
					}
				}
				else
				{
					if (tileType == typeof(Empty))
					{
						boardTileMapLayer.SetCell(new Vector2I(x, y), 0, new Vector2I(1, 1), 0);
					}
					else if (tileType == typeof(Wall))
					{
						boardTileMapLayer.SetCell(new Vector2I(x, y), 0, new Vector2I(3, 2), 0);
					}
				}
			}
		}
	}
}
