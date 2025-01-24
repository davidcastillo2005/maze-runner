using Godot;
using MazeRunner.Scripts;
using System;

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

	public void OnStartButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://Scenes/game.tscn");
	}

	//PlayerOne.
	public void OnPlayerOneShieldButtonPressed()
	{
		_global.Setting.CheckSkillPlayerOne(0);
	}

	public void OnPlayerOnePortalGunPressed()
	{
		_global.Setting.CheckSkillPlayerOne(1);
	}

	public void OnPlayerOneBlindnessButtonPressed()
	{
		_global.Setting.CheckSkillPlayerOne(2);
	}

	//PlayerTwo.
	public void OnPlayerTwoShieldButtonPressed()
	{
		_global.Setting.CheckSkillPlayerTwo(0);
	}

	public void OnPlayerTwoPortalGunButtonPressed()
	{
		_global.Setting.CheckSkillPlayerTwo(1);
	}

	public void OnPlayerTwoBlindnessButtonPressed()
	{
		_global.Setting.CheckSkillPlayerTwo(2);
	}
}
