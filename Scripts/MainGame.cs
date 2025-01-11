using Godot;
using System;
using MazeRunner.Scripts.Data;

namespace MazeRunner.Scripts;

public partial class MainGame : Node2D
{
    private Global _global;
    private PlayerCamera _playerCamera;
    private Token _token;
    private SpikeTrappedTimer _spikeTrappedTimer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _global = GetNode<Global>("/root/Global");
        _playerCamera = GetNode<PlayerCamera>("/root/Game/MainGame/PlayerCamera");
        _token = GetNode<Token>("/root/Game/MainGame/Token");
        _spikeTrappedTimer = GetNode<SpikeTrappedTimer>("/root/Game/MainGame/Token/SpikeTrappedTimer");

        GD.Print("SpawnerCoords: " + _global.Setting.MazeGenerator.SpawnerCoord);
        GD.Print("Size: " + _global.Setting.Size);
        GD.Print("Seed: " + _global.Setting.MazeGenerator.Seed);
        GdPrintMaze();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        GD.Print("Floor: " + _token.CurrentFloor);
        GD.Print("PlayerState: " + _token.CurrentState);
        if (_token.CurrentCondition == Token.Condition.SpikeTrapped)
        {
            GD.Print("TimeLeft for SpikeTrapped Condition: " + Math.Floor(_spikeTrappedTimer.TimeLeft + 1));
        }
        GD.Print("");
    }

    private void GdPrintMaze()
    {
        for (int i = 0; i < _global.Setting.MazeGenerator.Size; i++)
        {
            string rowLog = "";
            for (int j = 0; j < _global.Setting.MazeGenerator.Size; j++)
            {
                if (_global.Setting.MazeGenerator.Maze[j, i] is Empty)
                {
                    rowLog += "  ";
                }
                else if (_global.Setting.MazeGenerator.Maze[j, i] is Wall)
                {
                    rowLog += "# ";
                }
            }
            GD.Print(rowLog);
        }
    }
}