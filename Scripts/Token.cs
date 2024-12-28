using Godot;
using System;
using MazeRunner.Scripts.Data;
using System.Threading.Tasks;

public partial class Token : CharacterBody2D
{
	//Shared variables in a autoload script called Global.
	private Global _global;
	// Map generated, with empties, walls, traps and an exit.
	private Tile[,] _map;
	// Position of the token in the map.
	public (int x, int y) tokenTile;
	// Enum for the state of the token (Idle or Moving, not both).
	enum State { Idle, Moving }
	// Current state of the token.
	private State _currentState = State.Idle;
	// Direction based on inputs.
	(int x, int y) _input;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Get the global script.
		_global = GetNode<Global>("/root/Global");
		//Get the map from the global script.
		_map = _global.Setting.Map;

		//Spawn the token.
		Spawn();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Check if the current state is idle or moving state.
		if (_currentState == State.Idle)
			Idle();
		if (_currentState == State.Moving)
			Move();
	}

	/// <summary>
	/// Converts a tile index to a position in pixels.
	/// </summary>
	/// <param name="i"></param>
	/// <returns></returns>
	int GetConvertedPos(int i)
	{
		return (2 * i + 1) * 8;
	}

	/// <summary>
	/// Handles the idle state of the token.
	/// </summary>
	void Idle()
	{
		//Check if keys for up, down, left or right are pressed.
		if (Input.IsActionPressed("ui_up") || Input.IsActionPressed("ui_down") || Input.IsActionPressed("ui_left") || Input.IsActionPressed("ui_right"))
			//Change to moving state.
			_currentState = State.Moving;
		else
			//Change to idle state.
			_currentState = State.Idle;
	}

	/// <summary>
	/// Handles the moving state of the token.
	/// </summary>
	void Move()
	{
		//Change to idle state.
		_currentState = State.Idle;

		//Wait for 100ms.
		Task.Delay(100).Wait();

		_input.x = (Input.IsActionPressed("ui_right") ? 1 : 0) - (Input.IsActionPressed("ui_left") ? 1 : 0);
		_input.y = (Input.IsActionPressed("ui_down") ? 1 : 0) - (Input.IsActionPressed("ui_up") ? 1 : 0);

		//Check if input is diagonal.
		if (Mathf.Abs(_input.x) == Mathf.Abs(_input.y)) return;

		//Check if input is 0.
		if (_input.x == 0 && _input.y == 0)
		{
			//Change to idle state.
			_currentState = State.Idle;
			return;
		}

		//Calculate new position.
		(int x, int y) newTokenTile = (tokenTile.x + _input.x, tokenTile.y + _input.y);

		//Check if the new position is inside the map then move to that position, else move to the opposite position.
		if (IsInsideBounds(newTokenTile))
		{
			//Check if the new position is empty.
			if (_map[newTokenTile.x, newTokenTile.y].GetType() == typeof(Empty) || _map[newTokenTile.x, newTokenTile.y].GetType() == typeof(Spawner))
			{
				//Update the position and the token tile.
				tokenTile = newTokenTile;
				Position = new Vector2(GetConvertedPos(tokenTile.x), GetConvertedPos(tokenTile.y));
			}
			else
			{
				//Stay in idle state.
				_currentState = State.Idle;
				return;
			}
		}
		else
		{
			//Calculate the opposite position.
			(int x, int y) oppositePos = GetOppositePos(tokenTile, _input);

			//Check if the opposite position is empty.
			if (_map[oppositePos.x, oppositePos.y].GetType() == typeof(Empty))
			{
				//Update the position and the token tile.
				tokenTile = oppositePos;
				Position = new Vector2(GetConvertedPos(tokenTile.x), GetConvertedPos(tokenTile.y));
			}
			else
			{
				//Stay in idle state.
				_currentState = State.Idle;
				return;
			}
		}
		//Change to moving state.
		_currentState = State.Moving;
	}

	/// <summary>
	/// Checks if the given tile coordinates are within the boundaries of the map.
	/// </summary>
	/// <param name="tile"></param>
	/// <returns></returns>
	bool IsInsideBounds((int x, int y) tile) => tile.x >= 0 && tile.y >= 0 && tile.x < _map.GetLength(0) && tile.y < _map.GetLength(1);

	/// <summary>
	/// Checks if the input direction is valid (down, up, right or left).
	/// </summary>
	/// <param name="input"></param>
	/// <returns></returns>
	bool IsValidInput((int x, int y) input) => input == (0, 1) || input == (0, -1) || input == (1, 0) || input == (-1, 0);

	/// <summary>
	/// Gets the opposite position on the map when the token moves out of bounds.
	/// </summary>
	/// <param name="tile"></param>
	/// <param name="input"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="InvalidOperationException"></exception>
	(int x, int y) GetOppositePos((int x, int y) tile, (int x, int y) input)
	{
		//Check if the input direction is valid.
		if (!IsValidInput(input)) throw new ArgumentException($"Invalid input direction {input}");
		//Check if token is on any bound of the map.
		if (tile.x == 0 || tile.y == 0 || tile.x == _map.GetLength(0) - 1 || tile.y == _map.GetLength(1) - 1)
		{
			//Translate input to an opposites tile of the actual tile.
			if (input == (0, 1)) return (tile.x, 0);
			else if (input == (0, -1)) return (tile.x, _map.GetLength(1) - 1);
			else if (input == (1, 0)) return (0, tile.y);
			else if (input == (-1, 0)) return (_map.GetLength(0) - 1, tile.y);
		}
		throw new InvalidOperationException("Token is not at the edge of the _map.");
	}

	/// <summary>
	/// Spawns the token at the first available empty tile on the map.
	/// </summary>
	void Spawn()
	{
		//Two for loops, one for the x axis and one for the y axis, to iterate over the map.
		for (int x = 0; x < _map.GetLength(0); x++)
		{
			for (int y = 0; y < _map.GetLength(1); y++)
			{
				//Check if the tile is a spawner.
				if (_map[x, y].GetType() == typeof(Spawner))
				{
					//At the spawner set the position and the token tile.
					tokenTile = (x, y);
					Position = new Vector2(GetConvertedPos(x), GetConvertedPos(y));
					return;
				}
			}
		}
	}
}