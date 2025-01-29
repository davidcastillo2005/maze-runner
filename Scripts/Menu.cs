using Godot;

namespace MazeRunner.Scripts;

public partial class Menu : Control
{
	[Export] Button _playButton;
	[Export] Button _quitButton;

	public override void _Ready() { }
	public override void _Process(double delta) { }
	public void OnPlayButtonDown() { GetTree().ChangeSceneToFile("res://Scenes/editor.tscn"); }
	public void OnQuitButtonDown() { GetTree().Quit(); }
}
