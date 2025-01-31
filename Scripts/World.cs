using Godot;
using MazeRunner.Scripts.Data;

namespace MazeRunner.Scripts;

public partial class World : Node2D
{
    [Export] private Camera2D _playerOneCamera;
    [Export] private Camera2D _playerTwoCamera;
    [Export] private Player _playerOne;
    [Export] private Player _playerTwo;

    private Global _global;

    public override void _Ready()
    {
        _global = GetNode<Global>("/root/Global");
    }
    public override void _Input(InputEvent @event)
    {
        if (!Input.IsActionJustPressed("SwitchCamera")) return;
        SwitchCamera();
    }
    public void SwitchCamera()
    {
        if (_playerOneCamera.IsCurrent()) _playerTwoCamera.MakeCurrent();
        else if (_playerTwoCamera.IsCurrent()) _playerOneCamera.MakeCurrent();
    }

    private void GDPrintMaze()
    {
        for (int i = 0; i < _global.MazeGenerator.Size; i++)
        {
            string rowLog = "";
            for (int j = 0; j < _global.MazeGenerator.Size; j++)
            {
                if (_global.MazeGenerator.Maze[j, i] is Empty and not Spawner and not Exit and not Trap) rowLog += "  ";
                else if (_global.MazeGenerator.Maze[j, i] is Wall) rowLog += "# ";
                else if (_global.MazeGenerator.Maze[j, i] is Spawner) rowLog += "S ";
                else if (_global.MazeGenerator.Maze[j, i] is Exit) rowLog += "E ";
                else if (_global.MazeGenerator.Maze[j, i] is Trap) rowLog += "T ";
            }
            GD.Print(rowLog);
        }
    }
}