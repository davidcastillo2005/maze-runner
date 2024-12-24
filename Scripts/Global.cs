using Godot;
using MazeRunner.Scripts.Data;
using MazeRunner.Scripts.Logic;
using System;

public partial class Global : Node
{
	[Export] int width = 40;
	[Export] int height = 40;
	[Export] int seed;
	[Export] bool isRandomSeed = true;
	[Export] int fillPercentage = 25;
	public Tile[,] map;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		map = MapGenerator.GenerateMap(width, height, seed, isRandomSeed, fillPercentage);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_accept"))
		{
			map = MapGenerator.GenerateMap(width, height, seed, isRandomSeed, fillPercentage);
		}
	}
}
