using Godot;
using System;
using MazeRunner.Scripts.Data;

namespace MazeRunner.Scripts;

public partial class MainGame : Node2D
{
    private Global _global;
    private PlayerCamera _playerCamera;
    private Token _token;
    private SpikesTimer _spikesTimer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _global = GetNode<Global>("/root/Global");
        _playerCamera = GetNode<PlayerCamera>("/root/Game/MainGame/PlayerCamera");
        _token = GetNode<Token>("/root/Game/MainGame/Token");
        _spikesTimer = GetNode<SpikesTimer>("/root/Game/MainGame/Token/SpikesTimer");

        GD.Print("SpawnerCoord: " + _global.Setting.MazeGenerator.SpawnerCoord);
        GD.Print("ExitCoord: " + _global.Setting.MazeGenerator.ExitCoord);
        GD.Print("Size: " + _global.Setting.Size);
        GD.Print("Seed: " + _global.Setting.MazeGenerator.Seed);
        GDPrintMaze();
        GDPrintSkills();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // GD.Print("Floor: " + _token.CurrentFloor);
        // GD.Print("PlayerState: " + _token.CurrentState);
        // GD.Print("PlayerCondition: " + _token.CurrentCondition);
        // if (_token.CurrentCondition == Token.Condition.Spikes) GD.Print("TimeLeft for SpikeTrapped Condition: " + Math.Floor(_spikesTimer.TimeLeft + 1));
        // if (_token.CurrentCondition == Token.Condition.Sticky) GD.Print("_directionalKeysPressCount: " + _token._directionalKeysPressCount);
        // GD.Print("");
        for (int i = 0; i < _token.TokenSkillsBools.Length; i++)
        {
            if (_token.TokenSkillsBools[i])
            {
                GD.Print("ShieldHealth: " + _token.Shield.Health);
            }
        }
    }

    private void GDPrintMaze()
    {
        for (int i = 0; i < _global.Setting.MazeGenerator.Size; i++)
        {
            string rowLog = "";
            for (int j = 0; j < _global.Setting.MazeGenerator.Size; j++)
            {
                if (_global.Setting.MazeGenerator.Maze[j, i] is Empty) rowLog += "  ";
                else if (_global.Setting.MazeGenerator.Maze[j, i] is Wall) rowLog += "# ";
            }
            GD.Print(rowLog);
        }
    }

    private void GDPrintSkills()
    {
        for (int i = 0; i < _global.Setting.SkillBools.Length; i++)
        {
            GD.Print(_global.Setting.SkillBools[i]);
        }
        for (int i = 0; i < _token.TokenSkillsBools.Length; i++)
        {
            if (_token.TokenSkillsBools[i])
            {
                GD.Print(_token.TokenSkillsBools[i]);
            }
        }
    }
}