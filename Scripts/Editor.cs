using System;
using Godot;

namespace MazeRunner.Scripts;

public partial class Editor : Control
{
	[Export] Label SizeLabel;
	[Export] HSlider HSlider;
	public override void _Ready()
	{
		_global = GetNode<Global>("/root/Global");
		_global.Size = (int)HSlider.MinValue;
		SizeLabel.Text = HSlider.MinValue.ToString();
	}

	private Global _global;

	private void OnStartButtonPressed()
	{
		if (_global.Size == 0 || (_global.Seed == 0 && !_global.IsRandom)) return;
		_global.SetMaze();
		GetTree().ChangeSceneToFile("res://Scenes/game.tscn");
	}
	private void OnDifficultyLineEditTextChanged(string text)
	{
		if (text == "" || !int.TryParse(text, out int num)) return;
		_global.Size = num;
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
	private void OnPlayerOneOptionButtonItemSelected(int index) { _global.PlayerOneSkill = index; }
	private void OnPlayerTwoOptionButtonItemSelected(int index) { _global.PlayerTwoSkill = index; }
	private void OnSizeHSliderValueChanged(float f)
	{
		_global.Size = (int)f;
		SizeLabel.Text = f.ToString();
	}
}
