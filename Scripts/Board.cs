using Godot;
using MazeRunner.Scripts.Data;
using System;

public partial class Board : TileMapLayer
{
	//Map generated.
	private Tile[,] _map;
	//Shared variables in a autoload script called Global.
	private Global _global;
	//Paint board tile map layer with chess board pattern.
	[Export] bool isChessBoardPattern = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Get global script.
		_global = GetNode<Global>("/root/Global");
		//Get map from the global script.
		_map = _global.Setting.Map;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Paint Board Tile Map Layer.
		PaintBoardTileMapLayer();
	}

	/// <summary>
	/// Paints Board Tile Map Layer with the map generated.	
	/// </summary>
	void PaintBoardTileMapLayer()
	{
		//Clear Tile Map Layer.
		Clear();

		//A "for" loop to iterate through the map.
		for (int x = 0; x < _map.GetLength(0); x++)
		{
			for (int y = 0; y < _map.GetLength(1); y++)
			{
				//Get the type of the tile.
				Type tileType = _map[x, y].GetType();

				//Check if the chess board pattern is enabled.
				if (isChessBoardPattern)
				{
					//Check if the tile is empty or a wall.
					if (tileType == typeof(Empty))
						//Paint the tile map layer with the tile assigned.
						SetCell(new Vector2I(x, y), -1, new Vector2I(1, 1), 0);
					else if (tileType == typeof(Wall))
					{
						//Check the position of the tile based of the formula (-1) ^ ( x + y ).
						if (Math.Pow(-1, x + y) == 1)
							//Paint the tile map layer with the tile assigned.
							SetCell(new Vector2I(x, y), 0, new Vector2I(1, 1), 0);
						else if (Math.Pow(-1, x + y) == -1)
							//Paint the tile map layer with the tile assigned.
							SetCell(new Vector2I(x, y), 0, new Vector2I(3, 2), 0);
					}
				}
				else
				{
					//Check if the tile is empty or a wall.
					if (tileType == typeof(Empty))
						//Paint the tile map layer with the tile assigned.
						SetCell(new Vector2I(x, y), 0, new Vector2I(1, 1), 0);
					else if (tileType == typeof(Wall))
						//Paint the tile map layer with the tile assigned.
						SetCell(new Vector2I(x, y), 0, new Vector2I(3, 2), 0);
				}
			}
		}
	}
}
