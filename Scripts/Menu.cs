using Godot;

namespace MazeRunner.Scripts;

public partial class Menu : Control
{
	public void OnPlayButtonDown() { GetTree().ChangeSceneToFile("res://Scenes/editor.tscn"); }
	public void OnQuitButtonDown() { GetTree().Quit(); }
}
