using Godot;
using MazeRunner.Scripts.Logic;

public partial class Global : Node
{
	public int LevelDifficulty => _levelDifficulty;
	private int _levelDifficulty = 4;
	
	private int _seed = 202;
	
	private bool _isRandomSeed = true;
	
	public Setting Setting { get; set; }

	public override void _Ready()
	{
		Setting = new(_levelDifficulty, _seed, _isRandomSeed);
		GD.Print("Maze size: " + Setting.MazeGenerator.Size);
		GD.Print("Seed: " + Setting.MazeGenerator.Seed);
		Setting.MazeGenerator.GenerateMaze();
	}

	public override void _Process(double delta)
	{
	}
}
