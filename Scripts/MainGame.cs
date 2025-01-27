using Godot;
using System;
using MazeRunner.Scripts.Data;

namespace MazeRunner.Scripts;

public partial class MainGame : Node2D
{
    [Export] private Camera2D _playerOneCamera;
    [Export] private Camera2D _playerTwoCamera;
    [Export] private Player _playerOne;
    [Export] private Player _playerTwo;
    [Export] private Board _board;

    private Global _global;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _global = GetNode<Global>("/root/Global");

        GD.Print("SpawnerCoord: " + _global.Setting.MazeGenerator.SpawnerCoord);
        GD.Print("ExitCoord: " + _global.Setting.MazeGenerator.ExitCoord);
        GD.Print("Size: " + _global.Setting.Size);
        GD.Print("Seed: " + _global.Setting.MazeGenerator.Seed);
        GDPrintSkills();
        GDPrintMaze();
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("SwitchCamera"))
        {
            SwitchCamera();
        }
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
        // for (int i = 0; i < _token.TokenSkillsBools.Length; i++)
        // {
        //     if (_token.TokenSkillsBools[i] && i == 0)
        //     {
        //         GD.Print("ShieldHealth: " + _token.Shield.Health);
        //     }
        // }
        // GD.Print("Token.IsBoostOn: " + _token._isBoostOn);
        // GD.Print("Token.ISBoostStillOn: " + _token._isBoolStillOn);
        // GD.Print("Token.Boost.Battery: " + _token.Boost.Battery);
        // if (_playerOne.IsBlindnessOn)
        // {
        //     GD.Print("PlayerOne is Blind.");
        // }
        // if (_playerTwo.IsBlindnessOn)
        // {
        //     GD.Print("PlayerTwo is Blind.");
        // }

        if (_playerOne.IsBlindOn) GD.Print("TimeLeft for PlayerOne Blindness: " + Math.Floor(_playerOne._blindnessTimer.TimeLeft + 1) + " & " + "Blindness.Battery: " + _playerOne.Blindness.Battery);
        if (_playerTwo.IsBlindOn) GD.Print("TimeLeft for PlayerTwo Blindness: " + Math.Floor(_playerTwo._blindnessTimer.TimeLeft + 1) + " & " + "Blindness.Battery: " + _playerOne.Blindness.Battery);
        
        // GD.Print("Board.PixelSize: " + _board.PixelSize);
        
        GD.Print("");
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
        // for (int i = 0; i < _global.Setting.SkillBools.Length; i++)
        // {
        //     GD.Print($"Setting.SkillBools[{i}]: " + _global.Setting.SkillBools[i]);
        // }
        for (int i = 0; i < _playerOne.PlayerSkillsBools.Length; i++)
        {
            GD.Print($"PlayerOne.PlayerSkillBools[{i}]: " + _playerOne.PlayerSkillsBools[i]);
        }
        for (int i = 0; i < _playerTwo.PlayerSkillsBools.Length; i++)
        {
            GD.Print($"PlayerTwo.PlayerSkillBools[{i}]: " + _playerTwo.PlayerSkillsBools[i]);
        }
    }

    public void SwitchCamera()
    {
        if (_playerOneCamera.IsCurrent())
        {
            _playerTwoCamera.MakeCurrent();
        }
        else if (_playerTwoCamera.IsCurrent())
        {
            _playerOneCamera.MakeCurrent();
        }
    }
}