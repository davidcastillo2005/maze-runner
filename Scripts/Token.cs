using Godot;
using System;
using MazeRunner.Scripts.Data;

public partial class Token : CharacterBody2D
{
	private Global _globalInstance;
	private Tile[,] _map;
	public (int x, int y) tokenTile;
	enum State { Idle, Moving }
	private State _currentState = State.Idle;
	(int x, int y) _input;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_globalInstance = GetNode<Global>("/root/Global");
		_map = _globalInstance.Map;

		Spawn();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GD.Print(_currentState);
		if (_currentState == State.Idle) Idle();
		if (_currentState == State.Moving) Move();
	}

	// Converts a tile index to a position in pixels.
	static int GetConvertedPos(int i)
	{
		return (2 * i + 1) * 8;
	}

	// Handles the idle state of the token.
	void Idle()
	{
		if (Input.IsActionJustPressed("ui_up") || Input.IsActionJustPressed("ui_down") || Input.IsActionJustPressed("ui_left") || Input.IsActionJustPressed("ui_right")) _currentState = State.Moving;
		else _currentState = State.Idle;
	}

	// Handles the moving state of the token.
	void Move()
	{
		_currentState = State.Idle;
		_input.x = (Input.IsActionJustPressed("ui_right") ? 1 : 0) - (Input.IsActionJustPressed("ui_left") ? 1 : 0);
		_input.y = (Input.IsActionJustPressed("ui_down") ? 1 : 0) - (Input.IsActionJustPressed("ui_up") ? 1 : 0);

		if (Mathf.Abs(_input.x) == Mathf.Abs(_input.y)) return;
		if (_input.x == 0 && _input.y == 0)
		{
			_currentState = State.Idle;
			return;
		}

		(int x, int y) newTokenTile = (tokenTile.x + _input.x, tokenTile.y + _input.y);
		if (IsInsideLimits(newTokenTile))
		{
			if (_map[newTokenTile.x, newTokenTile.y].GetType() == typeof(Empty))
			{
				tokenTile = newTokenTile;
				Position = new Vector2(GetConvertedPos(tokenTile.x), GetConvertedPos(tokenTile.y));
			}
			else
			{
				_currentState = State.Idle;
				return;
			}
		}
		else
		{
			(int x, int y) oppositePos = GetOppositePos(tokenTile, _input);
			if (_map[oppositePos.x, oppositePos.y].GetType() == typeof(Empty))
			{
				tokenTile = oppositePos;
				Position = new Vector2(GetConvertedPos(tokenTile.x), GetConvertedPos(tokenTile.y));
			}
			else
			{
				_currentState = State.Idle;
				return;
			}
		}
		_currentState = State.Moving;
	}

	// Checks if the given tile coordinates are within the boundaries of the map.
	bool IsInsideLimits((int x, int y) newTokenTile) => newTokenTile.x >= 0 && newTokenTile.y >= 0 && newTokenTile.x < _map.GetLength(0) && newTokenTile.y < _map.GetLength(1);

	// Checks if the input direction is valid (down, up, right or left).
	bool IsValidInput((int x, int y) input) => input == (0, 1) || input == (0, -1) || input == (1, 0) || input == (-1, 0);

	// Gets the opposite position on the map when the token moves out of bounds.
	(int x, int y) GetOppositePos((int x, int y) tokenTile, (int x, int y) input)
	{
		if (!IsValidInput(input)) throw new ArgumentException($"Invalid input direction {input}");
		if (tokenTile.x == 0 || tokenTile.y == 0 || tokenTile.x == _map.GetLength(0) - 1 || tokenTile.y == _map.GetLength(1) - 1)
		{
			if (input == (0, 1)) return (tokenTile.x, 0);
			else if (input == (0, -1)) return (tokenTile.x, _map.GetLength(1) - 1);
			else if (input == (1, 0)) return (0, tokenTile.y);
			else if (input == (-1, 0)) return (_map.GetLength(0) - 1, tokenTile.y);
		}
		throw new InvalidOperationException("Token is not at the edge of the _map.");
	}

	// Spawns the token at the first available empty tile on the map.
	void Spawn()
	{
		for (int x = 0; x < _map.GetLength(0); x++)
		{
			for (int y = 0; y < _map.GetLength(1); y++)
			{
				if (_map[x, y].GetType() == typeof(Empty))
				{
					tokenTile = (x, y);
					Position = new Vector2(GetConvertedPos(x), GetConvertedPos(y));
					return;
				}
			}
		}
	}
}