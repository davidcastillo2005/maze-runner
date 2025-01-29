using Godot;

namespace MazeRunner.Scripts;

public partial class Editor : Control
{
	private Global _global;

	public override void _Ready() { _global = GetNode<Global>("/root/Global"); }
	public override void _Process(double delta) { }

	private void OnStartButtonPressed()
	{
		if (_global.Difficulty == null
			|| (_global.Seed == null && !_global.IsRandom)
			|| _global.PlayerOneName == string.Empty
			|| _global.PlayerTwoName == string.Empty) return;
		_global.SetMaze();
		_global.Setting.CheckSkillPlayerOne(_global.PlayerOneSkill);
		_global.Setting.CheckSkillPlayerTwo(_global.PlayerTwoSkill);
		GetTree().ChangeSceneToFile("res://Scenes/game.tscn");
	}
	private void OnDifficultyLineEditTextChanged(string text)
	{
		if (text == "") return;
		int num = int.Parse(text);
		_global.Difficulty = num;
	}
	private void OnSeedLineEditTextChanged(string text)
	{
		if (text == "") return;
		int num = int.Parse(text);
		_global.Seed = num;
	}
	private void OnRandomCheckButtonToggled(bool b) { _global.IsRandom = b; }
	private void OnPlayeOneNameLineEditTextChanged(string name) { _global.PlayerOneName = name; }
	private void OnPlayerTwoNameLineEditTextChanged(string name) { _global.PlayerTwoName = name; }
	private void OnPlayerOneOptionButtonItemSelected(int index) { _global.PlayerOneSkill = index - 1; }
	private void OnPlayerTwoOptionButtonItemSelected(int index) { _global.PlayerTwoSkill = index - 1; }
}
