using Godot;
using System;
using MazeRunner.Scripts.Data;
using MazeRunner.Scripts.Logic;

public partial class Token : CharacterBody2D
{
	enum State { Spawning, Idle, Moving, Winning }

	private State _currentState = State.Spawning;

	public (int x, int y) TokenCoord => _tokenCoord;
	private (int x, int y) _tokenCoord;

	private Global _global;
	private Tile[,] maze;
	private int size;
	private (int x, int y) _input;
	private MazeGenerator _mazeGenerator;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_global = GetNode<Global>("/root/Global");
		maze = _global.Setting.MazeGenerator.Maze;
		size = _global.Setting.MazeGenerator.Size;
		_mazeGenerator = _global.Setting.MazeGenerator;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_tokenCoord == _mazeGenerator.ExitCoord)
		{
			_currentState = State.Winning;
		}

		switch (_currentState)
		{
			case State.Spawning:
				Spawn();
				break;
			case State.Idle:
				Idle();
				break;
			case State.Moving:
				Move();
				break;
			case State.Winning:
				Win();
				break;
		}
	}

	int GetConvertedPos(int i)
	{
		return (2 * i + 1) * 8;
	}

	void Idle()
	{
		if (Input.IsActionPressed("ui_up") || Input.IsActionPressed("ui_down") || Input.IsActionPressed("ui_left") || Input.IsActionPressed("ui_right"))
		{
			_currentState = State.Moving;
		}
		else
		{
			_currentState = State.Idle;
		}
	}

	void Move()
	{
		if (!Input.IsActionPressed("ui_up") && !Input.IsActionPressed("ui_down") && !Input.IsActionPressed("ui_left") && !Input.IsActionPressed("ui_right"))
		{
			_currentState = State.Idle;
		}
		else
		{
			if (Input.IsActionPressed("ui_up"))
			{
				_input.y += -1;
			}
			if (Input.IsActionPressed("ui_down"))
			{
				_input.y += 1;
			}
			if (Input.IsActionPressed("ui_left"))
			{
				_input.x += -1;
			}
			if (Input.IsActionPressed("ui_right"))
			{
				_input.x += 1;
			}

			(int x, int y) targetCoord = (_input.x + _tokenCoord.x, _input.y + _tokenCoord.y);
			if (_mazeGenerator.IsInsideBounds(targetCoord) && maze[targetCoord.x, targetCoord.y] is Empty)
			{
				_tokenCoord = targetCoord;
				Position = new Vector2(GetConvertedPos(targetCoord.x), GetConvertedPos(targetCoord.y));
			}

			_input = (0, 0);
		}
	}

	bool IsValidInput((int x, int y) input) => input == (0, 1) || input == (0, -1) || input == (1, 0) || input == (-1, 0);

	(int x, int y) GetOppositePos((int x, int y) tile, (int x, int y) input)
	{
		if (!IsValidInput(input)) throw new ArgumentException($"Invalid input direction {input}");
		if (tile.x == 0 || tile.y == 0 || tile.x == size - 1 || tile.y == size - 1)
		{
			if (input == (0, 1)) return (tile.x, 0);
			else if (input == (0, -1)) return (tile.x, size - 1);
			else if (input == (1, 0)) return (0, tile.y);
			else if (input == (-1, 0)) return (size - 1, tile.y);
		}
		throw new InvalidOperationException("Token is not at the edge of the maze.");
	}

	void Spawn()
	{
		_tokenCoord = _mazeGenerator.SpawnerCoord;
		Position = new Vector2(GetConvertedPos(_tokenCoord.x), GetConvertedPos(_tokenCoord.y));
		_currentState = State.Idle;
	}

	void Win()
	{
		_global.QuitGame();
	}
}