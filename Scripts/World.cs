using Godot;
using MazeRunner.Scripts.Data;

namespace MazeRunner.Scripts;

public partial class World : Node2D
{
    [Export] private Player _playerOne;
    [Export] private Player _playerTwo;


    private Global _global;

    public override void _Ready()
    {
        _global = GetNode<Global>("/root/Global");
        _global.MazeGenerator.GenerateMaze();
    }

    public override void _Process(double delta)
    {
        if (_playerOne.CurrentState == Player.State.Winning)
        {
            _global.PlayerNameWon = _playerOne.StrName;
            _global.PlayerNameLost = _playerTwo.StrName;
            GetTree().ChangeSceneToFile("res://Scenes/game_over.tscn");
        }
        else if (_playerTwo.CurrentState == Player.State.Winning)
        {
            _global.PlayerNameWon = _playerTwo.StrName;
            _global.PlayerNameLost = _playerOne.StrName;
            GetTree().ChangeSceneToFile("res://Scenes/game_over.tscn");
        }
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