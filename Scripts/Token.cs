using Godot;
using System;
using MazeRunner.Scripts.Data;
using System.Collections.Generic;

public partial class Token : Node2D
{
	Token tokenNode2D;
	Global global;
	Tile[,] map;
	(int x, int y) playerPos;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tokenNode2D = GetNode<Token>("/root/Main/Token");
		global = GetNode<Global>("/root/Global");
		map = global.map;

		Spawn();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		ManageMovement();
	}

	int GetConvertedPos(int x)
	{
		int result = (2 * x + 1) * 8;
		return result;
	}

	void Spawn()
	{
		for (int x = 0; x < map.GetLength(0); x++)
		{
			for (int y = 0; y < map.GetLength(1); y++)
			{
				if (map[x, y].GetType() == typeof(Empty))
				{
					playerPos = (x, y);
					tokenNode2D.Position = new Vector2I(GetConvertedPos(x), GetConvertedPos(y));
					return;
				}
			}
		}
	}

	void ManageMovement()
	{
		if (Input.IsActionJustPressed("ui_right"))
		{
			(int x, int y) newPlayerPos;
			newPlayerPos.x = playerPos.x + 1;
			newPlayerPos.y = playerPos.y;
			if (newPlayerPos.x >= 0 && newPlayerPos.y >= 0 && newPlayerPos.x < map.GetLength(0) && newPlayerPos.y < map.GetLength(1))
			{
				if (map[newPlayerPos.x, newPlayerPos.y].GetType() == typeof(Empty))
				{
					playerPos = newPlayerPos;
					tokenNode2D.Position = new Vector2I(GetConvertedPos(playerPos.x), GetConvertedPos(playerPos.y));
				}
			}
			else if (map[0, playerPos.y].GetType() == typeof(Empty))
			{
				playerPos = (0, playerPos.y);
				tokenNode2D.Position = new Vector2I(GetConvertedPos(0), GetConvertedPos(playerPos.y));
			}
		}
		else if (Input.IsActionJustPressed("ui_left"))
		{
			(int x, int y) newPlayerPos;
			newPlayerPos.x = playerPos.x - 1;
			newPlayerPos.y = playerPos.y;
			if (newPlayerPos.x >= 0 && newPlayerPos.y >= 0 && newPlayerPos.x < map.GetLength(0) && newPlayerPos.y < map.GetLength(1))
			{
				if (map[newPlayerPos.x, newPlayerPos.y].GetType() == typeof(Empty))
				{
					playerPos = newPlayerPos;
					tokenNode2D.Position = new Vector2I(GetConvertedPos(playerPos.x), GetConvertedPos(playerPos.y));
				}
			}
			else if (map[map.GetLength(0) - 1, playerPos.y].GetType() == typeof(Empty))
			{
				playerPos = (map.GetLength(0) - 1, playerPos.y);
				tokenNode2D.Position = new Vector2I(GetConvertedPos(map.GetLength(0) - 1), GetConvertedPos(playerPos.y));
			}
		}
		else if (Input.IsActionJustPressed("ui_up"))
		{
			(int x, int y) newPlayerPos;
			newPlayerPos.x = playerPos.x;
			newPlayerPos.y = playerPos.y - 1;
			if (newPlayerPos.x >= 0 && newPlayerPos.y >= 0 && newPlayerPos.x < map.GetLength(0) && newPlayerPos.y < map.GetLength(1))
			{
				if (map[newPlayerPos.x, newPlayerPos.y].GetType() == typeof(Empty))
				{
					playerPos = newPlayerPos;
					tokenNode2D.Position = new Vector2I(GetConvertedPos(playerPos.x), GetConvertedPos(playerPos.y));
				}
			}
			else if (map[playerPos.x, map.GetLength(1) - 1].GetType() == typeof(Empty))
			{
				playerPos = (playerPos.x, map.GetLength(1) - 1);
				tokenNode2D.Position = new Vector2I(GetConvertedPos(playerPos.x), GetConvertedPos(map.GetLength(1) - 1));
			}
		}
		else if (Input.IsActionJustPressed("ui_down"))
		{
			(int x, int y) newPlayerPos;
			newPlayerPos.x = playerPos.x;
			newPlayerPos.y = playerPos.y + 1;
			if (newPlayerPos.x >= 0 && newPlayerPos.y >= 0 && newPlayerPos.x < map.GetLength(0) && newPlayerPos.y < map.GetLength(1))
			{
				if (map[newPlayerPos.x, newPlayerPos.y].GetType() == typeof(Empty))
				{
					playerPos = newPlayerPos;
					tokenNode2D.Position = new Vector2I(GetConvertedPos(playerPos.x), GetConvertedPos(playerPos.y));
				}
			}
			else if (map[playerPos.x, 0].GetType() == typeof(Empty))
			{
				playerPos = (playerPos.x, 0);
				tokenNode2D.Position = new Vector2I(GetConvertedPos(playerPos.x), GetConvertedPos(0));
			}
		}
	}
}
