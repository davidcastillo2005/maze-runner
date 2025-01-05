using System;
using Godot;
using MazeRunner.Scripts.Data;
using MazeRunner.Scripts.Logic;
public partial class Token : CharacterBody2D
{
	[Export] float speed = 20;

	private Global global;
	private PlayerCamera playerCamera;
	private Board board;
	private MazeGenerator mazeGenerator;

	enum State { Spawning, Idle, Moving, Teleporting, Winning }
	private State currentState;
	private Vector2 input;
	public Vector2 tokenCoord;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerCamera = GetNode<PlayerCamera>("/root/Game/Node2D/PlayerCamera");
		global = GetNode<Global>("/root/Global");
		mazeGenerator = global.Setting.MazeGenerator;
		board = GetNode<Board>("/root/Game/Node2D/Board");

		currentState = State.Spawning;
	}

	public override void _Input(InputEvent @event)
	{
		input = Input.GetVector("UILeft", "UIRight", "UIUp", "UIDown");

		if (input == Vector2.Zero)
		{
			currentState = State.Idle;
		}
		else
		{
			currentState = State.Moving;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		tokenCoord = board.LocalToMap(Position);
		if (mazeGenerator.portalTiles.Contains(new Portal((int)tokenCoord.X, (int)tokenCoord.Y, true, null)))
		{
			currentState = State.Teleporting;
		}

		if (tokenCoord.X == mazeGenerator.ExitCoord.x && tokenCoord.Y == mazeGenerator.ExitCoord.y)
		{
			currentState = State.Winning;
		}

		switch (currentState)
		{
			case State.Spawning: Spawn(); break;
			case State.Idle: Idle(); break;
			case State.Moving: Move(delta); break;
			case State.Winning: Win(); break;
			case State.Teleporting: GD.Print(currentState); break;
		}
		
	}


	float GetConvertedPos(int i)
	{
		return (i + 0.5f) * board.PixelSize;
	}

	void Idle() { }

	void Move(double delta)
	{
		Velocity = new Vector2(input[0] * speed, input[1] * speed);
		MoveAndSlide();
	}

	void Spawn()
	{
		(int x, int y) = mazeGenerator.SpawnerCoord;
		Position = new Vector2(GetConvertedPos(x), GetConvertedPos(y));
		currentState = State.Idle;
	}

	void Win()
	{
		GetTree().Quit();
	}
}