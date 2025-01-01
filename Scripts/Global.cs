using System.IO;
using Godot;
using MazeRunner.Scripts.Logic;

public partial class Global : Node
{
	//Level difficulty.
	private int _levelDifficulty = 10;
	//Seed for random generation.
	private int _seed = 202;
	//Random seed.
	private bool _isRandomSeed = false;
	//Instance of the setting class.
	public Setting Setting { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Create a new instance of the setting class.
		Setting = new(_levelDifficulty, _seed, _isRandomSeed);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
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
