using System.IO;
using Godot;
using MazeRunner.Scripts.Logic;

public partial class Global : Node
{
	private int _levelDifficulty = 6;
	
	private int _seed = 202;
	
	private bool _isRandomSeed = true;
	
	public Setting Setting { get; set; }

	public override void _Ready()
	{
		Setting = new(_levelDifficulty, _seed, _isRandomSeed);
		GD.Print("Maze size: " + Setting.MazeGenerator.Size);
		GD.Print("Seed: " + Setting.MazeGenerator.Seed);
	}

	public override void _Process(double delta)
	{
	}

	public void GoToScene(string path)
	{
		Setting.MazeGenerator.GenerateMaze();
		GetTree().ChangeSceneToFile(path);
	}

	public void QuitGame()
	{
		GetTree().Quit();
	}
}
