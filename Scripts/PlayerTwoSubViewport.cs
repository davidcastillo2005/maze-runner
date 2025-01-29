using Godot;

namespace MazeRunner.Scripts;

public partial class PlayerTwoSubViewport : SubViewport
{
	[Export] private SubViewport _subViewport;

	public override void _Ready() { World2D = _subViewport.World2D; }
}
