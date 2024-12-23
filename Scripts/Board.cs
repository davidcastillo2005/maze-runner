using Godot;
using MazeRunner.Scripts.Data;
using MazeRunner.Scripts.Logic;
using System;

public partial class Board : TileMapLayer
{
	[Export] int width = 20;
	[Export] int height = 20;
	[Export] TileMapLayer boardTileMapLayerNode;

	Tile[,] board;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		board = MapGenerator.GenerateMap(width, height);
		PaintBoard(board);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void PaintBoard(Tile[,] board)
	{
		for (int x = 0; x < board.GetLength(0); x++)
		{
			for (int y = 0; y < board.GetLength(1); y++)
			{
				Type tileType = board[x, y].GetType();
				if (tileType == typeof(Empty))
				{
					boardTileMapLayerNode.SetCell(new Vector2I(x, y), -1, new Vector2I(1, 1), 0);
				}
				else if (tileType == typeof(Wall))
				{
					boardTileMapLayerNode.SetCell(new Vector2I(x, y), 0, new Vector2I(1, 1), 0);
				}
			}
		}
	}
}
