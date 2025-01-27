using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace MazeRunner.Scripts;

public partial class Editor : Control
{
	private Global _global;

	public override void _Ready()
	{
		_global = GetNode<Global>("/root/Global");
	}

	public override void _Process(double delta)
	{
	}

	private void OnStartButtonPressed()
	{
		_global.SetMaze();
		_global.Setting.CheckSkillPlayerOne(_global.PlayerOneSkill);
		_global.Setting.CheckSkillPlayerTwo(_global.PlayerTwoSkill);
		GetTree().ChangeSceneToFile("res://Scenes/game.tscn");
	}

	void OnPlayerOneOptionButtonItemSelected(int index)
	{
		_global.PlayerOneSkill = index - 1;
		GD.Print(_global.PlayerOneSkill);
	}

	void OnPlayerTwoOptionButtonItemSelected(int index)
	{
		_global.PlayerTwoSkill = index - 1;
		GD.Print(_global.PlayerTwoSkill);
	}
}
