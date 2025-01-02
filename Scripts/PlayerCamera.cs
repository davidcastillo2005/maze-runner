using Godot;
using MazeRunner.Scripts.Logic;
using System;

public partial class PlayerCamera : Camera2D
{
	Global _global;
	MazeGenerator _mazeGenerator;
	int cameraSize;
	Vector2 screenSize;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_global = GetNode<Global>("/root/Global");
		_mazeGenerator = _global.Setting.MazeGenerator;

		Position = new Vector2(_mazeGenerator.Size * 8, _mazeGenerator.Size * 8);
		Zoom = new Vector2((float)(45 * Math.Pow(_mazeGenerator.Size, -1)), (float)(45 * Math.Pow(_mazeGenerator.Size, -1)));
		
		GD.Print("Player camera position: " + (Position.X, Position.Y));
		GD.Print("Player camera zoom: " + (Zoom.X, Zoom.Y));
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
