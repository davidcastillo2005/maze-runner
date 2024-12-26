using Godot;
using System;
using MazeRunner.Scripts.Data;

public partial class Token : CharacterBody2D
{
	Token tokenNode2D;
	Global global;
	Tile[,] map;
	(int x, int y) tokenTile;
	enum State { Idle, Moving }
	State currentState = State.Idle;

	public override void _Ready()
	{
		tokenNode2D = GetNode<Token>("/root/Main/Token");
		global = GetNode<Global>("/root/Global");
		map = global.map;

		Spawn();
	}

	public override void _Process(double delta)
	{
		HandleState();
	}

	int GetConvertedPos(int i)
	{
		int result = (2 * i + 1) * 8;
		return result;
	}

	void HandleState()
	{
		if (currentState == State.Idle) Idle();
		else if (currentState == State.Moving) HandleMovement();
	}

	void Idle()
	{
		if (Input.IsActionPressed("ui_up") || Input.IsActionPressed("ui_down") || Input.IsActionPressed("ui_left") || Input.IsActionPressed("ui_right")) currentState = State.Moving;
	}

	void HandleMovement()
	{
		(int x, int y) newTokenTile = tokenTile;
		if (Input.IsActionPressed("ui_right"))
		{
			newTokenTile.x += 1;
			Move(newTokenTile, (1, 0));
		}
		else if (Input.IsActionPressed("ui_left"))
		{
			newTokenTile.x -= 1;
			Move(newTokenTile, (-1, 0));
		}
		else if (Input.IsActionPressed("ui_down"))
		{
			newTokenTile.y += 1;
			Move(newTokenTile, (0, 1));
		}
		else if (Input.IsActionPressed("ui_up"))
		{
			newTokenTile.y -= 1;
			Move(newTokenTile, (0, -1));
		}
	}

	bool IsInsideLimits((int x, int y) newTokenTile) => newTokenTile.x >= 0 && newTokenTile.y >= 0 && newTokenTile.x < map.GetLength(0) && newTokenTile.y < map.GetLength(1);

	void Move((int x, int y) newTokenTile, (int x, int y) direction)
	{
		if (IsInsideLimits(newTokenTile))
		{
			if (map[newTokenTile.x, newTokenTile.y].GetType() == typeof(Empty))
			{
				tokenTile = newTokenTile;
				Position = new Vector2(GetConvertedPos(tokenTile.x), GetConvertedPos(tokenTile.y));
				currentState = State.Idle;
			}
		}
		else if (map[GetOppositePos(tokenTile, direction).x, GetOppositePos(tokenTile, direction).y].GetType() == typeof(Empty))
		{
			tokenTile = (GetOppositePos(tokenTile, direction).x, GetOppositePos(tokenTile, direction).y);
			Position = new Vector2I(GetConvertedPos(tokenTile.x), GetConvertedPos(tokenTile.y));
		}
	}

	(int x, int y) GetOppositePos((int x, int y) tokenTile, (int x, int y) direction)
	{
		if (tokenTile.x == 0 || tokenTile.y == 0 || tokenTile.x == map.GetLength(0) - 1 || tokenTile.y == map.GetLength(1) - 1)
		{
			if (direction == (0, 1)) return (tokenTile.x, 0);
			else if (direction == (0, -1)) return (tokenTile.x, map.GetLength(1) - 1);
			else if (direction == (1, 0)) return (0, tokenTile.y);
			else if (direction == (-1, 0)) return (map.GetLength(0) - 1, tokenTile.y);
			else throw new Exception("!direction");
		}
		else throw new Exception("!(tokenTile.x == 0 || tokenTile.y == 0 || tokenTile.x == map.GetLength(0) - 1 || tokenTile.y == map.GetLength(1) - 1)");
	}

	void Spawn()
	{
		for (int x = 0; x < map.GetLength(0); x++)
		{
			for (int y = 0; y < map.GetLength(1); y++)
			{
				if (map[x, y].GetType() == typeof(Empty))
				{
					tokenTile = (x, y);
					Position = new Vector2I(GetConvertedPos(x), GetConvertedPos(y));
					return;
				}
			}
		}
	}
}