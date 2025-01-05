using Godot;
using MazeRunner.Scripts.Logic;
using System;

public partial class PlayerCamera : Camera2D
{
	private Token _tokenNode;
	private Global _global;
	private Board _board;
	private MazeGenerator _mazeGenerator;

	private int tileSize;
	(int x, int y) _cameraOffset = (0, 0);

	public enum State
	{
		Player,
		Free,
		Extensive
	}

	public State CurrentState => _currentState;
	public State _currentState = State.Player;

	public override void _Ready()
	{
		_board = GetNode<Board>("/root/Game/Node2D/Board");
		tileSize = _board.PixelSize;
		_tokenNode = GetNode<Token>("/root/Game/Node2D/Token");
		_global = GetNode<Global>("/root/Global");
		_mazeGenerator = _global.Setting.MazeGenerator;

		Zoom = new Vector2((float)(Math.Pow(tileSize, -1) * Math.Pow(_mazeGenerator.Size, -1) * 720), (float)(Math.Pow(tileSize, -1) * Math.Pow(_mazeGenerator.Size, -1) * 720));

		if (_global.LevelDifficulty < 5)
		{
			_currentState = State.Extensive;
		}
	}

	public override void _Process(double delta)
	{
		switch (_currentState)
		{
			case State.Player:
				if (Input.IsActionJustPressed("UIShiftCamera"))
				{
					_currentState = State.Free;
				}
				else
				{

					OnPlayer();
				}
				break;
			case State.Free:
				if (Input.IsActionJustPressed("UIShiftCamera"))
				{
					_cameraOffset = (0, 0);
					_currentState = State.Player;
				}
				else
				{
					OnFree();
				}
				break;
			case State.Extensive: OnExtensive(); break;
		}
	}

	void OnPlayer()
	{
		Position = _tokenNode.Position;
	}

	void OnFree()
	{
		HandleOnFreeMovement();
	}

	void OnExtensive()
	{
		Position = new Vector2(_mazeGenerator.Size * tileSize * 0.5f, _mazeGenerator.Size * tileSize * 0.5f);
	}

	void HandleOnFreeMovement()
	{
		if (Input.IsActionPressed("UIRight"))
		{
			_cameraOffset.x += 1 * tileSize;
		}
		if (Input.IsActionPressed("UILeft"))
		{
			_cameraOffset.x -= 1 * tileSize;
		}
		if (Input.IsActionPressed("UIUp"))
		{
			_cameraOffset.y -= 1 * tileSize;
		}
		if (Input.IsActionPressed("UIDown"))
		{
			_cameraOffset.y += 1 * tileSize;
		}
		Position = new Vector2(_tokenNode.Position.X + _cameraOffset.x, _tokenNode.Position.Y + _cameraOffset.y);
	}
}
