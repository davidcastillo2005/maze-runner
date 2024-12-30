using System.IO;
using Godot;
using MazeRunner.Scripts.Logic;

public partial class Global : Node
{
	//Level difficulty.
	private int _levelDifficulty = 5;
	//Seed for random generation.
	private int _seed = 1;
	//Random seed.
	private bool _isRandomSeed = true;
	//Instance of the setting class.
	public Setting Setting { get => _setting; set => _setting = value; }
	private Setting _setting;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Create a new instance of the setting class.
		_setting = new(_levelDifficulty, _seed, _isRandomSeed);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void GoToScene(string path)
	{
		GetTree().ChangeSceneToFile(path);
	}

	public void QuitGame()
	{
		GetTree().Quit();
	}
}
