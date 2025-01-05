using Godot;
using MazeRunner.Scripts.Logic;

public partial class Global : Node
{
	public int LevelDifficulty => _levelDifficulty;
	private int _levelDifficulty = 12;
	
	private int _seed = 202;
	
	private bool _isRandomSeed = true;
	
	public Setting Setting { get; set; }

	public override void _Ready()
	{
		Setting = new(_levelDifficulty, _seed, _isRandomSeed);
		Setting.MazeGenerator.GenerateMaze();
	}

	public override void _Process(double delta)
	{
	}
}
