using Godot;
using MazeRunner.Scripts.Logic;

namespace MazeRunner.Scripts;
public partial class Setup : Node
{
    public Setting Setting { get; set; }

    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
    }

    public void GenerateMaze()
    {

        Setting.MazeGenerator.GenerateMaze();
    }
}
